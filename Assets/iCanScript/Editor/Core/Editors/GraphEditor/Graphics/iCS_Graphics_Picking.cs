using UnityEngine;
using System.Collections;

public partial class iCS_Graphics {
    // ======================================================================
    // Picking functionality
    // ----------------------------------------------------------------------
    public bool IsNodeTitleBarPicked(iCS_EditorObject node, Vector2 pick, iCS_IStorage storage) {
        if(!node.IsNode || storage.IsMinimized(node)) return false;
        Rect titleRect= GetDisplayPosition(node, storage);
        titleRect.height= kNodeTitleHeight;
        return titleRect.Contains(pick);
    }
    
    // ======================================================================
    // Fold/Unfold icon functionality.
    // ----------------------------------------------------------------------
    public bool IsNodeNamePicked(iCS_EditorObject node, Vector2 pick, iCS_IStorage storage) {
        if(!IsNodeTitleBarPicked(node, pick, storage)) return false;
        if(IsFoldIconPicked(node, pick, storage)) return false;
        if(IsMinimizeIconPicked(node, pick, storage)) return false;
        return true;
    }
    
    // ======================================================================
    // Fold/Unfold icon functionality.
    // ----------------------------------------------------------------------
    public bool IsFoldIconPicked(iCS_EditorObject node, Vector2 pick, iCS_IStorage storage) {
        if(!ShouldDisplayFoldIcon(node, storage)) return false;
        Rect foldIconPos= GetFoldIconPosition(node, storage);
        return foldIconPos.Contains(pick);
    }
    bool ShouldDisplayFoldIcon(iCS_EditorObject obj, iCS_IStorage storage) {
        if(storage.IsMinimized(obj)) return false;
        return (obj.IsModule || obj.IsStateChart || obj.IsState);
    }
    Rect GetFoldIconPosition(iCS_EditorObject obj, iCS_IStorage storage) {
        Rect objPos= GetDisplayPosition(obj, storage);
        return new Rect(objPos.x+8, objPos.y, foldedIcon.width, foldedIcon.height);
    }

    // ======================================================================
    // Minimize icon functionality
    // ----------------------------------------------------------------------
    public bool IsMinimizeIconPicked(iCS_EditorObject node, Vector2 pick, iCS_IStorage storage) {
        if(!ShouldDisplayMinimizeIcon(node, storage)) return false;
        Rect minimizeIconPos= GetMinimizeIconPosition(node, storage);
        return minimizeIconPos.Contains(pick);
    }
    bool ShouldDisplayMinimizeIcon(iCS_EditorObject obj, iCS_IStorage storage) {
        return obj.InstanceId != 0 && obj.IsNode && !storage.IsMinimized(obj);
    }
    Rect GetMinimizeIconPosition(iCS_EditorObject node, iCS_IStorage storage) {
        Rect objPos= GetDisplayPosition(node, storage);
        return new Rect(objPos.xMax-4-minimizeIcon.width, objPos.y, minimizeIcon.width, minimizeIcon.height);
    }

    // ======================================================================
    // Maximize icon functionality
    // ----------------------------------------------------------------------
    public static Vector2 GetMaximizeIconSize(iCS_EditorObject node, iCS_IStorage storage) {
        Texture2D icon= null;
        if(storage.Preferences.Icons.EnableMinimizedIcons && node != null && node.IconGUID != null) {
            icon= iCS_TextureCache.GetIconFromGUID(node.IconGUID);
            if(icon != null) return new Vector2(icon.width, icon.height);
        }
        return new Vector2(maximizeIcon.width, maximizeIcon.height);        
    }
    // ----------------------------------------------------------------------
    public Texture2D GetMaximizeIcon(iCS_EditorObject node, iCS_IStorage storage) {
        Texture2D icon= null;
        if(storage.Preferences.Icons.EnableMinimizedIcons && node != null && node.IconGUID != null) {
            icon= iCS_TextureCache.GetIconFromGUID(node.IconGUID);
            if(icon != null) return icon;
        }
        return GetNodeDefaultMaximizeIcon(node, storage);
    }
    // ----------------------------------------------------------------------
    public bool IsMaximizeIconPicked(iCS_EditorObject obj, Vector2 mousePos, iCS_IStorage storage) {
        if(!ShouldDisplayMaximizeIcon(obj, storage)) return false;
        Rect maximizeIconPos= GetMaximizeIconPosition(obj, storage);
        return maximizeIconPos.Contains(mousePos);
    }
    bool ShouldDisplayMaximizeIcon(iCS_EditorObject obj, iCS_IStorage storage) {
        return obj.InstanceId != 0 && obj.IsNode && storage.IsMinimized(obj);
    }
    Rect GetMaximizeIconPosition(iCS_EditorObject obj, iCS_IStorage storage) {
        return GetDisplayPosition(obj, storage);
    }

    // ======================================================================
    // Determines if the pick is within the port name label.
    // ----------------------------------------------------------------------
    public bool IsPortNamePicked(iCS_EditorObject port, Vector2 pick, iCS_IStorage iStorage) {
        if(!port.IsPort) return false;
        Vector2 portPos= Math3D.ToVector2(iStorage.GetPosition(port));
        if(pick.y < portPos.y-8 || pick.y > portPos.y+8) return false;
        if(port.IsOnRightEdge) {
            return pick.x < portPos.x;
        }
        if(port.IsOnLeftEdge) {
            return pick.x > portPos.x;
        }
        return false;
    }

    // ======================================================================
    // Determines if the pick is within the port value label.
    // ----------------------------------------------------------------------
    public bool IsPortValuePicked(iCS_EditorObject port, Vector2 pick, iCS_IStorage iStorage) {
        if(!port.IsPort) return false;
        Vector2 portPos= Math3D.ToVector2(iStorage.GetPosition(port));
        if(pick.y < portPos.y-8 || pick.y > portPos.y+8) return false;
        if(port.IsOnRightEdge) {
            return pick.x > portPos.x;
        }
        if(port.IsOnLeftEdge) {
            return pick.x < portPos.x;
        }
        return false;
    }

    // ======================================================================
    // Displays whish element is being picked.
    // ----------------------------------------------------------------------
    public void DebugGraphElementPicked(Vector2 pick, iCS_IStorage iStorage) {
        var port= iStorage.GetPortAt(pick);
        if(port != null) {
            Debug.Log("Port: "+port.Name+" is being picked");
            return;
        }

        var pickedNode= iStorage.GetNodeAt(pick);
        if(pickedNode != null) {
            Debug.Log("Node: "+pickedNode.Name+" is being picked");
            if(IsFoldIconPicked(pickedNode, pick, iStorage)) Debug.Log("Fold icon of: "+pickedNode.Name+" is being picked");
            if(IsMinimizeIconPicked(pickedNode, pick, iStorage)) Debug.Log("Minimize icon of: "+pickedNode.Name+" is being picked");
            if(IsNodeNamePicked(pickedNode, pick, iStorage)) Debug.Log("Node name: "+pickedNode.Name+" is being picked");
        }
        var closestPort= iStorage.GetClosestPortAt(pick);
        if(closestPort != null) {
            if(IsPortNamePicked(closestPort, pick, iStorage)) Debug.Log("Port name: "+closestPort.Name+" of "+iStorage.GetParent(closestPort).Name+" is being picked");
            if(IsPortValuePicked(closestPort, pick, iStorage)) Debug.Log("Port value: "+closestPort.Name+" of "+iStorage.GetParent(closestPort).Name+" is being picked");
        }
    }
}
