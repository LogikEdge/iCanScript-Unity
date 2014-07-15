using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {    
    // =========================================================================
    // Trigger Port
    // -------------------------------------------------------------------------
    public iCS_EditorObject CreateTriggerPort(int parentId) {
        var existingTriggerPort= GetTriggerPort(EditorObjects[parentId]);
        if(existingTriggerPort != null) return existingTriggerPort;
        iCS_EditorObject port= CreatePort(iCS_Strings.TriggerPort, parentId, typeof(bool), iCS_ObjectTypeEnum.TriggerPort, (int)iCS_PortIndex.Trigger);
        port.IsNameEditable= false;
        port.InitialPortValue= true;
        return port;
    }
    // -------------------------------------------------------------------------
    public bool HasTriggerPort(iCS_EditorObject node)  {
        return GetTriggerPort(node) != null;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject GetTriggerPort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsTriggerPort);
    }

    // ======================================================================
    // Enable Ports
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateEnablePort(int parentId) {
        iCS_EditorObject port= CreatePort(iCS_Strings.EnablePort, parentId, typeof(bool), iCS_ObjectTypeEnum.EnablePort, (int)GetNextAvailableEnablePortIndex(EditorObjects[parentId]));
        port.IsNameEditable= false;
        port.InitialPortValue= true;
        return port;
    }
    // -------------------------------------------------------------------------
    public bool HasEnablePort(iCS_EditorObject package) {
        return GetEnablePorts(package).Length != 0;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject[] GetEnablePorts(iCS_EditorObject package) {
        return BuildFilteredListOfChildren(c=> c.IsEnablePort, package);
    }
    // -------------------------------------------------------------------------
    public int GetNextAvailableEnablePortIndex(iCS_EditorObject node) {
        var enables= CleanupEnablePorts(node);
        if(enables == null) return (int)iCS_PortIndex.EnablesStart;
        return (int)iCS_PortIndex.EnablesStart + enables.Length;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject[] CleanupEnablePorts(iCS_EditorObject node) {
        var enables= GetEnablePorts(node);
        if(enables == null || enables.Length == 0) return null;
        for(int i= 0; i < enables.Length; ++i) {
            enables[i].PortIndex= (int)iCS_PortIndex.EnablesStart+i;
        }
        return enables;
    }
	
	
    // =========================================================================
    // Input This Port
    // -------------------------------------------------------------------------
	public iCS_EditorObject CreateInInstancePort(int parentId, Type runtimeType) {
		var port= CreatePort(GetInstancePortName(runtimeType), parentId, runtimeType,
							 iCS_ObjectTypeEnum.InFixDataPort, (int)iCS_PortIndex.InInstance);
		port.IsNameEditable= false;
		return port;
	}

    // -------------------------------------------------------------------------
    public bool HasInInstancePort(iCS_EditorObject node)  {
        return GetInInstancePort(node) != null;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject GetInInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsInInstancePort);
    }


    // =========================================================================
    // Output This Port
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateOutInstancePort(int parentId, Type runtimeType) {
        iCS_EditorObject port= CreatePort(GetInstancePortName(runtimeType), parentId, runtimeType,
                                          iCS_ObjectTypeEnum.OutProposedDataPort, (int)iCS_PortIndex.OutInstance);
        port.IsNameEditable= false;
        return port;
    }
    // -------------------------------------------------------------------------
    public bool HasOutInstancePort(iCS_EditorObject node)  {
        return GetOutInstancePort(node) != null;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject GetOutInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsOutInstancePort);
    }
    
	
    // ======================================================================
    // Dynamic Ports
    // ----------------------------------------------------------------------
	public iCS_EditorObject CreateInDynamicDataPort(string name, int parentId, Type valueType) {
		var parent= EditorObjects[parentId];
		int index= GetNextDynamicOrProposedPortIndex(parent);
		return CreatePort(name, parentId, valueType, iCS_ObjectTypeEnum.InDynamicDataPort, index);
	}
    // ----------------------------------------------------------------------
	public iCS_EditorObject CreateOutDynamicDataPort(string name, int parentId, Type valueType) {
		var parent= EditorObjects[parentId];
		int index= GetNextDynamicOrProposedPortIndex(parent);
		return CreatePort(name, parentId, valueType, iCS_ObjectTypeEnum.OutDynamicDataPort, index);		
	}


    // ======================================================================
    // Proposed Ports
    // ----------------------------------------------------------------------
	public iCS_EditorObject CreateInProposedDataPort(string name, int parentId, Type valueType) {
		var parent= EditorObjects[parentId];
		int index= GetNextDynamicOrProposedPortIndex(parent);
		return CreatePort(name, parentId, valueType, iCS_ObjectTypeEnum.InProposedDataPort, index);
	}
    // ----------------------------------------------------------------------
	public iCS_EditorObject CreateOutProposedDataPort(string name, int parentId, Type valueType) {
		var parent= EditorObjects[parentId];
		int index= GetNextDynamicOrProposedPortIndex(parent);
		return CreatePort(name, parentId, valueType, iCS_ObjectTypeEnum.OutProposedDataPort, index);		
	}


    // ======================================================================
    // Common Creation
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreatePort(string name, int parentId, Type valueType, iCS_ObjectTypeEnum portType, int index= -1) {
        int id= GetNextAvailableId();
        var parent= EditorObjects[parentId];
        if(index == -1) {
            if(portType == iCS_ObjectTypeEnum.TriggerPort) {
                index= (int)iCS_PortIndex.Trigger;
            } if(portType == iCS_ObjectTypeEnum.EnablePort) {
                index= GetNextAvailableEnablePortIndex(parent);
            }
            else {
        		index= GetNextDynamicOrProposedPortIndex(parent);                
            }
        }
        iCS_EditorObject port= iCS_EditorObject.CreateInstance(id, name, valueType, parentId, portType, this);
        port.PortIndex= index;
        if(parent.IsPort) {
//            port.LocalOffset= parent.LocalOffset;
            port.CollisionOffset= parent.CollisionOffset;
        } else {
            var globalPos= parent.GlobalPosition;
//    		port.GlobalPosition= globalPos;            
            port.CollisionOffsetFromGlobalPosition= globalPos;
        }
		// Set initial port edge.
		if(port.IsEnablePort) {
			port.Edge= iCS_EdgeEnum.Top;
		} else if(port.IsTriggerPort) {
			port.Edge= iCS_EdgeEnum.Bottom;
		} else if(port.IsInputPort) {
			port.Edge= iCS_EdgeEnum.Left;
		} else if(port.IsDataPort) {
			port.Edge= iCS_EdgeEnum.Right;
		} else {
			port.UpdatePortEdge();			
		}
		port.CleanupPortEdgePosition();
        return EditorObjects[id];        
    }


    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public void MoveDynamicPortToLastIndex(iCS_EditorObject port) {
		// Relocate the port at the end.
        iCS_EditorObject parent= port.Parent;
		port.PortIndex= RecalculatePortIndexes(parent).Length;
        // Rearrange port indexes
        RecalculatePortIndexes(parent);
    }

	// ======================================================================
	// High-order functions
    // ----------------------------------------------------------------------
	public iCS_EditorObject[] RecalculatePortIndexes(iCS_EditorObject node) {
		List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
		// Get all child data ports.
		ForEachChildDataPort(node, child=> ports.Add(child));
		// Sort child ports according to index.
		iCS_EditorObject[] result= SortPortsOnIndex(ports.ToArray());
		// Find first dynamic or proposed port
		int firstDynamicIdx= 0;
		for(int i= 0; i < result.Length; ++i) {
		    if(result[i].IsParameterPort && result[i].IsFixDataPort) {
		        firstDynamicIdx= result[i].PortIndex+1;
		    }
		}
		// Re-index dynamic and proposed ports
        for(int i= 0; i < result.Length; ++i) {
            if(result[i].IsDynamicDataPort || result[i].IsProposedDataPort) {
                result[i].PortIndex= firstDynamicIdx++;                
            }
        }
		return result;
	}
    // -------------------------------------------------------------------------
    public int GetNextDynamicOrProposedPortIndex(iCS_EditorObject node) {
        var ports= RecalculatePortIndexes(node);
        int lastIdx= 0;
        for(int i= 0; i < ports.Length; ++i) {
            var p= ports[i];
            if(p.IsDynamicDataPort || p.IsProposedDataPort) {
                lastIdx= p.PortIndex+1;
            }
        }
        return lastIdx;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject FindInputInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsInInstancePort);
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject FindOutputInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsOutInstancePort);
    }
    // ----------------------------------------------------------------------
    public static iCS_EditorObject[] SortPortsOnIndex(iCS_EditorObject[] lst) {
        // Find largest dynamic or proposed port.
        int lastIndex= -1;
        foreach(var p in lst) {
			if(p.IsDynamicDataPort || p.IsProposedDataPort) {
	            if(p.PortIndex > lastIndex) {
	                lastIndex= p.PortIndex;
	            }				
			}
        }
        // Assign all unassigned port indexes (we assume that it is a dynmic port).
        if(lastIndex != -1) {
            foreach(var p in lst) {
                if(p.PortIndex < 0) {
                    p.PortIndex= ++lastIndex;
                }
            }
        }
		Array.Sort(lst, (x,y)=> x.PortIndex - y.PortIndex);
        return lst;
    }

}
