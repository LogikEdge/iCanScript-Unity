using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Archive {
    // ======================================================================
    //  string to/from object conversions
	// ----------------------------------------------------------------------
    public static string Encode(object obj) {
		return Encode(obj, obj.GetType());
	}
    public static string Encode(object obj, Type expectedType) {
		if(expectedType.IsArray) {
			Type expectedDataType= iCS_Types.GetDataType(expectedType);
			Array array= obj as Array;
			string result= "{"+Encode(array.Length, typeof(int));
			for(int i= 0; i < array.Length; ++i) {
				result+= Encode(array.GetValue(i), expectedDataType);
			}
			result+= "}\n";
			return null;
		}
		if(obj is Type) {
			Type type= obj as Type;
	        return "{"+type.AssemblyQualifiedName+"}\n";
		}
		// Put type override if type is not the expected type.
        Type objType= obj.GetType();
		if(objType != expectedType) {
			return "{"+Encode(objType, typeof(Type))+Encode(obj, objType)+"}\n";
		}
        if(objType.IsEnum) {
            int enumValue= (int)obj;
            return "{"+enumValue.ToString()+"("+obj.ToString()+")}\n";
        }
        if(objType == typeof(string)) {
            string value= obj as string;
            string encoded= "{\"";
            for(int i= 0; i < value.Length; ++i) {
                switch(value[i]) {
                    case '\\': {
                        encoded+= "\\\\";
                        break;
                    }
                    case '"': {
                        encoded+= "\\\"";
                        break;
                    }
                    default: {
                        encoded+= value[i];
                        break;
                    }
                }
            }
            return encoded+"\"}\n";
        }
        if(objType == typeof(Vector2)) {
            Vector2 v= (Vector2)obj;
            return "{("+v.x+","+v.y+")}\n";
        }
        if(objType == typeof(Vector3)) {
            Vector3 v= (Vector3)obj;
            return "{("+v.x+","+v.y+","+v.z+")}\n";
        }
        if(objType == typeof(Vector4)) {
            Vector4 v= (Vector4)obj;
            return "{("+v.x+","+v.y+","+v.z+","+v.w+")}\n";
        }
        // Use converter for all remaining types.
        try {
            return "{"+(string)Convert.ChangeType(obj, typeof(string))+"}\n";            
        }
        catch(Exception) {
//            Debug.LogWarning("Unable to encode object of type: "+objType.Name);
        }
        return null;
    }
    
	// ----------------------------------------------------------------------
    public static T Decode<T>(string valueStr) {
        return (T)Decode(valueStr, typeof(T));
    }
	// ----------------------------------------------------------------------
    public static object Decode(string valueStr, Type type) {
		return DecodeNoMarkers(RemoveMarkers(valueStr), type);
    }
	// ----------------------------------------------------------------------
    public static T DecodeNoMarkers<T>(string valueStr) {
        return (T)DecodeNoMarkers(valueStr, typeof(T));
    }
	// ----------------------------------------------------------------------
    public static object DecodeNoMarkers(string valueStr, Type type) {
		if(type.IsArray) {
			Type dataType= iCS_Types.GetDataType(type);
			int len= Decode<int>(valueStr);
			Array array= Array.CreateInstance(dataType, len);
			for(int i= 0; i < len; ++i) {
				array.SetValue(Decode(valueStr, dataType), i);
			}
			return array;
		}
        if(type == typeof(Type)) {
            return Type.GetType(valueStr);
        }
		// Check for type override.
		if(valueStr[0] == '{') {
			string typeStr= ExtractDecodeString(ref valueStr);
			Type objType= Type.GetType(typeStr);			
			return Decode(valueStr, objType);
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
            end= valueStr.IndexOf('"');
            return value+valueStr.Substring(0, end);
        }
        if(type == typeof(Vector2)) {
            Vector2 v;
            int end= valueStr.IndexOf("(");
            if(end != 0) { Debug.LogWarning("Decode: Invalid Vector2 format !!!"); return Vector2.zero; }
            valueStr= valueStr.Substring(1, valueStr.Length-1);
            if(!DecodeWithSeperator(ref valueStr, ',', out v.x)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ')', out v.y)) return Vector4.zero;
            return v;                        
        }
        if(type == typeof(Vector3)) {
            Vector3 v;
            int end= valueStr.IndexOf("(");
            if(end != 0) { Debug.LogWarning("Decode: Invalid Vector3 format !!!"); return Vector3.zero; }
            valueStr= valueStr.Substring(1, valueStr.Length-1);
            if(!DecodeWithSeperator(ref valueStr, ',', out v.x)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ',', out v.y)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ')', out v.z)) return Vector4.zero;
            return v;                        
        }
        if(type == typeof(Vector4)) {
            Vector4 v;
            int end= valueStr.IndexOf("(");
            if(end != 0) { Debug.LogWarning("Decode: Invalid Vector4 format !!!"); return Vector4.zero; }
            valueStr= valueStr.Substring(1, valueStr.Length-1);
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
        value= DecodeNoMarkers<T>(valueStr.Substring(0, end));
        valueStr= valueStr.Substring(end+1, valueStr.Length-end-1);
        return true;
    }
	// ----------------------------------------------------------------------
	public static string ExtractDecodeStringWithMarkers(ref string encoded) {
		int len= encoded.Length;
		if(len < 3) {
			Debug.LogError("Decode: formatting error: encoded string does not have the proper markers {...}");			
			encoded= "";
			return null;
		}
		int end= FindDecodeEnd(encoded);
		string toDecode= encoded.Substring(0, end);
		encoded= encoded.Substring(end);
		return toDecode;
	}
	// ----------------------------------------------------------------------
	public static string ExtractDecodeString(ref string encoded) {
		return RemoveMarkers(ExtractDecodeStringWithMarkers(ref encoded));
	}
	// ----------------------------------------------------------------------
	public static string RemoveMarkers(string toDecode) {
		int len= toDecode.Length;
		if(toDecode[0] != '{' || toDecode[len-2] != '}' || toDecode[len-1] != '\n') {
			Debug.LogError("Decode: formatting error: encoded string does not have the proper markers {...}");			
			Debug.Log("Decode string is: "+toDecode);
			return null;
		}
		return toDecode.Substring(1, len-3);
	}
	// ----------------------------------------------------------------------
	static int FindDecodeEnd(string encoded) {
		int i;
		for(i= 0; i < encoded.Length; ++i) {
			if(encoded[i] == '\n') {
				if(i == 0) {
					++i;
					break;
				}
				if(encoded[i-1] == '}') {
					++i;
					break;
				}
			}
		}
		return i;		
	}
}
