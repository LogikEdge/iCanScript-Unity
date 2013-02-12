using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    public float NodeTitleHeight {
        get {
            if(IsIconizedOnDisplay) return 0;
            return 0.75f*iCS_EditorConfig.NodeTitleHeight;
        }
    }
    // ----------------------------------------------------------------------
	public float NodeTitleWidth {
		get {
			if(IsIconizedOnDisplay) return 0;
			return iCS_EditorConfig.GetNodeTitleWidth(Name)+2f*iCS_BuiltinTextures.kMinimizeIconSize+iCS_EditorConfig.kTitleFontSize;
		}
	}
    // ----------------------------------------------------------------------
    public float NodeTopPadding {
        get {
            if(IsIconizedOnDisplay) return 0;
            return NodeTitleHeight+iCS_EditorConfig.PaddingSize;
        }
    }
    // ----------------------------------------------------------------------
    public float NodeBottomPadding {
        get {
            if(IsIconizedOnDisplay) return 0;
            return iCS_EditorConfig.PaddingSize;            
        }
    }
    // ----------------------------------------------------------------------
    public float NodeLeftPadding {
        get {
            if(IsIconizedOnDisplay) return 0;
            float paddingBy2= 0.5f*iCS_EditorConfig.PaddingSize;
            float leftPadding= iCS_EditorConfig.PaddingSize;
            ForEachLeftChildPort(
                port=> {
                    if(!port.IsStatePort && port.IsPortOnParentEdge) {
                        Vector2 labelSize= iCS_EditorConfig.GetPortLabelSize(port.Name);
                        float nameSize= paddingBy2+labelSize.x+iCS_EditorConfig.PortDiameter;
                        if(leftPadding < nameSize) leftPadding= nameSize;
                    }
                }
            );
            return leftPadding;
        }
    }
    // ----------------------------------------------------------------------
    public float NodeRightPadding {
        get {
            if(IsIconizedOnDisplay) return 0;
            float paddingBy2= 0.5f*iCS_EditorConfig.PaddingSize;
            float rightPadding= iCS_EditorConfig.PaddingSize;
            ForEachRightChildPort(
                port=> {
                    if(!port.IsStatePort && port.IsPortOnParentEdge) {
                        Vector2 labelSize= iCS_EditorConfig.GetPortLabelSize(port.Name);
                        float nameSize= paddingBy2+labelSize.x+iCS_EditorConfig.PortDiameter;
                        if(rightPadding < nameSize) rightPadding= nameSize;
                    }
                }
            );
            return rightPadding;
        }
    }
}
