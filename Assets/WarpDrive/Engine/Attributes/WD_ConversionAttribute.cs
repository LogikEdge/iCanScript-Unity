using System;

[AttributeUsage(AttributeTargets.Class)]
public class WD_ConversionAttribute : Attribute {
    public readonly Type FromType;
    public readonly Type ToType;
    
    public WD_ConversionAttribute(Type from, Type to) {
        FromType= from;
        ToType= to;
    }

    public override string ToString() { return "WD_Conversion"; }
}
