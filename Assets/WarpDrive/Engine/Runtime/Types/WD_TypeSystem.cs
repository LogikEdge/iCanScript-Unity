using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WD_TypeSystem {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public static readonly WD_BoolType         BoolType       = new WD_BoolType();
    public static readonly WD_IntType          IntType        = new WD_IntType();
    public static readonly WD_FloatType        FloatType      = new WD_FloatType();
    public static readonly WD_Vector2Type      Vector2Type    = new WD_Vector2Type();
    public static readonly WD_Vector3Type      Vector3Type    = new WD_Vector3Type();
    public static readonly WD_Vector4Type      Vector4Type    = new WD_Vector4Type();
    public static readonly WD_StringType       StringType     = new WD_StringType();
    public static readonly WD_GameObjectType   GameObjectType = new WD_GameObjectType();
    

    private static Inf.Dictionary<string, WD_TypeInfo>    myDictionary= new Inf.Dictionary<string, WD_TypeInfo>();


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
        myDictionary.Add(theTypeInfo.ValueTypeName, theTypeInfo);
    }

    // ----------------------------------------------------------------------
    // Returns the name used to identify the given type.
    public static string GetTypeName(System.Type theType) {
        return theType != null ? theType.FullName : null;
    }
    
    // ----------------------------------------------------------------------
    // Retreives the TypeDesc associated with the given name.  Null is
    // returned if the TypeInfo is not found.
    public static WD_TypeInfo TryGetTypeInfo(string theFullName) {
        WD_TypeInfo theTypeInfo;
        return myDictionary.TryGetValue(theFullName, out theTypeInfo) ? theTypeInfo : null;
    }

    // ----------------------------------------------------------------------
    // Retreives the TypeDesc associated with the value type name.
    public static WD_TypeInfo GetTypeInfo(string theValueTypeName) {
        WD_TypeInfo theTypeInfo= TryGetTypeInfo(theValueTypeName);
        if(theTypeInfo == null) {
            Debug.LogWarning("Unable to retreive TypeInfo for type "+theValueTypeName);
            throw new System.ArgumentException();
        }
        return theTypeInfo;
    }
    
    // ----------------------------------------------------------------------
    // Retreives the TypeDesc associated with the value type.
    public static WD_TypeInfo GetTypeInfo(System.Type theValueType) {
        return GetTypeInfo(GetTypeName(theValueType));
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
        return type != null ? GetDisplayColor(GetTypeName(type)) : Color.white;
    }
    
    // ----------------------------------------------------------------------
    // Returns the display color to use to the given type.  White is returned
    // if type is not in database.
    public static Color GetDisplayColor(string typeName) {
        WD_TypeInfo typeInfo= TryGetTypeInfo(typeName);
        return typeInfo != null ? typeInfo.DisplayColor : Color.white;
    }

}
