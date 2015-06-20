using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    // ===================================================================
    /// This class implements control flow utilities.
    public static class ControlFlow {

        // ===================================================================
        // ENABLE PORTS
    	// -------------------------------------------------------------------------
        /// Returns the list of all enable ports on the given node.
        ///
        /// @param node The node from which to extract the enable ports.
        /// @return The list of enable ports.
        ///
        public static iCS_EditorObject[] GetEnablePorts(iCS_EditorObject node) {
            var enables= new List<iCS_EditorObject>();
            node.ForEachChildPort(
                p=> {
                    if(p.IsEnablePort) {
                        enables.Add(p);
                    }
                }
            );
            return enables.ToArray();
        }
    	// -------------------------------------------------------------------------
        /// Determines if the node is disabled in editor mode.
        ///
        /// @param node The node to verify.
        /// @return _true_ if the node is disable in editor mode. _false_ otherwise.
        ///
        public static bool IsDisabledInEditorMode(iCS_EditorObject node) {
            var enablePorts= GetEnablePorts(node);
            if(enablePorts.Length != 0) {
                if(IsAllEnablesAlwaysFalse(enablePorts)) {
                    return true;
                }
            }
            var parentNode= node.ParentNode;
            if(parentNode == null) return false;
            return IsDisabledInEditorMode(parentNode);
        }
    	// -------------------------------------------------------------------------
        /// Returns the list of enable ports that affects the function call
        ///
        /// @param funcNode Visual script representing the function call.
        /// @return Array of all enable ports that affects the function call.
        ///
        public static iCS_EditorObject[] GetAllRelatedEnablePorts(iCS_EditorObject funcNode) {
            // -- Gather all enable ports --
            var enablePorts= new List<iCS_EditorObject>();
            while(funcNode != null) {
                var enables= GetEnablePorts(funcNode);
                if(enables.Length != 0 && !IsAtLeastOneEnableAlwaysTrue(enables)) {
                    enables= P.filter(e=> !IsEnableAlwaysFalse(e), enables);
                    enablePorts.AddRange(enables);
                }
                funcNode= funcNode.ParentNode;                    
            }
            // -- Reorder ports starting from parent --
            enablePorts.Reverse();
            return enablePorts.ToArray();
        }        
    	// -------------------------------------------------------------------------
        /// Determines if the enable port is always _true_.
        ///
        /// @param enablePort The enable port.
        /// @return _true_ if the enable is always true. _false_ otherwise.
        ///
        public static bool IsEnableAlwaysTrue(iCS_EditorObject enablePort) {
            var producerPort= GraphInfo.GetProducerPort(enablePort);
            if(producerPort.IsInputPort) {
                var initialValue= producerPort.Value;
                if(initialValue is UndefinedTag) return false;
                var value= (bool)initialValue;
                return value == true;
            }  
            return false;
        }
    	// -------------------------------------------------------------------------
        /// Determines if the enable port is always _false_.
        ///
        /// @param enablePort The enable port.
        /// @return _true_ if the enable is always false. _false_ otherwise.
        ///
        public static bool IsEnableAlwaysFalse(iCS_EditorObject enablePort) {
            var producerPort= GraphInfo.GetProducerPort(enablePort);
            if(producerPort.IsInputPort) {
                var initialValue= producerPort.Value;
                if(initialValue is UndefinedTag || initialValue == null) return false;
                var value= (bool)initialValue;
                return value == false;
            }  
            return false;          
        }
    	// -------------------------------------------------------------------------
        /// Determines if one of the enable ports is always true.
        ///
        /// @param enablePorts An array of enable ports.
        /// @return _true_ if at least one enable is always _true_. _false_ otherwise.
        ///
        public static bool IsAtLeastOneEnableAlwaysTrue(iCS_EditorObject[] enablePorts) {
            foreach(var e in enablePorts) {
                if(ControlFlow.IsEnableAlwaysTrue(e)) {
                    return true;
                }
            }
            return false;
        }
    	// -------------------------------------------------------------------------
        /// Determines if all enable ports are always false.
        ///
        /// @param enablePorts An array of enable ports.
        /// @return _true_ if all enables are always false. _false_ otherwise.
        ///
        public static bool IsAllEnablesAlwaysFalse(iCS_EditorObject[] enablePorts) {
            foreach(var e in enablePorts) {
                if(!ControlFlow.IsEnableAlwaysFalse(e)) {
                    return false;
                }
            }
            return true;
        }
        

        // ===================================================================
        // TRIGGER PORTS
    	// -------------------------------------------------------------------------
        /// Finds the trigger port associated with the givne node.
        ///
        /// @param node Visual script node to serach for a trigger port.
        /// @return The trigger port or _null_ if not found.
        ///
        public static iCS_EditorObject GetTriggerPort(iCS_EditorObject node) {
            iCS_EditorObject triggerPort= null;
            node.ForEachChild(p=> { if(p.IsTriggerPort) triggerPort= p; });
            return triggerPort;
        }

    }    
}
