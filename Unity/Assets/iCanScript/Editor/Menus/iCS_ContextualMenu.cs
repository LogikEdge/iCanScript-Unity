using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ContextualMenu {
    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
	public enum MenuType { SelectedObject, ReleaseAfterDrag };
	
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    Vector2 				GraphPosition  = Vector2.zero;
	List<iCS_MenuContext>	GUICommandQueue= new List<iCS_MenuContext>();
    
    // ======================================================================
    // Menu Items
	// ----------------------------------------------------------------------
    const string ShowHierarchyStr              = "Show in hierarchy";
    const string DeleteStr                     = "- Delete";
    const string PackageStr                    = "+ Package";
    const string StateChartStr                 = "+ State Chart";
    const string StateStr                      = "+ State";
    const string EntryStateStr                 = "+ Entry State";
    const string SetAsEntryStr                 = "Set as Entry";
    const string OnEntryStr                    = "+ "+iCS_Strings.OnEntry;
    const string OnUpdateStr                   = "+ "+iCS_Strings.OnUpdate;
    const string OnExitStr                     = "+ "+iCS_Strings.OnExit;
	const string ObjectInstanceStr             = "+ Object Instance";
    const string EnablePortStr                 = "+ Enable Port";
    const string TriggerPortStr                = "+ Trigger Port";
    const string OutputThisPortStr             = "+ This Port (output)";
	const string WrapInPackageStr              = "+ Wrap in Package";
    const string MultiSelectionWrapInPackageStr= "+ Wrap Multi-Selection in Package";
    const string MultiSelectionDeleteStr       = "- Delete Multi-Selection";
    const string DeleteKeepChildrenStr         = "- Delete Keep Children";
    const string SeparatorStr                  = "";

    // ======================================================================
    // Menu state management
	// ----------------------------------------------------------------------
    void Reset() {
        GraphPosition= Vector2.zero;
    }
    
	// ----------------------------------------------------------------------
	public void OnGUI() {
		GUICommandQueue.ForEach(c=> ProcessMenu(c));
		GUICommandQueue.Clear();
	}
	// ----------------------------------------------------------------------
	// Note: reverseInOut is only applicable for Port :: ReleaseAfterDrag.
    public void Update(MenuType menuType, iCS_EditorObject selectedObject, iCS_IStorage storage, Vector2 graphPosition, bool reverseInOut= false) {
        // Update mouse position if not already done.
        if(GraphPosition == Vector2.zero) GraphPosition= graphPosition;

        // Multi-Selection has its own set of menus.
        if(storage.IsMultiSelectionActive) {
            MultiSelectionMenu(storage);
            return;
        }
        
        // Nothing to show if menu is inactive.
        if(selectedObject == null || GraphPosition != graphPosition) {
            Reset();
            return;
        }
        
        // Process the menu state.
        switch(selectedObject.ObjectType) {
            case iCS_ObjectTypeEnum.Behaviour:         BehaviourMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.StateChart:        StateChartMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.State:             StateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceMessage:   PackageMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassMessage:      PackageMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TransitionPackage: PackageMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Constructor:       FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceFunction:  FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassFunction:     FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TypeCast:          FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceField:     FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassField:        FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceProperty:  FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassProperty:     FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.OnStateEntry:      OnStatePackageMenu(selectedObject); break;
            case iCS_ObjectTypeEnum.OnStateUpdate:     OnStatePackageMenu(selectedObject); break;
            case iCS_ObjectTypeEnum.OnStateExit:       OnStatePackageMenu(selectedObject); break;
            case iCS_ObjectTypeEnum.Package:           if(selectedObject.IsInstanceNode) {
                                                             InstanceMenu(selectedObject, storage);
                                                       } else {
                                                             PackageMenu(selectedObject, storage);
                                                       }
                                                       break;
            default: if(selectedObject.IsPort)         PortMenu(menuType, selectedObject, storage, reverseInOut); break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        // Don't show any menu if behaviour not visible.
        if(selectedObject.IsIconizedOnDisplay || selectedObject.IsFoldedOnDisplay) return;

		var messages= iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour));
        int len= messages.Length;
		iCS_MenuContext[] menu= new iCS_MenuContext[len];
        for(int i= 0; i < len; ++i) {
			var message= messages[i];
            string name= message.DisplayName;
            if(iCS_AllowedChildren.CanAddChildNode(name, message.ObjectType, selectedObject, storage)) {
                menu[i]= new iCS_MenuContext(String.Concat("+ ", name), message);
            } else {
                menu[i]= new iCS_MenuContext(String.Concat("#+ ", name), message);
            }
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void PackageMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            // Base menu items
            menu= new iCS_MenuContext[5];
            menu[0]= new iCS_MenuContext(PackageStr);
            menu[1]= new iCS_MenuContext(StateChartStr);
            menu[2]= new iCS_MenuContext(SeparatorStr);
            menu[3]= new iCS_MenuContext(EnablePortStr);
            if(storage.HasTriggerPort(selectedObject)) {
                menu[4]= new iCS_MenuContext("#"+TriggerPortStr);
            } else {
                menu[4]= new iCS_MenuContext(TriggerPortStr);                
            }
        }
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        if(selectedObject.ObjectType == iCS_ObjectTypeEnum.Package) {
            if(selectedObject.HasChildNode()) {
                int idx= GrowMenuBy(ref menu, 1);
                menu[idx]= new iCS_MenuContext(DeleteKeepChildrenStr);                
            }
        }
		ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void OnStatePackageMenu(iCS_EditorObject targetObject) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!targetObject.IsIconizedInLayout && !targetObject.IsFoldedInLayout) {
            // Base menu items
            menu= new iCS_MenuContext[3];
            menu[0]= new iCS_MenuContext(PackageStr);
            menu[1]= new iCS_MenuContext(StateChartStr);
            menu[2]= new iCS_MenuContext(SeparatorStr);
        }
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
		ShowMenu(menu, targetObject, targetObject.IStorage);
    }
	// ----------------------------------------------------------------------
    void InstanceMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            // Determine if we should support output 'this' port.
            Type classType= selectedObject.RuntimeType;
            bool shouldSupportThis= !iCS_Types.IsStaticClass(classType);
            // Base menu items
            menu= new iCS_MenuContext[shouldSupportThis ? 3 : 2];
            menu[0]= new iCS_MenuContext(EnablePortStr);
            if(storage.HasTriggerPort(selectedObject)) {
                menu[1]= new iCS_MenuContext("#"+TriggerPortStr);
            } else {
                menu[1]= new iCS_MenuContext(TriggerPortStr);                
            }
            if(shouldSupportThis) {
                if(storage.HasOutInstancePort(selectedObject)) {
                    menu[2]= new iCS_MenuContext("#"+OutputThisPortStr);
                } else {
                    menu[2]= new iCS_MenuContext(OutputThisPortStr);                
                }                
            }
        }
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateChartMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            menu= new iCS_MenuContext[1];
            menu[0]= new iCS_MenuContext(StateStr); 
        }
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
        AddShowInHierarchyMenuItem(ref menu);
        if(selectedObject.InstanceId != 0) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu;
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            int len= iCS_AllowedChildren.StateChildNames.Length;
            menu= new iCS_MenuContext[len+4];
            menu[0]= new iCS_MenuContext(StateStr);
            menu[1]= new iCS_MenuContext(SeparatorStr);
            for(int i= 0; i < len; ++i) {
                string name= iCS_AllowedChildren.StateChildNames[i];
                if(iCS_AllowedChildren.CanAddChildNode(name, iCS_ObjectTypeEnum.Package, selectedObject, storage)) {
                    menu[i+2]= new iCS_MenuContext(String.Concat("+ ", name));
                } else {
                    menu[i+2]= new iCS_MenuContext(String.Concat("#+ ", name));
                }
            }
            menu[len+2]= new iCS_MenuContext(SeparatorStr);
            if(selectedObject.IsEntryState) {
                menu[len+3]= new iCS_MenuContext(String.Concat("#", SetAsEntryStr));
            } else {
                menu[len+3]= new iCS_MenuContext(SetAsEntryStr);
            }
        } else {
            menu= new iCS_MenuContext[0];
        }
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
    
	// ----------------------------------------------------------------------
    void FunctionMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu;
        if(!selectedObject.IsIconizedInLayout) {
            // Determine if we should support output 'this' port.
            Type classType= selectedObject.RuntimeType;
            bool shouldSupportThis= !iCS_Types.IsStaticClass(classType);
            // Base menu items
            menu= new iCS_MenuContext[shouldSupportThis ? 3 : 2];
            menu[0]= new iCS_MenuContext(EnablePortStr);
            if(storage.HasTriggerPort(selectedObject)) {
                menu[1]= new iCS_MenuContext("#"+TriggerPortStr);
            } else {
                menu[1]= new iCS_MenuContext(TriggerPortStr);                
            }
            if(shouldSupportThis) {
                if(storage.HasOutInstancePort(selectedObject)) {
                    menu[2]= new iCS_MenuContext("#"+OutputThisPortStr);
                } else {
                    menu[2]= new iCS_MenuContext(OutputThisPortStr);                
                }                
            }
        } else {
            menu= new iCS_MenuContext[0];
        }
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void PortMenu(MenuType menuType, iCS_EditorObject port, iCS_IStorage storage, bool reverseInOut) {
		switch(menuType) {
			case MenuType.SelectedObject:
				SelectedPortMenu(port, storage);
				break;
			case MenuType.ReleaseAfterDrag:
				ReleaseAfterDragPortMenu(port, storage, reverseInOut);
				break;
		}
    }
	// ----------------------------------------------------------------------
    void SelectedPortMenu(iCS_EditorObject port, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[1];
        menu[0]= new iCS_MenuContext(ShowHierarchyStr);
        // Allow to delete a port if its parent is a module.
        if(port.IsStatePort || !port.IsFixDataPort) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, port, storage);            
    }
	// ----------------------------------------------------------------------
    void ReleaseAfterDragPortMenu(iCS_EditorObject port, iCS_IStorage storage, bool reverseInOut) {
		// Different behaviour for "object instance" and standard packages.
        iCS_EditorObject newNodeParent= storage.GetNodeAt(GraphPosition);
		if(newNodeParent == null) return;
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
		if(!newNodeParent.IsInstanceNode) {
			// Add shortcut ti instance node creation.
			menu= new iCS_MenuContext[2];
			menu[0]= new iCS_MenuContext(ObjectInstanceStr);
	        menu[1]= new iCS_MenuContext(SeparatorStr);
		}
        // Get compatible functions.
        if(port.IsDataOrControlPort) {
            List<iCS_MethodBaseInfo> functionMenu= null;
			Type inputType= null;
			Type outputType= null;
			if(port.IsInputPort) {
                outputType= port.RuntimeType;
            } else {
                inputType= port.RuntimeType;
            }
			if(reverseInOut) {
				var tmp= inputType;
				inputType= outputType;
				outputType= tmp;
			}
			if(newNodeParent.IsInstanceNode) {
	            functionMenu= iCS_LibraryDatabase.BuildMenuForMembersOfType(newNodeParent.RuntimeType, inputType, outputType);								
			}
			else {
	            functionMenu= iCS_LibraryDatabase.BuildMenu(inputType, outputType);				
			}
            if(functionMenu.Count != 0) {
                int len= menu.Length;
                iCS_MenuContext[] tmp= null;
                if(len == 0) {
                    tmp= new iCS_MenuContext[functionMenu.Count];
                } else {
                    tmp= new iCS_MenuContext[len+1+functionMenu.Count];
                    menu.CopyTo(tmp, 0);
                    tmp[len]= new iCS_MenuContext(SeparatorStr);
                    ++len;                    
                }
                menu= tmp;                
                for(int i= 0; i < functionMenu.Count; ++i) {
                    menu[len+i]= new iCS_MenuContext(functionMenu[i].ToString(), functionMenu[i]);
                }
            }
        }
        ShowMenu(menu, port, storage);            
    }
	// ----------------------------------------------------------------------
    void MultiSelectionMenu(iCS_IStorage iStorage) {
        var multiSelectedObjects= iStorage.GetMultiSelectedObjects();
        if(multiSelectedObjects == null || multiSelectedObjects.Length == 0) return;
        iCS_MenuContext[] menu= new iCS_MenuContext[3];
        menu[0]= new iCS_MenuContext(MultiSelectionWrapInPackageStr);
        menu[1]= new iCS_MenuContext(SeparatorStr);
        menu[2]= new iCS_MenuContext(MultiSelectionDeleteStr);
        ShowMenu(menu, null, iStorage);
    }
	// ----------------------------------------------------------------------
	void AddWrapInPackageIfAppropriate(ref iCS_MenuContext[] menu, iCS_EditorObject obj) {
		// Don't add if object cannot support a parent package.
		if(!obj.CanHavePackageAsParent()) return;
		if(menu == null || menu.Length == 0) {
			menu= new iCS_MenuContext[1];
			menu[0]= new iCS_MenuContext(WrapInPackageStr);
			return;
		}
		int i= GrowMenuBy(ref menu, 2);
		menu[i]  = new iCS_MenuContext(WrapInPackageStr);
		menu[i+1]= new iCS_MenuContext(SeparatorStr);
	}
	// ----------------------------------------------------------------------
	int GrowMenuBy(ref iCS_MenuContext[] menu, int additionalSize) {
		int sze= menu == null ? 0 : menu.Length;
		if(additionalSize == 0) return sze;
		var newMenu= new iCS_MenuContext[sze+additionalSize];
		menu.CopyTo(newMenu, 0);
		menu= newMenu;
		return sze;
	}
	
    // ======================================================================
    // Menu Utilities
	// ----------------------------------------------------------------------
    void ShowMenu(iCS_MenuContext[] menu, iCS_EditorObject selected, iCS_IStorage storage) {
        int sepCnt= 0;
        GenericMenu gMenu= new GenericMenu();
        foreach(var item in menu) {
            if(item.Command == SeparatorStr) {
                if(sepCnt != 0) gMenu.AddSeparator("");
                sepCnt= 0;
            } else {
                if(item.Command[0] == '#') {
                    string tmp= item.Command.Substring(1);
                    gMenu.AddDisabledItem(new GUIContent(tmp));                                    
                } else {
					item.SelectedObject= selected;
					item.Storage= storage;
					item.GraphPosition= GraphPosition;
                    gMenu.AddItem(new GUIContent(item.Command), false,
                                  c=> GUICommandQueue.Add((iCS_MenuContext)c), item);                                    
                }
                ++sepCnt;
            }
        }
        gMenu.ShowAsContext();
        Reset();
    }
	// ----------------------------------------------------------------------
    void AddDeleteMenuItem(ref iCS_MenuContext[] existingMenu) {
        int idx= ResizeMenu(ref existingMenu, existingMenu.Length+2);
        existingMenu[idx]= new iCS_MenuContext(SeparatorStr);
        existingMenu[idx+1]= new iCS_MenuContext(DeleteStr);
    }
	// ----------------------------------------------------------------------
    void AddShowInHierarchyMenuItem(ref iCS_MenuContext[] existingMenu) {
        int idx= ResizeMenu(ref existingMenu, existingMenu.Length+2);
        existingMenu[idx]= new iCS_MenuContext(SeparatorStr);
        existingMenu[idx+1]= new iCS_MenuContext(ShowHierarchyStr);        
    }
	// ----------------------------------------------------------------------
    int ResizeMenu(ref iCS_MenuContext[] existingMenu, int newSize) {
        int idx= existingMenu.Length;
        if(idx > newSize) idx= newSize;
        iCS_MenuContext[] newMenu= new iCS_MenuContext[newSize];
        existingMenu.CopyTo(newMenu, 0);
        existingMenu= newMenu;
        return idx;
    }
	// ----------------------------------------------------------------------
    iCS_MemberInfo GetReflectionDescFromMenuCommand(iCS_MenuContext menuContext) {
        string menuCommand= iCS_TextUtil.StripBeforeIdent(menuContext.Command);
        return iCS_LibraryDatabase.GetDescriptor(menuCommand);
    }
	// ----------------------------------------------------------------------
    Type GetClassTypeFromMenuCommand(iCS_MenuContext menuContext) {
        string menuCommand= iCS_TextUtil.StripBeforeIdent(menuContext.Command);
        return iCS_LibraryDatabase.GetClassType(menuCommand);
    }
    
    // ======================================================================
    // Menu processing
	// ----------------------------------------------------------------------
    void ProcessMenu(object obj) {
        iCS_MenuContext context= obj as iCS_MenuContext;
        iCS_EditorObject targetObject= context.SelectedObject;
		Vector2 globalPos= context.GraphPosition;
        iCS_IStorage iStorage= context.Storage;
        // Process all other types of requests.
        switch(context.Command) {
            case PackageStr:        iCS_UserCommands.CreatePackage(targetObject, globalPos, null); break;
            case StateChartStr:     iCS_UserCommands.CreateStateChart(targetObject, globalPos, null); break;
            case StateStr:          iCS_UserCommands.CreateState(targetObject, globalPos, null);  break;
            case SetAsEntryStr:     iCS_UserCommands.SetAsStateEntry(targetObject); break;
            case OnEntryStr:        iCS_UserCommands.CreateOnEntryPackage(targetObject, globalPos); break;
            case OnUpdateStr:       iCS_UserCommands.CreateOnUpdatePackage(targetObject, globalPos); break;
            case OnExitStr:         iCS_UserCommands.CreateOnExitPackage(targetObject, globalPos); break;
			case ObjectInstanceStr: CreateObjectInstance(context); break;
            case ShowHierarchyStr:  iCS_UserCommands.ShowInHierarchy(targetObject); break;
            case DeleteStr:         iCS_UserCommands.DeleteObject(targetObject); break;
            case EnablePortStr:     iCS_UserCommands.CreateEnablePort(targetObject); break;
            case TriggerPortStr:    iCS_UserCommands.CreateTriggerPort(targetObject); break;
            case OutputThisPortStr: iCS_UserCommands.CreateOutInstancePort(targetObject); break;
			case WrapInPackageStr:  iCS_UserCommands.WrapInPackage(targetObject); break;
            case MultiSelectionWrapInPackageStr: iCS_UserCommands.WrapMultiSelectionInPackage(iStorage); break;
            case MultiSelectionDeleteStr:        iCS_UserCommands.DeleteMultiSelectedObjects(iStorage); break;
            case DeleteKeepChildrenStr:          iCS_UserCommands.DeleteKeepChildren(targetObject); break;
            default: {
				iCS_MethodBaseInfo desc= context.Descriptor;
				if(desc == null) {
					Debug.LogWarning(iCS_Config.ProductName+": Can find reflection descriptor to create node !!!");
					break;
				}
                if(targetObject == null) break;
                if(desc.IsMessage) {
                    iCS_UserCommands.CreateMessageHandler(targetObject, globalPos, desc as iCS_MessageInfo);
                } else {
					if(targetObject.IsPort) {
	                    CreateAttachedMethod(targetObject, iStorage, globalPos, desc);
					}
					else {
	                    iCS_UserCommands.CreateFunction(targetObject, globalPos, desc);
					}
                }
                break;                
            }
        }
    }


    // ======================================================================
    // High-Level Creation Utilities
    // -------------------------------------------------------------------------
    static void CreateObjectInstance(iCS_MenuContext context) {
		var iStorage= context.Storage;
		var sourcePort = context.SelectedObject;
		var pos= context.GraphPosition;
		var parent= iStorage.GetNodeAt(pos);
		if(parent == null) return;
		var instance= iCS_UserCommands.CreateObjectInstance(parent, pos, sourcePort.RuntimeType);   
		if(sourcePort != null) {
	        var thisPort= iStorage.InstanceWizardGetInputThisPort(instance);
	        iStorage.SetNewDataConnection(thisPort, sourcePort);					
		}
    }

	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateAttachedMethod(iCS_EditorObject port, iCS_IStorage storage, Vector2 globalPos, iCS_MethodBaseInfo desc) {
        iCS_EditorObject newNodeParent= storage.GetNodeAt(globalPos);
		if(newNodeParent == null) return null;
        if(!newNodeParent.IsKindOfPackage || newNodeParent.IsBehaviour) return null;
		iCS_EditorObject method= null;
		if(newNodeParent.IsInstanceNode) {
			method= iCS_UserCommands.CreateInstanceWizardElement(newNodeParent, desc);
		}
		else {
			bool createMethod= true;
			if(desc.IsInstanceFunction || desc.IsInstanceField) {
				if(desc.ClassType != port.RuntimeType) {
					int sel= EditorUtility.DisplayDialogComplex("Missing the Instance Node",
																"The function you selected requires an instance.\nPlease select one of the following:\n1) automatically create the Instance Builder and Instance Wizard;\n2) automatically create the Instance Wizard (I will provide the instance);\n3) just create the function. (I will provide the instance).",
																"Function",
																"Build Instance",
																"Instance Wizard");
					switch(sel) {
					case 0:
						break;
					case 1: {
						method= iCS_UserCommands.CreateInstanceBuilderAndObjectAndElement(newNodeParent, globalPos, desc.ClassType, desc);
						createMethod= false;
						break;
					}
					case 2: {
						method= iCS_UserCommands.CreateInstanceObjectAndElement(newNodeParent, globalPos, desc.ClassType, desc);
						createMethod= false;
						break;
					}}
				}
			}
			if(createMethod) {
                method= iCS_UserCommands.CreateFunction(newNodeParent, globalPos, desc);							
			}
		}

		// Inverse effective data flow if new node is inside port parent.
		bool isInputPort= port.IsInputPort;
		var portParent= port.ParentNode;
		if(portParent.IsParentOf(method)) isInputPort= !isInputPort;
        if(isInputPort) {
			iCS_EditorObject[] outputPorts= Prelude.filter(x=> iCS_Types.IsA(port.RuntimeType, x.RuntimeType), storage.GetChildOutputDataPorts(method)); 
			// Connect if only one possibility.
			if(outputPorts.Length == 1) {
				storage.SetNewDataConnection(port, outputPorts[0]);
			}
			else {
				var bestPort= GetClosestMatch(port, outputPorts);
				if(bestPort != null) {
					storage.SetNewDataConnection(port, bestPort);						
				}
			}
        } else {
			iCS_EditorObject[] inputPorts= Prelude.filter(x=> iCS_Types.IsA(x.RuntimeType, port.RuntimeType), storage.GetChildInputDataPorts(method));
			// Connect if only one posiibility
			if(inputPorts.Length == 1) {
				storage.SetNewDataConnection(inputPorts[0], port);
			}
			// Multiple choices exist so try the one with the closest name.
			else {
				var bestPort= GetClosestMatch(port, inputPorts);
				if(bestPort != null) {
					storage.SetNewDataConnection(bestPort, port);											
				}
			}
        }
        return method;
    }
    
    
    // ======================================================================
    // Utilities
	// ----------------------------------------------------------------------
	static iCS_EditorObject GetClosestMatch(iCS_EditorObject refPort, iCS_EditorObject[] otherPorts) {
		if(otherPorts.Length == 0) return null;
		float bestScore= 0f;
		iCS_EditorObject bestPort= null;
		string refName= refPort.Name;
		if(string.IsNullOrEmpty(refPort.RawName)) refName= refPort.ParentNode.Name;
		foreach(var p in otherPorts) {
			string pName= p.Name;
			if(string.IsNullOrEmpty(p.RawName)) pName= p.ParentNode.Name;
			var score= iCS_StringUtil.FuzzyCompare(refName, pName);
			if(score > bestScore) {
				bestScore= score;
				bestPort= p;
			}
		}
		return bestPort;		
	}
}
