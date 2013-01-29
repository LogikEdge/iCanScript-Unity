using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public static class iCS_LicenseUtil {
    // ----------------------------------------------------------------------
    public static byte[] GetMD5Hash(string input) {
        return GetMD5Hash(Encoding.UTF8.GetBytes(input));                
    }
    // ----------------------------------------------------------------------
    public static byte[] GetMD5Hash(byte[] input) {
        return MD5.Create().ComputeHash(input);                
    }
    // ----------------------------------------------------------------------
    public static byte[] Xor(byte[] a, byte[] b) {
        int maxLen= Mathf.Max(a.Length, b.Length);
        byte[] result= new byte[maxLen];
        for(int i= 0; i < maxLen; ++i) {
            if(i < a.Length && i < b.Length) {
                result[i]= (byte)((int)(a[i]) ^ (int)(b[i]));
            }
            else {
                result[i]= i < a.Length ? a[i] : b[i];
            }
        }
        return result;
    }
    // ----------------------------------------------------------------------
    public static bool IsEqual(byte[] a, byte[] b) {
        return IsZero(Xor(a,b));
    }
    // ----------------------------------------------------------------------
    public static bool IsZero(byte[] a) {
        foreach(var b in a) {
            if(b != 0) return false;
        }
        return true;
    }
    // ----------------------------------------------------------------------
    public static int GetSignature(byte[] securityCode) {
        if(securityCode.Length < 2) return 0;
        int signature= 256*securityCode[0]+securityCode[1];
        for(int i= 2; i < securityCode.Length-1; i+= 2) {
            int newValue= 256*securityCode[i]+securityCode[i+1];
            if(newValue != signature) return 0;
        }
        return signature;
    }
    // ----------------------------------------------------------------------
    public static byte[] FromString(string hexString) {
        List<byte> result= new List<byte>();
        hexString.ToUpper();
        int len= hexString.Length;
        for(int i= 0; i < len; i+= 3) {
            Char c= hexString[i];
            byte value= 0;
            if(Char.IsDigit(c)) value= (byte)(c-'0');
            if(Char.IsLetter(c) && c >= 'A' && c <= 'F') value= (byte)(10+(c-'A'));
            value <<= 4;
            c= hexString[i+1];
            if(Char.IsDigit(c)) value+= (byte)(c-'0');
            if(Char.IsLetter(c) && c >= 'A' && c <= 'F') value+= (byte)(10+(c-'A'));
            result.Add(value);
        }
        return result.ToArray();
    }
    // ----------------------------------------------------------------------
    public static string ToString(byte[] securityCode) {
        string result= "";
        int len= securityCode.Length;
        for(int i= 0; i < len; ++i) {
            result+= securityCode[i].ToString("x2");
            if(i != len-1) result+= '-';
        }
        return result;
    }
}
