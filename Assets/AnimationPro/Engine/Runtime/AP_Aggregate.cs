using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AP_Aggregate : AP_Object {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public List<AP_Object>    Children;
    
    // ======================================================================
    // OBJECT LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    protected new AP_Aggregate Init(string _name, AP_Aggregate _parent) {
        Children= new List<AP_Object>();
        base.Init(_name, _parent);
        return this;
    }
    // ----------------------------------------------------------------------
    public override void Dealloc() {
        AP_Object[] childrenArray= Children.ToArray();
        foreach(var child in childrenArray) { child.Dealloc(); }
        
        base.Dealloc();
    }

    // ----------------------------------------------------------------------
    // Returns "true" if this node is a parent of the given node.
    public bool IsParentOf(AP_Object obj) { return obj.IsChildOf(this); }
    
    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    // Children add/remove.
    public override void AddChild(AP_Object obj) {
        Children.Add(obj);
    }
    public override void RemoveChild(AP_Object _object) {
        Children.Remove(_object);
    }
    
    // ----------------------------------------------------------------------
    private class Enumerator : IEnumerator<AP_Object> {
        AP_Aggregate    myAggregate;
        int             myCursor;
        AP_Object       myCurrent;

        public Enumerator(AP_Aggregate aggregate)   { myAggregate= aggregate; Reset(); }
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
        public AP_Object Current            { get { return myCurrent; }}
               object IEnumerator.Current   { get { return Current; }}
               void   System.IDisposable.Dispose() {}
    }
    public override IEnumerator<AP_Object> GetEnumerator() { return new Enumerator(this); }
}
