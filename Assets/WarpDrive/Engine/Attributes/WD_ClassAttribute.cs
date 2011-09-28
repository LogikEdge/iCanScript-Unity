using System;

[AttributeUsage(AttributeTargets.Class)]
public class WD_FunctionAttribute : Attribute {
    public override string ToString() { return "WD_Function"; }
}
