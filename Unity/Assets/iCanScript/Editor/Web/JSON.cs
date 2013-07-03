using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class JSON {
    // -----------------------------------------------------------------------------
    // Returns the value of an attribute from the JSON formatted string.
    public static JValue GetValueFor(string jsonStr, string accessorStr) {
        int i= 0;
        var root= JSON.ParseNameValuePair(jsonStr, ref i);
        i= 0;
        var attribute= ParseAttribute(accessorStr, ref i);
        if(attribute != root.name) {
            return JNull.identity;
        }
        accessorStr= accessorStr.Substring(i, accessorStr.Length-i);
        return root.value.GetValueFor(accessorStr);
    }
    // -----------------------------------------------------------------------------
    public static JNameValuePair ParseNameValuePair(string s, ref int i) {
        // Parse attribute name
        string name= ParseName(s, ref i);
        // Look for JSON name / value seperator
        MustBeChar(s, ref i, ':');
        // Parse value
        var value= ParseValue(s, ref i);
        return new JNameValuePair(name, value);
    }
    // -----------------------------------------------------------------------------
    public static string ParseName(string s, ref int i) {
        return ParseString(s, ref i);
    }
    // -----------------------------------------------------------------------------
    public static JValue ParseValue(string s, ref int i) {
        RemoveWhiteSpaces(s, ref i);
        if(eof(s,i)) {
            throw new SystemException("JSON: eof seen where value was expected.");
        }
        switch(s[i]) {
            case '"': {
                return new JString(ParseString(s, ref i));
            }
            case '[': {
                return ParseArray(s, ref i);
            }
            case '{': {
                return ParseObject(s, ref i);
            }
            default: {
                if(s.Length-i >= 5) {
                    if(s.Substring(i, 5) == "false") {
                        i+= 5;
                        return new JBool(false);
                    }
                }
                if(s.Length-i >= 4) {
                    var v= s.Substring(i, 4);
                    if(v == "true") {
                        i+= 4;
                        return new JBool(true);
                    }
                    if(v == "null") {
                        i+= 4;
                        return new JNull();
                    }
                }
                Debug.LogWarning("JSON: parsing numeric not yet implemented !!!");
                return new JNull();
            }
        }
    }
    // -----------------------------------------------------------------------------
    public static string ParseString(string s, ref int i) {
        MustBeChar(s, ref i, '"');
        string result= "";
        do {
            switch(s[i]) {
                case '\\': {
                    if(eof(s, i+1)) {
                        result+= s[i];
                        ++i;
                        break;
                    }
                    if(s[i+1] == '"') {
                        result+= '"';
                        i+= 2;
                        break;
                    }
                    result+= s[i];
                    result+= s[i+1];
                    i+= 2;
                    break;
                }
                case '"' : { break; }
                default  : { result+= s[i]; ++i; break; }
            }
        } while(!eof(s,i) && s[i] != '"');
        if(eof(s,i)) {
            throw new SystemException("JSON: format corrupted in string parsing!");
        }
        ++i;
        return result;
    }
    // -----------------------------------------------------------------------------
    public static string ParseAttribute(string s, ref int i) {
        RemoveWhiteSpaces(s, ref i);
        int start= i;
        if(eof(s,i)) {
            throw new SystemException("JSON: format corrupted in attribute parsing!");
        }
        for(; !eof(s, i) && (Char.IsLetterOrDigit(s[i]) || s[i] == '_'); ++i);
        return s.Substring(start, i-start);
    }
    // -----------------------------------------------------------------------------
    static JValue ParseArray(string s, ref int i) {
        MustBeChar(s, ref i, '[');
        RemoveWhiteSpaces(s, ref i);
        if(eof(s,i)) {
            throw new SystemException("JSON: eof seen parsing array.");
        }
        var values= new List<JValue>();
        while(s[i] != ']') {
            values.Add(ParseValue(s, ref i));
            RemoveWhiteSpaces(s, ref i);
            if(eof(s,i)) {
                throw new SystemException("JSON: eof seen parsing array.");
            }
            switch(s[i]) {
                case ',': { ++i; break; }
                case ']': { break; }
                default: {
                    throw new SystemException("JSON: invalid character: "+s[i]+" used as seperator in parsing array.");
                }
            }
        }
        return new JArray(values);        
    }
    // -----------------------------------------------------------------------------
    static JValue ParseObject(string s, ref int i) {
        MustBeChar(s, ref i, '{');
        RemoveWhiteSpaces(s, ref i);
        if(eof(s,i)) {
            throw new SystemException("JSON: eof seen parsing object.");
        }
        var attributes= new Dictionary<string,JValue>();
        while(s[i] != '}') {
            var nv= ParseNameValuePair(s, ref i);
            attributes.Add(nv.name, nv.value);
            RemoveWhiteSpaces(s, ref i);
            if(eof(s,i)) {
                throw new SystemException("JSON: eof seen parsing object.");
            }
            switch(s[i]) {
                case ',': { ++i; break; }
                case '}': { break; }
                default: {
                    throw new SystemException("JSON: invalid character: "+s[i]+" used as seperator in parsing object.");
                }
            }
        }
        return new JObject(attributes);        
    }
    // -----------------------------------------------------------------------------
    public static void RemoveWhiteSpaces(string s, ref int i) {
        for(; !eof(s,i) && Char.IsWhiteSpace(s[i]); ++i);
    }
    // -----------------------------------------------------------------------------
    public static bool eof(string s, int i) {
        return i >= s.Length;
    }
    // -----------------------------------------------------------------------------
    static void MustBeChar(string s, ref int i, char c) {
        if(eof(s,i)) {
            throw new SystemException("JSON: eof reach but expected: "+c);
        }
        RemoveWhiteSpaces(s, ref i);
        if(eof(s,i)) {
            throw new SystemException("JSON: eof reach but expected: "+c);
        }
        if(s[i] != c) {
            throw new SystemException("JSON: format error: expected: "+c+" but seen: "+s[i]);
        }
        ++i;
    }
    // -----------------------------------------------------------------------------
    public static int ParseDigits(string s, ref int i) {
        RemoveWhiteSpaces(s, ref i);
        if(eof(s, i)) return 0;
        int result= 0;
        for(; Char.IsDigit(s, i); ++i) {
            result*= 10;
            result+= s[i]-'0';
        }
        return result;
    }
}

