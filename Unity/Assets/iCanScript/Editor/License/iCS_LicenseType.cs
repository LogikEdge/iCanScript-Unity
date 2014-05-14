using UnityEngine;
using System;
using System.Collections;

public enum iCS_LicenseTypeEnum {
    Trial= 0, Community= 1, Standard= 0x5f23, Pro= 0xdc45
}

public static class iCS_LicenseType {
//    public static iCS_LicenseTypeEnum ActualEdition {
//        get {
//            if(IsPro) return iCS_LicenseTypeEnum.Pro;
//            if(IsStandard) return iCS_LicenseTypeEnum.Standard;
//            if(IsTrial) return iCS_LicenseTypeEnum.Trial;
//            return iCS_LicenseTypeEnum.Community;
//        }
//    }
//    public static iCS_LicenseTypeEnum FeatureEdition {
//        get {
//            if(IsPro) return iCS_LicenseTypeEnum.Pro;
//            if(IsStandard) return iCS_LicenseTypeEnum.Standard;
//            if(IsTrial) return iCS_LicenseTypeEnum.Pro;
//            return iCS_LicenseTypeEnum.Community;
//        }        
//    }
//    public static bool IsTrial {
//        get {
//            int signature= Signature;
//            if(signature == (int)iCS_LicenseTypeEnum.Standard || signature == (int)iCS_LicenseTypeEnum.Pro) {
//                return false;
//            }
//            return RemainingTrialDays >= 0;
//        }
//    }
//    public static bool IsCommunity {
//        get {
//            int signature= Signature;
//            if(signature == (int)iCS_LicenseTypeEnum.Standard || signature == (int)iCS_LicenseTypeEnum.Pro) {
//                return false;
//            }
//            return RemainingTrialDays < 0;            
//        }
//    }
//    public static bool IsStandard {
//        get {
//            int signature= Signature;
//            if(signature == (int)iCS_LicenseTypeEnum.Standard) {
//                return true;
//            }
//            return false;            
//        }
//    }
//    public static bool IsPro {
//        get {
//            int signature= Signature;
//            if(signature == (int)iCS_LicenseTypeEnum.Pro) {
//                return true;
//            }
//            return false;            
//        }
//    }
//    static int Signature {
//        get {
//            var computerFingerPrint= iCS_ComputerFingerPrint.FingerPrint;
//            var activationKey= iCS_LicenseUtil.FromString(iCS_PreferencesController.UserLicense);
//            var fullKey= iCS_LicenseUtil.Xor(computerFingerPrint, activationKey);
//            int signature= iCS_LicenseUtil.GetSignature(fullKey);
//            return signature;
//        }
//    }
    public static int RemainingTrialDays {
        get {
            var today= DateTime.Today;
            var trialEndDate= iCS_PreferencesController.TrialStartDate.AddDays(15);
            var remainingTrialPeriod= trialEndDate.Subtract(today);
            return remainingTrialPeriod.Days;
        }
    }
    
    
}