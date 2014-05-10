using UnityEngine;
using System.Collections;

public static class iCS_UnlockKeyGenerator {

    public static byte[] Standard {
        get {
            byte[] fingerPrint= iCS_ComputerFingerPrint.FingerPrint;
            byte[] serialNumber= iCS_LicenseFile.SerialNumber;
            byte[] unlockKey= new byte[fingerPrint.Length];
            int b0= (((int)iCS_LicenseType.Standard >>8) & 255);
            int b1= ((int)iCS_LicenseType.Standard & 255);
            for(int i= 0; i < fingerPrint.Length; ++i) {
                unlockKey[i]= (byte)((int)fingerPrint[i] ^ (int)serialNumber[i] ^ ((i & 1) == 0 ? b0 : b1));
            }
            return unlockKey;
        }
    }

    public static byte[] Pro {
        get {
            byte[] fingerPrint= iCS_ComputerFingerPrint.FingerPrint;
            byte[] serialNumber= iCS_LicenseFile.SerialNumber;
            byte[] unlockKey= new byte[fingerPrint.Length];
            int b0= (((int)iCS_LicenseType.Pro >>8) & 255);
            int b1= ((int)iCS_LicenseType.Pro & 255);
            for(int i= 0; i < fingerPrint.Length; ++i) {
                unlockKey[i]= (byte)((int)fingerPrint[i] ^ (int)serialNumber[i] ^ ((i & 1) == 0 ? b0 : b1));
            }
            return unlockKey;
        }
    }
}
