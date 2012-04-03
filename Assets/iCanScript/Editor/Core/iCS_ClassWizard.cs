using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : EditorWindow {
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
    iCS_EditorObject        Target                = null;
    iCS_IStorage            Storage               = null;
    Type                    ClassType             = null;
    ControlPair[]           Constructors          = null;
    DSCellView              ConstructorView       = null;
    
    iCS_ClassListController     	ClassListController      = null;
	iCS_ClassVariablesController	ClassVariablesController = null;
	iCS_ClassOperationsController	ClassOperationsController= null;
    DSVerticalLayoutView        	LayoutView               = null;

    // =================================================================================
    // Layout info.
    // ---------------------------------------------------------------------------------
    float       MaxConstructorWidth= 0f;

    // =================================================================================
    // Constant GUI Content
    // ---------------------------------------------------------------------------------
    GUIContent  InstanceTitle= new GUIContent("Instance");
    GUIContent  VariableTitle= new GUIContent("Variables");
    GUIContent  MethodTitle  = new GUIContent("Operations");
    Vector2     LabelSize;
    Vector2     InstanceTitleSize;
    float       LabelHeight;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int     kSpacer           = 8;
    const int     kMarginSize       = 10;
    const float   kScrollerSize     = 16f;
    const float   kCheckBoxWidth    = 25f;
    const string  kInColumnId       = "In";
    const string  kOutColumnId      = "Out";
    const string  kNameColumnId     = "Name";
    const string  kTypeColumnId     = "Type";
	const string  kOperationColumnId= "Operation";
	const string  kClassTypeColumnId= "Class";
	const string  kPackageColumnId  = "Package";
	const string  kCompanyColumnId  = "Company";
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------------
    void Init() {
        Target= null;
        Storage= null;
    }
    // ---------------------------------------------------------------------------------
    public void OnActivate(iCS_EditorObject target, iCS_IStorage storage) {
        // Transform invalid activation to a deactivation.
        if(target == null || storage == null) {
            Init();
            return;
        }
        // Nothing to do if target does not change.
        if(target == Target && storage == Storage) return;
        // Update main state variables.
        Target= target;
        Storage= storage;
        if(LayoutView != null) LayoutView.Title= new GUIContent(Target.Name);
        // Build class data.
        ClassType= target.RuntimeType;
        List<ControlPair> constructors= new List<ControlPair>();
        foreach(var component in iCS_DataBase.GetClassConstructors(ClassType)) {
            bool isActive= false;
            iCS_EditorObject existing= Storage.ClassModuleGetConstructor(Target);
            if(existing != null && component.Method == existing.GetMethodBase(Storage.EditorObjects)) {
                isActive= true;
            }
            constructors.Add(new ControlPair(component, isActive));
            var constructorSize= EditorStyles.boldLabel.CalcSize(new GUIContent(component.FunctionSignatureNoThisNoOutput));
            if(constructorSize.x+12f > MaxConstructorWidth) {
                MaxConstructorWidth= constructorSize.x+12f;
            }
        }
        Constructors= constructors.ToArray();
    	Array.Sort(Constructors, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));        
        Target= target;
		InitConstantGUIContent();
        Repaint();
    }
    // ---------------------------------------------------------------------------------
    public void Deactivate() {
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
        LabelSize        = EditorStyles.label.CalcSize(new GUIContent("abc")); 
        InstanceTitleSize= EditorStyles.boldLabel.CalcSize(InstanceTitle);
                         
        LabelHeight      = 4f+LabelSize.y;
                         
        // Create frame layout object.
        string classTitle= Target != null ? Target.Name : "Class Wizard";
        GUIContent classWizardTitle= new GUIContent(classTitle);
		LayoutView= new DSVerticalLayoutView(classWizardTitle, TextAlignment.Center, false, new RectOffset(0,0,0,0));

        ConstructorView= new DSCellView(GetConstrcutorDisplaySize, DrawConstructorCell, new RectOffset(0,0,kSpacer,kSpacer), false);
        
        // Initialize table views.
        ClassListController= new iCS_ClassListController();
        ClassListController.View.DisplayRatio= new Vector2(1f, 0.25f);
		ClassVariablesController= new iCS_ClassVariablesController(Target.RuntimeType, Storage, VariableTitle, Target);
		ClassOperationsController= new iCS_ClassOperationsController(Target.RuntimeType, Storage, MethodTitle, Target);
        LayoutView.AddSubview(ClassListController.View);
        LayoutView.AddSubview(ConstructorView);
		LayoutView.AddSubview(ClassVariablesController.View);
		LayoutView.AddSubview(ClassOperationsController.View);
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        // Wait until window is configured.
        if(Target == null) return;
        EditorGUIUtility.LookLikeInspector();
        LayoutView.Display(new Rect(0,0,position.width, position.height));
    }

    // ---------------------------------------------------------------------------------
    void ShowConstructor(Rect headerRect) {
        float y= headerRect.y;
        GUI.Label(new Rect(headerRect.x, y, InstanceTitleSize.x, InstanceTitleSize.y), InstanceTitle, EditorStyles.boldLabel);
        float x= 2f*kSpacer+headerRect.x+InstanceTitleSize.x;
        // Fill-in available constructors.
        string[] instanceOptions= new string[1+Constructors.Length];
        instanceOptions[0]= "Use input port (this)";
        int instanceIdx= -1;
        for(int i= 0; i < Constructors.Length; ++i) {
            instanceOptions[i+1]= Constructors[i].Component.FunctionSignatureNoThisNoOutput;
            if(Constructors[i].IsActive) instanceIdx= i;
        }
        ++instanceIdx;
        float maxWidth= headerRect.width-x;
        float width= Mathf.Min(maxWidth, MaxConstructorWidth);
        int newIdx= EditorGUI.Popup(new Rect(x, y, width, LabelHeight), instanceIdx, instanceOptions, EditorStyles.toolbarPopup);
        if(newIdx != instanceIdx) {
            if(instanceIdx != 0) {
                Constructors[instanceIdx-1].IsActive= false;
                Storage.ClassModuleDestroyConstructor(Target);
            }
            if(newIdx != 0) {
                Constructors[newIdx-1].IsActive= true;
                Storage.ClassModuleCreateConstructor(Target, Constructors[newIdx-1].Component);
            }
        }
    }


    // =================================================================================
    // Helpers
    // ---------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------
    Vector2 GetConstrcutorDisplaySize() {
        float width= LayoutView.BodyArea.width;
        float height= LabelHeight;
        return new Vector2(width, height);
    }
    void DrawConstructorCell(Rect position) {
        ShowConstructor(position);
    }
}
