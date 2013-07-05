using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript {
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
        string result= "";
        for(int i= 0; i < value.Length; ++i) {
            result+= value[i].Encode();
            if(i < value.Length-1) result+= ",";
        }
        return "["+result+"]";
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
        string result= "";
        foreach(var nv in value) {
            result+= nv.Encode()+",";
        }
        int len= result.Length;
        if(len != 0) result= result.Substring(0, len-1);
        return "{"+result+"}";
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
} // namespace iCanScript