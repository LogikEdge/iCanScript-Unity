using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    public float NodeTitleHeight {
        get {
            return 0.75f*iCS_EditorConfig.NodeTitleHeight;
        }
    }
    // ----------------------------------------------------------------------
	public float NodeTitleWidth {
		get {
			var titleWidth= iCS_EditorConfig.GetNodeTitleWidth(DisplayName);
			var iconsWidth= 2f*iCS_BuiltinTextures.kMinimizeIconSize;
			var spacer= iCS_EditorConfig.kTitleFontSize;
			return titleWidth+iconsWidth+spacer;
		}
	}
    // ----------------------------------------------------------------------
    public float NodeTopPadding {
        get {
            return NodeTitleHeight+iCS_EditorConfig.PaddingSize;
        }
    }
    // ----------------------------------------------------------------------
    public float NodeBottomPadding {
        get {
            return iCS_EditorConfig.PaddingSize;            
        }
    }
    // ----------------------------------------------------------------------
    public float NodeLeftPadding {
        get {
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
