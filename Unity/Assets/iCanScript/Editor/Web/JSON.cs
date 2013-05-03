using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class JSON {
    public static JNameValuePair ParseNameValuePair(string s, ref int c) {
        // Parse attribute name
        string name= ParseName(s, ref c);
        // Look for JSON name / value seperator
        RemoveWhiteSpaces(s, ref c);
        if(eof(s,c) || s[c] != ',') {
            throw new SystemException("JSON: corrupted format: expected name value seperator ,");
        }
        ++c;
        // Parse value
        var value= ParseValue(s, ref c);
        return new JNameValuePair(name, value);
    }
    // -----------------------------------------------------------------------------
    public static string ParseName(string s, ref int c) {
        return ParseString(s, ref c);
    }
    // -----------------------------------------------------------------------------
    public static JValue ParseValue(string s, ref int c) {
        RemoveWhiteSpaces(s, ref c);
        if(eof(s,c)) {
            throw new SystemException("JSON: eof seen where value was expected.");
        }
        switch(s[c]) {
            case '"': {
                return new JString(ParseString(s, ref c));
            }
            case '[': {
                Debug.LogWarning("JSON: parsing array not yet implemented !!!");
                return new JNull();
            }
            case '{': {
                Debug.LogWarning("JSON: parsing object not yet implemented !!!");
                return new JNull();
            }
            default: {
                if(s.Length-c >= 5) {
                    if(s.Substring(c, 5) == "false") {
                        c+= 5;
                        return new JBool(false);
                    }
                }
                if(s.Length-c >= 4) {
                    var v= s.Substring(c, 4);
                    if(v == "true") {
                        c+= 4;
                        return new JBool(true);
                    }
                    if(v == "null") {
                        c+= 4;
                        return new JNull();
                    }
                }
                Debug.LogWarning("JSON: parsing numeric not yet implemented !!!");
                return new JNull();
            }
        }
    }
    // -----------------------------------------------------------------------------
    static void RemoveWhiteSpaces(string s, ref int c) {
        for(; !eof(s,c); ++c) {
            switch(s[c]) {
                case ' ' : { break; }
                case '\r': { break; }
                case '\n': { break; }
                case '\t': { break; }
                default:   { return; }
            }
        }
    }
    // -----------------------------------------------------------------------------
    static string ParseString(string s, ref int c) {
        RemoveWhiteSpaces(s, ref c);
        if(eof(s,c)) {
            throw new SystemException("JSON: eof seen where string was expected");
        }
        if(s[c] != '"') {
            throw new SystemException("JSON: name not starting with a double quote!");
        }
        int start= ++c;
        do {
            switch(s[c]) {
                case '\\': { c+= 2; break; }
                case '"' : { break; }
                default  : { ++c; break; }
            }
        } while(!eof(s,c) && s[c] != '"');
        if(eof(s,c)) {
            throw new SystemException("JSON: format corrupted in string parsing!");
        }
        int len= c-start;
        ++c;
        return s.Substring(start, len);
    }
    
    static bool eof(string s, int c) {
        return c >= s.Length;
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
}
public class JNull   : JValue {}
public class JBool   : JValue { public bool value;   public JBool(bool v) { value= v; }}
public class JString : JValue { public string value; public JString(string v) { value= v; }}
public class JNumber : JValue { public float value;  public JNumber(float v) { value= v; }}
public class JArray  : JValue { public List<JValue> value= new List<JValue>(); }
public class JObject : JValue { public List<JNameValuePair> value= new List<JNameValuePair>(); }

// =============================================================================
// JSON name / value pair
// -----------------------------------------------------------------------------
public class JNameValuePair {
    public string name= null;
    public JValue value= null;
    public JNameValuePair(string _name, JValue _value) { name= _name; value= _value; }
}