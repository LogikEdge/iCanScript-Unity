using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public abstract class WD_Object : WD_ObjectUtil, IEnumerable<WD_Object> {
    // ======================================================================
    // ATTRIBUTES
    // ----------------------------------------------------------------------
    [SerializeField]    private WD_Aggregate    myParent   = null;
    [SerializeField]    private bool            myIsVisible= true;
                        public  int             InstanceId = -1;
                        public  WD_Top          Top        = null;

    private bool    myIsEditorDirty= true;
                        
    // ======================================================================
    // CREATION UTILITIES
    // ----------------------------------------------------------------------
    public static DERIVED CreateInstance<DERIVED>(string _name, WD_Aggregate _parent) where DERIVED : WD_Object, new() {
        DERIVED instance= CreateInstance<DERIVED>();
        instance.Init(_name, _parent);
        return instance;
    }
    // ----------------------------------------------------------------------
    protected virtual void Init(string _name, WD_Aggregate _parent) {
        Name= _name;
        Parent= _parent;
        IsEditorDirty= true;
        // Add to the save list.
        if(!(this is WD_RootNode)) {
            if(this is WD_Top) {
                InstanceId= (this as WD_Top).RootNode.Graph.AddObject(this);
            }
            else {
                InstanceId= Top.RootNode.Graph.AddObject(this);
            }
        }
    }
    // ----------------------------------------------------------------------
    // Control removal of the object (as opposed to the automatic
    // deallocation from a level shutdown).
    public virtual void Dealloc() {
        // Remove from the save list
        if(!(this is WD_RootNode)) Top.RootNode.Graph.RemoveObject(this);

        Parent= null;
        IsEditorDirty= true;
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
    }

    // ----------------------------------------------------------------------
    // NAME & TYPE NAME
    // ----------------------------------------------------------------------
    public bool IsNameVisible { get { return name != null && name.Length != 0 && name[0] != WD_EditorConfig.PrivateStringPrefix; }}
    public string Name {
        get {
            if(name == null || name == "") return null;
            return name[0] == WD_EditorConfig.PrivateStringPrefix ? name.Substring(1) : name;
        }
        set { name= value; }
    }
    public string TypeName {
        get { return GetType().Name.Substring(WD_EditorConfig.TypePrefix.Length); }
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
            ExecuteIf<WD_Node>(
                (node)=> {
                    node.ForEachChild<WD_Port>(
                        (child)=> { child.IsVisible= value; }
                    );
                }
            );
            if(value) return;
            ExecuteIf<WD_Node>(
                (node)=> {
                    node.ForEachChild<WD_Node>(
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
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public WD_Aggregate Parent {
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
    public bool IsChildOf(WD_Aggregate _parent) {
        if(Parent == _parent) return true;
        return Parent == null ? false : Parent.IsChildOf(_parent);
    }
    public virtual void AddChild(WD_Object _object)     {}
    public virtual void RemoveChild(WD_Object _object)  {}
    private class Enumerator : IEnumerator<WD_Object> {
        public void Reset()                 {}
        public bool MoveNext()              { return false; }
        public WD_Object Current            { get { return null; }}
               object IEnumerator.Current   { get { return Current; }}
               void   System.IDisposable.Dispose() {}
    }
    public virtual IEnumerator<WD_Object>   GetEnumerator()                         { return new Enumerator(); }
                   IEnumerator<WD_Object>   IEnumerable<WD_Object>.GetEnumerator()  { return GetEnumerator(); }
                   IEnumerator              IEnumerable.GetEnumerator()             { return GetEnumerator(); }

    // ======================================================================
    // ITERATION UTILITIES
    // ----------------------------------------------------------------------
    public void ForEachChild(System.Action<WD_Object> fnc) {
        foreach(var child in this) { fnc(child); }
    }
    // ----------------------------------------------------------------------
    public void ForEachChild<T>(System.Action<T> fnc) where T : WD_Object {
        foreach(var child in this) { child.ExecuteIf<T>( (obj)=> { fnc(obj);} ); }
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthLast(System.Action<WD_Object> fnc) {
        fnc(this);
        foreach(var child in this) { child.ForEachRecursiveDepthLast(fnc); }
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthFirst(System.Action<WD_Object> fnc) {
        foreach(var child in this) { child.ForEachRecursiveDepthFirst(fnc); }
        fnc(this);
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursive(System.Action<WD_Object> fnc) {
        ForEachRecursiveDepthFirst(fnc);
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthLast<T>(System.Action<T> fnc) where T : WD_Object {
        ForEachRecursiveDepthLast( (obj)=> { obj.ExecuteIf<T>( (t)=> { fnc(t); } ); } );
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthFirst<T>(System.Action<T> fnc) where T : WD_Object {
        ForEachRecursiveDepthFirst( (obj)=> { obj.ExecuteIf<T>( (t)=> { fnc(t); } ); } );
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursive<T>(System.Action<T> fnc) where T : WD_Object {
        ForEachRecursiveDepthFirst<T>(fnc);
    }
    // ----------------------------------------------------------------------
    public int ChildCount() {
        int count= 0;
        ForEachChild( (child)=> { ++count; });
        return count;
    }
    // ----------------------------------------------------------------------
    public int ChildCount<T>() where T : WD_Object {
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
    public int ChildCountRecursive<T>() where T : WD_Object {
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
