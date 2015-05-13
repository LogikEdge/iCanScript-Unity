using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    public class iCS_ClassVariablesController : DSTableViewDataSource {
        // =================================================================================
        // Types
        // ---------------------------------------------------------------------------------
        class ControlPair {
            public LibraryObject    Component= null;
            public bool             IsActive= false;
            public ControlPair(LibraryObject libraryObject, bool isActive= false) {
                Component= libraryObject;
                IsActive= isActive;
            }
        };
        class VariablePair {
            public ControlPair InputControlPair= null;
            public ControlPair OutputControlPair= null;
            public VariablePair(LibraryObject inputComponent, bool inputActive, LibraryObject outputComponent, bool outputActive) {
                InputControlPair = new ControlPair(inputComponent, inputActive);
                OutputControlPair= new ControlPair(outputComponent, outputActive);
            }
    		public ControlPair GetAControlPair {
    			get {
    				if(InputControlPair != null && InputControlPair.Component != null) {
    					return InputControlPair;
    				}
    				return OutputControlPair;
    			}
    		}
    		public string RawName {
    			get {
                    return GetVariableName(GetAControlPair.Component);
    			}
    		}
    		public string DisplayName {
    			get { return NameUtility.ToDisplayName(RawName); }
    		}
    		public Type VariableType {
    			get {
    				return GetVariableType(GetAControlPair.Component);
    			}
    		}
    		public string DisplayTypeName {
    			get {
    				return NameUtility.ToDisplayName(VariableType.Name);
    			}
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
            var libraryDatabase= LibraryController.LibraryDatabase;
            var libraryType= libraryDatabase.GetLibraryType(classType);
            var libraryFields    = libraryType.GetMembers<LibraryField>();
            var libraryProperties= libraryType.GetMembers<LibraryProperty>();
            var libraryFieldsAndProperties= P.append<LibraryObject>(libraryFields, libraryProperties);
            List<VariablePair> variables= new List<VariablePair>();
            foreach(var libraryObject in libraryFieldsAndProperties) {
                bool isActive= (myTarget != null && myStorage != null) ? myStorage.PropertiesWizardFindFunction(myTarget, libraryObject) != null : false;
                string name= GetVariableName(libraryObject);
                var variablePair= GetVariablePair(name, variables);
                if(libraryObject is LibraryFieldSetter || libraryObject is LibraryPropertySetter) {
                    if(variablePair != null) {
                        variablePair.InputControlPair.Component= libraryObject;
                        variablePair.InputControlPair.IsActive= isActive;
                    } else {
                        variables.Add(new VariablePair(libraryObject, isActive, null, false));                        
                    }
                } else {
                    if(variablePair != null) {
                        variablePair.OutputControlPair.Component= libraryObject;
                        variablePair.OutputControlPair.IsActive= isActive;
                    } else {
                        variables.Add(new VariablePair(null, false, libraryObject, isActive));                                            
                    }
                }
            }
            myVariables= variables.ToArray();
        	Array.Sort(myVariables, (x,y)=> GetVariableName(x).CompareTo(GetVariableName(y)));

    		// Build view
            myTableView= new DSTableView(new RectOffset(0,0,0,0), true, myTitle, DSView.AnchorEnum.Center, true, true);
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
        static string GetVariableName(LibraryObject libraryObject) {
            var rawName= libraryObject.rawName;
            if(rawName.StartsWith("set_") || rawName.StartsWith("get_")) {
                rawName= rawName.Substring(4);
            }
    		return NameUtility.ToDisplayName(rawName);
        }
    	static string GetVariableName(VariablePair pair) {
    		return pair.DisplayName;
    	}
    	static string GetTypeName(LibraryObject libraryObject) {
            var type= GetVariableType(libraryObject);
            if(type == null) return null;
            return NameUtility.ToDisplayName(iCS_Types.TypeName(type));
    	}
    	static string GetTypeName(VariablePair pair) {
    		return GetTypeName(GetAComponent(pair));
    	}
        static Type GetVariableType(LibraryObject libraryObject) {
            var libraryField= libraryObject as LibraryField;
            if(libraryField != null) {
                return libraryField.fieldType;
            }
            var libraryGetProperty= libraryObject as LibraryPropertyGetter;
            if(libraryGetProperty != null) {
                return libraryGetProperty.returnType;
            }
            var librarySetProperty= libraryObject as LibraryPropertySetter;
            if(librarySetProperty != null) {
                return librarySetProperty.parameters[0].ParameterType;
            }
            return null;
        }
    	static Type GetVariableType(VariablePair pair) {
    		return GetVariableType(GetAComponent(pair));
    	}
        static LibraryObject GetAComponent(VariablePair pair) {
            return pair.InputControlPair.Component ?? pair.OutputControlPair.Component; 
        }
        VariablePair GetVariablePair(string name, List<VariablePair> lst) {
            foreach(var pair in lst) {
    			if(pair.DisplayName == name) return pair;
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
        public Vector2 LayoutSizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
    		if(myClassType == null) return Vector2.zero;
            string columnId= tableColumn.Identifier;
            if(string.Compare(columnId, kInColumnId) == 0 || string.Compare(columnId, kOutColumnId) == 0) {
                return myCheckBoxSize;
            }
            VariablePair variablePair= myVariables[row];
            string name= variablePair.DisplayName;
            string typeName= variablePair.DisplayTypeName;
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
            string name= variablePair.DisplayName;
            string typeName= variablePair.DisplayTypeName;
            ControlPair inputControlPair= variablePair.InputControlPair;
            ControlPair outputControlPair= variablePair.OutputControlPair;

            string columnId= tableColumn.Identifier;
            if(string.Compare(columnId, kInColumnId) == 0 && inputControlPair.Component != null) {
    			if(Math3D.IsEqual(position.height, myCheckBoxSize.y) && Math3D.IsEqual(position.width, myCheckBoxSize.x)) {
                    bool prevActive= inputControlPair.IsActive;
                    inputControlPair.IsActive= GUI.Toggle(position, inputControlPair.IsActive, "");
                    if(prevActive != inputControlPair.IsActive && myTarget != null && myStorage != null) {
                        if(inputControlPair.IsActive) {
                            iCS_UserCommands.CreatePropertiesWizardElement(myTarget, inputControlPair.Component);
                        } else {
                            iCS_UserCommands.DeletePropertiesWizardElement(myTarget, inputControlPair.Component);
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
                            iCS_UserCommands.CreatePropertiesWizardElement(myTarget, outputControlPair.Component);
                        } else {
                            iCS_UserCommands.DeletePropertiesWizardElement(myTarget, outputControlPair.Component);
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
        // ---------------------------------------------------------------------------------
    	public void OnMouseDown(DSTableView tableView, DSTableColumn tableColumn, int row) {}
	
    }

}
