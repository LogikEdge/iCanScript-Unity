using System;

[AttributeUsage(AttributeTargets.Class)]
public class WD_ClassAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Company {
        get { return myCompany; }
        set { myCompany= value; }
    }
    private string myCompany= null;

    public string Package {
        get { return myPackage; }
        set { myPackage= value; }
    }
    private string myPackage= null;
    
    // ======================================================================
    public override string ToString() { return "WD_Class"; }
}
