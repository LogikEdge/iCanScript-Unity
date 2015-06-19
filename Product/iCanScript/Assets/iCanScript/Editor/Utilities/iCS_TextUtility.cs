using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Text;
using System.Security;
using System.Security.Cryptography;

namespace iCanScript.Internal {
    
    public static class iCS_TextUtility {
        // ----------------------------------------------------------------------
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        // ---------------------------------------------------------------------------------
        // Converts the given string to a C# class name.
        public static string ToCSharpName(string proposedName) {
            StringBuilder fileName= new StringBuilder();
            foreach(var c in proposedName) {
                if(!Char.IsWhiteSpace(c)) {
                    fileName.Append((c == '_' || Char.IsLetterOrDigit(c)) ? c : '_');
                }
            }
            return fileName.ToString();
        }

        // ---------------------------------------------------------------------------------
    	// Converts a string to it Unicode equivalent.
    	public static string ToASCII(string inputString) {
    		return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(inputString));
    	}

        // ---------------------------------------------------------------------------------
        public static bool StartsWithAVowel(string str) {
            switch(Char.ToUpper(str[0])) {
                case 'A': case 'E': case 'I': case 'O': case 'U': return true;
            }
            return false;
        }

		// =================================================================================
		/// Combines an array of string inserting a seperator between each string.
		///
		/// @param list The array of string to combine.
		/// @param seperator The seperator to insert between each string.
		/// @return The combined string.
		///
		public static string CombineWith(string[] list, string seperator) {
			var len= list.Length;
			if(list == null || len == 0) return "";
			var result= new StringBuilder(list[0], 128);
			for(int i= 1; i < len; ++i) {
				result.Append(seperator);
				result.Append(list[i]);
			}
			return result.ToString();
		}
    }

}
