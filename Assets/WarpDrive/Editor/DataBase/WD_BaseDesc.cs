using UnityEngine;
using System;
using System.Collections;

public abstract class WD_BaseDesc {
    public string   Company;
    public string   Package;
    public string   Name;
    public string   ToolTip;
    public Type     ClassType;
    public string   IconPath;
    public WD_BaseDesc(string company, string package, string name, string toolTip, string icon, Type classType) {
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
