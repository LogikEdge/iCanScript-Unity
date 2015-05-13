using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal {
    
    public class iCS_Coder {    
        // ======================================================================
        // Fields
    	// ----------------------------------------------------------------------
    	Dictionary<string,Prelude.Tuple<string,string>>	myDictionary;

        // ======================================================================
        // Compression constants.
    	// ----------------------------------------------------------------------
    	const string CSharpTypeStr     = ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    	const string UnityEngineTypeStr= ", UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        const string DefaultTypeStr    = ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    	const string VersionStr        = ", Version=";
    	const string CultureStr        = ", Culture=neutral";
    	const string PublicKeyTokenStr = ", PublicKeyToken=";
	
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
    //		Debug.Log("Size= "+size+" String: "+value);
            string result= value.Substring(0, size);
            value= value.Substring(size);
    		return result;
        }

        // ======================================================================
        // Encoding
    	// ----------------------------------------------------------------------
    	void Add(string key, Type type, string valueStr) {
            Prelude.Tuple<string, string> value= new Prelude.Tuple<string, string>(EncodeType(type), valueStr);
    		if(myDictionary.ContainsKey(key)) {
    			myDictionary[key]= value;
    		} else {
    			myDictionary.Add(key, value);
    		}
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeObject(string key, object value, iCS_IVisualScriptData visualScript) {
            if(value == null) return;
    		Type valueType= value.GetType();
    		// Special case for arrays.
    		if(valueType.IsArray) {
                EncodeArrayOfObjects(key, value, visualScript);
                return;
    		}
    		// Special case for enums.
    		iCS_Coder coder= new iCS_Coder();
    		if(valueType.IsEnum) {
    			coder.EncodeInt("Numeric", (int)value);
    			coder.EncodeString("Name", value.ToString());
                Add(key, valueType, coder.Archive);
    			return;
            }
    		// Primitives.
            if(value is Type)               { EncodeType(key, (Type)value); return; }
    		if(value is byte)               { EncodeByte(key, (byte)value); return; }
    		if(value is sbyte)              { EncodeSByte(key, (sbyte)value); return; }
    		if(value is char)               { EncodeChar(key, (char)value); return; }
    		if(value is string)             { EncodeString(key, (string)value); return; }
    		if(value is bool)               { EncodeBool(key, (bool)value); return; }
    		if(value is int)                { EncodeInt(key, (int)value); return; }
    		if(value is uint)               { EncodeUInt(key, (uint)value); return; }
    		if(value is short)              { EncodeShort(key, (short)value); return; }
    		if(value is ushort)             { EncodeUShort(key, (ushort)value); return; }
    		if(value is long)               { EncodeLong(key, (long)value); return; }
    		if(value is ulong)              { EncodeULong(key, (ulong)value); return; }
    		if(value is float)              { EncodeFloat(key, (float)value); return; }
    		if(value is double)             { EncodeDouble(key, (double)value); return; }
    		if(value is decimal)            { EncodeDecimal(key, (decimal)value); return; }
    		if(value is Vector2)            { EncodeVector2(key, (Vector2)value); return; }
    		if(value is Vector3)            { EncodeVector3(key, (Vector3)value); return; }
    		if(value is Vector4)            { EncodeVector4(key, (Vector4)value); return; }
    		if(value is Color)              { EncodeColor(key, (Color)value); return; }
    		if(value is Quaternion)         { EncodeQuaternion(key, (Quaternion)value); return; }
    		if(value is Matrix4x4)          { EncodeMatrix4x4(key, (Matrix4x4)value); return; }
            // Special case for Unity Object inside a storage.
    		if(value is UnityEngine.Object && visualScript != null) {
                return;
            }
    		// All other types.
    		foreach(var field in valueType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                bool shouldEncode= true;
                if(field.IsPublic) {
                    foreach(var attribute in field.GetCustomAttributes(true)) {
                        if(attribute is System.NonSerializedAttribute) shouldEncode= false;
                    }
                } else {
                    shouldEncode= false;
                    foreach(var attribute in field.GetCustomAttributes(true)) {
                        if(attribute is SerializeField) shouldEncode= true;
                    }                
                }
                if(shouldEncode) {
        			coder.EncodeObject(field.Name, field.GetValue(value), visualScript);                                
                }
    		}
    		Add(key, valueType, coder.Archive);
    	}
    	// ----------------------------------------------------------------------
        void EncodeArrayOfObjects(string key, object value, iCS_IVisualScriptData visualScript) {
    		Type valueType= value.GetType();
    		// Special cases.
    		if(valueType == typeof(byte[])) {
                EncodeArrayOfChars(key, value as byte[], EncodeByte);
    			return;
    		}
    		if(valueType == typeof(sbyte[])) {
                EncodeArrayOfChars(key, value as sbyte[], EncodeSByte);
    			return;
    		}
    		if(valueType == typeof(char[])) {
                EncodeArrayOfChars(key, value as char[], EncodeChar);
    			return;
    		}
    		if(valueType == typeof(bool[])) {
                EncodeArrayOfChars(key, value as bool[], EncodeBool);
    			return;
    		}
    		if(valueType == typeof(int[])) {
                EncodeArrayOfNumerics(key, value as int[], EncodeInt);
    			return;
    		}
    		if(valueType == typeof(uint[])) {
                EncodeArrayOfNumerics(key, value as uint[], EncodeUInt);
    			return;
    		}
    		if(valueType == typeof(short[])) {
                EncodeArrayOfNumerics(key, value as short[], EncodeShort);
    			return;
    		}
    		if(valueType == typeof(ushort[])) {
                EncodeArrayOfNumerics(key, value as ushort[], EncodeUShort);
    			return;
    		}
    		if(valueType == typeof(long[])) {
                EncodeArrayOfNumerics(key, value as long[], EncodeLong);
    			return;
    		}
    		if(valueType == typeof(ulong[])) {
                EncodeArrayOfNumerics(key, value as ulong[], EncodeULong);
    			return;
    		}
    		if(valueType == typeof(float[])) {
                EncodeArrayOfNumerics(key, value as float[], EncodeFloat);
    			return;
    		}
    		if(valueType == typeof(double[])) {
                EncodeArrayOfNumerics(key, value as double[], EncodeDouble);
    			return;
    		}
    		if(valueType == typeof(decimal[])) {
                EncodeArrayOfNumerics(key, value as decimal[], EncodeDecimal);
    			return;
    		}
    		if(valueType == typeof(Vector2[])) {
                EncodeArrayOfNumerics(key, value as Vector2[], EncodeVector2);
    			return;
    		}
    		if(valueType == typeof(Vector3[])) {
                EncodeArrayOfNumerics(key, value as Vector3[], EncodeVector3);
    			return;
    		}
    		if(valueType == typeof(Vector4[])) {
                EncodeArrayOfNumerics(key, value as Vector4[], EncodeVector4);
    			return;
    		}
    		if(valueType == typeof(Quaternion[])) {
                EncodeArrayOfNumerics(key, value as Quaternion[], EncodeQuaternion);
    			return;
    		}
    		if(valueType == typeof(Color[])) {
                EncodeArrayOfNumerics(key, value as Color[], EncodeColor);
    			return;
    		}
    		if(valueType == typeof(string[])) {
                EncodeArray(key, value as string[], EncodeString);
    			return;
    		}
    		if(valueType == typeof(Type[])) {
                EncodeArray(key, value as Type[], EncodeType);
    			return;
    		}
    		// Default for all other array type.
    		Array array= value as Array;
    		iCS_Coder coder= new iCS_Coder();
    		coder.EncodeInt("Length", array.Length);
    		for(int i= 0; i < array.Length; ++i) {
    			coder.EncodeObject(i.ToString(), array.GetValue(i), visualScript);
    		}
            Add(key, valueType, coder.Archive);				
        }
        void EncodeArray<T>(string key, T[] array, Func<T,string> encoder) {
    		string arrayStr= "";
    		for(int i= 0; i < array.Length; ++i) {
    			string encoded= encoder(array[i]);
    			arrayStr+= EncodeInt(encoded.Length)+":"+encoded;
    		}
    		Add(key, typeof(T[]), arrayStr);        
        }
        void EncodeArrayOfNumerics<T>(string key, T[] array, Func<T,string> encoder) {
    		string arrayStr= "";
    		for(int i= 0; i < array.Length; ++i) {
    			arrayStr+= encoder(array[i] != null ? array[i] : default(T))+":";
    		}
    		Add(key, typeof(T[]), arrayStr);        
        }
        void EncodeArrayOfChars<T>(string key, T[] array, Func<T,string> encoder) {
    		string arrayStr= "";
    		for(int i= 0; i < array.Length; ++i) {
    			arrayStr+= encoder(array[i] != null ? array[i] : default(T));
    		}
    		Add(key, typeof(T[]), arrayStr);
        }
    	// ----------------------------------------------------------------------
    	string EncodeType(Type value) {
    		if(value == null) return "";
            // C# data types.
    		if(value == typeof(Type)) return "t";
    		if(value == typeof(string)) return "S";
    		if(value == typeof(bool)) return "b";
    		if(value == typeof(int)) return "i";
            if(value == typeof(uint)) return "u";
            if(value == typeof(long)) return "l";
            if(value == typeof(ulong)) return "ul";
            if(value == typeof(short)) return "s";
            if(value == typeof(ushort)) return "us";
    		if(value == typeof(float)) return "f";
    		if(value == typeof(double)) return "d";
            if(value == typeof(char)) return "c";
            if(value == typeof(byte)) return "B";
            if(value == typeof(System.Object)) return "o";
    		if(value == typeof(Type[])) return "t[]";
    		if(value == typeof(string[])) return "S[]";
    		if(value == typeof(bool[])) return "b[]";
    		if(value == typeof(int[])) return "i[]";
            if(value == typeof(uint[])) return "u[]";
            if(value == typeof(long[])) return "l[]";
            if(value == typeof(ulong)) return "ul[]";
            if(value == typeof(short)) return "s[]";
            if(value == typeof(ushort)) return "us[]";
    		if(value == typeof(float[])) return "f[]";
    		if(value == typeof(double[])) return "d[]";
            if(value == typeof(char[])) return "c[]";
            if(value == typeof(byte[])) return "B[]";
            if(value == typeof(System.Object[])) return "o[]";
            // Unity data types.
            if(value == typeof(Vector2)) return "v2";
            if(value == typeof(Vector3)) return "v3";
            if(value == typeof(Vector4)) return "v4";
            if(value == typeof(Color)) return "clr";
            if(value == typeof(Quaternion)) return "q";
            if(value == typeof(Matrix4x4)) return "m";
            if(value == typeof(UnityEngine.Object)) return "O";
            if(value == typeof(Vector2[])) return "v2[]";
            if(value == typeof(Vector3[])) return "v3[]";
            if(value == typeof(Vector4[])) return "v4[]";
            if(value == typeof(Color[])) return "clr[]";
            if(value == typeof(Quaternion[])) return "q[]";
            if(value == typeof(Matrix4x4[])) return "m[]";
            if(value == typeof(UnityEngine.Object[])) return "O[]";

    		string typeAsString= value.AssemblyQualifiedName;
    		// Try to compress type string.
    		int cSharpTypeIdx= typeAsString.IndexOf(CSharpTypeStr);
    		if(cSharpTypeIdx > 0) {
    			return typeAsString.Substring(0, cSharpTypeIdx)+"!!"+typeAsString.Substring(cSharpTypeIdx+CSharpTypeStr.Length);
    		}
    		int unityEngineTypeIdx= typeAsString.IndexOf(UnityEngineTypeStr);
    		if(unityEngineTypeIdx > 0) {
    			return typeAsString.Substring(0, unityEngineTypeIdx)+"!#"+typeAsString.Substring(unityEngineTypeIdx+UnityEngineTypeStr.Length);
    		}
    		int defaultTypeIdx= typeAsString.IndexOf(DefaultTypeStr);
    		if(defaultTypeIdx > 0) {
    			typeAsString= typeAsString.Substring(0, defaultTypeIdx)+"!%"+typeAsString.Substring(defaultTypeIdx+DefaultTypeStr.Length);
    		}
    		int versionIdx= typeAsString.IndexOf(VersionStr);
    		if(versionIdx > 0) {
    			typeAsString= typeAsString.Substring(0,versionIdx)+"!&"+typeAsString.Substring(versionIdx+VersionStr.Length);
    		}
    		int cultureIdx= typeAsString.IndexOf(CultureStr);
    		if(cultureIdx > 0) {
    			typeAsString= typeAsString.Substring(0,cultureIdx)+"!$"+typeAsString.Substring(cultureIdx+CultureStr.Length);
    		}
    		int publicKeyIdx= typeAsString.IndexOf(PublicKeyTokenStr);
    		if(publicKeyIdx > 0) {
    			typeAsString= typeAsString.Substring(0,publicKeyIdx)+"!^"+typeAsString.Substring(publicKeyIdx+PublicKeyTokenStr.Length);
    		}
    		return typeAsString;
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeType(string key, Type value) {
    		Add(key, typeof(Type), EncodeType(value));
    	}
    	// ----------------------------------------------------------------------
        string EncodeByte(byte value) {
            return ""+(char)(0x100+value);
        }
    	// ----------------------------------------------------------------------
        public void EncodeByte(string key, byte value) {
            Add(key, typeof(byte), EncodeByte(value));
        }
    	// ----------------------------------------------------------------------
        string EncodeSByte(sbyte value) {
            return ""+(char)(0x100+value);
        }
    	// ----------------------------------------------------------------------
        public void EncodeSByte(string key, sbyte value) {
            Add(key, typeof(sbyte), EncodeSByte(value));
        }
    	// ----------------------------------------------------------------------
        string EncodeChar(char value) {
            return ""+(char)(0x100+value);
        }
    	// ----------------------------------------------------------------------
        public void EncodeChar(string key, char value) {
            Add(key, typeof(char), EncodeChar(value));
        }
    	// ----------------------------------------------------------------------
        string EncodeString(string value) {
            return value == null ? "" : value;
        }
    	// ----------------------------------------------------------------------
        public void EncodeString(string key, string value) {
            Add(key, typeof(string), EncodeString(value));
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
    	string EncodeUInt(uint value) {
    		return (string)Convert.ChangeType(value, typeof(string));
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeUInt(string key, uint value) {
    		Add(key, typeof(uint), EncodeUInt(value));
    	}
    	// ----------------------------------------------------------------------
    	string EncodeShort(short value) {
    		return (string)Convert.ChangeType(value, typeof(string));
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeShort(string key, short value) {
    		Add(key, typeof(short), EncodeShort(value));
    	}
    	// ----------------------------------------------------------------------
    	string EncodeUShort(ushort value) {
    		return (string)Convert.ChangeType(value, typeof(string));
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeUShort(string key, ushort value) {
    		Add(key, typeof(ushort), EncodeUShort(value));
    	}
    	// ----------------------------------------------------------------------
    	string EncodeLong(long value) {
    		return (string)Convert.ChangeType(value, typeof(string));
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeLong(string key, long value) {
    		Add(key, typeof(long), EncodeLong(value));
    	}
    	// ----------------------------------------------------------------------
    	string EncodeULong(ulong value) {
    		return (string)Convert.ChangeType(value, typeof(string));
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeULong(string key, ulong value) {
    		Add(key, typeof(ulong), EncodeULong(value));
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
    	string EncodeDouble(double value) {
    		return (string)Convert.ChangeType(value, typeof(string));
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeDouble(string key, double value) {
    		Add(key, typeof(double), EncodeDouble(value));
    	}
    	// ----------------------------------------------------------------------
    	string EncodeDecimal(decimal value) {
    		return (string)Convert.ChangeType(value, typeof(string));
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeDecimal(string key, decimal value) {
    		Add(key, typeof(decimal), EncodeDecimal(value));
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
    	// ----------------------------------------------------------------------
    	string EncodeQuaternion(Quaternion value) {
    		return (string)value.x.ToString()+","+value.y+","+value.z+","+value.w;
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeQuaternion(string key, Quaternion value) {
    		Add(key, typeof(Quaternion), EncodeQuaternion(value));
    	}
    	// ----------------------------------------------------------------------
    	string EncodeMatrix4x4(Matrix4x4 value) {
            string result= value[0].ToString();
            for(int i= 1; i < 16; ++i) result+= ","+value[i];
    		return result;
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeMatrix4x4(string key, Matrix4x4 value) {
    		Add(key, typeof(Matrix4x4), EncodeMatrix4x4(value));
    	}
    	// ----------------------------------------------------------------------
    	string EncodeColor(Color value) {
    		return (string)value.r.ToString()+","+value.g+","+value.b+","+value.a;
    	}
    	// ----------------------------------------------------------------------
    	public void EncodeColor(string key, Color value) {
    		Add(key, typeof(Color), EncodeColor(value));
    	}
    
        // ======================================================================
        // Decoding
    	// ----------------------------------------------------------------------
        public object DecodeObjectForKey(string key, iCS_IVisualScriptData visualScript) {
            if(!myDictionary.ContainsKey(key)) { return null; }
            Prelude.Tuple<string,string> tuple= myDictionary[key];
            Type valueType= DecodeType(tuple.Item1);
            // Return "null" if type no longer available in assembly.
            if(valueType == null) return null;
            string valueStr= tuple.Item2;
            // Special case for arrays.
    		if(valueType.IsArray) {
                return DecodeArrayOfObjects(valueType, valueStr, visualScript);
    		}
    		// Special case for enums.
            iCS_Coder coder= new iCS_Coder();
    		if(valueType.IsEnum) {
                coder.Archive= valueStr;
                return Enum.ToObject(valueType, coder.DecodeIntForKey("Numeric"));
            }
    		// Primitives.
    		Type elementType= iCS_Types.GetElementType(valueType);
            if(elementType == typeof(Type))                    return DecodeType(valueStr);
    		if(elementType == typeof(byte))                    return DecodeByte(valueStr);
    		if(elementType == typeof(sbyte))                   return DecodeSByte(valueStr);
    		if(elementType == typeof(char))                    return DecodeChar(valueStr);
    		if(elementType == typeof(string))                  return DecodeString(valueStr);
    		if(elementType == typeof(bool))                    return DecodeBool(valueStr);
    		if(elementType == typeof(int))                     return DecodeInt(valueStr);
    		if(elementType == typeof(uint))                    return DecodeUInt(valueStr);
    		if(elementType == typeof(short))                   return DecodeShort(valueStr);
    		if(elementType == typeof(ushort))                  return DecodeUShort(valueStr);
    		if(elementType == typeof(long))                    return DecodeLong(valueStr);
    		if(elementType == typeof(ulong))                   return DecodeULong(valueStr);
    		if(elementType == typeof(float))                   return DecodeFloat(valueStr);
    		if(elementType == typeof(double))                  return DecodeDouble(valueStr);
    		if(elementType == typeof(decimal))                 return DecodeDecimal(valueStr);
    		if(elementType == typeof(Vector2))                 return DecodeVector2(valueStr);
    		if(elementType == typeof(Vector3))                 return DecodeVector3(valueStr);
    		if(elementType == typeof(Vector4))                 return DecodeVector4(valueStr);
    		if(elementType == typeof(Color))                   return DecodeColor(valueStr);
    		if(elementType == typeof(Quaternion))              return DecodeQuaternion(valueStr);
    		if(elementType == typeof(Matrix4x4))               return DecodeMatrix4x4(valueStr);
            // Special case for Unity Objects within a storage.
    		if(iCS_Types.IsA<UnityEngine.Object>(elementType) && visualScript != null) {
                return null;
            }
    		// All other types.
            coder.Archive= valueStr;
            object obj= iCS_Types.CreateInstance(valueType);
    		foreach(var field in valueType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                bool shouldDecode= true;
                if(field.IsPublic) {
                    foreach(var attribute in field.GetCustomAttributes(true)) {
                        if(attribute is System.NonSerializedAttribute) shouldDecode= false;
                    }
                } else {
                    shouldDecode= false;
                    foreach(var attribute in field.GetCustomAttributes(true)) {
                        if(attribute is SerializeField) shouldDecode= true;
                    }                
                }
                if(shouldDecode) {
        			field.SetValue(obj, coder.DecodeObjectForKey(field.Name, visualScript));                
                }
    		}
            return obj;
        }
    	// ----------------------------------------------------------------------
        object DecodeArrayOfObjects(Type valueType, string valueStr, iCS_IVisualScriptData visualScript) {
            Type arrayBaseType= iCS_Types.GetElementType(valueType);
    		// Special cases.
    		if(valueType == typeof(byte[])) {
                return DecodeArrayOfChars<byte>(valueStr, DecodeByte);
    		}
    		if(valueType == typeof(sbyte[])) {
                return DecodeArrayOfChars<sbyte>(valueStr, DecodeSByte);
    		}
    		if(valueType == typeof(char[])) {
                return DecodeArrayOfChars<char>(valueStr, DecodeChar);
    		}
    		if(valueType == typeof(bool[])) {
                return DecodeArrayOfChars<bool>(valueStr, DecodeBool);
    		}
    		if(valueType == typeof(int[])) {
                return DecodeArrayOfNumerics<int>(valueStr, DecodeInt);
    		}
    		if(valueType == typeof(uint[])) {
                return DecodeArrayOfNumerics<uint>(valueStr, DecodeUInt);
    		}
    		if(valueType == typeof(short[])) {
                return DecodeArrayOfNumerics<short>(valueStr, DecodeShort);
    		}
    		if(valueType == typeof(ushort[])) {
                return DecodeArrayOfNumerics<ushort>(valueStr, DecodeUShort);
    		}
    		if(valueType == typeof(long[])) {
                return DecodeArrayOfNumerics<long>(valueStr, DecodeLong);
    		}
    		if(valueType == typeof(ulong[])) {
                return DecodeArrayOfNumerics<ulong>(valueStr, DecodeULong);
    		}
    		if(valueType == typeof(float[])) {
                return DecodeArrayOfNumerics<float>(valueStr, DecodeFloat);
    		}
    		if(valueType == typeof(double[])) {
                return DecodeArrayOfNumerics<double>(valueStr, DecodeDouble);
    		}
    		if(valueType == typeof(decimal[])) {
                return DecodeArrayOfNumerics<decimal>(valueStr, DecodeDecimal);
    		}
    		if(valueType == typeof(Vector2[])) {
                return DecodeArrayOfNumerics<Vector2>(valueStr, DecodeVector2);
    		}
    		if(valueType == typeof(Vector3[])) {
                return DecodeArrayOfNumerics<Vector3>(valueStr, DecodeVector3);
    		}
    		if(valueType == typeof(Vector4[])) {
                return DecodeArrayOfNumerics<Vector4>(valueStr, DecodeVector4);
    		}
    		if(valueType == typeof(Quaternion[])) {
                return DecodeArrayOfNumerics<Quaternion>(valueStr, DecodeQuaternion);
    		}
    		if(valueType == typeof(Color[])) {
                return DecodeArrayOfNumerics<Color>(valueStr, DecodeColor);
    		}
    		if(valueType == typeof(string[])) {
                return DecodeArray<string>(valueStr, DecodeString);
    		}
    		if(valueType == typeof(Type[])) {
                return DecodeArray<Type>(valueStr, DecodeType);
    		}
    		// All other types of arrays.
            iCS_Coder coder= new iCS_Coder();
            coder.Archive= valueStr;
            int len= coder.DecodeIntForKey("Length");
            Array array= Array.CreateInstance(arrayBaseType, len);
    		for(int i= 0; i < len; ++i) {
                array.SetValue(coder.DecodeObjectForKey(i.ToString(), visualScript), i);
    		}
    		return array;						
        }
        Array DecodeArray<T>(string valueStr, Func<string,T> decoder) {
    		List<T> list= new List<T>();
    		while(valueStr.Length > 0) {
    			int end= valueStr.IndexOf(':');
    			if(end < 0) { DecodeError(':', valueStr); return list.ToArray(); }
    			int strLength= DecodeInt(valueStr.Substring(0, end));
    			valueStr= valueStr.Substring(end+1);
    			list.Add(decoder(valueStr.Substring(0, strLength)));
    			valueStr= valueStr.Substring(strLength);
    		}
    		return list.ToArray();
        }
        Array DecodeArrayOfNumerics<T>(string valueStr, Func<string,T> decoder) {
    		List<T> list= new List<T>();
    		while(valueStr.Length > 0) {
    			int end= valueStr.IndexOf(':');
    			if(end < 0) { DecodeError(':', valueStr); return list.ToArray(); }
    			list.Add(decoder(valueStr.Substring(0, end)));
    			valueStr= valueStr.Substring(end+1);
    		}
    		return list.ToArray();
        }
        Array DecodeArrayOfChars<T>(string valueStr, Func<string,T> decoder) {
    		T[] array= new T[valueStr.Length];
    		for(int i= 0; i < array.Length; ++i) {
                array[i]= decoder(""+valueStr[i]);
    		}
    		return array;
        }
    	// ----------------------------------------------------------------------
        public Type DecodeTypeForKey(string key) {
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
    		if(value.Length == 0) return null;
            // C# data types
    		if(value == "t") return typeof(Type);
    		if(value == "S") return typeof(string);
    		if(value == "b") return typeof(bool);
    		if(value == "i") return typeof(int);
    		if(value == "u") return typeof(uint);
    		if(value == "l") return typeof(long);
    		if(value == "ul") return typeof(ulong);
    		if(value == "s") return typeof(short);
    		if(value == "us") return typeof(ushort);		
    		if(value == "f") return typeof(float);
    		if(value == "d") return typeof(double);
            if(value == "c") return typeof(char);
            if(value == "B") return typeof(byte);
    		if(value == "o") return typeof(System.Object);
    		if(value == "t[]") return typeof(Type[]);
    		if(value == "S[]") return typeof(string[]);
    		if(value == "b[]") return typeof(bool[]);
    		if(value == "i[]") return typeof(int[]);
    		if(value == "u[]") return typeof(uint[]);
    		if(value == "l[]") return typeof(long[]);
    		if(value == "ul[]") return typeof(ulong[]);
    		if(value == "s[]") return typeof(short[]);
    		if(value == "us[]") return typeof(ushort[]);		
    		if(value == "f[]") return typeof(float[]);
    		if(value == "d[]") return typeof(double[]);
            if(value == "c[]") return typeof(char[]);
            if(value == "B[]") return typeof(byte[]);
    		if(value == "o[]") return typeof(System.Object[]);
            // Unity data types
    		if(value == "v2") return typeof(Vector2);
    		if(value == "v3") return typeof(Vector3);
    		if(value == "v4") return typeof(Vector4);
            if(value == "clr") return typeof(Color);
            if(value == "q") return typeof(Quaternion);
            if(value == "m") return typeof(Matrix4x4);
            if(value == "O") return typeof(UnityEngine.Object);
    		if(value == "v2[]") return typeof(Vector2[]);
    		if(value == "v3[]") return typeof(Vector3[]);
    		if(value == "v4[]") return typeof(Vector4[]);
            if(value == "clr[]") return typeof(Color[]);
            if(value == "q[]") return typeof(Quaternion);
            if(value == "m[]") return typeof(Matrix4x4);
            if(value == "O[]") return typeof(UnityEngine.Object);
    		// Decompress type string.
    		int cSharpTypeIdx= value.IndexOf("!!");
    		if(cSharpTypeIdx > 0) {
    			return iCS_Types.TypeFromAssemblyQualifiedName(value.Substring(0, cSharpTypeIdx)+CSharpTypeStr+value.Substring(cSharpTypeIdx+2));
    		}
    		int unityEngineTypeIdx= value.IndexOf("!#");
    		if(unityEngineTypeIdx > 0) {
    			return iCS_Types.TypeFromAssemblyQualifiedName(value.Substring(0, unityEngineTypeIdx)+UnityEngineTypeStr+value.Substring(unityEngineTypeIdx+2));
    		}
    		int defaultTypeIdx= value.IndexOf("!%");
    		if(defaultTypeIdx > 0) {
    			value= value.Substring(0, defaultTypeIdx)+DefaultTypeStr+value.Substring(defaultTypeIdx+2);
    		}
    		int versionIdx= value.IndexOf("!&");
    		if(versionIdx > 0) {
    			value= value.Substring(0, versionIdx)+VersionStr+value.Substring(versionIdx+2);			
    		}
    		int cultureIdx= value.IndexOf("!$");
    		if(cultureIdx > 0) {
    			value= value.Substring(0, cultureIdx)+CultureStr+value.Substring(cultureIdx+2);			
    		}
    		int publicKeyIdx= value.IndexOf("!^");
    		if(publicKeyIdx > 0) {
    			value= value.Substring(0, publicKeyIdx)+PublicKeyTokenStr+value.Substring(publicKeyIdx+2);			
    		}
    		return iCS_Types.TypeFromAssemblyQualifiedName(value);
        }
    	// ----------------------------------------------------------------------
        public bool DecodeBoolForKey(string key) {
            return DecodeForKey<bool>(key, DecodeBool);
        }
    	// ----------------------------------------------------------------------
        bool DecodeBool(string value) {
            return value == "T";
        }
    	// ----------------------------------------------------------------------
        public int DecodeIntForKey(string key) {
            return DecodeForKey<int>(key, DecodeInt);
        }
    	// ----------------------------------------------------------------------
        int DecodeInt(string value) {
            return (int)Convert.ChangeType(value, typeof(int));
        }
    	// ----------------------------------------------------------------------
        public uint DecodeUIntForKey(string key) {
            return DecodeForKey<uint>(key, DecodeUInt);
        }
    	// ----------------------------------------------------------------------
        uint DecodeUInt(string value) {
            return (uint)Convert.ChangeType(value, typeof(uint));
        }
    	// ----------------------------------------------------------------------
        public short DecodeShortForKey(string key) {
            return DecodeForKey<short>(key, DecodeShort);
        }
    	// ----------------------------------------------------------------------
        short DecodeShort(string value) {
            return (short)Convert.ChangeType(value, typeof(short));
        }
    	// ----------------------------------------------------------------------
        public ushort DecodeUShortForKey(string key) {
            return DecodeForKey<ushort>(key, DecodeUShort);
        }
    	// ----------------------------------------------------------------------
        ushort DecodeUShort(string value) {
            return (ushort)Convert.ChangeType(value, typeof(ushort));
        }
    	// ----------------------------------------------------------------------
        public long DecodeLongForKey(string key) {
            return DecodeForKey<long>(key, DecodeLong);
        }
    	// ----------------------------------------------------------------------
        long DecodeLong(string value) {
            return (long)Convert.ChangeType(value, typeof(long));
        }
    	// ----------------------------------------------------------------------
        public ulong DecodeULongForKey(string key) {
            return DecodeForKey<ulong>(key, DecodeULong);
        }
    	// ----------------------------------------------------------------------
        ulong DecodeULong(string value) {
            return (ulong)Convert.ChangeType(value, typeof(ulong));
        }
    	// ----------------------------------------------------------------------
        public float DecodeFloatForKey(string key) {
            return DecodeForKey<float>(key, DecodeFloat);
        }
    	// ----------------------------------------------------------------------
        float DecodeFloat(string value) {
            return (float)Convert.ChangeType(value, typeof(float));
        }
    	// ----------------------------------------------------------------------
        public double DecodeDoubleForKey(string key) {
            return DecodeForKey<double>(key, DecodeDouble);
        }
    	// ----------------------------------------------------------------------
        double DecodeDouble(string value) {
            return (double)Convert.ChangeType(value, typeof(double));
        }
    	// ----------------------------------------------------------------------
        public decimal DecodeDecimalForKey(string key) {
            return DecodeForKey<decimal>(key, DecodeDecimal);
        }
    	// ----------------------------------------------------------------------
        decimal DecodeDecimal(string value) {
            return (decimal)Convert.ChangeType(value, typeof(decimal));
        }
    	// ----------------------------------------------------------------------
        public byte DecodeByteForKey(string key) {
            return DecodeForKey<byte>(key, DecodeByte);
        }
    	// ----------------------------------------------------------------------
        byte DecodeByte(string value) {
            return (byte)(value[0]-0x100);
        }
    	// ----------------------------------------------------------------------
        public sbyte DecodeSByteForKey(string key) {
            return DecodeForKey<sbyte>(key, DecodeSByte);
        }
    	// ----------------------------------------------------------------------
        sbyte DecodeSByte(string value) {
            return (sbyte)(value[0]-0x100);
        }
    	// ----------------------------------------------------------------------
        public char DecodeCharForKey(string key) {
            return DecodeForKey<char>(key, DecodeChar);
        }
    	// ----------------------------------------------------------------------
        char DecodeChar(string value) {
            return (char)(value[0]-0x100);
        }
    	// ----------------------------------------------------------------------
        public string DecodeStringForKey(string key) {
            return DecodeForKey<string>(key, DecodeString);
        }
    	// ----------------------------------------------------------------------
        string DecodeString(string value) {
            return value;
        }
    	// ----------------------------------------------------------------------
        public Vector2 DecodeVector2ForKey(string key) {
            return DecodeForKey<Vector2>(key, DecodeVector2);
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
        public Vector3 DecodeVector3ForKey(string key) {
            return DecodeForKey<Vector3>(key, DecodeVector3);
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
        public Vector4 DecodeVector4ForKey(string key) {
            return DecodeForKey<Vector4>(key, DecodeVector4);
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
    	// ----------------------------------------------------------------------
        public Quaternion DecodeQuaternionForKey(string key) {
            return DecodeForKey<Quaternion>(key, DecodeQuaternion);
        }
    	// ----------------------------------------------------------------------
        Quaternion DecodeQuaternion(string value) {
    		int end= value.IndexOf(',');
    		if(end < 0) {
    			DecodeError(',', value);
    			return Quaternion.identity;
    		}
    		float x= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
    		value= value.Substring(end+1);
    		end= value.IndexOf(',');
    		if(end < 0) {
    			DecodeError(',', value);
    			return Quaternion.identity;			
    		}
    		float y= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
    		value= value.Substring(end+1);
    		end= value.IndexOf(',');
    		if(end < 0) {
    			DecodeError(',', value);
    			return Quaternion.identity;			
    		}
    		float z= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
    		float w= (float)Convert.ChangeType(value.Substring(end+1), typeof(float));
            return new Quaternion(x,y,z,w);
        }
    	// ----------------------------------------------------------------------
        public Matrix4x4 DecodeMatrix4x4ForKey(string key) {
            return DecodeForKey<Matrix4x4>(key, DecodeMatrix4x4);
        }
    	// ----------------------------------------------------------------------
        Matrix4x4 DecodeMatrix4x4(string value) {
            Matrix4x4 result= new Matrix4x4();
            for(int i= 0; i < 15; ++i) {
        		int end= value.IndexOf(',');
        		if(end < 0) {
        			DecodeError(',', value);
        			return Matrix4x4.identity;
        		}
        		result[i]= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
        		value= value.Substring(end+1);
            }
    		result[15]= (float)Convert.ChangeType(value, typeof(float));
            return result;
        }
    	// ----------------------------------------------------------------------
        public Color DecodeColorForKey(string key) {
            return DecodeForKey<Color>(key, DecodeColor);
        }
    	// ----------------------------------------------------------------------
        Color DecodeColor(string value) {
    		int end= value.IndexOf(',');
    		if(end < 0) {
    			DecodeError(',', value);
    			return Color.black;
    		}
    		float r= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
    		value= value.Substring(end+1);
    		end= value.IndexOf(',');
    		if(end < 0) {
    			DecodeError(',', value);
    			return Color.black;			
    		}
    		float g= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
    		value= value.Substring(end+1);
    		end= value.IndexOf(',');
    		if(end < 0) {
    			DecodeError(',', value);
    			return Color.black;			
    		}
    		float b= (float)Convert.ChangeType(value.Substring(0, end), typeof(float));
    		float a= (float)Convert.ChangeType(value.Substring(end+1), typeof(float));
            return new Color(r,g,b,a);
        }
    	// ----------------------------------------------------------------------
        T DecodeForKey<T>(string key, Func<string,T> decoder) {
            if(!myDictionary.ContainsKey(key)) return default(T);
            Prelude.Tuple<string,string> tuple= myDictionary[key];
            Type valueType= DecodeType(tuple.Item1);
            if(!iCS_Types.IsA<T>(valueType)) {
                DecodeTypeError(typeof(T).Name, valueType);
                return default(T);
            }
            return decoder(tuple.Item2);        
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

}
