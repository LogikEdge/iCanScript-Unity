using UnityEngine;
using System.Collections;
using System.Text;

namespace iCanScript.Internal.JSON {

public static class JSONPrettyPrint {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string kTab= "  ";
    
    // ----------------------------------------------------------------------
    public static string Print(string encoded, int lineWidth= 132) {
        int indent= 0;
        var len= encoded.Length;
        var result= new StringBuilder(len+(len>>4));
        for(int i= 0; i < len; ++i) {
            char c= encoded[i];
            switch(c) {
                case '[':
                case '{':
                    ++indent;
                    result.Append(c);
                    result.Append("\n");
                    result.Append(GenerateIndent(indent));
                    break;
                case ']':
                case '}':
                    --indent;
                    result.Append("\n");
                    result.Append(GenerateIndent(indent));
                    result.Append(c);
                    break;
                case ',':
                    result.Append(c);
                    result.Append("\n");
                    result.Append(GenerateIndent(indent));
                    break;
                case '"':
                    result.Append(c);
                    for(++i; i < len; ++i) {
                        c= encoded[i];
                        result.Append(c);
                        if(c == '"') {
                            break;
                        }
                        if(c == '\\') {
                            ++i;
                            result.Append(encoded[i]);
                        }
                    }
                    break;
                default:
                    result.Append(c);
                    break;
            }
        }
        return result.ToString();
    }
    static string GenerateIndent(int indent) {
        var result= new StringBuilder(100);
        for(int i= 0; i < indent; ++i) {
            result.Append(kTab);
        }
        return result.ToString();
    }
}

} // namespace iCanScript.Internal.JSON