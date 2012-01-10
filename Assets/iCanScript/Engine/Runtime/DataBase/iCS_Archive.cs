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
        iCS_Coder coder= new iCS_Coder();
        coder.Encode(obj, "key");
        Debug.Log("Coder: "+coder.Archive);
        
		string encoded= EncodeNoMarkers(obj, obj.GetType());
		return "{"+encoded.Length.ToString()+':'+encoded+"}";
	}
    static string EncodeNoMarkers(object obj, Type expectedType) {
		if(expectedType.IsArray) {
			Type expectedDataType= iCS_Types.GetElementType(expectedType);
			Array array= obj as Array;
			string result= Encode(array.Length, typeof(int));
			for(int i= 0; i < array.Length; ++i) {
				result+= Encode(array.GetValue(i), expectedDataType);
			}
			return result;
		}
		if(obj is Type) {
			Type type= obj as Type;
	        return type.AssemblyQualifiedName;
		}
		// Put type override if type is not the expected type.
        Type objType= obj.GetType();
		if(objType != expectedType) {
			return Encode(objType, typeof(Type))+Encode(obj, objType);
		}
        if(objType.IsEnum) {
            int enumValue= (int)obj;
            return enumValue.ToString()+"("+obj.ToString()+")";
        }
        if(objType == typeof(string)) {
            string value= obj as string;
            string encoded= "\"";
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
            return encoded+"\"";
        }
        if(objType == typeof(Vector2)) {
            Vector2 v= (Vector2)obj;
            return "("+v.x+","+v.y+")";
        }
        if(objType == typeof(Vector3)) {
            Vector3 v= (Vector3)obj;
            return "("+v.x+","+v.y+","+v.z+")";
        }
        if(objType == typeof(Vector4)) {
            Vector4 v= (Vector4)obj;
            return "("+v.x+","+v.y+","+v.z+","+v.w+")";
        }
        // Use converter for all remaining types.
        try {
            return (string)Convert.ChangeType(obj, typeof(string));            
        }
        catch(Exception) {
//            Debug.LogWarning("Unable to encode object of type: "+objType.Name);
        }
        return null;
    }
    
	// ----------------------------------------------------------------------
    public static T Decode<T>(ref string valueStr) {
        return (T)Decode(ref valueStr, typeof(T));
    }
	// ----------------------------------------------------------------------
    public static object Decode(ref string valueStr, Type type) {
		if(valueStr == null || valueStr[0] != '{') {
			Debug.LogWarning("iCanScript: Format error when decoding: expected '{': received: "+valueStr);
			SkipDecode(ref valueStr);
			return null;
		}
		int end= valueStr.IndexOf(':');
		int size= (int)Convert.ChangeType(valueStr.Substring(1,end-1), typeof(int));
		string toDecode= valueStr.Substring(end+1,size);
		object result= DecodeNoMarkers(toDecode, type);
		valueStr= valueStr.Substring(end+size+2);
		return result;
	}
	// ----------------------------------------------------------------------
    static object DecodeNoMarkers(string valueStr, Type type) {
		if(type.IsArray) {
			Type dataType= iCS_Types.GetElementType(type);
			int len= Decode<int>(ref valueStr);
			Array array= Array.CreateInstance(dataType, len);
			for(int i= 0; i < len; ++i) {
				array.SetValue(Decode(ref valueStr, dataType), i);
			}
			return array;
		}
        if(type == typeof(Type)) {
			return Type.GetType(valueStr);
        }
		// Check for type override.
		if(valueStr[0] == '{') {
			Type objType= Decode<Type>(ref valueStr);
			return Decode(ref valueStr, objType);
		}
		// Now we are only interrested in the data type.
		type= iCS_Types.GetElementType(type);
        if(type == typeof(string)) {
			if(valueStr[0] != '"') {
	            Debug.LogWarning("iCanScript: Format error when decoding string: string does not start with \"");
				SkipDecode(ref valueStr);
				return "";
			}
            string value= "";
			int i;
            for(i= 1; i < valueStr.Length && valueStr[i] != '"'; ++i) {
				if(valueStr[i] == '\\') ++i;
				value+= valueStr[i];
            }
			if(i != valueStr.Length) {
				valueStr= valueStr.Substring(i);
			} else {
	            Debug.LogWarning("iCanScript: Format error when decoding string: could not find matching \"");				
				valueStr= "";
			}
            return value;
        }
        if(type == typeof(Vector2)) {
            if(valueStr[0] != '(') {
				Debug.LogWarning("iCanScript: Decode: Invalid Vector2 format !!!");
				return Vector2.zero;
			}
            valueStr= valueStr.Substring(1);
            Vector2 v;
            if(!DecodeWithSeperator(ref valueStr, ',', out v.x)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ')', out v.y)) return Vector4.zero;
            return v;                        
        }
        if(type == typeof(Vector3)) {
            if(valueStr[0] != '(') {
	            Debug.LogWarning("iCanScript: Decode: Invalid Vector3 format !!!");
				return Vector3.zero;
			}
            valueStr= valueStr.Substring(1);
            Vector3 v;
            if(!DecodeWithSeperator(ref valueStr, ',', out v.x)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ',', out v.y)) return Vector4.zero;
            if(!DecodeWithSeperator(ref valueStr, ')', out v.z)) return Vector4.zero;
            return v;                        
        }
        if(type == typeof(Vector4)) {
            if(valueStr[0] != '(') {
	            Debug.LogWarning("iCanScript: Decode: Invalid Vector4 format !!!");
				return Vector4.zero;
			}
            valueStr= valueStr.Substring(1);
            Vector4 v;
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
        Debug.LogWarning("iCanScript: Decoding of type: "+type.Name+" is undefined.");
        return null;
    }
	// ----------------------------------------------------------------------
    static bool DecodeWithSeperator<T>(ref string valueStr, char seperator, out T value) {
		int end= valueStr.IndexOf(seperator);
		string toDecode= "";
		if(end < 0) {
			toDecode= valueStr;
			valueStr= "";
		} else {
			toDecode= valueStr.Substring(0, end);
			valueStr= valueStr.Substring(end+1);
		}
        value= (T)DecodeNoMarkers(toDecode, typeof(T));
        return true;
    }
	// ----------------------------------------------------------------------
	static void SkipDecode(ref string valueStr) {
		int len= valueStr.Length;
		int i;
		for(i= 0; i < len; ++i) {
			if(valueStr[i] == '}') {
				++i;
				break;
			}
		}
		valueStr= i == len ? "" : valueStr.Substring(i);		
	}
	// ----------------------------------------------------------------------
	static void EndDecode(ref string valueStr) {
		if(valueStr[0] != '}') {
			Debug.LogWarning("iCanScript: Format error when decoding: expected '}': received: "+valueStr);
			SkipDecode(ref valueStr);
			return;
		}
		valueStr= valueStr.Substring(1);
	}
}
