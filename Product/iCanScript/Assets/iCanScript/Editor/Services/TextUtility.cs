using UnityEngine;
using System;
using System.Text;
using System.Collections;

namespace iCanScript.Internal {
    
    public static class TextUtility {

        // =========================================================================
        /// Trim the space characters at the start of each lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static string TrimStartingWhiteSpaces(string line, ref int maxWhiteSpaces) {
            if(maxWhiteSpaces <= 0) return line;
            // -- Start by converting leading white spaces to spaces. --
            var len= line.Length;
            var noTabs= new StringBuilder(line.Length);
            for(int i= 0; i < len; ++i) {
                char c= line[i];
                switch(c) {
                    case ' ': noTabs.Append(c); break;
                    case '\t': noTabs.Append("        "); break;
                    default: {
                        noTabs.Append(line.Substring(i, len-i));
                        i= len;
                        break;
                    }
                }
            }
            line= noTabs.ToString();
            // -- Remove leading white spaces. --
            len= line.Length;
            var result= new StringBuilder(len);
            for(int i= 0; i < len; ++i) {
                char c= line[i];
                if(c != ' ' || i >= maxWhiteSpaces) {
                    result.Append(line.Substring(i, len-i));
                    if(i < maxWhiteSpaces) {
                        maxWhiteSpaces= i;
                    }
                    break;
                }
            }
            return result.ToString();
        }
        
        // =========================================================================
        /// Splits the given string into an array of lines.
        ///
        /// @param text The input text to split into lines.
        /// @return The array of text lines.
        ///
        public static string[] SplitIntoLines(string text) {
            // Split into lines.
            return text.Split(new Char[1]{'\n'});
        }

        // =========================================================================
        /// Trim the space characters at the end of each lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimEnd(ref string[] lines) {
            // -- Remove trailing blank lines. --
            TrimEndingBlankLines(ref lines);
            // -- Trim end of each line. --
            int len= lines.Length;
            for(int i= 0; i < len; ++i) {
                lines[i].TrimEnd(null);
            }
        }

        // =========================================================================
        /// Trim the space characters at the start of each lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimStart(ref string[] lines) {
            // -- Remove trailing blank lines. --
            TrimStartingBlankLines(ref lines);
            // -- Trim end of each line. --
            int len= lines.Length;
            for(int i= 0; i < len; ++i) {
                lines[i].TrimStart(null);
            }
        }

        // =========================================================================
        /// Remove trailing blank lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimEndingBlankLines(ref string[] lines) {
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

        // =========================================================================
        /// Remove leading blank lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimStartingBlankLines(ref string[] lines) {
            Array.Reverse(lines);
            TrimEndingBlankLines(ref lines);
            Array.Reverse(lines);
        }

        // =========================================================================
        /// Remove leading and trailing blank lines.
        ///
        /// @param lines Reference to the array of lines to be trimed.
        ///
        public static void TrimBlankLines(ref string[] lines) {
            TrimEndingBlankLines(ref lines);
            TrimStartingBlankLines(ref lines);
        }

    }
}