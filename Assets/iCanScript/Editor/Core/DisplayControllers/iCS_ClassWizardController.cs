using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizardController {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject                myTarget             = null;
    iCS_IStorage                    myStorage            = null;
    iCS_ReflectionDesc[]            myConstructors       = null;
    int                             myConstructorIdx     = -1;
    float                           myMaxConstructorWidth= 0f;

    DSTitleView                     myMainView            = null;
    DSVerticalLayoutView            myLayoutView          = null;
    DSCellView                      myConstructorView     = null;
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
    public DSView           View        { get { return myMainView; }}
    public bool             IsValid     { get { return myTarget != null && myStorage != null; }}
    public iCS_EditorObject Target      { get { return myTarget; }}
    public iCS_IStorage     IStorage    { get { return myStorage; }}
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_ClassWizardController(iCS_EditorObject target, iCS_IStorage storage) {
        // Update main state variables.
        myTarget= target;
        myStorage= storage;
        if(!IsValid) return;
        
        InitLayoutInfo();
        InitConstructorInfo();
        BuildViews();
    }
    
    // ---------------------------------------------------------------------------------
    void InitLayoutInfo() {
        // Compute content size.
        LabelSize           = EditorStyles.label.CalcSize(ConstructorTitle); 
        LabelHeight         = 4f+LabelSize.y;
        ConstructorTitleSize= EditorStyles.boldLabel.CalcSize(ConstructorTitle);        
    }
    
    // ---------------------------------------------------------------------------------
    void InitConstructorInfo() {
        // Get and sort all constructors for the given class.
        myConstructors= iCS_DataBase.GetClassConstructors(myTarget.RuntimeType);
    	Array.Sort(myConstructors, (x,y)=> x.FunctionSignatureNoThis.CompareTo(y.FunctionSignatureNoThis));        

        // Determine which constrcutor is currently used for this instance.
        myConstructorIdx= -1;
        for(int i= 0; i < myConstructors.Length; ++i) {
            iCS_EditorObject existing= myStorage.ClassModuleGetConstructor(myTarget);
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
        myConstructorView     = new DSCellView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), false, DrawConstructorCell, GetConstrcutorContentSize);
		myVariablesController = new iCS_ClassVariablesController(myTarget.RuntimeType, myStorage, VariableTitle, myTarget);
		myOperationsController= new iCS_ClassOperationsController(myTarget.RuntimeType, myStorage, MethodTitle, myTarget);

		// Build class wizard layout view.
		myLayoutView= new DSVerticalLayoutView(new RectOffset(0,0,0,0), false);
		myLayoutView.AddSubview(myConstructorView, new RectOffset(0, 0, 0, 0));
		myLayoutView.AddSubview(myVariablesController.View, new RectOffset(0, 0, 0, kSpacer), DSView.AnchorEnum.Center);
		myLayoutView.AddSubview(myOperationsController.View, new RectOffset(0, 0, 0, 0), DSView.AnchorEnum.Center);
		
		// Build class wizard title view.
        string classTitle= myTarget.Name;
        GUIContent classWizardTitle= new GUIContent(classTitle);
		myMainView= new DSTitleView(new RectOffset(0,0,0,0), false, classWizardTitle, DSView.AnchorEnum.Center, true);
		myMainView.SetSubview(myLayoutView);
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
                myStorage.ClassModuleDestroyConstructor(myTarget);
            }
            if(newIdx != 0) {
                myConstructorIdx= newIdx-1;
                myStorage.ClassModuleCreateConstructor(myTarget, myConstructors[newIdx-1]);
            }
        }
    }
}
