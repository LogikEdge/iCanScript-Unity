using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Engine {
    
    // ==========================================================================
    // The iCS_VisualScriptData class is divided into an instance section and
    // a utility section.
    //
    // The instance section can be used to create mementos of the visual
    // script data for caching and duplication purposes.
    //
    // The Utility section consists of class function used to manipulate the
    // visual script data.
    //
    public partial class iCS_VisualScriptData : iCS_IVisualScriptData {
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //               VISUAL SCRIPT DATA INSTANCE SECTION
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        public bool                     IsEditorScript        = false;
        public string                   CSharpFileName        = null;
        public bool                     BaseTypeOverride      = false;
        public string                   BaseType              = null;
        public string                   SourceFileGUID        = null;
        public int			            MajorVersion          = iCS_Config.MajorVersion;
        public int    		            MinorVersion          = iCS_Config.MinorVersion;
        public int    		            BugFixVersion         = iCS_Config.BugFixVersion;
        public int                      DisplayRoot           = -1;	
    	public int    		            SelectedObject        = -1;
        public Vector2                  SelectedObjectPosition= Vector2.zero;
    	public bool                     ShowDisplayRootNode   = true;
    	public float  		            GuiScale              = 1f;	
    	public Vector2		            ScrollPosition        = Vector2.zero;
        public int                      UndoRedoId            = 0;
        public List<iCS_EngineObject>   EngineObjects         = new List<iCS_EngineObject>();
        public iCS_NavigationHistory    NavigationHistory     = new iCS_NavigationHistory();
    

        // ======================================================================
        // Visual Script Data Interface Implementation
        // ----------------------------------------------------------------------
        bool iCS_IVisualScriptData.IsEditorScript {
            get { return IsEditorScript; }
            set { IsEditorScript= value; }
        }
        string iCS_IVisualScriptData.CSharpFileName {
            get { return CSharpFileName; }
            set { CSharpFileName= value; }
        }
        bool iCS_IVisualScriptData.BaseTypeOverride {
            get { return BaseTypeOverride; }
            set { BaseTypeOverride= value; }
        }
        string iCS_IVisualScriptData.BaseType {
            get { return BaseType; }
            set { BaseType= value; }
        }
        string iCS_IVisualScriptData.SourceFileGUID {
            get { return SourceFileGUID; }
            set { SourceFileGUID= value; }
        }
        int iCS_IVisualScriptData.MajorVersion {
            get { return MajorVersion; }
            set { MajorVersion= value; }
        }
        int iCS_IVisualScriptData.MinorVersion {
            get { return MinorVersion; }
            set { MinorVersion= value; }
        }
        int iCS_IVisualScriptData.BugFixVersion {
            get { return BugFixVersion; }
            set { BugFixVersion= value; }
        }
        List<iCS_EngineObject>  iCS_IVisualScriptData.EngineObjects {
            get { return EngineObjects; }
        }
        int iCS_IVisualScriptData.UndoRedoId {
            get { return UndoRedoId; }
            set { UndoRedoId= value; }
        }
    
        // ======================================================================
        // Visual Script Editor Data Interface Implementation
        // ----------------------------------------------------------------------
        int iCS_IVisualScriptData.DisplayRoot {
            get { return DisplayRoot; }
            set { DisplayRoot= value; }
        }
        int iCS_IVisualScriptData.SelectedObject {
            get { return SelectedObject; }
            set { SelectedObject= value; }
        }
        Vector2 iCS_IVisualScriptData.SelectedObjectPosition {
            get { return SelectedObjectPosition; }
            set { SelectedObjectPosition= value; }
        }
        bool iCS_IVisualScriptData.ShowDisplayRootNode {
            get { return ShowDisplayRootNode; }
            set { ShowDisplayRootNode= value; }
        }
        float iCS_IVisualScriptData.GuiScale {
            get { return GuiScale; }
            set { GuiScale= value; }
        }
        Vector2 iCS_IVisualScriptData.ScrollPosition {
            get { return ScrollPosition; }
            set { ScrollPosition= value; }
        }
        iCS_NavigationHistory iCS_IVisualScriptData.NavigationHistory {
            get { return NavigationHistory; }
        }

        // ======================================================================
        // Builders
        // ----------------------------------------------------------------------
        public iCS_VisualScriptData(/*UnityEngine.Object host*/) {
    //        HostObject= host;
        }
        public iCS_VisualScriptData(/*UnityEngine.Object host, */iCS_IVisualScriptData vsd) {
    //        HostObject= host;
            iCS_VisualScriptData.CopyFromTo(vsd, this);
        }

        // ======================================================================
        // Instance Utility functions.
        // ----------------------------------------------------------------------
        public bool IsValidEngineObject(int id) {
            return IsValidEngineObject(this, id);
    	}

        // ======================================================================
        // Duplication Utilities
        // ----------------------------------------------------------------------
        public void CopyTo(iCS_IVisualScriptData to) {
            CopyFromTo(this, to);
        }

        // ----------------------------------------------------------------------
        public void CopyDataTo(iCS_IVisualScriptData to) {
            CopyDataFromTo(this, to);
        }
        // ----------------------------------------------------------------------
        public void CopyEditorDataTo(iCS_IVisualScriptData to) {
            CopyEditorDataFromTo(this, to);
        }
    
        // ======================================================================
        // Tree Navigation Queries
        // ----------------------------------------------------------------------
        public iCS_EngineObject GetParent(iCS_EngineObject child) {
            return GetParent(this, child);
        }
        // ----------------------------------------------------------------------
    	public iCS_EngineObject GetParentNode(iCS_EngineObject child) {
            return GetParentNode(this, child);
    	}
        // ----------------------------------------------------------------------
    	public string GetFullName(UnityEngine.Object host, iCS_EngineObject obj) {
            return GetFullName(this, host, obj);
    	}
	
        // ======================================================================
        // Connection Queries
        // ----------------------------------------------------------------------
        // Returns the immediate source of the port.
        public iCS_EngineObject GetProducerPort(iCS_EngineObject port) {
            return GetProducerPort(this, port);
        }
        // ----------------------------------------------------------------------
        // Returns the endport source of a connection.
        public iCS_EngineObject GetSegmentProducerPort(iCS_EngineObject port) {
            return GetSegmentProducerPort(this, port);
        }
        // ----------------------------------------------------------------------
        public bool IsEndPort(iCS_EngineObject port) {
            return IsEndPort(this, port);
        }
        // ----------------------------------------------------------------------
        public bool IsRelayPort(iCS_EngineObject port) {
            return IsRelayPort(this, port);
        }
    

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //               VISUAL SCRIPT DATA UTILITY SECTION
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    
        // ======================================================================
        // Initialize / Destroy
        // ----------------------------------------------------------------------
        public static void DestroyEngineObject(iCS_IVisualScriptData vsd, iCS_EngineObject toDelete) {
            // -- Disconnect all connected ports --
            if(toDelete.IsPort) {
                int id= toDelete.InstanceId;
                if(id != -1) {
                    FilterWith(o=> o.IsPort && o.SourceId == id, p=> p.SourceId= -1, vsd);
                }
            }
            // -- Destroy the instance --
            toDelete.DestroyInstance();
        }
        // ----------------------------------------------------------------------
        public static void AddEngineObject(iCS_IVisualScriptData vsd, iCS_EngineObject toAdd) {
            // Try to find an available empty slot.
            int emptySlot= P.findFirst(o=> o.InstanceId == -1, (i,o)=> i, -1, vsd.EngineObjects);
        
            // Grow engine object array if no free slot exists.
            if(emptySlot != -1) {
    			toAdd.InstanceId= emptySlot;
                vsd.EngineObjects[emptySlot]= toAdd;
                return;
            }
    		toAdd.InstanceId= P.length(vsd.EngineObjects);
            vsd.EngineObjects.Add(toAdd);
        }
    
        // ======================================================================
        // Queries
        // ----------------------------------------------------------------------
        public static bool IsValidEngineObject(iCS_IVisualScriptData vsd, int id) {
            var engineObjects= vsd.EngineObjects;
    		return id >= 0 && id < engineObjects.Count && engineObjects[id].InstanceId != -1;
    	}

        // ======================================================================
        // Duplication Utilities
        // ----------------------------------------------------------------------
        public static void CopyFromTo(iCS_IVisualScriptData from, iCS_IVisualScriptData to) {
            CopyDataFromTo(from, to);
            CopyEditorDataFromTo(from, to);
        }

        // ----------------------------------------------------------------------
        public static void CopyDataFromTo(iCS_IVisualScriptData from, iCS_IVisualScriptData to) {
            to.IsEditorScript   = from.IsEditorScript;
            to.CSharpFileName   = from.CSharpFileName;
            to.BaseTypeOverride = from.BaseTypeOverride;
            to.BaseType         = from.BaseType;
            to.SourceFileGUID   = from.SourceFileGUID;
            to.MajorVersion     = from.MajorVersion;
            to.MinorVersion     = from.MinorVersion;
            to.BugFixVersion    = from.BugFixVersion;
            to.UndoRedoId       = from.UndoRedoId;
        
            // Resize destination engine object array.
            var fromEngineObjects= from.EngineObjects;
            var toEngineObjects= to.EngineObjects;
            int fromLen= fromEngineObjects.Count;
            int toLen= toEngineObjects.Count;
            if(toLen > fromLen) {
                toEngineObjects.RemoveRange(fromLen, toLen-fromLen);
            }
            toEngineObjects.Capacity= fromLen;
            // Copy engine objects.
            for(int i= 0; i < fromLen; ++i) {
                var fromObj= fromEngineObjects[i];
                if(fromObj == null) fromObj= iCS_EngineObject.CreateInvalidInstance();
                if(toEngineObjects.Count <= i) {
                    toEngineObjects.Add(fromObj.Clone());
                }
                else if(toEngineObjects[i] == null) {
                    toEngineObjects[i]= fromObj.Clone();                
                }
                else {
                    toEngineObjects[i]= fromObj.CopyTo(toEngineObjects[i]);                                
                }
            }            
        }
        // ----------------------------------------------------------------------
        public static void CopyEditorDataFromTo(iCS_IVisualScriptData from, iCS_IVisualScriptData to) {
            to.ShowDisplayRootNode   = from.ShowDisplayRootNode;
            to.ScrollPosition        = from.ScrollPosition;
            to.GuiScale              = from.GuiScale;
            to.SelectedObject        = from.SelectedObject;
            to.SelectedObjectPosition= from.SelectedObjectPosition;
            to.DisplayRoot           = from.DisplayRoot;
            // Copy navigation history
            to.NavigationHistory.CopyFrom(from.NavigationHistory);                    
        }
    
        // ======================================================================
        // Tree Navigation Queries
        // ----------------------------------------------------------------------
        public static iCS_EngineObject GetParent(iCS_IVisualScriptData vsd, iCS_EngineObject child) {
            if(child == null || child.ParentId == -1) return null;
            return vsd.EngineObjects[child.ParentId]; 
        }
        // ----------------------------------------------------------------------
    	public static iCS_EngineObject GetParentNode(iCS_IVisualScriptData vsd, iCS_EngineObject child) {
    		var parentNode= GetParent(vsd, child);
    		while(parentNode != null && !parentNode.IsNode) {
    			parentNode= GetParent(vsd, parentNode);
    		}
    		return parentNode;
    	}
        // ----------------------------------------------------------------------
        /// Returns the full name of the object including the name of the game object.
    	public static string GetFullName(iCS_IVisualScriptData vsd, UnityEngine.Object host, iCS_EngineObject obj) {
    		return "/"+host.name+"/"+GetRelativeName(vsd, obj);
    	}
        // ----------------------------------------------------------------------
        /// Returns the relative name of the object reference by the visual script.
    	public static string GetRelativeName(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
    		if(obj == null) return "";
    		string fullName= "";
    		for(; obj != null; obj= GetParentNode(vsd, obj)) {
                if( !obj.IsBehaviour ) {
        			fullName= obj.RawName+(string.IsNullOrEmpty(fullName) ? "" : "/"+fullName);                
                }
    		}
    		return fullName;
    	}
    
        // ======================================================================
        // Connection Queries
        // ----------------------------------------------------------------------
        // Returns the immediate source of the port.
        public static iCS_EngineObject GetProducerPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
            if(port == null || port.SourceId == -1) return null;
            return vsd.EngineObjects[port.SourceId];
        }
        // ----------------------------------------------------------------------
        // Returns the endport source of a connection.
        public static iCS_EngineObject GetSegmentProducerPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
            if(port == null || port.InstanceId == -1) return null;
            int linkLength= 0;
            for(iCS_EngineObject sourcePort= GetProducerPort(vsd, port); sourcePort != null; sourcePort= GetProducerPort(vsd, port)) {
                port= sourcePort;
                if(++linkLength > 100) {
                    Debug.LogWarning("iCanScript: Circular port connection detected on: "+GetParentNode(vsd, port).RawName+"."+port.RawName);
                    return null;                
                }
            }
            return IsValid(port, vsd) ? port : null;
        }
        // ----------------------------------------------------------------------
        // Returns the list of consumer ports.
        public static iCS_EngineObject[] GetConsumerPorts(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
            if(port == null) return new iCS_EngineObject[0];
            var consumerPorts= new List<iCS_EngineObject>();
            var engineObjects= vsd.EngineObjects;
            foreach(var obj in engineObjects) {
                if(obj.IsPort && GetProducerPort(vsd, obj) == port) {
                    consumerPorts.Add(obj);
                }
            }
            return consumerPorts.ToArray();
        }
        // ----------------------------------------------------------------------
        public static bool IsEndPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
            if(port == null) return false;
            if(!HasASource(vsd, port)) return true;
            return !HasADestination(vsd, port);
        }
        // ----------------------------------------------------------------------
        public static bool IsRelayPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
            if(port == null) return false;
            return HasASource(vsd, port) && HasADestination(vsd, port);
        }
        // ----------------------------------------------------------------------
        public static bool HasASource(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
            var source= GetProducerPort(vsd, port);
            return source != null && source != port; 
        }
        // ----------------------------------------------------------------------
        public static bool HasADestination(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
            return P.length(GetConsumerPorts(vsd, port)) != 0;
        }
    
        // ======================================================================
        // General Queries
        // ----------------------------------------------------------------------
        public static iCS_EngineObject[] GetChildPorts(iCS_IVisualScriptData vsd, iCS_EngineObject node) {
            List<iCS_EngineObject> childPorts= new List<iCS_EngineObject>();
            FilterWith(p=> p.IsPort && p.ParentId == node.InstanceId, childPorts.Add, vsd);
            return childPorts.ToArray();
        }
    }

}
