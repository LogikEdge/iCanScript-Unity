using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.JSON {
    
// =============================================================================
// JSON value
// -----------------------------------------------------------------------------
public abstract class JValue : JSON {
    public bool isBool   { get { return this is JBool; }}
    public bool isNull   { get { return this is JNull; }}
    public bool isString { get { return this is JString; }}
    public bool isNumber { get { return this is JNumber; }}
    public bool isArray  { get { return this is JArray; }}
    public bool isObject { get { return this is JObject; }}
    public virtual JValue GetValueFor(string accesor) { return this; }
    public static JValue Build(System.Object value) {
        // Process Null
        if(value == null)               { return JNull.identity; }
        // Process Arrays
        var valueType= value.GetType();
        if(valueType.IsArray) {
            var result= new List<JValue>();
            foreach(var obj in (Array)value) {
                result.Add(Build(obj));
            }
            return new JArray(result.ToArray());
        }
        if(valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>)) {
            var result= new List<JValue>();
            foreach(var obj in (IList)value) {
                result.Add(Build(obj));
            }
            return new JArray(result.ToArray());
        }
        // Basic Types
		if(value is bool)               { return new JBool((bool)value); }
		if(value is string)             { return new JString((string)value); }
		if(value is char)               { return new JString(new string((char)value,1)); }
		if(value is byte)               { return new JNumber((float)((byte)value)); }
		if(value is sbyte)              { return new JNumber((float)((sbyte)value)); }
		if(value is int)                { return new JNumber((float)((int)value)); }
		if(value is uint)               { return new JNumber((float)((uint)value)); }
		if(value is short)              { return new JNumber((float)((short)value)); }
		if(value is ushort)             { return new JNumber((float)((ushort)value)); }
		if(value is long)               { return new JNumber((float)((long)value)); }
		if(value is ulong)              { return new JNumber((float)((ulong)value)); }
		if(value is float)              { return new JNumber((float)value); }
		if(value is double)             { return new JNumber((float)((double)value)); }
		if(value is decimal)            { return new JNumber((float)((decimal)value)); }
        // Special case for Unity Engine Objects
        if(value is UnityEngine.Object && !(value is UnityEngine.ScriptableObject)) {
            var obj= value as UnityEngine.Object;
            var desc= new List<JNameValuePair>();
            desc.Add(new JNameValuePair("InstanceId", obj.GetInstanceID()));
            desc.Add(new JNameValuePair("Name", obj.name));
            desc.Add(new JNameValuePair("Type", obj.GetType().FullName));
            if(value is GameObject || value is Component) {
                GameObject parent= null;
                if(value is GameObject) {
                    var go= value as GameObject;
                    var transform= go.transform;
                    if(transform != null) {
                        transform= transform.parent;
                        if(transform != null) {
                            parent= transform.gameObject;
                        }
                    }
                }
                if(value is Component) {
                    parent= (value as Component).gameObject;
                }
                var parentList= new List<JValue>();
                while(parent != null) {
                    parentList.Add(new JString(parent.name));
                    var transform= parent.transform;
                    if(transform != null) {
                        transform= transform.parent;
                        if(transform != null) {
                            parent= transform.gameObject;                                                    
                        }
                        else {
                            parent= null;
                        }
                    }
                    else {
                        parent= null;
                    }
                }
                parentList.Reverse();
                desc.Add(new JNameValuePair("Parents", parentList));
                var scenePath= EditorApplication.currentScene;
                desc.Add(new JNameValuePair("SceneGUID", AssetDatabase.AssetPathToGUID(scenePath)));
            }
            // Handle Unity Assets
            else {
                var assetPath= AssetDatabase.GetAssetPath(obj.GetInstanceID());
                if(!string.IsNullOrEmpty(assetPath)) {
                    desc.Add(new JNameValuePair("AssetGUID", AssetDatabase.AssetPathToGUID(assetPath)));
                }
            }
            return new JObject(desc);
        }
        // Process Objects
        var attributes= new List<JNameValuePair>();
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
    			attributes.Add(new JNameValuePair(field.Name, Build(field.GetValue(value))));                                
            }
		}
        return new JObject(attributes);
    }
}
public class JNull   : JValue {
    public static JNull identity= new JNull();
    public override string Encode() { return "null"; }
}
public class JBool   : JValue {
    public bool value;
    public JBool(bool v) { value= v; }
    public override string Encode() { return value ? "true" : "false"; }
}
public class JString : JValue {
    public string value;
    public JString(string v) { value= v; }
    public override string Encode() {
        return Encode(value);
    }
}
public class JNumber : JValue {
    public float value;
    public JNumber(float v) { value= v; }
    public override string Encode() { return (string)Convert.ChangeType(value, typeof(string)); }
}
public class JArray  : JValue {
    public JValue[] value= new JValue[0];
    public JArray(JValue[] lst) { value= lst; }
    public JValue GetValueFor(int idx) {
        if(idx < 0 || idx >= value.Length) {
            return JNull.identity;
        }
        return value[idx];
    }
    public override JValue GetValueFor(string accessor) {
        int i= 0;
        JSON.RemoveWhiteSpaces(accessor, ref i);
        if(JSON.eof(accessor, i) || accessor[i] != '[') {
            return this;
        }
        ++i;
        int idx= JSON.ParseDigits(accessor, ref i);
        if(idx < 0 || idx >= value.Length) {
            return JNull.identity;
        }
        JSON.RemoveWhiteSpaces(accessor, ref i);
        if(JSON.eof(accessor, i) || accessor[i] != ']') {
            return value[idx];
        }
        ++i;
        JSON.RemoveWhiteSpaces(accessor, ref i);
        accessor= accessor.Substring(i, accessor.Length-i);
        return value[idx].GetValueFor(accessor);
    }
    public override string Encode() {
        var result= new StringBuilder("[", 1000);
        for(int i= 0; i < value.Length; ++i) {
            result.Append(value[i].Encode());
            if(i < value.Length-1) result.Append(",");
        }
        result.Append("]");
        return result.ToString();
    }
}
public class JObject : JValue {
    public JNameValuePair[] value= new JNameValuePair[0];
    public JObject(JNameValuePair[] v)     { value= v; }
    public JObject(List<JNameValuePair> v) : this(v.ToArray())             {}
    public JObject(JNameValuePair v)       : this(new JNameValuePair[]{v}) {}
    public JObject(JNameValuePair v1, JNameValuePair v2) : this(new JNameValuePair[]{v1,v2}) {}
    public JObject(JNameValuePair v1, JNameValuePair v2, JNameValuePair v3) : this(new JNameValuePair[]{v1,v2,v3}) {}
    public JNameValuePair FindPairFor(string name) {
        foreach(var nv in value) {
            if(name == nv.name) return nv;
        }
        return null;
    }
    public override JValue GetValueFor(string accessor) {
        int i= 0;
        RemoveWhiteSpaces(accessor, ref i);
        if(JSON.eof(accessor, i)) {
            return this;
        }
        if(accessor[i] == '.') {
            ++i;
        }
        var name= ParseAttribute(accessor, ref i);
        var nv= FindPairFor(name);
        if(nv == null) {
            return JNull.identity;
        }
        accessor= accessor.Substring(i, accessor.Length-i);
        return nv.value.GetValueFor(accessor);
    }
    public override string Encode() {
        var result= new StringBuilder("{", 1000);
        for(int i= 0; i < value.Length; ++i) {
            var nv= value[i];
            result.Append(nv.Encode());
            if(i < value.Length-1) result.Append(",");
        }
        result.Append("}");
        return result.ToString();
    }
}

