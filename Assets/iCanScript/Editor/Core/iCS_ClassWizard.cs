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
    VariablePair[]          Fields                = null;
    VariablePair[]          Properties            = null;
    ControlPair[]           Methods               = null;
    ControlPair[]           Constructors          = null;
    DSViewWithTitle         ClassWizardView       = null;
    DSView                  ConstructorView       = null;
    DSTableView             VariableTableView     = null;
    DSTableView             OperationTableView    = null;
    
    iCS_ClassListController ClassListController= new iCS_ClassListController();

    // =================================================================================
    // Layout info.
    // ---------------------------------------------------------------------------------
    float       MaxConstructorWidth= 0f;

    // =================================================================================
    // Constant GUI Content
    // ---------------------------------------------------------------------------------
    bool        IsGUIConstantInit= false;
    GUIContent  InstanceTitle= new GUIContent("Instance");
    GUIContent  VariableTitle= new GUIContent("Variables");
    GUIContent  MethodTitle  = new GUIContent("Operations");
    Vector2     LabelSize;
    Vector2     InstanceTitleSize;
    Vector2     CheckBoxSize;    
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
    int NbOfVariables { get { return Fields.Length + Properties.Length; }}
    int NbOfMethods   { get { return Methods.Length; }}
    
    // ---------------------------------------------------------------------------------
    void Init() {
        Target= null;
        Storage= null;
        IsGUIConstantInit= false;
    }
    // ---------------------------------------------------------------------------------
    public void Activate(iCS_EditorObject target, iCS_IStorage storage) {
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
        if(ClassWizardView != null) ClassWizardView.Title= new GUIContent(Target.Name);
        // Build class data.
        ClassType= target.RuntimeType;
        List<VariablePair> fields= new List<VariablePair>();
        List<VariablePair> properties= new List<VariablePair>();
        List<ControlPair> constructors= new List<ControlPair>();
        List<ControlPair> methods= new List<ControlPair>();
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(ClassType);
        foreach(var component in components) {
            bool isActive= Storage.ClassModuleFindFunction(Target, component) != null;
            if(component.IsField) {
                string name= component.FieldName;
                var variablePair= GetVariablePair(name, fields);
                if(component.IsSetField) {
                    if(variablePair != null) {
                        variablePair.InputControlPair.Component= component;
                        variablePair.InputControlPair.IsActive= isActive;
                    } else {
                        fields.Add(new VariablePair(component, isActive, null, false));                        
                    }
                } else {
                    if(variablePair != null) {
                        variablePair.OutputControlPair.Component= component;
                        variablePair.OutputControlPair.IsActive= isActive;
                    } else {
                        fields.Add(new VariablePair(null, false, component, isActive));                                            
                    }
                }
            } else if(component.IsProperty) {
                string name= component.PropertyName;
                var variablePair= GetVariablePair(name, properties);
                if(component.IsSetProperty) {
                    if(variablePair != null) {
                        variablePair.InputControlPair.Component= component;
                        variablePair.InputControlPair.IsActive= isActive;
                    } else {
                        properties.Add(new VariablePair(component, isActive, null, false));                        
                    }
                } else {
                    if(variablePair != null) {
                        variablePair.OutputControlPair.Component= component;
                        variablePair.OutputControlPair.IsActive= isActive;
                    } else {
                        properties.Add(new VariablePair(null, false, component, isActive));                        
                    }
                }
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
        Fields= fields.ToArray();
    	Array.Sort(Fields, (x,y)=> GetAComponent(x).FieldName.CompareTo(GetAComponent(y).FieldName));
        Properties= properties.ToArray();
    	Array.Sort(Properties, (x,y)=> GetAComponent(x).PropertyName.CompareTo(GetAComponent(y).PropertyName));
        Constructors= constructors.ToArray();
    	Array.Sort(Constructors, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));        
        Methods= methods.ToArray();
    	Array.Sort(Methods, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));
        Target= target;
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
        if(IsGUIConstantInit) return;
        IsGUIConstantInit= true;

        // Compute content size.
        LabelSize        = EditorStyles.label.CalcSize(new GUIContent("abc")); 
        InstanceTitleSize= EditorStyles.boldLabel.CalcSize(InstanceTitle);
                         
        LabelHeight      = 4f+LabelSize.y;
                         
        CheckBoxSize     = GUI.skin.toggle.CalcSize(new GUIContent(""));                

        // Initialize class wizard view.
        string classTitle= Target != null ? Target.Name : "Class Wizard";
        GUIContent classWizardTitle= new GUIContent(classTitle);
        ClassWizardView= new DSViewWithTitle(classWizardTitle, TextAlignment.Center, false,
                                             new RectOffset(kMarginSize, kMarginSize, 0, kMarginSize), false);

        ConstructorView= new DSView(new RectOffset(0,0,kSpacer,kSpacer), false);
        
        // Initialize table views.
        
        VariableTableView= new DSTableView(VariableTitle, TextAlignment.Center, false, new RectOffset(kSpacer,kSpacer,0,kSpacer));
        VariableTableView.DataSource= this;
        DSTableColumn inColumn= new DSTableColumn(kInColumnId, new GUIContent("In"), TextAlignment.Center, true,
                                                  new RectOffset(kSpacer,kSpacer,0,0));
        VariableTableView.AddSubview(inColumn);
        DSTableColumn outColumn= new DSTableColumn(kOutColumnId, new GUIContent("Out"), TextAlignment.Center, true,
                                                   new RectOffset(kSpacer,kSpacer,0,0));
        VariableTableView.AddSubview(outColumn);
        DSTableColumn variableNameColumn= new DSTableColumn(kNameColumnId, new GUIContent("Name"), TextAlignment.Left, true,
                                                            new RectOffset(kSpacer,kSpacer,0,0));
        VariableTableView.AddSubview(variableNameColumn);
        DSTableColumn variableTypeColumn= new DSTableColumn(kTypeColumnId, new GUIContent("Type"), TextAlignment.Left, true,
                                                            new RectOffset(kSpacer,kSpacer,0,0));
        VariableTableView.AddSubview(variableTypeColumn);

        OperationTableView= new DSTableView(MethodTitle, TextAlignment.Center, false, new RectOffset(kSpacer,kSpacer,0,kSpacer));
		OperationTableView.DataSource= this;
		DSTableColumn operationColumn= new DSTableColumn(kOperationColumnId, null, TextAlignment.Left, false, new RectOffset(0,0,0,0));
		OperationTableView.AddSubview(operationColumn);
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        // Wait until window is configured.
        if(Target == null) return;
        InitConstantGUIContent();
        EditorGUIUtility.LookLikeInspector();

        // Display Header.
        ClassWizardView.Display(new Rect(0,0,position.width, position.height));
        
        // Compute window parameters.
            Rect headerRect= ClassWizardView.BodyArea;
            headerRect.height= ConstructorView.Margins.vertical+LabelHeight;
        ConstructorView.Display(headerRect);
        ShowConstructor(ConstructorView.DisplayArea);
            Rect remainingArea= ClassWizardView.BodyArea;
            remainingArea.y+= ConstructorView.FrameArea.height;
            remainingArea.height-= ConstructorView.FrameArea.height;
            float variableHeight= VariableTableView.FullFrameSize.y;
            float operationHeight= OperationTableView.FullFrameSize.y;
            float neededHeight= variableHeight+operationHeight+kMarginSize;
            float remainingHeight= remainingArea.height;
            if(VariableTableView.HasHorizontalScroller && (variableHeight < 0.5f*remainingHeight || neededHeight < remainingHeight)) {
                variableHeight= VariableTableView.FullFrameSizeWithScrollers.y;
                neededHeight= variableHeight+operationHeight+kMarginSize;
            }
            if(OperationTableView.HasHorizontalScroller && (operationHeight < 0.5f*remainingHeight || neededHeight < remainingHeight)) {
                operationHeight= OperationTableView.FullFrameSizeWithScrollers.y;
                neededHeight= variableHeight+operationHeight+kMarginSize;
            }
            if(neededHeight > remainingHeight) {
                if(variableHeight < 0.5f*remainingHeight) {
                    operationHeight= remainingHeight-variableHeight;
                } else if(operationHeight < 0.5f*remainingHeight) {
                    variableHeight= remainingHeight-operationHeight;
                } else {
                    variableHeight= (variableHeight/neededHeight)*remainingHeight;
                    operationHeight= remainingHeight- variableHeight;
                }
            }
            Rect boxVariableRect= remainingArea;
            boxVariableRect.height= variableHeight;
            Rect boxMethodRect= remainingArea;
            boxMethodRect.y+= variableHeight+kMarginSize;
            boxMethodRect.height= operationHeight;
            
        // Display Variables.
//        VariableTableView.Display(boxVariableRect);
        ClassListController.Display(boxVariableRect);

        // Display Methods.
        OperationTableView.Display(boxMethodRect);
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
        if(tableView == VariableTableView) {
            return NbOfVariables;
        }
        if(tableView == OperationTableView) {
            return NbOfMethods;
        }
        return 0;
    }
    public Vector2 DisplaySizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
        if(tableView == VariableTableView) {
            string columnId= tableColumn.Identifier;
            if(string.Compare(columnId, kInColumnId) == 0 || string.Compare(columnId, kOutColumnId) == 0) {
                return CheckBoxSize;
            }
            string name;
            string typeName;
            VariablePair variablePair;
            if(row < Fields.Length) {
                variablePair= Fields[row];
                var field= GetAComponent(variablePair);
                name= field.FieldName;
                typeName= iCS_Types.TypeName(field.FieldType);
            } else {
                variablePair= Properties[row-Fields.Length];
                var property= GetAComponent(variablePair);
                name= property.PropertyName;
                typeName= iCS_Types.TypeName(property.PropertyType);
            }
            ControlPair inputControlPair= variablePair.InputControlPair;
            ControlPair outputControlPair= variablePair.OutputControlPair;
            GUIStyle labelStyle= inputControlPair.IsActive || outputControlPair.IsActive ? EditorStyles.boldLabel : EditorStyles.label;
            if(string.Compare(columnId, kNameColumnId) == 0) {
                return labelStyle.CalcSize(new GUIContent(name));
            }
            if(string.Compare(columnId, kTypeColumnId) == 0) {
                return labelStyle.CalcSize(new GUIContent(typeName));                
            }
            return Vector2.zero;
        }
        if(tableView == OperationTableView) {
            var signatureSize= EditorStyles.boldLabel.CalcSize(new GUIContent(Methods[row].Component.FunctionSignatureNoThis));
			signatureSize.x+= 12f;
			return signatureSize;
        }
        return Vector2.zero;
    }
    public void DisplayObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row, Rect position) {
        if(tableView == VariableTableView) {
            string name;
            string typeName;
            VariablePair variablePair;
            if(row < Fields.Length) {
                variablePair= Fields[row];
                var field= GetAComponent(variablePair);
                name= field.FieldName;
                typeName= iCS_Types.TypeName(field.FieldType);
            } else {
                variablePair= Properties[row-Fields.Length];
                var property= GetAComponent(variablePair);
                name= property.PropertyName;
                typeName= iCS_Types.TypeName(property.PropertyType);
            }
            ControlPair inputControlPair= variablePair.InputControlPair;
            ControlPair outputControlPair= variablePair.OutputControlPair;

            string columnId= tableColumn.Identifier;
            if(string.Compare(columnId, kInColumnId) == 0 && inputControlPair.Component != null) {
				if(Math3D.IsEqual(position.height, CheckBoxSize.y) && Math3D.IsEqual(position.width, CheckBoxSize.x)) {
	                bool prevActive= inputControlPair.IsActive;
	                inputControlPair.IsActive= GUI.Toggle(position, inputControlPair.IsActive, "");
	                if(prevActive != inputControlPair.IsActive) {
	                    if(inputControlPair.IsActive) {
	                        Storage.ClassModuleCreate(Target, inputControlPair.Component);
	                    } else {
	                        Storage.ClassModuleDestroy(Target, inputControlPair.Component);
	                    }                
	                }                					
				}
            }
            if(string.Compare(columnId, kOutColumnId) == 0 && outputControlPair.Component != null) {
				if(Math3D.IsEqual(position.height, CheckBoxSize.y) && Math3D.IsEqual(position.width, CheckBoxSize.x)) {
	                bool prevActive= outputControlPair.IsActive;
	                outputControlPair.IsActive= GUI.Toggle(position, outputControlPair.IsActive, "");
	                if(prevActive != outputControlPair.IsActive) {
	                    if(outputControlPair.IsActive) {
	                        Storage.ClassModuleCreate(Target, outputControlPair.Component);
	                    } else {
	                        Storage.ClassModuleDestroy(Target, outputControlPair.Component);
	                    }                
					}
                }                
            }
            GUIStyle labelStyle= inputControlPair.IsActive || outputControlPair.IsActive ? EditorStyles.boldLabel : EditorStyles.label;
            if(string.Compare(columnId, kNameColumnId) == 0) {
                GUI.Label(position, name, labelStyle);                
            }
            if(string.Compare(columnId, kTypeColumnId) == 0) {
                GUI.Label(position, typeName, labelStyle);                                
            }
        }
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
}
