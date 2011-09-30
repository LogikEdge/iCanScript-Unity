using System;

[AttributeUsage(AttributeTargets.Method)]
public class WD_ConversionAttribute : Attribute {
    public override string ToString() { return "WD_Conversion"; }
}