// =============================================================================
// JSON name / value pair
// -----------------------------------------------------------------------------
public class JNameValuePair : JSON {
    public string name= null;
    public JValue value= null;
    public JNameValuePair(string _name, JValue       _value) { name= _name; value= _value; }
    public JNameValuePair(string _name, string       _value) : this(_name, new JString(_value)) {}
    public JNameValuePair(string _name, float        _value) : this(_name, new JNumber(_value)) {}
    public JNameValuePair(string _name, bool         _value) : this(_name, new JBool(_value))   {}
    public JNameValuePair(string _name, JValue[]     _value) : this(_name, new JArray(_value))  {}
    public JNameValuePair(string _name, List<JValue> _value) : this(_name, _value.ToArray())    {}
    public JNameValuePair(string _name, JNameValuePair[] _value)     : this(_name, new JObject(_value)) {}
    public JNameValuePair(string _name, List<JNameValuePair> _value) : this(_name, _value.ToArray())    {}
    public override string Encode() {
        return Encode(name)+" : "+value.Encode();
    }
    public static JNameValuePair Decode(string s, ref int i) {
        // Parse attribute name
        string name= ParseName(s, ref i);
        // Look for JSON name / value seperator
        MustBeChar(s, ref i, ':');
        // Parse value
        var value= ParseValue(s, ref i);
        return new JNameValuePair(name, value);        
    }
}

} // namespace iCanScript.Internal.JSON