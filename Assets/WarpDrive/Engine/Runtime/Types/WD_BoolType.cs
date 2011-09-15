using UnityEngine;
using System.Collections;

public sealed class WD_BoolType : WD_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(bool); }
    protected override System.Type      GetUpConversionType()   { return typeof(int); }
    protected override Color            GetDisplayColor()       { return Color.red; }
}