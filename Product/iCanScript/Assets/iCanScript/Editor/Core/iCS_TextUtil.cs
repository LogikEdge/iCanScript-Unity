using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_TextUtil {

        // ----------------------------------------------------------------------
        // Character manipulation.
        public static bool IsIdent1(char c)          { return char.IsLetter(c) || c == '_'; }
        public static bool IsIdent(char c)           { return char.IsLetterOrDigit(c) || c == '_'; }
        public static bool IsAlphaNum(char c)        { return char.IsLetterOrDigit(c); }
        public static bool IsLetterOrDigit(char c)   { return char.IsLetterOrDigit(c); }

        // ----------------------------------------------------------------------
        // string manipulation.
        public static string StripBeforeIdent(string text) {
            int i= 0;
            for(; i < text.Length; ++i) {
                if(IsIdent1(text[i])) break;
            }
            int remaining= text.Length-i;
            return remaining != 0 ? text.Substring(i, remaining) : "";
        }
        public static string StripSpaces(string text) {
            int i= 0;
            for(; i < text.Length; ++i) {
                if(!char.IsSeparator(text[i])) break;
            }
            return text.Substring(i, text.Length-i);
        }
        public static string ParseIdent(ref string text) {
            if(text.Length == 0 || !IsIdent1(text[0])) return "";
            int i= 0;
            for(; i < text.Length; ++i) {
                if(!IsIdent(text[i])) break;
            }
            string ident= text.Substring(0, i);
            text= text.Substring(i, text.Length-i);
            return ident;
        }
    }

}
