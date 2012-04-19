using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassVariablesController : DSTableViewDataSource {
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
	Type				myClassType= null;
	iCS_EditorObject	myTarget   = null;
	iCS_IStorage		myStorage  = null;
    GUIContent    		myTitle    = null;
    DSTableView			myTableView= null;
	VariablePair[]		myVariables= null;
	Vector2     		myCheckBoxSize;    
    
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int     kSpacer       = 8;
    const int     kMarginSize   = 10;
    const float   kCheckBoxWidth= 25f;
	const string  kDefaultTitle = "Variables";
    const string  kInColumnId   = "In";
    const string  kOutColumnId  = "Out";
    const string  kNameColumnId = "Name";
    const string  kTypeColumnId = "Type";

    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public DSView View { get { return myTableView; }}
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
	public iCS_ClassVariablesController() {}
    public iCS_ClassVariablesController(Type classType, iCS_IStorage storage, GUIContent title= null, iCS_EditorObject target= null) {
		OnActivate(classType, storage, title, target);
	}
	public void OnActivate(Type classType, iCS_IStorage storage, GUIContent title= null, iCS_EditorObject target= null) {
		// Configuration parameters.
		myClassType= classType;
		myStorage= storage;
		myTitle= title ?? new GUIContent(kDefaultTitle);
		myTarget= target;
		
		// Common variables.
		myCheckBoxSize     = GUI.skin.toggle.CalcSize(new GUIContent(""));                

		// Extract fields & properties from class descriptor.
        List<VariablePair> variables= new List<VariablePair>();
        foreach(var component in iCS_DataBase.GetClassVariables(myClassType)) {
            bool isActive= (myTarget != null && myStorage != null) ? myStorage.ClassModuleFindFunction(myTarget, component) != null : false;
            string name= component.VariableName;
            var variablePair= GetVariablePair(name, variables);
            if(component.IsSetField || component.IsSetProperty) {
                if(variablePair != null) {
                    variablePair.InputControlPair.Component= component;
                    variablePair.InputControlPair.IsActive= isActive;
                } else {
                    variables.Add(new VariablePair(component, isActive, null, false));                        
                }
            } else {
                if(variablePair != null) {
                    variablePair.OutputControlPair.Component= component;
                    variablePair.OutputControlPair.IsActive= isActive;
                } else {
                    variables.Add(new VariablePair(null, false, component, isActive));                                            
                }
            }
        }
        myVariables= variables.ToArray();
    	Array.Sort(myVariables, (x,y)=> GetVariableName(x).CompareTo(GetVariableName(y)));

		// Build view
        myTableView= new DSTableView(new RectOffset(kSpacer,kSpacer,0,kSpacer), true, myTitle, DSView.AnchorEnum.Center, true);
        myTableView.DataSource= this;
        DSTableColumn inColumn= new DSTableColumn(kInColumnId, new RectOffset(kSpacer,kSpacer,0,0), new GUIContent("In"), DSView.AnchorEnum.Center);
        myTableView.AddColumn(inColumn);
        DSTableColumn outColumn= new DSTableColumn(kOutColumnId, new RectOffset(kSpacer,kSpacer,0,0), new GUIContent("Out"), DSView.AnchorEnum.Center);
        myTableView.AddColumn(outColumn);
        DSTableColumn variableNameColumn= new DSTableColumn(kNameColumnId, new RectOffset(kSpacer,kSpacer,0,0), new GUIContent("Name"), DSView.AnchorEnum.CenterLeft);
        myTableView.AddColumn(variableNameColumn);
        DSTableColumn variableTypeColumn= new DSTableColumn(kTypeColumnId, new RectOffset(kSpacer,kSpacer,0,0), new GUIContent("Type"), DSView.AnchorEnum.CenterLeft);
        myTableView.AddColumn(variableTypeColumn);
    }

    // =================================================================================
    // Helpers
    // ---------------------------------------------------------------------------------
	string GetVariableName(VariablePair pair) {
		return GetAComponent(pair).VariableName;
	}
	Type GetVariableType(VariablePair pair) {
		return GetAComponent(pair).VariableType;
	}
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
		return myClassType != null ? myVariables.Length : 0;
    }
    // ---------------------------------------------------------------------------------
    public Vector2 DisplaySizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
		if(myClassType == null) return Vector2.zero;
        string columnId= tableColumn.Identifier;
        if(string.Compare(columnId, kInColumnId) == 0 || string.Compare(columnId, kOutColumnId) == 0) {
            return myCheckBoxSize;
        }
        VariablePair variablePair= myVariables[row];
        string name= GetVariableName(variablePair);
        string typeName= iCS_Types.TypeName(GetVariableType(myVariables[row]));
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
    // ---------------------------------------------------------------------------------
    public void DisplayObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row, Rect position) {
		if(myClassType == null) return;
        VariablePair variablePair= myVariables[row];
        string name= GetVariableName(variablePair);
        string typeName= iCS_Types.TypeName(GetVariableType(myVariables[row]));
        ControlPair inputControlPair= variablePair.InputControlPair;
        ControlPair outputControlPair= variablePair.OutputControlPair;

        string columnId= tableColumn.Identifier;
        if(string.Compare(columnId, kInColumnId) == 0 && inputControlPair.Component != null) {
			if(Math3D.IsEqual(position.height, myCheckBoxSize.y) && Math3D.IsEqual(position.width, myCheckBoxSize.x)) {
                bool prevActive= inputControlPair.IsActive;
                inputControlPair.IsActive= GUI.Toggle(position, inputControlPair.IsActive, "");
                if(prevActive != inputControlPair.IsActive && myTarget != null && myStorage != null) {
                    if(inputControlPair.IsActive) {
                        myStorage.ClassModuleCreate(myTarget, inputControlPair.Component);
                    } else {
                        myStorage.ClassModuleDestroy(myTarget, inputControlPair.Component);
                    }                
                }                					
			}
        }
        if(string.Compare(columnId, kOutColumnId) == 0 && outputControlPair.Component != null) {
			if(Math3D.IsEqual(position.height, myCheckBoxSize.y) && Math3D.IsEqual(position.width, myCheckBoxSize.x)) {
                bool prevActive= outputControlPair.IsActive;
                outputControlPair.IsActive= GUI.Toggle(position, outputControlPair.IsActive, "");
                if(prevActive != outputControlPair.IsActive && myTarget != null && myStorage != null) {
                    if(outputControlPair.IsActive) {
                        myStorage.ClassModuleCreate(myTarget, outputControlPair.Component);
                    } else {
                        myStorage.ClassModuleDestroy(myTarget, outputControlPair.Component);
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

}
