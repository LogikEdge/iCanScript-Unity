using UnityEngine;
using System.Collections;

public sealed class AP_Vector2Type : AP_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(Vector2); }
    protected override System.Type      GetUpConversionType()   { return typeof(Vector3); }
    protected override Color            GetDisplayColor()       { return Color.yellow; }
}