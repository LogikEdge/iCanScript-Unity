using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_Coder {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
	Dictionary<string,string>	myDictionary;
	
    // ======================================================================
    // Construction/Destruction
	// ----------------------------------------------------------------------
	public iCS_Coder() {
		myDictionary= new Dictionary<string,string>();
	}
	public iCS_Coder(string archive) : this() {
		Archive= archive;
	}

    // ======================================================================
    // Archiving
	// ----------------------------------------------------------------------
	public string Archive {
		get {
			string result= "{";
			foreach(var pair in myDictionary) {
				result+= "{"+BuildArchive(pair.Key)+BuildArchive(pair.Value)+"}";
			}
			return result+"}";
		}
		set {
            // Validate header
			if(value[0] != '{') { DecodeError('{', value); return; }
            value= value.Substring(1);
            while(value.Length > 1) {
    			if(value[0] != '{') { DecodeError('{', value); return; }
                value= value.Substring(1);
                string key= ExtractFromArchive(ref value);
                string keyValue= ExtractFromArchive(ref value);
                myDictionary.Add(key, keyValue);
    			if(value[0] != '}') { DecodeError('}', value); return; }
            }
            // Validate trailer
			if(value[0] != '}') { DecodeError('}', value); }
		}
	}
	// ----------------------------------------------------------------------
	string BuildArchive(string str) {
		return "{"+str.Length.ToString()+":"+str+"}";
	}
	// ----------------------------------------------------------------------
    string ExtractFromArchive(ref string value) {
        // Validate header
		if(value[0] != '{') { DecodeError('{', value); value= ""; return ""; }
        value= value.Substring(1);
        // Extract size.
        int end= value.IndexOf(':');
        if(end < 0) { DecodeError(':', value); value= ""; return ""; }
        int size= DecodeInt(value.Substring(0, end));
        value= value.Substring(end+1);
        // Extract string.
        string result= value.Substring(0, size);
        value= value.Substring(size+1);
        // Validate trailer
		if(value[0] != '}') { DecodeError('}', value); value= ""; return ""; }
		value= value.Substring(1);
		return result;
    }

    // ======================================================================
    // Encoding
	// ----------------------------------------------------------------------
	string Encode(object value) {
		Type valueType= value.GetType();
		iCS_Coder coder= new iCS_Coder();
		// Special case for arrays.
		if(valueType.IsArray) {
			Array array= value as Array;
			coder.Encode(array.Length, "Length");
			for(int i= 0; i < array.Length; ++i) {
				coder.Encode(array.GetValue(i), "["+i+"]");
			}
			return coder.Archive;
		}
		// Special case for enums.
		if(valueType.IsEnum) {
			coder.Encode((int)value, "Numeric");
			coder.Encode(value.ToString(), "Name");
			return coder.Archive;
        }
		// Primitives.
		if(value is string) return (string)value;
		if(value is bool)   return Encode((bool)value);
		if(value is int)    return Encode((int)value);
		if(value is float)  return Encode((float)value);
		// All other types.
		foreach(var field in valueType.GetFields()) {
            if(!field.IsStatic) {
    			coder.Encode(field.GetValue(value), field.Name);                
            }
		}
		return coder.Archive;
	}
	// ----------------------------------------------------------------------
	public void Encode(object value, string key) {
		Encode(Encode(value), key);
	}
	// ----------------------------------------------------------------------
	public void Encode(string value, string key) {
		if(myDictionary.ContainsKey(key)) {
			myDictionary[key]= value;
		} else {
			myDictionary.Add(key, value);
		}
	}
	// ----------------------------------------------------------------------
	string Encode(Type value) {
        return value.AssemblyQualifiedName;
	}
	// ----------------------------------------------------------------------
	public void Encode(Type value, string key) {
		Encode(Encode(value), key);
	}
	// ----------------------------------------------------------------------
	string Encode(bool value) {
		return (string)Convert.ChangeType(value, typeof(string));
	}
	// ----------------------------------------------------------------------
	public void Encode(bool value, string key) {
		Encode(Encode(value), key);
	}
	// ----------------------------------------------------------------------
	string Encode(int value) {
		return (string)Convert.ChangeType(value, typeof(string));
	}
	// ----------------------------------------------------------------------
	public void Encode(int value, string key) {
		Encode(Encode(value), key);
	}
	// ----------------------------------------------------------------------
	string Encode(float value) {
		return (string)Convert.ChangeType(value, typeof(string));
	}
	// ----------------------------------------------------------------------
	public void Encode(float value, string key) {
		Encode(Encode(value), key);
	}

    // ======================================================================
    // Decoding
	// ----------------------------------------------------------------------
    public object DecodeObjectForKey(string key, Type valueType) {
        if(!myDictionary.ContainsKey(key)) return null;
        string valueStr= myDictionary[key];
        iCS_Coder coder= new iCS_Coder();
        // Special case for arrays.
		if(valueType.IsArray) {
            Type arrayBaseType= iCS_Types.GetElementType(valueType);
            coder.Archive= valueStr;
            int len= coder.DecodeIntForKey("Length");
            Array array= Array.CreateInstance(arrayBaseType, len);
			for(int i= 0; i < array.Length; ++i) {
                array.SetValue(coder.DecodeObjectForKey("["+i+"]", arrayBaseType), i); // THIS IS A BUG.  INHERITANCE NOT SUPPORTED.
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
		if(elementType == typeof(string)) return DecodeString(valueStr);
		if(elementType == typeof(bool))   return DecodeBool(valueStr);
		if(elementType == typeof(int))    return DecodeInt(valueStr);
		if(elementType == typeof(float))  return DecodeFloat(valueStr);
		// All other types.
        coder.Archive= valueStr;
        object obj= iCS_Types.CreateInstance(valueType);
		foreach(var field in valueType.GetFields()) {
            if(!field.IsStatic) {
    			field.SetValue(obj, coder.DecodeObjectForKey(field.Name, field.FieldType));
            }
		}
        return obj;
    }
	// ----------------------------------------------------------------------
    bool DecodeBoolForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return false;
        return DecodeBool(myDictionary[key]);
    }
	// ----------------------------------------------------------------------
    bool DecodeBool(string value) {
        return (bool)Convert.ChangeType(value, typeof(bool));
    }
	// ----------------------------------------------------------------------
    int DecodeIntForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return 0;
        return DecodeInt(myDictionary[key]);
    }
	// ----------------------------------------------------------------------
    int DecodeInt(string value) {
        return (int)Convert.ChangeType(value, typeof(int));
    }
	// ----------------------------------------------------------------------
    float DecodeFloatForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return 0f;
        return DecodeFloat(myDictionary[key]);
    }
	// ----------------------------------------------------------------------
    float DecodeFloat(string value) {
        return (float)Convert.ChangeType(value, typeof(float));
    }
	// ----------------------------------------------------------------------
    string DecodeStringForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return null;
        return DecodeString(myDictionary[key]);
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
}
