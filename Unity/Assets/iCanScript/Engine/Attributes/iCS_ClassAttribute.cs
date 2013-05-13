using System;

[AttributeUsage(AttributeTargets.Class)]
public class iCS_ClassAttribute : Attribute {
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
    
    public string Tooltip {
        get { return myTooltip; }
        set { myTooltip= value; }
    }
    private string myTooltip= null;
    
    public string Icon {
        get { return myIcon; }
        set { myIcon= value; }
    }
    private string myIcon= null;
    
    public bool BaseVisibility {
        get { return myBaseVisibility; }
        set { myBaseVisibility= value; }
    }
    private bool myBaseVisibility= false;
    
    // ======================================================================
    public override string ToString() { return "iCS_Class"; }
}
