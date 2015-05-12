using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
public partial class iCS_Graphics {
    // ======================================================================
    // Picking functionality
    // ----------------------------------------------------------------------
    public bool IsNodeTitleBarPicked(iCS_EditorObject node, Vector2 pick) {
        if(node == null || !node.IsNode || !node.IsVisibleOnDisplay) return false;
        if(node.IsIconizedOnDisplay) {
            Rect nodeNamePos= GetNodeNamePosition(node);
            return nodeNamePos.Contains(pick);
        }
        Rect titleRect= node.AnimatedRect;
        titleRect.height= kNodeTitleHeight;
        return titleRect.Contains(pick);
    }
    
    // ======================================================================
    // Fold/Unfold icon functionality.
    // ----------------------------------------------------------------------
    public bool IsNodeNamePicked(iCS_EditorObject node, Vector2 pick) {
        if(node.IsIconizedOnDisplay) {
            Rect nodePos= GetNodeNamePosition(node);
            float invScale= 1.0f/Scale;
            nodePos.width*= invScale;
            nodePos.height*= invScale;
            return nodePos.Contains(pick);
        } else {
            if(!IsNodeTitleBarPicked(node, pick)) return false;
            if(IsFoldIconPicked(node, pick)) return false;
            if(IsMinimizeIconPicked(node, pick)) return false;
            return true;            
        }
    }
    // ----------------------------------------------------------------------
    bool ShouldShowTitle() {
        return Scale >= 0.4f;    
    }
	
    // ======================================================================
    // Fold/Unfold icon functionality.
    // ----------------------------------------------------------------------
    public bool IsFoldIconPicked(iCS_EditorObject node, Vector2 pick) {
        if(!ShouldDisplayFoldIcon(node)) return false;
        Rect foldIconPos= GetFoldIconPosition(node);
        foldIconPos.x-=3; foldIconPos.y-=3; foldIconPos.width+=6; foldIconPos.height+=6;
        return foldIconPos.Contains(pick);
    }
    bool ShouldDisplayFoldIcon(iCS_EditorObject obj) {
        if(Scale < 0.4f) return false;
        if(obj.IsIconizedOnDisplay) return false;
        if(obj.IsKindOfFunction || obj.IsInstanceNode) return false;
        if(obj.IsDisplayRoot) return false;
        return (obj.IsKindOfPackage || obj.IsStateChart || obj.IsState);
    }
    Rect GetFoldIconPosition(iCS_EditorObject obj) {
        Rect objPos= obj.AnimatedRect;
        var foldedIcon= iCS_BuiltinTextures.FoldIcon(Scale);
        return new Rect(objPos.x+8, objPos.y, foldedIcon.width, foldedIcon.height);
    }

    // ======================================================================
    // Minimize icon functionality
    // ----------------------------------------------------------------------
    public bool IsMinimizeIconPicked(iCS_EditorObject node, Vector2 pick) {
        if(!ShouldDisplayMinimizeIcon(node)) return false;
        Rect minimizeIconPos= GetMinimizeIconPosition(node);
        return minimizeIconPos.Contains(pick);
    }
    bool ShouldDisplayMinimizeIcon(iCS_EditorObject obj) {
        if(Scale < 0.4f) return false;
        if(obj.IsDisplayRoot) return false;
        return obj.InstanceId != 0 && obj.IsNode && !obj.IsIconizedOnDisplay;
    }
    Rect GetMinimizeIconPosition(iCS_EditorObject node) {
        Rect objPos= node.AnimatedRect;
        return new Rect(objPos.xMax-2-/*minimizeIcon.width*/16, objPos.y, /*minimizeIcon.width*/16, /*minimizeIcon.height*/16);
    }

