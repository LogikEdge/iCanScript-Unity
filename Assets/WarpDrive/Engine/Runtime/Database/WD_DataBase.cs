using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_DataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<WD_ConversionDesc>  Conversions= new List<WD_ConversionDesc>();
    public static List<WD_FunctionDesc>    Functions  = new List<WD_FunctionDesc>();
    public static List<WD_ClassDesc>       Classes    = new List<WD_ClassDesc>();
    
    // ======================================================================
    // DataBase functionality
    // ----------------------------------------------------------------------
    // Returns all the company names for which a WarpDrive component exists.
    public static string[] GetCompanies() {
        List<string> companies= new List<string>();
        foreach(var conv in Conversions) {
            WarpDrive.AddUniqu<string>(conv.Company, companies);
        }
        foreach(var func in Functions) {
            WarpDrive.AddUniqu<string>(func.Company, companies);
        }
        foreach(var cls in Classes) {
            WarpDrive.AddUniqu<string>(cls.Company, companies);
        }
        return companies.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available packages of the given company.
    public static string[] GetPackages(string company) {
        List<string> packages= new List<string>();
        foreach(var conv in Conversions) {
            if(conv.Company == company) {
                WarpDrive.AddUniqu<string>(conv.Package, packages);                
            }
        }
        foreach(var func in Functions) {
            if(func.Company == company) {
                WarpDrive.AddUniqu<string>(func.Package, packages);                
            }
        }
        foreach(var cls in Classes) {
            if(cls.Company == company) {
                WarpDrive.AddUniqu<string>(cls.Package, packages);                
            }
        }
        return packages.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available conversions of the given company/package.
    public static string[] GetConversions(string company, string package) {
        List<string> conversions= new List<string>();
        foreach(var conv in Conversions) {
            if(conv.Company == company && conv.Package == package) {
                WarpDrive.AddUniqu<string>(conv.Name, conversions);                
            }
        }
        return conversions.ToArray();        
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
    // Returns all available classes of the given company/package.
    public static string[] GetClasses(string company, string package) {
        List<string> classes= new List<string>();
        foreach(var cls in Classes) {
            if(cls.Company == company && cls.Package == package) {
                WarpDrive.AddUniqu<string>(cls.Name, classes);                
            }
        }
        return classes.ToArray();
    }
    
    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        Conversions.Clear();
        Functions.Clear();
        Classes.Clear();
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(string company, string package, MethodInfo methodInfo, Type fromType, Type toType) {
        foreach(var desc in Conversions) {
            if(desc.FromType == fromType && desc.ToType == toType) {
                Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+desc.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                return;
            }
        }
        Conversions.Add(new WD_ConversionDesc(company, package, methodInfo, fromType, toType));
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddFunction(string company, string package, Type classType,            // Class info
                                   string methodName,                                         // Function info
                                   string[] paramNames, Type[] paramTypes, bool[] paramInOuts,// Parameters info
                                   string retName, Type retType,                              // Return value info
                                   string toolTip, MethodInfo methodInfo) {
        WD_FunctionDesc fd= new WD_FunctionDesc(company, package, classType,
                                                methodName, toolTip,
                                                paramNames, paramTypes, paramInOuts,
                                                methodInfo);
        Functions.Add(fd);
        
        if(methodName.CompareTo("Inc") == 0) {
            WD_RuntimeMethod m= fd.CreateRuntime();
            m.Args[0]= 1.5f;  // in parameter
            m.Args[1]= null;  // out parameter
            float r1= (float)m.Invoke();
            float p2= (float)m.Args[1]; // Extract out parameter
            Debug.Log("r1= "+r1+" p2= "+p2);
        }
    }
    // ----------------------------------------------------------------------
    // Adds a class.
    public static void AddClass(string company, string package, Type classType,                                                                 // Class info
                                string[] fieldNames, Type[] fieldTypes, bool[] fieldInOuts,                                                     // Field info
                                string[] propertyNames, Type[] propertyTypes, bool[] propertyInOuts,                                            // Property info
                                MethodInfo[] methodInfos, string[] methodNames, string[] returnNames, Type[] returnTypes, string[] toolTips,    // Method info
                                string[][] parameterNames, Type[][] parameterTypes, bool[][] parameterInOuts) {                                 // Method parameter info
        Classes.Add(new WD_ClassDesc(company, package, classType,
                                     fieldNames, fieldTypes, fieldInOuts,
                                     propertyNames, propertyTypes, propertyInOuts,
                                     methodInfos, methodNames, returnNames, returnTypes, toolTips,
                                     parameterNames, parameterTypes, parameterInOuts));    
    }


}
