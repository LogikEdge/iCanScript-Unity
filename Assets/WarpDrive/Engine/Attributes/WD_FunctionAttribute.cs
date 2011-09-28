using System;

[AttributeUsage(AttributeTargets.Class)]
public class WD_ClassAttribute : Attribute {
    public override string ToString() { return "WD_Class"; }
}
