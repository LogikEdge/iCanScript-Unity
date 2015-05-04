using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Engine;
using Prefs=iCanScript.Editor.PreferencesController;

namespace iCanScript.Editor {
    
    public class iCS_Reflection {
        // ======================================================================
        // Constants
        // ----------------------------------------------------------------------
        const string kUnityEnginePackage= "UnityEngine";
        const string kResourcesPath     = iCS_Config.ImagePath;
        const string kUnityIcon         = kResourcesPath+"/iCS_UnityLogo_32x32.png";

        // ======================================================================
        // Fileds.
        // ----------------------------------------------------------------------
        public static List<Type>    AllTypesWithDefaultConstructor= new List<Type>();
        public static bool          NeedToRunInstaller= false;
    
        // ----------------------------------------------------------------------
        /*
            TODO: Should remove all types if not used.
        */
        static void AddToAllTypes(Type type) {
            if(type == null || type.Name.Length <= 0) return;
            if(type.Name[0] != '<' && iCS_Types.CreateInstanceSupported(type)) {
                AllTypesWithDefaultConstructor.Add(type);
            }
        }
        // ----------------------------------------------------------------------
        public static Type[] GetAllTypesWithDefaultConstructorThatDeriveFrom(Type baseType) {
            List<Type> result= new List<Type>();
            foreach(var type in AllTypesWithDefaultConstructor) {
                if(iCS_Types.IsA(baseType, type)) result.Add(type);
            }
            return result.ToArray();
        }
    
