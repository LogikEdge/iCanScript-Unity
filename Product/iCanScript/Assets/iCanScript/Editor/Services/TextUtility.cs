using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
    public static class TextUtility {

    	// -------------------------------------------------------------------------
        /// Splits the given string into an array of lines.
        ///
        /// @param text The input text to split into lines.
        /// @return The array of text lines.
        ///
        public static string[] SplitIntoLines(string text) {
            // Split into lines.
            return text.Split(new Char[1]{'\n'});
        }

    	// -------------------------------------------------------------------------
        /// Remove trailing blank lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimTrailingLines(ref string[] lines) {
            // Remove empty trailing lines.
            for(int i= lines.Length-1; i >= 0; --i) {
                if(string.IsNullOrEmpty(lines[i].Trim())) {
                    Array.Resize(ref lines, i);
                }
                else {
                    break;
                }
            }
        }

    	// -------------------------------------------------------------------------
        /// Remove leading blank lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimLeadingLines(ref string[] lines) {
            Array.Reverse(lines);
            TrimTrailingLines(ref lines);
            Array.Reverse(lines);
        }

    	// -------------------------------------------------------------------------
        /// Remove leading and trailing blank lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimLines(ref string[] lines) {
            TrimTrailingLines(ref lines);
            TrimLeadingLines(ref lines);
        }

    }
}