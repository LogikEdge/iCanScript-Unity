using System;

[AttributeUsage(AttributeTargets.Method)]
public class WD_ConversionAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Icon {
        get { return myIcon; }
        set { myIcon= value; }
    }
    private string myIcon= null;
    
    // ======================================================================
    public override string ToString() { return "WD_Conversion"; }
}
