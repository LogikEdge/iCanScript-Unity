using UnityEngine;
using System;
using System.Text;
using System.Collections;
using iCanScript;

namespace iCanScript.Internal {
    
    public static class NameUtility {
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
            string prefix= "";
            NamingScheme namingScheme= NamingScheme.LOWER_CAMEL_CASE;
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
            string prefix= "";
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
			// -- iCanScript remains intact --
    		if(name == "iCanScript") return name;
            bool prependICanScript= false;
            if(name.StartsWith("iCanScript")) {
                prependICanScript= true;
                name= name.Substring(10, name.Length-10);
            }
			// -- Convert standard operators --
            if(name == "op_Assignment")         return "Set";            
            if(name == "op_Equality")           return "Is Equal";
            if(name == "op_Inequality")         return "Is Not Equal";
            if(name == "op_Addition")           return "Add";
            if(name == "op_Subtraction")        return "Subtract";
            if(name == "op_Multiply")           return "Multiply";
            if(name == "op_Division")           return "Division";
            if(name == "op_LeftShift")          return "Left Shift";
            if(name == "op_RightShift")         return "Right Shift";
            if(name == "op_AdditionAssign")     return "Add than Set";
            if(name == "op_SubtractionAssign")  return "Subtract than Set";
            if(name == "op_MultiplyAssign")     return "Multiply than Set";
            if(name == "op_DivisionAssign")     return "Division than Set";
            if(name == "op_LeftShiftAssign")    return "Left Shift than Set";
            if(name == "op_RightShiftAssign")   return "Right Shift than Set";
			if(name == "op_Increment")          return "Increment";
			if(name == "op_Decrement")          return "Decrement";
			if(name == "op_UnaryNegation")      return "Negate";
			if(name == "op_GreaterThan")        return "operator >";
			if(name == "op_LessThan")           return "operator <";
			if(name == "op_GreaterThanOrEqual")	return "operator >=";
			if(name == "op_LessThanOrEqual")    return "operator <=";
            if(name == "op_LogicalNot")         return "Inverse";
            if(name == "op_LogicalOr")          return "Conditional Or";
            if(name == "op_LogicalAnd")         return "Conditional And";
            if(name == "op_BitwiseOr")          return "Or";
            if(name == "op_BitwiseAnd")         return "And";
            if(name == "op_ExclusiveOr")        return "Xor";
            if(name == "op_BitwiseOrAssign")    return "Or than Set";
            if(name == "op_BitwiseAndAssign")   return "And than Set";
            if(name == "op_ExclusiveOrAssign")  return "Xor than Set";
			if(name == "get_Item")		        return "Get Item At[idx]";
			if(name == "set_Item")		        return "Set Item At[idx]";
			// -- Create nice readable name --
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
                    if(result.Length != 0 && (wasLetter || wasDigit)) {
                        result.Append(' ');                    
                    }
                    result.Append(c);
                    upperNext= true;
                    wasUpperCase= false;
                    wasLetter= false;
                    wasDigit= false;
                }
            }
            return (prependICanScript ? "iCanScript ":"")+result.ToString();
        }

    	// =================================================================================
    	// TYPE NAME UTILITIES
        // ---------------------------------------------------------------------------------
        /// Formats the given type for user display.
        ///
        /// Format for all name in the visual editor is word seperated with first letter
        /// of each word in upper case.
        ///
        /// @param type The type for which to generate a formated name.
        /// @return The formated name for purpose of visual script display.
        ///
        public static string ToDisplayName(Type type) {
            var name= ToDisplayNameNoGenericArguments(type);
    		// -- Special case for generic types --
            if(type.IsGenericType) {
    			// -- Add the generic arguments --
    			name+= ToDisplayGenericArguments(type);
            }
            return name;
    	}
	
        // ---------------------------------------------------------------------------------
        /// Formats the given type for user display.
        ///
        /// Format for all name in the visual editor is word seperated with first letter
        /// of each word in upper case.
        ///
        /// @param type The type for which to generate a formated name.
        /// @return The formated name for purpose of visual script display.
        ///
        public static string ToDisplayNameNoGenericArguments(Type type) {
            var name= ToDisplayName(iCS_Types.TypeName(type));
    		// -- Special case for generic types --
            if(type.IsGenericType) {
                // -- Remove number of parameter info --
                int end= name.IndexOf('`');
                if(end > 0 && end < name.Length) {
                    name= name.Substring(0, end);
                }
            }
            return name;
    	}
	
        // ---------------------------------------------------------------------------------
        /// Formats the given generic arguments for user display.
        ///
        /// Format for all name in the visual editor is word seperated with first letter
        /// of each word in upper case.
        ///
        /// @param type The type from which to extract the type arguments.
        /// @return The formated name for purpose of visual script display.
        ///
        public static string ToDisplayGenericArguments(Type type) {
    		return ToDisplayGenericArguments(type.GetGenericArguments());
    	}

        // ---------------------------------------------------------------------------------
        /// Formats the given generic arguments for user display.
        ///
        /// Format for all name in the visual editor is word seperated with first letter
        /// of each word in upper case.
        ///
        /// @param genericArguments The list of arguments.
        /// @return The formated name for purpose of visual script display.
        ///
        public static string ToDisplayGenericArguments(Type[] genericArguments) {
    		// -- Add the generic arguments --
            var name= new StringBuilder("<", 32);
            var len= genericArguments.Length;
            for(int i= 0; i < len; ++i) {
    			name.Append(genericArguments[i].Name);
                if(i < len-1) {
    				name.Append(" , ");
                }
            }
    		name.Append(">");
    		return name.ToString();
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
    	/// Converts internal runtime names to user displayable name.
    	///
    	/// @param runtimeName The internal name to be converted.
    	/// @return A user displayable name corrsponding the the given runtime name.
    	///
    	public static string ConvertRuntimeNameToDisplayName(string runtimeName) {
    		if(runtimeName == "op_GreaterThan")        	return "operator >";
    		if(runtimeName == "op_GreaterThanOrEqual") 	return "operator >=";
    		if(runtimeName == "op_LessThan")        	return "operator <";
    		if(runtimeName == "op_LessThanOrEqual") 	return "operator <=";
    		return runtimeName;
    	}
	
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
                case '|': return "Or";
                case '!': return "Not";
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
    	/// Adds the given prefix to the given name.
    	///
    	/// Special cae is taken for the 'a' prefix when the name starts with a vowel.
    	///
    	/// @param prefix The prefix to use.
    	/// @param name The name to be combined with the prefix.
    	/// @return The combined prefix and name.
    	///
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

}
