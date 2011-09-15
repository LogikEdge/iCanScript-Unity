using UnityEngine;
using System.Collections;

public sealed class WD_StringType : WD_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(string); }
    protected override System.Type      GetUpConversionType()   { return null; }
    protected override Color            GetDisplayColor()       { return Color.red; }
}
