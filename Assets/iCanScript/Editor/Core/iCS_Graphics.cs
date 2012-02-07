using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_Graphics {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const float kMinimizeSize    = 32f;
    const float kNodeCornerRadius= 8f;
    const float kNodeTitleHeight = 2f*kNodeCornerRadius;
    const int   kLabelFontSize   = 11;
    const int   kTitleFontSize   = 12;
    
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
    GUIStyle    LabelStyle              = null;
    GUIStyle    TitleStyle              = null;
    Texture2D   StateMaximizeIcon       = null;
    Texture2D   ModuleMaximizeIcon      = null;
    Texture2D   EntryStateMaximizeIcon  = null;
    Texture2D   ConstructionMaximizeIcon= null;
    Texture2D   FunctionMaximizeIcon    = null;
    Texture2D   DefaultMaximizeIcon     = null;

    // ----------------------------------------------------------------------
    iCS_EditorObject selectedObject= null;
    float            Scale= 1f;
    Vector2          Translation= Vector2.zero;
    Rect             ClipingArea= new Rect(0,0,0,0);
    Vector2          MousePosition= Vector2.zero;
    
    // ======================================================================
	// CONSTANTS
    // ----------------------------------------------------------------------
    public static readonly Vector2 UpDirection     = new Vector2(0,-1);
    public static readonly Vector2 DownDirection   = new Vector2(0,1);
    public static readonly Vector2 RightDirection  = new Vector2(1,0);
    public static readonly Vector2 LeftDirection   = new Vector2(-1,0);
    public static readonly Vector3 FacingNormal    = new Vector3(0,0,-1);
	       static readonly Color   BackgroundColor = new Color(0.24f, 0.27f, 0.32f);
	       static readonly Color   BlackShadowColor= new Color(0,0,0,0.06f);
	       static readonly Color   WhiteShadowColor= new Color(1f,1f,1f,0.06f);
        
    // ======================================================================
    // Construction/Destruction
    // ----------------------------------------------------------------------
    public iCS_Graphics() {
        // Load title style.
        BuildTitleStyle();
    }
    
    // ======================================================================
    // Drawing staging
	// ----------------------------------------------------------------------
    public void Begin(Vector2 translation, float scale, Rect clipingRect, iCS_EditorObject selObj, Vector2 mousePos, iCS_IStorage storage) {
        Translation= translation;
        Scale= scale;
        ClipingArea= clipingRect;
        MousePosition= mousePos;
        selectedObject= selObj;

        // Rebuild label style to match user preferences.
        BuildLabelStyle(storage);
        
        // Set font size according to scale.
        LabelStyle.fontSize= (int)(kLabelFontSize*Scale);
        TitleStyle.fontSize= (int)(kTitleFontSize*Scale);        
    }
    public void End() {
    }
    
    // ======================================================================
    // GUI Warppers
	// ----------------------------------------------------------------------
    bool IsVisible(Vector2 point, float radius= 0f) {
        if(ClipingArea.Contains(new Vector2(point.x+radius, point.y))) return true;
        if(ClipingArea.Contains(new Vector2(point.x-radius, point.y))) return true;
        if(ClipingArea.Contains(new Vector2(point.x, point.y+radius))) return true;
        if(ClipingArea.Contains(new Vector2(point.x, point.y-radius))) return true;
        return false;
    }
	// ----------------------------------------------------------------------
    bool IsVisble(Rect r) {
        Rect intersection= Clip(r);
        return Math3D.IsNotZero(intersection.width);        
    }
	// ----------------------------------------------------------------------
    bool IsFullyVisible(Rect r) {
        return (IsVisible(new Vector2(r.x, r.y)) && IsVisible(new Vector2(r.xMax, r.yMax)));
    }
	// ----------------------------------------------------------------------
    Rect Clip(Rect r) {
        return Math3D.Intersection(r, ClipingArea);
    }
	// ----------------------------------------------------------------------
    Vector2 TranslateAndScale(Vector2 v) {
        return Scale*(v-Translation);
    }
	// ----------------------------------------------------------------------
    Vector3 TranslateAndScale(Vector3 v) {
        return Scale*(v-new Vector3(Translation.x, Translation.y, 0));
    }
	// ----------------------------------------------------------------------
    Rect TranslateAndScale(Rect r) {
        Vector2 pos= TranslateAndScale(new Vector2(r.x, r.y));
        return new Rect(pos.x, pos.y, Scale*r.width, Scale*r.height);
    }
	// ----------------------------------------------------------------------
    Vector2 TranslateAndScale(float x, float y) {
        return new Vector2(Scale*(x-Translation.x), Scale*(y-Translation.y));
    }
    // ----------------------------------------------------------------------
    void GUI_Box(Rect pos, GUIContent content, Color nodeColor, Color backgroundColor, Color shadowColor) {
        Rect adjPos= TranslateAndScale(pos);
        DrawNode(adjPos, nodeColor, backgroundColor, shadowColor, content);
        string tooltip= content.tooltip;
        if(tooltip != null && tooltip != "") {
            GUI.Label(adjPos, new GUIContent("", tooltip), LabelStyle);
        }
    }
    // ----------------------------------------------------------------------
    void GUI_DrawTexture(Rect pos, Texture texture) {
        GUI.DrawTexture(TranslateAndScale(pos), texture, ScaleMode.ScaleToFit);                  
    }
    // ----------------------------------------------------------------------
    void EditorGUIUtility_AddCursorRect(Rect rect, MouseCursor cursor) {
        EditorGUIUtility.AddCursorRect(TranslateAndScale(rect), cursor);
    }
    // ----------------------------------------------------------------------
    void GUI_Label(Rect pos, GUIContent content, GUIStyle labelStyle) {
        if(Scale < 0.5f) return;
        GUI.Label(TranslateAndScale(pos), content, labelStyle);
    }
    // ----------------------------------------------------------------------
    void GUI_Label(Rect pos, String content, GUIStyle labelStyle) {
        if(Scale < 0.5f) return;
        GUI.Label(TranslateAndScale(pos), content, labelStyle);
    }
    // ----------------------------------------------------------------------
    void DrawNode(Rect r, Color nodeColor, Color backgroundColor, Color shadowColor, GUIContent content) {
        float radius= kNodeCornerRadius;
        radius*= Scale;
        
        // Show shadow.
        Vector3[] vectors= new Vector3[4];
//        Color shadowColor= new Color(1f,1f,1f,0.05f);
        for(int i= 5; i > 0; --i) {
            Handles.color= shadowColor;
            Handles.DrawSolidArc(new Vector3(i+r.xMax-radius, i+r.y+radius), FacingNormal, new Vector3(1f,0,0), 90f, radius);
            Handles.DrawSolidArc(new Vector3(i+r.xMax-radius, i+r.yMax-radius), FacingNormal, new Vector3(0,1f,0), 90f, radius);
            Handles.DrawSolidArc(new Vector3(i+r.x+radius, i+r.yMax-radius), FacingNormal, new Vector3(-1f,0,0), 90f, radius);
            vectors[0]= new Vector3(i+r.xMax-radius, i+r.y+radius, 0);
            vectors[1]= new Vector3(i+r.xMax, i+r.y+radius, 0);
            vectors[2]= new Vector3(i+r.xMax, i+r.yMax-radius, 0);
            vectors[3]= new Vector3(i+r.xMax-radius, i+r.yMax-radius, 0);
            Handles.color= Color.white;
            Handles.DrawSolidRectangleWithOutline(vectors, shadowColor, new Color(0,0,0,0));
            vectors[0]= new Vector3(i+r.x+radius, i+r.yMax-radius, 0);
            vectors[1]= new Vector3(i+r.xMax-radius, i+r.yMax-radius, 0);
            vectors[2]= new Vector3(i+r.xMax-radius, i+r.yMax, 0);
            vectors[3]= new Vector3(i+r.x+radius, i+r.yMax, 0);
            Handles.color= Color.white;
            Handles.DrawSolidRectangleWithOutline(vectors, shadowColor, new Color(0,0,0,0));
        }
        
        // Show background.
        Handles.color= backgroundColor;
        Handles.DrawSolidArc(new Vector3(r.x+radius, r.yMax-radius,0), FacingNormal, new Vector3(-1f,0,0), 90f, radius);
        Handles.DrawSolidArc(new Vector3(r.xMax-radius, r.yMax-radius,0), FacingNormal, new Vector3(0,1f,0), 90f, radius);
        vectors[0]= new Vector3(r.x, r.y+radius, 0);
        vectors[1]= new Vector3(r.xMax, r.y+radius, 0);
        vectors[2]= new Vector3(r.xMax, r.yMax-radius, 0);
        vectors[3]= new Vector3(r.x, r.yMax-radius, 0);
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vectors, backgroundColor, new Color(0,0,0,0));
        vectors[0]= new Vector3(r.x+radius, r.yMax-radius, 0);
        vectors[1]= new Vector3(r.xMax-radius, r.yMax-radius, 0);
        vectors[2]= new Vector3(r.xMax-radius, r.yMax, 0);
        vectors[3]= new Vector3(r.x+radius, r.yMax, 0);
        Handles.DrawSolidRectangleWithOutline(vectors, backgroundColor, new Color(0,0,0,0));
        
        // Show frame.
        Handles.color= nodeColor;
        Handles.DrawSolidArc(new Vector3(r.x+radius, r.y+radius,0), FacingNormal, new Vector3(0,-1f,0), 90f, radius);
        Handles.DrawSolidArc(new Vector3(r.xMax-radius, r.y+radius,0), FacingNormal, new Vector3(1f,0,0), 90f, radius);
        Handles.DrawWireArc(new Vector3(r.x+radius, r.yMax-radius,0), FacingNormal, new Vector3(-1f,0,0), 90f, radius);
        Handles.DrawWireArc(new Vector3(r.xMax-radius, r.yMax-radius,0), FacingNormal, new Vector3(0,1f,0), 90f, radius);
        Handles.DrawLine(new Vector3(r.x,r.y+radius,0), new Vector3(r.x, r.yMax-radius,0));
        Handles.DrawLine(new Vector3(r.xMax,r.y+radius,0), new Vector3(r.xMax, r.yMax-radius,0));
        Handles.DrawLine(new Vector3(r.x+radius,r.yMax,0), new Vector3(r.xMax-radius, r.yMax,0));
        vectors[0]= new Vector3(r.x+radius, r.y, 0);
        vectors[1]= new Vector3(r.xMax-radius, r.y, 0);
        vectors[2]= new Vector3(r.xMax-radius, r.y+radius, 0);
        vectors[3]= new Vector3(r.x+radius, r.y+radius, 0);
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vectors, nodeColor, new Color(0,0,0,0));
        vectors[0]= new Vector3(r.x, r.y+radius, 0);
        vectors[1]= new Vector3(r.xMax, r.y+radius, 0);
        vectors[2]= new Vector3(r.xMax, r.y+1.75f*radius, 0);
        vectors[3]= new Vector3(r.x, r.y+1.75f*radius, 0);
        Handles.DrawSolidRectangleWithOutline(vectors, nodeColor, new Color(0,0,0,0));

        // Show title.
        if(Scale < 0.4f) return;
        Vector2 titleCenter= new Vector2(0.5f*(r.x+r.xMax), r.y+0.8f*radius);
        Vector2 titleSize= TitleStyle.CalcSize(content);
        GUI.Label(new Rect(titleCenter.x-0.5f*titleSize.x, titleCenter.y-0.5f*titleSize.y, titleSize.x, titleSize.y), content, TitleStyle);
    }
    
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
    void BuildLabelStyle(iCS_IStorage storage) {
        Color labelColor= storage.Preferences.NodeColors.LabelColor;
        if(LabelStyle == null) LabelStyle= new GUIStyle();
        LabelStyle.normal.textColor= labelColor;
        LabelStyle.hover.textColor= labelColor;
        LabelStyle.focused.textColor= labelColor;
        LabelStyle.active.textColor= labelColor;
        LabelStyle.onNormal.textColor= labelColor;
        LabelStyle.onHover.textColor= labelColor;
        LabelStyle.onFocused.textColor= labelColor;
        LabelStyle.onActive.textColor= labelColor;
    }
    // ----------------------------------------------------------------------
    void BuildTitleStyle() {
        Color titleColor= Color.black;
        if(TitleStyle == null) TitleStyle= new GUIStyle();
        TitleStyle.normal.textColor= titleColor;
        TitleStyle.hover.textColor= titleColor;
        TitleStyle.focused.textColor= titleColor;
        TitleStyle.active.textColor= titleColor;
        TitleStyle.onNormal.textColor= titleColor;
        TitleStyle.onHover.textColor= titleColor;
        TitleStyle.onFocused.textColor= titleColor;
        TitleStyle.onActive.textColor= titleColor;
        TitleStyle.fontStyle= FontStyle.Bold;
        TitleStyle.fontSize= 12;
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
    public void DrawIconCenteredAt(Vector2 point, Texture2D icon) {
        if(icon == null) return;
        GUI_DrawTexture(new Rect(point.x-0.5f*icon.width,point.y-0.5f*icon.height, icon.width, icon.height), icon);
    }
    
    
    // ======================================================================
    //  GRID
    // ----------------------------------------------------------------------
    public void DrawGrid(Rect screenArea, Color backgroundColor, Color gridColor, float gridSpacing) {
        // Draw background.
        Vector3[] vect= { new Vector3(0,0,0),
                          new Vector3(screenArea.width, 0, 0),
                          new Vector3(screenArea.width,screenArea.height,0),
                          new Vector3(0,screenArea.height,0)};
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vect, backgroundColor, backgroundColor);

        // Draw grid lines.
        if(gridSpacing*Scale < 2) return;
        
        float gridSpacing5= 5f*gridSpacing;
        float x= (-Translation.x)-gridSpacing*Mathf.Floor((-Translation.x)/gridSpacing);
        float y= (-Translation.y)-gridSpacing*Mathf.Floor((-Translation.y)/gridSpacing);
        float x5= (-Translation.x)-gridSpacing5*Mathf.Floor((-Translation.x)/gridSpacing5);
        float y5= (-Translation.y)-gridSpacing5*Mathf.Floor((-Translation.y)/gridSpacing5);
        
        // Scale grid
        x*= Scale;
        y*= Scale;
        x5*= Scale;
        y5*=Scale;
        gridSpacing*= Scale;
        gridSpacing5*= Scale;
        
        Color gridColor2= new Color(gridColor.r, gridColor.g, gridColor.b, 0.25f);
        for(; x < screenArea.width; x+= gridSpacing) {
            if(Mathf.Abs(x-x5) < 1f) {
                Handles.color= gridColor;
                x5+= gridSpacing5;
            } else {
                Handles.color= gridColor2;                
            }
            Handles.DrawLine(new Vector3(x,0,0), new Vector3(x,screenArea.height,0));            
        }
        for(; y < screenArea.height; y+= gridSpacing) {
            if(Mathf.Abs(y-y5) < 1f) {
                Handles.color= gridColor;
                y5+= gridSpacing5;
            } else {
                Handles.color= gridColor2;                
            }
            Handles.DrawLine(new Vector3(0,y,0), new Vector3(screenArea.width,y,0));            
        }        
    }
    
    // ======================================================================
    //  NODE
    // ----------------------------------------------------------------------
    public void DrawNormalNode(iCS_EditorObject node, iCS_IStorage storage) {        
        // Don't draw minimized node.
        if(IsInvisible(node, storage) || IsMinimized(node, storage)) return;
        
        // Draw node box.
        Rect position= GetDisplayPosition(node, storage);
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.Name));
        // Change background color if node is selected.
        Color backgroundColor= BackgroundColor;
        if(node == selectedObject) {
            float adj= storage.Preferences.NodeColors.SelectedBrightness;
            backgroundColor= new Color(adj*BackgroundColor.r, adj*BackgroundColor.g, adj*BackgroundColor.b);
        }
        bool isMouseOver= position.Contains(MousePosition);
        GUI_Box(position, new GUIContent(title,node.ToolTip), GetNodeColor(node, storage), backgroundColor, isMouseOver ? WhiteShadowColor : BlackShadowColor);
        EditorGUIUtility_AddCursorRect (new Rect(position.x,  position.y, position.width, kNodeTitleHeight), MouseCursor.Link);
        // Fold/Unfold icon
        if(ShouldDisplayFoldIcon(node, storage)) {
            if(storage.IsFolded(node)) {
                GUI_DrawTexture(new Rect(position.x+6f, position.y, foldedIcon.width, foldedIcon.height), foldedIcon);                           
            } else {
                GUI_DrawTexture(new Rect(position.x+6f, position.y, unfoldedIcon.width, unfoldedIcon.height), unfoldedIcon);               
            }            
        }
        // Minimize Icon
        if(ShouldDisplayMinimizeIcon(node, storage)) {
            GUI_DrawTexture(new Rect(position.xMax-4-minimizeIcon.width, position.y, minimizeIcon.width, minimizeIcon.height), minimizeIcon);
        }
    }
    // ----------------------------------------------------------------------
    public void DrawMinimizedNode(iCS_EditorObject node, iCS_IStorage storage) {        
        if(!IsMinimized(node, storage)) return;
        
        // Draw minimized node.
        Rect position= GetDisplayPosition(node, storage);
        Texture icon= GetMaximizeIcon(node, storage);
        if(position.width < 12f || position.height < 12f) return;  // Don't show if too small.
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.Name));
        Rect texturePos= new Rect(position.x, position.y, icon.width, icon.height);                
        GUI_DrawTexture(texturePos, icon);                           
        EditorGUIUtility_AddCursorRect (texturePos, MouseCursor.Link);
        GUI_Label(texturePos, new GUIContent("", node.ToolTip), LabelStyle);
        Vector2 labelSize= iCS_EditorConfig.GetPortLabelSize(title);
        GUI_Label(new Rect(0.5f*(texturePos.x+texturePos.xMax-labelSize.x), texturePos.y-labelSize.y, labelSize.x, labelSize.y), new GUIContent(title, node.ToolTip), LabelStyle);
    }

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
    public static Vector2 GetMaximizeIconSize(iCS_EditorObject node, iCS_IStorage storage) {
        Texture2D icon= null;
        if(storage.Preferences.Icons.EnableMinimizedIcons && node.IconGUID != null) {
            icon= GetCachedIconFromGUID(node.IconGUID);
            if(icon != null) return new Vector2(icon.width, icon.height);
        }
        return new Vector2(maximizeIcon.width, maximizeIcon.height);        
    }
    // ----------------------------------------------------------------------
    public Texture2D GetMaximizeIcon(iCS_EditorObject node, iCS_IStorage storage) {
        Texture2D icon= null;
        if(storage.Preferences.Icons.EnableMinimizedIcons && node.IconGUID != null) {
            icon= GetCachedIconFromGUID(node.IconGUID);
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
    // Node style functionality
    // ----------------------------------------------------------------------
    // Returns the display color of the given node.
    static Color GetNodeColor(iCS_EditorObject node, iCS_IStorage storage) {
        if(node.IsEntryState) {
            return storage.Preferences.NodeColors.EntryStateColor;
        }
        if(node.IsState || node.IsStateChart) {
            return storage.Preferences.NodeColors.StateColor;
        }
        if(node.IsModule) {
            return storage.Preferences.NodeColors.ModuleColor;
        }
        if(node.IsConstructor) {
            return storage.Preferences.NodeColors.ConstructorColor;
        }
        if(node.IsStaticMethod || node.IsConversion || node.IsInstanceMethod || node.IsInstanceField || node.IsStaticField) {
            return storage.Preferences.NodeColors.FunctionColor;
        }
        return Color.gray;
    }
    // ----------------------------------------------------------------------
    // Returns the maximize icon for the given node.
    Texture2D GetNodeDefaultMaximizeIcon(iCS_EditorObject node, iCS_IStorage storage) {
        if(node.IsEntryState) {
            return BuildMaximizeIcon(node, storage, ref EntryStateMaximizeIcon);
        }
        if(node.IsState || node.IsStateChart) {
            return BuildMaximizeIcon(node, storage, ref StateMaximizeIcon);
        }
        if(node.IsModule) {
            return BuildMaximizeIcon(node, storage, ref ModuleMaximizeIcon);
        }
        if(node.IsConstructor) {
            return BuildMaximizeIcon(node, storage, ref ConstructionMaximizeIcon);
        }
        if(node.IsStaticMethod || node.IsConversion || node.IsInstanceMethod || node.IsInstanceField || node.IsStaticField) {
            return BuildMaximizeIcon(node, storage, ref FunctionMaximizeIcon);
        }
        return BuildMaximizeIcon(node, storage, ref DefaultMaximizeIcon);
    }
    // ----------------------------------------------------------------------
    Texture2D BuildMaximizeIcon(iCS_EditorObject node, iCS_IStorage storage, ref Texture2D icon) {
        if(icon == null) {
            Color nodeColor= GetNodeColor(node, storage);
            icon= new Texture2D(maximizeIcon.width, maximizeIcon.height);
            for(int x= 0; x < maximizeIcon.width; ++x) {
                for(int y= 0; y < maximizeIcon.height; ++y) {
                    icon.SetPixel(x, y, nodeColor * maximizeIcon.GetPixel(x,y));
                }
            }
            icon.Apply();
            icon.hideFlags= HideFlags.DontSave;
        }
        return icon;        
    }
    
    // ======================================================================
    //  PORT
    // ----------------------------------------------------------------------
    public void DrawPort(iCS_EditorObject port, iCS_IStorage storage) {
        // Update display position.
        Rect position= GetDisplayPosition(port, storage);

        // Only draw visible data ports.
        if(IsInvisible(port, storage) || IsMinimized(port, storage)) return;
        
        // Draw port
        iCS_EditorObject portParent= storage.GetParent(port);         
        Vector2 center= Math3D.ToVector2(position);
        Type portValueType= port.RuntimeType;
        Color portColor= storage.Preferences.TypeColors.GetColor(portValueType);
        Color nodeColor= GetNodeColor(portParent, storage);
		object portValue= null;
		float portRadius= port == selectedObject ? 1.67f*iCS_EditorConfig.PortRadius : iCS_EditorConfig.PortRadius;
        if(port.IsDataPort) {
    		if(Application.isPlaying && storage.Preferences.DisplayOptions.PlayingPortValues) portValue= storage.GetPortValue(port);
			Vector2 portCenter= center;
//			if(!port.IsEnablePort) portCenter.y-= 2;
			if(port.IsInputPort && storage.GetSource(port) == null) {
				if(!Application.isPlaying && storage.Preferences.DisplayOptions.EditorPortValues) portValue= storage.GetPortValue(port);
				if(portValue != null) {
	            	DrawSquarePort(portCenter, portColor, nodeColor, portRadius);
				} else {
		            DrawCircularPort(portCenter, portColor, nodeColor, portRadius);									
				}
			} else {
	            DrawCircularPort(portCenter, portColor, nodeColor, portRadius);				
			}
        } else if(port.IsStatePort) {
            if(port.IsOutStatePort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(TranslateAndScale(center), FacingNormal, portRadius*Scale);
            }
        } else if(port.IsInTransitionPort || port.IsOutTransitionPort) {
            Handles.color= Color.white;
            Handles.DrawSolidDisc(TranslateAndScale(center), FacingNormal, portRadius*Scale);            
        }
        else {
            DrawCircularPort(center, portColor, nodeColor, portRadius);
        }
        // Configure move cursor for port.
        Rect portPos= new Rect(center.x-portRadius*1.5f, center.y-portRadius*1.5f, portRadius*3f, portRadius*3f);
        if(!port.IsTransitionPort) {
            EditorGUIUtility_AddCursorRect (portPos, MouseCursor.Link);            
        }
        if(!port.IsFloating) {
            GUI_Label(portPos, new GUIContent("", port.ToolTip), LabelStyle);            
        }
        
        // Show port label.
        if(port.IsStatePort) return;     // State transition name is handle by DrawConnection. 
        string name= portValueType.IsArray ? "["+port.Name+"]" : port.Name;
		string valueAsStr= portValue != null ? GetValueAsString(portValue) : null;
        Vector2 labelSize= iCS_EditorConfig.GetPortLabelSize(name);
		GUIStyle valueStyle= GUI.skin.textField;
		Vector2 valueSize= (valueAsStr != null && valueAsStr != "") ? valueStyle.CalcSize(new GUIContent(valueAsStr)) : Vector2.zero;
		Vector2 valuePos= center;
        switch(port.Edge) {
            case iCS_EditorObject.EdgeEnum.Left:
                center.x  += 1 + iCS_EditorConfig.PortSize;
                center.y  -= -1 + 0.5f * labelSize.y;
				valuePos.x-= 1 + valueSize.x + iCS_EditorConfig.PortSize;
				valuePos.y-= 1 + 0.5f * valueSize.y;
                break;
            case iCS_EditorObject.EdgeEnum.Right:
                center.x  -= 1 + labelSize.x + iCS_EditorConfig.PortSize;
                center.y  -= -1 + 0.5f * labelSize.y;
				valuePos.x+= 1 + iCS_EditorConfig.PortSize;
				valuePos.y-= 1 + 0.5f * valueSize.y;
                break;
            case iCS_EditorObject.EdgeEnum.Top:            
                center.x-= 1 + 0.5f*labelSize.x;
                center.y-= iCS_EditorConfig.PortSize+0.8f*labelSize.y*(1+TopBottomLabelOffset(port, storage));
				valueAsStr= null;
                break;
            case iCS_EditorObject.EdgeEnum.Bottom:
                center.x-= 1 + 0.5f*labelSize.x;
                center.y+= iCS_EditorConfig.PortSize+0.8f*labelSize.y*TopBottomLabelOffset(port, storage)-0.2f*labelSize.y;
				valueAsStr= null;
                break;
        }
        GUI_Label(new Rect(center.x, center.y, labelSize.x, labelSize.y), new GUIContent(name, port.ToolTip), LabelStyle);
        if(!port.IsFloating) {
    		if(valueAsStr != null) {
    			GUI_Label(new Rect(valuePos.x, valuePos.y, valueSize.x, valueSize.y), valueAsStr, valueStyle);			
    		}            
        }
    }

	// ----------------------------------------------------------------------
    string GetValueAsString(object value) {
        if(value is bool) return ((bool)value) ? "true" : "false";
        if(value is float) return ((float)value).ToString();
        if(value is int) return ((int)value).ToString();
        if(value is Vector2) return ((Vector2)value).ToString();
        if(value is Vector3) return ((Vector3)value).ToString();
        if(value is Vector4) return ((Vector4)value).ToString();
        if(value is Color) return ((Color)value).ToString();
        if(value is string) return (string)value;
        if(value is UnityEngine.Object) return (value as UnityEngine.Object).name;
        return null;
    }
	// ----------------------------------------------------------------------
    void DrawCircularPort(Vector3 _center, Color _fillColor, Color _borderColor, float radius) {
        Color outlineColor= radius > iCS_EditorConfig.PortRadius ? Color.black : Color.black;
        Vector3 center= TranslateAndScale(_center);
        radius*= Scale;
        Handles.color= _borderColor;
        Handles.DrawSolidDisc(center, FacingNormal, radius*1.85f);
        Handles.color= outlineColor;
        Handles.DrawSolidDisc(center, FacingNormal, radius*1.5f);
        Handles.color= _fillColor;
        Handles.DrawSolidDisc(center, FacingNormal, radius);
    }

	// ----------------------------------------------------------------------
    void DrawSquarePort(Vector3 _center, Color _fillColor, Color _borderColor, float radius) {
        Color backgroundColor= radius > iCS_EditorConfig.PortRadius ? Color.black : Color.black;
        radius*= Scale;
        Vector3 center= TranslateAndScale(_center);
        Vector3[] vectors= new Vector3[4];
        float delta= radius*1.5f;

        delta= radius*1.35f;
        vectors[0]= new Vector3(center.x-delta, center.y-delta, 0);
        vectors[1]= new Vector3(center.x-delta, center.y+delta, 0);
        vectors[2]= new Vector3(center.x+delta, center.y+delta, 0);
        vectors[3]= new Vector3(center.x+delta, center.y-delta, 0);
        Handles.color= Color.white;
		Handles.DrawSolidRectangleWithOutline(vectors, backgroundColor, _borderColor);

        delta= radius*0.67f;
        vectors[0]= new Vector3(center.x-delta, center.y-delta, 0);
        vectors[1]= new Vector3(center.x-delta, center.y+delta, 0);
        vectors[2]= new Vector3(center.x+delta, center.y+delta, 0);
        vectors[3]= new Vector3(center.x+delta, center.y-delta, 0);
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vectors, _fillColor, _fillColor);
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
    public void DrawConnection(iCS_EditorObject port, iCS_IStorage storage, bool highlight= false, float lineWidth= 1.5f) {
        iCS_EditorObject portParent= storage.GetParent(port);
        if(IsVisible(portParent, storage) && storage.IsValid(port.Source)) {
            iCS_EditorObject source= storage.GetSource(port);
            iCS_EditorObject sourceParent= storage.GetParent(source);
            if(IsVisible(sourceParent, storage) && !port.IsOutStatePort) {
                // Determine if this connection is part of the selected object.
                float highlightWidth= 2f;
                Color highlightColor= new Color(0.67f, 0.67f, 0.67f);
                if(port == selectedObject || source == selectedObject || portParent == selectedObject || sourceParent == selectedObject) {
                    highlight= true;
                }
                // Determine if this connection is part of a drag.
                bool isFloating= (port.IsFloating || source.IsFloating);
                if(isFloating) {
                    highlight= false;
                }
                Color color= storage.Preferences.TypeColors.GetColor(source.RuntimeType);
                iCS_ConnectionParams cp= new iCS_ConnectionParams(port, GetDisplayPosition(port, storage), source, GetDisplayPosition(source, storage), storage);
                Vector3 startPos= TranslateAndScale(cp.Start);
                Vector3 endPos= TranslateAndScale(cp.End);
                Vector3 startTangent= TranslateAndScale(cp.StartTangent);
                Vector3 endTangent= TranslateAndScale(cp.EndTangent);
                lineWidth= Scale*lineWidth;
                if(lineWidth < 1f) lineWidth= 1f;
                if(highlight) {
                    highlightWidth= Scale*highlightWidth;
                    if(highlightWidth < 1f) highlightWidth= 1f;
            		Handles.DrawBezier(startPos, endPos, startTangent, endTangent, highlightColor, lineTexture, lineWidth+highlightWidth);                    
                }
        		Handles.DrawBezier(startPos, endPos, startTangent, endTangent, color, lineTexture, lineWidth);
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
                       if(!edObj.IsFloating) {
                           storage.StartAnimTimer(edObj);
                           return displayPosition;                           
                       }
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
	// ----------------------------------------------------------------------
	// Returns the time ratio of the animation between 0 and 1.
    static float GetAnimationRatio(iCS_EditorObject edObj, iCS_IStorage storage) {
        float time= storage.Preferences.Animation.AnimationTime;
        float invTime= Math3D.IsZero(time) ? 10000f : 1f/time;
        return invTime*(storage.GetAnimTime(edObj));        
    }
	// ----------------------------------------------------------------------
    // Returns true if the animation ratio >= 1.
    static bool IsAnimationCompleted(iCS_EditorObject edObj, iCS_IStorage storage) {
        return GetAnimationRatio(edObj, storage) >= 0.99f;
    }

    bool IsMinimized(iCS_EditorObject edObj, iCS_IStorage storage) {
        if(edObj.IsNode) {
            if(IsInvisible(edObj, storage)) return false;
            Rect position= GetDisplayPosition(edObj, storage);
            Texture icon= GetMaximizeIcon(edObj, storage);
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
