using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassOperationsController : DSTableViewDataSource {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    class ControlPair {
        public iCS_ReflectionDesc  Component= null;
        public bool                IsActive= false;
        public ControlPair(iCS_ReflectionDesc component, bool isActive= false) {
            Component= component;
            IsActive= isActive;
        }
    };

    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	Type				myClassType= null;
	iCS_EditorObject	myTarget   = null;
	iCS_IStorage		myStorage  = null;
    GUIContent    		myTitle    = null;
    DSTableView			myTableView= null;
	ControlPair[]		myMethods  = null;
    
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int     kSpacer           = 8;
    const int     kMarginSize       = 10;
	const string  kDefaultTitle     = "Operations";
	const string  kOperationColumnId= "Operation";

    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public DSView View { get { return myTableView; }}
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
	public iCS_ClassOperationsController() {}
    public iCS_ClassOperationsController(Type classType, iCS_IStorage storage, GUIContent title= null, iCS_EditorObject target= null) {
		OnActivate(classType, storage, title, target);
	}
	public void OnActivate(Type classType, iCS_IStorage storage, GUIContent title= null, iCS_EditorObject target= null) {
		// Configuration parameters.
		myClassType= classType;
		myStorage= storage;
		myTitle= title ?? new GUIContent(kDefaultTitle);
		myTarget= target;
		
		// Common varaibles.

		// Extract fields & properties from class descriptor.
        List<ControlPair> methods= new List<ControlPair>();
        foreach(var component in iCS_DataBase.GetClassMethods(myClassType)) {
            bool isActive= (myTarget != null && myStorage != null) ? myStorage.ClassModuleFindFunction(myTarget, component) != null : false;
            methods.Add(new ControlPair(component, isActive));
        }
        myMethods= methods.ToArray();
    	Array.Sort(myMethods, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));

		// Build view
        myTableView= new DSTableView(myTitle, TextAlignment.Center, false, new RectOffset(kSpacer,kSpacer,0,kSpacer));
        myTableView.DataSource= this;
		DSTableColumn operationColumn= new DSTableColumn(kOperationColumnId, null, TextAlignment.Left, false, new RectOffset(0,0,0,0));
		myTableView.AddSubview(operationColumn);
    }

    // =================================================================================
    // TableViewDataSource
    // ---------------------------------------------------------------------------------
    public int NumberOfRowsInTableView(DSTableView tableView) {
		return myClassType != null ? myMethods.Length : 0;
    }
    // ---------------------------------------------------------------------------------
    public Vector2 DisplaySizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
		if(myClassType == null) return Vector2.zero;
        var signatureSize= EditorStyles.boldLabel.CalcSize(new GUIContent(myMethods[row].Component.FunctionSignatureNoThis));
		signatureSize.x+= 12f;
		return signatureSize;
    }
    // ---------------------------------------------------------------------------------
    public void DisplayObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row, Rect position) {
		if(myClassType == null) return;
        GUIStyle style= GUI.skin.button;
        var alignment= style.alignment;
        var fontStyle= style.fontStyle;
        var textColor= style.normal.textColor;        
        var background= style.normal.background;
        style.alignment= TextAnchor.MiddleLeft;
        if(myMethods[row].IsActive) {
            style.normal.textColor= Color.white;
            style.fontStyle= FontStyle.Bold;
            style.normal.background= style.active.background;            
        } else {
            style.fontStyle= FontStyle.Italic;
        }
		position.width= tableColumn.DataSize.x;
        if(GUI.Button(position, myMethods[row].Component.FunctionSignatureNoThis) && myTarget != null && myStorage != null) {
            myMethods[row].IsActive^= true;
            if(myMethods[row].IsActive) {
                myStorage.ClassModuleCreate(myTarget, myMethods[row].Component);
            } else {
                myStorage.ClassModuleDestroy(myTarget, myMethods[row].Component);
            }
        }
        style.normal.textColor= textColor;
        style.normal.background= background;
        style.fontStyle= fontStyle;
        style.alignment= alignment;
    }

}
