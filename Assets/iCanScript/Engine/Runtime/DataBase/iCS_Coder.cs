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
    // Compression constants.
	// ----------------------------------------------------------------------
	const string DefaultTypeStr   = ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	const string AssemblyStr      = ", Assembly-";
	const string VersionStr       = ", Version=";
	const string CultureStr       = ", Culture=neutral";
	const string PublicKeyTokenStr= ", PublicKeyToken=";
	
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
                value= value.Substring(1);
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
			// special case for bool arrays.
			if(valueType == typeof(bool[])) {
				bool[] boolArray= value as bool[];
				string boolArrayStr= "";
				for(int i= 0; i < boolArray.Length; ++i) {
					boolArrayStr+= EncodeBool(boolArray[i]);
				}
				Add(key, valueType, boolArrayStr);
				return;
			}
			// special case for string arrays.
			if(valueType == typeof(string[])) {
				string[] stringArray= value as string[];
				string stringArrayStr= "";
				for(int i= 0; i < stringArray.Length; ++i) {
					string toEncode= stringArray[i];
					stringArrayStr+= EncodeInt(toEncode.Length)+":"+EncodeString(toEncode);
				}
				Add(key, valueType, stringArrayStr);
				return;
			}
			// special case for type arrays.
			if(valueType == typeof(Type[])) {
				Type[] typeArray= value as Type[];
				string typeArrayStr= "";
				for(int i= 0; i < typeArray.Length; ++i) {
					string encodedType= EncodeType(typeArray[i]);
					typeArrayStr+= EncodeInt(encodedType.Length)+":"+encodedType;
				}
				Add(key, valueType, typeArrayStr);
				return;
			}
			// special case for Vector arrays.
			if(valueType == typeof(Vector2[])) {
				Vector2[] v2Array= value as Vector2[];
				string v2ArrayStr= "";
				for(int i= 0; i < v2Array.Length; ++i) {
					string encodedV2= EncodeVector2(v2Array[i]);
					v2ArrayStr+= EncodeInt(encodedV2.Length)+":"+encodedV2;
				}
				Add(key, valueType, v2ArrayStr);
				return;
			}
			if(valueType == typeof(Vector3[])) {
				Vector3[] v3Array= value as Vector3[];
				string v3ArrayStr= "";
				for(int i= 0; i < v3Array.Length; ++i) {
					string encodedV3= EncodeVector3(v3Array[i]);
					v3ArrayStr+= EncodeInt(encodedV3.Length)+":"+encodedV3;
				}
				Add(key, valueType, v3ArrayStr);
				return;
			}
			if(valueType == typeof(Vector4[])) {
				Vector4[] v4Array= value as Vector4[];
				string v4ArrayStr= "";
				for(int i= 0; i < v4Array.Length; ++i) {
					string encodedV4= EncodeVector4(v4Array[i]);
					v4ArrayStr+= EncodeInt(encodedV4.Length)+":"+encodedV4;
				}
				Add(key, valueType, v4ArrayStr);
				return;
			}
			// Default for all other array type.
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
        if(value is Type)    { EncodeType(key, (Type)value); return; }
		if(value is string)  { EncodeString(key, (string)value); return; }
		if(value is bool)    { EncodeBool(key, (bool)value); return; }
		if(value is int)     { EncodeInt(key, (int)value); return; }
		if(value is float)   { EncodeFloat(key, (float)value); return; }
		if(value is Vector2) { EncodeVector2(key, (Vector2)value); return; }
		if(value is Vector3) { EncodeVector3(key, (Vector3)value); return; }
		if(value is Vector4) { EncodeVector4(key, (Vector4)value); return; }
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
        if(value == typeof(char)) return "c";
        if(value == typeof(Vector2)) return "v2";
        if(value == typeof(Vector3)) return "v3";
        if(value == typeof(Vector4)) return "v4";
        if(value == typeof(System.Object)) return "o";
		if(value == typeof(Type[])) return "t[]";
		if(value == typeof(string[])) return "s[]";
		if(value == typeof(bool[])) return "b[]";
		if(value == typeof(int[])) return "i[]";
		if(value == typeof(float[])) return "f[]";
        if(value == typeof(char[])) return "c[]";
        if(value == typeof(Vector2[])) return "v2[]";
        if(value == typeof(Vector3[])) return "v3[]";
        if(value == typeof(Vector4[])) return "v4[]";
        if(value == typeof(System.Object[])) return "o[]";
		if(value == typeof(iCS_RuntimeDesc)) return "iCS_RuntimeDesc";
		if(value == typeof(iCS_ObjectTypeEnum)) return "iCS_ObjectTypeEnum";
		if(value == typeof(iCS_Module)) return "iCS_Module";
		if(value == typeof(iCS_StateChart)) return "iCS_StateChart";
		if(value == typeof(iCS_State)) return "iCS_State";
        if(value.IsByRef) {
            Type elementType= value.GetElementType();
    		if(elementType == typeof(Type)) return "t&";
    		if(elementType == typeof(string)) return "s&";
    		if(elementType == typeof(bool)) return "b&";
    		if(elementType == typeof(int)) return "i&";
    		if(elementType == typeof(float)) return "f&";
            if(elementType == typeof(char)) return "c&";
            if(elementType == typeof(Vector2)) return "v2&";
            if(elementType == typeof(Vector3)) return "v3&";
            if(elementType == typeof(Vector4)) return "v4&";
            if(elementType == typeof(System.Object)) return "o&";
        }
		string typeAsString= value.AssemblyQualifiedName;
		// Try to compress type string.
		int defaultTypeIdx= typeAsString.IndexOf(DefaultTypeStr);
		if(defaultTypeIdx > 0) {
			return typeAsString.Substring(0, defaultTypeIdx)+"&&"+typeAsString.Substring(defaultTypeIdx+DefaultTypeStr.Length);
		}
		int assemblyIdx= typeAsString.IndexOf(AssemblyStr);
		if(assemblyIdx > 0) {
			typeAsString= typeAsString.Substring(0, assemblyIdx)+"&%"+typeAsString.Substring(assemblyIdx+AssemblyStr.Length);
		}
		int versionIdx= typeAsString.IndexOf(VersionStr);
		if(versionIdx > 0) {
			typeAsString= typeAsString.Substring(0,versionIdx)+"&!"+typeAsString.Substring(versionIdx+VersionStr.Length);
		}
		int cultureIdx= typeAsString.IndexOf(CultureStr);
		if(cultureIdx > 0) {
			typeAsString= typeAsString.Substring(0,cultureIdx)+"&$"+typeAsString.Substring(cultureIdx+CultureStr.Length);
		}
		int publicKeyIdx= typeAsString.IndexOf(PublicKeyTokenStr);
		if(publicKeyIdx > 0) {
			typeAsString= typeAsString.Substring(0,publicKeyIdx)+"&^"+typeAsString.Substring(publicKeyIdx+PublicKeyTokenStr.Length);
		}
		return typeAsString;
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
	// ----------------------------------------------------------------------
	string EncodeVector2(Vector2 value) {
		return (string)value.x.ToString()+","+value.y;
	}
	// ----------------------------------------------------------------------
	public void EncodeVector2(string key, Vector2 value) {
		Add(key, typeof(Vector2), EncodeVector2(value));
	}
	// ----------------------------------------------------------------------
	string EncodeVector3(Vector3 value) {
		return (string)value.x.ToString()+","+value.y+","+value.z;
	}
	// ----------------------------------------------------------------------
	public void EncodeVector3(string key, Vector3 value) {
		Add(key, typeof(Vector3), EncodeVector3(value));
	}
	// ----------------------------------------------------------------------
	string EncodeVector4(Vector4 value) {
		return (string)value.x.ToString()+","+value.y+","+value.z+","+value.w;
	}
	// ----------------------------------------------------------------------
	public void EncodeVector4(string key, Vector4 value) {
		Add(key, typeof(Vector4), EncodeVector4(value));
	}

    // ======================================================================
    // Decoding
	// ----------------------------------------------------------------------
    public object DecodeObjectForKey(string key) {
        if(!myDictionary.ContainsKey(key)) { return null; }
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        string valueStr= tuple.Item2;
        iCS_Coder coder= new iCS_Coder();
        // Special case for arrays.
		if(valueType.IsArray) {
            Type arrayBaseType= iCS_Types.GetElementType(valueType);
			// Special case for bool arrays.
			if(valueType == typeof(bool[])) {
				bool[] boolArray= new bool[valueStr.Length];
				for(int i= 0; i < boolArray.Length; ++i) {
	                boolArray[i]= DecodeBool(""+valueStr[i]);
				}
				return boolArray;
			}
			// special case for string arrays.
			if(valueType == typeof(string[])) {
				List<string> stringList= new List<string>();
				while(valueStr.Length > 0) {
					int end= valueStr.IndexOf(':');
					if(end < 0) { DecodeError(':', valueStr); return stringList.ToArray(); }
					int strLength= DecodeInt(valueStr.Substring(0, end));
					valueStr= valueStr.Substring(end+1);
					stringList.Add(DecodeString(valueStr.Substring(0, strLength)));
					valueStr= valueStr.Substring(strLength);
				}
				return stringList.ToArray();
			}
			// special case for type arrays.
			if(valueType == typeof(Type[])) {
				List<Type> typeList= new List<Type>();
				while(valueStr.Length > 0) {
					int end= valueStr.IndexOf(':');
					if(end < 0) { DecodeError(':', valueStr); return typeList.ToArray(); }
					int strLength= DecodeInt(valueStr.Substring(0, end));
					valueStr= valueStr.Substring(end+1);
					typeList.Add(DecodeType(valueStr.Substring(0, strLength)));
					valueStr= valueStr.Substring(strLength);
				}
				return typeList.ToArray();
			}
			// special case for vector arrays.
			if(valueType == typeof(Vector2[])) {
				List<Vector2> v2List= new List<Vector2>();
				while(valueStr.Length > 0) {
					int end= valueStr.IndexOf(':');
					if(end < 0) { DecodeError(':', valueStr); return v2List.ToArray(); }
					int strLength= DecodeInt(valueStr.Substring(0, end));
					valueStr= valueStr.Substring(end+1);
					v2List.Add(DecodeVector2(valueStr.Substring(0, strLength)));
					valueStr= valueStr.Substring(strLength);
				}
				return v2List.ToArray();
			}
			if(valueType == typeof(Vector3[])) {
				List<Vector3> v3List= new List<Vector3>();
				while(valueStr.Length > 0) {
					int end= valueStr.IndexOf(':');
					if(end < 0) { DecodeError(':', valueStr); return v3List.ToArray(); }
					int strLength= DecodeInt(valueStr.Substring(0, end));
					valueStr= valueStr.Substring(end+1);
					v3List.Add(DecodeVector3(valueStr.Substring(0, strLength)));
					valueStr= valueStr.Substring(strLength);
				}
				return v3List.ToArray();
			}
			if(valueType == typeof(Vector4[])) {
				List<Vector4> v4List= new List<Vector4>();
				while(valueStr.Length > 0) {
					int end= valueStr.IndexOf(':');
					if(end < 0) { DecodeError(':', valueStr); return v4List.ToArray(); }
					int strLength= DecodeInt(valueStr.Substring(0, end));
					valueStr= valueStr.Substring(end+1);
					v4List.Add(DecodeVector4(valueStr.Substring(0, strLength)));
					valueStr= valueStr.Substring(strLength);
				}
				return v4List.ToArray();
			}
			// All other types of arrays.
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
        if(elementType == typeof(Type))    return DecodeType(valueStr);
		if(elementType == typeof(string))  return DecodeString(valueStr);
		if(elementType == typeof(bool))    return DecodeBool(valueStr);
		if(elementType == typeof(int))     return DecodeInt(valueStr);
		if(elementType == typeof(float))   return DecodeFloat(valueStr);
		if(elementType == typeof(Vector2)) return DecodeVector2(valueStr);
		if(elementType == typeof(Vector3)) return DecodeVector3(valueStr);
		if(elementType == typeof(Vector4)) return DecodeVector4(valueStr);
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
        if(value == "c") return typeof(char);
		if(value == "v2") return typeof(Vector2);
		if(value == "v3") return typeof(Vector3);
		if(value == "v4") return typeof(Vector4);
		if(value == "o") return typeof(System.Object);
		if(value == "t&") return typeof(Type);
		if(value == "s&") return typeof(string);
		if(value == "b&") return typeof(bool);
		if(value == "i&") return typeof(int);
		if(value == "f&") return typeof(float);
        if(value == "c&") return typeof(char);
		if(value == "v2&") return typeof(Vector2);
		if(value == "v3&") return typeof(Vector3);
		if(value == "v4&") return typeof(Vector4);
		if(value == "o&") return typeof(System.Object);
		if(value == "t[]") return typeof(Type[]);
		if(value == "s[]") return typeof(string[]);
		if(value == "b[]") return typeof(bool[]);
		if(value == "i[]") return typeof(int[]);
		if(value == "f[]") return typeof(float[]);
        if(value == "c[]") return typeof(char[]);
		if(value == "v2[]") return typeof(Vector2[]);
		if(value == "v3[]") return typeof(Vector3[]);
		if(value == "v4[]") return typeof(Vector4[]);
		if(value == "o[]") return typeof(System.Object[]);
		if(value == "iCS_RuntimeDesc") return typeof(iCS_RuntimeDesc);
		if(value == "iCS_ObjectTypeEnum") return typeof(iCS_ObjectTypeEnum);
		if(value == "iCS_Module") return typeof(iCS_Module);
		if(value == "iCS_State") return typeof(iCS_State);
		if(value == "iCS_StateChart") return typeof(iCS_StateChart);
		// Decompress type string.
		int defaultTypeIdx= value.IndexOf("&&");
		if(defaultTypeIdx > 0) {
			return Type.GetType(value.Substring(0, defaultTypeIdx)+DefaultTypeStr+value.Substring(defaultTypeIdx+2));
		}
		int assemblyIdx= value.IndexOf("&%");
		if(assemblyIdx > 0) {
			value= value.Substring(0, assemblyIdx)+AssemblyStr+value.Substring(assemblyIdx+1);
		}
		int versionIdx= value.IndexOf("&!");
		if(versionIdx > 0) {
			value= value.Substring(0, versionIdx)+VersionStr+value.Substring(versionIdx+1);			
		}
		int cultureIdx= value.IndexOf("&$");
		if(cultureIdx > 0) {
			value= value.Substring(0, cultureIdx)+CultureStr+value.Substring(cultureIdx+1);			
		}
		int publicKeyIdx= value.IndexOf("&^");
		if(publicKeyIdx > 0) {
			value= value.Substring(0, publicKeyIdx)+PublicKeyTokenStr+value.Substring(publicKeyIdx+1);			
		}
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
	// ----------------------------------------------------------------------
    Vector2 DecodeVector2ForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return Vector2.zero;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<Vector2>(valueType)) {
            DecodeTypeError("Vector2", valueType);
            return Vector2.zero;
        }
        return DecodeVector2(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    Vector2 DecodeVector2(string value) {
		int end= value.IndexOf(',');
		if(end < 0) {
			DecodeError(',', value);
			return Vector2.zero;
		}
		float x= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
		float y= (float)Convert.ChangeType(value.Substring(end+1), typeof(float));
        return new Vector2(x,y);
    }
	// ----------------------------------------------------------------------
    Vector3 DecodeVector3ForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return Vector3.zero;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<Vector3>(valueType)) {
            DecodeTypeError("Vector3", valueType);
            return Vector3.zero;
        }
        return DecodeVector3(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    Vector3 DecodeVector3(string value) {
		int end= value.IndexOf(',');
		if(end < 0) {
			DecodeError(',', value);
			return Vector3.zero;
		}
		float x= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
		value= value.Substring(end+1);
		end= value.IndexOf(',');
		if(end < 0) {
			DecodeError(',', value);
			return Vector3.zero;			
		}
		float y= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
		float z= (float)Convert.ChangeType(value.Substring(end+1), typeof(float));
        return new Vector3(x,y,z);
    }
	// ----------------------------------------------------------------------
    Vector4 DecodeVector4ForKey(string key) {
        if(!myDictionary.ContainsKey(key)) return Vector4.zero;
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        if(!iCS_Types.IsA<Vector4>(valueType)) {
            DecodeTypeError("Vector4", valueType);
            return Vector4.zero;
        }
        return DecodeVector4(tuple.Item2);
    }
	// ----------------------------------------------------------------------
    Vector4 DecodeVector4(string value) {
		int end= value.IndexOf(',');
		if(end < 0) {
			DecodeError(',', value);
			return Vector4.zero;
		}
		float x= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
		value= value.Substring(end+1);
		end= value.IndexOf(',');
		if(end < 0) {
			DecodeError(',', value);
			return Vector4.zero;			
		}
		float y= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
		value= value.Substring(end+1);
		end= value.IndexOf(',');
		if(end < 0) {
			DecodeError(',', value);
			return Vector4.zero;			
		}
		float z= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
		float w= (float)Convert.ChangeType(value.Substring(end+1), typeof(float));
        return new Vector4(x,y,z,w);
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
