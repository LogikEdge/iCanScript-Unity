using UnityEngine;
using System.Collections;

namespace iCanScript {

public static class JSONPrettyPrint {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string kTab= "  ";
    
    // ----------------------------------------------------------------------
    public static string Beautify(string encoded) {
        int indent= 0;
        string result= "";
        foreach(var c in encoded) {
            switch(c) {
                case '[':
                case '{':
                    ++indent;
                    result+= c+"\n"+GenerateIndent(indent);
                    break;
                case ']':
                case '}':
                    --indent;
                    result+= "\n"+GenerateIndent(indent)+c;
                    break;
                case ',':
                    result+= c+"\n"+GenerateIndent(indent);
                    break;
                default:
                    result+= c;
                    break;
            }
        }
        return result;
    }
    static string GenerateIndent(int indent) {
        string result= "";
        for(int i= 0; i < indent; ++i) {
            result+= kTab;
        }
        return result;
    }
}

} // namespace iCanScript