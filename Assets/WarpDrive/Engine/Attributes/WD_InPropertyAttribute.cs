using System;

[AttributeUsage(AttributeTargets.Property)]
public class WD_InPropertyAttribute : Attribute {
    public override string ToString() { return "WD_InProperty"; }
}
