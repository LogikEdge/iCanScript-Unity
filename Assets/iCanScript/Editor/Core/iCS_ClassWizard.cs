using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject                myTarget        = null;
    iCS_IStorage                    myStorage       = null;
    iCS_ReflectionDesc[]            myConstructors  = null;
    int                             myConstructorIdx= -1;

	DSAccordionView					MainView             = null;
    DSVerticalLayoutView            ClassListLayoutView  = null;
    DSVerticalLayoutView            ClassWizardLayoutView= null;
    DSTitleView                     ClassWizardTitleView = null;
	DSSearchView					SearchView               = null;
    DSCellView                      ConstructorView          = null;
    iCS_ClassListController     	ClassListController      = null;
	iCS_ClassVariablesController	ClassVariablesController = null;
	iCS_ClassOperationsController	ClassOperationsController= null;

    // =================================================================================
    // Layout info.
    // ---------------------------------------------------------------------------------
    float       MaxConstructorWidth= 0f;

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
    
    // ---------------------------------------------------------------------------------
    void Init() {
        myTarget= null;
        myStorage= null;
    }
    // ---------------------------------------------------------------------------------
    public void OnActivate(iCS_EditorObject target, iCS_IStorage storage) {
        // Transform invalid activation to a deactivation.
        if(target == null || storage == null) {
            Init();
            return;
        }
        
        // Update main state variables.
        if(target == myTarget && storage == myStorage) return;
        myTarget= target;
        myStorage= storage;
        if(ClassWizardTitleView != null) ClassWizardTitleView.Title= new GUIContent(myTarget.Name);

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
            if(constructorSize.x+12f > MaxConstructorWidth) {
                MaxConstructorWidth= constructorSize.x+12f;
            }
        }

        // Initialize display variables.
		InitConstantGUIContent();
        Repaint();
    }
    // ---------------------------------------------------------------------------------
    public void OnDeactivate() {
        Init();
        Repaint();
    }
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        Init();

    }
    // ---------------------------------------------------------------------------------
    void OnDisable() {
        Init();
    }
    // ---------------------------------------------------------------------------------
    void InitConstantGUIContent() {
        // Compute content size.
        LabelSize           = EditorStyles.label.CalcSize(ConstructorTitle); 
        ConstructorTitleSize= EditorStyles.boldLabel.CalcSize(ConstructorTitle);
        LabelHeight      = 4f+LabelSize.y;
                         
        // Create children views.
		SearchView= new DSSearchView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), false, 12, OnSearch);
		SearchView.Anchor= DSView.AnchorEnum.Center;
        ConstructorView= new DSCellView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), false, DrawConstructorCell, GetConstrcutorContentSize);
		
        ClassListController= new iCS_ClassListController();
		ClassVariablesController= new iCS_ClassVariablesController(myTarget.RuntimeType, myStorage, VariableTitle, myTarget);
		ClassOperationsController= new iCS_ClassOperationsController(myTarget.RuntimeType, myStorage, MethodTitle, myTarget);

		// Build Class list layout view.
		ClassListLayoutView= new DSVerticalLayoutView(new RectOffset(0,0,0,0), true);
		ClassListLayoutView.AddSubview(SearchView);
		ClassListLayoutView.AddSubview(ClassVariablesController.View);
		ClassListLayoutView.AddSubview(ClassOperationsController.View);
		
		// Build class wizard layout view.
		ClassWizardLayoutView= new DSVerticalLayoutView(new RectOffset(0,0,0,0), true);
		ClassWizardLayoutView.AddSubview(ConstructorView);
		ClassWizardLayoutView.AddSubview(ClassVariablesController.View);
		ClassWizardLayoutView.AddSubview(ClassOperationsController.View);
		
		// Build class wizard title view.
        string classTitle= myTarget != null ? myTarget.Name : "Class Wizard";
        GUIContent classWizardTitle= new GUIContent(classTitle);
		ClassWizardTitleView= new DSTitleView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), true, classWizardTitle, DSView.AnchorEnum.Center, true);
		ClassWizardTitleView.SetSubview(ClassWizardLayoutView);
		
		// Build accrodion view.
		MainView= new DSAccordionView(new RectOffset(0,0,0,0), false);
		MainView.AddSubview(new GUIContent("Create")      , ClassListLayoutView);
		MainView.AddSubview(new GUIContent("Class Wizard"), ClassWizardTitleView);
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        // Wait until window is configured.
        if(myTarget == null) return;
        EditorGUIUtility.LookLikeInspector();
		MainView.Display(new Rect(0,0,position.width, position.height));
    }

    // =================================================================================
    // Constructor selection view
    // ---------------------------------------------------------------------------------
    Vector2 GetConstrcutorContentSize(DSView view, Rect displayArea) {
        return new Vector2(ConstructorTitleSize.x+2f*kSpacer+MaxConstructorWidth, LabelHeight);
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
        float width= Mathf.Min(maxWidth, MaxConstructorWidth);
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


    // =================================================================================
    // View Test
    // ---------------------------------------------------------------------------------
	void OnSearch(DSSearchView searchView, string searchString) {
//		ClassListController.Filter(searchString, null, null);
        Debug.Log("Searching for: "+searchString);
	}
}
