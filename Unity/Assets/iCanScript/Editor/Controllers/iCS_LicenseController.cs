using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


public enum iCS_LicenseType {
    Trial= 0, Community= 1, Pro= 0xdc45
}

public static class iCS_LicenseController {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    static byte[]   ourFingerPrint;
    static byte[]   ourSignature;
    static int      ourReinitializeCnt= 0;
    static bool     hasProLicense= false;
    static bool     hasTrialLicense= false;
    static bool     hasCommunityLicense= true;
    static bool     isProOperatingMode= false;
    static bool     isCommunityOperatingMode= true;
    
    // =================================================================================
    // Installation
    // ---------------------------------------------------------------------------------
    static iCS_LicenseController() {
        Initialize();
    }
    public static void Initialize() {
        ourFingerPrint          = GetMD5Hash(System.Environment.MachineName);
        hasProLicense      = HasProLicense;
        hasTrialLicense    = HasTrialLicense;
        hasCommunityLicense= !(hasProLicense || hasTrialLicense);
        
        isProOperatingMode      = hasProLicense || hasTrialLicense;
        isCommunityOperatingMode= !isProOperatingMode;

        // Restart the trial if the version changed.
        var communityVersion= iCS_PreferencesController.TrialVersion;
        if(communityVersion.IsEqual(iCS_Version.Current) == false) {
            var remainingTrialDays= RemainingTrialDays;
            if(remainingTrialDays < 7) {
                remainingTrialDays= 7;
            }
            RestartTrial(remainingTrialDays);                
        }
        ourReinitializeCnt= 0;
    }

    public static void Start() {}
    public static void Shutdown() {}
    
    
    // ======================================================================
    // ----------------------------------------------------------------------
    public static byte[] FingerPrint {
        get { return ourFingerPrint; }
    }
    public static byte[] Signature {
        get {
            ourSignature= Xor(ourFingerPrint, FromString(iCS_PreferencesController.UserLicense));            
            return ourSignature;
        }
    }
    public static bool IsActivated {
        get {
            return hasProLicense;
        }
    }
    
    // ======================================================================
    // License Type
    // ----------------------------------------------------------------------
    public static bool IsProOperatingMode {
        get { Refresh(); return isProOperatingMode; }
    }
    public static bool IsCommunityOperatingMode {
        get { Refresh(); return isCommunityOperatingMode; }
    }
    static void Refresh() {
        if(++ourReinitializeCnt > 1000) {
            Initialize();
        }
    }
    public static string OperatingModeAsString() {
        if(hasProLicense)   return "Pro";
        if(hasTrialLicense) return "Pro (Trial)";
        return "Community";
    }
    // ----------------------------------------------------------------------
    public static bool HasTrialLicense {
        get {
            return iCS_EditionController.IsTrialEdition && RemainingTrialDays >= 0;
        }
    }
    public static bool HasProLicense {
        get {
            return iCS_EditionController.IsDevEdition || iCS_EditionController.IsStoreEdition;
        }
    }
    public static bool IsLicensed {
        get {
            return hasProLicense;
        }
    }
    public static string LicenseTypeAsString() {
        if(hasTrialLicense)     return "Trial";
        if(hasCommunityLicense) return "Community";
        if(hasProLicense)       return "Pro";
        return "Unknown";
    }
	public static string LicenseAsString() {
        return iCS_PreferencesController.UserLicense;
	}
    public static void RestartTrial(int timeoutInDays) {
        ResetUserLicense();
        iCS_PreferencesController.TrialStartDate= DateTime.Today.AddDays(timeoutInDays-15);
    }
    public static void ResetUserLicense() {
        iCS_PreferencesController.ResetUserLicense();
    }
    
