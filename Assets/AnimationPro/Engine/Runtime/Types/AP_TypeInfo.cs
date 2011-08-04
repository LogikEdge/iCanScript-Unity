using UnityEngine;
using System.Collections;

public abstract class AP_TypeInfo {
    
    public System.Type          ValueType        { get { return GetValueType(); }}
    public System.Type          UpConversionType { get { return GetUpConversionType(); }}
    public Color                DisplayColor     { get { return GetDisplayColor(); }}
    public string               ValueTypeName    { get { return AP_TypeSystem.GetTypeName(ValueType); }}
    
    protected abstract System.Type   GetValueType();
    protected abstract System.Type   GetUpConversionType();
    protected virtual  Color         GetDisplayColor()     { return Color.white; }
}
