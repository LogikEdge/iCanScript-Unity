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
		// Special case for arrays.
		if(valueType.IsArray) {
            EncodeArrayOfObjects(key, value);
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
        if(value is Type)    { EncodeType(key, (Type)value); return; }
		if(value is byte)    { EncodeByte(key, (byte)value); return; }
		if(value is sbyte)   { EncodeSByte(key, (sbyte)value); return; }
		if(value is char)    { EncodeChar(key, (char)value); return; }
		if(value is string)  { EncodeString(key, (string)value); return; }
		if(value is bool)    { EncodeBool(key, (bool)value); return; }
		if(value is int)     { EncodeInt(key, (int)value); return; }
		if(value is uint)    { EncodeUInt(key, (uint)value); return; }
		if(value is short)   { EncodeShort(key, (short)value); return; }
		if(value is ushort)  { EncodeUShort(key, (ushort)value); return; }
		if(value is long)    { EncodeLong(key, (long)value); return; }
		if(value is ulong)   { EncodeULong(key, (ulong)value); return; }
		if(value is float)   { EncodeFloat(key, (float)value); return; }
		if(value is double)  { EncodeDouble(key, (double)value); return; }
		if(value is decimal) { EncodeDecimal(key, (decimal)value); return; }
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
    void EncodeArrayOfObjects(string key, object value) {
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
			coder.EncodeObject(i.ToString(), array.GetValue(i));
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
			arrayStr+= encoder(array[i])+":";
		}
		Add(key, typeof(T[]), arrayStr);        
    }
    void EncodeArrayOfChars<T>(string key, T[] array, Func<T,string> encoder) {
		string arrayStr= "";
		for(int i= 0; i < array.Length; ++i) {
			arrayStr+= encoder(array[i]);
		}
		Add(key, typeof(T[]), arrayStr);
    }
	// ----------------------------------------------------------------------
	string EncodeType(Type value) {
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
        if(value == typeof(Vector2)) return "v2";
        if(value == typeof(Vector3)) return "v3";
        if(value == typeof(Vector4)) return "v4";
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
        if(value == typeof(Vector2[])) return "v2[]";
        if(value == typeof(Vector3[])) return "v3[]";
        if(value == typeof(Vector4[])) return "v4[]";
        if(value == typeof(System.Object[])) return "o[]";
        // iCanScript type compression
        if(value == typeof(iCS_Behaviour)) return "iBeh";
		if(value == typeof(iCS_RuntimeDesc)) return "iRtD";
		if(value == typeof(iCS_ObjectTypeEnum)) return "iOTE";
		if(value == typeof(iCS_Module)) return "iMd";
		if(value == typeof(iCS_StateChart)) return "iSC";
		if(value == typeof(iCS_State)) return "iSt";
//        if(value.IsByRef) {
//            Type elementType= value.GetElementType();
//    		if(elementType == typeof(Type)) return "t&";
//    		if(elementType == typeof(string)) return "S&";
//    		if(elementType == typeof(bool)) return "b&";
//    		if(elementType == typeof(int)) return "i&";
//            if(elementType == typeof(uint)) return "u&";
//            if(elementType == typeof(long)) return "l&";
//            if(elementType == typeof(ulong)) return "ul&";
//            if(elementType == typeof(short)) return "s&";
//            if(elementType == typeof(ushort)) return "us&";
//    		if(elementType == typeof(float)) return "f&";
//    		if(elementType == typeof(double)) return "d&";
//            if(elementType == typeof(char)) return "c&";
//            if(elementType == typeof(byte)) return "B&";
//            if(elementType == typeof(Vector2)) return "v2&";
//            if(elementType == typeof(Vector3)) return "v3&";
//            if(elementType == typeof(Vector4)) return "v4&";
//            if(elementType == typeof(System.Object)) return "o&";
//        }
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
    public string EncodeByte(byte value) {
        return ""+value;
    }
	// ----------------------------------------------------------------------
    public void EncodeByte(string key, byte value) {
        Add(key, typeof(byte), EncodeByte(value));
    }
	// ----------------------------------------------------------------------
    public string EncodeSByte(sbyte value) {
        return ""+value;
    }
	// ----------------------------------------------------------------------
    public void EncodeSByte(string key, sbyte value) {
        Add(key, typeof(sbyte), EncodeSByte(value));
    }
	// ----------------------------------------------------------------------
    public string EncodeChar(char value) {
        return ""+value;
    }
	// ----------------------------------------------------------------------
    public void EncodeChar(string key, char value) {
        Add(key, typeof(char), EncodeChar(value));
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

    // ======================================================================
    // Decoding
	// ----------------------------------------------------------------------
    public object DecodeObjectForKey(string key) {
        if(!myDictionary.ContainsKey(key)) { return null; }
        Prelude.Tuple<string,string> tuple= myDictionary[key];
        Type valueType= DecodeType(tuple.Item1);
        string valueStr= tuple.Item2;
        // Special case for arrays.
		if(valueType.IsArray) {
            return DecodeArrayOfObjects(valueType, valueStr);
		}
		// Special case for enums.
        iCS_Coder coder= new iCS_Coder();
		if(valueType.IsEnum) {
            coder.Archive= valueStr;
            return coder.DecodeIntForKey("Numeric");
        }
		// Primitives.
		Type elementType= iCS_Types.GetElementType(valueType);
        if(elementType == typeof(Type))    return DecodeType(valueStr);
		if(elementType == typeof(byte))    return DecodeByte(valueStr);
		if(elementType == typeof(sbyte))   return DecodeSByte(valueStr);
		if(elementType == typeof(char))    return DecodeChar(valueStr);
		if(elementType == typeof(string))  return DecodeString(valueStr);
		if(elementType == typeof(bool))    return DecodeBool(valueStr);
		if(elementType == typeof(int))     return DecodeInt(valueStr);
		if(elementType == typeof(uint))    return DecodeUInt(valueStr);
		if(elementType == typeof(short))   return DecodeShort(valueStr);
		if(elementType == typeof(ushort))  return DecodeUShort(valueStr);
		if(elementType == typeof(long))    return DecodeLong(valueStr);
		if(elementType == typeof(ulong))   return DecodeULong(valueStr);
		if(elementType == typeof(float))   return DecodeFloat(valueStr);
		if(elementType == typeof(double))  return DecodeDouble(valueStr);
		if(elementType == typeof(decimal)) return DecodeDecimal(valueStr);
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
    object DecodeArrayOfObjects(Type valueType, string valueStr) {
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
            array.SetValue(coder.DecodeObjectForKey(i.ToString()), i);
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
		if(value == "v2") return typeof(Vector2);
		if(value == "v3") return typeof(Vector3);
		if(value == "v4") return typeof(Vector4);
		if(value == "o") return typeof(System.Object);
		if(value == "t&") return typeof(Type);
		if(value == "S&") return typeof(string);
		if(value == "b&") return typeof(bool);
		if(value == "i&") return typeof(int);
		if(value == "u&") return typeof(uint);
		if(value == "l&") return typeof(long);
		if(value == "ul&") return typeof(ulong);
		if(value == "s&") return typeof(short);
		if(value == "us&") return typeof(ushort);		
		if(value == "f&") return typeof(float);
		if(value == "d&") return typeof(double);
        if(value == "c&") return typeof(char);
        if(value == "B&") return typeof(byte);
		if(value == "v2&") return typeof(Vector2);
		if(value == "v3&") return typeof(Vector3);
		if(value == "v4&") return typeof(Vector4);
		if(value == "o&") return typeof(System.Object);
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
		if(value == "v2[]") return typeof(Vector2[]);
		if(value == "v3[]") return typeof(Vector3[]);
		if(value == "v4[]") return typeof(Vector4[]);
		if(value == "o[]") return typeof(System.Object[]);
		if(value == "iBeh") return typeof(iCS_Behaviour);
		if(value == "iRtD") return typeof(iCS_RuntimeDesc);
		if(value == "iOTE") return typeof(iCS_ObjectTypeEnum);
		if(value == "iMd") return typeof(iCS_Module);
		if(value == "iSt") return typeof(iCS_State);
		if(value == "iSC") return typeof(iCS_StateChart);
		// Decompress type string.
		int cSharpTypeIdx= value.IndexOf("!!");
		if(cSharpTypeIdx > 0) {
			return Type.GetType(value.Substring(0, cSharpTypeIdx)+CSharpTypeStr+value.Substring(cSharpTypeIdx+2));
		}
		int unityEngineTypeIdx= value.IndexOf("!#");
		if(unityEngineTypeIdx > 0) {
			return Type.GetType(value.Substring(0, unityEngineTypeIdx)+UnityEngineTypeStr+value.Substring(unityEngineTypeIdx+2));
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
		return Type.GetType(value);
    }
	// ----------------------------------------------------------------------
    bool DecodeBoolForKey(string key) {
        return DecodeForKey<bool>(key, DecodeBool);
    }
	// ----------------------------------------------------------------------
    bool DecodeBool(string value) {
        return value == "T";
    }
	// ----------------------------------------------------------------------
    int DecodeIntForKey(string key) {
        return DecodeForKey<int>(key, DecodeInt);
    }
	// ----------------------------------------------------------------------
    int DecodeInt(string value) {
        return (int)Convert.ChangeType(value, typeof(int));
    }
	// ----------------------------------------------------------------------
    uint DecodeUIntForKey(string key) {
        return DecodeForKey<uint>(key, DecodeUInt);
    }
	// ----------------------------------------------------------------------
    uint DecodeUInt(string value) {
        return (uint)Convert.ChangeType(value, typeof(uint));
    }
	// ----------------------------------------------------------------------
    short DecodeShortForKey(string key) {
        return DecodeForKey<short>(key, DecodeShort);
    }
	// ----------------------------------------------------------------------
    short DecodeShort(string value) {
        return (short)Convert.ChangeType(value, typeof(short));
    }
	// ----------------------------------------------------------------------
    ushort DecodeUShortForKey(string key) {
        return DecodeForKey<ushort>(key, DecodeUShort);
    }
	// ----------------------------------------------------------------------
    ushort DecodeUShort(string value) {
        return (ushort)Convert.ChangeType(value, typeof(ushort));
    }
	// ----------------------------------------------------------------------
    long DecodeLongForKey(string key) {
        return DecodeForKey<long>(key, DecodeLong);
    }
	// ----------------------------------------------------------------------
    long DecodeLong(string value) {
        return (long)Convert.ChangeType(value, typeof(long));
    }
	// ----------------------------------------------------------------------
    ulong DecodeULongForKey(string key) {
        return DecodeForKey<ulong>(key, DecodeULong);
    }
	// ----------------------------------------------------------------------
    ulong DecodeULong(string value) {
        return (ulong)Convert.ChangeType(value, typeof(ulong));
    }
	// ----------------------------------------------------------------------
    float DecodeFloatForKey(string key) {
        return DecodeForKey<float>(key, DecodeFloat);
    }
	// ----------------------------------------------------------------------
    float DecodeFloat(string value) {
        return (float)Convert.ChangeType(value, typeof(float));
    }
	// ----------------------------------------------------------------------
    double DecodeDoubleForKey(string key) {
        return DecodeForKey<double>(key, DecodeDouble);
    }
	// ----------------------------------------------------------------------
    double DecodeDouble(string value) {
        return (double)Convert.ChangeType(value, typeof(double));
    }
	// ----------------------------------------------------------------------
    decimal DecodeDecimalForKey(string key) {
        return DecodeForKey<decimal>(key, DecodeDecimal);
    }
	// ----------------------------------------------------------------------
    decimal DecodeDecimal(string value) {
        return (decimal)Convert.ChangeType(value, typeof(decimal));
    }
	// ----------------------------------------------------------------------
    byte DecodeByteForKey(string key) {
        return DecodeForKey<byte>(key, DecodeByte);
    }
	// ----------------------------------------------------------------------
    byte DecodeByte(string value) {
        return (byte)value[0];
    }
	// ----------------------------------------------------------------------
    sbyte DecodeSByteForKey(string key) {
        return DecodeForKey<sbyte>(key, DecodeSByte);
    }
	// ----------------------------------------------------------------------
    sbyte DecodeSByte(string value) {
        return (sbyte)value[0];
    }
	// ----------------------------------------------------------------------
    char DecodeCharForKey(string key) {
        return DecodeForKey<char>(key, DecodeChar);
    }
	// ----------------------------------------------------------------------
    char DecodeChar(string value) {
        return value[0];
    }
	// ----------------------------------------------------------------------
    string DecodeStringForKey(string key) {
        return DecodeForKey<string>(key, DecodeString);
    }
	// ----------------------------------------------------------------------
    string DecodeString(string value) {
        return value;
    }
	// ----------------------------------------------------------------------
    Vector2 DecodeVector2ForKey(string key) {
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
    Vector3 DecodeVector3ForKey(string key) {
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
    Vector4 DecodeVector4ForKey(string key) {
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
