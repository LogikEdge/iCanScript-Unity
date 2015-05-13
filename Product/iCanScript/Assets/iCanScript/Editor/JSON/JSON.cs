using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.JSON {
    
public abstract class JSON {
    // =============================================================================
    // Public functionality
    // -----------------------------------------------------------------------------
    public abstract string Encode();

    // =============================================================================
    // Returns the decoded root JSON object.
    // -----------------------------------------------------------------------------
    public static JObject GetRootObject(string _jsonStr) {
		// Convert to ASCII string.
		string jsonStr= iCS_TextUtility.ToASCII(_jsonStr);
		// Read the root json object
        int i= 0;
		return ParseObject(jsonStr, ref i);
    }

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
    protected static JValue GetValueFor(string jsonStr, string accessorStr) {
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
    protected static string ParseName(string s, ref int i) {
        return ParseString(s, ref i);
    }
    // -----------------------------------------------------------------------------
    protected static JValue ParseValue(string s, ref int i) {
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
				// Assume that we have a number.
				return ParseNumber(s, ref i);
            }
        }
    }
    // -----------------------------------------------------------------------------
	// Parses the string representation of a JSON string.
    protected static string ParseString(string s, ref int i) {
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
                    if(s[i+1] == '"') {  // Accept string quote
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
	// Parses the string representation of a JSON number.
	protected static JValue ParseNumber(string s, ref int i) {
		int sign= 1;
		int integer= 0;
		float decimal_= 0;
		int exponent= 0;
		// Parse sign section
		if(s[i] == '-') {
			sign= -1;
			++i;
			if(eof(s,i)) {
	            throw new SystemException("JSON: EOF while parsing number!");
			}
		}
		// Parse integer section
		if(!Char.IsDigit(s[i])) {
            throw new SystemException("JSON: Error parsing number: Expected digit and received: "+s[i]);			
		}
  	  	if(s[i] != 0) {
  	  		while(Char.IsDigit(s[i])) {
  	  			integer*= 10;
				integer+= s[i]-'0';
				++i;
				if(eof(s,i)) {
					// Integer Number
					return new JNumber((float)(sign*integer));
				}
  	  		}
  	  	}
		// Return integer value
		if(s[i] != '.' && s[i] != 'e' && s[i] != 'E') {
			return new JNumber((float)(sign*integer));
		}
		// Parse decimal section
		if(s[i] == '.') {
			++i;
			if(eof(s,i)) {
				// FLoat Number
				return new JNumber((float)(sign*integer));
			}
			float scale= 0.1f;
			while(Char.IsDigit(s[i])) {
				decimal_+= scale*(s[i]-'0');
				scale /= 10;
				++i;
				if(eof(s,i)) {
					// Float
					return new JNumber(sign*(integer+decimal_));
				}
			}
		}
		float number= sign*(integer+decimal_);
		if(s[i] != 'e' && s[i] != 'E') {
			return new JNumber(number);
		}
		// Parse exponent section
		++i;
		if(eof(s,i)) {
			// Float
			return new JNumber(number);			
		}
		float expSign= 1f;
		if(s[i] == '-') {
			expSign= -1f;
		}
		if(s[i] == '-' || s[i] == '+') {
			++i;
			if(eof(s,i)) {
				// Float
				return new JNumber(number);				
			}
		}
		while(Char.IsDigit(s[i])) {
			exponent*= 10;
			exponent+= s[i]-'0';
			++i;
			if(eof(s,i)) {
				// Float with exponent.
				return new JNumber(number*Mathf.Pow(10f,expSign*exponent));
			}
		}
		return new JNumber(number*Mathf.Pow(10f,expSign*exponent));
	}
    // -----------------------------------------------------------------------------
	// Parses the string representation of a JSON array.
    protected static JValue ParseArray(string s, ref int i) {
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
                case ',': { ++i; RemoveWhiteSpaces(s, ref i); break; }
                case ']': { break; }
                default: {
                    throw new SystemException("JSON: invalid character: "+s[i]+" used as seperator in parsing array.");
                }
            }
        }
		++i;
        return new JArray(values.ToArray());        
    }
    // -----------------------------------------------------------------------------
	// Parses the string representation of a JSON object.
    protected static JObject ParseObject(string s, ref int i) {
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
                case ',': { ++i; RemoveWhiteSpaces(s, ref i); break; }
                case '}': { break; }
                default: {
                    throw new SystemException("JSON: invalid character: "+s[i]+" used as seperator in parsing object.");
                }
            }
        }
		++i;
        return new JObject(attributes.ToArray());        
    }
    // -----------------------------------------------------------------------------
    protected static void RemoveWhiteSpaces(string s, ref int i) {
        for(; !eof(s,i) && Char.IsWhiteSpace(s[i]); ++i);
    }
    // -----------------------------------------------------------------------------
    protected static bool eof(string s, int i) {
        return i >= s.Length;
    }
    // -----------------------------------------------------------------------------
    protected static void MustBeChar(string s, ref int i, char c) {
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
    protected static int ParseDigits(string s, ref int i) {
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
    protected static string ParseAttribute(string s, ref int i) {
        RemoveWhiteSpaces(s, ref i);
        int start= i;
        if(eof(s,i)) {
            throw new SystemException("JSON: format corrupted in attribute parsing!");
        }
        for(; !eof(s, i) && (Char.IsLetterOrDigit(s[i]) || s[i] == '_'); ++i);
        return s.Substring(start, i-start);
    }
}

}   // namespace iCanScript.Internal.JSON
