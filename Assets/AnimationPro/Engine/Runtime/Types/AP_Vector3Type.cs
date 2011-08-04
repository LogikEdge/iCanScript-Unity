using UnityEngine;
using System.Collections;

public sealed class AP_Vector3Type : AP_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(Vector3); }
    protected override System.Type      GetUpConversionType()   { return typeof(Vector4); }
    protected override Color            GetDisplayColor()       { return Color.green; }
}
