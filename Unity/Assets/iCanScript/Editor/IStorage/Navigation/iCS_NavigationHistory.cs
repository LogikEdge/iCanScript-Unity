using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public void Save(iCS_IStorage iStorage) {
        // Erase previous forward history.
        if(HasForwardHistory) {
            myMementos.RemoveRange(myCursor, myMementos.Count-myCursor);
        }
        // Save new memento.
        myMementos.Add(new iCS_NavigationMemento(iStorage));
        // Limit the size the the navigation history
        if(myMementos.Count > kMaxHistory) {
            myMementos.RemoveRange(0, myMementos.Count-kMaxHistory);
            
        }
        myCursor= myMementos.Count;
    }
    // ----------------------------------------------------------------------
    // Reloads the navigation state of the given IStorage from the backward
    // history.
    public void ReloadFromBackwardHistory(iCS_IStorage iStorage) {
        if(!HasBackwardHistory) return;
        var forwardMemento= new iCS_NavigationMemento(iStorage);
        --myCursor;
        myMementos[myCursor].RestoreState(iStorage);
        myMementos[myCursor]= forwardMemento;
    }
    // ----------------------------------------------------------------------
    // Reloads the navigation state of the given IStorage from the forward
    // history.
    public void ReloadFromForwardHistory(iCS_IStorage iStorage) {
        if(!HasForwardHistory) return;
        var backwardMemento= new iCS_NavigationMemento(iStorage);
        myMementos[myCursor].RestoreState(iStorage);
        myMementos[myCursor]= backwardMemento;
        ++myCursor;
    }
}