    // ----------------------------------------------------------------------
    public static int RemainingTrialDays {
        get {
            var today= DateTime.Today;
            var trialEndDate= iCS_PreferencesController.TrialStartDate.AddDays(15);
            var remainingTrialPeriod= trialEndDate.Subtract(today);
            return remainingTrialPeriod.Days;
        }
    }
    
    
    // ======================================================================
    // Utilities
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
    public static byte[] FromString(string hexString) {
        List<byte> result= new List<byte>();
        int len= hexString.Length;
        for(int i= 0; i < len; i+= 3) {
            Char c= hexString[i];
            byte value= 0;
            if(Char.IsDigit(c)) value= (byte)(c-'0');
            if(Char.IsLetter(c)) {
                c= Char.ToUpper(c);
                if(c >= 'A' && c <= 'F') value= (byte)(10+(c-'A'));
                
            }
            value <<= 4;
            c= hexString[i+1];
            if(Char.IsDigit(c)) value+= (byte)(c-'0');
            if(Char.IsLetter(c)) {
                c= Char.ToUpper(c);
                if(c >= 'A' && c <= 'F') value+= (byte)(10+(c-'A'));
            }
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
    // ----------------------------------------------------------------------
    public static bool GetSignature(byte[] securityCode, out int key1, out int key2) {
        if(securityCode.Length < 4) {
            key1= key2= -1;
            return false;
        }
        int w0= 256*securityCode[0]+securityCode[1];
        int w1= 256*securityCode[2]+securityCode[3];
        int c= w0^w1;
        key1= 0;
        for(int i= 0, bit= 1, mask= 1; i < 16; ++i, mask <<= 1, bit <<= 1) {
            bit = (c^bit) & mask;
            key1 |= bit;
        }
        key2= w0 ^ key1;
        int r= (key1 << 2) & 0xfffc;
        r |= (key1 >> 14) & 3;
        for(int j= 4; j < securityCode.Length-1; j+= 2) {
            int newValue= (256*securityCode[j]+securityCode[j+1]) ^ key2;
            if(newValue != (r | 1)) {
                key1= key2= -1;
                return false;
            }
            r = (r << 1) & 0x1fffe;
            r = (r & 0xfffe) | ((r >> 16) & 1);
        }
        return true;
    }
    // ----------------------------------------------------------------------
    public static byte[] BuildSignature(byte[] toEncode, int k1, int k2) {
        var sig= new byte[toEncode.Length];
        int w0= 256*toEncode[0]+toEncode[1];
        int s0= w0^k1^k2;
        sig[0]= (byte)((s0 >> 8) & 0xff);
        sig[1]= (byte)(s0 & 0xff);
        int r= ((k1 << 1) & 0xfffe) | ((k1 >> 15) & 1);
        for(int i= 2; i < toEncode.Length-1; i+= 2) {
            int w= 256*toEncode[i]+toEncode[i+1];
            s0= w^(r | 1)^k2;
            sig[i]= (byte)((s0 >> 8) & 0xff);
            sig[i+1]= (byte)(s0 & 0xff);
            r= (r << 1) & 0x1fffe;
            r= (r & 0xfffe) | ((r >> 16) & 1);
        }
        return sig;
    }
    // ----------------------------------------------------------------------
    public static string CleanupLicenseString(string licenseStr) {
        var license= FromString(licenseStr);
        return ToString(license);
    }
    // ----------------------------------------------------------------------
    public static bool ValidateLicense(string licenseStr, out iCS_LicenseType licenseType, out uint version, out string errorMessage) {
        licenseType= iCS_LicenseType.Community;
        version= 0;
        var license= FromString(licenseStr);
        var securityCode= Xor(ourFingerPrint, license);
        int key1, key2;
        if(!GetSignature(securityCode, out key1, out key2)) {
            errorMessage= "Unable to decode license string=> "+licenseStr;
            return false;
        }
        licenseType= (iCS_LicenseType)key1;
        version= (uint)key2;
        if(licenseType != iCS_LicenseType.Pro) {
            errorMessage= "Invalid license type.";
            return false;
        }
        if(version < iCS_Config.MajorVersion) {
            errorMessage= "The license is for an older version of iCanScript=> "+version;
            return false;
        }
        errorMessage= null;
        return true;
    }
    // ----------------------------------------------------------------------
    public static void PurchaseUserLicense() {
        Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/16872");
    }
}
