using UnityEngine;
using System;
using System.Collections;

public abstract class WD_ReflectionBaseDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string   Company;
    public string   Package;
    public Type     ClassType;
    public string   Name;
    public string   ToolTip;
    public string   IconPath;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_ReflectionBaseDesc(string company, string package, string name, string toolTip, string iconPath, Type classType) {
        Company  = company;
        Package  = package;
        Name     = name;
        ToolTip  = toolTip;
        ClassType= classType;
        IconPath = iconPath;
    }    

    // ======================================================================
    // Archiving
    // ----------------------------------------------------------------------
    public abstract string Encode();
}
