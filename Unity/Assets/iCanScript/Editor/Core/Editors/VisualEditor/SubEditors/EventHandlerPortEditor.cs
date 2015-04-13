using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Editor {

	public class EventHandlerPortEditor : PortEditor {
        // ===================================================================
        // BUILDER
        // -------------------------------------------------------------------
        /// Creates a port editor window at the given screen position.
        ///
        /// @param screenPosition The screen position where the editor
        ///                       should be displayed.
        ///
        public static new EditorWindow Create(iCS_EditorObject port, Vector2 screenPosition) {
            if(port == null) return null;
            var self= EventHandlerPortEditor.CreateInstance<EventHandlerPortEditor>();
            self.vsObject= port;
            self.title= "Event Handler Port Editor";
            self.ShowUtility();
            return self;
        }

        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        protected override void OnPortSpecificGUI() {
            // Edit the value of the port.
            EditPortValue();
            
            // Show port value type.
            EditPortValueType();
        }
                
	}

}