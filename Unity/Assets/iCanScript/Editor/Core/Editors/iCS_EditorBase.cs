using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;

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
        iCS_EditorMgr.Add(this);
    }
    public void OnDisable() {
        iCS_EditorMgr.Remove(this);
    }

    // =================================================================================
    // OnGUI
    // ---------------------------------------------------------------------------------
    public void OnGUI() {
        // Install iCanScript icon in tab title area.
        // (must hack since Unity does not provide a direct way of adding editor title images).
        var propertyInfo= GetType().GetProperty("cachedTitleContent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if(propertyInfo != null) {
            var methodInfo= propertyInfo.GetGetMethod(true);
            if(methodInfo != null) {
                var r= methodInfo.Invoke(this, null) as GUIContent;
                if(r.image == null) {
                    Texture2D iCanScriptLogo= null;
                    if(iCS_TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo)) {
                        r.image= iCanScriptLogo;
                    }
                }
            }
        }
    }
    
    // =================================================================================
    // Update the editor manager.
    // ---------------------------------------------------------------------------------
    protected void UpdateMgr() {
        iCS_EditorMgr.Update();
        myIStorage= iCS_StorageMgr.IStorage;
    }
}
