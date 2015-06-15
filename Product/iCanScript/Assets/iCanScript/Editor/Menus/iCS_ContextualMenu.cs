using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
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
    	// Canvas Menu
    	const string VariableCreationStr		   = "#+ Create a Variable";
        const string FunctionCreationStr           = "+ Create a Function";
    	const string AddUnityEventStr			   = "Add a Unity Event/";
    	const string NestedTypeCreationStr		   = "#+ Create a Nested Type";
	
        const string ShowHierarchyStr              = "Show in hierarchy";
        const string SetAsDisplayRootStr           = "Work on Node in Isolation";
        const string ClearNavigationHistoryStr     = "Back to Full View";
        const string DeleteStr                     = "- Delete";
        const string PackageStr                    = "+ Create a Package";
        const string InlineCodeStr                 = "+ Create Inline Code";
        const string StateChartStr                 = "+ Create a State Chart";
        const string StateStr                      = "+ Create a State";
        const string EntryStateStr                 = "+ Create an Entry State";
        const string SetAsEntryStr                 = "Set as Entry";
        const string OnEntryStr                    = "+ "+iCS_Strings.OnEntry;
        const string OnUpdateStr                   = "+ "+iCS_Strings.OnUpdate;
        const string OnExitStr                     = "+ "+iCS_Strings.OnExit;
    	const string ObjectInstanceStr             = "+ Instance Node";
        const string EnablePortStr                 = "+ Add Enable Port";
        const string TriggerPortStr                = "+ Add Trigger Port";
    	const string WrapInPackageStr              = "+ Wrap in Package";
        const string MultiSelectionWrapInPackageStr= "+ Wrap Multi-Selection in Package";
        const string MultiSelectionDeleteStr       = "- Delete Multi-Selection";
        const string UnwrapPackageStr              = "- Unwrap Package";
        const string AddNodeStr                    = "+ Add Node";  
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
                case VSObjectType.Behaviour:         CanvasMenu(selectedObject, storage); break;
                case VSObjectType.StateChart:        StateChartMenu(selectedObject, storage); break;
                case VSObjectType.State:             StateMenu(selectedObject, storage); break;
                case VSObjectType.InstanceMessage:   MessageHandlerMenu(selectedObject, storage); break;
                case VSObjectType.StaticMessage:     PackageMenu(selectedObject, storage); break;
                case VSObjectType.TransitionPackage: PackageMenu(selectedObject, storage); break;
                case VSObjectType.Constructor:       FunctionMenu(selectedObject, storage); break;
                case VSObjectType.NonStaticFunction: FunctionMenu(selectedObject, storage); break;
                case VSObjectType.StaticFunction:    FunctionMenu(selectedObject, storage); break;
                case VSObjectType.TypeCast:          FunctionMenu(selectedObject, storage); break;
                case VSObjectType.NonStaticField:    FunctionMenu(selectedObject, storage); break;
                case VSObjectType.StaticField:       FunctionMenu(selectedObject, storage); break;
                case VSObjectType.NonStaticProperty: FunctionMenu(selectedObject, storage); break;
                case VSObjectType.StaticProperty:    FunctionMenu(selectedObject, storage); break;
                case VSObjectType.InlineCode:        FunctionMenu(selectedObject, storage); break;
                case VSObjectType.OnStateEntry:      OnStatePackageMenu(selectedObject); break;
                case VSObjectType.OnStateUpdate:     OnStatePackageMenu(selectedObject); break;
                case VSObjectType.OnStateExit:       OnStatePackageMenu(selectedObject); break;
                case VSObjectType.Package:           if(selectedObject.IsInstanceNode) {
                                                                 InstanceMenu(selectedObject, storage);
                                                           } else {
                                                                 PackageMenu(selectedObject, storage);
                                                           }
                                                           break;
                default: if(selectedObject.IsPort)         PortMenu(menuType, selectedObject, storage, reverseInOut); break;
            }
        }

    	// ----------------------------------------------------------------------
        void CanvasMenu(iCS_EditorObject selectedObject, iCS_IStorage iStorage) {
            // Don't show any menu if behaviour not visible.
            if(selectedObject.IsIconizedInLayout || selectedObject.IsFoldedInLayout) return;

            iCS_MenuContext[] menu= new iCS_MenuContext[3];
            menu[0]= new iCS_MenuContext(VariableCreationStr);
            menu[1]= new iCS_MenuContext(FunctionCreationStr);
            menu[2]= new iCS_MenuContext(NestedTypeCreationStr);
            // Add Unity message handlers
            var baseType= CodeGenerationUtility.GetBaseType(iStorage);
            if(iCS_Types.IsA<MonoBehaviour>(baseType)) {
                var libraryType= LibraryController.LibraryDatabase.GetLibraryType(typeof(MonoBehaviour));
        		var eventHandlers= libraryType.GetMembers<LibraryEventHandler>();
                int len= P.length(eventHandlers);
        		int idx= GrowMenuBy(ref menu, len);
                for(int i= 0; i < len; ++i) {
        			var eventHandler= eventHandlers[i];
                    string nodeName= eventHandler.nodeName;
                    string displayString= eventHandler.displayString;
                    if(iCS_AllowedChildren.CanAddChildNode(nodeName, VSObjectType.InstanceMessage, selectedObject, iStorage)) {
                        menu[idx+i]= new iCS_MenuContext(String.Concat("+ "+AddUnityEventStr, displayString), eventHandler);
                    } else {
                        menu[idx+i]= new iCS_MenuContext(String.Concat("#+ "+AddUnityEventStr, displayString), eventHandler);
                    }
                }            
            }
            ShowMenu(menu, selectedObject, iStorage);
        }
    	// ----------------------------------------------------------------------
        void MessageHandlerMenu(iCS_EditorObject messageHandler, iCS_IStorage storage) {
            iCS_MenuContext[] menu= StartWithFocusMenu(messageHandler);
            CommonPackageMenu(messageHandler, storage, ref menu);
    		ShowMenu(menu, messageHandler, storage);
        }
    	// ----------------------------------------------------------------------
        void PackageMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
            iCS_MenuContext[] menu= StartWithFocusMenu(selectedObject);
            CommonPackageMenu(selectedObject, storage, ref menu);
    		ShowMenu(menu, selectedObject, storage);
        }
    	// ----------------------------------------------------------------------
        void CommonPackageMenu(iCS_EditorObject selectedObject, iCS_IStorage storage, ref iCS_MenuContext[] menu) {
            int idx;
            if(!selectedObject.IsIconizedInLayout && !selectedObject.IsFoldedInLayout) {
                // Base menu items
                idx= GrowMenuBy(ref menu, 4);
                menu[idx]= new iCS_MenuContext(PackageStr);
                menu[idx+1]= new iCS_MenuContext(StateChartStr);
                menu[idx+2]= new iCS_MenuContext(InlineCodeStr);
                menu[idx+3]= new iCS_MenuContext(SeparatorStr);
            }
            if(!selectedObject.IsFunctionDefinition && !selectedObject.IsEventHandler) {
                idx= GrowMenuBy(ref menu, 2);
                menu[idx]= new iCS_MenuContext(EnablePortStr);
                if(storage.HasTriggerPort(selectedObject)) {
                    menu[idx+1]= new iCS_MenuContext("#"+TriggerPortStr);
                } else {
                    menu[idx+1]= new iCS_MenuContext(TriggerPortStr);                
                }            
            }
    		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
            if(selectedObject.ObjectType == VSObjectType.Package) {
                if(selectedObject.HasChildNode()) {
                    idx= GrowMenuBy(ref menu, 1);
                    menu[idx]= new iCS_MenuContext(UnwrapPackageStr);                
                }
            }
            AddShowInHierarchyMenuItem(ref menu);
            AddDeleteMenuItem(ref menu);
            if(selectedObject.ObjectType == VSObjectType.Package) {
                if(selectedObject.HasChildNode()) {
                    idx= GrowMenuBy(ref menu, 1);
                    menu[idx]= new iCS_MenuContext(UnwrapPackageStr);                
                }
            }
        }
    	// ----------------------------------------------------------------------
        void OnStatePackageMenu(iCS_EditorObject targetObject) {
            iCS_MenuContext[] menu= StartWithFocusMenu(targetObject);
            if(!targetObject.IsIconizedInLayout && !targetObject.IsFoldedInLayout) {
                // Base menu items
                int idx= GrowMenuBy(ref menu, 3);
                menu[idx]= new iCS_MenuContext(PackageStr);
                menu[idx+1]= new iCS_MenuContext(StateChartStr);
                menu[idx+2]= new iCS_MenuContext(SeparatorStr);
            }
            AddShowInHierarchyMenuItem(ref menu);
            AddDeleteMenuItem(ref menu);
    		ShowMenu(menu, targetObject, targetObject.IStorage);
        }
    	// ----------------------------------------------------------------------
        void InstanceMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
            iCS_MenuContext[] menu= new iCS_MenuContext[0];
            if(!selectedObject.IsIconizedInLayout) {
                // Base menu items
                menu= new iCS_MenuContext[3];
                menu[0]= new iCS_MenuContext(EnablePortStr);
                if(storage.HasTriggerPort(selectedObject)) {
                    menu[1]= new iCS_MenuContext("#"+TriggerPortStr);
                } else {
                    menu[1]= new iCS_MenuContext(TriggerPortStr);                
                }
                menu[2]= new iCS_MenuContext(SeparatorStr);
            }
     		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
            AddShowInHierarchyMenuItem(ref menu);
            AddDeleteMenuItem(ref menu);
            ShowMenu(menu, selectedObject, storage);
        }
    	// ----------------------------------------------------------------------
        void StateChartMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
            iCS_MenuContext[] menu= StartWithFocusMenu(selectedObject);
            AddEnableAndTriggerMenuItem(ref menu, selectedObject);
            if(!selectedObject.IsIconizedInLayout && !selectedObject.IsFoldedInLayout) {
                int idx= GrowMenuBy(ref menu, 1);
                menu[idx]= new iCS_MenuContext(StateStr); 
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
            iCS_MenuContext[] menu= StartWithFocusMenu(selectedObject);
            if(!selectedObject.IsIconizedInLayout && !selectedObject.IsFoldedInLayout) {
                int len= iCS_AllowedChildren.StateChildNames.Length;
                int idx= GrowMenuBy(ref menu, len+3);
                menu[idx]= new iCS_MenuContext(StateStr);
                menu[idx+1]= new iCS_MenuContext(SeparatorStr);
                for(int i= 0; i < len; ++i) {
                    string name= iCS_AllowedChildren.StateChildNames[i];
                    if(iCS_AllowedChildren.CanAddChildNode(name, VSObjectType.Package, selectedObject, storage)) {
                        menu[idx+i+2]= new iCS_MenuContext(String.Concat("+ ", name));
                    } else {
                        menu[idx+i+2]= new iCS_MenuContext(String.Concat("#+ ", name));
                    }
                }
                menu[idx+len+2]= new iCS_MenuContext(SeparatorStr);
            }
            int cursor= GrowMenuBy(ref menu, 1);
            if(selectedObject.IsEntryState) {
                menu[cursor]= new iCS_MenuContext(String.Concat("#", SetAsEntryStr));
            } else {
                menu[cursor]= new iCS_MenuContext(SetAsEntryStr);
            }
            AddShowInHierarchyMenuItem(ref menu);
            AddDeleteMenuItem(ref menu);
            ShowMenu(menu, selectedObject, storage);
        }
    
    	// ----------------------------------------------------------------------
        void FunctionMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
            iCS_MenuContext[] menu;
            if(!selectedObject.IsIconizedInLayout) {
                // Base menu items
                menu= new iCS_MenuContext[0];
                int idx= 0;
                // -- Add Enable & Trigger --
                idx= GrowMenuBy(ref menu, 1);
                menu[idx]= new iCS_MenuContext(EnablePortStr);
                idx= GrowMenuBy(ref menu, 1);
                if(storage.HasTriggerPort(selectedObject)) {
                    menu[idx]= new iCS_MenuContext("#"+TriggerPortStr);
                } else {
                    menu[idx]= new iCS_MenuContext(TriggerPortStr);                
                }
                idx= GrowMenuBy(ref menu, 1);
                menu[idx]= new iCS_MenuContext(SeparatorStr);
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
            if(port.CanBeDeleted()) {
                AddDeleteMenuItem(ref menu);
            }
            ShowMenu(menu, port, storage);            
        }
    	// ----------------------------------------------------------------------
        void ReleaseAfterDragPortMenu(iCS_EditorObject port, iCS_IStorage storage, bool reverseInOut) {
    		// Different behaviour for "object instance" and standard packages.
            var newNodeParent= storage.GetNodeAt(GraphPosition);
            var portParentNode= port.ParentNode;
    		if(newNodeParent == null) return;
            iCS_MenuContext[] menu= new iCS_MenuContext[0];
            var portType= iCS_Types.RemoveRefOrPointer(port.RuntimeType);
    		if(!newNodeParent.IsInstanceNode && portType != typeof(float) && portType != typeof(int) && portType != typeof(bool) &&
                (port.IsOutputPort && newNodeParent != portParentNode && !portParentNode.IsParentOf(newNodeParent)) ||
                (port.IsInputPort && (newNodeParent == portParentNode || portParentNode.IsParentOf(newNodeParent)))) {
    			// Add shortcut to instance node creation.
    			menu= new iCS_MenuContext[2];
    			menu[0]= new iCS_MenuContext(ObjectInstanceStr);
                menu[0].Command= "+ Create Property Accessor";
    	        menu[1]= new iCS_MenuContext(SeparatorStr);
    		}
			// TODO: Rebuild release after drag menu
//            // Get compatible functions.
//            if(port.IsDataOrControlPort) {
//                List<iCS_FunctionPrototype> functionMenu= null;
//    			Type inputType= null;
//    			Type outputType= null;
//    			if(port.IsInputPort) {
//                    outputType= port.RuntimeType;
//                } else {
//                    inputType= port.RuntimeType;
//                }
//    			if(reverseInOut) {
//    				var tmp= inputType;
//    				inputType= outputType;
//    				outputType= tmp;
//    			}
//    			if(newNodeParent.IsInstanceNode) {
//    	            functionMenu= iCS_LibraryDatabase.BuildMenuForMembersOfType(newNodeParent.RuntimeType, inputType, outputType);								
//    			}
//    			else {
//    	            functionMenu= iCS_LibraryDatabase.BuildMenu(inputType, outputType);				
//    			}
//                if(functionMenu.Count != 0) {
//                    int len= menu.Length;
//                    iCS_MenuContext[] tmp= null;
//                    if(len == 0) {
//                        tmp= new iCS_MenuContext[functionMenu.Count];
//                    } else {
//                        tmp= new iCS_MenuContext[len+1+functionMenu.Count];
//                        menu.CopyTo(tmp, 0);
//                        tmp[len]= new iCS_MenuContext(SeparatorStr);
//                        ++len;                    
//                    }
//                    menu= tmp;                
//                    for(int i= 0; i < functionMenu.Count; ++i) {
//                        menu[len+i]= new iCS_MenuContext(functionMenu[i].ToMenu(), functionMenu[i]);
//                    }
//                }
//            }
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
    		menu[i]  = new iCS_MenuContext(SeparatorStr);
    		menu[i+1]= new iCS_MenuContext(WrapInPackageStr);
    	}
    	// ----------------------------------------------------------------------
        iCS_MenuContext[] StartWithFocusMenu(iCS_EditorObject selectedObject) {
            iCS_MenuContext[] menu= new iCS_MenuContext[0];
            AddNavigationMenu(ref menu, selectedObject);
            int idx= GrowMenuBy(ref menu, 1);
            menu[idx]= new iCS_MenuContext(SeparatorStr);
            return menu;        
        }
    	// ----------------------------------------------------------------------
        void AddNavigationMenu(ref iCS_MenuContext[] menu, iCS_EditorObject obj) {
            var iStorage= obj.IStorage;
            if(iStorage.DisplayRoot != iStorage.RootObject ||
               iStorage.HasForwardNavigationHistory || iStorage.HasBackwardNavigationHistory) {
                int idx= GrowMenuBy(ref menu, 1);
                menu[idx]= new iCS_MenuContext(ClearNavigationHistoryStr);
            }
            if(obj != iStorage.DisplayRoot) {
                int idx= GrowMenuBy(ref menu, 1);
                menu[idx]= new iCS_MenuContext(SetAsDisplayRootStr);            
            }        
        }
    	// ----------------------------------------------------------------------
        void AddNodeMenu(ref iCS_MenuContext[] menu) {
			// TODO: Rebuild expert menu
//            var fullMenu= iCS_LibraryDatabase.BuildExpertMenu();
//            if(fullMenu.Count == 0) return;
//            int idx= GrowMenuBy(ref menu, fullMenu.Count+1);
//            for(int i= 0; i < fullMenu.Count; ++i) {
//                menu[idx]= new iCS_MenuContext(AddNodeStr+"/"+fullMenu[i].ToMenu(), fullMenu[i]);
//                ++idx;
//            }
//            menu[idx]= new iCS_MenuContext(SeparatorStr);
        }
	
        // ======================================================================
        // Menu Creation Utilities
    	// ----------------------------------------------------------------------
    	int GrowMenuBy(ref iCS_MenuContext[] menu, int additionalSize) {
    		int sze= menu == null ? 0 : menu.Length;
    		if(additionalSize == 0) return sze;
    		var newMenu= new iCS_MenuContext[sze+additionalSize];
    		menu.CopyTo(newMenu, 0);
    		menu= newMenu;
    		return sze;
    	}
    	// ----------------------------------------------------------------------
        void AddSeparator(ref iCS_MenuContext[] menu) {
            int idx= GrowMenuBy(ref menu, 1);
            menu[idx]= new iCS_MenuContext(SeparatorStr);
        }
    	// ----------------------------------------------------------------------
        void AddDeleteMenuItem(ref iCS_MenuContext[] menu) {
            int idx= GrowMenuBy(ref menu, 2);
            menu[idx]= new iCS_MenuContext(SeparatorStr);
            menu[idx+1]= new iCS_MenuContext(DeleteStr);
        }
    	// ----------------------------------------------------------------------
        void AddShowInHierarchyMenuItem(ref iCS_MenuContext[] menu) {
            int idx= GrowMenuBy(ref menu, 2);
            menu[idx]= new iCS_MenuContext(SeparatorStr);
            menu[idx+1]= new iCS_MenuContext(ShowHierarchyStr);        
        }
    	// ----------------------------------------------------------------------
        void AddEnableAndTriggerMenuItem(ref iCS_MenuContext[] menu, iCS_EditorObject node) {
            if(node.IsIconizedInLayout) return;
            int idx= GrowMenuBy(ref menu, 3);
            menu[idx]= new iCS_MenuContext(EnablePortStr);
            var iStorage= node.IStorage;
            if(iStorage.HasTriggerPort(node)) {
                menu[idx+1]= new iCS_MenuContext("#"+TriggerPortStr);
            } else {
                menu[idx+1]= new iCS_MenuContext(TriggerPortStr);
            }
            menu[idx+2]= new iCS_MenuContext(SeparatorStr);
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

        // ======================================================================
        // Menu processing
    	// ----------------------------------------------------------------------
        void ProcessMenu(object obj) {
            iCS_MenuContext context= obj as iCS_MenuContext;
            iCS_EditorObject targetObject= context.SelectedObject;
    		Vector2 globalPos= context.GraphPosition;
            iCS_IStorage iStorage= context.Storage;
            // Process all other types of requests.
            switch(context.HiddenCommand) {
                case SetAsDisplayRootStr:       iCS_UserCommands.SetAsDisplayRoot(targetObject); break;
                case ClearNavigationHistoryStr: iCS_UserCommands.ResetDisplayRoot(iStorage); break;
                case PackageStr:                iCS_UserCommands.CreatePackage(targetObject, globalPos, null); break;
                case InlineCodeStr:             iCS_UserCommands.CreateInlineCode(targetObject, globalPos, "InlineCode"); break;
                case StateChartStr:             iCS_UserCommands.CreateStateChart(targetObject, globalPos, null); break;
                case StateStr:                  iCS_UserCommands.CreateState(targetObject, globalPos, null);  break;
                case SetAsEntryStr:             iCS_UserCommands.SetAsStateEntry(targetObject); break;
                case OnEntryStr:                iCS_UserCommands.CreateOnEntryPackage(targetObject, globalPos); break;
                case OnUpdateStr:               iCS_UserCommands.CreateOnUpdatePackage(targetObject, globalPos); break;
                case OnExitStr:                 iCS_UserCommands.CreateOnExitPackage(targetObject, globalPos); break;
    			case ObjectInstanceStr:         CreateObjectInstance(context); break;
                case ShowHierarchyStr:          iCS_UserCommands.ShowInHierarchy(targetObject); break;
                case DeleteStr:                 iCS_UserCommands.DeleteObject(targetObject); break;
                case EnablePortStr:             iCS_UserCommands.CreateEnablePort(targetObject); break;
                case TriggerPortStr:            iCS_UserCommands.CreateTriggerPort(targetObject); break;
    			case WrapInPackageStr:          iCS_UserCommands.WrapInPackage(targetObject); break;
                case MultiSelectionWrapInPackageStr: iCS_UserCommands.WrapMultiSelectionInPackage(iStorage); break;
                case MultiSelectionDeleteStr:        iCS_UserCommands.DeleteMultiSelectedObjects(iStorage); break;
                case UnwrapPackageStr:               iCS_UserCommands.DeleteKeepChildren(targetObject); break;
                case FunctionCreationStr:            iCS_UserCommands.CreateFunctionDefinition(targetObject, globalPos); break;
                default: {
                    if(targetObject == null) break;
                    var libraryObject= context.myLibraryObject;
    				if(libraryObject == null) {
    					Debug.LogWarning(iCS_Config.ProductName+": Can find reflection descriptor to create node !!!");
    					break;
    				}
                    if(libraryObject is LibraryEventHandler) {
                        iCS_UserCommands.CreateEventHandler(targetObject, globalPos, libraryObject as LibraryEventHandler);
                        break;
                    }
					if(targetObject.IsPort) {
	                    CreateAttachedMethod(targetObject, iStorage, globalPos, libraryObject);
					}
					else {
	                    iCS_UserCommands.CreateFunctionCallNode(targetObject, globalPos, libraryObject);
					}
					break;                
                }
            }
        }


        // ======================================================================
        // High-Level Creation Utilities
        // ----------------------------------------------------------------------
        static void CreateObjectInstance(iCS_MenuContext context) {
    		var iStorage= context.Storage;
    		var sourcePort = context.SelectedObject;
    		var pos= context.GraphPosition;
    		var parent= iStorage.GetNodeAt(pos);
    		if(parent == null) return;
            iCS_UserCommands.OpenTransaction(iStorage);
    		var instance= iCS_UserCommands.CreateObjectInstance(parent, pos, sourcePort.RuntimeType);   
    		if(sourcePort != null) {
    	        var thisPort= iStorage.PropertiesWizardGetInputThisPort(instance);
    	        iStorage.SetNewDataConnection(thisPort, sourcePort);					
    		}
            iCS_UserCommands.CloseTransaction(iStorage, "Create Object Instance");
        }

    	// ----------------------------------------------------------------------
        static iCS_EditorObject CreateAttachedMethod(iCS_EditorObject port, iCS_IStorage iStorage, Vector2 globalPos, LibraryObject libraryObject) {
            iCS_EditorObject newNodeParent= iStorage.GetNodeAt(globalPos);
    		if(newNodeParent == null) return null;
            if(!newNodeParent.IsKindOfPackage || newNodeParent.IsBehaviour) return null;
            // FIXME: Animation & force layout sometime conflict.
            // Open a transaction for multi-operations
            iCS_UserCommands.OpenTransaction(iStorage);
    		iCS_EditorObject method= null;
            iStorage.AnimateGraph(null,
                _=> {
            		if(newNodeParent.IsInstanceNode) {
            			method= iCS_UserCommands.CreatePropertiesWizardElement(newNodeParent, libraryObject);
            		}
            		else {
                        method= iCS_UserCommands.CreateFunctionCallNode(newNodeParent, globalPos, libraryObject);							
            		}

            		// Inverse effective data flow if new node is inside port parent.
            		bool isConsumerPort= port.IsInputPort;
            		var portParent= port.ParentNode;
            		if(portParent.IsParentOf(method)) isConsumerPort= !isConsumerPort;
                    iCS_EditorObject attachedPort= null;
                    iCS_EditorObject providerPort= null;
                    iCS_EditorObject consumerPort= null;
                    if(isConsumerPort) {
            			iCS_EditorObject[] outputPorts= Prelude.filter(x=> iCS_Types.IsA(port.RuntimeType, x.RuntimeType), iStorage.GetChildOutputDataPorts(method)); 
            			// Connect if only one possibility.
            			if(outputPorts.Length == 1) {
                            attachedPort= outputPorts[0];
                            consumerPort= port;
                            providerPort= attachedPort;
            			}
            			else {
            				var bestPort= GetClosestMatch(port, outputPorts);
            				if(bestPort != null) {
                                attachedPort= bestPort;
                                consumerPort= port;
                                providerPort= bestPort;
            				}
            			}
                    } else {
            			iCS_EditorObject[] inputPorts= Prelude.filter(x=> iCS_Types.IsA(x.RuntimeType, port.RuntimeType), iStorage.GetChildInputDataPorts(method));
            			// Connect if only one posiibility
            			if(inputPorts.Length == 1) {
                            attachedPort= inputPorts[0];
                            consumerPort= attachedPort;
                            providerPort= port;
            			}
            			// Multiple choices exist so try the one with the closest name.
            			else {
            				var bestPort= GetClosestMatch(port, inputPorts);
            				if(bestPort != null) {
                                attachedPort= bestPort;
                                consumerPort= bestPort;
                                providerPort= port;
            				}
            			}
                    }
                    // Position attached port and layout binding.
                    if(attachedPort != null && consumerPort != null && providerPort != null) {
                        iStorage.AutoLayoutPort(attachedPort, port.GlobalPosition, attachedPort.ParentNode.GlobalPosition);
                        iStorage.SetAndAutoLayoutNewDataConnection(consumerPort, providerPort);
                    }                
                    iStorage.ForcedRelayoutOfTree();
                }
            );
            iCS_UserCommands.CloseTransaction(iStorage, "Create => "+libraryObject.nodeName);
            return method;
        }
    
    
        // ======================================================================
        // Utilities
    	// ----------------------------------------------------------------------
    	static iCS_EditorObject GetClosestMatch(iCS_EditorObject refPort, iCS_EditorObject[] otherPorts) {
    		if(otherPorts.Length == 0) return null;
    		float bestScore= -1f;
    		iCS_EditorObject bestPort= null;
    		string refName= refPort.DisplayName;
    		if(string.IsNullOrEmpty(refPort.DisplayName)) refName= refPort.ParentNode.DisplayName;
    		foreach(var p in otherPorts) {
    			string pName= p.DisplayName;
    			if(string.IsNullOrEmpty(p.DisplayName)) pName= p.ParentNode.DisplayName;
    			var score= iCS_StringUtil.FuzzyCompare(refName, pName);
    			if(score > bestScore) {
    				bestScore= score;
    				bestPort= p;
    			}
    		}
    		return bestPort;		
    	}
    }
    
}

