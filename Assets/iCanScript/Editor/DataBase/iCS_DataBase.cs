using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_DataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static bool                      IsMenuDirty = true;
    public static List<iCS_ReflectionDesc>  Functions   = new List<iCS_ReflectionDesc>();
    public static string[]                  FunctionMenu= null;
    
    // ======================================================================
    // DataBase functionality
    // ----------------------------------------------------------------------
    public static void QSort() {
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
                    iCS_ReflectionDesc tmp= Functions[i];
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
        Debug.Log("Len: "+len+" Cmp: "+cmpCnt+" QSort reorder: "+reorderCnt);
    }
    // ----------------------------------------------------------------------
    public static void BubbleSort() {
        int reorderCnt= 0;
        int cmpCnt= 0;
        int len= Functions.Count;
        int min= 0;
        int max= len-1;
        int minRestart= min;
        while(min != max) {
            ++cmpCnt;
            if(CompareFunctionNames(Functions[min], Functions[min+1]) > 0) {
                ++reorderCnt;
                iCS_ReflectionDesc tmp= Functions[min];
                Functions[min]= Functions[min+1];
                Functions[min+1]= tmp;
                if(min != 0) --min;
            } else {
                ++min;
                if(min < minRestart) {
                    min= minRestart;
                } else {
                    minRestart= min;
                }
            }
        }
        Debug.Log("Len: "+len+" Cmp: "+cmpCnt+" BubbleSort reorder: "+reorderCnt);
    }
    // ----------------------------------------------------------------------
    static bool IsSorted() {
        int len= Functions.Count;
        for(int i= 0; i < len-1; ++i) {
            if(CompareFunctionNames(Functions[i], Functions[i+1]) > 0) {
                iCS_ReflectionDesc tmp= Functions[i];
                Functions[i]= Functions[i+1];
                Functions[i+1]= tmp;
                return false;
            }
        }
        return true;
    }
    // ----------------------------------------------------------------------
    // Returns all the company names for which a uCode component exists.
    public static string[] GetCompanies() {
        List<string> companies= new List<string>();
        foreach(var func in Functions) {
            uCode.AddUniqu<string>(func.Company, companies);
        }
        return companies.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available packages of the given company.
    public static string[] GetPackages(string company) {
        List<string> packages= new List<string>();
        foreach(var func in Functions) {
            if(func.Company == company) {
                uCode.AddUniqu<string>(func.Package, packages);                
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
                uCode.AddUniqu<string>(func.DisplayName, functions);                
            }
        }
        return functions.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available functions parameters for the given
    // company/package/function.
    public static string[] GetFunctionSignatures(string company, string package, string functionName) {
        List<string> parameters= new List<string>();
        foreach(var func in Functions) {
            if(func.Company == company && func.Package == package && func.DisplayName == functionName) {
                parameters.Add(GetFunctionSignature(func));
            }
        }
        return parameters.ToArray();
    }

    // ----------------------------------------------------------------------
    public static string GetFunctionSignature(iCS_ReflectionDesc desc) {
        string signature= TypeName(desc.ReturnType);
        signature+= " "+desc.DisplayName+"(";
        if(desc.ObjectType == iCS_ObjectTypeEnum.InstanceMethod) {
            signature+= TypeName(desc.ClassType)+" this";
            if(desc.ParamNames.Length != 0) signature+=", ";
        }
        for(int i= 0; i < desc.ParamNames.Length; ++i) {
            signature+= TypeName(desc.RuntimeDesc.ParamTypes[i])+" "+desc.ParamNames[i];
            if(i != desc.ParamNames.Length-1) signature+=", ";
        }
        return signature+")";
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/displayName".
    public static string GetFunctionName(iCS_ReflectionDesc desc) {
        return desc.Company+"/"+desc.Package+"/"+desc.DisplayName;
    }
    // ----------------------------------------------------------------------
    // Returns 0 if equal, negative if first is smaller and
    // positive if first is greather.
    public static int CompareFunctionNames(iCS_ReflectionDesc d1, iCS_ReflectionDesc d2) {
        int result= d1.Company.CompareTo(d2.Company);
        if(result != 0) return result;
        result= d1.Package.CompareTo(d2.Package);
        if(result != 0) return result;
        return d1.DisplayName.CompareTo(d2.DisplayName);
    }
    // ----------------------------------------------------------------------
    public static string[] BuildMenu() {
        if(!IsMenuDirty) return FunctionMenu;
        QSort();
//        BubbleSort();
        if(!IsSorted()) {
            Debug.Log("Menu is not properly sorted");
        }
        List<string> menu= new List<string>();
        string previousName= "";
        bool needsSignature= false;
        for(int i= 0; i < Functions.Count; ++i) {
            string newName= GetFunctionName(Functions[i]);
            if(previousName == newName) needsSignature= true;
            if(previousName != "") {
                if(needsSignature) { menu.Add(previousName+"/"+GetFunctionSignature(Functions[i-1])); }
                else               { menu.Add(previousName); }
            }
            if(previousName != newName) {
                needsSignature= false;
                previousName= newName;
            }
        }
        if(previousName != "") {
            if(needsSignature) { menu.Add(previousName+"/"+GetFunctionSignature(Functions[Functions.Count-1])); }
            else               { menu.Add(previousName); }
        }
        FunctionMenu= menu.ToArray();
        IsMenuDirty= false;
        return FunctionMenu;
    }
    // ----------------------------------------------------------------------
    static string TypeName(Type type) {
        if(type == null) return "void";
        if(type == typeof(float)) return "float";
        return type.Name;
    }
    // ----------------------------------------------------------------------
    // Returns the descriptor associated with the given company/package/function.
    public static iCS_ReflectionDesc GetDescriptor(string company, string package, string functionName, string signature) {
        foreach(var desc in Functions) {
            if(desc.Company == company &&
               desc.Package == package &&
               desc.DisplayName == functionName) {
                   if(signature == null) return desc;
                   if(signature == GetFunctionSignature(desc)) return desc;
               }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Finds a conversion that matches the given from/to types.
    public static iCS_ReflectionDesc FindConversion(Type fromType, Type toType) {
        foreach(var desc in Functions) {
            if(IsConversion(desc)) {
                iCS_RuntimeDesc conv= desc.RuntimeDesc;
                if(iCS_Types.CanBeConnectedWithoutConversion(fromType, conv.ParamTypes[0]) &&
                   iCS_Types.CanBeConnectedWithoutConversion(conv.ReturnType, toType)) return desc;
            }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns true if the given desc is a conversion function.
    public static bool IsConversion(iCS_ReflectionDesc desc) {
        return desc.RuntimeDesc.ObjectType == iCS_ObjectTypeEnum.Conversion;
    }
    
    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        Functions.Clear();
    }
    // ----------------------------------------------------------------------
    public static void AddStaticField(string company, string package, string displayName, string toolTip, string iconPath,
                                      Type classType,
                                      bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.StaticField, classType, null,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            null, null);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceField(string company, string package, string displayName, string toolTip, string iconPath,
                                        Type classType,
                                        bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.InstanceField, classType, null,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            null, null);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                       Type classType, MethodInfo methodInfo,
                                       bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                       string retName, Type retType) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.InstanceMethod, classType, methodInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName, retType);
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddStaticMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                       Type classType, MethodInfo methodInfo,
                                       bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                       string retName, Type retType) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.StaticMethod, classType, methodInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName, retType);
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(string company, string package, string iconPath, Type classType, MethodInfo methodInfo, Type fromType, Type toType) {
        // Don't accept automatic conversion if it already exist.
        foreach(var desc in Functions) {
            if(IsConversion(desc)) {
                iCS_RuntimeDesc conv= desc.RuntimeDesc;
                if(conv.ParamTypes[0] == fromType && conv.ReturnType == toType) {
                    Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+conv.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                    return;
                }                
            }
        }
        Add(company, package, fromType.Name+"->"+toType.Name, "Converts from "+fromType.Name+" to "+toType.Name, iconPath,
            iCS_ObjectTypeEnum.Conversion, classType, methodInfo,
            new bool[1]{false}, new string[1]{fromType.Name}, new Type[1]{fromType}, new object[1]{null},
            toType.Name, toType);        
    }
    // ----------------------------------------------------------------------
    // Adds a new database record.
    public static iCS_ReflectionDesc Add(string company, string package, string displayName, string toolTip, string iconPath,
                                        iCS_ObjectTypeEnum objType, Type classType, MethodInfo methodInfo,
                                        bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                        string retName, Type retType) {
        iCS_ReflectionDesc fd= new iCS_ReflectionDesc(company, package, displayName, toolTip, iconPath,
                                                    objType, classType, methodInfo,
                                                    paramIsOuts, paramNames, paramTypes, paramDefaults,
                                                    retName, retType);
        Functions.Add(fd);
        IsMenuDirty= true;
        return fd;
    }
    
}
