using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_CreateClassController {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject                myTarget             = null;
    iCS_IStorage                    myStorage            = null;
    iCS_ConstructorInfo[]           myConstructors       = null;
    int                             myConstructorIdx     = -1;
    float                           myMaxConstructorWidth= 0f;

    DSVerticalLayoutView            myMainView            = null;
	DSSearchView					mySearchView          = null;
//    DSCellView                      myConstructorView     = null;
    iCS_ClassListController     	myClassListController = null;
	iCS_ClassVariablesController	myVariablesController = null;
	iCS_ClassOperationsController	myOperationsController= null;

    // =================================================================================
    // Constant GUI Content
    // ---------------------------------------------------------------------------------
    GUIContent  ConstructorTitle= new GUIContent("Instance");
    GUIContent  VariableTitle   = new GUIContent("Variables");
    GUIContent  MethodTitle     = new GUIContent("Operations");
    Vector2     ConstructorTitleSize;
    Vector2     LabelSize;
    float       LabelHeight;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kSpacer= 8;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public DSView View { get { return myMainView; }}
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    public iCS_CreateClassController() {
        InitLayoutInfo();
        InitConstructorInfo();
        BuildViews();        
    }
    
    // ---------------------------------------------------------------------------------
    void InitLayoutInfo() {
        LabelSize           = EditorStyles.label.CalcSize(ConstructorTitle); 
        LabelHeight         = 4f+LabelSize.y;
        ConstructorTitleSize= EditorStyles.boldLabel.CalcSize(ConstructorTitle);
    }

    // ---------------------------------------------------------------------------------
    void InitConstructorInfo() {      
        // Get and sort all constructors for the given class.
        myConstructors= iCS_LibraryDatabase.GetConstructors(myTarget.RuntimeType);
    	Array.Sort(myConstructors, (x,y)=> x.FunctionSignatureNoThis.CompareTo(y.FunctionSignatureNoThis));        

        // Determine which constrcutor is currently used for this instance.
        myConstructorIdx= -1;
        for(int i= 0; i < myConstructors.Length; ++i) {
            iCS_EditorObject existing= myStorage.InstanceWizardGetConstructor(myTarget);
            if(existing != null && myConstructors[i].Method == existing.GetMethodBase(myStorage.EditorObjects)) {
                myConstructorIdx= i;
            }
            var constructorSize= EditorStyles.boldLabel.CalcSize(new GUIContent(myConstructors[i].FunctionSignatureNoThisNoOutput));
            if(constructorSize.x+12f > myMaxConstructorWidth) {
                myMaxConstructorWidth= constructorSize.x+12f;
            }
        }
    }

    // ---------------------------------------------------------------------------------
    void BuildViews() {
        // Create children views.
		mySearchView= new DSSearchView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), false, 12, OnSearch);
		mySearchView.Anchor= DSView.AnchorEnum.Center;
//        myConstructorView= new DSCellView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), false, DrawConstructorCell, GetConstrcutorContentSize);
		
        myClassListController= new iCS_ClassListController(OnClassSelection);
		myVariablesController= new iCS_ClassVariablesController(myTarget.RuntimeType, myStorage, VariableTitle, myTarget);
		myOperationsController= new iCS_ClassOperationsController(myTarget.RuntimeType, myStorage, MethodTitle, myTarget);

		// Build Class list layout view.
		myMainView= new DSVerticalLayoutView(new RectOffset(0,0,0,0), true);
		myMainView.AddSubview(mySearchView, new RectOffset(kSpacer, kSpacer, 0, kSpacer), DSView.AnchorEnum.Center);
		myMainView.AddSubview(myClassListController.View, new RectOffset(kSpacer, kSpacer, 0, kSpacer), DSView.AnchorEnum.Center);
		myMainView.AddSubview(myVariablesController.View, new RectOffset(kSpacer, kSpacer, 0, kSpacer), DSView.AnchorEnum.Center);
		myMainView.AddSubview(myOperationsController.View, new RectOffset(kSpacer, kSpacer, 0, kSpacer), DSView.AnchorEnum.Center);
    }

    // =================================================================================
    // Constructor selection view
    // ---------------------------------------------------------------------------------
    Vector2 GetConstrcutorContentSize(DSView view, Rect displayArea) {
        return new Vector2(ConstructorTitleSize.x+2f*kSpacer+myMaxConstructorWidth, LabelHeight);
    }
    void DrawConstructorCell(DSView view, Rect displayArea) {
        float y= displayArea.y;
        GUI.Label(new Rect(displayArea.x, y, ConstructorTitleSize.x, ConstructorTitleSize.y), ConstructorTitle, EditorStyles.boldLabel);
        float x= displayArea.x+ConstructorTitleSize.x+2f*kSpacer;
        // Fill-in available constructors.
        string[] instanceOptions= new string[1+myConstructors.Length];
        instanceOptions[0]= "Use input port (this)";
        for(int i= 0; i < myConstructors.Length; ++i) {
            instanceOptions[i+1]= myConstructors[i].FunctionSignatureNoThisNoOutput;
        }
        float maxWidth= displayArea.width-(ConstructorTitleSize.x+2f*kSpacer);
        float width= Mathf.Min(maxWidth, myMaxConstructorWidth);
        int instanceIdx= myConstructorIdx+1;
        int newIdx= EditorGUI.Popup(new Rect(x, y, width, LabelHeight), instanceIdx, instanceOptions, EditorStyles.toolbarPopup);
        if(GUI.changed) {
            myConstructorIdx= -1;
            if(instanceIdx != 0) {
                myStorage.InstanceWizardDestroyConstructor(myTarget);
            }
            if(newIdx != 0) {
                myConstructorIdx= newIdx-1;
                myStorage.InstanceWizardCreateConstructor(myTarget, myConstructors[newIdx-1]);
            }
        }
    }


    // =================================================================================
    // View Test
    // ---------------------------------------------------------------------------------
	void OnSearch(DSSearchView searchView, string searchString) {
		myClassListController.Filter(searchString, null, null);
	}
	void OnClassSelection(Type classType) {
	    Debug.Log("Selection class: "+classType.Name);
	}
}
