using UnityEngine;
using System;
using System.Collections;

public abstract class WD_BaseDesc {
    public string   Company;
    public string   Package;
    public string   Name;
    public Type     ClassType;
    public WD_BaseDesc(string company, string package, string name, Type classType) {
        Company  = company;
        Package  = package;
        Name     = name;
        ClassType= classType;
    }    
}
