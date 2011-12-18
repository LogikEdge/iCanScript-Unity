using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System.Security;
using System.Collections;

public static class iCS_License {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    static byte[]  myLicense= new byte[0];
    
    // ----------------------------------------------------------------------
    static public byte[] License {
        get { return myLicense; }
        set { myLicense= value; }
    }
    
    // ----------------------------------------------------------------------
    public static byte[] Decode() {
        return iCS_LicenseUtil.Xor(iCS_FingerPrint.FingerPrint, myLicense);
    }

    // ----------------------------------------------------------------------
    public static new string ToString() {
        return iCS_LicenseUtil.ToString(myLicense);
    }
}
