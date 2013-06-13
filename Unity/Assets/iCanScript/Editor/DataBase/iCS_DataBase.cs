using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_DataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<iCS_ReflectionInfo>  Functions   = new List<iCS_ReflectionInfo>();
    public static bool                      IsSorted    = false;
    
    // ======================================================================
    // DataBase functionality
    // ----------------------------------------------------------------------
    public static void QSort() {
        if(IsSorted) return;
        int reorderCnt= 0;
        int cmpCnt= 0;
        int len= Functions.Count;
        int step= (len >> 1) + (len & 1);
        while(step != 0) {
            int i= 0;
            int j= step;
            while(j < len) {
                ++cmpCnt;
                if(CompareFunctionNames(Functions[i], Functions[j]) > 0) {
                    ++reorderCnt;
                    iCS_ReflectionInfo tmp= Functions[i];
                    Functions[i]= Functions[j];
                    Functions[j]= tmp;
                    int k= i-step;
                    while(k >= 0) {
                        ++cmpCnt;
                        if(CompareFunctionNames(Functions[k], Functions[k+step]) < 0) break;
                        ++reorderCnt;
                        tmp= Functions[k];
                        Functions[k]= Functions[k+step];
                        Functions[k+step]= tmp;
                        k-= step;
                    }
                }
                ++i;
                ++j;
            }
            step >>= 1;
        }
        IsSorted= true;
    }

    // ----------------------------------------------------------------------
    // Returns 0 if equal, negative if first is smaller and
    // positive if first is greather.
    public static int CompareFunctionNames(iCS_ReflectionInfo d1, iCS_ReflectionInfo d2) {
        if(d1.Company == null && d2.Company != null) return -1;
        if(d1.Company != null && d2.Company == null) return 1;
        int result;
        if(d1.Company != null) {
            result= d1.Company.CompareTo(d2.Company);
            if(result != 0) return result;
        }
        if(d1.Package == null && d2.Package != null) return -1;
        if(d1.Package != null && d2.Package == null) return 1;
        if(d1.Package != null) {
            result= d1.Package.CompareTo(d2.Package);
            if(result != 0) return result;            
        }
        return d1.DisplayName.CompareTo(d2.DisplayName);
    }
    // ----------------------------------------------------------------------
    public static List<iCS_ReflectionInfo> BuildExpertMenu() {
        return AllFunctions();
    }
    // ----------------------------------------------------------------------
    public static List<iCS_ReflectionInfo> AllFunctions() {
        QSort();
        return Functions;
    }
    // ----------------------------------------------------------------------
    public static List<iCS_ReflectionInfo> BuildNormalMenu() {
        QSort();
        List<iCS_ReflectionInfo> menu= new List<iCS_ReflectionInfo>();
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
    // Returns one descriptor per class
    public static List<iCS_ReflectionInfo> GetClasses() {
        QSort();
        List<iCS_ReflectionInfo> classes= new List<iCS_ReflectionInfo>();
        foreach(var desc in Functions) {
            Type classType= desc.ClassType;
            bool found= false;
            foreach(var existing in classes) {
                if(classType == existing.ClassType) {
                    found= true;
                    break;
                }
            }
            if(!found) {
                classes.Add(desc);
            }
        }
        return classes;
    }
    // ----------------------------------------------------------------------
    // Returns all components of the given class.
    public static iCS_ReflectionInfo[] GetClassComponents(Type classType) {
        List<iCS_ReflectionInfo> components= new List<iCS_ReflectionInfo>();
        foreach(var desc in Functions) {
            if(desc.ClassType == classType) {
                components.Add(desc);
            }
        }
        return components.ToArray();
    }
    // ----------------------------------------------------------------------
	public static iCS_ReflectionInfo[] GetClassConstructors(Type classType) {
		return Prelude.filter(c=> c.IsConstructor, GetClassComponents(classType));
	}
    // ----------------------------------------------------------------------
	public static iCS_ReflectionInfo[] GetClassFields(Type classType) {
		return Prelude.filter(c=> c.IsField, GetClassComponents(classType));
	}
    // ----------------------------------------------------------------------
	public static iCS_ReflectionInfo[] GetClassProperties(Type classType) {
		return Prelude.filter(c=> c.IsProperty, GetClassComponents(classType));
	}
    // ----------------------------------------------------------------------
	public static iCS_ReflectionInfo[] GetClassVariables(Type classType) {
		return Prelude.filter(c=> c.IsField || c.IsProperty, GetClassComponents(classType));
	}
    // ----------------------------------------------------------------------
	public static iCS_ReflectionInfo[] GetClassMethods(Type classType) {
		return Prelude.filter(c=> !(c.IsConstructor || c.IsField || c.IsProperty), GetClassComponents(classType));
	}
    // ----------------------------------------------------------------------
    public static List<iCS_ReflectionInfo> BuildMenu(Type inputType, Type outputType) {
        QSort();
        List<iCS_ReflectionInfo> menu= new List<iCS_ReflectionInfo>();
        for(int i= 0; i < Functions.Count; ++i) {
            // Filter functions according to input or output filter.
            bool shouldInclude= false;
            var func= Functions[i];
            if(inputType != null) {
                if(func.ClassType == inputType) {
                    switch(func.ObjectType) {
                        case iCS_ObjectTypeEnum.InstanceMethod:
                        case iCS_ObjectTypeEnum.InstanceField: {
                            shouldInclude= true;
                            break;
                        }
                    }
                }
                for(int j= 0; !shouldInclude && j < func.Parameters.Length; ++j) {
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
                        case iCS_ObjectTypeEnum.InstanceMethod:
                        case iCS_ObjectTypeEnum.InstanceField: {
                            shouldInclude= true;
                            break;
                        }
                    }
                }
                if(func.ReturnType == outputType) shouldInclude= true;
                for(int j= 0; !shouldInclude && j < func.Parameters.Length; ++j) {
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
    public static iCS_ReflectionInfo GetDescriptor(string pathAndSignature) {
        foreach(var desc in Functions) {
            if(desc.ToString() == pathAndSignature) return desc;
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
    public static iCS_ReflectionInfo FindTypeCast(Type fromType, Type toType) {
        foreach(var desc in Functions) {
            if(IsTypeCast(desc)) {
                if(iCS_Types.CanBeConnectedWithoutConversion(fromType, desc.Parameters[0].type) &&
                   iCS_Types.CanBeConnectedWithoutConversion(desc.ReturnType, toType)) return desc;
            }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns true if the given desc is a conversion function.
    public static bool IsTypeCast(iCS_ReflectionInfo desc) {
        return desc.ObjectType == iCS_ObjectTypeEnum.TypeCast;
    }
    
    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        Functions.Clear();
    }
    // ----------------------------------------------------------------------
    public static void AddConstructor(string company, string package, string displayName, string toolTip, string iconPath,
                                      Type classType, ConstructorInfo constructorInfo,
                                      iCS_Parameter[] parameters) {
        var record= new iCS_ConstructorInfo(company, package, displayName, toolTip, iconPath,
                                            classType, constructorInfo,
                                            parameters,
                                            null);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddStaticField(string company, string package, string displayName, string toolTip, string iconPath,
                                      Type classType, FieldInfo fieldInfo,
                                      iCS_Parameter[] parameters,
                                      string retName) {
        var record= new iCS_FieldInfo(company, package, displayName, toolTip, iconPath,
                                      iCS_ObjectTypeEnum.StaticField, classType, fieldInfo,
                                      parameters,
                                      retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceField(string company, string package, string displayName, string toolTip, string iconPath,
                                        Type classType, FieldInfo fieldInfo,
                                        iCS_Parameter[] parameters,
                                        string retName) {
        var record= new iCS_FieldInfo(company, package, displayName, toolTip, iconPath,
                                      iCS_ObjectTypeEnum.InstanceField, classType, fieldInfo,
                                      parameters,
                                      retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                         Type classType, MethodInfo methodInfo,
                                         iCS_Parameter[] parameters,
                                         string retName) {
        var record= new iCS_MethodInfo(company, package, displayName, toolTip, iconPath,
            						   iCS_ObjectTypeEnum.InstanceMethod, classType, methodInfo,
            						   parameters,
            						   retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddStaticMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                       Type classType, MethodInfo methodInfo,
                                       iCS_Parameter[] parameters,
                                       string retName) {
		var record= new iCS_MethodInfo(company, package, displayName, toolTip, iconPath,
						               iCS_ObjectTypeEnum.StaticMethod, classType, methodInfo,
						               parameters,
									   retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddTypeCast(string company, string package, string iconPath, Type classType, MethodInfo methodInfo, Type fromType) {
        // Don't accept automatic conversion if it already exist.
        Type toType= methodInfo.ReturnType;
        foreach(var desc in Functions) {
            if(IsTypeCast(desc)) {
                if(desc.Parameters[0].type == fromType && desc.ReturnType == toType) {
                    Debug.LogWarning("Duplicate type cast from "+fromType+" to "+toType+" exists in classes "+desc.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                    return;
                }                
            }
        }
        string fromTypeName= iCS_Types.TypeName(fromType);
        string toTypeName= iCS_Types.TypeName(toType);
        string toTypeNameUpper= Char.ToUpper(toTypeName[0])+toTypeName.Substring(1);

        var parameters= new iCS_Parameter[1];
        parameters[0].name= fromTypeName;
        parameters[0].type= fromType;
        parameters[0].direction= iCS_ParamDirection.In;
        parameters[0].initialValue= null; 
        var record= new iCS_TypeCastInfo(company, package, "To"+toTypeNameUpper, "Converts from "+fromTypeName+" to "+toTypeName, iconPath,
                                         classType, methodInfo,
                                         parameters,
                                         toTypeName);        
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds a new database record.
    public static void AddDataBaseRecord(iCS_ReflectionInfo record) {
        Functions.Add(record);
        IsSorted= false;	
	}
}
