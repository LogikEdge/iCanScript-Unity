using UnityEngine;
using System.Collections;

public sealed class AP_IntType : AP_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(int); }
    protected override System.Type      GetUpConversionType()   { return typeof(float); }
    protected override Color            GetDisplayColor()       { return Color.magenta; }
}
