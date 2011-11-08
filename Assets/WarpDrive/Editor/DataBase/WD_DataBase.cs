using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_DataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<WD_BaseDesc>    Functions  = new List<WD_BaseDesc>();
    
    // ======================================================================
    // DataBase functionality
    // ----------------------------------------------------------------------
    // Returns all the company names for which a WarpDrive component exists.
    public static string[] GetCompanies() {
        List<string> companies= new List<string>();
        foreach(var func in Functions) {
            WarpDrive.AddUniqu<string>(func.Company, companies);
        }
        return companies.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available packages of the given company.
    public static string[] GetPackages(string company) {
        List<string> packages= new List<string>();
        foreach(var func in Functions) {
            if(func.Company == company) {
                WarpDrive.AddUniqu<string>(func.Package, packages);                
            }
        }
        return packages.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available functions of the given company/package.
    public static string[] GetFunctions(string company, string package) {
        List<string> functions= new List<string>();
        foreach(var func in Functions) {
            if(func.Company == company && func.Package == package) {
                WarpDrive.AddUniqu<string>(func.Name, functions);                
            }
        }
        return functions.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns the descriptor associated with the given company/package/function.
    public static WD_BaseDesc GetDescriptor(string company, string package, string function) {
        foreach(var desc in Functions) {
            if(desc.Company == company &&
               desc.Package == package &&
               desc.Name    == function) return desc;
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Finds a conversion that matches the given from/to types.
    public static WD_ConversionDesc FindConversion(Type fromType, Type toType) {
        foreach(var desc in Functions) {
            if(desc is WD_ConversionDesc) {
                WD_ConversionDesc conv= desc as WD_ConversionDesc;
                if(WD_Types.CanBeConnectedWithoutConversion(fromType, conv.FromType) &&
                   WD_Types.CanBeConnectedWithoutConversion(conv.ToType, toType)) return conv;
            }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns a string that uniquely describes the descriptor.
    // Format: ObjectType:company:package:classType:methodName<[out] paramName[:=defaultValue]:paramType; ...>{[children]}
    public static string ToString(WD_BaseDesc desc) {
        string result= desc.Company+":"+desc.Package+":"+WD_Types.ToString(desc.ClassType);
        if(desc is WD_FunctionDesc) {
            WD_FunctionDesc funcDesc= desc as WD_FunctionDesc;
            result= WD_Types.ToString(WD_ObjectTypeEnum.Function)+":"+result+":"+desc.Name+"<";
            for(int i= 0; i < funcDesc.ParameterTypes.Length; ++i) {
                if(funcDesc.ParameterIsOuts[i]) result+= "out ";
                result+= funcDesc.ParameterNames[i];
                if(funcDesc.ParameterDefaults[i] != null) {
                    result+= ":="+WD_Types.ToString(funcDesc.ParameterDefaults[i]);
                }
                result+= ":"+WD_Types.ToString(funcDesc.ParameterTypes[i]);
                result+= ";";
            }
            result+= "out "+funcDesc.ReturnName+":"+(funcDesc.ReturnType != null ? WD_Types.ToString(funcDesc.ReturnType) : typeof(void).ToString());
        } else if(desc is WD_ConversionDesc) {
            WD_ConversionDesc convDesc= desc as WD_ConversionDesc;
            result= WD_Types.ToString(WD_ObjectTypeEnum.Conversion)+":"+result+":"+convDesc.Method.Name+"<";
            result+= convDesc.FromType.ToString()+":"+WD_Types.ToString(convDesc.FromType)+";out "+convDesc.ToType.ToString()+":"+WD_Types.ToString(convDesc.ToType);
        }
        result+=">{}";
        return result;
    }
    // ----------------------------------------------------------------------
    // Returns a string that uniquely describes the descriptor.
    public static string ToString(WD_Descriptor desc) {
        string result= WD_Types.ToString(desc.ObjectType)+":"+desc.Company+":"+desc.Package+":"+WD_Types.ToString(desc.ClassType)+":"+desc.Name+"<";
        for(int i= 0; i < desc.ParamTypes.Length; ++i) {
            if(desc.ParamIsOuts[i]) result+= "out ";
            result+= desc.ParamNames[i];
            if(desc.ParamDefaultValues[i] != null) {
                result+= ":="+WD_Types.ToString(desc.ParamDefaultValues[i]);
            }
            result+= ":"+WD_Types.ToString(desc.ParamTypes[i]);
            if(i != desc.ParamTypes.Length-1) result+= ";";
        }
        result+=">{";
        foreach(var child in desc.Children) result+= child;
        result+="}";
        return result;
    }
    // ----------------------------------------------------------------------
    // Returns the BaseDesc associated with the given string.
    public static WD_BaseDesc FromString(string encoded) {
        foreach(var desc in Functions) {
            if(desc.ToString() == encoded) return desc;
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Decodes the string into its constituants.
    public static WD_Descriptor ParseDescriptorString(string encoded) {
        WD_Descriptor desc= new WD_Descriptor();
        // object type
        int end= encoded.IndexOf(':');
        string objectTypeStr= encoded.Substring(0, end);
        desc.ObjectType= WD_Types.FromString<WD_ObjectTypeEnum>(objectTypeStr);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // company
        end= encoded.IndexOf(':');
        desc.Company= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // package
        end= encoded.IndexOf(':');
        desc.Package= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // class type
        end= encoded.IndexOf(':');
        string className= encoded.Substring(0, end);
        desc.ClassType= WD_Types.FromString<Type>(className);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // name
        end= encoded.IndexOf('<');
        desc.Name= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // parameters
        end= encoded.IndexOf('>');
        string parameterString= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        ParseParameters(parameterString, out desc.ParamIsOuts, out desc.ParamTypes, out desc.ParamNames, out desc.ParamDefaultValues);
        // children
        desc.Children= new string[0];
        return desc;
    }
    // ----------------------------------------------------------------------
    // Extracts the type of the parameters from the given string.
    static void ParseParameters(string paramStr, out bool[] isOut, out Type[] types, out string[] names, out object[] defaultValues) {
        List<bool>      paramIsOut   = new List<bool>();
        List<Type>      paramTypes   = new List<Type>();
        List<string>    paramNames   = new List<string>();
        List<object>    paramDefaults= new List<object>();
        while(paramStr.Length > 0) {
            // in/out parameter type
            if(paramStr.StartsWith("out ")) {
                paramIsOut.Add(true);
                paramStr= paramStr.Substring(4, paramStr.Length-4);
            } else {
                paramIsOut.Add(false);
            }                
            // parameter name
            int end= paramStr.IndexOf(':');
            paramNames.Add(paramStr.Substring(0, end));
            paramStr= paramStr.Substring(end+1, paramStr.Length-end-1);
            // parameter default value (part 1)
            string defaultValueStr= null;
            if(paramStr.StartsWith("=")) {
                end= paramStr.IndexOf(':');
                defaultValueStr= paramStr.Substring(1, end-1);
                paramStr= paramStr.Substring(end+1, paramStr.Length-end-1);                
            }
            // parameter type.
            end= paramStr.IndexOf(';');
            Type paramType= WD_Types.FromString<Type>(paramStr.Substring(0, end > 0 ? end : paramStr.Length));
            paramTypes.Add(paramType);
            paramStr= end > 0 ? paramStr.Substring(end+1, paramStr.Length-end-1) : "";
            // parameter default value (part 2)
            if(defaultValueStr != null) {
                paramDefaults.Add(WD_Types.FromString(defaultValueStr, paramType));
            } else {
                paramDefaults.Add(null);                
            }
        }
        isOut= paramIsOut.ToArray();
        types= paramTypes.ToArray();
        names= paramNames.ToArray();
        defaultValues= paramDefaults.ToArray();
    }
    
    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        Functions.Clear();
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(string company, string package, Type classType, string icon, MethodInfo methodInfo, Type fromType, Type toType) {
        foreach(var desc in Functions) {
            if(desc is WD_ConversionDesc) {
                WD_ConversionDesc conv= desc as WD_ConversionDesc;
                if(conv.FromType == fromType && conv.ToType == toType) {
                    Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+conv.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                    return;
                }                
            }
        }
        Functions.Add(new WD_ConversionDesc(company, package, classType, icon, methodInfo, fromType, toType));
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddFunction(string company, string package, string classToolTip, Type classType, // Class info
                                   string methodName,                                                   // Function info
                                   string[] paramNames, Type[] paramTypes,                              // Parameter info
                                   bool[] paramInOuts, object[] paramDefaults,
                                   string retName, Type retType,                                        // Return value info
                                   string toolTip, string icon, MethodInfo methodInfo) {
        WD_FunctionDesc fd= new WD_FunctionDesc(company, package, classToolTip, classType,
                                                methodName, retName, retType, toolTip, icon,
                                                paramNames, paramTypes, paramInOuts, paramDefaults,
                                                methodInfo);
        Functions.Add(fd);
    }
    // ----------------------------------------------------------------------
    // Adds a class.
    public static void AddClass(string company, string package, string className, string classToolTip, Type classType, string classIcon,                        // Class info
                                string[] fieldNames, Type[] fieldTypes, bool[] fieldInOuts,                                                                     // Field info
                                string[] propertyNames, Type[] propertyTypes, bool[] propertyInOuts,                                                            // Property info
                                MethodInfo[] methodInfos, string[] methodNames, string[] returnNames, Type[] returnTypes, string[] toolTips, string[] icons,    // Method info
                                string[][] parameterNames, Type[][] parameterTypes, bool[][] parameterInOuts) {                                                 // Method parameter info
        Functions.Add(new WD_ClassDesc(company, package, className, classToolTip, classType, classIcon,
                                       fieldNames, fieldTypes, fieldInOuts,
                                       propertyNames, propertyTypes, propertyInOuts,
                                       methodInfos, methodNames, returnNames, returnTypes, toolTips, icons,
                                       parameterNames, parameterTypes, parameterInOuts));    
    }


}
