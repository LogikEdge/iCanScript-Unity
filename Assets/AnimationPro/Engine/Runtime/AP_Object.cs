using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class AP_Object : AP_ObjectUtil, IEnumerable<AP_Object> {
    // ======================================================================
    // ATTRIBUTES
    // ----------------------------------------------------------------------
                        public  AP_Top          Top            = null;
    [SerializeField]    private AP_Aggregate    myParent       = null;
    [SerializeField]    private bool            myIsVisible    = true;
                        private bool            myIsEditorDirty= true;
                        
    // ----------------------------------------------------------------------
    // NAME & TYPE NAME
    // ----------------------------------------------------------------------
    public bool IsNameVisible { get { return name != null && name.Length != 0 && name[0] != AP_EditorConfig.PrivateStringPrefix; }}
    public string Name {
        get {
            if(name == null || name == "") return null;
            return name[0] == AP_EditorConfig.PrivateStringPrefix ? name.Substring(1) : name;
        }
        set { name= value; }
    }
    public string TypeName {
        get { return GetType().Name.Substring(AP_EditorConfig.TypePrefix.Length); }
    }
    public string NameOrTypeName {
        get {
            string displayName= Name;
            return (displayName == null) ? (":"+TypeName) : displayName;
        }
    }
    // ----------------------------------------------------------------------
    // EDITOR VISIBILITY
    // ----------------------------------------------------------------------
    public bool IsVisible {
        get { return IsTop ? false : myIsVisible; }
        set {
            if(myIsVisible != value) IsEditorDirty= true;
            myIsVisible= value;
            if(IsTop) return;
            ExecuteIf<AP_Node>(
                (node)=> {
                    node.ForEachChild<AP_Port>(
                        (child)=> { child.IsVisible= value; }
                    );
                }
            );
            if(value) return;
            ExecuteIf<AP_Node>(
                (node)=> {
                    node.ForEachChild<AP_Node>(
                        (child)=> { child.IsVisible= value; }
                    );
                }
            );
        }
    }
    
    // ----------------------------------------------------------------------
    // DIRTY FLAG
    // ----------------------------------------------------------------------
    public bool IsEditorDirty {
        get { return myIsEditorDirty; }
        set {
            myIsEditorDirty= value;
            if(value && Parent != null) Parent.IsEditorDirty= true;
        }
    }

    // ======================================================================
    // LIFECYCLE
    // ----------------------------------------------------------------------
    protected AP_Object Init(string _name, AP_Aggregate _parent) {
        Name  = _name;
        Parent= _parent;
        IsEditorDirty= true;
        return this;
    }
    // ----------------------------------------------------------------------
    // Control removal of the object (as opposed to the automatic
    // deallocation from a level shutdown).
    public virtual void Dealloc() {
        Parent= null;
        IsEditorDirty= true;
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
    }
 
    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public AP_Aggregate Parent {
        get { return myParent; }
        set {
            if(myParent != null && myParent != value) {
                myParent.RemoveChild(this);
            }
            if(value != null) {
                value.AddChild(this);
                Top= value.Top;
                IsVisible= value.IsTop ? true: value.IsVisible;
            }
            myParent= value;
        }
    }
    // ----------------------------------------------------------------------
    // Returns "true" if this node is a child of the given node.
    public bool IsChildOf(AP_Aggregate _parent) {
        if(Parent == _parent) return true;
        return Parent == null ? false : Parent.IsChildOf(_parent);
    }
    public virtual void AddChild(AP_Object _object)     {}
    public virtual void RemoveChild(AP_Object _object)  {}
    private class Enumerator : IEnumerator<AP_Object> {
        public void Reset()                 {}
        public bool MoveNext()              { return false; }
        public AP_Object Current            { get { return null; }}
               object IEnumerator.Current   { get { return Current; }}
               void   System.IDisposable.Dispose() {}
    }
    public virtual IEnumerator<AP_Object>   GetEnumerator()                         { return new Enumerator(); }
                   IEnumerator<AP_Object>   IEnumerable<AP_Object>.GetEnumerator()  { return GetEnumerator(); }
                   IEnumerator              IEnumerable.GetEnumerator()             { return GetEnumerator(); }

    // ======================================================================
    // ITERATION UTILITIES
    // ----------------------------------------------------------------------
    public void ForEachChild(System.Action<AP_Object> fnc) {
        foreach(var child in this) { fnc(child); }
    }
    // ----------------------------------------------------------------------
    public void ForEachChild<T>(System.Action<T> fnc) where T : AP_Object {
        foreach(var child in this) { child.ExecuteIf<T>( (obj)=> { fnc(obj);} ); }
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthLast(System.Action<AP_Object> fnc) {
        fnc(this);
        foreach(var child in this) { child.ForEachRecursiveDepthLast(fnc); }
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthFirst(System.Action<AP_Object> fnc) {
        foreach(var child in this) { child.ForEachRecursiveDepthFirst(fnc); }
        fnc(this);
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursive(System.Action<AP_Object> fnc) {
        ForEachRecursiveDepthFirst(fnc);
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthLast<T>(System.Action<T> fnc) where T : AP_Object {
        ForEachRecursiveDepthLast( (obj)=> { obj.ExecuteIf<T>( (t)=> { fnc(t); } ); } );
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthFirst<T>(System.Action<T> fnc) where T : AP_Object {
        ForEachRecursiveDepthFirst( (obj)=> { obj.ExecuteIf<T>( (t)=> { fnc(t); } ); } );
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursive<T>(System.Action<T> fnc) where T : AP_Object {
        ForEachRecursiveDepthFirst<T>(fnc);
    }
    // ----------------------------------------------------------------------
    public int ChildCount() {
        int count= 0;
        ForEachChild( (child)=> { ++count; });
        return count;
    }
    // ----------------------------------------------------------------------
    public int ChildCount<T>() where T : AP_Object {
        int count= 0;
        ForEachChild<T>( (child)=> { ++count; });
        return count;
    }
    // ----------------------------------------------------------------------
    public int ChildCountRecursive() {
        int count= 0;
        ForEachRecursive( (child)=> { ++count; });
        return count;
    }
    // ----------------------------------------------------------------------
    public int ChildCountRecursive<T>() where T : AP_Object {
        int count= 0;
        ForEachRecursive<T>( (child)=> { ++count; });
        return count;
    }
    
    // ======================================================================
    // UPDATE    
    // ----------------------------------------------------------------------
    public bool             IsValid     { get { return doIsValid(); }}
    protected virtual bool  doIsValid() { return true; }
    
    // ======================================================================
    // GUI
    // ----------------------------------------------------------------------
    public            void Layout()         { DoLayout(); IsEditorDirty= false; }
    public    virtual void DoLayout()       {}

}
