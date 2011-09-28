using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WD_TypeSystem {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public static readonly WD_TypeInfo   BoolType       = new WD_TypeInfo(typeof(bool),       typeof(int),     Color.red);
    public static readonly WD_TypeInfo   IntType        = new WD_TypeInfo(typeof(int),        typeof(float),   Color.magenta);
    public static readonly WD_TypeInfo   FloatType      = new WD_TypeInfo(typeof(float),      typeof(Vector2), Color.cyan);
    public static readonly WD_TypeInfo   Vector2Type    = new WD_TypeInfo(typeof(Vector2),    typeof(Vector3), Color.yellow);
    public static readonly WD_TypeInfo   Vector3Type    = new WD_TypeInfo(typeof(Vector3),    typeof(Vector4), Color.green);
    public static readonly WD_TypeInfo   Vector4Type    = new WD_TypeInfo(typeof(Vector4),    null,            Color.blue);
    public static readonly WD_TypeInfo   StringType     = new WD_TypeInfo(typeof(string),     null,            Color.red);
    public static readonly WD_TypeInfo   GameObjectType = new WD_TypeInfo(typeof(GameObject), null,            Color.blue);
    
    private static List<WD_TypeInfo>    myTypeInfos= new List<WD_TypeInfo>();

    // ----------------------------------------------------------------------
    // Fills the database with the built-in types.
    static WD_TypeSystem() {
        // Concrete types
        Add(BoolType);
        Add(IntType);
        Add(FloatType);
        Add(Vector2Type);
        Add(Vector3Type);
        Add(Vector4Type);
        Add(StringType);
        Add(GameObjectType);
    }
    
    // ----------------------------------------------------------------------
    // Adds a new type descriptor in the type database.
    public static void Add(WD_TypeInfo theTypeInfo) {
        myTypeInfos.Add(theTypeInfo);
    }

    // ----------------------------------------------------------------------
    // Retreives the TypeDesc associated with the value type.
    public static WD_TypeInfo GetTypeInfo(System.Type theValueType) {
        foreach(var t in myTypeInfos) {
            if(t.ValueType == theValueType) return t;
        }
        return null;
    }
    
    // ----------------------------------------------------------------------
    // Retreives the type up conversion list.
    public static System.Type[] GetUpConversionList(System.Type theValueType) {
        List<System.Type> theResult= new List<System.Type>();
        WD_TypeInfo theTypeInfo= GetTypeInfo(theValueType);
        while(theTypeInfo != null) {
            System.Type theUpConversion= theTypeInfo.UpConversionType;
            if(theUpConversion == null) break;
            theResult.Add(theUpConversion);
            theTypeInfo= GetTypeInfo(theUpConversion);
        }
        return theResult.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns the best up conversion type between.  It is assumed that the
    // given type are concrete types (can be instantiated).
    public static System.Type GetBestUpConversionType(System.Type theType1, System.Type theType2) {
        if(theType1 == null) return theType2;
        if(theType2 == null) return theType1;
        if(theType1 == theType2) return theType1;
        System.Type[] theConversionList= GetUpConversionList(theType1);
        foreach(var theTestType in theConversionList) {
            if(theTestType == theType2) return theType2;
        }
        theConversionList= GetUpConversionList(theType2);
        foreach(var theTestType in theConversionList) {
            if(theTestType == theType1) return theType1;
        }
        return null;
    }

    // ----------------------------------------------------------------------
    // Returns the display color to use to the given type.  White is returned
    // if type is not in database.
    public static Color GetDisplayColor(System.Type type) {
        if(type.IsArray) type= type.GetElementType();
        if(type == null) return Color.white;
        WD_TypeInfo typeInfo= GetTypeInfo(type);
        return typeInfo != null ? typeInfo.DisplayColor : Color.white;
    }
    
}
