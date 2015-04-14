using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        CodeBase[]  myObjectToCodeTable= null;

        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Namespace in which to generate the code.
		public CodeBase[] ObjectToCodeTable {
			get { return myObjectToCodeTable; }
		}

        // -------------------------------------------------------------------
        /// Builds a code context to share among all CodeBase of the same
        /// visual script.
        ///
        /// @param vsObjects A visual script object from which to extract the
        ///                  vsual script storage.
        /// @return The newly created code context.
        ///
        public CodeContext(iCS_EditorObject vsObject) {
            AllocateObjectToCodeTable(vsObject.IStorage);
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
            if(vsObject == null) return;
            myObjectToCodeTable[vsObject.InstanceId]= code;
        }

        // -------------------------------------------------------------------
        /// Returns the Code associated with the given visual script object.
        ///
        /// @param vsObject The visual script object.
        /// @return The associated Code or _'null'_ if not found.
        public CodeBase GetCodeFor(iCS_EditorObject vsObject) {
            return ObjectToCodeTable[vsObject.InstanceId];
        }
    }

}