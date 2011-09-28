using UnityEngine;
using System;
using System.Collections;

public sealed class WD_TypeInfo {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------    
    public Type     ValueType       = null;
    public Type     UpConversionType= null;
    public Color    DisplayColor    = Color.white;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_TypeInfo(Type valueType, Type upConversionType, Color displayColor) {
        ValueType= valueType;
        UpConversionType= upConversionType;
        DisplayColor= displayColor;
    }
}

