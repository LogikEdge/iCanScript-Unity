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

//    DSVerticalLayoutView            MainView                 = null;
//	DSAccordionView					AccordionView            = null;
//    DSVerticalLayoutView            LayoutView               = null;
    DSTitleView                     TitleView= null;
    DSCellView                      ConstructorView          = null;
	DSSearchView					SearchView               = null;
//    iCS_ClassListController     	ClassListController      = null;
//	iCS_ClassVariablesController	ClassVariablesController = null;
//	iCS_ClassOperationsController	ClassOperationsController= null;

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
//        if(LayoutView != null) LayoutView.Title= new GUIContent(myTarget.Name);

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
        LabelSize        = EditorStyles.label.CalcSize(ConstructorTitle); 
        ConstructorTitleSize= EditorStyles.boldLabel.CalcSize(ConstructorTitle);
                         
        LabelHeight      = 4f+LabelSize.y;
                         

        // Create frame layout object.
        string classTitle= myTarget != null ? myTarget.Name : "Class Wizard";
        GUIContent classWizardTitle= new GUIContent(classTitle);
        TitleView= new DSTitleView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), true,
                                   classWizardTitle, DSView.AnchorEnum.Center, true,
                                   TitleViewDisplay, TitleViewGetSizeToDisplay);
        TitleView.Anchor= DSView.AnchorEnum.Center;
//		LayoutView= new DSVerticalLayoutView(classWizardTitle, TextAlignment.Center, false, new RectOffset(0,0,0,0));
        
        // Populate frame layout.
		SearchView= new DSSearchView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), false, 12, OnSearch);
		SearchView.Anchor= DSView.AnchorEnum.Center;
		
//        ClassListController= new iCS_ClassListController();
//        ClassListController.View.DisplayRatio= new Vector2(1f, 0.25f);

        ConstructorView= new DSCellView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), false, DrawConstructorCell, GetConstrcutorContentSize);
        ConstructorView.Anchor= DSView.AnchorEnum.CenterRight;

//		ClassVariablesController= new iCS_ClassVariablesController(myTarget.RuntimeType, myStorage, VariableTitle, myTarget);
//		ClassOperationsController= new iCS_ClassOperationsController(myTarget.RuntimeType, myStorage, MethodTitle, myTarget);

//        LayoutView.AddSubview(SearchView);
//        LayoutView.AddSubview(ClassListController.View);
//        LayoutView.AddSubview(ConstructorView);
//		LayoutView.AddSubview(ClassVariablesController.View);
//		LayoutView.AddSubview(ClassOperationsController.View);

		// Build accrodion view
//		AccordionView= new DSAccordionView(new RectOffset(0,0,0,0), false);
//		AccordionView.AddSubview(ClassListController.View);
//		AccordionView.AddSubview(ClassVariablesController.View);
//		AccordionView.AddSubview(ClassOperationsController.View);

//		MainView= new DSVerticalLayoutView(classWizardTitle, TextAlignment.Center, false, new RectOffset(0,0,0,0));
//		MainView.AddSubview(SearchView);
//		MainView.AddSubview(AccordionView);
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        // Wait until window is configured.
        if(myTarget == null) return;
        EditorGUIUtility.LookLikeInspector();
//        LayoutView.Display(new Rect(0,0,position.width, position.height));
//		MainView.Display(new Rect(0,0,position.width, position.height));
//		AccordionView.Display(new Rect(0,0,position.width, position.height));
        TitleView.Display(new Rect(0,0,position.width, position.height));
//        ConstructorView.Display(new Rect(0,0,position.width, position.height));
        SearchView.Display(new Rect(0,0,position.width, position.height));
    }

    // =================================================================================
    // Constructor selection view
    // ---------------------------------------------------------------------------------
    void TitleViewDisplay(DSTitleView view, Rect displayArea) {
        ConstructorView.Display(displayArea);
    }
    Vector2 TitleViewGetSizeToDisplay(DSTitleView view, Rect displayArea) {
        return ConstructorView.GetSizeToDisplay(displayArea);
    }
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
