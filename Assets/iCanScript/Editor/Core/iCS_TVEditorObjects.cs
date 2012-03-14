using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCS_TVEditorObjects : iCS_ITreeViewItem {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_IStorage		myStorage = null;
	iCS_EditorObject	myObject  = null;
	bool				myIsFolded= true;

    // =================================================================================
    // ITreeViewItem implementation
    // ---------------------------------------------------------------------------------
	public new string	ToString()			{ return myObject.Name; }
	public bool			IsFolded()			{ return myIsFolded; }
	public void			SetIsFolded(bool f)	{ myIsFolded= f; }
	public object		GetParent()			{ return myStorage.GetParent(myObject); }
	public bool			HasChildren()		{ return myStorage.NbOfChildren(myObject) != 0; }
	public object[]		GetChildren()		{
		List<iCS_TVEditorObjects> children= new List<iCS_TVEditorObjects>();
		myStorage.ForEachChild(myObject,
			c=> children.Add(new iCS_TVEditorObjects(c, myStorage))
		);
		return children.ToArray();
	}
	
    // =================================================================================
    // Constructor
    // ---------------------------------------------------------------------------------
	public iCS_TVEditorObjects(iCS_EditorObject obj, iCS_IStorage storage) {
		myStorage= storage;
		myObject= obj;
	}
}
