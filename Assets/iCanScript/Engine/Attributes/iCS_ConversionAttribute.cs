using System;

[AttributeUsage(AttributeTargets.Method)]
public class iCS_ConversionAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Icon {
        get { return myIcon; }
        set { myIcon= value; }
    }
    private string myIcon= null;
    
    // ======================================================================
    public override string ToString() { return "iCS_Conversion"; }
}
