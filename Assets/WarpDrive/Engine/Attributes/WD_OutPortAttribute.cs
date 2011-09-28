using System;

[AttributeUsage(AttributeTargets.Field)]
public class WD_OutPortAttribute : Attribute {
    public override string ToString() { return "WD_OutPort"; }
}
