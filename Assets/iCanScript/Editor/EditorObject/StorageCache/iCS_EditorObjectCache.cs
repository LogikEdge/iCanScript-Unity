using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public class iCS_EditorObjectCache {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public bool             IsValid     = false;
    public int              ParentId    = -1;
    public List<int>        Children    = new List<int>();
//	public object		    InitialValue= null;
	// Graph Animation
	public Rect				VisiblePosition = new Rect(0,0,0,0);					
//	public P.Animate<Rect>	AnimatedPosition= new P.Animate<Rect>();
	
    // ======================================================================
    // Operations
    // ----------------------------------------------------------------------
    public iCS_EditorObjectCache()  { Reset(); }
    public void Reset() {
        IsValid= false;
        ParentId= -1;
        Children.Clear();
//		InitialValue= null;
//		AnimatedPosition.Reset();
    }
    public void AddChild(int id, iCS_EditorObjectCache toAdd) {
        if(Prelude.elem(id, Children.ToArray())) return;
        Children.Add(id);
    }
    public void RemoveChild(int id, iCS_EditorObjectCache toDelete) {
        for(int i= 0; i < Children.Count; ++i) {
            if(Children[i] == id) {
                Children.RemoveAt(i);
                return;
            }
        }
    }
    public bool AreChildrenInSameOrder(int[] orderedChildren) {
        int i= 0;
        for(int j= 0; j < Children.Count; ++j) {
            if(Children[j] == orderedChildren[i]) {
                if(++i >= orderedChildren.Length) return true;
            };
        }
        return false;
    }
    public void ReorderChildren(int[] orderedChildren) {
        if(AreChildrenInSameOrder(orderedChildren)) return;
        List<int> others= Prelude.filter(c=> Prelude.notElem(c,orderedChildren), Children);
        int i= 0;
        Prelude.forEach(c=> Children[i++]= c, orderedChildren);
        Prelude.forEach(c=> Children[i++]= c, others);
    }
}
