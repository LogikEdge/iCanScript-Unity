using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public class iCS_LibraryDataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<iCS_TypeInfo>    types    = new List<iCS_TypeInfo>();
    public static List<iCS_MemberInfo>  Functions= new List<iCS_MemberInfo>();
    public static bool                  IsSorted = false;
    
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
                    iCS_MemberInfo tmp= Functions[i];
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
    public static int CompareFunctionNames(iCS_MemberInfo d1, iCS_MemberInfo d2) {
        if(d1.company == null && d2.company != null) return -1;
        if(d1.company != null && d2.company == null) return 1;
        int result;
        if(d1.company != null) {
            result= d1.company.CompareTo(d2.Company);
            if(result != 0) return result;
        }
        if(d1.package == null && d2.package != null) return -1;
        if(d1.package != null && d2.package == null) return 1;
        if(d1.package != null) {
            result= d1.package.CompareTo(d2.package);
            if(result != 0) return result;            
        }
        return d1.displayName.CompareTo(d2.displayName);
    }
    // ----------------------------------------------------------------------
    public static List<iCS_MemberInfo> BuildExpertMenu() {
        return AllFunctions();
    }
    // ----------------------------------------------------------------------
    public static List<iCS_MemberInfo> AllFunctions() {
        QSort();
        return Functions;
    }
    // ----------------------------------------------------------------------
    public static List<iCS_MemberInfo> BuildNormalMenu() {
        QSort();
        List<iCS_MemberInfo> menu= new List<iCS_MemberInfo>();
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
            if(t.compilerType == compilerType) {
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
		return typeInfo.members.ToArray();
    }
    // ----------------------------------------------------------------------
	public static iCS_ConstructorInfo[] GetConstructors(Type compilerType) {
		return Prelude.filter(c=> c.IsConstructor, GetMembers(compilerType)) as iCS_ConstructorInfo[];
	}
    // ----------------------------------------------------------------------
	public static iCS_FieldInfo[] GetFields(Type compilerType) {
		return Prelude.filter(c=> c.IsField, GetTypeMembers(compilerType)) as iCS_FieldInfo[];
	}
    // ----------------------------------------------------------------------
	public static iCS_PropertyInfo[] GetProperties(Type compilerType) {
		return Prelude.filter(c=> c.IsProperty, GetMembers(compilerType)) as iCS_PropertyInfo[];
	}
    // ----------------------------------------------------------------------
	public static iCS_MemberInfo[] GetPropertiesAndFields(Type compilerType) {
		return Prelude.filter(c=> c.IsField || c.IsProperty, GetMembers(compilerType));
	}
    // ----------------------------------------------------------------------
	public static iCS_MethodInfo[] GetMethods(Type compilerType) {
		return Prelude.filter(c=> !(c.IsConstructor || c.IsField || c.IsProperty), GetMembers(compilerType));
	}
    // ----------------------------------------------------------------------
    public static List<iCS_MemberInfo> BuildMenu(Type inputType, Type outputType) {
        QSort();
        List<iCS_MemberInfo> menu= new List<iCS_MemberInfo>();
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
    public static iCS_MemberInfo GetDescriptor(string pathAndSignature) {
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
    public static iCS_MemberInfo FindTypeCast(Type fromType, Type toType) {
        foreach(var t in types) {
			var cast= FindTypeCast(t, fromType, toType);
			if(cast != null) {
				return cast;
			}
        }
        return null;
    }
    public static iCS_MemberInfo FindTypeCast(iCS_TypeInfo typeInfo, Type fromType, Type toType) {
		foreach(var m in typeInfo.members) {
	        if(m.isTypeCast) {
	            if(iCS_Types.CanBeConnectedWithoutConversion(fromType, m.parameters[0].type) &&
	               iCS_Types.CanBeConnectedWithoutConversion(m.returnType, toType)) {
					return m;
				}
	        }
			if(m.isTypeInfo) {
				var cast= FindTypeCast(m, fromType, toType);
				if(cast != null) {
					return cast;
				}
			}
		}
		return null;
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
    public static iCS_TypeInfo AddTypeInfo(Type compilerType, string displayName, string company, string package, string iconPath, string toolTip) {
        var typeInfo= GetTypeInfo(compilerType);
        if(typeInfo != null) {
            return typeInfo;
        }
        typeInfo= new iCS_TypeInfo(compilerType, displayName, company, package, iconPath, toolTip);
        types.Add(typeInfo);
        return typeInfo;
    }
    // ----------------------------------------------------------------------
    public static void AddConstructor(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                      ConstructorInfo constructorInfo, iCS_Parameter[] parameters) {
        var record= new iCS_ConstructorInfo(classInfo, displayName, description, iconPath,
                                            constructorInfo, parameters);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddStaticField(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                      FieldInfo fieldInfo,
                                      iCS_Parameter[] parameters,
                                      string retName) {
        var record= new iCS_FieldInfo(classInfo, displayName, description, iconPath,
                                      iCS_ObjectTypeEnum.StaticField, fieldInfo,
                                      parameters,
                                      retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceField(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                        FieldInfo fieldInfo,
                                        iCS_Parameter[] parameters,
                                        string retName) {
        var record= new iCS_FieldInfo(classInfo, displayName, description, iconPath,
                                      iCS_ObjectTypeEnum.InstanceField, classType, fieldInfo,
                                      parameters,
                                      retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceMethod(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                         MethodInfo methodInfo,
                                         iCS_Parameter[] parameters,
                                         string retName) {
        var record= new iCS_MethodInfo(classInfo, displayName, description, iconPath,
            						   iCS_ObjectTypeEnum.InstanceMethod, methodInfo,
            						   parameters,
            						   retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddStaticMethod(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                       MethodInfo methodInfo,
                                       iCS_Parameter[] parameters,
                                       string retName) {
		var record= new iCS_MethodInfo(classInfo, displayName, description, iconPath,
						               iCS_ObjectTypeEnum.StaticMethod, methodInfo,
						               parameters,
									   retName);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddTypeCast(iCS_TypeInfo classInfo, string iconPath, MethodInfo methodInfo, Type fromType) {
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
        parameters[0]= new iCS_Parameter();
        parameters[0].name= fromTypeName;
        parameters[0].type= fromType;
        parameters[0].direction= iCS_ParamDirection.In;
        parameters[0].initialValue= null; 
        var record= new iCS_TypeCastInfo(classInfo, "To"+toTypeNameUpper, "Converts from "+fromTypeName+" to "+toTypeName, iconPath,
                                         methodInfo,
                                         parameters,
                                         toTypeName);        
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    // Adds a new database record.
    public static void AddDataBaseRecord(iCS_MemberInfo record) {
        Functions.Add(record);
        IsSorted= false;	
	}
}
