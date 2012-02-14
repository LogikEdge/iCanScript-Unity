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
//    public static bool                      IsMenuDirty = true;
//    public static string[]                  FunctionMenu= null;
    
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
    // Returns 0 if equal, negative if first is smaller and
    // positive if first is greather.
    public static int CompareFunctionNames(iCS_ReflectionDesc d1, iCS_ReflectionDesc d2) {
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
    public static List<iCS_ReflectionDesc> BuildMenu() {
        QSort();
        return Functions;
    }
    // ----------------------------------------------------------------------
    // Returns one descriptor per class
    public static List<iCS_ReflectionDesc> GetClasses() {
        QSort();
        List<iCS_ReflectionDesc> classes= new List<iCS_ReflectionDesc>();
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
    public static iCS_ReflectionDesc[] GetClassComponents(Type classType) {
        List<iCS_ReflectionDesc> components= new List<iCS_ReflectionDesc>();
        foreach(var desc in Functions) {
            if(desc.ClassType == classType) {
                components.Add(desc);
            }
        }
        return components.ToArray();
    }
    // ----------------------------------------------------------------------
    public static List<iCS_ReflectionDesc> BuildMenu(Type inputType, Type outputType) {
        QSort();
        List<iCS_ReflectionDesc> menu= new List<iCS_ReflectionDesc>();
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
                for(int j= 0; !shouldInclude && j < func.ParamTypes.Length; ++j) {
                    if(func.ParamIsOuts[j] == false) {
						if(func.ParamTypes[j] == inputType) {
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
                for(int j= 0; !shouldInclude && j < func.ParamTypes.Length; ++j) {
                    if(func.ParamIsOuts[j]) {
                        if(outputType == func.ParamTypes[j]) {
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
    public static iCS_ReflectionDesc GetDescriptor(string pathAndSignature) {
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
            null);
    }
    // ----------------------------------------------------------------------
    public static void AddStaticField(string company, string package, string displayName, string toolTip, string iconPath,
                                      Type classType, FieldInfo fieldInfo,
                                      bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                      string retName) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.StaticField, classType, null, fieldInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceField(string company, string package, string displayName, string toolTip, string iconPath,
                                        Type classType, FieldInfo fieldInfo,
                                        bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                        string retName) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.InstanceField, classType, null, fieldInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                         Type classType, MethodInfo methodInfo,
                                         bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                         string retName) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.InstanceMethod, classType, methodInfo, null,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName);
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddStaticMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                       Type classType, MethodInfo methodInfo,
                                       bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                       string retName) {
        Add(company, package, displayName, toolTip, iconPath,
            iCS_ObjectTypeEnum.StaticMethod, classType, methodInfo, null,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName);
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(string company, string package, string iconPath, Type classType, MethodInfo methodInfo, Type fromType) {
        // Don't accept automatic conversion if it already exist.
        Type toType= methodInfo.ReturnType;
        foreach(var desc in Functions) {
            if(IsConversion(desc)) {
                if(desc.ParamTypes[0] == fromType && desc.ReturnType == toType) {
                    Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+desc.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                    return;
                }                
            }
        }
        string fromTypeName= iCS_Types.TypeName(fromType);
        string toTypeName= iCS_Types.TypeName(toType);
        Add(company, package, fromTypeName+"->"+toTypeName, "Converts from "+fromTypeName+" to "+toTypeName, iconPath,
            iCS_ObjectTypeEnum.Conversion, classType, methodInfo, null,
            new bool[1]{false}, new string[1]{fromTypeName}, new Type[1]{fromType}, new object[1]{null},
            toTypeName);        
    }
    // ----------------------------------------------------------------------
    // Adds a new database record.
    public static iCS_ReflectionDesc Add(string company, string package, string displayName, string toolTip, string iconPath,
                                        iCS_ObjectTypeEnum objType, Type classType, MethodBase methodInfo, FieldInfo fieldInfo,
                                        bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                        string retName) {
        iCS_ReflectionDesc fd= new iCS_ReflectionDesc(company, package, displayName, toolTip, iconPath,
                                                      objType, classType, methodInfo, fieldInfo,
                                                      paramIsOuts, paramNames, paramTypes, paramDefaults,
                                                      retName);
        Functions.Add(fd);
        IsSorted= false;
        return fd;
    }
    
}
