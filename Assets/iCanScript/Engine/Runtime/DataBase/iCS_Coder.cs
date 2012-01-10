using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_Coder {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
	Dictionary<string,Prelude.Tuple<string,string>>	myDictionary;
	
    // ======================================================================
    // Construction/Destruction
	// ----------------------------------------------------------------------
	public iCS_Coder() {
		myDictionary= new Dictionary<string,Prelude.Tuple<string,string>>();
	}
	public iCS_Coder(string archive) : this() {
		Archive= archive;
	}

    // ======================================================================
    // Archiving
	// ----------------------------------------------------------------------
	public string Archive {
		get {
			string result= "";
			foreach(var pair in myDictionary) {
				result+= "{"+BuildArchive(pair.Key)+BuildArchive(pair.Value.Item1)+BuildArchive(pair.Value.Item2)+"}";
			}
			return result;
		}
		set {
            myDictionary.Clear();
            while(value.Length > 0 && value[0] != '}') {
    			if(value[0] != '{') { DecodeError('{', value); return; }
                value= value.Substring(1);
                string key= ExtractFromArchive(ref value);
                string valueType= ExtractFromArchive(ref value);
                string valueStr= ExtractFromArchive(ref value);
                myDictionary.Add(key, new Prelude.Tuple<string,string>(valueType, valueStr));
    			if(value[0] != '}') { DecodeError('}', value); return; }
            }
		}
	}
	// ----------------------------------------------------------------------
	string BuildArchive(string str) {
		return str.Length.ToString()+":"+str;
	}
	// ----------------------------------------------------------------------
    string ExtractFromArchive(ref string value) {
        // Extract size.
        int end= value.IndexOf(':');
        if(end < 0) { DecodeError(':', value); value= ""; return ""; }
        int size= DecodeInt(value.Substring(0, end));
        value= value.Substring(end+1);
        // Extract string.
        string result= value.Substring(0, size);
        value= value.Substring(size);
		return result;
    }

    // ======================================================================
    // Encoding
	// ----------------------------------------------------------------------
	public void Add(string key, Type type, string valueStr) {
        Prelude.Tuple<string, string> value= new Prelude.Tuple<string, string>(EncodeType(type), valueStr);
		if(myDictionary.ContainsKey(key)) {
			myDictionary[key]= value;
		} else {
			myDictionary.Add(key, value);
		}
	}
	// ----------------------------------------------------------------------
	public void EncodeObject(string key, object value) {
        if(value == null) return;
		Type valueType= value.GetType();
		iCS_Coder coder= new iCS_Coder();
		// Special case for arrays.
		if(valueType.IsArray) {
			Array array= value as Array;
			coder.EncodeInt("Length", array.Length);
			for(int i= 0; i < array.Length; ++i) {
				coder.EncodeObject(i.ToString(), array.GetValue(i));
			}
            Add(key, valueType, coder.Archive);
            return;
		}
		// Special case for enums.
		if(valueType.IsEnum) {
			coder.EncodeInt("Numeric", (int)value);
			coder.EncodeString("Name", value.ToString());
            Add(key, valueType, coder.Archive);
			return;
        }
		// Primitives.
        if(value is Type)   { EncodeType(key, (Type)value); return; }
		if(value is string) { EncodeString(key, (string)value); return; }
		if(value is bool)   { EncodeBool(key, (bool)value); return; }
		if(value is int)    { EncodeInt(key, (int)value); return; }
		if(value is float)  { EncodeFloat(key, (float)value); return; }
		// All other types.
		foreach(var field in valueType.GetFields()) {
            if(!field.IsStatic) {
    			coder.EncodeObject(field.Name, field.GetValue(value));                
            }
		}
		Add(key, valueType, coder.Archive);
	}
	// ----------------------------------------------------------------------
    public string EncodeString(string value) {
        return value;
    }
	// ----------------------------------------------------------------------
    public void EncodeString(string key, string value) {
        Add(key, typeof(string), EncodeString(value));
    }
	// ----------------------------------------------------------------------
	string EncodeType(Type value) {
		if(value == typeof(Type)) return "t";
		if(value == typeof(string)) return "s";
		if(value == typeof(bool)) return "b";
		if(value == typeof(int)) return "i";
		if(value == typeof(float)) return "f";
        if(value == typeof(Vector2)) return "v2";
        if(value == typeof(Vector3)) return "v3";
        if(value == typeof(Vector4)) return "v4";
        if(value == typeof(System.Object)) return "o";
		if(value == typeof(Type[])) return "t[]";
		if(value == typeof(string[])) return "s[]";
		if(value == typeof(bool[])) return "b[]";
		if(value == typeof(int[])) return "i[]";
		if(value == typeof(float[])) return "f[]";
        if(value == typeof(Vector2[])) return "v2[]";
        if(value == typeof(Vector3[])) return "v3[]";
        if(value == typeof(Vector4[])) return "v4[]";
        if(value == typeof(System.Object[])) return "o[]";
		if(value == typeof(iCS_RuntimeDesc)) return "iCS_RuntimeDesc";
		if(value == typeof(iCS_ObjectTypeEnum)) return "iCS_ObjectTypeEnum";
        if(value.IsByRef) {
            Type elementType= value.GetElementType();
    		if(elementType == typeof(Type)) return "t&";
    		if(elementType == typeof(string)) return "s&";
    		if(elementType == typeof(bool)) return "b&";
    		if(elementType == typeof(int)) return "i&";
    		if(elementType == typeof(float)) return "f&";
            if(elementType == typeof(Vector2)) return "v2&";
            if(elementType == typeof(Vector3)) return "v3&";
            if(elementType == typeof(Vector4)) return "v4&";
            if(elementType == typeof(System.Object)) return "o&";
        }
		return value.AssemblyQualifiedName;
	}
	// ----------------------------------------------------------------------
	public void EncodeType(string key, Type value) {
		Add(key, typeof(Type), EncodeType(value));
	}
	// ----------------------------------------------------------------------
	string EncodeBool(bool value) {
		return value ? "T" : "F";
	}
	// ----------------------------------------------------------------------
	public void EncodeBool(string key, bool value) {
		Add(key, typeof(bool), EncodeBool(value));
	}
	// ----------------------------------------------------------------------
	string EncodeInt(int value) {
		return (string)Convert.ChangeType(value, typeof(string));
	}
	// ----------------------------------------------------------------------
	public void EncodeInt(string key, int value) {
		Add(key, typeof(int), EncodeInt(value));
	}
	// ----------------------------------------------------------------------
	string EncodeFloat(float value) {
		return (string)Convert.ChangeType(value, typeof(string));
	}
	// ----------------------------------------------------------------------
	public void EncodeFloat(string key, float value) {
		Add(key, typeof(float), EncodeFloat(value));
	}

    // ======================================================================
    // Decoding
	// ----------------------------------------------------------------------
    public object DecodeObjectForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return null;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        string valueStr= tuple.Item2;
        iCS_Coder coder= new iCS_Coder();
        // Special case for arrays.
		if(valueType.IsArray) {
            Type arrayBaseType= iCS_Types.GetElementType(valueType);
            coder.Archive= valueStr;
            int len= coder.DecodeIntForKey("Length");
            Array array= Array.CreateInstance(arrayBaseType, len);
			for(int i= 0; i < len; ++i) {
                array.SetValue(coder.DecodeObjectForKey(i.ToString()), i);
			}
			return array;
		}
		// Special case for enums.
		if(valueType.IsEnum) {
            coder.Archive= valueStr;
            return coder.DecodeIntForKey("Numeric");
        }
		// Primitives.
		Type elementType= iCS_Types.GetElementType(valueType);
        if(elementType == typeof(Type))   return DecodeType(valueStr);
		if(elementType == typeof(string)) return DecodeString(valueStr);
		if(elementType == typeof(bool))   return DecodeBool(valueStr);
		if(elementType == typeof(int))    return DecodeInt(valueStr);
		if(elementType == typeof(float))  return DecodeFloat(valueStr);
		// All other types.
        coder.Archive= valueStr;
        object obj= iCS_Types.CreateInstance(valueType);
		foreach(var field in valueType.GetFields()) {
            if(!field.IsStatic) {
    			field.SetValue(obj, coder.DecodeObjectForKey(field.Name));
            }
		}
        return obj;
    }
	// ----------------------------------------------------------------------
    Type DecodeTypeForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return typeof(void);
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<Type>(valueType)) {
            DecodeTypeError("Type", valueType);
            return typeof(void);
        }
        return DecodeType(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    Type DecodeType(string value) {
		if(value == "t") return typeof(Type);
		if(value == "s") return typeof(string);
		if(value == "b") return typeof(bool);
		if(value == "i") return typeof(int);
		if(value == "f") return typeof(float);
		if(value == "v2") return typeof(Vector2);
		if(value == "v3") return typeof(Vector3);
		if(value == "v4") return typeof(Vector4);
		if(value == "o") return typeof(System.Object);
		if(value == "t&") return typeof(Type);
		if(value == "s&") return typeof(string);
		if(value == "b&") return typeof(bool);
		if(value == "i&") return typeof(int);
		if(value == "f&") return typeof(float);
		if(value == "v2&") return typeof(Vector2);
		if(value == "v3&") return typeof(Vector3);
		if(value == "v4&") return typeof(Vector4);
		if(value == "o&") return typeof(System.Object);
		if(value == "t[]") return typeof(Type[]);
		if(value == "s[]") return typeof(string[]);
		if(value == "b[]") return typeof(bool[]);
		if(value == "i[]") return typeof(int[]);
		if(value == "f[]") return typeof(float[]);
		if(value == "v2[]") return typeof(Vector2[]);
		if(value == "v3[]") return typeof(Vector3[]);
		if(value == "v4[]") return typeof(Vector4[]);
		if(value == "o[]") return typeof(System.Object[]);
		if(value == "iCS_RuntimeDesc") return typeof(iCS_RuntimeDesc);
		if(value == "iCS_ObjectTypeEnum") return typeof(iCS_ObjectTypeEnum);
		return Type.GetType(value);
    }
	// ----------------------------------------------------------------------
    bool DecodeBoolForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return false;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<int>(valueType)) {
            DecodeTypeError("bool", valueType);
            return false;
        }
        return DecodeBool(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    bool DecodeBool(string value) {
        return value == "T";
    }
	// ----------------------------------------------------------------------
    int DecodeIntForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return 0;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<int>(valueType)) {
            DecodeTypeError("int", valueType);
            return 0;
        }
        return DecodeInt(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    int DecodeInt(string value) {
        return (int)Convert.ChangeType(value, typeof(int));
    }
	// ----------------------------------------------------------------------
    float DecodeFloatForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return 0f;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<float>(valueType)) {
            DecodeTypeError("float", valueType);
            return 0f;
        }
        return DecodeFloat(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    float DecodeFloat(string value) {
        return (float)Convert.ChangeType(value, typeof(float));
    }
	// ----------------------------------------------------------------------
    string DecodeStringForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return null;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<string>(valueType)) {
            DecodeTypeError("string", valueType);
            return null;
        }
        return DecodeString(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    string DecodeString(string value) {
        return value;
    }
    
    // ======================================================================
    // Error processing.
	// ----------------------------------------------------------------------
    static void DecodeError(char expected, string encoded) {
    	Debug.LogWarning("iCanScript: Format error when decoding: expected '"+expected+"': received: "+encoded);    
    }
    static void DecodeTypeError(string expectedTypeName, Type type) {
        Debug.LogWarning("iCanScript: Type decoding error: desired type is: "+expectedTypeName+" Received: "+type.Name);
    }
}
