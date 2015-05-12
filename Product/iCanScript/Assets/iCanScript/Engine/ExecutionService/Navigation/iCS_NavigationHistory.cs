using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Engine {
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
        public void Save(iCS_IVisualScriptData storage) {
            // Erase previous forward history.
            if(HasForwardHistory) {
                myMementos.RemoveRange(myCursor, myMementos.Count-myCursor);
            }
            // Save new memento.
            myMementos.Add(new iCS_NavigationMemento(storage));
            // Limit the size the the navigation history
            if(myMementos.Count > kMaxHistory) {
                myMementos.RemoveRange(0, myMementos.Count-kMaxHistory);
            }
            myCursor= myMementos.Count;
        }
        // ----------------------------------------------------------------------
        // Reloads the navigation state of the given IStorage from the backward
        // history.
        public void ReloadFromBackwardHistory(iCS_IVisualScriptData storage) {
            if(!HasBackwardHistory) return;
            var forwardMemento= new iCS_NavigationMemento(storage);
            --myCursor;
            myMementos[myCursor].RestoreState(storage);
            myMementos[myCursor]= forwardMemento;
        }
        // ----------------------------------------------------------------------
        // Reloads the navigation state of the given IStorage from the forward
        // history.
        public void ReloadFromForwardHistory(iCS_IVisualScriptData storage) {
            if(!HasForwardHistory) return;
            var backwardMemento= new iCS_NavigationMemento(storage);
            myMementos[myCursor].RestoreState(storage);
            myMementos[myCursor]= backwardMemento;
            ++myCursor;
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
        // ----------------------------------------------------------------------
        public void RemoveDisplayRoot(int displayRootId) {
            myMementos.RemoveAll(m=> m.IsDisplayRoot(displayRootId));
            if(myCursor > myMementos.Count) {
                myCursor= myMementos.Count;
            }
        }
    }
    
}


