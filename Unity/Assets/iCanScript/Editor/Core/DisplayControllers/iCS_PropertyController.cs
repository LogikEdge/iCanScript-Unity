using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_PropertyController {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject                myTarget             = null;
    iCS_IStorage                    myStorage            = null;

    DSTitleView                     myMainView            = null;
    DSVerticalLayoutView            myLayoutView          = null;
	iCS_ClassVariablesController	myVariablesController = null;

    // =================================================================================
    // Constant GUI Content
    // ---------------------------------------------------------------------------------
    GUIContent  VariableTitle   = new GUIContent("Properties");
    Vector2     LabelSize;
    float       LabelHeight;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kSpacer= 8;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public DSView           View        { get { return myMainView; }}
    public bool             IsValid     { get { return myTarget != null && myStorage != null; }}
    public iCS_EditorObject Target      { get { return myTarget; }}
    public iCS_IStorage     IStorage    { get { return myStorage; }}
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_PropertyController(iCS_EditorObject target, iCS_IStorage storage) {
        // Update main state variables.
        myTarget= target;
        myStorage= storage;
        if(!IsValid) return;
        
        BuildViews();
    }
    
    // ---------------------------------------------------------------------------------
    void BuildViews() {    
		myVariablesController = new iCS_ClassVariablesController(myTarget.RuntimeType, myStorage, VariableTitle, myTarget);

		// Build class wizard layout view.
		myLayoutView= new DSVerticalLayoutView(new RectOffset(0,0,0,0), false);
		myLayoutView.AddSubview(myVariablesController.View, new RectOffset(0, 0, 0, kSpacer), DSView.AnchorEnum.Center);
		
		// Build class wizard title view.
        string instanceTitle= myTarget.DisplayName;
        GUIContent instanceEditorTitle= new GUIContent(instanceTitle);
		myMainView= new DSTitleView(new RectOffset(0,0,0,0), false, instanceEditorTitle, DSView.AnchorEnum.Center, true);
		myMainView.SetSubview(myLayoutView);
    }

}
