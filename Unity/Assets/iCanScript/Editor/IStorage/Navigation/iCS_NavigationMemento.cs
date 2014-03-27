using UnityEngine;
using System.Collections;

public class iCS_NavigationMemento {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    bool                    ShowDisplayRootNode;
    Vector2                 ScrollPosition;
    float                   GuiScale;
    int                     DisplayRoot;
    int                     SelectedObject;

    // ----------------------------------------------------------------------
    // Creates a new memento and saves the navigation state of the given
    // IStorage.
    public iCS_NavigationMemento(iCS_IStorage iStorage) {
        SaveState(iStorage);
    }
    // ----------------------------------------------------------------------
    // Save the navigation state of the given IStorage.
    public void SaveState(iCS_IStorage iStorage) {
        ShowDisplayRootNode    = iStorage.ShowDisplayRootNode;
        ScrollPosition         = iStorage.ScrollPosition;
        GuiScale               = iStorage.GuiScale;
        DisplayRoot            = iStorage.DisplayRoot.InstanceId;
        SelectedObject         = iStorage.SelectedObject.InstanceId;
    }
    // ----------------------------------------------------------------------
    // Restores the navigation state into the given IStorage.
    public void RestoreState(iCS_IStorage iStorage) {
        if(!iStorage.IsValid(DisplayRoot)) return;
        iStorage.ShowDisplayRootNode= ShowDisplayRootNode;
        iStorage.ScrollPosition     = ScrollPosition;
        iStorage.GuiScale           = GuiScale;
        iStorage.DisplayRoot        = iStorage[DisplayRoot];
        iStorage.SelectedObject     = iStorage.IsValid(SelectedObject) ? iStorage[SelectedObject] : null; 
        iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
    }
}
