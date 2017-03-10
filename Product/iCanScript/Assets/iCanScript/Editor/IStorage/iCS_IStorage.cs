using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using iCanScript.Internal.Engine;
using P= iCanScript.Internal.Prelude;
using Prefs= iCanScript.Internal.Editor.PreferencesController;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
                bool                    myForceRelayout = true;
				PackageInfo				myPackage       = null;
        public  iCS_MonoBehaviourImp    iCSMonoBehaviour= null;
        public  iCS_VisualScriptData    EditorStorage   = null;
        List<iCS_EditorObject>          myEditorObjects = null;
        public  int                     ModificationId  = -1;
        public  bool                    CleanupDeadPorts= true;
        public  int                     NumberOfNodes   = 0;
    
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public List<iCS_EditorObject>   EditorObjects    { get { return myEditorObjects; }}
        public List<iCS_EngineObject>   EngineObjects    { get { return Storage.EngineObjects; }}
		public PackageInfo	Package {
			get {
				if(myPackage == null) {
					myPackage= PackageController.GetProjectFor(this);
				}
				return myPackage;
			}
		}
        public iCS_VisualScriptData Storage {
            get { return EditorStorage; }
            set { EditorStorage= value; }
        }
        public iCS_IVisualScriptData EngineStorage {
            get { return iCSMonoBehaviour; }
        }
        public GameObject HostGameObject {
            get { return iCSMonoBehaviour.gameObject; }
        }
        public iCS_VisualScriptImp VisualScript {
            get {
                return iCSMonoBehaviour as iCS_VisualScriptImp;
            }
        }
        public iCS_MonoBehaviourImp VSMonoBehaviour {
            get {
                return iCSMonoBehaviour;
            }
        }
        public bool HasRootObject {
            get { return EditorObjects != null && EditorObjects.Count != 0; }
        }
        public iCS_EditorObject RootObject {
            get { return EditorObjects[0]; }
        }
        public bool IsRootObjectAType {
            get { return HasRootObject && IsBehaviour; }
        }
        public int DisplayRootId {
            get {
                return Storage.DisplayRoot;
            }
            set {
                Storage.DisplayRoot= value;
                if(!IsUserTransactionActive) {
                    EngineStorage.DisplayRoot= value;
                }
            }
        }
        public iCS_EditorObject DisplayRoot {
            get {
                if(myEditorObjects == null || myEditorObjects.Count == 0) {
                    return null;
                }
                int id= Storage.DisplayRoot;
                if(!IsIdValid(id)) {
                    Storage.DisplayRoot= 0;
                    return EditorObjects[0];
                }
                var obj= EditorObjects[id];
                if(!IsValid(obj) || !obj.IsNode) {
                    Storage.DisplayRoot= 0;
                    return EditorObjects[0];
                }
                return obj;
            }
            set {
                // Keep EngineStorage & Storage in sync since DisplayRoot is not comprised in the Undo.
                if(value == null || !IsIdValid(value.InstanceId)) {
                    Storage.DisplayRoot= 0;
                    if(!IsUserTransactionActive) {
                        EngineStorage.DisplayRoot= 0;                    
                    }
                    return;
                }
                if(!value.IsNode) return;
                Storage.DisplayRoot= value.InstanceId;
                if(!IsUserTransactionActive) {
                    EngineStorage.DisplayRoot= value.InstanceId;
                }
            }
        } 
    	public bool ShowDisplayRootNode {
    		get { return Storage.ShowDisplayRootNode; }
    		set {
                Storage.ShowDisplayRootNode= value;
            }
    	}
        public bool ForceRelayout {
            get { return myForceRelayout; }
            set { myForceRelayout= value; }
        }
        public bool IsEditorScript {
            get { return Storage.IsEditorScript; }
            set { Storage.IsEditorScript= value; }
        }
        public string CSharpFileName {
            get { return Storage.CSharpFileName; }
            set { Storage.CSharpFileName= value; }
        }
        public string TypeName {
            get { return RootObject.CodeName; }
            set { RootObject.DisplayName= value; }
        }
        public bool BaseTypeOverride {
            get { return Storage.BaseTypeOverride; }
            set { Storage.BaseTypeOverride= value; }
        }
        public string BaseType {
            get { return Storage.BaseType; }
            set { Storage.BaseType= value; }
        }
        public string Namespace {
            get { return CodeGenerationUtility.GetNamespace(this); }
        }
    	public Vector2 ScrollPosition {
    	    get { return Storage.ScrollPosition; }
    	    set {
                Storage.ScrollPosition= value;
                if(!IsUserTransactionActive) {
                    EngineStorage.ScrollPosition= value;
                }
            }
    	}
        public float GuiScale {
            get { return Storage.GuiScale; }
            set {
                Storage.GuiScale= value;
                if(!IsUserTransactionActive) {
                    EngineStorage.GuiScale= value;
                }
            }
        }
        public int UndoRedoId {
            get { return Storage.UndoRedoId; }
        }
        public iCS_EditorObject this[int id] {
            get {
                if(!IsIdValid(id)) return null;
                return EditorObjects[id];
            }
            set {
                DetectUndoRedo();
                EditorObjects[id]= value;
            }
        }
        public iCS_NavigationHistory NavigationHistory {
            get { return Storage.NavigationHistory; }
        }

    

        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
        public iCS_IStorage(iCS_MonoBehaviourImp monoBehaviour) {
            Init(monoBehaviour);
        }
        
        public void Init(iCS_MonoBehaviourImp monoBehaviour) {
            // Update the MonoBehaviour variable
            var oldMonoBehaviour= iCSMonoBehaviour;
            iCSMonoBehaviour= monoBehaviour;
            // Rebuild the editor information if the MonoBehaviour has changed.
            if(oldMonoBehaviour != monoBehaviour) {
                InitEditorData();
                // Reset display root if no navigation history present
                if(!NavigationHistory.HasBackwardHistory && EditorObjects.Count > 0) {
                    DisplayRoot= EditorObjects[0];
                }
                // Force a relayout
                ForceRelayout= true;
            }

            // -- Force that a top-level type object exists. --
            if(!HasRootObject) {
                if(myEditorObjects == null) {
                    myEditorObjects= new List<iCS_EditorObject>();
                }
                if(VisualScript != null) {
                    CreateBehaviour(HostGameObject.name);                    
                }
            }
            
            // -- Count number of nodes to limit community version --
            if(EditionController.IsCommunityEdition) {
                NumberOfNodes= 0;
                ForEach(obj=> {
                    if(obj.IsNode && obj != RootObject && !obj.ParentNode.IsInstanceNode) {
                        ++NumberOfNodes;
                    }
                });            
            }
        
            // Perform an initial sanity check.
    		Cleanup();
            SanityCheck();

    //        TestFind_iCanScript();
        }
    
    //    void TestFind_iCanScript() {
    //        var go= HostGameObject;
    //        var components= go.GetComponents<MonoBehaviour>();
    //        foreach(var c in components) {
    //            var t= c.GetType();
    //            if(iCS_Types.IsGeneratedByiCanScript(t.Namespace, t.Name)) {
    //                Debug.Log(t.Namespace+"."+t.Name+" was generated by iCanScript");
    //                Debug.Log("Source File: "+iCS_Types.GetICanScriptFile(t));
    //                Debug.Log("Source File GUID: "+iCS_Types.GetICanScriptFileGUID(t));                
    //            }
    //        }
    //
    //        var baseTypeName= Prefs.CodeGenerationBaseTypeName;
    //        var baseType= iCS_Types.GetTypeFromTypeString(baseTypeName);
    //        if(baseType != null) {
    //            var abstractMethods= iCS_Types.GetAbstractMethods(baseType);
    //            Debug.Log("# abstract= "+abstractMethods.Length);
    //            foreach(var m in abstractMethods) {
    //                Debug.Log(m.Name);
    //            }
    //        }
    //    }
    
    
        // ----------------------------------------------------------------------
        public bool IsBehaviour {
    		get {
    			return IsValid(EditorObjects[0]) && EditorObjects[0].IsBehaviour;
    		}
    	}
        public bool IsEmptyBehaviour    {
            get {
                if(!IsBehaviour) return false;
                for(int i= 1; i < EditorObjects.Count; ++i) {
                    if(IsValid(EditorObjects[i])) return false;
                }
                return true;
            }
        }
        // ----------------------------------------------------------------------
        public bool IsIdValid(int id) {
    		return id >= 0 && id < EngineObjects.Count;
    	}
    	public bool IsValid(int id) {
    		return IsIdValid(id) && IsValid(EditorObjects[id]);
    	}
        public bool IsValid(iCS_EditorObject obj) {
    		return obj != null && IsIdValid(obj.InstanceId);
    	}
        public bool IsSourceValid(iCS_EditorObject obj)  { return IsIdValid(obj.ProducerPortId); }
        public bool IsParentValid(iCS_EditorObject obj)  { return IsIdValid(obj.ParentId); }
        // ----------------------------------------------------------------------
    	public bool IsAnimationPlaying {
    		get { UpdateAllAnimations(); return myAnimatedObjects.Count != 0; }
    	}
        // ----------------------------------------------------------------------
    	public iCS_EditorObject GetParentMuxPort(iCS_EditorObject eObj) {
    		return eObj.IsParentMuxPort ? eObj : (eObj.IsChildMuxPort ? eObj.Parent : null);
    	}
        // ----------------------------------------------------------------------
        public System.Object GetRuntimeObject(iCS_EditorObject obj) {
    		return obj == null ? null : obj.GetRuntimeObject;
        }
    
        // ======================================================================
        // Storage Update
        // ----------------------------------------------------------------------
        public void Update() {
            // Processing any changed caused by Undo/Redo
            DetectUndoRedo();
        
            // Force a relayout if it is requested
            if(myForceRelayout) {
                myForceRelayout= false;
                ForcedRelayoutOfTree();    			
            }
		
            // Update object animations.
    		if(IsAnimationPlaying) {
    			UpdateAllAnimations();			
    		}
        }

        // ----------------------------------------------------------------------
    	// This function is invoked after a change in the visual script.  It
    	// is assumed that the visual script is in a stable state when this
    	// function is invoked.
        public bool Cleanup() {
            bool modified= false;
    		bool needsRelayout= false;
            NumberOfNodes= 0;
            ForEach(
                obj=> {
                    // -- Count number of nodes to limit trial version --
                    if(obj.IsNode && obj != RootObject && !obj.ParentNode.IsInstanceNode) ++NumberOfNodes;
                
                    // Keep a copy of the final position.
                    obj.AnimationTargetRect= obj.GlobalRect;
                    // Cleanup disconnected or dangling ports.
                    if(CleanupDeadPorts) {
    					if(obj.IsPort) {
    						bool shouldRemove= false;
    	                    if(obj.IsDynamicDataPort && IsPortDisconnected(obj)) {
    	                        shouldRemove= true;
    	                    } else if(obj.IsParentMuxPort && IsPortDisconnected(obj) && obj.HasChildPort() == false) {
    	                        shouldRemove= true;
    	                    } else if(obj.IsChildMuxPort && obj.ProducerPort == null) {
    	                        shouldRemove= true;
    	                    } else if(obj.ProducerPort == null) {
    							if(obj.IsChildMuxPort || obj.IsInStatePort || obj.IsInTransitionPort) {
    		                        shouldRemove= true;								
    							}
    	                    } else if(obj.IsStatePort && IsPortDisconnected(obj)) {
    	                        shouldRemove= true;
    						} else if(obj.Parent == null) {
    							shouldRemove= true;
    						} else if(obj.Parent.IsPort && !obj.Parent.IsParentMuxPort) {
    							shouldRemove= true;
    						} 
    						if(shouldRemove) {
    	                        DestroyInstanceInternal(obj);                            
    	                        modified= true;						
    						}
    						// -- Convert input mux to dynamic port if no children. --
    						if(obj.IsInParentMuxPort) {
    	                        switch(obj.NumberOfChildPorts()) {
    	                            case 0:
    	    					        obj.ObjectType= VSObjectType.InDynamicDataPort;
    	                                break;
    	                            case 1:
    	                                var childPorts= obj.BuildListOfChildPorts(_=> true);
    	                                obj.ProducerPort= childPorts[0].ProducerPort;
    	    					        obj.ObjectType= VSObjectType.InDynamicDataPort;
    	                                DestroyInstanceInternal(childPorts[0]);
                                        modified= true;
    	                                break;
    	                        }
    						}
                            // -- Convert any dynamic ports on public functions to proposed ports --
                            if(obj.IsDynamicDataPort && obj.ParentNode.IsFunctionDefinition) {
                                obj.ObjectType= obj.IsInDynamicDataPort ?
                                    VSObjectType.InProposedDataPort :
                                    VSObjectType.OutProposedDataPort;
                            }
    					}
                        // Cleanup disconnected typecasts.
        				if(obj.IsTypeCast) {
    						var inDataPort= FindInChildren(obj, c=> c.IsInDataOrControlPort);
                            if(inDataPort.ProducerPort == null ||
                               FindAConnectedPort(FindInChildren(obj, c=> c.IsOutDataOrControlPort)) == null) {
                               DestroyInstanceInternal(obj);
                               modified= true;
                            }
                        }
    					// Cleanup disconnected state transitions.
    					if(obj.IsTransitionPackage) {
    						bool hasInTransitionPort= false;
    						bool hasOutTransitionPort= false;
    						bool hasTriggerPort= false;
    						obj.ForEachChild(
    							c=> {
    								if(c.IsInTransitionPort) {
    									hasInTransitionPort= true;
    								} else if(c.IsOutTransitionPort) {
    									hasOutTransitionPort= true;
    								} else if(c.IsOutFixDataPort && c.IsTriggerPort) {
    									hasTriggerPort= true;
    								}
    							}
    						);
    						if(!(hasInTransitionPort && hasOutTransitionPort && hasTriggerPort)) {
    							DestroyInstanceInternal(obj);
    						}
    					}                    
    				}
                }
            );
    		if(needsRelayout) {
    			ForcedRelayoutOfTree();
    		}
            return modified;
        }

        // ======================================================================
        // Editor Object Creation/Destruction
        // ----------------------------------------------------------------------
        int GetNextAvailableId() {
            // Covers Undo?redo for all creation operation
            DetectUndoRedo();
            // Find the next available id.
            int id= 0;
            int len= EditorObjects.Count;
            while(id < len && IsValid(EditorObjects[id])) { ++id; }
            return id;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject Copy(iCS_EditorObject srcObj, iCS_IStorage srcStorage,
                                     iCS_EditorObject destParent, Vector2 globalPos, iCS_IStorage destStorage) {
            // Create new EditorObject
            List<Prelude.Tuple<int, int>> xlat= new List<Prelude.Tuple<int, int>>();
            iCS_EditorObject instance= Copy(srcObj, srcStorage, destParent, destStorage, globalPos, xlat);
            ReconnectCopy(srcObj, srcStorage, destStorage, xlat);
            instance.CollisionOffsetFromGlobalPosition= globalPos;
            return instance;
        }
        iCS_EditorObject Copy(iCS_EditorObject srcObj, iCS_IStorage srcStorage,
                              iCS_EditorObject destParent, iCS_IStorage destStorage, Vector2 globalPos, List<Prelude.Tuple<int,int>> xlat) {
            // Create new EditorObject
            int id= destStorage.GetNextAvailableId();
            xlat.Add(new Prelude.Tuple<int,int>(srcObj.InstanceId, id));
            var newObj= destStorage[id]= iCS_EditorObject.Clone(id, srcObj, destParent, destStorage);
            if(newObj.IsNode) {
                newObj.LocalAnchorFromGlobalPosition= globalPos;
            }
            newObj.IconGUID= srcObj.IconGUID;
            srcObj.ForEachChild(
                child=> Copy(child, srcStorage, newObj, destStorage, globalPos+child.LocalAnchorPosition, xlat)
            );
            return newObj;
        }
        void ReconnectCopy(iCS_EditorObject srcObj, iCS_IStorage srcStorage, iCS_IStorage destStorage, List<Prelude.Tuple<int,int>> xlat) {
            srcStorage.ForEachRecursive(srcObj,
                child=> {
                    if(child.ProducerPortId != -1) {
                        int id= -1;
                        int sourceId= -1;
                        foreach(var pair in xlat) {
                            if(pair.Item1 == child.InstanceId) {
                                id= pair.Item2;
                                if(sourceId != -1) break;
                            }
                            if(pair.Item1 == child.ProducerPortId) {
                                sourceId= pair.Item2;
                                if(id != -1) break;
                            }
                        }
                        if(sourceId != -1) {
                            destStorage.SetSource(destStorage.EditorObjects[id], destStorage.EditorObjects[sourceId]);                        
                        }
                    }
                }
            );
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject CreateBehaviour(string name) {
            // Create the function node.
            int id= GetNextAvailableId();
            // Validate that behaviour is at the root.
            if(id != 0) {
                Debug.LogError("Behaviour MUST be the root object !!!");
            }
            // Create new EditorObject
            iCS_EditorObject.CreateInstance(0, name, typeof(iCS_VisualScriptImp), -1, VSObjectType.Behaviour, this);
            this[0].LocalAnchorFromGlobalPosition= VisualEditorCenter();
            return this[0];
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject CreatePackage(int parentId, string name= "", VSObjectType objectType= VSObjectType.Package, Type runtimeType= null) {
    		if(runtimeType == null) runtimeType= typeof(iCS_Package);
            // Create the function node.
            int id= GetNextAvailableId();
            // Create new EditorObject
            var instance= iCS_EditorObject.CreateInstance(id, name, runtimeType, parentId, objectType, this);
            if(instance.IsInstanceNode) {
                PropertiesWizardCompleteCreation(instance);
                instance.DisplayName= NameUtility.ToTypeName(iCS_Types.TypeName(runtimeType))+" Properties";
                CreateSelfPort(id);
            }
            return instance;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject CreateStateChart(int parentId, string name= "") {
            // Create the function node.
            int id= GetNextAvailableId();
            // Create new EditorObject
            var instance= iCS_EditorObject.CreateInstance(id, name, typeof(iCS_StateChart), parentId, VSObjectType.StateChart, this);
            return instance;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject CreateState(int parentId, string name= "") {
            // Validate that we have a good parent.
            iCS_EditorObject parent= EditorObjects[parentId];
            if(parent == null || (!parent.IsStateChart && !parent.IsState)) {
                Debug.LogError("State must be created as a child of StateChart or State.");
            }
            // Create the function node.
            int id= GetNextAvailableId();
            // Create new EditorObject
            var instance= iCS_EditorObject.CreateInstance(id, name, typeof(iCS_State), parentId, VSObjectType.State, this);
            // Set first state as the default entry state.
            instance.IsEntryState= !UntilMatchingChild(parent,
                child=> {
                    if(child.IsEntryState) {
                        return true;
                    }
                    return false;
                }
            );
            return instance;
        }
        // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        // ----------------------------------------------------------------------
		/// Create base node.
		///
		/// @param parentId The ID of the parent node.
		/// @param libraryMethodInfo The library method information object.
		/// @param nodeType The visual script object type for the node.
		/// @return The newly created node.
		///
		iCS_EditorObject CreateBaseNode(int parentId, LibraryMemberInfo libraryMemberInfo, VSObjectType nodeType) {
            // -- Grab a unique ID for this node. --
            int id= GetNextAvailableId();
            // -- Create node --
            var nodeName     = libraryMemberInfo.nodeName;
    		var libraryParent= libraryMemberInfo.parent as LibraryType;
    		var declaringType= libraryParent.type;
            var instance= iCS_EditorObject.CreateInstance(id, nodeName, declaringType, parentId, nodeType, this);
    		instance.MethodName= libraryMemberInfo.memberName;
			return instance;
		}
        // ----------------------------------------------------------------------
    	/// Create parameter ports on the given node.
    	///
    	/// @param node The parent node.
    	/// @param parameters The array of parameter info.
    	///
    	void CreateParameterPorts(iCS_EditorObject node, ParameterInfo[] parameters) {
    		int nodeId= node.InstanceId;
    		var parametersLen= P.length(parameters);
    		node.NbOfParams= parametersLen;
            for(int idx= 0; idx < parametersLen; ++idx) {
                var p= parameters[idx];
    			var parameterType= p.ParameterType;
                if(parameterType != typeof(void)) {
                    VSObjectType portType= p.IsOut ? VSObjectType.OutFixDataPort : VSObjectType.InFixDataPort;
                    var port= CreatePort(p.Name, nodeId, parameterType, portType, (int)iCS_PortIndex.ParametersStart+idx);
    				object initialValue= p.DefaultValue;
    				if(initialValue == null || initialValue.GetType() != parameterType) {
    					initialValue= iCS_Types.DefaultValue(parameterType);
    				}
                    port.Value= initialValue;
                }
            }		
    	}
        // ----------------------------------------------------------------------
    	/// Create return port on the given node.
    	///
    	/// @param node The parent node.
		/// @param libraryMethodInfo The library method information object.
		/// @note The return port name is derived from the name of the node.
    	///
    	void CreateReturnPort(iCS_EditorObject node, LibraryMethodInfo libraryMethodInfo) {
    		var returnType= libraryMethodInfo.returnType;
    		if(returnType != null && returnType != typeof(void)) {
				string returnName= libraryMethodInfo.memberName;
				if(returnName.StartsWith("get_")) {
					returnName= returnName.Substring(4);
				}
                CreatePort(returnName, node.InstanceId, returnType, VSObjectType.OutFixDataPort, (int)iCS_PortIndex.Return);
    		}
		}
		
        // ----------------------------------------------------------------------
    	/// Creates the appropriate node type needed by the given library object.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryObject The library object from which to create a node.
    	/// @return The newly created node.
    	///
        public iCS_EditorObject CreateNode(int parentId, LibraryObject libraryObject) {
			if(libraryObject is LibraryConstructor) {
				var libraryConstructor= libraryObject as LibraryConstructor;
				if(libraryConstructor.isStatic) {
					return CreateStaticConstructorCallNode(parentId, libraryConstructor);
				}
				return CreateConstructorCallNode(parentId, libraryConstructor);
			}
			if(libraryObject is LibraryFunction) {
				var libraryFunction= libraryObject as LibraryFunction;
				if(libraryFunction.isStatic) {
					return CreateStaticFunctionCallNode(parentId, libraryFunction);
				}
				return CreateFunctionCallNode(parentId, libraryFunction);
			}
			if(libraryObject is LibraryFieldGetter) {
				var libraryGetField= libraryObject as LibraryFieldGetter;
				if(libraryGetField.isStatic) {
					return CreateStaticFieldGetterCallNode(parentId, libraryGetField);
				}
				return CreateFieldGetterCallNode(parentId, libraryGetField);
			}
			if(libraryObject is LibraryFieldSetter) {
				var librarySetField= libraryObject as LibraryFieldSetter;
				if(librarySetField.isStatic) {
					return CreateStaticFieldSetterCallNode(parentId, librarySetField);
				}
				return CreateFieldSetterCallNode(parentId, librarySetField);
			}
			if(libraryObject is LibraryPropertyGetter) {
				var libraryGetProperty= libraryObject as LibraryPropertyGetter;
				if(libraryGetProperty.isStatic) {
					return CreateStaticPropertyGetterCallNode(parentId, libraryGetProperty);
				}
				return CreatePropertyGetterCallNode(parentId, libraryGetProperty);
			}
			if(libraryObject is LibraryPropertySetter) {
				var librarySetProperty= libraryObject as LibraryPropertySetter;
				if(librarySetProperty.isStatic) {
					return CreateStaticPropertySetterCallNode(parentId, librarySetProperty);
				}
				return CreatePropertySetterCallNode(parentId, librarySetProperty);
			}
			if(libraryObject is LibraryEventHandler) {
				var libraryEventHandler= libraryObject as LibraryEventHandler;
				return CreateEventHandlerDefinitionNode(parentId, libraryEventHandler);
			}
			Debug.LogWarning("iCanScript: Unknown library type. Contact support.");
			return null;
		}

        // ----------------------------------------------------------------------
    	/// Creates a node that represents a static function.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryMethodInfo The library object of the function to create.
    	/// @return The newly created static function node.
    	///
        iCS_EditorObject CreateStaticFunctionCallNode(int parentId, LibraryMethodInfo libraryMethodInfo) {
			// -- Create base function node. --
			var instance= CreateBaseFunctionCallNode(parentId, libraryMethodInfo, VSObjectType.StaticFunction); 
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a non-static function.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryMethodInfo The library object of the function to create.
    	/// @return The newly created function node.
    	///
        iCS_EditorObject CreateFunctionCallNode(int parentId, LibraryMethodInfo libraryMethodInfo) {
			// -- Create base function node. --
			var instance= CreateBaseFunctionCallNode(parentId, libraryMethodInfo, VSObjectType.NonStaticFunction); 
    		// -- Create target & self ports. --
			var id= instance.InstanceId;
            CreateTargetPort(id);
            CreateSelfPort(id);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a common part of function node.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryMethodInfo The library object of the function to create.
		/// @param nodeType The visual script object type for this node.
    	/// @return The newly created function node.
    	///
        iCS_EditorObject CreateBaseFunctionCallNode(int parentId, LibraryMethodInfo libraryMethodInfo, VSObjectType nodeType) {
			// -- Create base node --
			var instance= CreateBaseNode(parentId, libraryMethodInfo, nodeType);
            // -- Create parameter ports. --
    		var parameters= libraryMethodInfo.parameters;
    		CreateParameterPorts(instance, parameters);
    		// -- Create return port. --
			CreateReturnPort(instance, libraryMethodInfo);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a static constructor.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryConstructor The library object of the constructor to create.
    	/// @return The newly created constructor node.
    	///
        iCS_EditorObject CreateStaticConstructorCallNode(int parentId, LibraryConstructor libraryConstructor) {
			// -- Create base constructor node --
			var instance= CreateBaseConstructorCallNode(parentId, libraryConstructor, VSObjectType.StaticConstructor);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a non-static constructor.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryConstructor The library object of the constructor to create.
    	/// @return The newly created constructor node.
    	///
        iCS_EditorObject CreateConstructorCallNode(int parentId, LibraryConstructor libraryConstructor) {
			// -- Create base constructor node --
			var instance= CreateBaseConstructorCallNode(parentId, libraryConstructor, VSObjectType.Constructor);
    		// -- Create return port (Self). --
			var declaringType= libraryConstructor.declaringType;
            CreatePort("Self", instance.InstanceId, declaringType, VSObjectType.OutFixDataPort, (int)iCS_PortIndex.Return);
    		return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates the common part of a constructor node.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryConstructor The library object of the constructor to create.
    	/// @return The newly created constructor node.
    	///
        iCS_EditorObject CreateBaseConstructorCallNode(int parentId, LibraryConstructor libraryConstructor, VSObjectType nodeType) {
			// -- Create base node --
			var instance= CreateBaseNode(parentId, libraryConstructor, nodeType);
            // -- Create parameter ports. --
    		var parameters= libraryConstructor.parameters;
    		CreateParameterPorts(instance, parameters);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a static field getter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryField The library object of the field getter to create.
    	/// @return The newly created field getter node.
    	///
        iCS_EditorObject CreateStaticFieldGetterCallNode(int parentId, LibraryFieldGetter libraryField) {
			// -- Create base field getter node --
			var instance= CreateBaseFieldGetterCallNode(parentId, libraryField, VSObjectType.StaticField);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a non-static field getter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryField The library object of the field getter to create.
    	/// @return The newly created field getter node.
    	///
        iCS_EditorObject CreateFieldGetterCallNode(int parentId, LibraryFieldGetter libraryField) {
			// -- Create base get field node --
			var instance= CreateBaseFieldGetterCallNode(parentId, libraryField, VSObjectType.NonStaticField);
    		// -- Create target & self ports. --
			var id= instance.InstanceId;
            CreateTargetPort(id);
            CreateSelfPort(id);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a base node that represents a static field getter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryField The library object of the field getter to create.
		/// @param nodeType The visual script object type for the field getter.
    	/// @return The newly created field getter node.
    	///
        iCS_EditorObject CreateBaseFieldGetterCallNode(int parentId, LibraryFieldGetter libraryField, VSObjectType nodeType) {
			// -- Create base node --
			var instance= CreateBaseNode(parentId, libraryField, nodeType);
    		// -- Create return port. --
			var fieldName= libraryField.fieldName;
            var fieldType= libraryField.fieldType;
            var portType = VSObjectType.OutFixDataPort;
            var portIndex= (int)iCS_PortIndex.Return;
            CreatePort(fieldName, instance.InstanceId, fieldType, portType, portIndex);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a static field setter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryField The library object of the field setter to create.
    	/// @return The newly created field setter node.
    	///
        iCS_EditorObject CreateStaticFieldSetterCallNode(int parentId, LibraryFieldSetter libraryField) {
            // -- Create base field setter node --
            var instance= CreateBaseFieldSetterCallNode(parentId, libraryField, VSObjectType.StaticField);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a non-static field setter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryField The library object of the field setter to create.
    	/// @return The newly created field setter node.
    	///
        iCS_EditorObject CreateFieldSetterCallNode(int parentId, LibraryFieldSetter libraryField) {
            // -- Create base field setter node --
            var instance= CreateBaseFieldSetterCallNode(parentId, libraryField, VSObjectType.NonStaticField);
    		// -- Create target & self ports. --
            var id= instance.InstanceId;
            CreateTargetPort(id);
            CreateSelfPort(id);        
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a base node that represents a field setter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryField The library object of the field setter to create.
        /// @param nodeType The visual script object type for the field setter.
    	/// @return The newly created field setter node.
    	///
        iCS_EditorObject CreateBaseFieldSetterCallNode(int parentId, LibraryFieldSetter libraryField, VSObjectType nodeType) {
            // -- Create base node --
            var instance= CreateBaseNode(parentId, libraryField, nodeType);
            // -- Create parameter ports. --
            var fieldName= libraryField.fieldName;
            var fieldType= libraryField.fieldType;
            var portType = VSObjectType.InFixDataPort;
            var portIndex= (int)iCS_PortIndex.ParametersStart;
            CreatePort(fieldName, instance.InstanceId, fieldType, portType, portIndex);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a static property setter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryProperty The library object of the property setter to create.
    	/// @return The newly property setter node.
    	///
        iCS_EditorObject CreateStaticPropertySetterCallNode(int parentId, LibraryPropertySetter libraryProperty) {
            // -- Create base field setter node --
            var instance= CreateBasePropertySetterCallNode(parentId, libraryProperty, VSObjectType.StaticFunction);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a non-static property setter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryProperty The library object of the property setter to create.
    	/// @return The newly created property setter node.
    	///
        iCS_EditorObject CreatePropertySetterCallNode(int parentId, LibraryPropertySetter libraryProperty) {
            // -- Create base field setter node --
            var instance= CreateBasePropertySetterCallNode(parentId, libraryProperty, VSObjectType.NonStaticFunction);
    		// -- Create target & self ports. --
            var id= instance.InstanceId;
            CreateTargetPort(id);
            CreateSelfPort(id);        
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a base node that represents a property setter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryProperty The library object of the property setter to create.
        /// @param nodeType The visual script object type for the property setter.
    	/// @return The newly created property setter node.
    	///
        iCS_EditorObject CreateBasePropertySetterCallNode(int parentId, LibraryPropertySetter libraryProperty, VSObjectType nodeType) {
            // -- Create base node --
            var instance= CreateBaseNode(parentId, libraryProperty, nodeType);
            // -- Create parameter ports. --
            var propertyName= libraryProperty.propertyName.Substring(4);
            var propertyType= libraryProperty.parameters[0].ParameterType;
            var portType = VSObjectType.InFixDataPort;
            var portIndex= (int)iCS_PortIndex.ParametersStart;
            CreatePort(propertyName, instance.InstanceId, propertyType, portType, portIndex);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a static property getter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryProperty The library object of the property getter to create.
    	/// @return The newly property getter node.
    	///
        iCS_EditorObject CreateStaticPropertyGetterCallNode(int parentId, LibraryPropertyGetter libraryProperty) {
            // -- Create base field getter node --
            var instance= CreateBasePropertyGetterCallNode(parentId, libraryProperty, VSObjectType.StaticFunction);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a non-static property getter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryProperty The library object of the property getter to create.
    	/// @return The newly created property getter node.
    	///
        iCS_EditorObject CreatePropertyGetterCallNode(int parentId, LibraryPropertyGetter libraryProperty) {
            // -- Create base field getter node --
            var instance= CreateBasePropertyGetterCallNode(parentId, libraryProperty, VSObjectType.NonStaticFunction);
    		// -- Create target & self ports. --
            var id= instance.InstanceId;
            CreateTargetPort(id);
            CreateSelfPort(id);        
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a base node that represents a property getter.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryProperty The library object of the property getter to create.
        /// @param nodeType The visual script object type for the property getter.
    	/// @return The newly created property getter node.
    	///
        iCS_EditorObject CreateBasePropertyGetterCallNode(int parentId, LibraryPropertyGetter libraryProperty, VSObjectType nodeType) {
            // -- Create base node --
            var instance= CreateBaseNode(parentId, libraryProperty, nodeType);
    		// -- Create return port. --
			CreateReturnPort(instance, libraryProperty);
            return instance;
        }
        // ----------------------------------------------------------------------
    	/// Creates a node that represents a Unity eventhandler.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param libraryEventHandler The library object of the event handler to create.
    	/// @return The newly created event handler node.
    	///
        iCS_EditorObject CreateEventHandlerDefinitionNode(int parentId, LibraryEventHandler libraryEventHandler) {
            // -- Grab next available ID --
            int id= GetNextAvailableId();
            // -- Create event handler node --
            var nodeName     = libraryEventHandler.nodeName;
    		var declaringType= libraryEventHandler.declaringType;
    		var objectType   = VSObjectType.InstanceMessage;
            var instance= iCS_EditorObject.CreateInstance(id, nodeName, declaringType, parentId, objectType, this);
            // -- Create target port. --
    		iCS_EditorObject port= null;
            port= CreateTargetPort(id);
            port.PortSpec= PortSpecification.Owner;
            // -- Create parameter ports --
			var parameterTypes= libraryEventHandler.parameterTypes;
			var parameterNames= libraryEventHandler.parameterNames;
			var nbOfParams= parameterTypes.Length;
            for(int parameterIdx= 0; parameterIdx < nbOfParams; ++parameterIdx) {
				var paramType= parameterTypes[parameterIdx];
				var paramName= parameterNames[parameterIdx];
                if(paramType != typeof(void)) {
                    VSObjectType portType= VSObjectType.InFixDataPort;
                    port= CreatePort(paramName, id, paramType, portType, (int)iCS_PortIndex.ParametersStart+parameterIdx);
                    port.Value= iCS_Types.DefaultValue(paramType);
                }
            }
            return instance;
        }    

    	// ----------------------------------------------------------------------
    	/// Creates a property wizard node for the given type.
    	///
    	/// @param parentId The id of the parent node.
    	/// @param type The programmatic type for the property wizard.
    	/// @return The newly created property wizard node.
    	///
    	public iCS_EditorObject CreatePropertyWizardNode(int parentId, Type type) {
            return CreatePackage(parentId, null, VSObjectType.Package, type);
    	}
    	// ----------------------------------------------------------------------
    	public iCS_EditorObject CreateObjectInstance(int parentId, string name, Type instanceType) {
            return CreatePackage(parentId, name, VSObjectType.Package, instanceType);
    	}
        // ----------------------------------------------------------------------
    	public iCS_EditorObject CreateInParameterPort(string name, int parentId, Type valueType, int index) {
    		return CreatePort(name, parentId, valueType, VSObjectType.InFixDataPort, index);
    	}
        // ----------------------------------------------------------------------
    	public iCS_EditorObject CreateOutParameterPort(string name, int parentId, Type valueType, int index) {
    		return CreatePort(name, parentId, valueType, VSObjectType.OutFixDataPort, index);
    	}
        // ----------------------------------------------------------------------
    	private iCS_EditorObject CreateParameterPort(string name, int parentId, Type valueType, VSObjectType portType, int index) {
    		if(index < (int)iCS_PortIndex.ParametersStart || index > (int)iCS_PortIndex.ParametersEnd) {
    			Debug.LogError("iCanScript: Invalid parameter port index: "+index);
    		}
    		return CreatePort(name, parentId, valueType, portType, index);
    	}
        // ----------------------------------------------------------------------
        Vector2 VisualEditorCenter() {
            iCS_VisualEditor editor= iCS_EditorController.FindVisualEditor();
            var center= editor == null ? Vector2.zero : editor.ViewportToGraph(editor.ViewportCenter);
    		return center;
        }
    }

}
