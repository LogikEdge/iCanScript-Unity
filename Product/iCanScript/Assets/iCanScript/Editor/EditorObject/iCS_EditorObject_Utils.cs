using UnityEngine;
using System;
using System.Collections;
using iCanScript.Editor;
using iCanScript.Engine;

public partial class iCS_EditorObject {
    // =====================================================================
    // Edition Utility
    // ----------------------------------------------------------------------
	public bool CanHavePackageAsParent() {
		if(IsPort || IsBehaviour || IsState || IsMessage || IsOnStatePackage) {
			return false;
		}
		return true;
	}
    // ----------------------------------------------------------------------
    public bool CanBeDeleted() {
        if(IsBehaviour || IsFixDataPort) return false;
        return true;
    }
	
    // =====================================================================
    // Layout utilites
    // ----------------------------------------------------------------------
    // Adds a margin around the given rectangle
    static Rect AddMargins(Rect r) {
        var m= iCS_EditorConfig.MarginSize;
        var m2= 2f*m;
        return new Rect(r.x-m, r.y-m, r.width+m2, r.height+m2);
    }
    // ----------------------------------------------------------------------
    // Returns true if a collision exists in the given node array.
    static bool DoesCollide(iCS_EditorObject[] nodes) {
       var len= nodes.Length;
       Rect[] rs= new Rect[len];
       for(int i= 0; i < len; ++ i) {
           rs[i]= nodes[i].AnimatedRect;
       }
       for(int i= 0; i < len-1; ++i) {
           var r1= AddMargins(rs[i]);
           for(int j= i+1; j < len; ++j) {
               if(Math3D.DoesCollide(r1, rs[j])) {
                   return true;
               }
           }
       }
       return false;
    } 

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
           var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(this);
           if(memberInfo == null && !IStorage.IsLocalType(this)) {
               var message= "Unable to find the runtime code for "+FullName;
               ErrorController.AddError(serviceKey, message, visualScript, InstanceId);
			   return false;
           }
		   if(IsObsolete) {
			   var message= CodeName+" is obsolete. "+ObsoleteMessage;
			   ErrorController.AddWarning(serviceKey, message, visualScript, InstanceId);
		   }
       }
	   if(IsTargetPort) {
		   if(ProducerPort == null || ProducerPort == this) {
			   var parentNode= ParentNode;
			   if(parentNode.IsKindOfFunction) {
				   if(!(InitialValue is OwnerTag)) {
					   var message= "Value for Target port is not valid: "+FullName;
					   ErrorController.AddError(serviceKey, message, visualScript, InstanceId);
					   return false;				   	
				   }
				   else {
					   if(parentNode.IsFunction("get_transform", "GameObject", "UnityEngine")) {
						   var message= "When 'Target' is set to 'Owner', the generated code has no effect.  The node can be removed safely. => "+ParentNode.FullName;
						   ErrorController.AddWarning(serviceKey, message, visualScript, ParentNode.InstanceId);
					   }
				   }
			   }
		   }
	   }
	   if(IsEnablePort) {
		   var producerPort= ProducerPort;
		   if(producerPort != null) {
			   var parentNode= producerPort.ParentNode;
			   if(parentNode.IsFunction("Or", "iCS_Boolean", "")) {
				   if(producerPort.EndConsumerPorts.Length <= 1) {
					   var message= "Consider using additional 'Enable(s)' ports instead of an OR function. => "+parentNode.FullName;
					   ErrorController.AddWarning(serviceKey, message, visualScript, parentNode.InstanceId);
					   ErrorController.AddWarning(serviceKey, message, visualScript, ParentNode.InstanceId);				   	
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
