using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
	public class Tree<T> {
	    // =================================================================================
	    // Fields
	    // ---------------------------------------------------------------------------------
		T				myValue= default(T);
		List<Tree<T>>	myChildren= null;
		
	    // =================================================================================
	    // Properties
	    // ---------------------------------------------------------------------------------
		public T 		     Value		{ get { return myValue; } set { myValue= value; }}
		public List<Tree<T>> Children	{ get { return myChildren; }}

	    // =================================================================================
	    // Initialization
	    // ---------------------------------------------------------------------------------
		public Tree(T value) { myValue= value; }

	    // =================================================================================
	    // Child management.
	    // ---------------------------------------------------------------------------------
		public void AddChild(Tree<T> node) {
			if(myChildren == null) myChildren= new List<Tree<T>>();
			myChildren.Add(node);			
		}
		public void AddChild(T childValue)	{
			AddChild(new Tree<T>(childValue));
		}
		public bool RemoveChild(T childValue) {
			if(myChildren == null) return false;
			int idx= myChildren.FindIndex(x=> x.Value.Equals(childValue));
			if(idx < 0) return false;
			myChildren.RemoveAt(idx);
			return true;
		}
		public void Sort(Func<T,T,int> criteria) {
			if(myChildren == null) return;
			myChildren.Sort((x,y)=> criteria(x.Value, y.Value));
		}
	}
}

