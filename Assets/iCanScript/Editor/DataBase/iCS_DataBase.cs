using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_DataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<iCS_ReflectionDesc>  Functions   = new List<iCS_ReflectionDesc>();
    public static bool                      IsSorted    = false;
    public static bool                      IsMenuDirty = true;
    public static string[]                  FunctionMenu= null;
    
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
        IsSorted= true;
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
            signature+= TypeName(desc.ParamTypes[i])+" "+desc.ParamNames[i];
            if(i != desc.ParamNames.Length-1) signature+=", ";
        }
        return signature+")";
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/displayName".
    public static string GetFunctionPath(iCS_ReflectionDesc desc) {
        return desc.Company+"/"+desc.Package;
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/displayName".
    public static string GetFunctionName(iCS_ReflectionDesc desc) {
        return GetFunctionPath(desc)+"/"+desc.DisplayName;
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
        List<string> menu= new List<string>();
        string previousName= "";
        bool needsSignature= false;
        for(int i= 0; i < Functions.Count; ++i) {
            string newName= GetFunctionName(Functions[i]);
            if(previousName == newName) needsSignature= true;
            if(previousName != "") {
                var func= Functions[i-1];
                var signature= "/"+GetFunctionSignature(func);
                if(needsSignature) { menu.Add(previousName+signature); }
//                else               { menu.Add(GetFunctionPath(func)+signature); }
                else               { menu.Add(previousName); }
            }
            if(previousName != newName) {
                needsSignature= false;
                previousName= newName;
            }
        }
        if(previousName != "") {
            var func= Functions[Functions.Count-1];
            var signature= "/"+GetFunctionSignature(func);
            if(needsSignature) { menu.Add(previousName+signature); }
//            else               { menu.Add(GetFunctionPath(func)+signature); }
            else               { menu.Add(previousName); }
        }
        FunctionMenu= menu.ToArray();
        IsMenuDirty= false;
        return FunctionMenu;
    }
    // ----------------------------------------------------------------------
    public static string[] BuildMenu(bool filterOnInput, Type inputType, bool filterOnOutput, Type outputType) {
        QSort();
        List<string> menu= new List<string>();
        string previousName= "";
        bool needsSignature= false;
        for(int i= 0; i < Functions.Count; ++i) {
            // Filter functions according to input or output filter.
            bool shouldInclude= false;
            var func= Functions[i];
            if(filterOnInput) {
                if(func.ClassType == inputType) {
                    switch(func.ObjectType) {
                        case iCS_ObjectTypeEnum.InstanceMethod:
                        case iCS_ObjectTypeEnum.InstanceField: {
                            shouldInclude= true;
                            break;
                        }
                    }
                }
                for(int j= 0; !shouldInclude && j < func.ParamTypes.Length; ++j) {
                    if(func.ParamIsOuts[j] == false) {
                        if(func.ParamTypes[j] == inputType) {
                            shouldInclude= true;
                        }
                    }
                }
            }
            if(!shouldInclude && filterOnOutput) {
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
                for(int j= 0; !shouldInclude && j < func.ParamTypes.Length; ++j) {
                    if(func.ParamIsOuts[j]) {
                        if(func.ParamTypes[j] == outputType) {
                            shouldInclude= true;
                        }
                    }
                }
            }
            if(shouldInclude) {
                string newName= GetFunctionName(func);
                if(previousName == newName) needsSignature= true;
                if(previousName != "") {
                    var prevFunc= Functions[i-1];
                    var signature= "/"+GetFunctionSignature(prevFunc);
                    if(needsSignature) { menu.Add(previousName+signature); }
    //                else               { menu.Add(GetFunctionPath(prevFunc)+signature); }
                    else               { menu.Add(previousName); }
                }                
                if(previousName != newName) {
                    needsSignature= false;
                    previousName= newName;
                }
            }
        }
        if(previousName != "") {
            var func= Functions[Functions.Count-1];
            var signature= "/"+GetFunctionSignature(func);
            if(needsSignature) { menu.Add(previousName+signature); }
//            else               { menu.Add(GetFunctionPath(func)+signature); }
            else               { menu.Add(previousName); }
        }
        return menu.ToArray();
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
                if(iCS_Types.CanBeConnectedWithoutConversion(fromType, desc.ParamTypes[0]) &&
                   iCS_Types.CanBeConnectedWithoutConversion(desc.ReturnType, toType)) return desc;
            }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns true if the given desc is a conversion function.
    public static bool IsConversion(iCS_ReflectionDesc desc) {
        return desc.ObjectType == iCS_ObjectTypeEnum.Conversion;
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
                                      bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.Constructor, classType, constructorInfo, null,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            null, null);
    }
    // ----------------------------------------------------------------------
    public static void AddStaticField(string company, string package, string displayName, string toolTip, string iconPath,
                                      Type classType, FieldInfo fieldInfo,
                                      bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.StaticField, classType, null, fieldInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            null, null);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceField(string company, string package, string displayName, string toolTip, string iconPath,
                                        Type classType, FieldInfo fieldInfo,
                                        bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.InstanceField, classType, null, fieldInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            null, null);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                         Type classType, MethodInfo methodInfo,
                                         bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                         string retName, Type retType) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.InstanceMethod, classType, methodInfo, null,
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
            iCS_ObjectTypeEnum.StaticMethod, classType, methodInfo, null,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName, retType);
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(string company, string package, string iconPath, Type classType, MethodInfo methodInfo, Type fromType, Type toType) {
        // Don't accept automatic conversion if it already exist.
        foreach(var desc in Functions) {
            if(IsConversion(desc)) {
                if(desc.ParamTypes[0] == fromType && desc.ReturnType == toType) {
                    Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+desc.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                    return;
                }                
            }
        }
        Add(company, package, fromType.Name+"->"+toType.Name, "Converts from "+fromType.Name+" to "+toType.Name, iconPath,
            iCS_ObjectTypeEnum.Conversion, classType, methodInfo, null,
            new bool[1]{false}, new string[1]{fromType.Name}, new Type[1]{fromType}, new object[1]{null},
            toType.Name, toType);        
    }
    // ----------------------------------------------------------------------
    // Adds a new database record.
    public static iCS_ReflectionDesc Add(string company, string package, string displayName, string toolTip, string iconPath,
                                        iCS_ObjectTypeEnum objType, Type classType, MethodBase methodInfo, FieldInfo fieldInfo,
                                        bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                        string retName, Type retType) {
        iCS_ReflectionDesc fd= new iCS_ReflectionDesc(company, package, displayName, toolTip, iconPath,
                                                      objType, classType, methodInfo, fieldInfo,
                                                      paramIsOuts, paramNames, paramTypes, paramDefaults,
                                                      retName, retType);
        Functions.Add(fd);
        IsMenuDirty= true;
        IsSorted= false;
        return fd;
    }
    
}
