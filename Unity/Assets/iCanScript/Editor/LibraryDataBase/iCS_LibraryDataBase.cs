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
    public static List<iCS_TypeInfo>        types    = new List<iCS_TypeInfo>();
    public static List<iCS_MethodBaseInfo>  Functions= new List<iCS_MethodBaseInfo>();
    public static bool                      IsSorted = false;
    
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
                    var tmp= Functions[i];
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
            result= d1.company.CompareTo(d2.company);
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
    public static List<iCS_MethodBaseInfo> BuildExpertMenu() {
        return AllFunctions();
    }
    // ----------------------------------------------------------------------
    public static List<iCS_MethodBaseInfo> AllFunctions() {
        QSort();
        return Functions;
    }
    // ----------------------------------------------------------------------
    public static List<iCS_MethodBaseInfo> BuildNormalMenu() {
        QSort();
        var menu= new List<iCS_MethodBaseInfo>();
        foreach(var desc in Functions) {
            Type classType= desc.classType;
            if(iCS_Types.IsStaticClass(classType)) {
                menu.Add(desc);
            } else {
                bool found= false;
                foreach(var existing in menu) {
                    if(classType == existing.classType) {
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
	    var constructors= new List<iCS_ConstructorInfo>();
        foreach(var c in GetMembers(compilerType)) {
            if(c.isConstructor) {
                constructors.Add(c.toConstructorInfo);
            }
        }
        return constructors.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_FieldInfo[] GetFields(Type compilerType) {
	    var fields= new List<iCS_FieldInfo>();
	    foreach(var f in GetMembers(compilerType)) {
	        if(f.isField) {
	            fields.Add(f.toFieldInfo);
	        }
	    }
        return fields.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_PropertyInfo[] GetProperties(Type compilerType) {
	    var properties= new List<iCS_PropertyInfo>();
	    foreach(var p in GetMembers(compilerType)) {
            if(p.isProperty) {
                properties.Add(p.toPropertyInfo);
            }
        }
	    return properties.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_MethodBaseInfo[] GetPropertiesAndFields(Type compilerType) {
	    var variables= new List<iCS_MethodBaseInfo>();
	    foreach(var v in GetMembers(compilerType)) {
	        if(v.isField || v.isProperty) {
	            variables.Add(v.toMethodBaseInfo);
	        }
	    }
	    return variables.ToArray();
	}
    // ----------------------------------------------------------------------
	public static iCS_MethodInfo[] GetMethods(Type compilerType) {
        var methods= new List<iCS_MethodInfo>();
        foreach(var m in GetMembers(compilerType)) {
            if(m.isMethod && !m.isConstructor && !m.isProperty) {
                methods.Add(m.toMethodInfo);
            }
        }
        return methods.ToArray();
	}
    // ----------------------------------------------------------------------
    public static List<iCS_MethodBaseInfo> BuildMenu(Type inputType, Type outputType) {
        QSort();
        var menu= new List<iCS_MethodBaseInfo>();
        for(int i= 0; i < Functions.Count; ++i) {
            // Filter functions according to input or output filter.
            bool shouldInclude= false;
            var func= Functions[i];
            if(inputType != null) {
                if(func.classType == inputType) {
                    switch(func.objectType) {
                        case iCS_ObjectTypeEnum.InstanceMethod:
                        case iCS_ObjectTypeEnum.InstanceField: {
                            shouldInclude= true;
                            break;
                        }
                    }
                }
                for(int j= 0; !shouldInclude && j < func.parameters.Length; ++j) {
                    var param= func.parameters[j];
                    if(param.direction != iCS_ParamDirection.Out) {
						if(param.type == inputType) {
//                        if(iCS_Types.IsA(func.ParamTypes[j], inputType)) {
                            shouldInclude= true;
                        }
                    }
                }
            }
            if(!shouldInclude && outputType != null) {
                if(func.classType == outputType) {
                    switch(func.objectType) {
                        case iCS_ObjectTypeEnum.Constructor:
                        case iCS_ObjectTypeEnum.InstanceMethod:
                        case iCS_ObjectTypeEnum.InstanceField: {
                            shouldInclude= true;
                            break;
                        }
                    }
                }
                if(func.returnType == outputType) shouldInclude= true;
                for(int j= 0; !shouldInclude && j < func.parameters.Length; ++j) {
                    var param= func.parameters[j];
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
            if(desc.functionPath == classPath) return desc.classType;
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
		foreach(var m in typeInfo.members) {
	        if(m.isTypeCast) {
	            var typeCast= m.toTypeCastInfo;
	            if(iCS_Types.CanBeConnectedWithoutConversion(fromType, typeCast.parameters[0].type) &&
	               iCS_Types.CanBeConnectedWithoutConversion(typeCast.returnType, toType)) {
					return typeCast;
				}
	        }
			if(m.isTypeInfo) {
				var cast= FindTypeCast(m.toTypeInfo, fromType, toType);
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
    public static iCS_TypeInfo AddTypeInfo(string company, string package, Type compilerType, iCS_TypeInfo parentTypeInfo, string displayName, string iconPath, string description) {
        var typeInfo= GetTypeInfo(compilerType);
        if(typeInfo != null) {
            return typeInfo;
        }
        typeInfo= new iCS_TypeInfo(company, package, compilerType, parentTypeInfo, displayName, iconPath, description);
        types.Add(typeInfo);
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
        var record= new iCS_FieldInfo(iCS_ObjectTypeEnum.StaticField, classInfo, displayName, description, iconPath,
                                      parameters, functionReturn,
                                      storageClass, accessorType, fieldInfo);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddMethod(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                 iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                 iCS_StorageClass storageClass, MethodInfo methodInfo) {
        var record= new iCS_MethodInfo(iCS_ObjectTypeEnum.InstanceMethod, classInfo,
                                       displayName, description, iconPath,
            						   parameters, functionReturn,
            						   storageClass, methodInfo);
		AddDataBaseRecord(record);
    }
    // ----------------------------------------------------------------------
    public static void AddProperty(iCS_TypeInfo classInfo, string displayName, string description, string iconPath,
                                   iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                   iCS_StorageClass storageClass, iCS_AccessorType accessorType,
                                   MethodInfo methodInfo) {
        var record= new iCS_PropertyInfo(iCS_ObjectTypeEnum.InstanceMethod, classInfo,
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
            if(desc.isTypeCast) {
                var typeCast= desc.toTypeCastInfo;
                if(typeCast.parameters[0].type == fromType && typeCast.returnType == toType) {
                    Debug.LogWarning("Duplicate type cast from "+fromType+" to "+toType+" exists in classes "+typeCast.method.DeclaringType+" and "+methodInfo.DeclaringType);
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
    // Adds a new database record.
    public static void AddDataBaseRecord(iCS_MemberInfo record) {
        if(record.isMethodBase) {
            Functions.Add(record.toMethodBaseInfo);
            IsSorted= false;	            
        }
	}
}
