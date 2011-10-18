using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class WD_Graphics {
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
    static bool         lineTextureErrorSeen        = false;
    static bool         defaultNodeTextureErrorSeen = false; 
    static bool         nodeMaskTextureErrorSeen    = false;   
	static bool         foldedIconErrorSeen         = false;
	static bool         unfoldedIconErrorSeen       = false;
	static bool         minimizeIconErrorSeen       = false;
	static bool         maximizeIconErrorSeen       = false;
	
    // ----------------------------------------------------------------------
	internal class NodeStyle {
	    public GUIStyle    guiStyle    = null;
	    public Color       nodeColor   = new Color(0,0,0,0);
	    public Texture2D   maximizeIcon= null;
	}
	NodeStyle   stateStyle      = null;
	NodeStyle   moduleStyle     = null;
    NodeStyle   classStyle      = null;
	NodeStyle   functionStyle   = null;
	NodeStyle   defaultStyle    = null;
	NodeStyle   selectedStyle   = null;
	NodeStyle   nodeInErrorStyle= null;

    // ----------------------------------------------------------------------
    Vector2     drawOffset= Vector2.zero;

    
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
    static public bool Init(WD_Storage storage) {
        // Load AA line texture.
        string texturePath;     
        if(lineTexture == null) {
            if(!LoadTexture("WD_LineTexture.psd", ref lineTexture, ref lineTextureErrorSeen)) {
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                lineTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        // Load node texture templates.
        if(defaultNodeTexture == null) {
            if(!LoadTexture("WD_DefaultNodeTexture.psd", ref defaultNodeTexture, ref defaultNodeTextureErrorSeen)) {
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                defaultNodeTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        if(nodeMaskTexture == null) {
            texturePath= WD_EditorConfig.GuiAssetPath + "/WD_NodeMaskTexture.psd";
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
        if(!LoadIcon(WD_EditorConfig.FoldedIcon, ref foldedIcon, ref foldedIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        if(!LoadIcon(WD_EditorConfig.UnfoldedIcon,ref unfoldedIcon, ref unfoldedIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        // Load maximize/minimize icon.
        if(!LoadIcon(WD_EditorConfig.MinimizeIcon, ref minimizeIcon, ref minimizeIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        if(!LoadIcon(WD_EditorConfig.MaximizeIcon, ref maximizeIcon, ref maximizeIconErrorSeen, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        // Graphic resources properly initialized.
        IsInitialized= true;
        return IsInitialized;
    }

    // ----------------------------------------------------------------------
    static bool LoadIcon(string fileName, ref Texture2D icon, ref bool errorSeen, WD_Storage storage) {
        icon= LoadIcon(fileName, storage);
        if(icon == null) {
            ResourceMissingError(fileName, ref errorSeen);            
            return false;
        }
        return true;
    }
    // ----------------------------------------------------------------------
    static Texture2D LoadIcon(string fileName, WD_Storage storage) {
        string iconPath= WD_UserPreferences.UserIconPaths.WarpDriveIconPath+"/"+fileName;
        Texture2D icon= AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D)) as Texture2D;
        if(icon != null) return icon;
        foreach(var path in storage.Preferences.IconPaths.CustomIconPaths) {
            icon= AssetDatabase.LoadAssetAtPath(path+"/"+fileName, typeof(Texture2D)) as Texture2D;
            if(icon != null) return icon;
        }
        icon= AssetDatabase.LoadAssetAtPath(fileName, typeof(Texture2D)) as Texture2D;
        if(icon == null) Debug.LogWarning("Unable to locate icon: "+fileName);
        return icon;
    }
    // ----------------------------------------------------------------------
    static bool LoadTexture(string fileName, ref Texture2D texture, ref bool errorSeen) {
        string texturePath= WD_EditorConfig.GuiAssetPath+"/"+fileName;
        texture= AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
        if(texture == null) {
            ResourceMissingError(texturePath, ref errorSeen);
            return false;
        }        
        return true;            
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
            nodeStyle.guiStyle.overflow= new RectOffset(0,6,0,6);
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
    //  TOOL TIP
    // ---------------------------------------------------------------------
    public void ShowToolTip(WD_EditorObject obj, WD_Behaviour graph) {
        if(obj.IsPort) {
            WD_EditorObject port= obj;
            Rect tmp= graph.EditorObjects.GetPosition(port);
            Vector2 pos= new Vector2(tmp.x, tmp.y);
            string name= port.Name + ":" + port.TypeName;
            Vector2 labelSize= WD_EditorConfig.GetLabelSize(name);
            GUI.Label(new Rect(pos.x, pos.y, labelSize.x, labelSize.y), name);            
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
    public void DrawNode(WD_EditorObject node, WD_EditorObject selectedObject, WD_Storage storage) {
        // Don't show hiden nodes.
        if(storage.EditorObjects.IsVisible(node) == false) return;
        
        // Draw minimized node.
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        if(node.IsMinimized) {
            Rect nodePos= storage.EditorObjects.GetPosition(node);
            Texture icon= GetMaximizeIcon(node, nodeStyle, storage);
            GUI.DrawTexture(new Rect(nodePos.x, nodePos.y, icon.width, icon.height), icon);                           
            return;
        }
        
        // Draw node box.
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.NameOrTypeName));
        GUIStyle guiStyle= nodeStyle.guiStyle;
        Rect position= storage.EditorObjects.GetPosition(node);
        float leftOffset= guiStyle.overflow.left + (guiStyle.padding.left-guiStyle.overflow.left)/2;
        float rightOffset= guiStyle.overflow.right - (guiStyle.padding.right-guiStyle.overflow.right)/2;
        position.x-= leftOffset;
        position.y-= guiStyle.overflow.top;
        position.width+= leftOffset + rightOffset;
        position.height+= guiStyle.overflow.top + guiStyle.overflow.bottom;
        GUI.Box(position, title, guiStyle);            
        EditorGUIUtility.AddCursorRect (new Rect(position.x,  position.y, position.width, WD_EditorConfig.NodeTitleHeight), MouseCursor.MoveArrow);
        // Fold/Unfold icon
        if(ShouldDisplayFoldIcon(node, storage)) {
            if(node.IsFolded) {
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
    // Fold/Unfold icon functionality.
    // ----------------------------------------------------------------------
    public bool IsFoldIconPressed(WD_EditorObject obj, Vector2 mousePos, WD_Storage storage) {
        if(!ShouldDisplayFoldIcon(obj, storage)) return false;
        Rect foldIconPos= GetFoldIconPosition(obj, storage);
        return foldIconPos.Contains(mousePos);
    }
    bool ShouldDisplayFoldIcon(WD_EditorObject obj, WD_Storage storage) {
        if(obj.IsMinimized) return false;
        if(obj.IsModule || obj.IsStateChart || obj.IsState || obj.IsClass) {
            if(obj.IsClass) {
                bool needFoldIcon= false;
                storage.EditorObjects.ForEachChild(obj, (child) => { if(child.IsNode) needFoldIcon= true; });
                return needFoldIcon;
            }
            return true;
        }
        return false;
    }
    Rect GetFoldIconPosition(WD_EditorObject obj, WD_Storage storage) {
        Rect objPos= storage.EditorObjects.GetPosition(obj);
        return new Rect(objPos.x+8, objPos.y, foldedIcon.width, foldedIcon.height);
    }
    // ======================================================================
    // Minimize icon functionality
    // ----------------------------------------------------------------------
    public bool IsMinimizeIconPressed(WD_EditorObject obj, Vector2 mousePos, WD_Storage storage) {
        if(!ShouldDisplayMinimizeIcon(obj, storage)) return false;
        Rect minimizeIconPos= GetMinimizeIconPosition(obj, storage);
        return minimizeIconPos.Contains(mousePos);
    }
    bool ShouldDisplayMinimizeIcon(WD_EditorObject obj, WD_Storage storage) {
        return obj.InstanceId != 0 && obj.IsNode && !obj.IsMinimized;
    }
    Rect GetMinimizeIconPosition(WD_EditorObject obj, WD_Storage storage) {
        Rect objPos= storage.EditorObjects.GetPosition(obj);
        return new Rect(objPos.xMax-4-minimizeIcon.width, objPos.y, minimizeIcon.width, minimizeIcon.height);
    }
    // ======================================================================
    // Maximize icon functionality
    // ----------------------------------------------------------------------
    static Texture2D GetMaximizeIcon(WD_EditorObject node, NodeStyle nodeStyle, WD_Storage storage) {
        Texture2D icon= null;
        if(node.Icon != null && node.Icon != "") {
            icon= LoadIcon(node.Icon, storage);
//            icon.Resize(24,24,TextureFormat.ARGB32,false);
//            icon.Apply();    
            if(icon == null) {
                Debug.LogWarning("Unable to load Icon: "+node.Icon);
            }            
        }
        if(icon == null) {
            icon= nodeStyle.maximizeIcon;            
        }
        return icon;       
    }
    // ----------------------------------------------------------------------
    public bool IsMaximizeIconPressed(WD_EditorObject obj, Vector2 mousePos, WD_Storage storage) {
        if(!ShouldDisplayMaximizeIcon(obj, storage)) return false;
        Rect maximizeIconPos= GetMaximizeIconPosition(obj, storage);
        return maximizeIconPos.Contains(mousePos);
    }
    bool ShouldDisplayMaximizeIcon(WD_EditorObject obj, WD_Storage storage) {
        return obj.InstanceId != 0 && obj.IsNode && obj.IsMinimized;
    }
    Rect GetMaximizeIconPosition(WD_EditorObject obj, WD_Storage storage) {
        return storage.EditorObjects.GetPosition(obj);
    }
    // ======================================================================
    // Node style functionality
    // ----------------------------------------------------------------------
    NodeStyle GetNodeStyle(WD_EditorObject node, WD_EditorObject selectedObject, WD_Storage storage) {
        // Node background is dependant on node type.
//        WD_Node runtimeNode= storage.EditorObjects.GetRuntimeObject(node) as WD_Node;
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
        if(node.IsClass) {
            GenerateNodeStyle(ref classStyle, storage.Preferences.NodeColors.ClassColor);
            return classStyle;
        }
        if(node.IsFunction || node.IsConversion) {
            GenerateNodeStyle(ref functionStyle, storage.Preferences.NodeColors.FunctionColor);
            return functionStyle;
        }
        GenerateNodeStyle(ref defaultStyle, Color.gray);
        return defaultStyle;
    }
    GUIStyle GetNodeGUIStyle(WD_EditorObject node, WD_EditorObject selectedObject, WD_Storage storage) {
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        return nodeStyle.guiStyle;
    }
    
    // ----------------------------------------------------------------------
    // Returns the display color of the given node.
    Color GetNodeColor(WD_EditorObject node, WD_EditorObject selectedObject, WD_Storage storage) {
        NodeStyle nodeStyle= GetNodeStyle(node, selectedObject, storage);
        return nodeStyle.nodeColor;
    }
    
    // ======================================================================
    //  PORT
    // ----------------------------------------------------------------------
    public void DrawPort(WD_EditorObject port, WD_EditorObject selectedObject, WD_Storage storage) {
        // Only draw visible data ports.
        if(storage.EditorObjects.IsVisible(port) == false) return;
        // Don't draw ports on minimized node.
        if(storage.EditorObjects.IsMinimized(port)) return;
        
        // Build visible port name
        WD_EditorObject portParent= storage.EditorObjects[port.ParentId];
        Type portValueType= port.RuntimeType;
        string name= portValueType.IsArray ? "["+port.Name+"]" : port.Name;
         
        Rect tmp= storage.EditorObjects.GetPosition(port);
        Vector2 pos= new Vector2(tmp.x, tmp.y);
        Color portColor= storage.Preferences.TypeColors.GetColor(portValueType);
        Color nodeColor= GetNodeColor(portParent, selectedObject, storage);
        DrawPort(WD_Graphics.PortShape.Circular, pos, portColor, nodeColor);                                        
        // Show name if requested.
        Vector2 labelSize= WD_EditorConfig.GetPortLabelSize(name);
        if(port.IsOnLeftEdge) {                
            pos.x+= 1 + WD_EditorConfig.PortSize;
            pos.y-= 1 + 0.5f * labelSize.y;
        }
        if(port.IsOnRightEdge) {
            pos.x-= 1 + labelSize.x + WD_EditorConfig.PortSize;
            pos.y-= 1 + 0.5f * labelSize.y;        
        }
        GUI.Label(new Rect(pos.x, pos.y, labelSize.x, labelSize.y), name);
    }
    public enum PortShape { Circular, Square, Diamond, UpTriangle, DownTriangle, LeftTriangle, RightTriangle };
    public void DrawPort(PortShape _shape, Vector3 _center, Color _fillColor, Color _borderColor) {
        // Readjust to screen coordinates.
        Vector3 center= new Vector3(_center.x-drawOffset.x, _center.y-drawOffset.y, _center.z);

        // Configure move cursor for port.
        EditorGUIUtility.AddCursorRect (new Rect(center.x-WD_EditorConfig.PortRadius,
                                                 center.y-WD_EditorConfig.PortRadius,
                                                 WD_EditorConfig.PortSize,
                                                 WD_EditorConfig.PortSize),
                                        MouseCursor.MoveArrow);   

        // Activate fill color.
        Handles.color= _fillColor;
        
        // Draw specific shapes.
        switch(_shape) {
            case PortShape.Circular:       DrawCircularPort(center, _fillColor, _borderColor); break;
            case PortShape.Square:         DrawSquarePort(center, _borderColor); break;
            case PortShape.Diamond:        DrawDiamondPort(center, _borderColor); break;
            case PortShape.UpTriangle:     DrawUpTrianglePort(center, _borderColor); break;
            case PortShape.DownTriangle:   DrawDownTrianglePort(center, _borderColor); break;
            case PortShape.LeftTriangle:   DrawLeftTrianglePort(center, _borderColor); break;
            case PortShape.RightTriangle:  DrawRightTrianglePort(center, _borderColor); break;
        }        
    }

	// ----------------------------------------------------------------------
    void DrawCircularPort(Vector3 _center, Color _fillColor, Color _borderColor) {
        Handles.color= Color.black;
        Handles.DrawSolidDisc(_center, FacingNormal, WD_EditorConfig.PortRadius+2.0f);
        Handles.color= _fillColor;
        Handles.DrawSolidDisc(_center, FacingNormal, WD_EditorConfig.PortRadius);
        Handles.color= _borderColor;
        Handles.DrawWireDisc(_center, FacingNormal, WD_EditorConfig.PortRadius+2.0f);
    }

	// ----------------------------------------------------------------------
    void DrawSquarePort(Vector3 _center, Color _borderColor) {
        // Draw connector.
        Vector3[] vectors= new Vector3[5];
        Handles.DrawSolidDisc(_center, FacingNormal, WD_EditorConfig.PortRadius);
        float delta= WD_EditorConfig.PortRadius-1;
        float minSize= 0.707f * WD_EditorConfig.PortRadius;
        for(; delta > minSize; --delta) {
            vectors[0]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[3]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[4]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= WD_EditorConfig.PortRadius;
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
        float radius= WD_EditorConfig.PortRadius+1;
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
        float radius= WD_EditorConfig.PortRadius+1;
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
        float radius= WD_EditorConfig.PortRadius+1;
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
        float radius= WD_EditorConfig.PortRadius+1;
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
        float radius= WD_EditorConfig.PortRadius+1;
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


    // ======================================================================
    //  CONNECTION
    // ----------------------------------------------------------------------
    public void DrawConnection(WD_EditorObject port, WD_EditorObject selectedObject, WD_Storage storage) {
        if(storage.EditorObjects.IsVisible(port.ParentId)) {
            if(storage.EditorObjects.IsValid(port.Source)) {
                WD_EditorObject source= storage.EditorObjects[port.Source];
                WD_EditorObject sourceParent= storage.EditorObjects[source.ParentId];
                if(storage.EditorObjects.IsVisible(sourceParent)) {
                    Rect sourcePos= storage.EditorObjects.GetPosition(source);
                    Rect portPos  = storage.EditorObjects.GetPosition(port);
                    Vector2 start= new Vector2(sourcePos.x, sourcePos.y);
                    Vector2 end= new Vector2(portPos.x, portPos.y);
                    Vector2 startDirection= ConnectionDirectionForTo(source, port, storage);
                    Vector2 endDirection= ConnectionDirectionForTo(port, source, storage);
//                    Vector2 startDirection= source.IsOnHorizontalEdge ? DownDirection : RightDirection;
//                    Vector2 endDirection= port.IsOnHorizontalEdge ? UpDirection : LeftDirection;
//                    Vector2 diff= end-start;
//                    if(Vector2.Dot(diff, startDirection) < 0) {
//                        startDirection= -startDirection;
//                    }
//                    if(Vector2.Dot(diff, endDirection) > 0) {
//                        endDirection  = - endDirection;
//                    }
                    Color color= storage.Preferences.TypeColors.GetColor(source.RuntimeType);
                    DrawBezierCurve(start, end, startDirection, endDirection, color);
                }                                    
            }
        }        
    }
    // ----------------------------------------------------------------------
    Vector2 ConnectionDirectionForTo(WD_EditorObject port, WD_EditorObject to, WD_Storage storage) {
        Vector2 direction;
        if(port.IsOnLeftEdge) {
            direction= LeftDirection;
        } else if(port.IsOnRightEdge) {
            direction= RightDirection;
        } else if(port.IsOnTopEdge) {
            direction= UpDirection;
        } else {
            direction= DownDirection;
        }
        // Inverse direction for connection between nested nodes.
        WD_EditorObject portParent= storage.EditorObjects[port.ParentId];
        WD_EditorObject toParent= storage.EditorObjects[to.ParentId];
        if(storage.EditorObjects.IsChildOf(toParent, portParent)) {
            direction= -direction;
        }
        return direction;
    }
    // ----------------------------------------------------------------------
    public void DrawBezierCurve(Vector3 _start, Vector3 _end, Vector3 _startDir, Vector3 _endDir, Color _color) {
        // Readjust to screen coordinates.
        Vector3 start= new Vector3(_start.x-drawOffset.x, _start.y-drawOffset.y, _start.z);
        Vector3 end  = new Vector3(_end.x-drawOffset.x  , _end.y-drawOffset.y  , _end.z);
        
		// Determine weight of the tangents.
		Vector3 vertex= end-start;

        // Compute Bezier tangents.
        Vector3 startTangent= start + _startDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector3.Dot(_startDir, vertex)));
        Vector3 endTangent  = end + _endDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector3.Dot(_endDir, vertex)));

        // Use a Bezier for the connections.
        Color color= new Color(_color.r, _color.g, _color.b, 0.5f*_color.a);
		Handles.DrawBezier(start, end, startTangent, endTangent, color, lineTexture, 1.5f);
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
