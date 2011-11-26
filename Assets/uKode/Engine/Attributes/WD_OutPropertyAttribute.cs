using System;

[AttributeUsage(AttributeTargets.Property)]
public class WD_OutPropertyAttribute : Attribute {
    public override string ToString() { return "WD_OutProperty"; }
}