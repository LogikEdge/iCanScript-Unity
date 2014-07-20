using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class iCS_NavigationHistory {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    const int                   kMaxHistory= 10;
    int                         myCursor   = 0;
    List<iCS_NavigationMemento> myMementos = new List<iCS_NavigationMemento>();    

    // ======================================================================
    // History management methods.
    // ----------------------------------------------------------------------
    // Clear history
    public void Clear() {
        myCursor= 0;
        myMementos.Clear();
    }
    // ----------------------------------------------------------------------
    // Returns "true" if a backward history is available.
    public bool HasBackwardHistory { get { return myCursor > 0; }}
    // ----------------------------------------------------------------------
    // Returns "true" if a forward history is available.
    public bool HasForwardHistory { get { return myMementos.Count > myCursor; }}
    // ----------------------------------------------------------------------
    // Saves the navigation state of the given IStorage.
    public void Save(iCS_IVisualScriptData storage, Vector2 selectedObjectGlobalPosition) {
        // Erase previous forward history.
        if(HasForwardHistory) {
            myMementos.RemoveRange(myCursor, myMementos.Count-myCursor);
        }
        // Save new memento.
        myMementos.Add(new iCS_NavigationMemento(storage, selectedObjectGlobalPosition));
        // Limit the size the the navigation history
        if(myMementos.Count > kMaxHistory) {
            myMementos.RemoveRange(0, myMementos.Count-kMaxHistory);
        }
        myCursor= myMementos.Count;
    }
    // ----------------------------------------------------------------------
    // Reloads the navigation state of the given IStorage from the backward
    // history.
    public Vector2 ReloadFromBackwardHistory(iCS_IVisualScriptData storage, Vector2 selectedObjectGlobalPosition) {
        if(!HasBackwardHistory) return selectedObjectGlobalPosition;
        var forwardMemento= new iCS_NavigationMemento(storage, selectedObjectGlobalPosition);
        --myCursor;
        selectedObjectGlobalPosition= myMementos[myCursor].RestoreState(storage, selectedObjectGlobalPosition);
        myMementos[myCursor]= forwardMemento;
        return selectedObjectGlobalPosition;
    }
    // ----------------------------------------------------------------------
    // Reloads the navigation state of the given IStorage from the forward
    // history.
    public Vector2 ReloadFromForwardHistory(iCS_IVisualScriptData storage, Vector2 selectedObjectGlobalPosition) {
        if(!HasForwardHistory) return selectedObjectGlobalPosition;
        var backwardMemento= new iCS_NavigationMemento(storage, selectedObjectGlobalPosition);
        selectedObjectGlobalPosition= myMementos[myCursor].RestoreState(storage, selectedObjectGlobalPosition);
        myMementos[myCursor]= backwardMemento;
        ++myCursor;
        return selectedObjectGlobalPosition;
    }
    // ----------------------------------------------------------------------
    // Copy history
    public void CopyFrom(iCS_NavigationHistory from) {
        // Copy cursor
        myCursor= from.myCursor;
        // Adjust size
        int fromLen= from.myMementos.Count;
        int toLen= myMementos.Count;
        if(toLen > fromLen) {
            myMementos.RemoveRange(fromLen, toLen-fromLen);
        }
        myMementos.Capacity= fromLen;
        // Copy each memento
        for(int i= 0; i < fromLen; ++i) {
            if(i < myMementos.Count) {
                myMementos[i].CopyFrom(from.myMementos[i]);
            }
            else {
                myMementos.Add(from.myMementos[i].Clone());
            }
        }
    }
    // ----------------------------------------------------------------------
    // Equivalence operator
    public bool IsEquivalentTo(iCS_NavigationHistory other) {
        if(myCursor != other.myCursor) return false;
        for(int i= 0; i < myCursor; ++i) {
            if(!myMementos[i].IsEquivalentTo(other.myMementos[i])) {
                return false;
            }
        }
        return true;
    }
}
