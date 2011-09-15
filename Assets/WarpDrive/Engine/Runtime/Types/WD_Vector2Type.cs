using UnityEngine;
using System.Collections;

public sealed class WD_Vector2Type : WD_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(Vector2); }
    protected override System.Type      GetUpConversionType()   { return typeof(Vector3); }
    protected override Color            GetDisplayColor()       { return Color.yellow; }
}