using System;

[AttributeUsage(AttributeTargets.Property)]
public class UK_InPropertyAttribute : Attribute {
    public override string ToString() { return "UK_InProperty"; }
}
