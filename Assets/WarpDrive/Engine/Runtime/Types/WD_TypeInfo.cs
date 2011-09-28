using UnityEngine;
using System.Collections;

public abstract class WD_TypeInfo {
    
    public System.Type          ValueType        { get { return GetValueType(); }}
    public System.Type          UpConversionType { get { return GetUpConversionType(); }}
    public Color                DisplayColor     { get { return GetDisplayColor(); }}
    
    protected abstract System.Type   GetValueType();
    protected abstract System.Type   GetUpConversionType();
    protected virtual  Color         GetDisplayColor()     { return Color.white; }
}
