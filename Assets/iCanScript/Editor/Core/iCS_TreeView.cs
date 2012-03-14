using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_TreeView : EditorWindow {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    internal class Tree<KEY,DATA> {
        public Tree<KEY,DATA>       Parent= null;
        public KEY                  Key;
        public DATA                 Data= default(DATA);
        public List<Tree<KEY,DATA>> Children= null;
        public Tree(Tree<KEY,DATA> parent, KEY k, DATA d)   { Parent= parent; Key= k; Data= d; }
               ~Tree()                                      { Parent= null; Key= default(KEY); Data= default(DATA); }
        public bool IsLeaf                                  { get { return Children == null || Children.Count == 0; }}
        public void Add(KEY key, DATA data)                 { Children.Add(new Tree<KEY,DATA>(this,key,data)); }
        public bool Remove(KEY key) {
            int i= Children.FindIndex(node=> Object.Equals(node.Key, key));
            if(i < 0) return false;
            Children.RemoveAt(i);
            return true;
        }
    }

    internal class Control {
        public bool IsFolded= true;
        public bool IsSelected= false;
    }
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	Tree<object,Control>    myTree= null;
	Vector2		            myScrollPosition= Vector2.zero;
	
    // ---------------------------------------------------------------------------------
    public void Refresh(object newItems) {
        myTree= Refresh(myTree, newItems);
    }
    // ---------------------------------------------------------------------------------
    Tree<object,Control> Refresh(Tree<object,Control> existingItems, object newItems) {
        // Create items if non-existing.
        if(existingItems == null && newItems != null) {
            Tree<object,Control> result= new Tree<object,Control>(null, newItems, new Control());
            iCS_ITree iTree= newItems as iCS_ITree;
            if(iTree != null) {
                object[] children= iTree.GetChildren();
                if(children != null && children.Length != 0) {
                    result.Children= new List<Tree<object,Control>>();
                    foreach(var c in children) {
                        var child= new Tree<object,Control>(result,c,new Control());
                        result.Children.Add(child);
                        Refresh(child,c);
                    }
                }
            }
            return result;
        }
        return existingItems;
    }
    // ---------------------------------------------------------------------------------
    void OnEnable() {
    }
    // ---------------------------------------------------------------------------------
    void OnDisable() {
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
		if(myTree == null) return;

		int level= 0;
		myScrollPosition= EditorGUILayout.BeginScrollView(myScrollPosition);
        DisplayItem(myTree);
		EditorGUILayout.EndScrollView();
	}
    // ---------------------------------------------------------------------------------
	void DisplayItem(Tree<object,Control> item) {
		if(item.Children == null || item.Children.Count == 0) {
			EditorGUILayout.SelectableLabel(item.Key.ToString());
		} else {
			item.Data.IsFolded= EditorGUILayout.Foldout(item.Data.IsFolded, item.Key.ToString());
            if(!item.Data.IsFolded) {
                foreach(var i in item.Children) {
                    DisplayItem(i);
                }
            }
		}
	}
}
