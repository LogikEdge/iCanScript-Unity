using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WD_Aggregate : WD_Object {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public List<WD_Object>    Children;
    
    // ======================================================================
    // OBJECT LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    protected override void Init(string _name, WD_Aggregate _parent) {
        Children= new List<WD_Object>();
        base.Init(_name, _parent);
    }
    // ----------------------------------------------------------------------
    public override void Dealloc() {
        WD_Object[] childrenArray= Children.ToArray();
        foreach(var child in childrenArray) { child.Dealloc(); }
        
        base.Dealloc();
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    // Children add/remove.
    public override void AddChild(WD_Object obj) {
        Children.Add(obj);
    }
    public override void RemoveChild(WD_Object _object) {
        Children.Remove(_object);
    }
    
    // ----------------------------------------------------------------------
    private class Enumerator : IEnumerator<WD_Object> {
        WD_Aggregate    myAggregate;
        int             myCursor;
        WD_Object       myCurrent;

        public Enumerator(WD_Aggregate aggregate)   { myAggregate= aggregate; Reset(); }
        public void Reset()                         { myCursor= -1; myCurrent= null; }
        public bool MoveNext() {
            ++myCursor;
            if(myCursor < myAggregate.Children.Count) {
                myCurrent= myAggregate.Children[myCursor];
                return true;
            }
            myCurrent= null;
            return false;
        }
        public WD_Object Current            { get { return myCurrent; }}
               object IEnumerator.Current   { get { return Current; }}
               void   System.IDisposable.Dispose() {}
    }
    public override IEnumerator<WD_Object> GetEnumerator() { return new Enumerator(this); }
}
