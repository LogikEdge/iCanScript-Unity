using UnityEngine;
using System;
using System.Text;
using System.Collections;

public static class iCS_ObjectNames {
    public enum NamingScheme {
        VISUAL_EDITOR,
        LOWER_CAMEL_CASE, UPPER_CAMEL_CASE,
        SNAKE_CASE, LOWER_SNAKE_CASE, UPPER_SNAKE_CASE
    };
    
    // =================================================================================
    // Programmatic name conversions
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a function parameter name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToFunctionParameterName(string name) {
        string prefix= "a";
        NamingScheme namingScheme= NamingScheme.UPPER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a local variable name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToLocalVariableName(string name) {
        string prefix= "the";
        NamingScheme namingScheme= NamingScheme.LOWER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a public class field name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPublicFieldName(string name) {
        string prefix= "my";
        NamingScheme namingScheme= NamingScheme.LOWER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a private class field name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPrivateFieldName(string name) {
        string prefix= "p_";
        NamingScheme namingScheme= NamingScheme.LOWER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a public class field name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPublicStaticFieldName(string name) {
        string prefix= "our";
        NamingScheme namingScheme= NamingScheme.LOWER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a private class field name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPrivateStaticFieldName(string name) {
        string prefix= "p_our";
        NamingScheme namingScheme= NamingScheme.LOWER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a public function name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPublicFunctionName(string name) {
        string prefix= null;
        NamingScheme namingScheme= NamingScheme.UPPER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a private function name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPrivateFunctionName(string name) {
        string prefix= null;
        NamingScheme namingScheme= NamingScheme.UPPER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a public function name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPublicStaticFunctionName(string name) {
        string prefix= null;
        NamingScheme namingScheme= NamingScheme.UPPER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a private function name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToPrivateStaticFunctionName(string name) {
        string prefix= null;
        NamingScheme namingScheme= NamingScheme.UPPER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }
    // ---------------------------------------------------------------------------------
    /// Convert the given name to a type name using the predefined naming
    /// scheme.
    ///
    /// @param name The name to be converted.
    /// @return The converted name.
    ///
    public static string ToTypeName(string name) {
        string prefix= null;
        NamingScheme namingScheme= NamingScheme.UPPER_CAMEL_CASE;
        return ToCodeName(namingScheme, name, prefix);
    }

    // ---------------------------------------------------------------------------------
    /// Converts the given name to a code formatter name usng the given naming scheme.
    /// A prefix can also be prepended.
    ///
    /// @param namingScheme The naming scheme used in the conversion.
    /// @param name The name to be converted.
    /// @param prefix The string to be prepended.
    /// @return The converted name.
    ///
    public static string ToCodeName(NamingScheme namingScheme, string name, string prefix= null) {
        // Don't prepend the prefix if it is already present.
        if(!string.IsNullOrEmpty(prefix)) {
            if(name.StartsWith(prefix)) {
                prefix= null;
            }
        }
        // Adjust Camel case if prefix exists.
        if(!string.IsNullOrEmpty(prefix)) {
            bool prefixEndsWithUnderscore= prefix[prefix.Length-1] == '_';
            if(prefixEndsWithUnderscore == false) {
                switch(namingScheme) {
                    case NamingScheme.LOWER_CAMEL_CASE: {
                        namingScheme= NamingScheme.UPPER_CAMEL_CASE;
                        break;
                    }
                    case NamingScheme.SNAKE_CASE:
                    case NamingScheme.LOWER_SNAKE_CASE:
                    case NamingScheme.UPPER_SNAKE_CASE: {
                        prefix+= '_';
                        break;
                    }
                }                
            }
        }
        // Perform naming scheme conversion.
        name= ToNamingScheme(namingScheme, name);
        // Add prefix.
        if(!string.IsNullOrEmpty(prefix)) {
            name= AddPrefix(prefix, name);
        }
        return name;
    }

    // ---------------------------------------------------------------------------------
    public static string ToNamingScheme(NamingScheme namingScheme, string name) {
        switch(namingScheme) {
            case NamingScheme.VISUAL_EDITOR: {
                return ToDisplayName(name);
            }
            case NamingScheme.LOWER_CAMEL_CASE: {
                return ToLowerCamelCase(name);
            }
            case NamingScheme.UPPER_CAMEL_CASE: {
                return ToUpperCamelCase(name);
            }
            case NamingScheme.SNAKE_CASE: {
                return ToSnakeCase(name);
            }
            case NamingScheme.LOWER_SNAKE_CASE: {
                return ToLowerSnakeCase(name);
            }
            case NamingScheme.UPPER_SNAKE_CASE: {
                return ToUpperSnakeCase(name);
            }
        }
        return ToUpperCamelCase(name);
    }
    
    // =================================================================================
    // Visual Editor name conversions
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

    // =================================================================================
    // Snake Case conversions
    // ---------------------------------------------------------------------------------
    /// Converts the given name to a Snake case name. (words seperated by an underscore).
    ///
    /// @param name The name to be converted.
    /// @return The upper Camel case formated name.
    ///
    public static string ToSnakeCase(string name) {
        if(name == null) return null;
        name= ToDisplayName(name);
        var result= new StringBuilder(128);
        bool wasSpace= false;
        for(int i= 0; i < name.Length; ++i) {
            var c= name[i];
            if(Char.IsWhiteSpace(c)) {
                if(!wasSpace) {
                    wasSpace= true;
                    result.Append('_');
                }
            }
            else {
                wasSpace= false;
                if(Char.IsLetterOrDigit(c)) {
                   result.Append(c);
               }
               else {
                   result.Append(ConvertSpecialChar(c));                
               }                
            }
        }        
        return result.ToString();        
    }
    // ---------------------------------------------------------------------------------
    /// Converts the given name to a lower Snake case name.
    /// (words seperated by an underscore).
    ///
    /// @param name The name to be converted.
    /// @return The lower Snake case formated name.
    ///
    public static string ToLowerSnakeCase(string name) {
        return ToSnakeCase(name).ToLower();
    }
    // ---------------------------------------------------------------------------------
    /// Converts the given name to a upper Snake case name.
    /// (words seperated by an underscore).
    ///
    /// @param name The name to be converted.
    /// @return The upper Snake case formated name.
    ///
    public static string ToUpperSnakeCase(string name) {
        return ToSnakeCase(name).ToUpper();
    }

    // =================================================================================
    // Camel Case conversions
    // ---------------------------------------------------------------------------------
    /// Converts the given name to a upper Camel case name.
    ///
    /// @param name The name to be converted.
    /// @return The Snake case formated name.
    ///
    public static string ToUpperCamelCase(string name) {
        if(name == null) return null;
        name= ToDisplayName(name);
        var result= new StringBuilder(128);
        for(int i= 0; i < name.Length; ++i) {
            var c= name[i];
            if(Char.IsWhiteSpace(c)) continue;
            if(Char.IsLetterOrDigit(c)) {
                result.Append(c);
            }
            else {
                result.Append(ConvertSpecialChar(c));                
            }
        }        
        return result.ToString();
    }
    // ---------------------------------------------------------------------------------
    /// Converts the given name to a lower Camel case name.
    ///
    /// @param name The name to be converted.
    /// @return The lower Camel case formated name.
    ///
    public static string ToLowerCamelCase(string name) {
        if(name == null) return null;
        var result= ToUpperCamelCase(name);
        return Char.ToLower(result[0])+result.Substring(1);
    }

    // =================================================================================
    // Utilities
    // ---------------------------------------------------------------------------------
    /// Converts a special character into code valid code name.
    ///
    /// @param c The special character to be converted.
    /// @return A string that is a valid code name.
    ///
    static string ConvertSpecialChar(Char c) {
        switch(c) {
            case '+': return "Plus";
            case '-': return "Minus";
            case '*': return "Times";
            case '/': return "Divide";
            case '&': return "And";
            case '!': return "Or";
            case '<': return "SmallerThan";
            case '>': return "LargerThan";
            case '{': return "OpenCurlyBrace";
            case '}': return "CloseCurlyBrace";
            case '(': return "OpenParenthasis";
            case ')': return "CloseParenthasis";
            case '[': return "OpenBrace";
            case ']': return "CloseBrace";
            case '\'': return "Quote";
            case '\"': return "DoubleQuote";
            case '\\': return "Backslash";
            case '?': return "QuestionMark";
            case '.': return "Period";
            case ',': return "Comma";
            case ';': return "SemiComma";
            case ':': return "Colon";
            case '=': return "Equal";
            case '@': return "At";
            case '#': return "Number";
            case '$': return "Dollar";
            case '%': return "Percent";
            case '^': return "Exponent";
        }
        return "_";
    }
    
    // ---------------------------------------------------------------------------------
    public static string AddPrefix(string prefix, string name) {
        // Special case for "a" prefix.
        if(prefix == "a") {
            if(StartsWithAVowel(name)) {
                prefix= "an";
            }
        }
        return prefix+name;
    }

    // ---------------------------------------------------------------------------------
    /// Retruns true if the given name starts with vowel.
    ///
    /// @param name The name on which the first character will be tested.
    /// @return Returns _'true'_ if the name starts with a vowel.
    ///
    public static bool StartsWithAVowel(string name) {
        switch(Char.ToUpper(name[0])) {
            case 'A': case 'E': case 'I': case 'O': case 'U': return true;
        }
        return false;
    }

}
