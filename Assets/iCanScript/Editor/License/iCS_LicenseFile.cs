using UnityEngine;
using System;
using System.IO;
using System.Collections;

public static class iCS_LicenseFile {
    public enum LicenseTypeEnum { Demo, Standard, Pro }
    public static LicenseTypeEnum  LicenseType = LicenseTypeEnum.Demo; 
    public static string           CustomerName= null;
    public static string           SerialNumber= null;
    public static string           InstallDate = null;
    public static byte[]           MD5Hash     = null;
    
    public static void FillCustomerInformation(string name, string serial, DateTime installDate) {
        CustomerName= name;
        SerialNumber= serial;
        InstallDate= installDate.Date.ToString("d");
        MD5Hash= ComputeMD5_v1();
    }
    
    static byte[] ComputeMD5_v1() {
        return iCS_LicenseUtil.Xor(ComputeMD5(), iCS_LicenseUtil.GetMD5Hash("iCanScript-v1"));
    }
    static byte[] ComputeMD5() {
        byte[] md5Hash= iCS_LicenseUtil.GetMD5Hash(CustomerName);
        md5Hash= iCS_LicenseUtil.Xor(iCS_LicenseUtil.GetMD5Hash(SerialNumber), md5Hash);
        md5Hash= iCS_LicenseUtil.Xor(iCS_LicenseUtil.GetMD5Hash(InstallDate), md5Hash);
        return md5Hash;
    }
    
    public static bool IsValid_v1() {
        return iCS_LicenseUtil.IsZero(iCS_LicenseUtil.Xor(MD5Hash, ComputeMD5_v1()));
    }
    
    public static new string ToString() {
        return "<Customer Name: "+CustomerName+">"+
               "<Serial #: "+SerialNumber+">"+
               "<Install Date: "+InstallDate+">"+
               "<License Type: "+LicenseType+">"+
               "<MD5 Hash: "+iCS_LicenseUtil.ToString(MD5Hash)+">";
    }

    static string PreparePath() {
        string appPath= System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        string companyPath= appPath+"/DisruptiveSoftware";
        if(!Directory.Exists(companyPath)) {
            Directory.CreateDirectory(companyPath);
        }
        string iCanScriptPath= companyPath+"/iCanScript";
        if(!Directory.Exists(iCanScriptPath)) {
            Directory.CreateDirectory(iCanScriptPath);            
        }
        return iCanScriptPath;
    }

    public static void Write() {
        string folder= PreparePath();
        string licenseFile= folder+"/iCanScript.license";
        StreamWriter stream= new StreamWriter(licenseFile);
        stream.Write(ToString());
        stream.Close();
    }
    
    public static void Read() {
        StreamReader stream= new StreamReader("iCanScript.license");
        stream.Close();
    }
}
