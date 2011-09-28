using System;

[AttributeUsage(AttributeTargets.Field)]
public class WD_InPortAttribute : Attribute {
    public override string ToString() { return "WD_InPort"; }
}
