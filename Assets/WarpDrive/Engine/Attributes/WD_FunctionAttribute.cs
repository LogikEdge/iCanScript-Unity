using System;

[AttributeUsage(AttributeTargets.Method)]
public class WD_FunctionAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Name {
        get { return myName; }
        set { myName= value; }
    }
    private string myName= null;
    
    public string Return {
        get { return myReturn; }
        set { myReturn= value; }
    }
    private string myReturn= null;
    
    // ======================================================================
    public override string ToString() { return "WD_Function"; }
}
