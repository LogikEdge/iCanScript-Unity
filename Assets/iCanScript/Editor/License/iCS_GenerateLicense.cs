using UnityEngine;
using System.Collections;

public static class iCS_LicenseGenerator {

    public static byte[] Standard {
        get {
            byte[] fingerPrint= iCS_FingerPrint.FingerPrint;
            byte[] serialNumber= new byte[fingerPrint.Length];
            int b0= (((int)iCS_LicenseType.Standard >>8) & 255);
            int b1= ((int)iCS_LicenseType.Standard & 255);
            for(int i= 0; i < fingerPrint.Length; ++i) {
                serialNumber[i]= (byte)((int)fingerPrint[i] ^ ((i & 1) == 0 ? b0 : b1));
            }
            return serialNumber;
        }
    }

    public static byte[] Pro {
        get {
            byte[] fingerPrint= iCS_FingerPrint.FingerPrint;
            byte[] serialNumber= new byte[fingerPrint.Length];
            int b0= (((int)iCS_LicenseType.Pro >>8) & 255);
            int b1= ((int)iCS_LicenseType.Pro & 255);
            for(int i= 0; i < fingerPrint.Length; ++i) {
                serialNumber[i]= (byte)((int)fingerPrint[i] ^ ((i & 1) == 0 ? b0 : b1));
            }
            return serialNumber;
        }
    }
}
