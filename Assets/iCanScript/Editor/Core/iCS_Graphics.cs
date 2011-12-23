using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_Graphics {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const float kMinimizeSize= 32f;
    
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    static public bool  IsInitialized= false;    
    static Texture2D    lineTexture       = null;
    static Texture2D    defaultNodeTexture= null;
    static Texture2D    nodeMaskTexture   = null;
    static Texture2D    foldedIcon        = null;
    static Texture2D    unfoldedIcon      = null;
    static Texture2D    minimizeIcon      = null;
    static Texture2D    maximizeIcon      = null;
    static Texture2D    upArrowHeadIcon   = null;
    static Texture2D    downArrowHeadIcon = null;
    static Texture2D    leftArrowHeadIcon = null;
    static Texture2D    rightArrowHeadIcon= null;
    static bool         lineTextureErrorSeen        = false;
    static bool         defaultNodeTextureErrorSeen = false; 
    static bool         nodeMaskTextureErrorSeen    = false;   
	static bool         foldedIconErrorSeen         = false;
	static bool         unfoldedIconErrorSeen       = false;
	static bool         minimizeIconErrorSeen       = false;
	static bool         maximizeIconErrorSeen       = false;
	static bool         upArrowHeadIconErrorSeen    = false;
	static bool         downArrowHeadIconErrorSeen  = false;
	static bool         leftArrowHeadIconErrorSeen  = false;
	static bool         rightArrowHeadIconErrorSeen = false;
	
    // ----------------------------------------------------------------------
    static Dictionary<string, Texture2D>   cachedTextures= new Dictionary<string, Texture2D>();
    
    // ----------------------------------------------------------------------
	public class NodeStyle {
	    public GUIStyle    guiStyle    = null;
	    public Color       nodeColor   = new Color(0,0,0,0);
	    public Texture2D   maximizeIcon= null;
	}
	NodeStyle   stateStyle      = null;
	NodeStyle   entryStateStyle = null;
	NodeStyle   moduleStyle     = null;
	NodeStyle   functionStyle   = null;
	NodeStyle   defaultStyle    = null;
	NodeStyle   selectedStyle   = null;
//	NodeStyle   nodeInErrorStyle= null;
    GUIStyle    labelStyle      = null;

    // ======================================================================
	// CONSTANTS
    // ----------------------------------------------------------------------
    public static readonly Vector2 UpDirection   = new Vector2(0,-1);
    public static readonly Vector2 DownDirection = new Vector2(0,1);
    public static readonly Vector2 RightDirection= new Vector2(1,0);
    public static readonly Vector2 LeftDirection = new Vector2(-1,0);
    public static readonly Vector3 FacingNormal  = new Vector3(0,0,-1);
        

    // ======================================================================
    //  INITIALIZATION
	// ----------------------------------------------------------------------
    static public bool Init(iCS_IStorage storage) {
        // Load AA line texture.
        string texturePath;     
        if(lineTexture == null) {
            if(!GetCachedTexture(iCS_EditorStrings.AALineTexture, out lineTexture, ref lineTextureErrorSeen)) {
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                lineTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        // Load node texture templates.
        if(defaultNodeTexture == null) {
            if(!GetCachedTexture(iCS_EditorStrings.DefaultNodeTexture, out defaultNodeTexture, ref defaultNodeTextureErrorSeen)) {
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                defaultNodeTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        if(nodeMaskTexture == null) {
            texturePath= iCS_EditorConfig.GuiAssetPath +"/"+iCS_EditorStrings.NodeMaskTexture;
            nodeMaskTexture= AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            if(nodeMaskTexture == null) {
                ResourceMissingError(texturePath, ref nodeMaskTextureErrorSeen);
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                nodeMaskTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        // Load folded/unfolded icons.
        if(!GetCachedIcon(iCS_EditorStrings.FoldedIcon, out foldedIcon, ref foldedIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        if(!GetCachedIcon(iCS_EditorStrings.UnfoldedIcon, out unfoldedIcon, ref unfoldedIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        // Load maximize/minimize icon.
        if(!GetCachedIcon(iCS_EditorStrings.MinimizeIcon, out minimizeIcon, ref minimizeIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        if(!GetCachedIcon(iCS_EditorStrings.MaximizeIcon, out maximizeIcon, ref maximizeIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        // Load line arrow heads.
        if(!GetCachedIcon(iCS_EditorStrings.UpArrowHeadIcon, out upArrowHeadIcon, ref upArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!GetCachedIcon(iCS_EditorStrings.DownArrowHeadIcon, out downArrowHeadIcon, ref downArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!GetCachedIcon(iCS_EditorStrings.LeftArrowHeadIcon, out leftArrowHeadIcon, ref leftArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!GetCachedIcon(iCS_EditorStrings.RightArrowHeadIcon, out rightArrowHeadIcon, ref rightArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        // Graphic resources properly initialized.
        IsInitialized= true;
        return IsInitialized;
    }

    // ----------------------------------------------------------------------
    GUIStyle GetLabelStyle(iCS_IStorage storage) {
        Color labelColor= storage.Preferences.NodeColors.LabelColor;
        if(labelStyle == null) labelStyle= new GUIStyle();
        labelStyle.normal.textColor= labelColor;
        labelStyle.hover.textColor= labelColor;
        labelStyle.focused.textColor= labelColor;
        labelStyle.active.textColor= labelColor;
        labelStyle.onNormal.textColor= labelColor;
        labelStyle.onHover.textColor= labelColor;
        labelStyle.onFocused.textColor= labelColor;
        labelStyle.onActive.textColor= labelColor;
        return labelStyle;
    }
    // ----------------------------------------------------------------------
    public static Texture2D GetCachedTextureFromGUID(string guid) {
        return guid != null ? GetCachedTexture(AssetDatabase.GUIDToAssetPath(guid)) : null;
    }
    // ----------------------------------------------------------------------
    public static Texture2D GetCachedTexture(string fileName) {
        Texture2D texture= null;
        if(cachedTextures.ContainsKey(fileName)) {
            cachedTextures.TryGetValue(fileName, out texture);
            if(texture != null) return texture;
            cachedTextures.Remove(fileName);
        }
        texture= AssetDatabase.LoadAssetAtPath(fileName, typeof(Texture2D)) as Texture2D;
        if(texture != null) {
            cachedTextures.Add(fileName, texture);
            texture.hideFlags= HideFlags.DontSave;
        }
        return texture;
    }
    // ----------------------------------------------------------------------
    static bool GetCachedTexture(string fileName, out Texture2D texture, ref bool errorSeen) {
        string texturePath= iCS_EditorConfig.GuiAssetPath+"/"+fileName;
        texture= GetCachedTexture(texturePath);
        if(texture == null) {
            ResourceMissingError(texturePath, ref errorSeen);
            return false;
        }        
        return true;            
    }
    // ----------------------------------------------------------------------
    public static Texture2D GetCachedIconFromGUID(string guid) {
        return GetCachedTextureFromGUID(guid);
    }
    // ----------------------------------------------------------------------
    public static Texture2D GetCachedIcon(string fileName, iCS_IStorage storage) {
        // Try with the WarpDrice Icon prefix.
        string iconPath= iCS_UserPreferences.UserIcons.uCodeIconPath+"/"+fileName;
        Texture2D icon= GetCachedTexture(iconPath);
        if(icon == null) {
            // Try with the user definable Icon prefixes.
            foreach(var path in storage.Preferences.Icons.CustomIconPaths) {
                icon= GetCachedTexture(path+"/"+fileName);
                if(icon != null) break;
            }
            // Try without any prefix.
            if(icon == null) {
                icon= GetCachedTexture(fileName);                            
            }
        }
        return icon;
    }
    // ----------------------------------------------------------------------
    public static string IconPathToGUID(string fileName, iCS_IStorage storage) {
        if(fileName == null) return null;
        Texture2D icon= GetCachedIcon(fileName, storage);
        if(icon == null) return null;
        string path= AssetDatabase.GetAssetPath(icon);
        return AssetDatabase.AssetPathToGUID(path);
    }
    // ----------------------------------------------------------------------
    public static bool GetCachedIcon(string fileName, out Texture2D icon, ref bool errorSeen, iCS_IStorage storage) {
        icon= GetCachedIcon(fileName, storage);
        if(icon == null) {
            ResourceMissingError(fileName, ref errorSeen);            
            return false;
        }
        return true;
    }
    // ----------------------------------------------------------------------
    public static void DrawIconCenteredAt(Vector2 point, Texture2D icon) {
        if(icon == null) return;
        GUI.DrawTexture(new Rect(point.x-0.5f*icon.width,point.y-0.5f*icon.height, icon.width, icon.height), icon);
    }
    
    // ----------------------------------------------------------------------
    void GenerateNodeStyle(ref NodeStyle nodeStyle, Color nodeColor) {
        // Build node style descriptor.
        if(nodeStyle == null) {
            nodeStyle= new NodeStyle();
        }
        if(nodeStyle.guiStyle == null) {
            nodeStyle.guiStyle= new GUIStyle();
            nodeStyle.guiStyle.normal.textColor= Color.black;
            nodeStyle.guiStyle.hover.textColor= Color.black;
            nodeStyle.guiStyle.border= new RectOffset(13,21,20,13);
            nodeStyle.guiStyle.padding= new RectOffset(3,8,17,8);
            nodeStyle.guiStyle.contentOffset= new Vector2(-3, -17);
            nodeStyle.guiStyle.overflow= new RectOffset(0,5,0,3);
            nodeStyle.guiStyle.alignment= TextAnchor.UpperCenter;
            nodeStyle.guiStyle.fontStyle= FontStyle.Bold;
        }
        if(nodeStyle.guiStyle.normal.background == null) {
            nodeStyle.nodeColor= new Color(0,0,0,0);            
            nodeStyle.guiStyle.normal.background= new Texture2D(nodeMaskTexture.width, nodeMaskTexture.height);
        }
        if(nodeStyle.guiStyle.hover.background == null) {
            nodeStyle.guiStyle.hover.background= new Texture2D(nodeMaskTexture.width, nodeMaskTexture.height);            
        }
        // Generate node normal texture.
        if(nodeColor == nodeStyle.nodeColor) return;
        for(int x= 0; x < nodeMaskTexture.width; ++x) {
            for(int y= 0; y < nodeMaskTexture.height; ++y) {
                if(nodeMaskTexture.GetPixel(x,y).a > 0.5f) {
                    nodeStyle.guiStyle.normal.background.SetPixel(x,y, nodeColor);
                }
                else {
                    nodeStyle.guiStyle.normal.background.SetPixel(x,y, defaultNodeTexture.GetPixel(x,y));
                }
            }
        }
        nodeStyle.guiStyle.normal.background.Apply();
        nodeStyle.guiStyle.normal.background.hideFlags= HideFlags.DontSave; 
        nodeStyle.nodeColor= nodeColor;
        // Generate node hover texture.
        for(int x= 0; x < defaultNodeTexture.width; ++x) {
            for(int y= 0; y < defaultNodeTexture.height; ++y) {
                if(defaultNodeTexture.GetPixel(x,y).a > 0.95f) {
                    nodeStyle.guiStyle.hover.background.SetPixel(x,y, nodeStyle.guiStyle.normal.background.GetPixel(x,y));
                }
                else {
                    nodeStyle.guiStyle.hover.background.SetPixel(x,y, new Color(1,1,1, defaultNodeTexture.GetPixel(x,y).a));
                }
            }
        }
        nodeStyle.guiStyle.hover.background.Apply();
        nodeStyle.guiStyle.hover.background.hideFlags= HideFlags.DontSave;
        // Generate minimized Icon.
        if(nodeStyle.maximizeIcon == null) {
            nodeStyle.maximizeIcon= new Texture2D(maximizeIcon.width, maximizeIcon.height);
            for(int x= 0; x < maximizeIcon.width; ++x) {
                for(int y= 0; y < maximizeIcon.height; ++y) {
                    nodeStyle.maximizeIcon.SetPixel(x, y, nodeStyle.nodeColor * maximizeIcon.GetPixel(x,y));
                }
            }
            nodeStyle.maximizeIcon.Apply();
            nodeStyle.maximizeIcon.hideFlags= HideFlags.DontSave;
        }
    }
    
    // ======================================================================
    //  GRID
    // ----------------------------------------------------------------------
    public void DrawGrid(Rect position, Color backgroundColor, Color gridColor, float gridSpacing, Vector2 offset) {
        // Draw background.
        Vector3[] vect= { new Vector3(0,0,0), new Vector3(position.width,0,0), new Vector3(position.width,position.height,0), new Vector3(0,position.height,0)};
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vect, backgroundColor, backgroundColor);

        // Draw grid lines.
        if(gridSpacing < 2) gridSpacing= 2.0f;

        float x= offset.x/gridSpacing;    // Compute grid offset dependency on viewport.
        x= gridSpacing-gridSpacing*(x-Mathf.Floor(x));
        float y= offset.y/gridSpacing;
        y= gridSpacing-gridSpacing*(y-Mathf.Floor(y));
        
        float gridSpacing5= 5.0f*gridSpacing;
        float xStepOffset= offset.x/gridSpacing5; // Compute major grid line offset dependency on viewport.
        int xSteps= Mathf.FloorToInt(5.0f*(xStepOffset-Mathf.Floor(xStepOffset)));
        float yStepOffset= offset.y/gridSpacing5; // Compute major grid line offset dependency on viewport.
        int ySteps= Mathf.FloorToInt(5.0f*(yStepOffset-Mathf.Floor(yStepOffset)));

        Color gridColor2= new Color(gridColor.r, gridColor.g, gridColor.b, 0.25f);
        for(; x < position.width; x+= gridSpacing, ++xSteps) {
            Handles.color= (xSteps % 5) == 0 ? gridColor : gridColor2;
            Handles.DrawLine(new Vector3(x,0,0), new Vector3(x,position.height,0));            
        }
        for(; y < position.width; y+= gridSpacing, ++ySteps) {
            Handles.color= (ySteps % 5) == 0 ? gridColor : gridColor2;
            Handles.DrawLine(new Vector3(0,y,0), new Vector3(position.width,y,0));            
        }
    }
    
    // ======================================================================
    //  NODE
    // ----------------------------------------------------------------------
    public void DrawNormalNode(iCS_EditorObject node, iCS_EditorObject selectedObject, iCS_IStorage storage) {        
        // Don't draw minimized node.
        if(IsInvisible(node, storage) || IsMinimized(node, storage)) return;
        
        // Draw node box.
        Rect position= GetDisplayPosition(node, storage);
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.Name));
        GUIStyle guiStyle= nodeStyle.guiStyle;
        float leftOffset= guiStyle.overflow.left + (guiStyle.padding.left-guiStyle.overflow.left)/2;
        float rightOffset= guiStyle.overflow.right - (guiStyle.padding.right-guiStyle.overflow.right)/2;
        position.x-= leftOffset;
        position.y-= guiStyle.overflow.top;
        position.width+= leftOffset + rightOffset;
        position.height+= guiStyle.overflow.top + guiStyle.overflow.bottom;
        GUI.Box(position, new GUIContent(title,node.ToolTip), guiStyle);            
        EditorGUIUtility.AddCursorRect (new Rect(position.x,  position.y, position.width, iCS_EditorConfig.NodeTitleHeight), MouseCursor.MoveArrow);
        // Fold/Unfold icon
        if(ShouldDisplayFoldIcon(node, storage)) {
            if(storage.IsFolded(node)) {
                GUI.DrawTexture(new Rect(position.x+8, position.y, foldedIcon.width, foldedIcon.height), foldedIcon);                           
            } else {
                GUI.DrawTexture(new Rect(position.x+8, position.y, unfoldedIcon.width, unfoldedIcon.height), unfoldedIcon);               
            }            
        }
        // Minimize Icon
        if(ShouldDisplayMinimizeIcon(node, storage)) {
            GUI.DrawTexture(new Rect(position.xMax-4-minimizeIcon.width, position.y, minimizeIcon.width, minimizeIcon.height), minimizeIcon);
        }
    }
    // ----------------------------------------------------------------------
    public void DrawMinimizedNode(iCS_EditorObject node, iCS_EditorObject selectedObject, iCS_IStorage storage) {        
        if(!IsMinimized(node, storage)) return;
        
        // Draw minimized node.
        Rect position= GetDisplayPosition(node, storage);
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        Texture icon= GetMaximizeIcon(node, nodeStyle, storage);
        if(position.width < 12f || position.height < 12f) return;  // Don't show if too small.
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.Name));
        Rect texturePos= new Rect(position.x, position.y, icon.width, icon.height);                
        GUI.DrawTexture(texturePos, icon);                           
        GUIStyle labelStyle= GetLabelStyle(storage);
        GUI.Label(texturePos, new GUIContent("", node.ToolTip), labelStyle);
        Vector2 labelSize= iCS_EditorConfig.GetPortLabelSize(title);
        GUI.Label(new Rect(0.5f*(texturePos.x+texturePos.xMax-labelSize.x), texturePos.y-labelSize.y, labelSize.x, labelSize.y), new GUIContent(title, node.ToolTip), labelStyle);
    }

    // ======================================================================
    // Picking functionality
    // ----------------------------------------------------------------------
    public bool IsNodeTitleBarPicked(iCS_EditorObject node, Vector2 pick, iCS_IStorage storage) {
        if(!node.IsNode || storage.IsMinimized(node)) return false;
        Rect titleRect= GetDisplayPosition(node, storage);
        GUIStyle style= GetNodeGUIStyle(node, null, storage);
        titleRect.height= style.border.top;
        return titleRect.Contains(pick);
    }
    // ======================================================================
    // Fold/Unfold icon functionality.
    // ----------------------------------------------------------------------
    public bool IsFoldIconPicked(iCS_EditorObject obj, Vector2 mousePos, iCS_IStorage storage) {
        if(!ShouldDisplayFoldIcon(obj, storage)) return false;
        Rect foldIconPos= GetFoldIconPosition(obj, storage);
        return foldIconPos.Contains(mousePos);
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
    public bool IsMinimizeIconPicked(iCS_EditorObject obj, Vector2 mousePos, iCS_IStorage storage) {
        if(!ShouldDisplayMinimizeIcon(obj, storage)) return false;
        Rect minimizeIconPos= GetMinimizeIconPosition(obj, storage);
        return minimizeIconPos.Contains(mousePos);
    }
    bool ShouldDisplayMinimizeIcon(iCS_EditorObject obj, iCS_IStorage storage) {
        return obj.InstanceId != 0 && obj.IsNode && !storage.IsMinimized(obj);
    }
    Rect GetMinimizeIconPosition(iCS_EditorObject obj, iCS_IStorage storage) {
        Rect objPos= GetDisplayPosition(obj, storage);
        return new Rect(objPos.xMax-4-minimizeIcon.width, objPos.y, minimizeIcon.width, minimizeIcon.height);
    }
    // ======================================================================
    // Maximize icon functionality
    // ----------------------------------------------------------------------
    public static Texture2D GetMaximizeIcon(iCS_EditorObject node, NodeStyle nodeStyle, iCS_IStorage storage) {
        Texture2D icon= null;
        if(storage.Preferences.Icons.EnableMinimizedIcons && node.IconGUID != null) {
            icon= GetCachedIconFromGUID(node.IconGUID);
            if(icon != null) return icon;
        }
        if(nodeStyle != null) {
            icon= nodeStyle.maximizeIcon;            
        } else {
            icon= maximizeIcon;
        }
        return icon;       
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
    // Node style functionality
    // ----------------------------------------------------------------------
    NodeStyle GetNodeStyle(iCS_EditorObject node, iCS_EditorObject selectedObject, iCS_IStorage storage) {
        // Node background is dependant on node type.
//        iCS_Node runtimeNode= storage.EditorObjects.GetRuntimeObject(node) as iCS_Node;
//        if(!runtimeNode.IsValid && ((int)EditorApplication.timeSinceStartup & 1) == 0) {
//            GenerateNodeStyle(ref nodeInErrorStyle, Color.red);
//            return nodeInErrorStyle;
//        }
        if(node == selectedObject) {
            GenerateNodeStyle(ref selectedStyle, storage.Preferences.NodeColors.SelectedColor);
            return selectedStyle;
        }
        if(node.IsEntryState) {
            GenerateNodeStyle(ref entryStateStyle, storage.Preferences.NodeColors.EntryStateColor);
            return entryStateStyle;            
        }
        if(node.IsState || node.IsStateChart) {
            GenerateNodeStyle(ref stateStyle, storage.Preferences.NodeColors.StateColor);
            return stateStyle;
        }
        if(node.IsModule) {
            GenerateNodeStyle(ref moduleStyle, storage.Preferences.NodeColors.ModuleColor);
            return moduleStyle;
        }
        if(node.IsStaticMethod || node.IsConversion || node.IsInstanceMethod) {
            GenerateNodeStyle(ref functionStyle, storage.Preferences.NodeColors.FunctionColor);
            return functionStyle;
        }
        GenerateNodeStyle(ref defaultStyle, Color.gray);
        return defaultStyle;
    }
    GUIStyle GetNodeGUIStyle(iCS_EditorObject node, iCS_EditorObject selectedObject, iCS_IStorage storage) {
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        return nodeStyle.guiStyle;
    }
    
    // ----------------------------------------------------------------------
    // Returns the display color of the given node.
    Color GetNodeColor(iCS_EditorObject node, iCS_EditorObject selectedObject, iCS_IStorage storage) {
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        return nodeStyle.nodeColor;
    }
    
    // ======================================================================
    //  PORT
    // ----------------------------------------------------------------------
    public void DrawPort(iCS_EditorObject port, iCS_EditorObject selectedObject, iCS_IStorage storage) {
        // Update display position.
        Rect position= GetDisplayPosition(port, storage);

        // Only draw visible data ports.
        if(IsInvisible(port, storage) || IsMinimized(port, storage)) return;
        
        // Draw port
        iCS_EditorObject portParent= storage.GetParent(port);         
        Vector2 center= Math3D.ToVector2(position);
        Type portValueType= port.RuntimeType;
        Color portColor= storage.Preferences.TypeColors.GetColor(portValueType);
        Color nodeColor= GetNodeColor(portParent, selectedObject, storage);
        if(port.IsDataPort) {
            DrawCircularPort(center, portColor, nodeColor);
        } else if(port.IsStatePort) {
            if(port.IsOutStatePort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(center, FacingNormal, iCS_EditorConfig.PortRadius);
            }
        } else if(port.IsInTransitionPort || port.IsOutTransitionPort) {
            Handles.color= Color.white;
            Handles.DrawSolidDisc(center, FacingNormal, iCS_EditorConfig.PortRadius);            
        }
        else {
            DrawCircularPort(center, portColor, nodeColor);
        }
        // Configure move cursor for port.
        Rect portPos= new Rect(center.x-iCS_EditorConfig.PortRadius*1.5f, center.y-iCS_EditorConfig.PortRadius*1.5f, iCS_EditorConfig.PortSize*1.5f, iCS_EditorConfig.PortSize*1.5f);
        EditorGUIUtility.AddCursorRect (portPos, MouseCursor.MoveArrow);
        GUIStyle labelStyle= GetLabelStyle(storage);
        GUI.Label(portPos, new GUIContent("", port.ToolTip), labelStyle);
        
        // Show port label.
        if(port.IsStatePort) return;     // State transition name is handle by DrawConnection. 
        string name= portValueType.IsArray ? "["+port.Name+"]" : port.Name;
        Vector2 labelSize= iCS_EditorConfig.GetPortLabelSize(name);
        switch(port.Edge) {
            case iCS_EditorObject.EdgeEnum.Left:
                center.x+= 1 + iCS_EditorConfig.PortSize;
                center.y-= 1 + 0.5f * labelSize.y;
                break;
            case iCS_EditorObject.EdgeEnum.Right:
                center.x-= 1 + labelSize.x + iCS_EditorConfig.PortSize;
                center.y-= 1 + 0.5f * labelSize.y;
                break;
            case iCS_EditorObject.EdgeEnum.Top:            
                center.x-= 1 + 0.5f*labelSize.x;
                center.y-= iCS_EditorConfig.PortSize+0.8f*labelSize.y*(1+TopBottomLabelOffset(port, storage));
                break;
            case iCS_EditorObject.EdgeEnum.Bottom:
                center.x-= 1 + 0.5f*labelSize.x;
                center.y+= iCS_EditorConfig.PortSize+0.8f*labelSize.y*TopBottomLabelOffset(port, storage)-0.2f*labelSize.y;
                break;
        }
        GUI.Label(new Rect(center.x, center.y, labelSize.x, labelSize.y), new GUIContent(name, port.ToolTip), labelStyle);
    }

	// ----------------------------------------------------------------------
    void DrawCircularPort(Vector3 _center, Color _fillColor, Color _borderColor) {
        Handles.color= Color.black;
        Handles.DrawSolidDisc(_center, FacingNormal, iCS_EditorConfig.PortRadius+2.0f);
        Handles.color= _fillColor;
        Handles.DrawSolidDisc(_center, FacingNormal, iCS_EditorConfig.PortRadius);
        Handles.color= _borderColor;
        Handles.DrawWireDisc(_center, FacingNormal, iCS_EditorConfig.PortRadius+2.0f);
    }

	// ----------------------------------------------------------------------
    void DrawSquarePort(Vector3 _center, Color _borderColor) {
        // Draw connector.
        Vector3[] vectors= new Vector3[5];
        Handles.DrawSolidDisc(_center, FacingNormal, iCS_EditorConfig.PortRadius);
        float delta= iCS_EditorConfig.PortRadius-1;
        float minSize= 0.707f * iCS_EditorConfig.PortRadius;
        for(; delta > minSize; --delta) {
            vectors[0]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[3]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[4]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= iCS_EditorConfig.PortRadius;
        vectors[0]= new Vector3(_center.x-delta, _center.y-delta, 0);
        vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x+delta, _center.y+delta, 0);
        vectors[3]= new Vector3(_center.x+delta, _center.y-delta, 0);
        vectors[4]= vectors[0];
        Handles.DrawPolyLine(vectors);
    }
    
	// ----------------------------------------------------------------------
    void DrawDiamondPort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[5];
        float radius= iCS_EditorConfig.PortRadius+1;
        Handles.DrawSolidDisc(_center, FacingNormal, 0.707f * radius);
        float delta= radius-1;
        float minSize= 0.707f * radius;
        for(; delta > minSize; --delta) {
            vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y, 0);
            vectors[2]= new Vector3(_center.x, _center.y+delta, 0);
            vectors[3]= new Vector3(_center.x+delta, _center.y, 0);
            vectors[4]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
        vectors[1]= new Vector3(_center.x-delta, _center.y, 0);
        vectors[2]= new Vector3(_center.x, _center.y+delta, 0);
        vectors[3]= new Vector3(_center.x+delta, _center.y, 0);
        vectors[4]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }
    
	// ----------------------------------------------------------------------
    void DrawDownTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= iCS_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x, _center.y+delta, 0);
            vectors[1]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x, _center.y+delta, 0);
        vectors[1]= new Vector3(_center.x+delta, _center.y-delta, 0);
        vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }

	// ----------------------------------------------------------------------
    void DrawUpTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= iCS_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= Color.black;
        delta= radius+1;
        vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
        vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x-delta, _center.y+delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
		Handles.color= _borderColor;
        delta= radius+2;
        vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
        vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x-delta, _center.y+delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }

	// ----------------------------------------------------------------------
    void DrawRightTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= iCS_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x+delta, _center.y, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x+delta, _center.y, 0);
        vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }

	// ----------------------------------------------------------------------
    void DrawLeftTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= iCS_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x-delta, _center.y, 0);
            vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x-delta, _center.y, 0);
        vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x+delta, _center.y-delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }

	// ----------------------------------------------------------------------
    static float[] portTopBottomRatio      = new float[]{ 1f/2f, 1f/4f, 3f/4f, 1f/6f, 5f/6f, 1f/8f, 3f/8f, 5f/8f, 7f/8f };
    static float[] portLabelTopBottomOffset= new float[]{ 0f   , 0f   , 0.8f , 0.8f , 0.8f , 0f   , 0.8f , 0f   , 0.8f };
    static float TopBottomLabelOffset(iCS_EditorObject port, iCS_IStorage storage) {
        float ratio= port.LocalPosition.x/GetDisplayPosition(storage.GetParent(port), storage).width;
        float error= 100f;
        float offset= 0f;
        for(int i= 0; i < portTopBottomRatio.Length; ++i) {
            float delta= Mathf.Abs(ratio-portTopBottomRatio[i]);
            if(delta < error) {
                error= delta;
                offset= portLabelTopBottomOffset[i];
            }
        }
        return offset;
    }
    
    // ======================================================================
    //  CONNECTION
    // ----------------------------------------------------------------------
    public void DrawConnection(iCS_EditorObject port, iCS_IStorage storage) {
        if(IsVisible(storage.GetParent(port), storage) && storage.IsValid(port.Source)) {
            iCS_EditorObject source= storage.GetSource(port);
            iCS_EditorObject sourceParent= storage.GetParent(source);
            if(IsVisible(sourceParent, storage) && !port.IsOutStatePort) {
                Color color= storage.Preferences.TypeColors.GetColor(source.RuntimeType);
                color.a*= iCS_EditorConfig.ConnectionTransparency;
                iCS_ConnectionParams cp= new iCS_ConnectionParams(port, GetDisplayPosition(port, storage), source, GetDisplayPosition(source, storage), storage);
        		Handles.DrawBezier(cp.Start, cp.End, cp.StartTangent, cp.EndTangent, color, lineTexture, 1.5f);
                // Show transition name for state connections.
                if(port.IsInStatePort) {
                    // Show transition input port.
                    Vector2 tangent= new Vector2(cp.EndTangent.x-cp.End.x, cp.EndTangent.y-cp.End.y);
                    tangent.Normalize();
                    if(Mathf.Abs(tangent.x) > Mathf.Abs(tangent.y)) {
                        if(tangent.x > 0) {
                            DrawIconCenteredAt(cp.End, leftArrowHeadIcon);                            
                        } else {
                            DrawIconCenteredAt(cp.End, rightArrowHeadIcon);
                        }
                    } else {
                        if(tangent.y > 0) {
                            DrawIconCenteredAt(cp.End, upArrowHeadIcon);
                        } else {
                            DrawIconCenteredAt(cp.End, downArrowHeadIcon);
                        }
                    }                 
                }
            }                                    
        }
    }

    // ======================================================================
    //  Utilities
    // ----------------------------------------------------------------------
    static Rect GetDisplayPosition(iCS_EditorObject edObj, iCS_IStorage storage) {
        Rect layoutPosition= GetLayoutPosition(edObj, storage);
        Rect displayPosition= storage.GetDisplayPosition(edObj);
        if(IsAnimationCompleted(edObj, storage)) {
            if((GetAnimationRatio(edObj, storage)-1f)*storage.Preferences.Animation.AnimationTime > 0.2f) {  // 200ms
                if(displayPosition.x != layoutPosition.x || displayPosition.y != layoutPosition.y ||
                   displayPosition.width!= layoutPosition.width || displayPosition.height != layoutPosition.height) {
                       storage.StartAnimTimer(edObj);
                       return displayPosition;
                   }
            }
            storage.SetDisplayPosition(edObj, layoutPosition);
            return layoutPosition;
        }
        float ratio= GetAnimationRatio(edObj, storage);
        displayPosition= Math3D.Lerp(displayPosition, layoutPosition, ratio);
        return displayPosition;
    }
    static Rect GetLayoutPosition(iCS_EditorObject edObj, iCS_IStorage storage) {
        if(!storage.IsVisible(edObj)) {
            iCS_EditorObject parent= storage.GetParent(edObj);
            for(; !storage.IsVisible(parent); parent= storage.GetParent(parent));
            Vector2 midPoint= Math3D.Middle(storage.GetPosition(parent));
            return new Rect(midPoint.x, midPoint.y, 0, 0);
        }
        return storage.GetPosition(edObj);
    }
    static float GetAnimationRatio(iCS_EditorObject edObj, iCS_IStorage storage) {
        float time= storage.Preferences.Animation.AnimationTime;
        float invTime= Math3D.IsZero(time) ? 10000f : 1f/time;
        return invTime*(storage.GetAnimTime(edObj));        
    }
    static bool IsAnimationCompleted(iCS_EditorObject edObj, iCS_IStorage storage) {
        return GetAnimationRatio(edObj, storage) >= 0.99f;
    }

    bool IsMinimized(iCS_EditorObject edObj, iCS_IStorage storage) {
        if(edObj.IsNode) {
            if(IsInvisible(edObj, storage)) return false;
            Rect position= GetDisplayPosition(edObj, storage);
            NodeStyle nodeStyle= GetNodeStyle(edObj, null, storage);
            Texture icon= GetMaximizeIcon(edObj, nodeStyle, storage);
            return (position.width*position.height <= icon.width*icon.height+1f);
        }
        return storage.IsMinimized(edObj) && IsAnimationCompleted(edObj, storage);
    }
    static bool IsFolded(iCS_EditorObject edObj, iCS_IStorage storage) {
        return storage.IsFolded(edObj) && IsAnimationCompleted(edObj, storage);
    }
    bool IsInvisible(iCS_EditorObject edObj, iCS_IStorage storage) {
        return !IsVisible(edObj, storage);
    }
    bool IsVisible(iCS_EditorObject edObj, iCS_IStorage storage) {
        if(edObj.IsNode) {
            Rect position= GetDisplayPosition(edObj, storage);
            return position.width >= 12f && position.height >= 12;  // Invisible if too small.
        }
        iCS_EditorObject parent= storage.GetParent(edObj);
        return IsVisible(parent, storage) && !IsMinimized(parent, storage);
    }
    
    // ======================================================================
    //  ERROR MANAGEMENT
    // ----------------------------------------------------------------------
	public static void ResourceMissingError(string _name, ref bool _alreadySeen) {
        string errorMsg= "Unable to locate editor resource: " + _name;
        if(!_alreadySeen) {
    		EditorUtility.DisplayDialog ("State Chart Abort Message", errorMsg, "Ok");            
        }
		else {
		    Debug.LogError(errorMsg);
	    }
		_alreadySeen= true;
	}


}
