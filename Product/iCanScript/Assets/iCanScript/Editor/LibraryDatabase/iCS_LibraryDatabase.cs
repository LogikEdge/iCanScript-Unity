using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using iCanScript.Engine;
using P=iCanScript.Prelude;

namespace iCanScript.Editor {
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
}

