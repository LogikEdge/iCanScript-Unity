using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ----------------------------------------------------------------------
    public void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_TypeCastInfo conversion= null) {
		iCS_EditorObject inParentNode  = inPort.ParentNode;
        iCS_EditorObject outParentNode = outPort.ParentNode;
        iCS_EditorObject inGrandParent = inParentNode.ParentNode;        
        iCS_EditorObject outGrandParent= outParentNode.ParentNode;

        // No need to create module ports if both connected nodes are under the same parent.
        if(inGrandParent == outGrandParent || inGrandParent == outParentNode || inParentNode == outGrandParent) {
            SetSource(inPort, outPort, conversion);
            OptimizeDataConnection(inPort, outPort);
            return;
        }
        // Create inPort if inParent is not part of the outParent hierarchy.
        bool inParentSeen= false;
        for(iCS_EditorObject op= outGrandParent.ParentNode; op != null; op= op.ParentNode) {
            if(inGrandParent == op) {
                inParentSeen= true;
                break;
            }
        }
        if(!inParentSeen && inGrandParent != null) {
            iCS_EditorObject newPort= CreatePort(outPort.Name, inGrandParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicDataPort);
            SetSource(inPort, newPort, conversion);
			SetBestPositionForAutocreatedPort(newPort, inPort.LayoutPosition, outPort.LayoutPosition);
            SetNewDataConnection(newPort, outPort);
            OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Create outPort if outParent is not part of the inParent hierarchy.
        bool outParentSeen= false;
        for(iCS_EditorObject ip= inGrandParent.ParentNode; ip != null; ip= ip.ParentNode) {
            if(outGrandParent == ip) {
                outParentSeen= true;
                break;
            }
        }
        if(!outParentSeen && outGrandParent != null) {
            iCS_EditorObject newPort= CreatePort(outPort.Name, outGrandParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
            SetSource(newPort, outPort, conversion);
			SetBestPositionForAutocreatedPort(newPort, inPort.LayoutPosition, outPort.LayoutPosition);
            SetNewDataConnection(inPort, newPort);
            OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        SetSource(inPort, outPort, conversion);
        OptimizeDataConnection(inPort, outPort);
    }
	// ----------------------------------------------------------------------
	// This attempt to properly locate an autocreated data port.
	public void SetBestPositionForAutocreatedPort(iCS_EditorObject port, Vector2 inPortPosition, Vector2 outPortPosition) {
		// Determine the parent edge position to use.
		var parent= port.Parent;
		var parentGlobalRect= parent.LayoutRect;
		float x= port.IsInputPort ? parentGlobalRect.xMin : parentGlobalRect.xMax;
		// Assure that the in position X value is smaller then the out position.
		if(inPortPosition.x > outPortPosition.x) {
			var tmp= inPortPosition; inPortPosition= outPortPosition; outPortPosition= tmp;
		}
		// Manage situation where new port is between the in & out ports.
		var parentGlobalPosition= Math3D.Middle(parentGlobalRect);
		var top   = parentGlobalPosition.y+parent.VerticalPortsTop;
		var bottom= parentGlobalPosition.y+parent.VerticalPortsBottom;
		float y;
		if(Math3D.IsSmaller(inPortPosition.x, x) && Math3D.IsGreater(outPortPosition.x, x)) {
			float ratio= (x-inPortPosition.x)/(outPortPosition.x-inPortPosition.x);
			y= Math3D.Lerp(inPortPosition.y, outPortPosition.y, ratio);
			if(y < top) { 
				y= top;
			}
			if(y > bottom) {
				y= bottom;
			}
			port.SetAnchorAndLayoutPosition(new Vector2(x,y));
			return;
		}
		if(Math3D.IsEqual(inPortPosition.y, outPortPosition.y)) {
			port.SetAnchorAndLayoutPosition(new Vector2(x, 0.5f*(top+bottom)));
			return;
		}
		// Assure that the in position Y value is smaller then the out position.
		if(inPortPosition.y > outPortPosition.y) {
			var tmp= inPortPosition; inPortPosition= outPortPosition; outPortPosition= tmp;
		}
		// Compute some type of ratio if Y position traverse the top port position
		if(Math3D.IsSmaller(inPortPosition.y, top) && Math3D.IsGreater(outPortPosition.y, top)) {
			float y1= outPortPosition.y-top;
			float y2= top-inPortPosition.y;
			y= top+(y1*y1/(y1+y2));
		} else {
			float y2= outPortPosition.y-bottom;
			float y1= bottom-inPortPosition.y;
			y= bottom-(y1*y1/(y1+y2));			
		}
		port.SetAnchorAndLayoutPosition(new Vector2(x,y));
		return;			
	}
}
