using System;

[AttributeUsage(AttributeTargets.Method)]
public class UK_PropertyAttribute : Attribute {
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
    
    public string ToolTip {
        get { return myToolTip; }
        set { myToolTip= value; }
    }
    private string myToolTip= null;
    
    // ======================================================================
    public override string ToString() { return "UK_Property"; }
}
