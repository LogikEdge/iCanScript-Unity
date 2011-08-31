using UnityEngine;
using System.Collections;

public sealed class AP_BoolType : AP_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(bool); }
    protected override System.Type      GetUpConversionType()   { return typeof(int); }
    protected override Color            GetDisplayColor()       { return Color.red; }
}