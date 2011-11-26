using System;

[AttributeUsage(AttributeTargets.Method)]
public class UK_ConversionAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Icon {
        get { return myIcon; }
        set { myIcon= value; }
    }
    private string myIcon= null;
    
    // ======================================================================
    public override string ToString() { return "UK_Conversion"; }
}
