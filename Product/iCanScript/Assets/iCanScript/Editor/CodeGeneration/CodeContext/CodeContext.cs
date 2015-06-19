using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        CodeBase[]          myObjectToCodeTable= null;
        string              myServiceKey       = null;
        iCS_VisualScriptImp myVisualScript     = null;
        List<string>        myUsedNamespaces   = new List<string>();

        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Namespace in which to generate the code.
		public CodeBase[] ObjectToCodeTable {
			get { return myObjectToCodeTable; }
		}
        public List<string> UsedNamespaces {
            get { return myUsedNamespaces; }
        }

        // -------------------------------------------------------------------
        /// Builds a code context to share among all CodeBase of the same
        /// visual script.
        ///
        /// @param vsObjects A visual script object from which to extract the
        ///                  visual script storage.
        /// @return The newly created code context.
        ///
        public CodeContext(iCS_EditorObject vsObject) {
            // Allocate conversion tables.
            var iStorage= vsObject.IStorage;
            AllocateObjectToCodeTable(iStorage);

            // Clear any pending code generation error.
            myVisualScript= iStorage.VisualScript;
            myServiceKey= "C# Code Generation: "+NameUtility.ToTypeName(vsObject.CodeName);
            ErrorController.Clear(myServiceKey);
        }

        // -------------------------------------------------------------------
        /// Allocates the VS Object to Code translation table.
        ///
        /// @param iStorage The visual script storage.
        ///
		void AllocateObjectToCodeTable(iCS_IStorage iStorage) {
			if(myObjectToCodeTable != null) return;
            var visualScriptSize= iStorage.EditorObjects.Count;
            myObjectToCodeTable= new CodeBase[visualScriptSize];
            for(int i= 0; i < visualScriptSize; ++i) {
                myObjectToCodeTable[i]= null;
            }
		}

        // -------------------------------------------------------------------
        /// Registers an association between visual script object and this
        /// associated code.
        ///
        /// @param vsObject The visual script object.
        /// @param code     The associated code context.
        ///
        public void Register(iCS_EditorObject vsObject, CodeBase code) {
            if(vsObject != null) {
	            myObjectToCodeTable[vsObject.InstanceId]= code;            	
            }
        }

        // -------------------------------------------------------------------
        /// Returns the Code associated with the given visual script object.
        ///
        /// @param vsObject The visual script object.
        /// @return The associated Code or _'null'_ if not found.
        public CodeBase GetCodeFor(iCS_EditorObject vsObject) {
            return ObjectToCodeTable[vsObject.InstanceId];
        }

        // -------------------------------------------------------------------
		/// Returns the runtime type for the given visual script object.
		///
		/// @param vsObject The visual script object.
		/// @return The runtime type.
		public Type GetRuntimeTypeFor(iCS_EditorObject vsObject) {
			var code= GetCodeFor(vsObject);
			return code != null ? code.GetRuntimeType() : vsObject.RuntimeType;
		}
		
        // -------------------------------------------------------------------
        /// Adds a namespace to the used namespace container.
        ///
        /// @param namespaceName The name of the namespace to add.
        ///
        public void AddNamespace(string namespaceName) {
            if(string.IsNullOrEmpty(namespaceName)) return;
            if(!myUsedNamespaces.Exists((s1)=> s1 == namespaceName)) {
                myUsedNamespaces.Add(namespaceName);
            }
        }


		// ===================================================================
		// ERROR/WARNING MANAGEMENT UTILITIES
        // -------------------------------------------------------------------
        /// Register a code generation error message.
        ///
        /// @param message The error string message to be displayed.
        /// @param objectId The identifier of the visual script object in error.
        ///
        public void AddError(string message, int objectId) {
            ErrorController.AddError(myServiceKey, message, myVisualScript, objectId);
        }

        // -------------------------------------------------------------------
        /// Register a code generation warning message.
        ///
        /// @param message The error string message to be displayed.
        /// @param objectId The identifier of the visual script object in error.
        ///
        public void AddWarning(string message, int objectId) {
            ErrorController.AddWarning(myServiceKey, message, myVisualScript, objectId);
        }

        // -------------------------------------------------------------------
		/// Determines if errors are reported for the function call.
		public bool IsInError(int objectId) {
			return P.length(ErrorController.GetErrorsFor(myServiceKey, myVisualScript, objectId)) != 0;
		}

    }

}