using UnityEngine;
using System.Collections;

public class DSTree {
	int[]		myParentIdx;
	object[]	myTreeObjects;
	
	public int Length { get { return myTreeObjects.Length; }}
	public object this[int idx] {
		get { return (idx < 0 || idx >= myTreeObjects.Length) ? null : myTreeObjects[idx]; }
	}
	
	public DSTree() {
		myParentIdx  = new int[0];
		myTreeObjects= new object[0];
	}
	
	public int IdForObject(object theObject) {
		if(theObject == null) return -1;
		for(int i= 0; i < myTreeObjects.Length; ++i) {
			if(myTreeObjects[i] == theObject) return i;
		}
		return -1;
	}
	public void Add(object theObject, object theParent, object thePreviousObject= null) {
		if(theObject == null) return;
		int parentIdx= -1;
		int prevousIdx= -1;
		// Search for the parent.
		if(theParent != null) {
			parentIdx= IdForObject(theParent);
			if(parentIdx < 0) {
				Debug.LogWarning("DSTree: Add aborted: unable to find parent: "+theParent);
				return;
			}
		}
		// Search for the previoud object.
		if(thePreviousObject != null) {
			prevousIdx= IdForObject(thePreviousObject);
			if(prevousIdx < 0) {
				Debug.LogWarning("DSTree: Add aborted: unable to find previous object: "+thePreviousObject);
				return;				
			}
		}
		// Now determine where the new object should be added.
		int objectIdx= -1;
		if(thePreviousObject == null || thePreviousObject == theParent) {
			objectIdx= theParent == null ? 0 : parentIdx+1;
		} else if(myParentIdx[prevousIdx] == parentIdx) {
			objectIdx= prevousIdx+1;
		} else {
			...
		}
	}
}
