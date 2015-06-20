using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using iCanScript.Internal.Editor.CodeGeneration;
using P=iCanScript.Internal.Prelude;

#if COMMUNITY_EDITION
#else
#if PRO_EDITION
#else
namespace iCanScript.Internal.Editor {
    public static class iCS_DevMenus {
        // ======================================================================
        // Snapshot definitions
    	const string ScreenShotsFolder= "/../../../ScreenShots";
    
    //    // ======================================================================
    //    // Code Generation Tests
    //	[MenuItem("iCanScript/DevTools/Generate Code",false,2000)]
    //    public static void GenerateCode() {
    //        var className= CSharpFileUtils.MakeUniqueClassName("VisualScript");
    //        Debug.Log("Class Name=> "+className);
    //        var code= "namespace iCanScript.Internal { public class "+className+" {}; }";
    //        CSharpFileUtils.WriteCSharpFile("VisualScripts", className, code);
    //    }
    
        // ======================================================================
        // Visual Editor Snapshot
    	[MenuItem("iCanScript/DevTools/Visual Editor Snapshot",false,2000)]
    	public static void MenuVisualEditorSnapshot() {
    		EditorWindow edWindow= iCS_EditorController.FindVisualEditorWindow();
    		if(edWindow == null) return;
    		iCS_DevToolsConfig.SnapshotWithoutBackground= false;
    		iCS_DevToolsConfig.TakeVisualEditorSnapshot= true;
    	}
    	[MenuItem("iCanScript/DevTools/Visual Editor Snapshot",true,2000)]
    	public static bool ValidateMenuVisualEditorSnapshot() {
            return IsVisualEditor;
    	}
    	[MenuItem("iCanScript/DevTools/Visual Editor Snapshot - No Background",false,2001)]
    	public static void MenuVisualEditorSnapshotNoBackground() {
    		EditorWindow edWindow= iCS_EditorController.FindVisualEditorWindow();
    		if(edWindow == null) return;
    		iCS_DevToolsConfig.SnapshotWithoutBackground= true;
    		iCS_DevToolsConfig.TakeVisualEditorSnapshot= true;
    	}
    	[MenuItem("iCanScript/DevTools/Visual Editor Snapshot - No Background",true,2001)]
    	public static bool ValidateMenuVisualEditorSnapshotNoBackground() {
            return IsVisualEditor;
    	}
        [MenuItem("iCanScript/DevTools/Toggle Asset Store Big Image Frame",false,2006)]
        public static void ToggleBigImageFrame() {
            iCS_DevToolsConfig.ShowAssetStoreBigImageFrame = !iCS_DevToolsConfig.ShowAssetStoreBigImageFrame;
            if(iCS_DevToolsConfig.ShowAssetStoreBigImageFrame) {
                iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame= false;
                iCS_DevToolsConfig.ShowBoldImage= true;
            }
            else {
                iCS_DevToolsConfig.ShowBoldImage= false;
            }
        }
        [MenuItem("iCanScript/DevTools/Toggle Asset Store Small Image Frame",false,2007)]
        public static void ToggleSmallImageFrame() {
            iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame= !iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame;
            if(iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame) {
                iCS_DevToolsConfig.ShowAssetStoreBigImageFrame= false;
                iCS_DevToolsConfig.ShowBoldImage= true;
            }
            else {
                iCS_DevToolsConfig.ShowBoldImage= false;
            }
        }
        [MenuItem("iCanScript/DevTools/Toggle Bold Image",false,2008)]
        public static void ToggleBoldImage() {
            iCS_DevToolsConfig.ShowBoldImage= !iCS_DevToolsConfig.ShowBoldImage;
        }
        [MenuItem("iCanScript/DevTools/Toggle Background Image", false, 2009)]
        public static void ToggleBackgroundImage() {
            iCS_DevToolsConfig.UseBackgroundImage= !iCS_DevToolsConfig.UseBackgroundImage;
        }
        public static bool IsVisualEditor {
    		get {
                EditorWindow edWindow= iCS_EditorController.FindVisualEditorWindow();
    		    return edWindow != null;
            }
        }
        // ======================================================================
        // Sanity Check
    	[MenuItem("iCanScript/DevTools/Sanity Check Selection",false,2020)]
    	public static void MenuSanityCheck() {
    		iCS_IStorage storage= iCS_VisualScriptDataController.IStorage;
    		if(storage == null) return;
    		Debug.Log("iCanScript: Start Sanity Check on: "+storage.iCSMonoBehaviour.name);
    		storage.SanityCheck();
    		Debug.Log("iCanScript: Completed Sanity Check on: "+storage.iCSMonoBehaviour.name);
    	}
        // ======================================================================
        // Trigger Periodic Software Update Verification
    	[MenuItem("iCanScript/DevTools/Invoke Periodic Software Update Verification",false,2021)]
    	public static void MenuPeriodicSoftwareUpdateVerification() {
    		SoftwareUpdateController.PeriodicUpdateVerification();
    	}	
        // ======================================================================
        // Extract some info.
    	[MenuItem("iCanScript/DevTools/Get Layout Info",false,2022)]
    	public static void MenuGetLayoutInfo() {
    		iCS_IStorage iStorage= iCS_VisualScriptDataController.IStorage;
    		if(iStorage == null) return;
            var selectedObj= iStorage.SelectedObject;
            if(selectedObj == null) return;
            Debug.Log("Layout Info for => "+selectedObj.DisplayName+"\n"+
                "LayoutRect => "+selectedObj.GlobalRect
            );
        }
        [MenuItem("iCanScript/DevTools/Environment", false, 2050)]
        public static void ShowEnvironment() {
            Debug.Log("Machine Name=> "+System.Environment.MachineName);
            Debug.Log("User Name=> "+System.Environment.UserName);
        }    
        [MenuItem("iCanScript/DevTools/Toggle Show User Transactions", false, 2051)]
        public static void ToggleShowUserTransactions() {
            UserTransactionController.ShowUserTransaction= UserTransactionController.ShowUserTransaction ^ true;
        }    
    }
    
}

#endif
#endif
