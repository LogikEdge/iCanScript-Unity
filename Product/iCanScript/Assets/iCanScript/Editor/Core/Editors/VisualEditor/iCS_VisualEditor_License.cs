using UnityEngine;
//using System.Management;
using System.Collections;

namespace iCanScript.Internal.Editor {
// ===========================================================================
// User license verification
// ===========================================================================
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ----------------------------------------------------------------------
//	public void ActivateLicense() {
//        Debug.Log("AppContentPath: "+EditorApplication.applicationContentsPath);
//        Debug.Log("AppPath: "+EditorApplication.applicationPath);
//        if(!iCS_LicenseFile.Exists) {
//            Debug.Log("Generating license file.");
//            iCS_LicenseFile.FillCustomerInformation("Michel Launier", "11-22-33-44-55-66-77-88-99-aa-bb-cc-dd-ee-ff-00", iCS_LicenseFile.LicenseTypeEnum.Pro);            
//            iCS_LicenseFile.SetUnlockKey(iCS_UnlockKeyGenerator.Pro);            
//        }
//        if(iCS_LicenseFile.IsCorrupted) {
//            EditorApplication.Beep();
//            EditorUtility.DisplayDialog("Corrupted iCanScript License File", "The iCanScript license file has been corrupted.  Disruptive Software will be advise of the situation and your serial number may be revoqued. iCanScript will go back to Demo mode.", "Clear License File");
//            iCS_LicenseFile.Reset();
//            iCS_LicenseFile.FillCustomerInformation("Michel Launier", "11-22-33-44-55-66-77-88-99-aa-bb-cc-dd-ee-ff-00", iCS_LicenseFile.LicenseTypeEnum.Pro);            
//            iCS_LicenseFile.SetUnlockKey(iCS_UnlockKeyGenerator.Pro);            
//        }
//    }

//    public static string GetCPUID() {
//        string cpuInfo = string.Empty;
//        ManagementClass mc = new ManagementClass("win32_processor");
//        ManagementObjectCollection moc = mc.GetInstances();
//
//        foreach (ManagementObject mo in moc)
//        {
//             if (cpuInfo == "")
//             {
//                  //Get only the first CPU's ID
//                  cpuInfo = mo.Properties["processorID"].Value.ToString();
//                  break;
//             }
//        }
//        return cpuInfo;        
//    }
}
}