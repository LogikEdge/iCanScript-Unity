using UnityEngine;
using System.Collections;

public sealed class WD_GameObjectType : WD_TypeInfo {
    protected override System.Type      GetValueType()          { return typeof(GameObject); }
    protected override System.Type      GetUpConversionType()   { return null; }
    protected override Color            GetDisplayColor()       { return Color.blue; }
}
