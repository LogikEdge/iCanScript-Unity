using UnityEngine;
using System.Collections;

public partial class UK_IStorage {
    // ----------------------------------------------------------------------
    // Determine if we should hide when minimize is activated.
    public bool ShouldHideOnMinimize(UK_EditorObject edObj) {
        if(IsTransitionEntryModule(edObj)) return true;
        if(IsTransitionExitModule(edObj)) return true;
        return false;
    }
    // ----------------------------------------------------------------------
    // Determine if the edition should be in a popup editor window.
    public bool ShouldUsePopupEditorWindow(UK_EditorObject edObj) {
        if(IsTransitionEntryModule(edObj)) return true;
        if(IsTransitionExitModule(edObj)) return true;
        return false;
    }
}