    // ======================================================================
    // Maximize icon functionality
    // ----------------------------------------------------------------------
    public static Vector2 GetMaximizeIconSize(iCS_EditorObject node) {
        Texture2D icon= null;
        if(node != null && node.IconGUID != null) {
            icon= TextureCache.GetIconFromGUID(node.IconGUID);
            if(icon != null) return new Vector2(icon.width, icon.height);
        }
        // Avoid null exception of first layout.
        if(maximizeIcon == null) {
            Init(node.IStorage);
        }
        return new Vector2(maximizeIcon.width, maximizeIcon.height);        
    }
    // ----------------------------------------------------------------------
    public bool IsMaximizeIconPicked(iCS_EditorObject obj, Vector2 mousePos) {
        if(!ShouldDisplayMaximizeIcon(obj)) return false;
        Rect maximizeIconPos= GetMaximizeIconPosition(obj);
        return maximizeIconPos.Contains(mousePos);
    }
    bool ShouldDisplayMaximizeIcon(iCS_EditorObject obj) {
        return obj.InstanceId != 0 && obj.IsNode && obj.IsIconizedOnDisplay;
    }
    Rect GetMaximizeIconPosition(iCS_EditorObject obj) {
        return obj.AnimatedRect;
    }

    // ======================================================================
    // Determines if the pick is within the port name label.
    // ----------------------------------------------------------------------
    public bool IsPortNamePicked(iCS_EditorObject port, Vector2 pick, iCS_IStorage iStorage) {
        if(!ShouldDisplayPortName(port)) return false;
        Rect portNamePos= GetPortNamePosition(port, iStorage);
        float invScale= 1.0f/Scale;
        portNamePos.width*= invScale;
        portNamePos.height*= invScale;
        return portNamePos.Contains(pick);
    }
    // ----------------------------------------------------------------------
    bool ShouldShowLabel() {
        return Scale >= 0.5f;        
    }
    // ----------------------------------------------------------------------
    bool ShouldShowPort() {
        return Scale >= 0.45f;        
    }
    
    // ======================================================================
    // Determines if the pick is within the port value label.
    // ----------------------------------------------------------------------
    public bool IsPortValuePicked(iCS_EditorObject port, Vector2 pick) {
        if(!ShouldDisplayPortValue(port)) return false;
        if(!port.IsInputPort) return false;
        if(port.ProducerPort != null) return false;
        Rect portValuePos= GetPortValuePosition(port);
        float invScale= 1.0f/Scale;
        portValuePos.width*= invScale;
        portValuePos.height*= invScale;
        return portValuePos.Contains(pick);
    }
	
