using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : EditorWindow, DSTableViewDataSource {
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
    class VariablePair {
        public ControlPair InputControlPair= null;
        public ControlPair OutputControlPair= null;
        public VariablePair(iCS_ReflectionDesc inputComponent, bool inputActive, iCS_ReflectionDesc outputComponent, bool outputActive) {
            InputControlPair= new ControlPair(inputComponent, inputActive);
            OutputControlPair= new ControlPair(outputComponent, outputActive);
        }
    };
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject        Target                = null;
    iCS_IStorage            Storage               = null;
    Type                    ClassType             = null;
    ControlPair[]           Methods               = null;
    ControlPair[]           Constructors          = null;
    DSCellView              ConstructorView       = null;
    DSTableView             OperationTableView    = null;
    
    iCS_ClassListController     	ClassListController     = null;
	iCS_ClassVariablesController	ClassVariablesController= null;
    DSVerticalLayoutView        	LayoutView              = null;

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
    int NbOfMethods   { get { return Methods.Length; }}
    
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
        List<ControlPair> methods= new List<ControlPair>();
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(ClassType);
        foreach(var component in components) {
            bool isActive= Storage.ClassModuleFindFunction(Target, component) != null;
            if(component.IsField) {
            } else if(component.IsProperty) {
            } else if(component.IsConstructor) {
                isActive= false;
                iCS_EditorObject existing= Storage.ClassModuleGetConstructor(Target);
                if(existing != null && component.Method == existing.GetMethodBase(Storage.EditorObjects)) {
                    isActive= true;
                }
                constructors.Add(new ControlPair(component, isActive));
                var constructorSize= EditorStyles.boldLabel.CalcSize(new GUIContent(component.FunctionSignatureNoThisNoOutput));
                if(constructorSize.x+12f > MaxConstructorWidth) {
                    MaxConstructorWidth= constructorSize.x+12f;
                }
            } else {
                methods.Add(new ControlPair(component, isActive));
            }
        }
        Constructors= constructors.ToArray();
    	Array.Sort(Constructors, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));        
        Methods= methods.ToArray();
    	Array.Sort(Methods, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));
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
        OperationTableView= new DSTableView(MethodTitle, TextAlignment.Center, false, new RectOffset(kSpacer,kSpacer,0,kSpacer));
		OperationTableView.DataSource= this;
		DSTableColumn operationColumn= new DSTableColumn(kOperationColumnId, null, TextAlignment.Left, false, new RectOffset(0,0,0,0));
		OperationTableView.AddSubview(operationColumn);
		
        ClassListController= new iCS_ClassListController();
        ClassListController.View.DisplayRatio= new Vector2(1f, 0.25f);
		ClassVariablesController= new iCS_ClassVariablesController(Target.RuntimeType, Storage, VariableTitle, Target);
        LayoutView.AddSubview(ClassListController.View);
        LayoutView.AddSubview(ConstructorView);
		LayoutView.AddSubview(ClassVariablesController.View);
		LayoutView.AddSubview(OperationTableView);
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
    iCS_ReflectionDesc GetAComponent(VariablePair pair) {
        return pair.InputControlPair.Component ?? pair.OutputControlPair.Component; 
    }
    VariablePair GetVariablePair(string name, List<VariablePair> lst) {
        foreach(var pair in lst) {
            iCS_ReflectionDesc inputComponent= pair.InputControlPair.Component;
            if(inputComponent != null) {
                if(inputComponent.IsField) {
                    if(inputComponent.FieldName == name) return pair;
                } else {
                    if(inputComponent.PropertyName == name) return pair;
                }
            }
            iCS_ReflectionDesc outputComponent= pair.OutputControlPair.Component;
            if(outputComponent != null) {
                if(outputComponent.IsField) {
                    if(outputComponent.FieldName == name) return pair;
                } else {
                    if(outputComponent.PropertyName == name) return pair;
                }                
            }
        }
        return null;
    }

    // =================================================================================
    // TableViewDataSource
    // ---------------------------------------------------------------------------------
    public int NumberOfRowsInTableView(DSTableView tableView) {
        if(tableView == OperationTableView) {
            return NbOfMethods;
        }
        return 0;
    }
    public Vector2 DisplaySizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
        if(tableView == OperationTableView) {
            var signatureSize= EditorStyles.boldLabel.CalcSize(new GUIContent(Methods[row].Component.FunctionSignatureNoThis));
			signatureSize.x+= 12f;
			return signatureSize;
        }
        return Vector2.zero;
    }
    public void DisplayObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row, Rect position) {
        if(tableView == OperationTableView) {
	        GUIStyle style= GUI.skin.button;
	        var alignment= style.alignment;
	        var fontStyle= style.fontStyle;
	        var textColor= style.normal.textColor;        
	        var background= style.normal.background;
	        style.alignment= TextAnchor.MiddleLeft;
	        if(Methods[row].IsActive) {
	            style.normal.textColor= Color.white;
	            style.fontStyle= FontStyle.Bold;
	            style.normal.background= style.active.background;            
	        } else {
	            style.fontStyle= FontStyle.Italic;
	        }
			position.width= tableColumn.DataSize.x;
	        if(GUI.Button(position, Methods[row].Component.FunctionSignatureNoThis)) {
	            Methods[row].IsActive^= true;
	            if(Methods[row].IsActive) {
	                Storage.ClassModuleCreate(Target, Methods[row].Component);
	            } else {
	                Storage.ClassModuleDestroy(Target, Methods[row].Component);
	            }
	        }
	        style.normal.textColor= textColor;
	        style.normal.background= background;
	        style.fontStyle= fontStyle;
	        style.alignment= alignment;
        }        
    }
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
