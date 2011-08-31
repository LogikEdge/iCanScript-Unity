using UnityEngine;
using System.Collections;

public sealed class AP_Vector4Type : AP_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(Vector4); }
    protected override System.Type      GetUpConversionType()   { return null; }
    protected override Color            GetDisplayColor()       { return Color.blue; }
}

