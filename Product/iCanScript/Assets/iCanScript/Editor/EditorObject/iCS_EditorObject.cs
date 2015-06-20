using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public partial class iCS_EditorObject {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        iCS_IStorage    myIStorage       = null;
        int             myId             = -1;
        bool            myIsFloating     = false;
        List<int>		myChildren       = new List<int>();
        bool            myIsSticky       = false;

        // ======================================================================
        // Cache
        // ----------------------------------------------------------------------
        Type    c_RuntimeType     = null;
        string  c_NodeTitle       = null;
        Vector2 c_NodeTitleSize   = Vector2.zero;
        string  c_NodeSubTitle    = null;
        Vector2 c_NodeSubTitleSize= Vector2.zero;
        string  c_CodeName        = null;
        string  c_DisplayName     = null;

        // ======================================================================
        // Conversion Utilities
        // ----------------------------------------------------------------------
        public iCS_IStorage IStorage {
            get { return myIStorage; }
        }
    	public iCS_VisualScriptData Storage {
    		get { return myIStorage.Storage; }
    	}
        public iCS_MonoBehaviourImp iCSMonoBehaviour {
            get { return myIStorage.iCSMonoBehaviour; }
        }
        public List<iCS_EditorObject> EditorObjects {
            get { return myIStorage.EditorObjects; }
        }
        public iCS_EditorObject EditorObject {
    		get { return EditorObjects[myId]; }
    	}
        public List<iCS_EngineObject> EngineObjects {
            get { return myIStorage.EngineObjects; }
        }
        public iCS_EngineObject EngineObject {
    		get { return EngineObjects[myId]; }
    	}
        List<iCS_EngineObject> EditorToEngineList(List<iCS_EditorObject> editorObjects) {
            return Prelude.map(eo=> (eo != null ? eo.EngineObject : null), editorObjects);
        }
    
        // ======================================================================
        // Engine Object Accessors
        // ----------------------------------------------------------------------
        public VSObjectType ObjectType {
    		get { return EngineObject.ObjectType; }
    		set {
                var engineObject= EngineObject;
                if(engineObject.ObjectType == value) return;
    		    engineObject.ObjectType= value;
    		}
    	}
        // ----------------------------------------------------------------------
        public int ParentId {
    		get { return EngineObject.ParentId; }
    		set {
    			int pid= EngineObject.ParentId;
    			if(pid == value) return;
    			if(IsIdValid(pid)) {
    				var oldParent= EditorObjects[pid];
    				oldParent.RemoveChild(this);
    			}
    			EngineObject.ParentId= value;
    			if(IsIdValid(value)) {
    				var newParent= EditorObjects[value];
    				newParent.AddChild(this);
    			}
    		}
    	}
        // ----------------------------------------------------------------------
        public iCS_DisplayOptionEnum DisplayOption {
            get { return EngineObject.DisplayOption; }
            set {
                var engineObject= EngineObject;
                if(engineObject.DisplayOption == value) return;
                engineObject.DisplayOption= value;
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the port type of this object.
        public PortSpecification PortSpec {
            set { EngineObject.PortSpec= value; }
            get { return EngineObject.PortSpec; }
        }
        // ----------------------------------------------------------------------
        /// Returns the node type of this object.
        public NodeSpecification NodeSpec {
            get { return EngineObject.NodeSpec; }
            set { EngineObject.NodeSpec= value; }
        }
        // ----------------------------------------------------------------------
    	/// Returns the runtime type of the visual script object.
        public Type RuntimeType {
    		get {
                if(c_RuntimeType == null) {
                    c_RuntimeType= EngineObject.RuntimeType;
					// -- Convert language primitive types. --
					if(c_RuntimeType == typeof(CSharp.Primitives.Int)) {
						c_RuntimeType= typeof(int);
					}
					if(c_RuntimeType == typeof(CSharp.Primitives.Float)) {
						c_RuntimeType= typeof(float);
					}
					if(c_RuntimeType == typeof(CSharp.Primitives.Bool)) {
						c_RuntimeType= typeof(bool);
					}
					if(c_RuntimeType == typeof(CSharp.Primitives.String)) {
						c_RuntimeType= typeof(String);
					}
					if(c_RuntimeType == typeof(CSharp.Primitives.Object)) {
						c_RuntimeType= typeof(System.Object);
					}
                    c_RuntimeType= iCS_Types.RemoveRefOrPointer(c_RuntimeType);
                }
                return c_RuntimeType;
            }
    	}

        // ----------------------------------------------------------------------
    	/// Returns the qualified type name of the visual script object.
    	public string QualifiedTypeName {
    		get {
    			var assemblyQualifiedName= EngineObject.QualifiedType;
    			int size= assemblyQualifiedName.IndexOf(',');
    			if(size < 0 || size >= assemblyQualifiedName.Length) {
    				return assemblyQualifiedName;
    			}
    			return assemblyQualifiedName.Substring(0, size);
    		}
    	}
	
        // ----------------------------------------------------------------------
    	/// Returns the type name from the qualified type.
    	public string TypeName {
    		get {
    			var qualifiedTypeName= QualifiedTypeName;
    			int start= qualifiedTypeName.LastIndexOf('.')+1;
    			if(start < 0 || start >= qualifiedTypeName.Length) {
    				return qualifiedTypeName;
    			}
    			int len= qualifiedTypeName.Length;
    			return qualifiedTypeName.Substring(start, len-start);
    		}
    	}
	
        // ----------------------------------------------------------------------
    	/// Returns the type name from the qualified type.
    	public string Namespace {
    		get {
    			var qualifiedTypeName= QualifiedTypeName;
    			int size= qualifiedTypeName.LastIndexOf('.');
    			if(size < 0 || size >= qualifiedTypeName.Length) {
    				return "";
    			}
    			return qualifiedTypeName.Substring(0, size);			
    		}
    	}
	
        // ----------------------------------------------------------------------
    	/// Returns true if the runtime type of this visual object is included
    	/// the the given type.
    	///
    	/// @param parentType The type to verify against.
    	/// @return _true_ if the object runtime type is included in the given
    	///         parent type.  _false_ otherwise.
    	///
    	public bool IsIncludedInType(Type parentType) {
    		return iCS_Types.IsA(RuntimeType, parentType);
    	}
	
        // ======================================================================
        // ----------------------------------------------------------------------
        /// Returns the name as per the underlying code.
        public string CodeName {
            get {
                if(c_CodeName == null) {
    				var name= EngineObject.RawName;
                    if(IsPort) {
                        if(IsDataPort && IsProgrammaticInstancePort) {
                            if(ParentNode.IsConstructor) {
                                // Use the name of the variable for constructor output.
                                c_CodeName= NameUtility.ToLowerCamelCase(ParentNode.CodeName);
                            }
                            else {
                                // -- Accept a variable name for Target ports. --
                                c_CodeName= "this";
                                if(IsTargetPort && IsTypeVariable) {
                                    if(!String.IsNullOrEmpty(name)) {
                                        c_CodeName= NameUtility.ToLowerCamelCase(name);                                        
                                    }
                                }
                            }
                        }
                        else if(IsTriggerPort) {
                        	c_CodeName= "trigger";
                        }
    					else if(IsEnablePort) {
    						c_CodeName= "enable";
    					}
    					else {
                            c_CodeName= EngineObject.RawName;
							// -- Try to use the original name if the user erased the port name. --
    						if(string.IsNullOrEmpty(c_CodeName)) {
    							var parent= ParentNode;
    							if(parent != null) {
    		                        var libraryMethodBase= GetLibraryObject() as LibraryMethodBase;
    								if(libraryMethodBase != null) {
										var parameters= libraryMethodBase.parameters;
										if(parameters != null && PortIndex < parameters.Length) {
											c_CodeName= NameUtility.ToFunctionParameterName(parameters[PortIndex].Name);
										}
										else if(IsReturnPort) {
											c_CodeName= NameUtility.ToLocalVariableName(parent.CodeName);
										}
    								}
    							}								
    						}                       
    						if(string.IsNullOrEmpty(c_CodeName)) {
    							c_CodeName= IsReturnPort ? "output" : "p";
    						}
                        }
                    }
                    else if(IsNode) {
    					c_CodeName= name;
                        if(IsBehaviour) {
                            var endIdx= name.IndexOf(':');
                            if(endIdx != -1) {
                                name= name.Substring(0, endIdx);
                            }
                            c_CodeName= NameUtility.ToTypeName(name);
                        }
                        else if(IsConstructor) {
                            c_CodeName= iCS_Types.TypeName(RuntimeType);
                        }
    					else if(IsTransitionPackage) {
                            c_CodeName= string.IsNullOrEmpty(name) ? "StateTransition" : name;						
    					}
                        else if(IsStateChart) {
                            c_CodeName= string.IsNullOrEmpty(name) ? "StateChart" : name;
                        }
                        else if(IsState) {
                            c_CodeName= string.IsNullOrEmpty(name) ? "State" : name;
                        }
                        else if(IsPackage) {
                            c_CodeName= string.IsNullOrEmpty(name) ? "Package" : name;
                        }
                        else {
                            var methodName= EngineObject.MethodName;
                            if(!string.IsNullOrEmpty(methodName)) {
                                c_CodeName= methodName;
                            }
                            else {
                                c_CodeName= name;
                            }
                            if(string.IsNullOrEmpty(c_CodeName)) {
                                c_CodeName= "null";
                            }
                        }
                    }
                    else {
                        c_CodeName= string.IsNullOrEmpty(name) ? "null" : name;
                    }
                }
                return c_CodeName ?? "null";
            }
        }
        // ======================================================================
        // ----------------------------------------------------------------------
        /// Returns the user visible name of the object.
        public string DisplayName {
            get {
                if(c_DisplayName == null) {
                    if(IsDataPort && IsProgrammaticInstancePort) {
                        if(IsOutputPort || ParentNode.IsEventHandler || ParentNode.IsFunctionDefinition) {
                            c_DisplayName= "Self";
                        }
                        else {
                            c_DisplayName= "Target";
                            var rawName= EngineObject.RawName;
                            if(IsTypeVariable) {
                                if(!string.IsNullOrEmpty(rawName)) {
                                    if(!(string.Compare(rawName, "this", true) == 0 || string.Compare(rawName, "target", true) == 0)){
                                        c_DisplayName+= "/"+rawName;
                                    }
                                }
                            }
                        }
                    }
                    else if(IsInstanceNode) {
    					var typeName= NameUtility.ToTypeName(iCS_Types.TypeName(RuntimeType));
                        c_DisplayName= typeName+" Properties";
                    }
                    else {
                        c_DisplayName= EngineObject.RawName;
                        if(string.IsNullOrEmpty(c_DisplayName)) {
                            c_DisplayName= CodeName;
                        }
                    }
                    c_DisplayName= NameUtility.ToDisplayName(c_DisplayName);
                }
                return c_DisplayName;
            }
            set {
                var engineObject= EngineObject;
                if(engineObject.RawName == value) return;
    		    engineObject.RawName= value;
                ResetNameCaches();
            }
        }

        // ----------------------------------------------------------------------
        public string FullName {
            get { return Storage.GetFullName(iCSMonoBehaviour, EngineObject); }
        }
        // ----------------------------------------------------------------------
        /// This functions resets all name related caches.
        void ResetNameCaches() {
    		c_CodeName       = null;
            c_NodeTitle      = null;
            c_NodeTitleSize  = Vector2.zero;
            c_DisplayName    = null;
        }
        // ----------------------------------------------------------------------
        public bool IsNameEditable {
    		get {
                if(IsBehaviour) return false;
                if(IsEventHandler) return false;
                if(IsTargetPort || IsSelfPort) return false;
                if(IsInstanceNode) return false;
                if(IsDataPort && ParentNode.IsInstanceNode) return false;
                if(IsStatePort || IsTransitionPort) return false;
                if(IsOnStatePackage) return false;
                return true;
            }
    	}
        // ----------------------------------------------------------------------
        public string Description {
    		get { return EngineObject.Description; }
    		set {
                var engineObject= EngineObject;
                if(engineObject.Description == value) return;
    		    engineObject.Description= value;
    		}
    	}
        // ----------------------------------------------------------------------
        /// Returns the node title string.
        ///
        /// The frame ID is appended to the node title string if the application
        /// is running and show frame ID option is selected.
        ///
        public string NodeTitle {
            get {
                // Fill the node title cache.
                if(c_NodeTitle == null) {
                    c_NodeTitle= NameUtility.ToDisplayName(DisplayName);
                }
                // Return editor node title.
                return c_NodeTitle;
            }
        }
        // ----------------------------------------------------------------------
        public Vector2 NodeTitleSize {
            get {
                if(Math3D.IsZero(c_NodeTitleSize)) {
                    var titleContent= new GUIContent(NodeTitle);
                    c_NodeTitleSize= iCS_Layout.DefaultTitleStyle.CalcSize(titleContent);
                }
                return c_NodeTitleSize;
            }
        }

        // ----------------------------------------------------------------------
        /// Builds and returns the Node SubTitle text.
        public string NodeSubTitle {
            get {
                if(c_NodeSubTitle == null) {
                    c_NodeSubTitleSize= Vector2.zero;
                    if(IsConstructor) {
                        c_NodeSubTitle= BuildIsASubTitle("Self", RuntimeType);
                    }
                    else if(IsEventHandler || IsFunctionDefinition) {
                        c_NodeSubTitle= "Self is a "+NameUtility.ToDisplayName(EditorObjects[0].DisplayName);                    
                    }
                    else if(IsKindOfFunction || IsInstanceNode) {
                        c_NodeSubTitle= BuildIsASubTitle("Target", RuntimeType);
                    }
                    else if(IsKindOfPackage) {
                        c_NodeSubTitle= "Node is a Package";
                    }
                    else if(IsInlineCode) {
                        c_NodeSubTitle= BuildIsASubTitle("Node", RuntimeType);
                    }
                    else {
                        c_NodeSubTitle= null;
                    }
                }
                return c_NodeSubTitle ?? "";
            }
        }

        // ----------------------------------------------------------------------
        /// Builds a standard IsA type of node subtitle string.
        ///
        /// @param name The name to be used that _"is a"_.
        /// @param type The type of the _"is a"_.
        ///
        string BuildIsASubTitle(string name, Type type) {
            var result= new StringBuilder(name, 64);
            result.Append(" is a");
            var typeName= NameUtility.ToDisplayName(iCS_Types.TypeName(type));
            if(iCS_TextUtility.StartsWithAVowel(typeName)) {
                result.Append('n');
            }
            result.Append(" ");
            result.Append(typeName);
            return result.ToString();        
        }

        // ----------------------------------------------------------------------
        /// Returns the rendering dimension of the Node SubTitle text.
        public Vector2 NodeSubTitleSize {
            get {
                if(Math3D.IsZero(c_NodeSubTitleSize)) {
                    var guiContent= new GUIContent(NodeSubTitle);
                    c_NodeSubTitleSize= iCS_Layout.DefaultSubTitleStyle.CalcSize(guiContent);
                }
                return c_NodeSubTitleSize;
            }
        }    
        
        // ======================================================================
        // High-Level Properties
        // ----------------------------------------------------------------------
    	public bool IsIdValid(int id)	{ return id >= 0 && id < EditorObjects.Count && id < EngineObjects.Count && EditorObjects[id] != null; }
    	public bool	IsParentValid		{ get { return IsIdValid(ParentId); }}
    	public bool IsSourceValid		{ get { return IsIdValid(ProducerPortId); }}
        public bool IsSelected          { get { return myIStorage.SelectedObject == this; }}

    	public bool IsValid {
    		get { return IsIdValid(InstanceId); }
    	}
        public int InstanceId { 
    		get { return myId; }
    	}
        public iCS_EditorObject Parent {
    		get { return IsParentValid ? myIStorage[ParentId] : null; }
    		set { ParentId= (value != null ? value.InstanceId : -1); }
    	}
    	public iCS_EditorObject ParentNode {
    	    get {
    	        var parent= Parent;
    	        while(parent != null && !parent.IsNode) parent= parent.Parent;
    	        return parent;
    	    }
    	}
    	public iCS_EditorObject ParentTypeNode {
    		get {
    			return EditorObjects[0];
    		}
    	}
        public bool IsFloating {
    		get { return myIsFloating; }
    		set {
                if(myIsFloating == value) return;
    		    myIsFloating= value;
    		}
    	}
    	public bool IsParentFloating {
    		get {
    			for( var parent= ParentNode; parent != null; parent= parent.ParentNode)  {
    				if( parent.IsFloating ) return true;
    			}
    			return false;
    		}
    	}
    	public bool IsSticky {
    	    get { return myIsSticky; }
    	    set { myIsSticky= value; }
    	}
    	public bool IsDisplayRoot {
    	    get {
                return this.InstanceId == Storage.DisplayRoot;
    	    }
    	}
	
        // ======================================================================
        // Constructors/Builders
        // ----------------------------------------------------------------------
    	// Creates an instance of an editor/engine object pair.
    	public static iCS_EditorObject CreateInstance(int id, string name, Type type,
    												  int parentId, VSObjectType objectType,
                                					  iCS_IStorage iStorage) {
    		if(id < 0) return null;
    		// Create engine object.
    		var engineObject= new iCS_EngineObject(id, name, type, parentId, objectType);
    		AddEngineObject(id, engineObject, iStorage);
    		// Create editor object.
    		var editorObject= new iCS_EditorObject(id, iStorage);
    		AddEditorObject(id, editorObject);
            RunOnCreated(editorObject);
    		return editorObject;
    	}
        // ----------------------------------------------------------------------
        // Duplicate the given editor object with a new id and parent.
        public static iCS_EditorObject Clone(int id, iCS_EditorObject toClone, iCS_EditorObject parent,
                                             iCS_IStorage iStorage) {
    		if(id < 0) return null;
    		// Create engine object.
            var engineObject= iCS_EngineObject.Clone(id, toClone.EngineObject,
    												 (parent == null ? null : parent.EngineObject));
    		AddEngineObject(id, engineObject, iStorage);
    		// Create editor object.
    		var editorObject= new iCS_EditorObject(id, iStorage);
    		AddEditorObject(id, editorObject);
            editorObject.LocalSize= toClone.LocalSize;
            RunOnCreated(editorObject);
            editorObject.Value= toClone.Value;
            return editorObject;
        }

        // ----------------------------------------------------------------------
        // Reinitialize the editor object to its default values.
        public void DestroyInstance() {
            // Invoke event
            RunOnWillDestroy(this);
            // Destroy any children.
            ForEachChild(child=> child.DestroyInstance());        
            // Disconnect any port sourcing from this object.
            if(IsPort) {
                myIStorage.ForEach(
                    child=> {
                        if(child.IsPort && child.ProducerPortId == InstanceId) {
                            child.ProducerPortId= -1;
                        }                    
                    }
                );
            }
    		// Update child lists.
    		if(IsParentValid) {
    			Parent.RemoveChild(this);
    		}
            // Assure that the selected object is not us.
            if(myIStorage.SelectedObject == EditorObject) myIStorage.SelectedObject= null;
    		// Reset the associated engine object.
            EngineObject.DestroyInstance();
            // Remove editor object.
            EditorObjects[myId]= null;
        }
    
        // ======================================================================
    	// Grow database if needed.
        // ----------------------------------------------------------------------
    	private static void AddEngineObject(int id, iCS_EngineObject engineObject, iCS_IStorage iStorage) {
    		int len= iStorage.EngineObjects.Count;
    		if(id < 0) return;
    		if (id < len) {
    			iStorage.EngineObjects[id]= engineObject;
    			return;
    		}
    		if(id == len) {
    			iStorage.EngineObjects.Add(engineObject);
    			return;
    		}
    		if(id > len) {
    			GrowEngineObjectList(id, iStorage);
    			iStorage.EngineObjects.Add(engineObject);
    		}		
    	}
        // ----------------------------------------------------------------------
    	private static void AddEditorObject(int id, iCS_EditorObject editorObject) {
    		var iStorage= editorObject.myIStorage;
    		int len= iStorage.EditorObjects.Count;
    		if(id < 0) return;
    		if (id < len) {
    			iStorage.EditorObjects[id]= editorObject;
    			return;
    		}
    		if(id == len) {
    			iStorage.EditorObjects.Add(editorObject);
    			return;
    		}
    		if(id > len) {
    			GrowEditorObjectList(id, iStorage);
    			iStorage.EditorObjects.Add(editorObject);
    		}		
    	}
    	private static void GrowEngineObjectList(int size, iCS_IStorage iStorage) {
            // Reserve space to contain the total amount of objects.
            if(size > iStorage.EngineObjects.Capacity) {
                iStorage.EngineObjects.Capacity= size;
            }
            // Add the number of missing objects.
            for(int len= iStorage.EngineObjects.Count; size > len; ++len) {
                iStorage.EngineObjects.Add(iCS_EngineObject.CreateInvalidInstance());
            }
    	}
    	private static void GrowEditorObjectList(int size, iCS_IStorage iStorage) {
            // Reserve space to contain the total amount of objects.
            if(size > iStorage.EditorObjects.Capacity) {
                iStorage.EditorObjects.Capacity= size;
            }
            // Add the number of missing objects.
            for(int len= iStorage.EditorObjects.Count; size > len; ++len) {
                iStorage.EditorObjects.Add(null);
            }
    	}
    
        // ======================================================================
    	// Rebuild from engine database.
        // ----------------------------------------------------------------------
        public iCS_EditorObject(int id, iCS_IStorage iStorage) {
            myIStorage= iStorage;
            myId= id;
    		var parent= Parent;
    		if(parent != null) parent.AddChild(this);
        }
        // ----------------------------------------------------------------------
    	public static void RebuildFromEngineObjects(iCS_IStorage iStorage) {
    		iStorage.EditorObjects.Clear();
    		iStorage.EditorObjects.Capacity= iStorage.EngineObjects.Count;		
    		for(int i= 0; i < iStorage.EngineObjects.Count; ++i) {
                iCS_EditorObject editorObj= null;
                var engineObj= iStorage.EngineObjects[i];
    		    if(iCS_VisualScriptData.IsValid(engineObj, iStorage.EngineStorage)) {
    		        editorObj= new iCS_EditorObject(i, iStorage);
    		    }
    	        iStorage.EditorObjects.Add(editorObj);
    		}
    		RebuildChildrenLists(iStorage);
    	}
        // ----------------------------------------------------------------------
    	private static void RebuildChildrenLists(iCS_IStorage iStorage) {
    		iStorage.ForEach(
    		    obj=> {
        			if(obj.IsParentValid) {
        				obj.Parent.AddChild(obj);
        			}					        
    		    }
    		);
    	}

        // ======================================================================
        // Child container management.
        // ----------------------------------------------------------------------
    	public List<int> Children { get { return myChildren; }}
        // ----------------------------------------------------------------------
        public void AddChild(iCS_EditorObject toAdd) {
            int id= toAdd.InstanceId;
            if(Prelude.elem(id, myChildren.ToArray())) return;
            myChildren.Add(id);
        }
        // ----------------------------------------------------------------------
        public void RemoveChild(iCS_EditorObject toDelete) {
            int id= toDelete.InstanceId;
            for(int i= 0; i < myChildren.Count; ++i) {
                if(myChildren[i] == id) {
                    myChildren.RemoveAt(i);
                    return;
                }
            }
        }
        // ----------------------------------------------------------------------
        public bool AreChildrenInSameOrder(int[] orderedChildren) {
            int i= 0;
            for(int j= 0; j < Children.Count; ++j) {
                if(Children[j] == orderedChildren[i]) {
                    if(++i >= orderedChildren.Length) return true;
                };
            }
            return false;
        }
        // ----------------------------------------------------------------------
        public void ReorderChildren(int[] orderedChildren) {
            if(AreChildrenInSameOrder(orderedChildren)) return;
            List<int> others= Prelude.filter(c=> Prelude.notElem(c,orderedChildren), Children);
            int i= 0;
            Prelude.forEach(c=> Children[i++]= c, orderedChildren);
            Prelude.forEach(c=> Children[i++]= c, others);
        }
        // ----------------------------------------------------------------------
		/// Find the library object that matches this editor object.
		///
		/// @return The library object that match this visual script object. _null_ otherwise.
		///
		public LibraryObject GetLibraryObject() {
			// -- Only fields, properties, functions, and types are in the library. --
			if(IsPort) return null;
			// -- Locate the declaring type --
			var libraryType= LibraryController.LibraryDatabase.GetLibraryType(RuntimeType);
			if(libraryType == null) return null;
			// -- Try field first --
			var methodName= EngineObject.MethodName;
			if(IsFieldGet) {
				foreach(var field in libraryType.GetMembers<LibraryFieldGetter>()) {
					if(field.fieldName == methodName) {
						return field;
					}
				}				
			}
			if(IsFieldSet) {
				foreach(var field in libraryType.GetMembers<LibraryFieldSetter>()) {
					if(field.fieldName == methodName) {
						return field;
					}
				}
			}
			// -- Try properties --
			if(IsPropertyGet) {
				foreach(var property in libraryType.GetMembers<LibraryPropertyGetter>()) {
					if(property.propertyName == methodName) {
						return property;
					}
				}				
			}
			if(IsPropertySet) {
				foreach(var property in libraryType.GetMembers<LibraryPropertySetter>()) {
					if(property.propertyName == methodName) {
						return property;
					}
				}
			}
			// -- Try constructors --
			var parameterPorts= GetParameterPorts();
			var nbOfParameters= parameterPorts.Length;
			if(IsConstructor) {
				foreach(var constructor in libraryType.GetMembers<LibraryConstructor>()) {
					var constructorParameters= constructor.parameters;
					if(nbOfParameters == constructorParameters.Length) {
						bool sameParamTypes= true;
						for(int i= 0; i < nbOfParameters; ++i) {
							var portType = iCS_Types.RemoveRefOrPointer(parameterPorts[i].RuntimeType);
							var paramType= iCS_Types.RemoveRefOrPointer(constructorParameters[i].ParameterType);
							if(portType != paramType) {
								sameParamTypes= false;
								break;
							}
						}
						if(sameParamTypes) {
							return constructor;							
						}
					}
				}				
			}
			// -- Try functions --
			if(IsNonStaticFunction || IsStaticFunction) {
				foreach(var function in libraryType.GetMembers<LibraryFunction>()) {
					if(function.functionName == methodName) {
						var functionParameters= function.parameters;
						if(nbOfParameters == functionParameters.Length) {
							bool sameParamTypes= true;
							for(int i= 0; i < nbOfParameters; ++i) {
								var portType = iCS_Types.RemoveRefOrPointer(parameterPorts[i].RuntimeType);
								var paramType= iCS_Types.RemoveRefOrPointer(functionParameters[i].ParameterType);
								if(portType != paramType) {
									sameParamTypes= false;
									break;
								}
							}
							if(sameParamTypes) {
								return function;						
							}
						}
					}
				}				
			}
			return null;
		}
    }
}

