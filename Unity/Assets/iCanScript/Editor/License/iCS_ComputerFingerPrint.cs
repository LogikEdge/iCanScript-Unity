using System.Security.Cryptography;
using System.Text;

public static class iCS_ComputerFingerPrint {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    static byte[]  ourFingerPrint;
    
    // ----------------------------------------------------------------------
    static iCS_ComputerFingerPrint() {
        ourFingerPrint= iCS_LicenseController.GetMD5Hash(System.Environment.MachineName);        
    }
    // ----------------------------------------------------------------------
    public static byte[] FingerPrint {
        get { return ourFingerPrint; }
    }
    // ----------------------------------------------------------------------
    public static new string ToString() {
        return iCS_LicenseController.ToString(FingerPrint);
    }
}