        // ----------------------------------------------------------------------
        // Scan the application for uCode attributes.
        public static void ParseAppDomain() {
            // Type used for user installs
            Type installerType= null;
            // Remove all previously registered functions.
            iCS_LibraryDatabase.Clear();
            // Scan the application for functions/methods/conversions to register.
            var useUnityEditor= Prefs.UseUnityEditorLibrary;
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                // -- Install all Unity public types --
                string unityPackageName= null;
                var assemblyName= assembly.FullName;
                if(assemblyName.StartsWith("UnityEngine")) {
                    unityPackageName= "UnityEngine";
                }
                else if(useUnityEditor && assemblyName.StartsWith("UnityEditor")) {
                    unityPackageName= "UnityEditor";
                }
                if(unityPackageName != null) {
                    foreach(var classType in assembly.GetTypes()) {
                        if(classType.IsPublic && !classType.IsGenericType) {
                            DecodeUnityClassInfo(classType, unityPackageName);                        
                        }
                    }                
                    continue;
                }
                foreach(var classType in assembly.GetTypes()) {
                    AddToAllTypes(classType);
                    if(classType.Name == "iCS_Installer") {
                        installerType= classType;
                        continue;
                    }
                    bool extractClass= false;
                    iCS_ClassAttribute classAttribute= null;
                    foreach(var classCustomAttribute in classType.GetCustomAttributes(true)) {
                        // Don't include classes that have been marked with iCS_Exclude.
                        if(classCustomAttribute is iCS_ExcludeAttribute) {
                            extractClass= false;
                            break;
                        }
                        // Only register classes that have been tagged for iCanScript.
                        if(classCustomAttribute is iCS_ClassAttribute) {
                            extractClass= true;
                            // Validate that the class is public.
                            if(classType.IsPublic == false) {
                                Debug.LogWarning("iCanScript: Class "+classType+" is not public and tagged for "+iCS_Config.ProductName+".  Ignoring class !!!");
                                continue;
                            }
                            // Keep copy of class information.
                            classAttribute= classCustomAttribute as iCS_ClassAttribute;
                        }
                    }
                    // Only register classes that have been tagged for iCanScript.
                    if(extractClass) {
                        // Extract class information.
                        string  company        = classAttribute.Company;
                        string  library        = classAttribute.Library;
                        string  description    = classAttribute.Description;
                        string  icon           = classAttribute.Icon;
                        bool    baseVisibility = classAttribute.BaseVisibility;
    					bool	hideFromLibrary= classAttribute.HideClassFromLibrary;
                        DecodeClassInfo(classType, company, library, description, icon, false, baseVisibility, hideFromLibrary);
                    }
                }
            }
            // Invoke user installation.
            NeedToRunInstaller= true;
            // Run user installer
            if(installerType != null) {
                var installMethod= installerType.GetMethod("Install");
                if(installMethod != null) {
                    installMethod.Invoke(null, null);
                }
            }
            AllTypesWithDefaultConstructor.Sort((t1,t2)=>{ return String.Compare(t1.Name, t2.Name); });
        }
        // ======================================================================
        // The following are helper functions to register Unity3D classes
        // ----------------------------------------------------------------------
        // Use this function to register Unity3d classes.
        // All public fields/properties and methods will be registered.
        //
        // This function can be called by the iCanScript user to add to the
        // existing Unity library.
        // 
        public static void DecodeUnityClassInfo(Type classType, string package= "UnityEngine", string iconPath= null, string description= null) {
            string                  company               = "Unity";
            bool                    decodeAllPublicMembers= true;
            if(package == null)     package               = kUnityEnginePackage;
            if(description == null) description           = null;
            if(iconPath == null)    iconPath              = kUnityIcon;
            iCS_Reflection.DecodeClassInfo(classType, company, package, description, iconPath, decodeAllPublicMembers,true);
        }
        // ----------------------------------------------------------------------
        public static void DecodeClassInfo(Type classType, string company, string library, string description, string classIconPath,
                                           bool acceptAllPublic= false,
                                           bool baseVisibility = true,
    									   bool hideFromLibrary= false) {
            if(classType.IsGenericType) {
                Debug.LogWarning("iCanScript: Generic class not supported yet.  Skiping: "+classType.Name);
                return;
            }
            // Build TypeInfo.
            var baseType         = classType.BaseType;
            var declaringType    = classType.DeclaringType;
            var declaringTypeInfo= iCS_LibraryDatabase.GetTypeInfo(declaringType);
            var _classTypeInfo   = iCS_LibraryDatabase.AddTypeInfo(company, library, classType, baseType, declaringType, declaringTypeInfo,
                                                                   classType.Name, description, classIconPath, hideFromLibrary);
            // Decode class members
            DecodeConstructors(_classTypeInfo, acceptAllPublic);
            DecodeClassFields(_classTypeInfo, acceptAllPublic, baseVisibility);
            DecodeFunctionsAndMethods(_classTypeInfo, acceptAllPublic, baseVisibility);
        }
    	// ======================================================================
    	// Decode Constructors
        // ----------------------------------------------------------------------
        static void DecodeConstructors(iCS_TypeInfo _classTypeInfo, bool acceptAllPublic= false) {
            foreach(var constructor in _classTypeInfo.CompilerType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                bool registerMethod= false;
                string displayName= iCS_Types.TypeName(_classTypeInfo.CompilerType);
                string description= "";
                string iconPath= _classTypeInfo.IconPath;
                foreach(var constructorAttribute in constructor.GetCustomAttributes(true)) {
                    if(constructorAttribute is iCS_FunctionAttribute) {                                    
                        if(constructor.IsPublic) {
                            registerMethod= true;
                            // Register execution functions/methods.
                            iCS_FunctionAttribute funcAttr= constructorAttribute as iCS_FunctionAttribute;
                            if(funcAttr.Name        != null) displayName= funcAttr.Name; 
                            if(funcAttr.Description != null) description= funcAttr.Description;
                            if(funcAttr.Icon        != null) iconPath   = funcAttr.Icon;
                        } else {
                            Debug.LogWarning("iCanScript: Constrcutor of class "+_classTypeInfo.DisplayName+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring constructor !!!");                        
                        }
                        break;                                        
                    }
                }
                if(acceptAllPublic && constructor.IsPublic) {
                    registerMethod= true;
                }
                if(registerMethod) {
                    if(constructor.IsGenericMethod) {
                        Debug.LogWarning("iCanScript: Generic method not yet supported.  Skiping constrcutor from class "+_classTypeInfo.DisplayName);
                        continue;
                    }
                    DecodeConstructor(_classTypeInfo, "new "+displayName, description, iconPath, constructor);
                }
            }                               
        }
        // ----------------------------------------------------------------------
        static void DecodeConstructor(iCS_TypeInfo _classTypeInfo, string displayName, string description, string iconPath, ConstructorInfo constructor) {
            // Parse parameters.
            if(!AreAllParamTypesSupported(constructor)) return;
            var parameters= ParseParameters(constructor);
        
            iCS_LibraryDatabase.AddConstructor(_classTypeInfo, displayName, description, iconPath,
                                        	   parameters, constructor);
        }
        // ----------------------------------------------------------------------
        static void DecodeClassFields(iCS_TypeInfo _classTypeInfo,
                                      bool acceptAllPublic= false,
                                      bool baseVisibility= true) {
            // Gather field information.
            var classType= _classTypeInfo.CompilerType;
            foreach(var field in classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
                bool registerField= false;
                iCS_ParamDirection direction= iCS_ParamDirection.InOut;
                foreach(var fieldAttr in field.GetCustomAttributes(true)) {
                    if(!field.IsPublic && (fieldAttr is iCS_InPortAttribute || fieldAttr is iCS_OutPortAttribute || fieldAttr is iCS_InOutPortAttribute)) {
                        Debug.LogWarning("iCanScript: Field "+field.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring field !!!");
                        continue;
                    }
                    if(fieldAttr is iCS_InPortAttribute) {
                        direction= iCS_ParamDirection.In;
                        registerField= true;
                    }
                    if(fieldAttr is iCS_OutPortAttribute) {
                        direction= iCS_ParamDirection.Out;
                        registerField= true;
                    }
                    if(fieldAttr is iCS_InOutPortAttribute) {
                        direction= iCS_ParamDirection.InOut;
                        registerField= true;
                    }
                }
                if(registerField == false && field.IsPublic) {
                    var declaringType= field.DeclaringType;
                    bool isDefinedInBase= declaringType != classType;
                    if(baseVisibility == true || !isDefinedInBase) {
                        if(acceptAllPublic) {
                            registerField= true;
                            direction= iCS_ParamDirection.InOut;
                        } else if(isDefinedInBase) {
                            // Don't override iCS attributes
                            bool isTagged= false;
                            foreach(var baseAttr in declaringType.GetCustomAttributes(true)) {
                                if(baseAttr is iCS_ClassAttribute) {
                                    isTagged= true;
                                    break;
                                }
                            }
                            // Add base fields if class is not using iCS attributes.
                            if(isTagged == false) {
                                registerField= true;
                                direction= iCS_ParamDirection.InOut;                            
                            }
                        }                
                    }
                }
                if(registerField) {
                    if(field.IsStatic) {
                        DecodeStaticField(_classTypeInfo, field, direction);
                    } else {
                        DecodeInstanceField(_classTypeInfo, field, direction);
                    }                
                }
            }        
        }
        // ----------------------------------------------------------------------
        static void DecodeStaticField(iCS_TypeInfo _classTypeInfo, FieldInfo field, iCS_ParamDirection dir) {
            string description= "";
            string iconPath= "";
            if((dir == iCS_ParamDirection.In || dir == iCS_ParamDirection.InOut) && !field.IsInitOnly) {
                var parameters            = new iCS_Parameter[1];
                parameters[0]             = new iCS_Parameter();
                parameters[0].name        = field.Name;
                parameters[0].type        = field.FieldType;
                parameters[0].direction   = iCS_ParamDirection.In;
                parameters[0].initialValue= iCS_Types.DefaultValue(field.FieldType);
                iCS_LibraryDatabase.AddField(_classTypeInfo, "set_"+field.Name, description, iconPath,
                                             parameters, null,
                                             iCS_StorageClass.Class, iCS_AccessorType.Set, field);                    
            }
            if(dir == iCS_ParamDirection.Out || dir == iCS_ParamDirection.InOut) {
                var returnType= new iCS_FunctionReturn(field.Name, field.FieldType);
                iCS_LibraryDatabase.AddField(_classTypeInfo, "get_"+field.Name, description, iconPath,
                                             new iCS_Parameter[0], returnType,
                                             iCS_StorageClass.Class, iCS_AccessorType.Get, field);                    
            }
        }
        // ----------------------------------------------------------------------
        static void DecodeInstanceField(iCS_TypeInfo _classTypeInfo, FieldInfo field, iCS_ParamDirection dir) {
            string description= "";
            string iconPath= "";
            if((dir == iCS_ParamDirection.In || dir == iCS_ParamDirection.InOut) && !field.IsInitOnly) {
                var parameters        = new iCS_Parameter[1];
                parameters[0]         = new iCS_Parameter();
                parameters[0].name        = field.Name;
                parameters[0].type        = field.FieldType;
                parameters[0].direction   = iCS_ParamDirection.In;
                parameters[0].initialValue= iCS_Types.DefaultValue(field.FieldType);
                iCS_LibraryDatabase.AddField(_classTypeInfo, "set_"+field.Name, description, iconPath,
                                             parameters, null,
                                             iCS_StorageClass.Instance, iCS_AccessorType.Set, field);                    
            }
            if(dir == iCS_ParamDirection.Out || dir == iCS_ParamDirection.InOut) {
                var returnType= new iCS_FunctionReturn(field.Name, field.FieldType);
                iCS_LibraryDatabase.AddField(_classTypeInfo, "get_"+field.Name, description, iconPath,
                                             new iCS_Parameter[0], returnType,
                                             iCS_StorageClass.Instance, iCS_AccessorType.Get, field);                    
            }
        }
        // ----------------------------------------------------------------------
        static void DecodeFunctionsAndMethods(iCS_TypeInfo _classTypeInfo,
                                              bool acceptAllPublic= false,
                                              bool baseVisibility= true) {
            var classType= _classTypeInfo.CompilerType;
            foreach(var method in classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
                bool registerMethod= false;
                string displayName= method.Name;
                string returnName= null;
                string classDescription= _classTypeInfo.Description;
                string description= classDescription;
                string iconPath= "";
                foreach(var methodAttribute in method.GetCustomAttributes(true)) {
                    if(methodAttribute is iCS_TypeCastAttribute) {
                        if(method.IsPublic) {
                            iCS_TypeCastAttribute typeCastAttr= methodAttribute as iCS_TypeCastAttribute;
                            if(typeCastAttr.Icon != null) iconPath= typeCastAttr.Icon;
                            DecodeTypeCast(_classTypeInfo, iconPath, method);
                        } else {                        
                            Debug.LogWarning("iCanScript: Type Cast "+method.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring function !!!");
                        }
                        break;
                    }
                    else if(methodAttribute is iCS_FunctionAttribute) {                                    
                        if(method.IsPublic) {
                            registerMethod= true;
                            // Register execution functions/methods.
                            iCS_FunctionAttribute funcAttr= methodAttribute as iCS_FunctionAttribute;
                            if(funcAttr.Name        != null) displayName= funcAttr.Name; 
                            if(funcAttr.Return      != null) returnName = funcAttr.Return;
                            if(funcAttr.Description != null) description= funcAttr.Description;
                            if(funcAttr.Icon        != null) iconPath   = funcAttr.Icon;
    						if(funcAttr.Obsolete    != null) iCS_LibraryDatabase.AddObsolete(method, funcAttr.Obsolete);
                        } else {
                            Debug.LogWarning("iCanScript: Function "+method.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring function !!!");                        
                        }
                        break;                                        
                    }
                }
                if(registerMethod == false && method.IsPublic) {
                    var declaringType= method.DeclaringType;
                    bool isDefinedInBase= declaringType != classType;
                    if(baseVisibility == true || !isDefinedInBase) {
                        if(acceptAllPublic) {
                            registerMethod= true;
                        } else {
                            if(isDefinedInBase) {
                                // Don't override iCS attributes
                                bool isTagged= false;
                                foreach(var baseAttr in declaringType.GetCustomAttributes(true)) {
                                    if(baseAttr is iCS_ClassAttribute) {
                                        isTagged= true;
                                        break;
                                    }
                                }
                                // Add base fields if class is not using iCS attributes.
                                if(isTagged == false) {
                                    registerMethod= true;
                                }
                            }
                        }                    
                    }
                }
                if(registerMethod) {
                    if(method.IsGenericMethod) {
    //                    Debug.LogWarning("iCanScript: Generic method not yet supported.  Skiping "+method.Name+" from class "+className);
                        continue;
                    }
    				displayName= NameUtility.ConvertRuntimeNameToDisplayName(displayName);
                    if(method.IsStatic) {
                        DecodeStaticMethod(_classTypeInfo, displayName, description, iconPath, returnName, method);
                    } else {
                        DecodeInstanceFunction(_classTypeInfo, displayName, description, iconPath, returnName, method);
                    }
                }
            }                               
        }
        // ----------------------------------------------------------------------
        static void DecodeTypeCast(iCS_TypeInfo _classTypeInfo, string iconPath, MethodInfo method) {
            var classType= _classTypeInfo.CompilerType;
            Type toType= method.ReturnType;
            ParameterInfo[] parameters= method.GetParameters();
            if(parameters.Length != 1 || toType == null) {
                Debug.LogWarning("iCanScript: Conversion function must have one return type and one parameter. Ignoring conversion function in "+classType.Name);
                return;
            }
            Type fromType= parameters[0].ParameterType;
            if(method.IsPublic == false) {
                Debug.LogWarning("iCanScript: Conversion from "+fromType+" to "+toType+" in class "+classType.Name+
                                 " is not public and tagged for "+iCS_Config.ProductName+". Ignoring conversion !!!");
                return;                                        
            }
            if(method.IsStatic == false) {
                Debug.LogWarning("iCanScript: Conversion from "+fromType+" to "+toType+" in class "+classType.Name+
                                 " is not static and tagged for "+iCS_Config.ProductName+". Ignoring conversion !!!");
                return;                                        
            }
            iCS_LibraryDatabase.AddTypeCast(_classTypeInfo, iconPath, fromType, method);                                        
        }
        // ----------------------------------------------------------------------
        static void DecodeInstanceFunction(iCS_TypeInfo _classTypeInfo,
                                           string displayName, string description, string iconPath,
                                           string retName, MethodInfo method) {
            // Parse parameters.
            if(!AreAllParamTypesSupported(method)) return; 
            var parameters= ParseParameters(method);
            var returnType= new iCS_FunctionReturn(retName, method.ReturnType);       

            // Register a property if the method has only one input or one output.
            bool isProperty= false;
            iCS_AccessorType accessor= iCS_AccessorType.None;
            if(parameters.Length == 0 && returnType.type != typeof(void) && displayName.StartsWith("get_")) {
                isProperty= true;
                accessor= iCS_AccessorType.Get;
            }
            if(parameters.Length == 1 && returnType.type == typeof(void) && displayName.StartsWith("set_")) {
                isProperty= true;
                accessor= iCS_AccessorType.Set;
                parameters[0].name= ObjectNames.NicifyVariableName(displayName.Substring(4));
            }
            if(isProperty) {
    			returnType.name= displayName.Substring(4);
                iCS_LibraryDatabase.AddProperty(_classTypeInfo,
                                                displayName, description, iconPath,
                                                parameters, returnType,
                                                iCS_StorageClass.Instance, accessor,
                                                method);
            } else {
    //            if(displayName.StartsWith("get_") || displayName.StartsWith("set_")) {
    //                Debug.Log("Method: "+displayName+" #p=> "+parameters.Length+" return=> "+returnType.type);
    //            }
                iCS_LibraryDatabase.AddMethod(_classTypeInfo, displayName, description, iconPath,
                                              parameters, returnType,
                                              iCS_StorageClass.Instance, method);            
            }
        }
        // ----------------------------------------------------------------------
        static void DecodeStaticMethod(iCS_TypeInfo _classTypeInfo,
                                       string displayName, string description, string iconPath,
                                       string retName, MethodInfo method) {
            // Parse parameters.
            if(!AreAllParamTypesSupported(method)) return;
            var parameters= ParseParameters(method);       
            var returnType= new iCS_FunctionReturn(retName, method.ReturnType);       

            // Register a property if the method has only one input or one output.
            bool isProperty= false;
            iCS_AccessorType accessor= iCS_AccessorType.None;
            if(parameters.Length == 0 && returnType.type != typeof(void) && displayName.StartsWith("get_")) {
                isProperty= true;
                accessor= iCS_AccessorType.Get;
            }
            if(parameters.Length == 1 && returnType.type == typeof(void) && displayName.StartsWith("set_")) {
                isProperty= true;
                accessor= iCS_AccessorType.Set;
            }
            if(isProperty) {
                iCS_LibraryDatabase.AddProperty(_classTypeInfo,
                                                displayName, description, iconPath,
                                                parameters, returnType,
                                                iCS_StorageClass.Class, accessor,
                                                method);            
            } else {
                // Register standard method.
    //            if(displayName.StartsWith("get_") || displayName.StartsWith("set_")) {
    //                Debug.Log("Function: "+displayName+" #p=> "+parameters.Length+" return=> "+returnType.type);
    //            }
                iCS_LibraryDatabase.AddMethod(_classTypeInfo,
                                              displayName, description, iconPath,
                                              parameters, returnType,
                                              iCS_StorageClass.Class, method);
            }
        }

   
        // ======================================================================
        // ----------------------------------------------------------------------
        static iCS_Parameter[] ParseParameters(MethodBase method) {
            ParameterInfo[] paramInfo= method.GetParameters();
            iCS_Parameter[] parameters= new iCS_Parameter[paramInfo.Length];
            if(paramInfo.Length == 0) return parameters;
            for(int i= 0; i < paramInfo.Length; ++i) {
                var p= paramInfo[i];
                parameters[i]= new iCS_Parameter();
                parameters[i].name= p.Name;
                parameters[i].type= p.ParameterType;
                // IMPROVE: Mono seems broken for ParameterInfo.IsIn. So we use !IsOut as IsIn. That prevents InOut parameters...
                parameters[i].direction= p.IsOut ? (p.IsIn ? iCS_ParamDirection.InOut : iCS_ParamDirection.Out) : iCS_ParamDirection.In;
                object defaultValue= p.DefaultValue; 
                parameters[i].initialValue= (defaultValue == null || defaultValue.GetType() != p.ParameterType) ? null : defaultValue;
            }
            return parameters;   
        }
        // ----------------------------------------------------------------------
        static bool AreAllParamTypesSupported(MethodBase method) {
            foreach(var paramInfo in method.GetParameters()) {
                if(paramInfo.ParameterType.IsPointer) return false;
            }
            return true;
        }
    }

}
