using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript {
    
public abstract class JSON {
    // =============================================================================
    // Public functionality
    // -----------------------------------------------------------------------------
    public abstract string Encode();

    // =============================================================================
    // Encoding functions
    // -----------------------------------------------------------------------------
    protected static string Encode(string value) {
        string result= "\"";
        for(int i= 0; i < value.Length; ++i) {
            switch(value[i]) {
                case '"':
                    result+= "\\\"";
                    break;
                case '\\':
                    result+= "\\\\";
                    break;
                default:
                    result+= value[i];
                    break;
            }
        }
        return result+"\"";        
    }
    
    // =============================================================================
    // Decoding functions
    // -----------------------------------------------------------------------------
    // Returns the value of an attribute from the JSON formatted string.
    public static JValue GetValueFor(string jsonStr, string accessorStr) {
        int i= 0;
        var root= JNameValuePair.Decode(jsonStr, ref i);
        i= 0;
        var attribute= ParseAttribute(accessorStr, ref i);
        if(attribute != root.name) {
            return JNull.identity;
        }
        accessorStr= accessorStr.Substring(i, accessorStr.Length-i);
        return root.value.GetValueFor(accessorStr);
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
        return new JArray(values.ToArray());        
    }
    // -----------------------------------------------------------------------------
    public static JObject ParseObject(string s, ref int i) {
        MustBeChar(s, ref i, '{');
        RemoveWhiteSpaces(s, ref i);
        if(eof(s,i)) {
            throw new SystemException("JSON: eof seen parsing object.");
        }
        var attributes= new List<JNameValuePair>();
        while(s[i] != '}') {
            var nv= JNameValuePair.Decode(s, ref i);
            attributes.Add(nv);
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
        return new JObject(attributes.ToArray());        
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
    public static void MustBeChar(string s, ref int i, char c) {
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

    // =============================================================================
    // Query parsing
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
}

}   // namespace iCanScript
