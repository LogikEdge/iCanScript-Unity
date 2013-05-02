using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class JSON {
    public static JNameValuePair ParseNameValuePair(string s, ref int c) {
        string name= ParseName(s, ref c);
        RemoveWhiteSpaces(s, ref c);
        if(eof(s,c) || s[c] != ',') {
            throw new SystemException("JSON corrupted format: expected name value seperator ,");
        }
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
        return null;
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
        if(eof(s,c)) return null;
        if(s[c] != '"') {
            throw new SystemException("JSON name not starting with a double quote!");
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
            throw new SystemException("JSON format corrupted in string parsing!");
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