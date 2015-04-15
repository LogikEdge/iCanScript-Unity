using UnityEngine;
using System;
using System.Collections;
using iCanScript.Editor;

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
           if(memberInfo == null) {
               var message= "Unable to find the runtime code for "+FullName;
               ErrorController.AddError(serviceKey, message, visualScript, InstanceId);
			   return false;
           }
       }
	   if(IsInInstancePort) {
		   if(ProducerPort == null || ProducerPort == this) {
			   var parentNode= ParentNode;
			   if(parentNode.IsKindOfFunction && !isDefinedInType(null, typeof(MonoBehaviour))) {
				   if(GetParentTypeName() != iCS_ObjectNames.ToTypeName(iCS_Types.TypeName(parentNode.RuntimeType))) {
					   var message= "Value for Target port is not valid: "+FullName;
					   ErrorController.AddError(serviceKey, message, visualScript, InstanceId);
					   return false;				   	
				   }
			   }
		   }
	   }
	   return true;
    }

    // ----------------------------------------------------------------------
	/// Returns the parent type name.
	string GetParentTypeName() {
		var typeNode= GetParentTypeNode();
		if(typeNode == null) return "";
		return iCS_ObjectNames.ToTypeName(typeNode.CodeName);
	}
	
    // ----------------------------------------------------------------------
	/// Returns the parent type node.
	iCS_EditorObject GetParentTypeNode() {
		// TODO: To be changed once nested type nodes are supported.
		return EditorObjects[0];
	}

    // ----------------------------------------------------------------------
	/// Determines if the vsObject refers to an element of the given type.
	///
	/// @param ourType The type defined by this visual script.
	/// @param baseType The type this visual script derives from.
	/// @param vsObj The object on which the search occurs.
	///
	public bool isDefinedInType(Type ourType, Type baseType) {
		var derivedType= ourType;
		if(derivedType == null) {
			derivedType= baseType;
			if(derivedType == null) return false;
		}
		var typeToSearch= RuntimeType;
		if(typeToSearch == null)  return false;
		if(derivedType == typeToSearch) return true;
		return iCS_Types.IsA(typeToSearch, derivedType);
	}

}
