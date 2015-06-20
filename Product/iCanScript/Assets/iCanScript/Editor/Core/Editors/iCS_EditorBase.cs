using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using iCanScript;

namespace iCanScript.Internal.Editor {
    
    public class iCS_EditorBase : EditorWindow {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
    	iCS_IStorage		myIStorage= null;
    
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
    	public iCS_IStorage 	IStorage 	   { get { return myIStorage; } set { myIStorage= value; }}
    	public iCS_EditorObject SelectedObject {
    	    get { return myIStorage != null ? IStorage.SelectedObject : null; }
    	    set { if(IStorage != null) IStorage.SelectedObject= value; }
    	}
        	
        // =================================================================================
        // Activation/Deactivation.
        // ---------------------------------------------------------------------------------
        public void OnEnable() {
            iCS_EditorController.Add(this);
        }
        public void OnDisable() {
            iCS_EditorController.Remove(this);
        }

        // =================================================================================
        // OnGUI
        // ---------------------------------------------------------------------------------
        public void OnGUI() {
            // Stop all processing if not registered & trial period is over
            if(EditionController.IsCommunityLimitReached) {
                string message= null;
                var area= Math3D.Area(position);
                if(area > 200000) {
                    message= "iCanScript Community Edition limit reached !\n\nUse the Pro Edition of iCanScript for large projects.\n\nPurchase the Pro Edition from the Unity Assets Store.";
                }
                else if(area > 150000){
                    message= "iCanScript Community Edition limit reached !\n\nPurchase the Pro Edition from the Unity Assets Store.";
                }
                else {
                    message= "iCanScript Community Edition limit reached !\n\nPlease purchase the Pro Edition.";
                }
                ShowNotification(new GUIContent(message));
                return;
            }
        }
    
        // =================================================================================
        // Common processing.
        // ---------------------------------------------------------------------------------
        public void OnSelectionChange() {
            UpdateMgr();
            Repaint();
        }
        public void OnHierarchyChange() {
            UpdateMgr();
            Repaint();
        }
        public void OnProjectChange() {
            UpdateMgr();
            Repaint();
        }

        // =================================================================================
        // Update the editor manager.
        // ---------------------------------------------------------------------------------
        protected void UpdateMgr() {
            iCS_VisualScriptDataController.Update();
            myIStorage= iCS_VisualScriptDataController.IStorage;
        }
    }
}

