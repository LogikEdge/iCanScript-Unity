using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class UK_Graphics {
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
	NodeStyle   moduleStyle     = null;
	NodeStyle   functionStyle   = null;
	NodeStyle   defaultStyle    = null;
	NodeStyle   selectedStyle   = null;
//	NodeStyle   nodeInErrorStyle= null;

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
    static public bool Init(UK_IStorage storage) {
        // Load AA line texture.
        string texturePath;     
        if(lineTexture == null) {
            if(!GetCachedTexture(UK_EditorStrings.AALineTexture, out lineTexture, ref lineTextureErrorSeen)) {
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                lineTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        // Load node texture templates.
        if(defaultNodeTexture == null) {
            if(!GetCachedTexture(UK_EditorStrings.DefaultNodeTexture, out defaultNodeTexture, ref defaultNodeTextureErrorSeen)) {
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                defaultNodeTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        if(nodeMaskTexture == null) {
            texturePath= UK_EditorConfig.GuiAssetPath +"/"+UK_EditorStrings.NodeMaskTexture;
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
        if(!GetCachedIcon(UK_EditorStrings.FoldedIcon, out foldedIcon, ref foldedIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        if(!GetCachedIcon(UK_EditorStrings.UnfoldedIcon, out unfoldedIcon, ref unfoldedIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        // Load maximize/minimize icon.
        if(!GetCachedIcon(UK_EditorStrings.MinimizeIcon, out minimizeIcon, ref minimizeIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        if(!GetCachedIcon(UK_EditorStrings.MaximizeIcon, out maximizeIcon, ref maximizeIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        // Load line arrow heads.
        if(!GetCachedIcon(UK_EditorStrings.UpArrowHeadIcon, out upArrowHeadIcon, ref upArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!GetCachedIcon(UK_EditorStrings.DownArrowHeadIcon, out downArrowHeadIcon, ref downArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!GetCachedIcon(UK_EditorStrings.LeftArrowHeadIcon, out leftArrowHeadIcon, ref leftArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!GetCachedIcon(UK_EditorStrings.RightArrowHeadIcon, out rightArrowHeadIcon, ref rightArrowHeadIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        // Graphic resources properly initialized.
        IsInitialized= true;
        return IsInitialized;
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
        string texturePath= UK_EditorConfig.GuiAssetPath+"/"+fileName;
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
    public static Texture2D GetCachedIcon(string fileName, UK_IStorage storage) {
        // Try with the WarpDrice Icon prefix.
        string iconPath= UK_UserPreferences.UserIcons.uCodeIconPath+"/"+fileName;
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
    public static string IconPathToGUID(string fileName, UK_IStorage storage) {
        if(fileName == null) return null;
        Texture2D icon= GetCachedIcon(fileName, storage);
        if(icon == null) return null;
        string path= AssetDatabase.GetAssetPath(icon);
        return AssetDatabase.AssetPathToGUID(path);
    }
    // ----------------------------------------------------------------------
    public static bool GetCachedIcon(string fileName, out Texture2D icon, ref bool errorSeen, UK_IStorage storage) {
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
    public void DrawNode(UK_EditorObject node, UK_EditorObject selectedObject, UK_IStorage storage) {
        // Don't show hiden nodes.
        if(storage.IsVisible(node) == false) return;
        
        // Draw minimized node.
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.Name));
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        if(storage.IsMinimized(node)) {
            Texture icon= GetMaximizeIcon(node, nodeStyle, storage);
            Rect nodePos= storage.GetPosition(node);
            Rect texturePos= new Rect(nodePos.x, nodePos.y, icon.width, icon.height);                
            GUI.DrawTexture(texturePos, icon);                           
            GUI.Label(texturePos, new GUIContent("", node.ToolTip));
            Vector2 labelSize= UK_EditorConfig.GetPortLabelSize(title);
            GUI.Label(new Rect(0.5f*(texturePos.x+texturePos.xMax-labelSize.x), texturePos.y-labelSize.y, labelSize.x, labelSize.y), new GUIContent(title, node.ToolTip));
            return;
        }
        
        // Draw node box.
        GUIStyle guiStyle= nodeStyle.guiStyle;
        Rect position= storage.GetPosition(node);
        float leftOffset= guiStyle.overflow.left + (guiStyle.padding.left-guiStyle.overflow.left)/2;
        float rightOffset= guiStyle.overflow.right - (guiStyle.padding.right-guiStyle.overflow.right)/2;
        position.x-= leftOffset;
        position.y-= guiStyle.overflow.top;
        position.width+= leftOffset + rightOffset;
        position.height+= guiStyle.overflow.top + guiStyle.overflow.bottom;
        GUI.Box(position, new GUIContent(title,node.ToolTip), guiStyle);            
        EditorGUIUtility.AddCursorRect (new Rect(position.x,  position.y, position.width, UK_EditorConfig.NodeTitleHeight), MouseCursor.MoveArrow);
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

    // ======================================================================
    // Picking functionality
    // ----------------------------------------------------------------------
    public bool IsNodeTitleBarPicked(UK_EditorObject node, Vector2 pick, UK_IStorage storage) {
        if(!node.IsNode || storage.IsMinimized(node)) return false;
        Rect titleRect= storage.GetPosition(node);
        GUIStyle style= GetNodeGUIStyle(node, null, storage);
        titleRect.height= style.border.top;
        return titleRect.Contains(pick);
    }
    // ======================================================================
    // Fold/Unfold icon functionality.
    // ----------------------------------------------------------------------
    public bool IsFoldIconPicked(UK_EditorObject obj, Vector2 mousePos, UK_IStorage storage) {
        if(!ShouldDisplayFoldIcon(obj, storage)) return false;
        Rect foldIconPos= GetFoldIconPosition(obj, storage);
        return foldIconPos.Contains(mousePos);
    }
    bool ShouldDisplayFoldIcon(UK_EditorObject obj, UK_IStorage storage) {
        if(storage.IsMinimized(obj)) return false;
        return (obj.IsModule || obj.IsStateChart || obj.IsState);
    }
    Rect GetFoldIconPosition(UK_EditorObject obj, UK_IStorage storage) {
        Rect objPos= storage.GetPosition(obj);
        return new Rect(objPos.x+8, objPos.y, foldedIcon.width, foldedIcon.height);
    }
    // ======================================================================
    // Minimize icon functionality
    // ----------------------------------------------------------------------
    public bool IsMinimizeIconPicked(UK_EditorObject obj, Vector2 mousePos, UK_IStorage storage) {
        if(!ShouldDisplayMinimizeIcon(obj, storage)) return false;
        Rect minimizeIconPos= GetMinimizeIconPosition(obj, storage);
        return minimizeIconPos.Contains(mousePos);
    }
    bool ShouldDisplayMinimizeIcon(UK_EditorObject obj, UK_IStorage storage) {
        return obj.InstanceId != 0 && obj.IsNode && !storage.IsMinimized(obj);
    }
    Rect GetMinimizeIconPosition(UK_EditorObject obj, UK_IStorage storage) {
        Rect objPos= storage.GetPosition(obj);
        return new Rect(objPos.xMax-4-minimizeIcon.width, objPos.y, minimizeIcon.width, minimizeIcon.height);
    }
    // ======================================================================
    // Maximize icon functionality
    // ----------------------------------------------------------------------
    public static Texture2D GetMaximizeIcon(UK_EditorObject node, NodeStyle nodeStyle, UK_IStorage storage) {
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
    public bool IsMaximizeIconPicked(UK_EditorObject obj, Vector2 mousePos, UK_IStorage storage) {
        if(!ShouldDisplayMaximizeIcon(obj, storage)) return false;
        Rect maximizeIconPos= GetMaximizeIconPosition(obj, storage);
        return maximizeIconPos.Contains(mousePos);
    }
    bool ShouldDisplayMaximizeIcon(UK_EditorObject obj, UK_IStorage storage) {
        return obj.InstanceId != 0 && obj.IsNode && storage.IsMinimized(obj);
    }
    Rect GetMaximizeIconPosition(UK_EditorObject obj, UK_IStorage storage) {
        return storage.GetPosition(obj);
    }
    // ======================================================================
    // Node style functionality
    // ----------------------------------------------------------------------
    NodeStyle GetNodeStyle(UK_EditorObject node, UK_EditorObject selectedObject, UK_IStorage storage) {
        // Node background is dependant on node type.
//        UK_Node runtimeNode= storage.EditorObjects.GetRuntimeObject(node) as UK_Node;
//        if(!runtimeNode.IsValid && ((int)EditorApplication.timeSinceStartup & 1) == 0) {
//            GenerateNodeStyle(ref nodeInErrorStyle, Color.red);
//            return nodeInErrorStyle;
//        }
        if(node == selectedObject) {
            GenerateNodeStyle(ref selectedStyle, storage.Preferences.NodeColors.SelectedColor);
            return selectedStyle;
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
    GUIStyle GetNodeGUIStyle(UK_EditorObject node, UK_EditorObject selectedObject, UK_IStorage storage) {
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        return nodeStyle.guiStyle;
    }
    
    // ----------------------------------------------------------------------
    // Returns the display color of the given node.
    Color GetNodeColor(UK_EditorObject node, UK_EditorObject selectedObject, UK_IStorage storage) {
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        return nodeStyle.nodeColor;
    }
    
    // ======================================================================
    //  PORT
    // ----------------------------------------------------------------------
    public void DrawPort(UK_EditorObject port, UK_EditorObject selectedObject, UK_IStorage storage) {
        // Only draw visible data ports.
        if(storage.IsVisible(port) == false) return;
        // Don't draw ports on minimized node.
        if(storage.IsMinimized(port)) return;
        
        // Draw port
        UK_EditorObject portParent= storage.GetParent(port);         
        Vector2 center= Math3D.ToVector2(storage.GetPosition(port));
        Type portValueType= port.RuntimeType;
        Color portColor= storage.Preferences.TypeColors.GetColor(portValueType);
        Color nodeColor= GetNodeColor(portParent, selectedObject, storage);
        if(port.IsDataPort) {
            DrawCircularPort(center, portColor, nodeColor);
        } else if(port.IsStatePort) {
            if(port.IsOutStatePort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(center, FacingNormal, UK_EditorConfig.PortRadius);
            }
        } else {
            DrawCircularPort(center, portColor, nodeColor);
        }
        // Configure move cursor for port.
        Rect portPos= new Rect(center.x-UK_EditorConfig.PortRadius, center.y-UK_EditorConfig.PortRadius, UK_EditorConfig.PortSize, UK_EditorConfig.PortSize);
        EditorGUIUtility.AddCursorRect (portPos, MouseCursor.MoveArrow);
        GUI.Label(portPos, new GUIContent("", port.ToolTip));
        
        // Show port label.
        if(port.IsStatePort) return;     // State transition name is handle by DrawConnection. 
        string name= portValueType.IsArray ? "["+port.Name+"]" : port.Name;
        Vector2 labelSize= UK_EditorConfig.GetPortLabelSize(name);
        switch(port.Edge) {
            case UK_EditorObject.EdgeEnum.Left:
                center.x+= 1 + UK_EditorConfig.PortSize;
                center.y-= 1 + 0.5f * labelSize.y;
                break;
            case UK_EditorObject.EdgeEnum.Right:
                center.x-= 1 + labelSize.x + UK_EditorConfig.PortSize;
                center.y-= 1 + 0.5f * labelSize.y;
                break;
            case UK_EditorObject.EdgeEnum.Top:            
                center.x-= 1 + 0.5f*labelSize.x;
                center.y-= UK_EditorConfig.PortSize+0.8f*labelSize.y*(1+TopBottomLabelOffset(port, storage));
                break;
            case UK_EditorObject.EdgeEnum.Bottom:
                center.x-= 1 + 0.5f*labelSize.x;
                center.y+= UK_EditorConfig.PortSize+0.8f*labelSize.y*TopBottomLabelOffset(port, storage)-0.2f*labelSize.y;
                break;
        }
        GUI.Label(new Rect(center.x, center.y, labelSize.x, labelSize.y), new GUIContent(name, port.ToolTip));
    }

	// ----------------------------------------------------------------------
    void DrawCircularPort(Vector3 _center, Color _fillColor, Color _borderColor) {
        Handles.color= Color.black;
        Handles.DrawSolidDisc(_center, FacingNormal, UK_EditorConfig.PortRadius+2.0f);
        Handles.color= _fillColor;
        Handles.DrawSolidDisc(_center, FacingNormal, UK_EditorConfig.PortRadius);
        Handles.color= _borderColor;
        Handles.DrawWireDisc(_center, FacingNormal, UK_EditorConfig.PortRadius+2.0f);
    }

	// ----------------------------------------------------------------------
    void DrawSquarePort(Vector3 _center, Color _borderColor) {
        // Draw connector.
        Vector3[] vectors= new Vector3[5];
        Handles.DrawSolidDisc(_center, FacingNormal, UK_EditorConfig.PortRadius);
        float delta= UK_EditorConfig.PortRadius-1;
        float minSize= 0.707f * UK_EditorConfig.PortRadius;
        for(; delta > minSize; --delta) {
            vectors[0]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[3]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[4]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= UK_EditorConfig.PortRadius;
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
        float radius= UK_EditorConfig.PortRadius+1;
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
        float radius= UK_EditorConfig.PortRadius+1;
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
        float radius= UK_EditorConfig.PortRadius+1;
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
        float radius= UK_EditorConfig.PortRadius+1;
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
        float radius= UK_EditorConfig.PortRadius+1;
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
    static float TopBottomLabelOffset(UK_EditorObject port, UK_IStorage storage) {
        float ratio= port.LocalPosition.x/storage.GetPosition(storage.GetParent(port)).width;
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
    public void DrawConnection(UK_EditorObject port, UK_IStorage storage) {
        if(storage.IsVisible(port.ParentId) && storage.IsValid(port.Source)) {
            UK_EditorObject source= storage.GetSource(port);
            UK_EditorObject sourceParent= storage.GetParent(source);
            if(storage.IsVisible(sourceParent) &&
               !(port.IsOutStatePort && storage.IsMinimized(sourceParent))) {
                Color color= storage.Preferences.TypeColors.GetColor(source.RuntimeType);
                color.a*= UK_EditorConfig.ConnectionTransparency;
                UK_ConnectionParams cp= new UK_ConnectionParams(port, storage);
        		Handles.DrawBezier(cp.Start, cp.End, cp.StartTangent, cp.EndTangent, color, lineTexture, 1.5f);
                // Show transition name for state connections.
                if(port.IsInStatePort) {
                    // Show transition name.
//                    string transitionName= storage.GetTransitionName(port);
//                    Vector2 labelSize= UK_EditorConfig.GetPortLabelSize(transitionName);
//                    Vector2 pos= new Vector2(cp.Center.x-0.5f*labelSize.x, cp.Center.y-0.5f*labelSize.y);
//                    GUI.Label(new Rect(pos.x, pos.y, labelSize.x, labelSize.y), new GUIContent(transitionName, port.ToolTip));
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
