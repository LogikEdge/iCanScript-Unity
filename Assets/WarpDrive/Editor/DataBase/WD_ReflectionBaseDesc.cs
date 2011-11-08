using UnityEngine;
using System;
using System.Collections;

public abstract class WD_ReflectionBaseDesc {
    public string   Company;
    public string   Package;
    public Type     ClassType;
    public string   Name;
    public string   ToolTip;
    public string   IconPath;
    public WD_ReflectionBaseDesc(string company, string package, string name, string toolTip, string icon, Type classType) {
        Company  = company;
        Package  = package;
        Name     = name;
        ToolTip  = toolTip;
        ClassType= classType;
        IconPath = icon;
    }    
    public override string ToString() {
        return WD_DataBase.ToString(this);
    }
}