// =============================================================================
// JSON value
// -----------------------------------------------------------------------------
public class JValue {
    public bool isBool   { get { return this is JBool; }}
    public bool isNull   { get { return this is JNull; }}
    public bool isString { get { return this is JString; }}
    public bool isNumber { get { return this is JNumber; }}
    public bool isArray  { get { return this is JArray; }}
    public bool isObject { get { return this is JObject; }}
    public virtual JValue GetValueFor(string accesor) { return this; }
}
public class JNull   : JValue { public static JNull identity= new JNull(); }
public class JBool   : JValue { public bool value;   public JBool(bool v) { value= v; }}
public class JString : JValue { public string value; public JString(string v) { value= v; }}
public class JNumber : JValue { public float value;  public JNumber(float v) { value= v; }}
public class JArray  : JValue {
    public List<JValue> value= new List<JValue>();
    public JArray(List<JValue> lst) { value= lst; }
    public JValue GetValueFor(int idx) {
        if(idx < 0 || idx >= value.Count) {
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
        if(idx < 0 || idx >= value.Count) {
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
}
public class JObject : JValue {
    public Dictionary<string, JValue> value= new Dictionary<string, JValue>();
    public JObject(Dictionary<string, JValue> dict) { value= dict; }
    public override JValue GetValueFor(string accessor) {
        int i= 0;
        JSON.RemoveWhiteSpaces(accessor, ref i);
        if(JSON.eof(accessor, i)) {
            return this;
        }
        if(accessor[i] == '.') {
            ++i;
        }
        var name= JSON.ParseAttribute(accessor, ref i);
        if(!value.ContainsKey(name)) {
            return JNull.identity;
        }
        accessor= accessor.Substring(i, accessor.Length-i);
        return value[name].GetValueFor(accessor);
    }
}

// =============================================================================
// JSON name / value pair
// -----------------------------------------------------------------------------
public class JNameValuePair {
    public string name= null;
    public JValue value= null;
    public JNameValuePair(string _name, JValue _value) { name= _name; value= _value; }
}