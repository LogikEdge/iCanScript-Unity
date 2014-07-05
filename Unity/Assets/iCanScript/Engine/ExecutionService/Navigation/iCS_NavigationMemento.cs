using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class iCS_NavigationMemento {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    bool                    ShowDisplayRootNode;
    Vector2                 ScrollPosition;
    float                   GuiScale;
    int                     SelectedObject;
    int                     DisplayRoot;

    // ----------------------------------------------------------------------
    // Creates a new memento and saves the navigation state of the given
    // IStorage.
    public iCS_NavigationMemento(iCS_IVisualScriptData storage) {
        SaveState(storage);
    }
    // ----------------------------------------------------------------------
    // Save the navigation state of the given IStorage.
    public void SaveState(iCS_IVisualScriptData storage) {
        ShowDisplayRootNode= storage.ShowDisplayRootNode;
        ScrollPosition     = storage.ScrollPosition;
        GuiScale           = storage.GuiScale;
        SelectedObject     = storage.SelectedObject;
        DisplayRoot        = storage.DisplayRoot;
    }
    // ----------------------------------------------------------------------
    // Restores the navigation state into the given IStorage.
    public void RestoreState(iCS_IVisualScriptData storage) {
        if(!iCS_VisualScriptData.IsValidEngineObject(storage, DisplayRoot)) return;
        storage.ShowDisplayRootNode= ShowDisplayRootNode;
        storage.ScrollPosition     = ScrollPosition;
        storage.GuiScale           = GuiScale;
        storage.DisplayRoot        = DisplayRoot;
        storage.SelectedObject     = SelectedObject; 
    }

    // ----------------------------------------------------------------------
    // Duplication functionality
    public void CopyFrom(iCS_NavigationMemento from) {
        ShowDisplayRootNode= from.ShowDisplayRootNode;
        ScrollPosition     = from.ScrollPosition;
        GuiScale           = from.GuiScale;
        DisplayRoot        = from.DisplayRoot;
        SelectedObject     = from.SelectedObject;
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
}