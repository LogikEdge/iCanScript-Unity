using UnityEngine;
using System;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ----------------------------------------------------------------------
        /// Performs a snaity check on the visual script object.
    	///
        /// @param serviceKey The service key to use when registering with the
        ///					  error controller.
        /// @return _true_ is return if object is sane. _false_ otherwise.
    	///
       	public bool SanityCheck(string serviceKey) {
    		var visualScript= IStorage.VisualScript;
           // Verify that the runtime portion still exists.
           if(IsKindOfFunction) {
               var runtimeType= RuntimeType;
               var memberInfo= runtimeType != null ? LibraryController.LibraryDatabase.GetLibraryType(runtimeType) : null;
               if(memberInfo == null && !GraphInfo.IsLocalType(this)) {
                   var message= "Unable to find the runtime code for "+FullName;
                   ErrorController.AddError(serviceKey, message, visualScript, InstanceId);
    			   return false;
               }
           }
    	   if(IsTargetPort) {
    		   if(ProducerPort == null || ProducerPort == this) {
    			   var parentNode= ParentNode;
    			   if(parentNode.IsKindOfFunction) {
    				   if(!IsOwner && !IsTypeVariable) {
    					   var message= "<color=red><b>Value</b></color> for <b>Target</b> port is not valid for node: <b>"+FullName+"</b>";
    					   ErrorController.AddError(serviceKey, message, visualScript, InstanceId);
    					   return false;				   	
    				   }
    				   else {
    					   if(parentNode.IsFunction("get_transform", "GameObject", "UnityEngine")) {
    						   var message= "When <b>'Target'</b> is set to <b>'Owner'</b>, the generated code has no effect.  The node can be removed safely: <b>"+ParentNode.FullName+"</b>";
    						   ErrorController.AddWarning(serviceKey, message, visualScript, ParentNode.InstanceId);
    					   }
    				   }
    			   }
    		   }
    	   }
    	   if(IsEnablePort) {
    		   var producerPort= SegmentProducerPort;
    		   if(producerPort != this) {
                   // -- Verify for unnecessary OR function --
    			   var producerNode= producerPort.ParentNode;
    			   if(producerNode.IsFunction("Or", "iCS_Boolean", "")) {
    				   if(producerPort.SegmentEndConsumerPorts.Length <= 1) {
    					   var message= "Consider using additional 'Enable(s)' ports instead of an OR function. => "+producerNode.FullName;
    					   ErrorController.AddWarning(serviceKey, message, visualScript, producerNode.InstanceId);
    					   ErrorController.AddWarning(serviceKey, message, visualScript, producerNode.InstanceId);				   	
    				   }
    			   }
                   // -- Verify for unnecessary trigger->enble binding when
                   //    data flow already exist --
                   var parentNode= ParentNode;
                   if(producerPort.IsTriggerPort &&
                      producerNode.IsKindOfFunction && parentNode.IsKindOfFunction) {
                       bool doesDataFlowExist= false;
                       parentNode.ForEachChildPort(
                           p=> {
                               if(p.IsInDataPort) {
                                   var pp= p.SegmentProducerPort;
                                   if(pp.ParentNode == producerNode) {
                                       doesDataFlowExist= true;
                                   }
                               }
                           }
                       );
                       if(doesDataFlowExist) {
                           var message= "<b>Trigger</b> to <b>Enable</b> connection not needed since data flow already exist between nodes.  Consider removing the control flow.";
                           ErrorController.AddWarning(serviceKey, message, visualScript, InstanceId);
     				       ErrorController.AddWarning(serviceKey, message, visualScript, producerPort.InstanceId);				   	
                       }
                       // -- Determine if self to target connection should be used for
                       //    sequencing operation on same object.
                       var producerNodeTargetPort= producerNode.TargetPort;
                       var parentNodeTargetPort  = parentNode.TargetPort;
                       if(producerNodeTargetPort != null && parentNodeTargetPort != null) {
                           var targetProducerPort= parentNodeTargetPort.SegmentProducerPort;
                           if(producerNodeTargetPort.SegmentProducerPort == targetProducerPort) {
                               var targetProducerNode= targetProducerPort.ParentNode;
                               var producerNodeSelfPort= producerNode.SelfPort;
                               var message= "Consider connecting the <b>Self</b> port of '<b>"+producerNode.DisplayName
                                            +"</b>' to the <b>Target</b> port of '<b>"+parentNode.DisplayName
                                            +"</b> to sequence operations on <b>"+targetProducerNode.DisplayName
                                            +"</b>.";
                               ErrorController.AddWarning(serviceKey, message, visualScript, parentNodeTargetPort.InstanceId);
                               ErrorController.AddWarning(serviceKey, message, visualScript, producerNodeSelfPort.InstanceId);
                           }
                       }
                   }
    		   }
    	   }
    	   return true;
        }
	
        // ----------------------------------------------------------------------
    	/// Determines if node corresponds to given specification.
    	///
    	/// @param funcName The function name.
    	/// @param typeName The type name of the function.
    	/// @param namespaceName The namespace of the type name.
    	/// @return _true_ if the object corresponds to the given specification.
    	///         _false_ otherwise.
    	///
    	public bool IsFunction(string funcName, string typeName, string namespaceName) {
    	   if(this.CodeName == funcName) {
    		   if(this.TypeName == typeName && this.Namespace == namespaceName) {
    			   return true;
    		   }
    	   }
    	   return false;
    	}

    }
}
