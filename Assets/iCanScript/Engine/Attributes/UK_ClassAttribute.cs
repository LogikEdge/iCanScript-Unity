using System;

[AttributeUsage(AttributeTargets.Class)]
public class UK_ClassAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Name {
        get { return myName; }
        set { myName= value; }
    }
    private string myName= null;
    
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
    
    public string ToolTip {
        get { return myToolTip; }
        set { myToolTip= value; }
    }
    private string myToolTip= null;
    
    public string Icon {
        get { return myIcon; }
        set { myIcon= value; }
    }
    private string myIcon= null;

    // ======================================================================
    public override string ToString() { return "UK_Class"; }
}
