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
    // Properties
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
			
		}
	}

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
	string BuildArchive(string str) {
		return "{"+str.Length.ToString()+":"+str+"}";
	}
    // ======================================================================
    // Encoding
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
		Debug.Log("Encoding int");
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
			coder.Encode(field.GetValue(value), field.Name);
		}
		return coder.Archive;
	}
	// ----------------------------------------------------------------------
	public void Encode(object value, string key) {
		Encode(Encode(value), key);
	}

}
