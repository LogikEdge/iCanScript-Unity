using UnityEngine;
using System.Collections;

public sealed class AP_FloatType : AP_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(float); }
    protected override System.Type      GetUpConversionType()   { return typeof(Vector2); }
    protected override Color            GetDisplayColor()       { return Color.cyan; }
}
