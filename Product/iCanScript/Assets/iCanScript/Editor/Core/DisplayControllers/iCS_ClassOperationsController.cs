using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor {
    
    public class iCS_ClassOperationsController : DSTableViewDataSource {
        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
        const int     kSpacer           = 8;
        const int     kMarginSize       = 10;
    	const string  kDefaultTitle     = "Operations";
    	const string  kOperationColumnId= "Operation";

        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
    	Type				myClassType      = null;
    	iCS_EditorObject	myTarget         = null;
    	iCS_IStorage		myStorage        = null;
        GUIContent    		myTitle          = null;
        DSTableView			myTableView      = null;
    	iCS_MethodInfo[]    myMethods        = null;
    	bool[]              myIsMethodPresent= null;
    
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
		
    		// Extract fields & properties from class descriptor.
            myMethods= iCS_LibraryDatabase.GetMethods(myClassType);
        	Array.Sort(myMethods, (x,y)=> x.FunctionSignatureNoThis.CompareTo(y.FunctionSignatureNoThis));

            // Build method presence on the given target.
        	myIsMethodPresent= new bool[myMethods.Length];
            if(myTarget != null && myStorage != null) {
                for(int i= 0; i < myMethods.Length; ++i) {
                    myIsMethodPresent[i]= myStorage.InstanceWizardFindFunction(myTarget, myMethods[i]) != null;
                }            
            }

    		// Build view
            myTableView= new DSTableView(new RectOffset(0,0,0,0), true, myTitle, DSView.AnchorEnum.Center, true, true);
            myTableView.DataSource= this;
    		DSTableColumn operationColumn= new DSTableColumn(kOperationColumnId, new RectOffset(0,0,0,0), null, DSView.AnchorEnum.CenterLeft);
    		myTableView.AddColumn(operationColumn);
        }

        // =================================================================================
        // TableViewDataSource
        // ---------------------------------------------------------------------------------
        public int NumberOfRowsInTableView(DSTableView tableView) {
    		return myClassType != null ? myMethods.Length : 0;
        }
        // ---------------------------------------------------------------------------------
        public Vector2 LayoutSizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
    		if(myClassType == null) return Vector2.zero;
            var signatureSize= EditorStyles.boldLabel.CalcSize(new GUIContent(myMethods[row].FunctionSignatureNoThis));
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
            if(myIsMethodPresent[row]) {
                style.normal.textColor= Color.white;
                style.fontStyle= FontStyle.Bold;
                style.normal.background= style.active.background;            
            } else {
                style.fontStyle= FontStyle.Italic;
            }
    		position.width= tableColumn.DataSize.x;
            if(GUI.Button(position, myMethods[row].FunctionSignatureNoThis) && myTarget != null && myStorage != null) {
                myIsMethodPresent[row]^= true;
                if(myIsMethodPresent[row]) {
                    iCS_UserCommands.CreateInstanceWizardElement(myTarget, myMethods[row]);
                } else {
                    iCS_UserCommands.DeleteInstanceWizardElement(myTarget, myMethods[row]);
                }
            }
            style.normal.textColor= textColor;
            style.normal.background= background;
            style.fontStyle= fontStyle;
            style.alignment= alignment;
        }
        // ---------------------------------------------------------------------------------
    	public void OnMouseDown(DSTableView tableView, DSTableColumn tableColumn, int row) {}

    }

}
