using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
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
        public iCS_PropertyController(iCS_EditorObject target, iCS_IStorage iStorage) {
            // Update main state variables.
            myTarget= target;
            myStorage= iStorage;
            if(!IsValid) return;
        
            BuildViews();
        }
    
        // ---------------------------------------------------------------------------------
        void BuildViews() {    
    		var typeName= new GUIContent(NameUtility.ToDisplayName(iCS_Types.TypeName(myTarget.RuntimeType)));
    		myVariablesController = new iCS_ClassVariablesController(myTarget.RuntimeType, myStorage, typeName, myTarget);

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

}