    // ======================================================================
    // Displays which element is being picked.
    // ----------------------------------------------------------------------
    public iCS_PickInfo GetPickInfo(Vector2 pick, iCS_IStorage iStorage) {
        iCS_PickInfo pickInfo= new iCS_PickInfo(iStorage);
        pickInfo.PickedPoint= pick;
        pickInfo.PickedPointInGUISpace= TranslateAndScale(pick);
        var port= iStorage.GetPortAt(pick);
        if(port != null) {
//            Debug.Log("Port: "+port.Name+" is being picked");
            pickInfo.PickedObject= port;
            pickInfo.PickedPart= iCS_PickPartEnum.EditorObject;
            pickInfo.PickedPartGraphPosition= port.GlobalRect;
            pickInfo.PickedPartGUIPosition= TranslateAndScale(pickInfo.PickedPartGraphPosition);
            return pickInfo;
        }
        var pickedNode= iStorage.GetNodeAt(pick);
        if(pickedNode != null) {
            if(IsFoldIconPicked(pickedNode, pick)) {
//                Debug.Log("Fold icon of: "+pickedNode.Name+" is being picked");
                pickInfo.PickedObject= pickedNode;
                pickInfo.PickedPart= iCS_PickPartEnum.FoldIcon;
                pickInfo.PickedPartGraphPosition= GetFoldIconPosition(pickedNode);
                pickInfo.PickedPartGUIPosition= TranslateAndScale(pickInfo.PickedPartGraphPosition);
                return pickInfo;
            }
            if(IsMinimizeIconPicked(pickedNode, pick)) {
//                Debug.Log("Minimize icon of: "+pickedNode.Name+" is being picked");
                pickInfo.PickedObject= pickedNode;
                pickInfo.PickedPart= iCS_PickPartEnum.MinimizeIcon;
                pickInfo.PickedPartGraphPosition= GetMinimizeIconPosition(pickedNode);
                pickInfo.PickedPartGUIPosition= TranslateAndScale(pickInfo.PickedPartGraphPosition);
                return pickInfo;
            }
            if(IsNodeNamePicked(pickedNode, pick)) {
//                Debug.Log("Node name: "+pickedNode.Name+" is being picked");
                pickInfo.PickedObject= pickedNode;
                pickInfo.PickedPart= iCS_PickPartEnum.Name;
                Rect namePos= GetNodeNamePosition(pickedNode);
                float invScale= 1.0f/Scale;
                pickInfo.PickedPartGraphPosition= new Rect(namePos.x, namePos.y, namePos.width*invScale, namePos.height*invScale);
                var guiPos= TranslateAndScale(Math3D.ToVector2(namePos));
                pickInfo.PickedPartGUIPosition= new Rect(guiPos.x, guiPos.y, namePos.width, namePos.height);
                return pickInfo;
            }
            bool result= iStorage.UntilMatchingChildNode(pickedNode,
                c=> {
                    if(c.IsIconizedOnDisplay) {
                        if(IsNodeNamePicked(c, pick)) {
                            pickInfo.PickedObject= c;
                            pickInfo.PickedPart= iCS_PickPartEnum.Name;
                            Rect namePos= GetNodeNamePosition(c);
                            float invScale= 1.0f/Scale;
                            pickInfo.PickedPartGraphPosition= new Rect(namePos.x, namePos.y, namePos.width*invScale, namePos.height*invScale);
                            var guiPos= TranslateAndScale(Math3D.ToVector2(namePos));
                            pickInfo.PickedPartGUIPosition= new Rect(guiPos.x, guiPos.y, namePos.width, namePos.height);
                            return true;
                        }
                    } 
                    return false;
                }
            );
            if(result) return pickInfo;
        }
        var closestPort= iStorage.GetClosestPortAt(pick);
        if(closestPort != null) {
            if(IsPortNamePicked(closestPort, pick, iStorage)) {
                pickInfo.PickedObject= closestPort;
                pickInfo.PickedPart= iCS_PickPartEnum.Name;
                Rect namePos= GetPortNamePosition(closestPort, iStorage);
                float invScale= 1.0f/Scale;
                pickInfo.PickedPartGraphPosition= new Rect(namePos.x, namePos.y, namePos.width*invScale, namePos.height*invScale);
                var guiPos= TranslateAndScale(Math3D.ToVector2(namePos));
                pickInfo.PickedPartGUIPosition= new Rect(guiPos.x, guiPos.y, namePos.width, namePos.height);
                return pickInfo;
            }
            if(IsPortValuePicked(closestPort, pick)) {
                pickInfo.PickedObject= closestPort;
                pickInfo.PickedPart= iCS_PickPartEnum.Value;
                Rect namePos= GetPortValuePosition(closestPort);
                float invScale= 1.0f/Scale;
                pickInfo.PickedPartGraphPosition= new Rect(namePos.x, namePos.y, namePos.width*invScale, namePos.height*invScale);
                var guiPos= TranslateAndScale(Math3D.ToVector2(namePos));
                pickInfo.PickedPartGUIPosition= new Rect(guiPos.x, guiPos.y, namePos.width, namePos.height);
                return pickInfo;
            }
        }
        if(pickedNode != null) {
//            Debug.Log("Node: "+pickedNode.Name+" is being picked");
            pickInfo.PickedObject= pickedNode;
            pickInfo.PickedPart= iCS_PickPartEnum.EditorObject;
            pickInfo.PickedPartGraphPosition= pickedNode.GlobalRect;
            pickInfo.PickedPartGUIPosition= TranslateAndScale(pickInfo.PickedPartGraphPosition);
            return pickInfo;
        }
        return null;
    }
    // ======================================================================
    // Extract graph information at a given point.
    // ----------------------------------------------------------------------
    Prelude.Tuple<iCS_EditorObject,iCS_EditorObject> GetGraphInfoAt(Vector2 point, iCS_IStorage iStorage) {
        iCS_EditorObject objectAt= iStorage.GetPortAt(point);
        if(objectAt == null) objectAt= iStorage.GetNodeAt(point);
        return new Prelude.Tuple<iCS_EditorObject,iCS_EditorObject>(objectAt, iStorage.GetClosestPortAt(point));
    }
}
}