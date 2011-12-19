using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;

public static class iCS_LicenseFile {
    // ======================================================================
    // File Constants
    // ----------------------------------------------------------------------
    const string CompanyName= "DisruptiveSoftware";
    const string ProductName= "iCanScript";
    const string LicenseFileName= "iCanScript.license";
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public enum LicenseTypeEnum { Demo= 0, Standard= 0x6719, Pro= 0x9823 }
    public static LicenseTypeEnum  LicenseType = LicenseTypeEnum.Demo; 
    public static string           CustomerName= "";
    public static string           InstallDate = "";
    public static byte[]           SerialNumber= new byte[16];
    public static byte[]           UnlockKey   = new byte[16];    
    public static byte[]           MD5Hash     = new byte[16];
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    static string CompanyPath {
        get {
            string appPath= System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            return appPath+"/"+CompanyName;            
        }
    }
    // ----------------------------------------------------------------------
    public static bool Exists {
        get {
            return File.Exists(CompanyPath+"/"+ProductName+"/"+LicenseFileName);
        }
    }
    // ----------------------------------------------------------------------
    public static bool HasBeenTamperedWith {
        get {
            return Exists && IsCorrupted;            
        }
    }
    // ----------------------------------------------------------------------
    public static bool IsCorrupted {
        get {
            return !iCS_LicenseUtil.IsZero(iCS_LicenseUtil.Xor(ComputeMD5_v1(), MD5Hash));            
        }
    }
    

    // ======================================================================
    // Methods
    // ----------------------------------------------------------------------
    static iCS_LicenseFile() {
        if(Exists) Read();
    }
    // ----------------------------------------------------------------------
    public static void Reset() {
        CustomerName= "";
        InstallDate = "";
        LicenseType = LicenseTypeEnum.Demo; 
        SerialNumber= new byte[16];
        UnlockKey   = new byte[16];            
        MD5Hash= ComputeMD5_v1();
        Write();
    }
    // ----------------------------------------------------------------------
    public static void FillCustomerInformation(string name, string serial, LicenseTypeEnum licenceType) {
        CustomerName= name;
        SerialNumber= iCS_LicenseUtil.FromString(serial);
        LicenseType= licenceType;
        InstallDate= DateTime.Now.Date.ToString("d");
        MD5Hash= ComputeMD5_v1();
        Write();
    }
    
    // ----------------------------------------------------------------------
    public static byte[] Decode() {
        return iCS_LicenseUtil.Xor(iCS_LicenseUtil.Xor(iCS_FingerPrint.FingerPrint, UnlockKey), SerialNumber);
    }

    // ----------------------------------------------------------------------
    public static void SetUnlockKey(byte[] unlockKey) {
        UnlockKey= unlockKey;
        MD5Hash= ComputeMD5_v1();
        Write();
    }
    
    // ----------------------------------------------------------------------
    static byte[] ComputeMD5_v1() {
        return iCS_LicenseUtil.Xor(ComputeMD5(), iCS_LicenseUtil.GetMD5Hash("iCanScript-v1"));
    }
    // ----------------------------------------------------------------------
    static byte[] ComputeMD5() {
        byte[] md5Hash= iCS_LicenseUtil.GetMD5Hash(CustomerName);
        md5Hash= iCS_LicenseUtil.Xor(iCS_LicenseUtil.GetMD5Hash(InstallDate), md5Hash);
        md5Hash= iCS_LicenseUtil.Xor(iCS_LicenseUtil.GetMD5Hash(SerialNumber), md5Hash);
        md5Hash= iCS_LicenseUtil.Xor(iCS_LicenseUtil.GetMD5Hash(UnlockKey), md5Hash);        
        md5Hash= iCS_LicenseUtil.Xor(iCS_LicenseUtil.GetMD5Hash(LicenseType.ToString()), md5Hash);        
        return md5Hash;
    }
    
    // ----------------------------------------------------------------------
    static string PreparePath() {
        string companyPath= CompanyPath;
        if(!Directory.Exists(companyPath)) {
            Directory.CreateDirectory(companyPath);
        }
        string iCanScriptPath= companyPath+"/"+ProductName;
        if(!Directory.Exists(iCanScriptPath)) {
            Directory.CreateDirectory(iCanScriptPath);            
        }
        return iCanScriptPath;
    }

    // ----------------------------------------------------------------------
    static void Write() {
        string folder= PreparePath();
        string licenseFile= folder+"/"+LicenseFileName;
        StreamWriter stream= new StreamWriter(licenseFile);
        stream.WriteLine(CustomerName);
        stream.WriteLine(InstallDate);
        stream.Write(new char[1]{(char)LicenseType}, 0, 1);
        stream.Write(BytesToChars(SerialNumber), 0, 16);
        stream.Write(BytesToChars(UnlockKey), 0, 16);
        stream.Write(BytesToChars(MD5Hash), 0, 16);
        stream.Close();
    }
    
    // ----------------------------------------------------------------------
    static void Read() {
        string folder= PreparePath();
        string licenseFile= folder+"/"+LicenseFileName;
        StreamReader stream= new StreamReader(licenseFile);
        CustomerName= stream.ReadLine();
        InstallDate= stream.ReadLine();
        char[] tmp= new char[16];
        stream.Read(tmp, 0, 1);
        LicenseType= (LicenseTypeEnum)tmp[0];
        stream.Read(tmp, 0, 16);
        SerialNumber= CharsToBytes(tmp);
        stream.Read(tmp, 0, 16);
        UnlockKey= CharsToBytes(tmp);
        stream.Read(tmp, 0, 16);
        MD5Hash= CharsToBytes(tmp);
        stream.Close();
    }

    // ----------------------------------------------------------------------
    static char[] BytesToChars(byte[] bytes) {
        char[] chars= new char[bytes.Length];
        for(int i= 0; i < bytes.Length; ++i) {
            chars[i]= (char)bytes[i];
        }
        return chars;
    }
    // ----------------------------------------------------------------------
    static byte[] CharsToBytes(char[] chars) {
        byte[] bytes= new byte[chars.Length];
        for(int i= 0; i < chars.Length; ++i) {
            bytes[i]= (byte)chars[i];
        }
        return bytes;
    }

    // ----------------------------------------------------------------------
    public static new string ToString() {
        return "<Customer Name: "+CustomerName+">"+
               "<Serial #: "+iCS_LicenseUtil.ToString(SerialNumber)+">"+
               "<Install Date: "+InstallDate+">"+
               "<License Type: "+LicenseType+">"+
               "<Unlock Key: "+iCS_LicenseUtil.ToString(UnlockKey)+">"+
               "<MD5 Hash: "+iCS_LicenseUtil.ToString(MD5Hash)+">";
    }

}
