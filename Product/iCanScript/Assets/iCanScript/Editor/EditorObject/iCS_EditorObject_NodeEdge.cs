using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {

    public partial class iCS_EditorObject {
        // ----------------------------------------------------------------------
        public float NodeTitleHeight {
            get {
                return iCS_EditorConfig.kNodeTitleHeight;
            }
        }
        // ----------------------------------------------------------------------
    	public float NodeTitleWidth {
    		get {
    			var titleWidth= NodeTitleSize.x;
                var subTitleWidth= NodeSubTitleSize.x;
                titleWidth= Mathf.Max(titleWidth, subTitleWidth);
    			var iconsWidth= iCS_EditorConfig.kNodeTitleIconSize+iCS_BuiltinTextures.kMinimizeIconSize;
    			var spacer= iCS_EditorConfig.kTitleFontSize;
    			return titleWidth+iconsWidth+spacer;
    		}
    	}
        // ----------------------------------------------------------------------
        public float NodeTopPadding {
            get {
                return NodeTitleHeight+iCS_EditorConfig.kPaddingSize;
            }
        }
        // ----------------------------------------------------------------------
        public float NodeBottomPadding {
            get {
                return iCS_EditorConfig.kPaddingSize;            
            }
        }
        // ----------------------------------------------------------------------
        public float NodeLeftPadding {
            get {
                float paddingBy2= 0.5f*iCS_EditorConfig.kPaddingSize;
                float leftPadding= iCS_EditorConfig.kPaddingSize;
                ForEachLeftChildPort(
                    port=> {
                        if(!port.IsStatePort && !port.IsFloating) {
                            var portName= port.DisplayName;
                            Vector2 labelSize= iCS_Layout.DefaultLabelSize(portName);
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
                float paddingBy2= 0.5f*iCS_EditorConfig.kPaddingSize;
                float rightPadding= iCS_EditorConfig.kPaddingSize;
                ForEachRightChildPort(
                    port=> {
                        if(!port.IsStatePort && !port.IsFloating) {
                            var portName= port.DisplayName;
                            Vector2 labelSize= iCS_Layout.DefaultLabelSize(portName);
                            float nameSize= paddingBy2+labelSize.x+iCS_EditorConfig.PortDiameter;
                            if(rightPadding < nameSize) rightPadding= nameSize;
                        }
                    }
                );
                return rightPadding;
            }
        }
    }    
}
