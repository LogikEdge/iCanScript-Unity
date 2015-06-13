using UnityEngine;
using System.Collections;
using iCanScript.Internal.Editor;

namespace iCanScript.Internal.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Properties.
	// ----------------------------------------------------------------------
    iCS_IStorage    myPreviousIStorage= null;
    
    // ======================================================================
    // Initialization
	// ----------------------------------------------------------------------
    // Prepares the editor for editing a graph.  Note that the graph to edit
    // is not configured at this point.  We must wait for an activate from
    // the graph inspector to know which graph to edit. 
	public new void OnEnable() {        
        base.OnEnable();
        
        // -- Set window title --
        Texture2D iCanScriptLogo= null;
        TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
        titleContent= new GUIContent("Visual Editor", iCanScriptLogo);
        
		// Tell Unity we want to be informed of move drag events
		wantsMouseMove= true;

        // Create worker objects.
        myGraphics      = new iCS_Graphics();
        myContextualMenu= new iCS_ContextualMenu();
        
        // Get snapshot for realtime clock.
        myCurrentTime= Time.realtimeSinceStartup;	
        
        // Update visual editor cache.
        UpdateVisualEditorCache();
        
        // Register for window under mouse change
        SystemEvents.OnWindowUnderMouseChange+= helpWindowChange;
        
        // -- Initialize Sub-Systems --
        QueueOnGUICommand(WorkflowAssistantInit);
        QueueOnGUICommand(HelpInit);
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the iCS_VisualScript editor.
    public new void OnDisable() {
        base.OnDisable();
        
        // Release all worker objects.
        myGraphics      = null;
        myContextualMenu= null;
        CloseSubEditor();
        
        // Unregister for window under mouse change
        SystemEvents.OnWindowUnderMouseChange-= helpWindowChange;
    }

	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	bool IsInitialized() {
        // Nothing to do if we don't have a Graph to edit...
        UpdateMgr();
		if(IStorage == null) {
            myBookmark= null;
            DragType= DragTypeEnum.None;
            CloseSubEditor();
		    return false;
		}
        if(IStorage != myPreviousIStorage) {
            // Avoid using a storage without a root node.
            if(StorageRoot == null) {
                return false;
            }
            myPreviousIStorage= IStorage;
            myBookmark= null;
            DragType= DragTypeEnum.None;
            CloseSubEditor();
            IStorage.ForceRelayout= true;
        }
        
		// Don't run if graphic sub-system did not initialise.
		if(iCS_Graphics.IsInitialized == false) {
            iCS_Graphics.Init(IStorage);
		}

        // Update visual script cache.
        UpdateVisualScriptCache();
        return true;
	}

	// ----------------------------------------------------------------------
    // System level cache update.
    void UpdateVisualEditorCache() {
    }
    
	// ----------------------------------------------------------------------
    // Visual script level cache update.
    void UpdateVisualScriptCache() {
        
    }
}
}