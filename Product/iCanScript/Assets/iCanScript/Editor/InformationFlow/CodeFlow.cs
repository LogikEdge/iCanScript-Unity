using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    // ===================================================================
    /// This class implements code flow utilities.
    public static class CodeFlow {

    	// -------------------------------------------------------------------------
        /// Returns the producer port usable by the code.
        ///
        /// @param consumerPort The VS consumer port.
        /// @return The producer port usable by the code.
        ///
        public static iCS_EditorObject GetProducerPort(iCS_EditorObject consumerPort) {
            var producerPort= consumerPort.FirstProducerPort;
            // Follow the target/self port chain.
            while(producerPort.IsSelfPort) {
                producerPort= DataFlow.GetTargetPort(producerPort.ParentNode);
                producerPort= producerPort.FirstProducerPort;
            }
            while(producerPort.IsTriggerPort && producerPort.ParentNode.IsKindOfPackage) {
                var package= producerPort.ParentNode;
                var nestedFunctions= VSStructure.GetListOfFunctions(package);
                if(nestedFunctions.Count != 0) {
                    // FIXME: Need to create a trigger variable.
                    return producerPort;
                }
                var enables= ControlFlow.GetEnablePorts(package);
                switch(enables.Length) {
                    case 0: {
                        // FIXME: Should remove enable to nothing.
                        return producerPort;
                    }
                    case 1: {
                        return GetProducerPort(enables[0]);
                    }
                    default: {
                        // FIXME: Need to create a trigger variable.
                        return producerPort;
                    }
                }
            }
            return producerPort;
        }

    }
    
}
