using System;

[AttributeUsage(AttributeTargets.Property)]
public class UK_OutPropertyAttribute : Attribute {
    public override string ToString() { return "UK_OutProperty"; }
}