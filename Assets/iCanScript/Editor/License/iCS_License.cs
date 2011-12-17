using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System.Security;
using System.Collections;

public static class iCS_License {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    static byte[]  mySerialNumber= new byte[0];
    
    // ----------------------------------------------------------------------
    static public byte[] SerialNumber {
        get { return mySerialNumber; }
        set { mySerialNumber= value; }
    }
    
    // ----------------------------------------------------------------------
    public static byte[] Decode() {
        return iCS_LicenseUtil.Xor(iCS_FingerPrint.FingerPrint, mySerialNumber);
    }

    // ----------------------------------------------------------------------
    public static new string ToString() {
        return iCS_LicenseUtil.ToString(mySerialNumber);
    }
}
