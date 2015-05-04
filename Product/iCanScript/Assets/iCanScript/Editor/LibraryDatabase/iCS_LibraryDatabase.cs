using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using iCanScript.Editor;
using P=iCanScript.Prelude;

public class iCS_LibraryDatabase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<iCS_TypeInfo>        	types    = new List<iCS_TypeInfo>();
    public static List<iCS_FunctionPrototype>	Functions= new List<iCS_FunctionPrototype>();
	public static Dictionary<MemberInfo,string> obsoletes= new Dictionary<MemberInfo,string>();
    
    // ======================================================================
    // Utility Type
    // ----------------------------------------------------------------------
	class FunctionComparer : IComparer<iCS_FunctionPrototype> {
		public int Compare(iCS_FunctionPrototype x, iCS_FunctionPrototype y) {
			return CompareFunctionNames(x, y);
		}
	};
	static FunctionComparer	myFunctionComparer= new FunctionComparer();
	
    // ======================================================================
    // DataBase functionality
    // ----------------------------------------------------------------------
    // Returns 0 if equal, negative if first is smaller and
    // positive if first is greather.
    public static int CompareFunctionNames(iCS_FunctionPrototype d1, iCS_FunctionPrototype d2) {
        if(d1.Company == null && d2.Company != null) return -1;
        if(d1.Company != null && d2.Company == null) return 1;
        int result;
        if(d1.Company != null) {
            result= d1.Company.CompareTo(d2.Company);
            if(result != 0) return result;
        }
        if(d1.Library == null && d2.Library != null) return -1;
        if(d1.Library != null && d2.Library == null) return 1;
        if(d1.Library != null) {
            result= d1.Library.CompareTo(d2.Library);
            if(result != 0) return result;            
        }
        result= iCS_Types.TypeName(d1.ClassType).CompareTo(iCS_Types.TypeName(d2.ClassType));
        if(result != 0) return result;
        result= d1.DisplayName.CompareTo(d2.DisplayName);
		if(result != 0) return result;
		result= d1.InterfaceTypesAsString().CompareTo(d2.InterfaceTypesAsString());
		if(result != 0) return result;
		if(d1.IsMethod && d2.IsMethod) {
			var m1= d1.ToMethodInfo;
			var m2= d2.ToMethodInfo;
			return iCS_Types.TypeName(m1.DeclaringType).CompareTo(iCS_Types.TypeName(m2.DeclaringType));
		}
		return 0;
	}
    // ----------------------------------------------------------------------
    public static List<iCS_FunctionPrototype> BuildExpertMenu() {
        return AllFunctions();
    }
    // ----------------------------------------------------------------------
    public static List<iCS_FunctionPrototype> AllFunctions() {
        return Functions;
    }
    // ----------------------------------------------------------------------
    public static List<iCS_FunctionPrototype> AllFunctionsUnsorted() {
        return Functions;
    }
    // ----------------------------------------------------------------------
    public static List<iCS_FunctionPrototype> BuildNormalMenu() {
        var menu= new List<iCS_FunctionPrototype>();
        foreach(var desc in Functions) {
            Type classType= desc.ClassType;
            if(iCS_Types.IsStaticClass(classType)) {
                menu.Add(desc);
            } else {
                bool found= false;
                foreach(var existing in menu) {
                    if(classType == existing.ClassType) {
                        found= true;
                        break;
                    }
                }
                if(!found) {
                    menu.Add(desc);
                }                
            }
        }
        return menu;        
    }
    // ----------------------------------------------------------------------
    public static iCS_TypeInfo GetTypeInfo(Type compilerType) {
        foreach(var t in types) {
            if(t.CompilerType == compilerType) {
                return t;
            }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns all components of the given class.
    public static iCS_MemberInfo[] GetMembers(Type compilerType) {
		var typeInfo= GetTypeInfo(compilerType);
		if(typeInfo == null) {
			return new iCS_MemberInfo[0];
		}
		return typeInfo.Members.ToArray();
    }
    // ----------------------------------------------------------------------
	public static iCS_ConstructorInfo[] GetConstructors(Type compilerType) {
	    var constructors= new List<iCS_ConstructorInfo>();
        foreach(var c in GetMembers(compilerType)) {
            if(c.IsConstructor) {
                constructors.Add(c.ToConstructorInfo);
            }
        }
        return constructors.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_FieldInfo[] GetFields(Type compilerType) {
	    var fields= new List<iCS_FieldInfo>();
	    foreach(var f in GetMembers(compilerType)) {
	        if(f.IsField) {
	            fields.Add(f.ToFieldInfo);
	        }
	    }
        return fields.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_PropertyInfo[] GetProperties(Type compilerType) {
	    var properties= new List<iCS_PropertyInfo>();
	    foreach(var p in GetMembers(compilerType)) {
            if(p.IsProperty) {
                properties.Add(p.ToPropertyInfo);
            }
        }
	    return properties.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_FunctionPrototype[] GetPropertiesAndFields(Type compilerType) {
	    var variables= new List<iCS_FunctionPrototype>();
	    foreach(var v in GetMembers(compilerType)) {
	        if(v.IsField || v.IsProperty) {
	            variables.Add(v.ToFunctionPrototypeInfo);
	        }
	    }
	    return variables.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_MethodInfo[] GetMethods(Type compilerType) {
        var methods= new List<iCS_MethodInfo>();
        foreach(var m in GetMembers(compilerType)) {
            if(m.IsMethod && !m.IsConstructor && !m.IsProperty) {
                methods.Add(m.ToMethodInfo);
            }
        }
        return methods.ToArray();
	}
    // ----------------------------------------------------------------------
    public static iCS_MessageInfo[] GetMessages(Type compilerType) {
        var messages= new List<iCS_MessageInfo>();
        foreach(var m in GetMembers(compilerType)) {
            if(m.IsMessage) {
                messages.Add(m.ToMessageInfo);
            }
        }
        P.sort(messages, (a,b)=> { return string.Compare(a.DisplayName, b.DisplayName); });
        return messages.ToArray();
    }
    // ----------------------------------------------------------------------
    public static bool IsInherited(iCS_MemberInfo memberInfo) {
        var methodInfo= memberInfo.ToMethodInfo;
        if(methodInfo != null) {
            return methodInfo.Method.DeclaringType != methodInfo.ClassType;
        }
        var fieldInfo= memberInfo.ToFieldInfo;
        if(fieldInfo != null) {
            return fieldInfo.Field.DeclaringType != fieldInfo.ClassType;            
        }
        var messageInfo= memberInfo.ToMessageInfo;
        if(messageInfo != null) {
            var parentTypeInfo= messageInfo.ParentTypeInfo;
            var baseType= parentTypeInfo.BaseType;
            foreach(var m in GetMembers(baseType)) {
                if(m.IsMessage) {
                    if(m.DisplayName == messageInfo.DisplayName) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    // ----------------------------------------------------------------------
    public static List<iCS_FunctionPrototype> BuildMenuForMembersOfType(Type classType, Type inputType, Type outputType) {
        var menu= new List<iCS_FunctionPrototype>();
		classType= iCS_Types.GetElementType(classType);
		inputType= iCS_Types.GetElementType(inputType);
		outputType= iCS_Types.GetElementType(outputType);
        for(int i= 0; i < Functions.Count; ++i) {
            var func= Functions[i];
            if(func.ClassType == classType) {
                switch(func.ObjectType) {
					case iCS_ObjectTypeEnum.StaticFunction:
					case iCS_ObjectTypeEnum.StaticField:
                    case iCS_ObjectTypeEnum.InstanceFunction:
                    case iCS_ObjectTypeEnum.InstanceField: {
			            bool shouldInclude= false;
			            if(inputType != null) {
			                for(int j= 0; !shouldInclude && j < P.length(func.Parameters); ++j) {
			                    var param= func.Parameters[j];
			                    if(param.direction != iCS_ParamDirection.Out) {
									if(param.type == inputType) {
			//                        if(iCS_Types.IsA(func.ParamTypes[j], inputType)) {
			                            shouldInclude= true;
			                        }
			                    }
			                }
						}
			            if(!shouldInclude && outputType != null) {
			                if(func.ReturnType == outputType) shouldInclude= true;
			                for(int j= 0; !shouldInclude && j < P.length(func.Parameters); ++j) {
			                    var param= func.Parameters[j];
			                    if(param.direction != iCS_ParamDirection.In) {
			                        if(outputType == param.type) {
			//                        if(iCS_Types.IsA(outputType, func.ParamTypes[j])) {
			                            shouldInclude= true;
			                        }
			                    }
			                }
						}
						if(shouldInclude) {
			                menu.Add(func);							
						}
                        break;
                    }
                }
            }
		}
		return menu;
	}
    // ----------------------------------------------------------------------
    public static List<iCS_FunctionPrototype> BuildMenu(Type inputType, Type outputType) {
        var menu= new List<iCS_FunctionPrototype>();
		inputType= iCS_Types.GetElementType(inputType);
        for(int i= 0; i < Functions.Count; ++i) {
            // Filter functions according to input or output filter.
            bool shouldInclude= false;
            var func= Functions[i];
            if(inputType != null && !func.IsConstructor) {
                if(func.ClassType == inputType) {
                    switch(func.ObjectType) {
                        case iCS_ObjectTypeEnum.InstanceFunction:
                        case iCS_ObjectTypeEnum.InstanceField: {
                            shouldInclude= true;
                            break;
                        }
                    }
                }
                for(int j= 0; !shouldInclude && j < P.length(func.Parameters); ++j) {
                    var param= func.Parameters[j];
                    if(param.direction != iCS_ParamDirection.Out) {
						if(param.type == inputType) {
//                        if(iCS_Types.IsA(func.ParamTypes[j], inputType)) {
                            shouldInclude= true;
                        }
                    }
                }
            }
            if(!shouldInclude && outputType != null) {
                if(func.ClassType == outputType) {
                    switch(func.ObjectType) {
                        case iCS_ObjectTypeEnum.Constructor:
                        case iCS_ObjectTypeEnum.InstanceFunction:
                        case iCS_ObjectTypeEnum.InstanceField: {
                            shouldInclude= true;
                            break;
                        }
                    }
                }
                if(func.ReturnType == outputType) shouldInclude= true;
                for(int j= 0; !shouldInclude && j < P.length(func.Parameters); ++j) {
                    var param= func.Parameters[j];
                    if(param.direction != iCS_ParamDirection.In) {
                        if(outputType == param.type) {
//                        if(iCS_Types.IsA(outputType, func.ParamTypes[j])) {
                            shouldInclude= true;
                        }
                    }
                }
            }
            if(shouldInclude) {
                menu.Add(func);
            }
        }
        return menu;
    }
    // ----------------------------------------------------------------------
    // Returns the descriptor associated with the given company/package/function.
    public static iCS_MemberInfo GetDescriptor(string pathAndSignature) {
        foreach(var desc in Functions) {
            if(desc.ToString() == pathAndSignature) return desc;
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns the descriptor associated with the given editor object.
    public static iCS_MemberInfo GetAssociatedDescriptor(iCS_EditorObject edObj) {
		
		if(edObj==null) {
			return null;
		}
		
        if( edObj.IsPort || (edObj.IsPackage && !edObj.IsInstanceNode)) {
            return null;
		}
		
        var runtimeType= edObj.RuntimeType;
        var engineObject= edObj.EngineObject;
        int numberOfFunctionOutputPorts= edObj.NumberOfFunctionOutputPorts();
        foreach(var t in types) {

            if(t.CompilerType == runtimeType) {
                foreach(var member in t.Members) {		
					if(edObj.IsInstanceNode) {
						return member.ParentTypeInfo;
					}
					if(edObj.IsEventHandler) {
						var memberInfo= member as iCS_MemberInfo;
						if(engineObject.RawName == memberInfo.DisplayName) {
							return member;
						}
					}
                    if(member is iCS_MethodInfo) {
                        var methodInfo= member as iCS_MethodInfo;
                        if(edObj.MethodName != methodInfo.MethodName) continue;
                        if(edObj.NbOfParams != methodInfo.Parameters.Length) continue;
                        bool isFound= true;
                        edObj.ForEachChildPort(
                            p=> {
                                if(p.IsParameterPort && !p.IsFloating) {
                                    if(p.RuntimeType != methodInfo.Parameters[p.PortIndex].type) {
                                        isFound= false;
                                    }
                                }
                            }
                        );
                        if(isFound) {
                            return member;
                        }
                    }
                    if(member is iCS_FieldInfo && edObj.IsField) {
                        var fieldInfo= member as iCS_FieldInfo;
                        var objFieldInfo= engineObject.GetFieldInfoNoWarning();
                        if(objFieldInfo.Name == fieldInfo.Field.Name) {
                            if(numberOfFunctionOutputPorts == 0 && fieldInfo.IsSet) {
                                return member;                                                            
                            }
                            if(numberOfFunctionOutputPorts == 1 && fieldInfo.IsGet) {
                                return member;
                            }
                        }
                    }
                }
            }
        }
        return null;        
    }
	
    // ----------------------------------------------------------------------
    // Returns the class type associated with the given company/package.
    public static Type GetClassType(string classPath) {
        foreach(var desc in Functions) {
            if(desc.FunctionPath == classPath) return desc.ClassType;
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Finds a conversion that matches the given from/to types.
    public static iCS_TypeCastInfo FindTypeCast(Type fromType, Type toType) {
        foreach(var t in types) {
			var cast= FindTypeCast(t, fromType, toType);
			if(cast != null) {
				return cast;
			}
        }
        return null;
    }
    public static iCS_TypeCastInfo FindTypeCast(iCS_TypeInfo typeInfo, Type fromType, Type toType) {
		foreach(var m in typeInfo.Members) {
	        if(m.IsTypeCast) {
	            var typeCast= m.ToTypeCastInfo;
	            if(iCS_Types.CanBeConnectedWithoutConversion(fromType, typeCast.Parameters[0].type) &&
	               iCS_Types.CanBeConnectedWithoutConversion(typeCast.ReturnType, toType)) {
					return typeCast;
				}
	        }
			if(m.IsTypeInfo) {
				var cast= FindTypeCast(m.ToTypeInfo, fromType, toType);
				if(cast != null) {
					return cast;
				}
			}
		}
		return null;
    }
    // ----------------------------------------------------------------------
	/// Find an obsolete message for the given MemberInfo.
	public static string GetObsoleteMessage(MemberInfo memberInfo) {
		string message= null;
		obsoletes.TryGetValue(memberInfo, out message);
		return message;
	}

    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        types.Clear();
        Functions.Clear();
    }
    // ----------------------------------------------------------------------
    public static iCS_TypeInfo AddTypeInfo(string company, string package, Type compilerType, Type baseType, Type declaringType,
                                           iCS_TypeInfo declaringTypeInfo, string displayName, string iconPath, string description,
										   bool hideFromLibrary) {
        var typeInfo= GetTypeInfo(compilerType);
        if(typeInfo != null) {
            return typeInfo;
        }
        typeInfo= new iCS_TypeInfo(company, package, compilerType, baseType, declaringType, declaringTypeInfo, displayName, iconPath, description, hideFromLibrary);
        types.Add(typeInfo);
        // Extract all messages configured on base class.
        if(baseType != null && baseType != typeof(void)) {
            var baseTypeInfo= GetTypeInfo(baseType);
            if(baseTypeInfo != null) {
                foreach(var m in baseTypeInfo.Members) {
                    if(m.IsMessage) {
                        var msgInfo= m.ToMessageInfo;
                        var record= new iCS_MessageInfo(typeInfo, msgInfo.DisplayName, msgInfo.Description, msgInfo.IconPath,
                                                        msgInfo.Parameters, msgInfo.FunctionReturn, msgInfo.StorageClass);
                        AddDataBaseRecord(record);
                    }
                }                
            }
        }
        return typeInfo;
    }
    // ----------------------------------------------------------------------
    public static void AddConstructor(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                      iCS_Parameter[] parameters, ConstructorInfo constructorInfo) {
        var record= new iCS_ConstructorInfo(classInfo, displayName, description, iconPath,
                                            parameters, constructorInfo);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddField(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                iCS_StorageClass storageClass, iCS_AccessorType accessorType,
                                FieldInfo fieldInfo) {
        var objectType= storageClass == iCS_StorageClass.Instance ?
                            iCS_ObjectTypeEnum.InstanceField :
                            iCS_ObjectTypeEnum.StaticField;
        var record= new iCS_FieldInfo(objectType, classInfo, displayName, description, iconPath,
                                      parameters, functionReturn,
                                      storageClass, accessorType, fieldInfo);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddMethod(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                 iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                 iCS_StorageClass storageClass, MethodInfo methodInfo) {
        var objectType= storageClass == iCS_StorageClass.Instance ?
                            iCS_ObjectTypeEnum.InstanceFunction :
                            iCS_ObjectTypeEnum.StaticFunction;
        var record= new iCS_MethodInfo(objectType, classInfo,
                                       ImproveDisplayName(displayName), description, iconPath,
            						   parameters, functionReturn,
            						   storageClass, methodInfo);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddProperty(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                   iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                   iCS_StorageClass storageClass, iCS_AccessorType accessorType,
                                   MethodInfo methodInfo) {
        var objectType= storageClass == iCS_StorageClass.Instance ?
                            iCS_ObjectTypeEnum.InstanceFunction :
                            iCS_ObjectTypeEnum.StaticFunction;
        var record= new iCS_PropertyInfo(objectType, classInfo,
                                         displayName, description, iconPath,
            						     parameters, functionReturn, storageClass, accessorType,
            						     methodInfo);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddTypeCast(iCS_TypeInfo classInfo, string iconPath, Type fromType, MethodInfo methodInfo) {
        // Don't accept automatic conversion if it already exist.
        Type toType= methodInfo.ReturnType;
        foreach(var desc in Functions) {
            if(desc.IsTypeCast) {
                var typeCast= desc.ToTypeCastInfo;
                if(typeCast.Parameters[0].type == fromType && typeCast.ReturnType == toType) {
                    Debug.LogWarning("Duplicate type cast from "+fromType+" to "+toType+" exists in classes "+typeCast.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                    return;
                }                
            }
        }
        string fromTypeName= iCS_Types.TypeName(fromType);
        string toTypeName= iCS_Types.TypeName(toType);
        string toTypeNameUpper= Char.ToUpper(toTypeName[0])+toTypeName.Substring(1);

        var parameters= new iCS_Parameter[1];
        parameters[0]= new iCS_Parameter();
        parameters[0].name= fromTypeName;
        parameters[0].type= fromType;
        parameters[0].direction= iCS_ParamDirection.In;
        parameters[0].initialValue= iCS_Types.DefaultValue(fromType);
        var record= new iCS_TypeCastInfo(classInfo, "To"+toTypeNameUpper, "Converts from "+fromTypeName+" to "+toTypeName, iconPath,
                                         parameters, new iCS_FunctionReturn(toTypeName, toType),
                                         methodInfo);        
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds a message on a class
    public static void AddMessage(Type classType, string messageName, iCS_StorageClass storageClass,
                                  iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                  string description, string iconPath) {
        // Add event to given type.
        var typeInfo= iCS_LibraryDatabase.GetTypeInfo(classType);
        var record= new iCS_MessageInfo(typeInfo, messageName, description, iconPath,
                                        parameters, functionReturn, storageClass);
        AddDataBaseRecord(record);
        // Also add message to all deriving classes.
        foreach(var t in types) {
            if(t.BaseType == classType) {
                AddMessage(t.CompilerType, messageName, storageClass, parameters, functionReturn, description, iconPath);
            }
        }
    }
    // ----------------------------------------------------------------------
    // Adds a new database record.
    public static void AddDataBaseRecord(iCS_MemberInfo record) {
        if(record.IsFunctionPrototype) {
			var function= record.ToFunctionPrototypeInfo;
			var insertIdx= Functions.BinarySearch(function, myFunctionComparer);
			if(insertIdx >= 0 && Functions.Count > 0) {
//				Debug.Log("Function Found at=> "+insertIdx);
			}
			else {
				Functions.Insert(~insertIdx, function);
			}			
		}
	}

    // ----------------------------------------------------------------------
	/// Add an obsoleted qualified name with its message.
	public static void AddObsolete(MemberInfo memberInfo, string obsoleteMessage) {
		obsoletes.Add(memberInfo, obsoleteMessage);
	}
	
    // ======================================================================
    // Fix operator display names
    // ----------------------------------------------------------------------
    static string ImproveDisplayName(string displayName) {
        if(displayName.StartsWith("op_") == false) return displayName;
        if(displayName == "op_Equality")     return "operator ==";
        if(displayName == "op_Inequality")   return "operator !=";
        if(displayName == "op_Addition")     return "operator +";
        if(displayName == "op_Subtraction")  return "operator -";
        if(displayName == "op_Multiply")     return "operator *";
        if(displayName == "op_Division")     return "operator /";
        return displayName;
    }
}
