using UnityEngine;
using System.Collections;

public sealed class WD_Vector3Type : WD_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(Vector3); }
    protected override System.Type      GetUpConversionType()   { return typeof(Vector4); }
    protected override Color            GetDisplayColor()       { return Color.green; }
}
