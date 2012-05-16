using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_Graphics {
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
    static Texture2D    foldedIcon        = null;
    static Texture2D    unfoldedIcon      = null;
    static Texture2D    minimizeIcon      = null;
    static Texture2D    maximizeIcon      = null;
    static Texture2D    upArrowHeadIcon   = null;
    static Texture2D    downArrowHeadIcon = null;
    static Texture2D    leftArrowHeadIcon = null;
    static Texture2D    rightArrowHeadIcon= null;
	
    // ----------------------------------------------------------------------
    public GUIStyle    LabelStyle              = null;
    public GUIStyle    TitleStyle              = null;
    public GUIStyle    ValueStyle              = null;
    public Texture2D   StateMaximizeIcon       = null;
    public Texture2D   ModuleMaximizeIcon      = null;
    public Texture2D   EntryStateMaximizeIcon  = null;
    public Texture2D   ConstructionMaximizeIcon= null;
    public Texture2D   FunctionMaximizeIcon    = null;
    public Texture2D   DefaultMaximizeIcon     = null;

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
    // Drawing staging
	// ----------------------------------------------------------------------
    public void Begin(Vector2 translation, float scale, Rect clipingRect, iCS_EditorObject selObj, Vector2 mousePos, iCS_IStorage iStorage) {
        Translation= translation;
        Scale= scale;
        ClipingArea= clipingRect;
        MousePosition= mousePos;
        selectedObject= selObj;

        // Rebuild label style to match user preferences.
        BuildLabelStyle(iStorage);
        BuildTitleStyle(iStorage);
        BuildValueStyle(iStorage);
        
        // Set font size according to scale.
        LabelStyle.fontSize= (int)(kLabelFontSize*Scale);
        TitleStyle.fontSize= (int)(kTitleFontSize*Scale);
        ValueStyle.fontSize= (int)(kLabelFontSize*Scale);        
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
        if(ShouldShowLabel()) {
            GUI.Label(TranslateAndScale(pos), content, labelStyle);            
        }
    }
    // ----------------------------------------------------------------------
    void GUI_Label(Rect pos, String content, GUIStyle labelStyle) {
        if(ShouldShowLabel()) {
            GUI.Label(TranslateAndScale(pos), content, labelStyle);            
        }
    }
    
    // ----------------------------------------------------------------------
    public static void DrawRect(Rect rect, Color fillColor, Color outlineColor) {
        Vector3[] vectors= new Vector3[4];
        vectors[0]= new Vector3(rect.x, rect.y, 0);
        vectors[1]= new Vector3(rect.xMax, rect.y, 0);
        vectors[2]= new Vector3(rect.xMax, rect.yMax, 0);
        vectors[3]= new Vector3(rect.x, rect.yMax, 0);        
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vectors, fillColor, outlineColor);
    }
    // ----------------------------------------------------------------------
    void DrawNode(Rect r, Color nodeColor, Color backgroundColor, Color shadowColor, GUIContent content) {
        float radius= kNodeCornerRadius;
        radius*= Scale;
        
        // Show shadow.
        Vector3[] vectors= new Vector3[4];
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
        Handles.DrawLine(new Vector3(r.x,r.y+radius,0), new Vector3(r.x, r.yMax-radius+1f,0));
        Handles.DrawLine(new Vector3(r.xMax,r.y+radius,0), new Vector3(r.xMax, r.yMax-radius,0));
        Handles.DrawLine(new Vector3(r.x+radius,r.yMax,0), new Vector3(r.xMax-radius+1f, r.yMax,0));
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
        if(!ShouldShowTitle()) return;
        Vector2 titleCenter= new Vector2(0.5f*(r.x+r.xMax), r.y+0.8f*radius);
        Vector2 titleSize= TitleStyle.CalcSize(content);
        GUI.Label(new Rect(titleCenter.x-0.5f*titleSize.x, titleCenter.y-0.5f*titleSize.y, titleSize.x, titleSize.y), content, TitleStyle);
    }
    // ----------------------------------------------------------------------
    void DrawMinimizedTransitionModule(Vector2 dir, Rect r, Color nodeColor) {
        r= TranslateAndScale(r);
        Vector3 center= Math3D.Middle(r);
        Vector3 tangent= Vector3.Cross(dir, Vector3.forward);
        float size= 9f*Scale;
        Vector3 head= size*dir;
        Vector3 bottom= size*tangent;
        
        Vector3[] vectors= new Vector3[4];
        vectors[0]= center+head;
        vectors[1]= center-head+bottom;
        vectors[2]= center-0.6f*head;
        vectors[3]= center-head-bottom;
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vectors, nodeColor, new Color(0.25f, 0.25f, 0.25f));
	}

    // ======================================================================
    //  INITIALIZATION
	// ----------------------------------------------------------------------
    static public bool Init(iCS_IStorage iStorage) {
        // Load AA line texture.
        if(lineTexture == null) {
            if(!iCS_TextureCache.GetTexture(iCS_EditorStrings.AALineTexture, out lineTexture)) {
                IsInitialized= false;
                return IsInitialized;
            }
            else {
                lineTexture.hideFlags= HideFlags.DontSave;
            }            
        }
        // Load folded/unfolded icons.
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.FoldedIcon, out foldedIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.UnfoldedIcon, out unfoldedIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        // Load maximize/minimize icon.
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.MinimizeIcon, out minimizeIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.MaximizeIcon, out maximizeIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        // Load line arrow heads.
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.UpArrowHeadIcon, out upArrowHeadIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.DownArrowHeadIcon, out downArrowHeadIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.LeftArrowHeadIcon, out leftArrowHeadIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.RightArrowHeadIcon, out rightArrowHeadIcon, iStorage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        // Graphic resources properly initialized.
        IsInitialized= true;
        return IsInitialized;
    }
    // ----------------------------------------------------------------------
    void BuildLabelStyle(iCS_IStorage iStorage) {
        Color labelColor= iStorage.Preferences.NodeColors.LabelColor;
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
    void BuildTitleStyle(iCS_IStorage iStorage) {
        Color titleColor= iStorage.Preferences.NodeColors.TitleColor;
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
    void BuildValueStyle(iCS_IStorage iStorage) {
        Color valueColor= iStorage.Preferences.NodeColors.ValueColor;
        if(ValueStyle == null) ValueStyle= new GUIStyle();
        ValueStyle.normal.textColor= valueColor;
        ValueStyle.hover.textColor= valueColor;
        ValueStyle.focused.textColor= valueColor;
        ValueStyle.active.textColor= valueColor;
        ValueStyle.onNormal.textColor= valueColor;
        ValueStyle.onHover.textColor= valueColor;
        ValueStyle.onFocused.textColor= valueColor;
        ValueStyle.onActive.textColor= valueColor;
        TitleStyle.fontStyle= FontStyle.Bold;
        ValueStyle.fontSize= 11;
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
    public void DrawNormalNode(iCS_EditorObject node, iCS_IStorage iStorage) {        
        // Don't draw minimized node.
        if(IsInvisible(node, iStorage) || IsMinimized(node, iStorage)) return;
        
        // Draw node box.
        Rect position= GetDisplayPosition(node, iStorage);
        string title= GetNodeName(node, iStorage);
        // Change background color if node is selected.
        Color backgroundColor= GetBackgroundColor(node, iStorage);
        bool isMouseOver= position.Contains(MousePosition);
        GUI_Box(position, new GUIContent(title,node.ToolTip), GetNodeColor(node, iStorage), backgroundColor, isMouseOver ? WhiteShadowColor : BlackShadowColor);
        EditorGUIUtility_AddCursorRect (new Rect(position.x,  position.y, position.width, kNodeTitleHeight), MouseCursor.Link);
        // Fold/Unfold icon
        if(ShouldDisplayFoldIcon(node, iStorage)) {
            if(iStorage.IsFolded(node)) {
                GUI_DrawTexture(new Rect(position.x+6f, position.y-1f, foldedIcon.width, foldedIcon.height), foldedIcon);                           
            } else {
                GUI_DrawTexture(new Rect(position.x+6f, position.y-1f, unfoldedIcon.width, unfoldedIcon.height), unfoldedIcon);               
            }            
        }
        // Minimize Icon
        if(ShouldDisplayMinimizeIcon(node, iStorage)) {
            GUI_DrawTexture(new Rect(position.xMax-4-minimizeIcon.width, position.y-1f, minimizeIcon.width, minimizeIcon.height), minimizeIcon);
        }
    }
    // ----------------------------------------------------------------------
    Color GetBackgroundColor(iCS_EditorObject node, iCS_IStorage iStorage) {
        Color backgroundColor= BackgroundColor;
        if(node == selectedObject) {
            float adj= iStorage.Preferences.NodeColors.SelectedBrightness;
            backgroundColor= new Color(adj*BackgroundColor.r, adj*BackgroundColor.g, adj*BackgroundColor.b);
        }
        return backgroundColor;        
    }
    // ----------------------------------------------------------------------
    public void DrawMinimizedNode(iCS_EditorObject node, iCS_IStorage iStorage) {        
        if(!IsMinimized(node, iStorage)) return;
        
        // Draw minimized node.
        Rect position= GetDisplayPosition(node, iStorage);
        Texture icon= GetMaximizeIcon(node, iStorage);
        if(position.width < 12f || position.height < 12f) return;  // Don't show if too small.
        Rect texturePos= new Rect(position.x, position.y, icon.width, icon.height);                
        if(node.IsTransitionModule) {
            DrawMinimizedTransitionModule(iStorage.GetTransitionModuleVector(node), texturePos, Color.white);
        } else {
            GUI_DrawTexture(texturePos, icon);                                       
        }
        EditorGUIUtility_AddCursorRect (texturePos, MouseCursor.Link);
        GUI_Label(texturePos, new GUIContent("", node.ToolTip), LabelStyle);
		ShowTitleOver(texturePos, node, iStorage);
    }
    // ----------------------------------------------------------------------
	void ShowTitleOver(Rect pos, iCS_EditorObject node, iCS_IStorage iStorage) {
        if(!ShouldShowTitle()) return;
        string title= GetNodeName(node, iStorage);
        Vector2 labelSize= GetNodeNameSize(node, iStorage);
		pos.y-=5f;	// Put title a bit higher.
        pos= TranslateAndScale(pos);
        Rect labelRect= new Rect(0.5f*(pos.x+pos.xMax-labelSize.x), pos.y-labelSize.y, labelSize.x, labelSize.y);
        if(node == selectedObject) {
            Vector3[] vectors= new Vector3[4];
            vectors[0]= new Vector3(labelRect.x-2, labelRect.y-2, 0);
            vectors[1]= new Vector3(labelRect.xMax+2, labelRect.y-2, 0);
            vectors[2]= new Vector3(labelRect.xMax+2, labelRect.yMax+2, 0);
            vectors[3]= new Vector3(labelRect.x-2, labelRect.yMax+2, 0);
            Handles.color= Color.white;
            Handles.DrawSolidRectangleWithOutline(vectors, GetBackgroundColor(node, iStorage), GetNodeColor(node, iStorage));
        }
        GUI.Label(labelRect, new GUIContent(title, node.ToolTip), LabelStyle);		
	}
	
    // ======================================================================
    // Node style functionality
    // ----------------------------------------------------------------------
    // Returns the display color of the given node.
    static Color GetNodeColor(iCS_EditorObject node, iCS_IStorage iStorage) {
        if(node.IsEntryState) {
            return iStorage.Preferences.NodeColors.EntryStateColor;
        }
        if(node.IsState || node.IsStateChart) {
            return iStorage.Preferences.NodeColors.StateColor;
        }
        if(node.IsClassModule) {
            return iStorage.Preferences.NodeColors.ClassColor;
        }
        if(node.IsModule) {
            return iStorage.Preferences.NodeColors.ModuleColor;
        }
        if(node.IsConstructor) {
            return iStorage.Preferences.NodeColors.ConstructorColor;
        }
        if(node.IsFunction) {
            return iStorage.Preferences.NodeColors.FunctionColor;
        }
        return Color.gray;
    }
    // ----------------------------------------------------------------------
    // Returns the maximize icon for the given node.
    Texture2D GetNodeDefaultMaximizeIcon(iCS_EditorObject node, iCS_IStorage iStorage) {
        if(node.IsEntryState) {
            return BuildMaximizeIcon(node, iStorage, ref EntryStateMaximizeIcon);
        }
        if(node.IsState || node.IsStateChart) {
            return BuildMaximizeIcon(node, iStorage, ref StateMaximizeIcon);
        }
        if(node.IsModule) {
            return BuildMaximizeIcon(node, iStorage, ref ModuleMaximizeIcon);
        }
        if(node.IsConstructor) {
            return BuildMaximizeIcon(node, iStorage, ref ConstructionMaximizeIcon);
        }
        if(node.IsFunction) {
            return BuildMaximizeIcon(node, iStorage, ref FunctionMaximizeIcon);
        }
        return BuildMaximizeIcon(node, iStorage, ref DefaultMaximizeIcon);
    }
    // ----------------------------------------------------------------------
    Texture2D BuildMaximizeIcon(iCS_EditorObject node, iCS_IStorage iStorage, ref Texture2D icon) {
        if(icon == null) {
            Color nodeColor= GetNodeColor(node, iStorage);
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
    public void DrawPort(iCS_EditorObject port, iCS_IStorage iStorage) {
        // Only draw visible data ports.
        if(port == null || iStorage == null) return;
        if(IsInvisible(port, iStorage) || IsMinimized(port, iStorage)) return;
        
        // Get port type information.
        Type portValueType= GetPortValueType(port);
        if(portValueType == null) return;

		// Determine port colors
        Color portColor= iStorage.Preferences.TypeColors.GetColor(portValueType);
        Color nodeColor= GetNodeColor(iStorage.GetParent(port), iStorage);

        // Determine if port is selected.
        bool isSelectedPort= port == selectedObject || (selectedObject != null && selectedObject.IsDataPort && port == iStorage.GetParent(selectedObject));

        // Determine if port is a static port (a port that feeds information into the graph).
        bool isStaticPort= port.IsInDataPort && iStorage.GetSource(port) == null;

		// Compute port radius (radius is increased if port is selected).
		float portRadius= iCS_Config.PortRadius;
		if(isSelectedPort) {
			portRadius= 1.67f*iCS_Config.PortRadius;			
		}

        // Draw port icon
		Vector2 portCenter= GetPortCenter(port, iStorage);
        if(port.IsDataPort) {
            // Data ports.
			if(port.IsOutMuxPort) {
				DrawMuxPort(portCenter, portColor, nodeColor, portRadius);
			} else {
				if(isStaticPort) {
		            DrawSquarePort(portCenter, portColor, nodeColor, portRadius);
				} else {
	    	    	DrawCircularPort(portCenter, portColor, nodeColor, portRadius);							        
				}				
			}
        } else if(port.IsStatePort) {
            // State ports.
            if(port.IsOutStatePort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(TranslateAndScale(portCenter), FacingNormal, portRadius*Scale);
            }
        } else if(port.IsInTransitionPort || port.IsOutTransitionPort) {
            // Transition ports.
            Handles.color= Color.white;
            Handles.DrawSolidDisc(TranslateAndScale(portCenter), FacingNormal, portRadius*Scale);            
        }
        else {
            // All other types of ports (should not exists).
            DrawCircularPort(portCenter, portColor, nodeColor, portRadius);
        }
        
        // Configure move cursor for port.
        Rect portPos= new Rect(portCenter.x-portRadius*1.5f, portCenter.y-portRadius*1.5f, portRadius*3f, portRadius*3f);
        if(!port.IsTransitionPort) {
            EditorGUIUtility_AddCursorRect (portPos, MouseCursor.Link);            
        }
        if(!port.IsFloating) {
            GUI_Label(portPos, new GUIContent("", port.ToolTip), LabelStyle);            
        }
        
        // State transition name is handle by DrawConnection.
        if(port.IsStatePort) return;         

        // Display port name.
        if(!ShouldDisplayPortName(port, iStorage)) return;
        string name= GetPortName(port);
        Rect portNamePos= GetPortNameGUIPosition(port, iStorage);
        GUI.Label(portNamePos, new GUIContent(name, port.ToolTip), LabelStyle);            

        // Display port value (if applicable).
        if(ShouldDisplayPortValue(port, iStorage)) {    
            if(!port.IsFloating) {
    			EditorGUIUtility.LookLikeControls();
                Rect portValuePos= GetPortValueGUIPosition(port, iStorage);
        		if(Math3D.IsNotZero(portValuePos.width)) {
            		string valueAsStr= GetPortValueAsString(port, iStorage);
        			GUI.Label(portValuePos, valueAsStr, ValueStyle);			
        		}            				
    
                /*
                    CHANGED: ==> Experimental <==
                */
    			// Bring up port editor for selected static ports.
                object portValue= GetPortValue(port, iStorage);
    			if(isStaticPort && portValue != null && Scale > 0.75f) {
    				EditorGUIUtility.LookLikeControls();
    				if(portValueType == typeof(bool)) {
    					GUI.changed= false;
    					bool newValue= GUI.Toggle(new Rect(portNamePos.xMax, portNamePos.y-2, 16, 16), (bool)portValue, "");					
    					if(GUI.changed) {
    						iStorage.UpdatePortInitialValue(port, newValue);
    					}
    				} else if(portValueType == typeof(float)) {
    					GUI.changed= false;
    					float newValue= GUI.HorizontalSlider(new Rect(portNamePos.xMax, portNamePos.y-2, 40*Scale, 16), (float)portValue, 0, 1f);
    					if(GUI.changed) {
    						iStorage.UpdatePortInitialValue(port, newValue);
    					}
    				}
    			}
           }
       }
    }

	// ----------------------------------------------------------------------
    void DrawCircularPort(Vector3 _center, Color _fillColor, Color _borderColor, float radius) {
        Color outlineColor= Color.black;
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
        Color backgroundColor= Color.black;
        radius*= Scale;
        Vector3 center= TranslateAndScale(_center);
        Vector3[] vectors= new Vector3[4];
        float delta= radius*1.35f;

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
    void DrawMuxPort(Vector3 _center, Color _fillColor, Color _borderColor, float radius) {
        Color backgroundColor= Color.black;
        radius*= Scale;
        Vector3 center= TranslateAndScale(_center);
        Vector3[] vectors= new Vector3[4];
        float delta= radius*1.35f;

        vectors[0]= new Vector3(center.x-delta, center.y-2f*delta, 0);
        vectors[1]= new Vector3(center.x-delta, center.y+2f*delta, 0);
        vectors[2]= new Vector3(center.x+delta, center.y+delta, 0);
        vectors[3]= new Vector3(center.x+delta, center.y-delta, 0);
        Handles.color= Color.white;
		Handles.DrawSolidRectangleWithOutline(vectors, backgroundColor, _borderColor);

        delta= radius*0.67f;
        vectors[0]= new Vector3(center.x-delta, center.y-2f*delta, 0);
        vectors[1]= new Vector3(center.x-delta, center.y+2f*delta, 0);
        vectors[2]= new Vector3(center.x+delta, center.y+delta, 0);
        vectors[3]= new Vector3(center.x+delta, center.y-delta, 0);
        Handles.color= Color.white;
        Handles.DrawSolidRectangleWithOutline(vectors, _fillColor, _fillColor);
    }
    
	// ----------------------------------------------------------------------
    static float[] portTopBottomRatio      = new float[]{ 1f/2f, 1f/4f, 3f/4f, 1f/6f, 5f/6f, 1f/8f, 3f/8f, 5f/8f, 7f/8f };
    static float[] portLabelTopBottomOffset= new float[]{ 0f   , 0f   , 0.8f , 0.8f , 0.8f , 0f   , 0.8f , 0f   , 0.8f };
    static float TopBottomLabelOffset(iCS_EditorObject port, iCS_IStorage iStorage) {
        float ratio= port.LocalPosition.x/GetDisplayPosition(iStorage.GetParent(port), iStorage).width;
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
    public void DrawConnection(iCS_EditorObject port, iCS_IStorage iStorage, bool highlight= false, float lineWidth= 1.5f) {
        iCS_EditorObject portParent= iStorage.GetParent(port);
        if(IsVisible(portParent, iStorage) && iStorage.IsValid(port.Source)) {
            iCS_EditorObject source= iStorage.GetSource(port);
            iCS_EditorObject sourceParent= iStorage.GetParent(source);
            if(IsVisible(sourceParent, iStorage) && !port.IsOutStatePort) {
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
                Color color= iStorage.Preferences.TypeColors.GetColor(source.RuntimeType);
                iCS_ConnectionParams cp= new iCS_ConnectionParams(port, GetDisplayPosition(port, iStorage), source, GetDisplayPosition(source, iStorage), iStorage);
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
            		Handles.DrawBezier(startPos, endPos, startTangent, endTangent, color, lineTexture, lineWidth);
                } else {
                    color.a= 0.6f*color.a;
            		Handles.DrawBezier(startPos, endPos, startTangent, endTangent, color, lineTexture, lineWidth);                    
                }
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
    static Rect GetDisplayPosition(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        Rect layoutPosition= GetLayoutPosition(edObj, iStorage);
        Rect displayPosition= iStorage.GetDisplayPosition(edObj);
        if(IsAnimationCompleted(edObj, iStorage)) {
            if((GetAnimationRatio(edObj, iStorage)-1f)*iStorage.Preferences.ControlOptions.AnimationTime > 0.2f) {  // 200ms
                if(displayPosition.x != layoutPosition.x || displayPosition.y != layoutPosition.y ||
                   displayPosition.width!= layoutPosition.width || displayPosition.height != layoutPosition.height) {
                       if(!edObj.IsFloating) {
                           iStorage.StartAnimTimer(edObj);
                           return displayPosition;                           
                       }
                   }
            }
            iStorage.SetDisplayPosition(edObj, layoutPosition);
            return layoutPosition;
        }
        iStorage.IsAnimationPlaying= true;
        float ratio= GetAnimationRatio(edObj, iStorage);
        displayPosition= Math3D.Lerp(displayPosition, layoutPosition, ratio);
        return displayPosition;
    }
   	// ----------------------------------------------------------------------
    static Rect GetLayoutPosition(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        if(!iStorage.IsVisible(edObj)) {
            iCS_EditorObject parent= iStorage.GetParent(edObj);
            for(; !iStorage.IsVisible(parent); parent= iStorage.GetParent(parent));
            Vector2 midPoint= Math3D.Middle(iStorage.GetPosition(parent));
            return new Rect(midPoint.x, midPoint.y, 0, 0);
        }
        return iStorage.GetPosition(edObj);
    }
	// ----------------------------------------------------------------------
	// Returns the time ratio of the animation between 0 and 1.
    static float GetAnimationRatio(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        float time= iStorage.Preferences.ControlOptions.AnimationTime;
        float invTime= Math3D.IsZero(time) ? 10000f : 1f/time;
        return invTime*(iStorage.GetAnimTime(edObj));        
    }
	// ----------------------------------------------------------------------
    // Returns true if the animation ratio >= 1.
    static bool IsAnimationCompleted(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        return GetAnimationRatio(edObj, iStorage) >= 0.99f;
    }

   	// ----------------------------------------------------------------------
 	bool IsMinimized(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        if(edObj.IsNode) {
            if(IsInvisible(edObj, iStorage)) return false;
            Rect position= GetDisplayPosition(edObj, iStorage);
            Texture icon= GetMaximizeIcon(edObj, iStorage);
            return (position.width*position.height <= icon.width*icon.height+1f);
        }
        return iStorage.IsMinimized(edObj) && IsAnimationCompleted(edObj, iStorage);
    }
   	// ----------------------------------------------------------------------
    static bool IsFolded(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        return iStorage.IsFolded(edObj) && IsAnimationCompleted(edObj, iStorage);
    }
   	// ----------------------------------------------------------------------
    bool IsInvisible(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        return !IsVisible(edObj, iStorage);
    }
   	// ----------------------------------------------------------------------
    bool IsVisible(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        if(edObj.IsNode) {
            Rect position= GetDisplayPosition(edObj, iStorage);
            return position.width >= 12f && position.height >= 12;  // Invisible if too small.
        }
        iCS_EditorObject parent= iStorage.GetParent(edObj);
		if(parent.IsDataPort) return false;
        return IsVisible(parent, iStorage) && !IsMinimized(parent, iStorage);
    }
    
}
