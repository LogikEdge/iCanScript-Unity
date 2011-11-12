using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_Archive {
    // ======================================================================
    //  string to/from object conversions
	// ----------------------------------------------------------------------
    public static string Encode(Type type) {
        return type.AssemblyQualifiedName;
    }
    public static string Encode(object obj) {
        if(obj.GetType().IsEnum) {
            int enumValue= (int)obj;
            return enumValue.ToString()+"("+obj.ToString()+")";
        }
        if(obj.GetType() == typeof(string)) {
            string value= obj as string;
            string encoded= "\"";
            int end= value.IndexOf('"');
            while(end > 0) {
                encoded+= value.Substring(0, end-1);
                encoded+= '\\';
                encoded+= '"';
                value= value.Substring(end+1, value.Length-end-1);
            }
            return encoded+value+"\"";
        }
        if(obj.GetType() == typeof(Vector2)) {
            Vector2 v= (Vector2)obj;
            return "("+v.x+","+v.y+")";
        }
        if(obj.GetType() == typeof(Vector3)) {
            Vector3 v= (Vector3)obj;
            return "("+v.x+","+v.y+","+v.z+")";
        }
        if(obj.GetType() == typeof(Vector4)) {
            Vector4 v= (Vector4)obj;
            return "("+v.x+","+v.y+","+v.z+","+v.w+")";
        }
        return obj.ToString();
    }
    
	// ----------------------------------------------------------------------
    public static T Decode<T>(string valueStr) {
        return (T)Decode(valueStr, typeof(T));
    }
	// ----------------------------------------------------------------------
    public static object Decode(string valueStr, Type type) {
        if(type == typeof(Type)) {
            return Type.GetType(valueStr);
        }
        if(type == typeof(string)) {
            string value= "";
            int end= valueStr.IndexOf('"');
            if(end != 0) { Debug.LogWarning("Decode: Invalid string format !!!"); return ""; }
            valueStr= valueStr.Substring(1, valueStr.Length-1);
            int escape= valueStr.IndexOf('\\');
            while(escape > 0 && escape < valueStr.Length-1) {
                switch(valueStr[escape+1]) {
                    case '\\':
                        value+= valueStr.Substring(0, escape);
                        valueStr= valueStr.Substring(escape+2, valueStr.Length-escape-2);
                        break;
                    case '"':
                        value+= valueStr.Substring(0, escape-1)+'"';
                        valueStr= valueStr.Substring(escape+2, valueStr.Length-escape-2);
                        break;
                }
            }
            string tmp;
            DecodeWithSeperator(ref valueStr, '"', out tmp);
            return value+tmp;
        }
        if(type == typeof(Vector2)) {
            Vector2 v;
            int end= valueStr.IndexOf("(");
            if(end != 0) { Debug.LogWarning("Decode: Invalid Vector2 format !!!"); return Vector2.zero; }
            if(!DecodeWithSeperator(ref valueStr, ',', out v.x)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ')', out v.y)) return Vector4.zero;
            return v;                        
        }
        if(type == typeof(Vector3)) {
            Vector3 v;
            int end= valueStr.IndexOf("(");
            if(end != 0) { Debug.LogWarning("Decode: Invalid Vector3 format !!!"); return Vector3.zero; }
            if(!DecodeWithSeperator(ref valueStr, ',', out v.x)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ',', out v.y)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ')', out v.z)) return Vector4.zero;
            return v;                        
        }
        if(type == typeof(Vector4)) {
            Vector4 v;
            int end= valueStr.IndexOf("(");
            if(end != 0) { Debug.LogWarning("Decode: Invalid Vector4 format !!!"); return Vector4.zero; }
            if(!DecodeWithSeperator(ref valueStr, ',', out v.x)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ',', out v.y)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ',', out v.z)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ')', out v.w)) return Vector4.zero;
            return v;                        
        }
        if(type.IsEnum) {
            int value;
            DecodeWithSeperator(ref valueStr, '(', out value);
            return value;
        }
        // Use System.Convert for all object that supports IConvertible.
        Type[] useConvert= type.FindInterfaces((t,s)=> t.ToString() == s.ToString(), "System.IConvertible");
        if(useConvert.Length > 0) {
            return Convert.ChangeType(valueStr, type);
        }
        // We don't know this type ...
        Debug.LogWarning("FromString for type: "+type.Name+" is undefined.");
        return null;
    }
	// ----------------------------------------------------------------------
    static bool DecodeWithSeperator<T>(ref string valueStr, char seperator, out T value) {
        int end= valueStr.IndexOf(seperator);
        if(end < 0) { Debug.LogWarning("Decode: Invalid "+typeof(T).Name+" format !!!"); value= default(T); return false; }
        value= Decode<T>(valueStr.Substring(0, end-1));
        valueStr= valueStr.Substring(end+1, valueStr.Length-end-1);
        return true;
    }
}
