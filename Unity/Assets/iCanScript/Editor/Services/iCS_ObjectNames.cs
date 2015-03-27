using UnityEngine;
using System;
using System.Text;
using System.Collections;

public static class iCS_ObjectNames {
    // ---------------------------------------------------------------------------------
    /// Formats the given name for user display.
    ///
    /// Format for all name in the visual editor is word seperated with first letter
    /// of each word in upper case.
    ///
    /// @param name The name to be formated.
    /// @return The formated name for purpose of visual script display.
    ///
    public static string ToDisplayName(string name) {
        var result= new StringBuilder(128);
        bool upperNext= true;
        bool wasUpperCase= false;
        bool wasLetter= false;
        bool wasDigit= false;
        for(int i= 0; i < name.Length; ++i) {
            var c= name[i];
            if(upperNext) {
                c= Char.ToUpper(c);
                upperNext= false;
            }
            if(c == '_' || Char.IsSeparator(c)) {
                upperNext= true;
                wasUpperCase= false;
                wasLetter= false;
                wasDigit= false;
            }
            else if(Char.IsDigit(c)) {
                if(!wasDigit && result.Length != 0) {
                    result.Append(' ');
                }
                result.Append(c);
                upperNext= true;
                wasUpperCase= false;
                wasLetter= false;
                wasDigit= true;
            }
            else if(Char.IsLetter(c)) {
                // Add space seperator
                if(!wasLetter && result.Length != 0) {
                    result.Append(' ');
                }
                else if(Char.IsUpper(c)) {
                    if(!wasUpperCase && result.Length != 0) {
                        result.Append(' ');
                    }
                }                    
                if(Char.IsUpper(c)) {
                    wasUpperCase= true;
                }
                else {
                    wasUpperCase= false;
                }
                result.Append(c);
                wasLetter= true;
                wasDigit= false;
            }
            else {
                if(result.Length != 0) {
                    result.Append(' ');                    
                }
                result.Append(c);
                upperNext= true;
                wasUpperCase= false;
                wasLetter= false;
                wasDigit= false;
            }
        }
        return result.ToString();
    }

}
