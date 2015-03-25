using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Text;
using System.Security;
using System.Security.Cryptography;

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
    public static string NicifyName(string name) {
        var result= new StringBuilder();
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
