using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class iCS_NavigationMemento {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    int                     DisplayRoot;
    bool                    ShowDisplayRootNode;
    Vector2                 ScrollPosition;
    float                   GuiScale;
    int                     SelectedObject;
    Vector2                 SelectedObjectGlobalPosition;
    
    // ----------------------------------------------------------------------
    // Creates a new memento and saves the navigation state of the given
    // IStorage.
    public iCS_NavigationMemento(iCS_IVisualScriptData storage, Vector2 selectedObjectGlobalPosition) {
        SaveState(storage, selectedObjectGlobalPosition);
    }
    // ----------------------------------------------------------------------
    // Save the navigation state of the given IStorage.
    public void SaveState(iCS_IVisualScriptData storage, Vector2 selectedObjectGlobalPosition) {
        DisplayRoot                 = storage.DisplayRoot;
        ShowDisplayRootNode         = storage.ShowDisplayRootNode;
        ScrollPosition              = storage.ScrollPosition;
        GuiScale                    = storage.GuiScale;
        SelectedObject              = storage.SelectedObject;
        SelectedObjectGlobalPosition= selectedObjectGlobalPosition;
    }
    // ----------------------------------------------------------------------
    // Restores the navigation state into the given IStorage.
    public Vector2 RestoreState(iCS_IVisualScriptData storage, Vector2 selectedObjectGlobalPosition) {
        if(!iCS_VisualScriptData.IsValidEngineObject(storage, DisplayRoot)) {
            return selectedObjectGlobalPosition;
        }
        storage.ShowDisplayRootNode= ShowDisplayRootNode;
        storage.ScrollPosition     = ScrollPosition;
        storage.GuiScale           = GuiScale;
        storage.DisplayRoot        = DisplayRoot;
        storage.SelectedObject     = SelectedObject; 
        return SelectedObjectGlobalPosition;
    }

    // ----------------------------------------------------------------------
    // Duplication functionality
    public void CopyFrom(iCS_NavigationMemento from) {
        ShowDisplayRootNode         = from.ShowDisplayRootNode;
        ScrollPosition              = from.ScrollPosition;
        GuiScale                    = from.GuiScale;
        DisplayRoot                 = from.DisplayRoot;
        SelectedObject              = from.SelectedObject;
        SelectedObjectGlobalPosition= from.SelectedObjectGlobalPosition;
    }
    // ----------------------------------------------------------------------
    public iCS_NavigationMemento Clone() {
        var newMemento= new iCS_NavigationMemento();
        newMemento.CopyFrom(this);
        return newMemento;
    }
    private iCS_NavigationMemento() {}
    // ----------------------------------------------------------------------
    public bool IsEquivalentTo(iCS_NavigationMemento other) {
        if(ShowDisplayRootNode != other.ShowDisplayRootNode) return false;
        if(ScrollPosition != other.ScrollPosition) return false;
        if(GuiScale != other.GuiScale) return false;
        if(SelectedObject != other.SelectedObject) return false;
        if(DisplayRoot != other.DisplayRoot) return false;
        return true;
    }
    // ----------------------------------------------------------------------
    public bool IsDisplayRoot(int displayRootId) {
        return DisplayRoot == displayRootId;
    }
}
