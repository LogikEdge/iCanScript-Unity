using UnityEngine;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Properties.
	// ----------------------------------------------------------------------
    iCS_IStorage    myPreviousIStorage= null;
    
    // ======================================================================
    // Cached Properties.
	// ----------------------------------------------------------------------
    GUIContent myCached_RemainingTrialDaysMessage;
    
    // ======================================================================
    // Initialization
	// ----------------------------------------------------------------------
    // Prepares the editor for editing a graph.  Note that the graph to edit
    // is not configured at this point.  We must wait for an activate from
    // the graph inspector to know which graph to edit. 
	public new void OnEnable() {        
        base.OnEnable();
        
		// Tell Unity we want to be informed of move drag events
		wantsMouseMove= true;

        // Create worker objects.
        myGraphics      = new iCS_Graphics();
        myContextualMenu= new iCS_ContextualMenu();
        
        // Get snapshot for realtime clock.
        myCurrentTime= Time.realtimeSinceStartup;	
        
        // Update visual editor cache.
        UpdateVisualEditorCache();    
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the iCS_Behaviour editor.
    public new void OnDisable() {
        base.OnDisable();
        
        // Release all worker objects.
        myGraphics      = null;
        myContextualMenu= null;
		mySubEditor     = null;
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
            mySubEditor= null;
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
            mySubEditor= null;
            IStorage.ForceRelayout= true;
        }
        
		// Don't run if graphic sub-system did not initialise.
		if(iCS_Graphics.IsInitialized == false) {
            iCS_Graphics.Init(IStorage);
		}
        iCS_AppController.Start();

        // Update visual script cache.
        UpdateVisualScriptCache();
        return true;
	}

	// ----------------------------------------------------------------------
    // System level cache update.
    void UpdateVisualEditorCache() {
        string remainingTimeMessage= "Trial: ";
        remainingTimeMessage+= " ("+iCS_LicenseController.RemainingTrialDays.ToString()+" days remaining) ";
        myCached_RemainingTrialDaysMessage= new GUIContent(remainingTimeMessage);        
    }
    
	// ----------------------------------------------------------------------
    // Visual script level cache update.
    void UpdateVisualScriptCache() {
        
    }
}
