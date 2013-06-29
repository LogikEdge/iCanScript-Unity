using System;

[AttributeUsage(AttributeTargets.Class)]
public class iCS_ExcludeAttribute : Attribute {
    // ======================================================================
    public override string ToString() { return "iCS_Exclude"; }
}