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
    static Texture2D    foldedIcon        = null;
    static Texture2D    unfoldedIcon      = null;
    static Texture2D    minimizeIcon      = null;
    static Texture2D    maximizeIcon      = null;
    static Texture2D    upArrowHeadIcon   = null;
    static Texture2D    downArrowHeadIcon = null;
    static Texture2D    leftArrowHeadIcon = null;
    static Texture2D    rightArrowHeadIcon= null;
	
    // ----------------------------------------------------------------------
    GUIStyle    LabelStyle              = null;
    GUIStyle    TitleStyle              = null;
    GUIStyle    ValueStyle              = null;
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
        BuildTitleStyle(storage);
        BuildValueStyle(storage);
        
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
	// ----------------------------------------------------------------------
    bool ShouldShowLabel() {
        return Scale >= 0.5f;        
    }
	// ----------------------------------------------------------------------
    bool ShouldShowTitle() {
        return Scale >= 0.4f;    
    }

    // ======================================================================
    //  INITIALIZATION
	// ----------------------------------------------------------------------
    static public bool Init(iCS_IStorage storage) {
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
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.FoldedIcon, out foldedIcon, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.UnfoldedIcon, out unfoldedIcon, storage)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        // Load maximize/minimize icon.
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.MinimizeIcon, out minimizeIcon, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.MaximizeIcon, out maximizeIcon, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }
        // Load line arrow heads.
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.UpArrowHeadIcon, out upArrowHeadIcon, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.DownArrowHeadIcon, out downArrowHeadIcon, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.LeftArrowHeadIcon, out leftArrowHeadIcon, storage)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.RightArrowHeadIcon, out rightArrowHeadIcon, storage)) {
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
    void BuildTitleStyle(iCS_IStorage storage) {
        Color titleColor= storage.Preferences.NodeColors.TitleColor;
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
    void BuildValueStyle(iCS_IStorage storage) {
        Color valueColor= storage.Preferences.NodeColors.ValueColor;
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
    public void DrawNormalNode(iCS_EditorObject node, iCS_IStorage storage) {        
        // Don't draw minimized node.
        if(IsInvisible(node, storage) || IsMinimized(node, storage)) return;
        
        // Draw node box.
        Rect position= GetDisplayPosition(node, storage);
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.Name));
        // Change background color if node is selected.
        Color backgroundColor= GetBackgroundColor(node, storage);
        bool isMouseOver= position.Contains(MousePosition);
        GUI_Box(position, new GUIContent(title,node.ToolTip), GetNodeColor(node, storage), backgroundColor, isMouseOver ? WhiteShadowColor : BlackShadowColor);
        EditorGUIUtility_AddCursorRect (new Rect(position.x,  position.y, position.width, kNodeTitleHeight), MouseCursor.Link);
        // Fold/Unfold icon
        if(ShouldDisplayFoldIcon(node, storage)) {
            if(storage.IsFolded(node)) {
                GUI_DrawTexture(new Rect(position.x+6f, position.y-1f, foldedIcon.width, foldedIcon.height), foldedIcon);                           
            } else {
                GUI_DrawTexture(new Rect(position.x+6f, position.y-1f, unfoldedIcon.width, unfoldedIcon.height), unfoldedIcon);               
            }            
        }
        // Minimize Icon
        if(ShouldDisplayMinimizeIcon(node, storage)) {
            GUI_DrawTexture(new Rect(position.xMax-4-minimizeIcon.width, position.y-1f, minimizeIcon.width, minimizeIcon.height), minimizeIcon);
        }
    }
    // ----------------------------------------------------------------------
    Color GetBackgroundColor(iCS_EditorObject node, iCS_IStorage storage) {
        Color backgroundColor= BackgroundColor;
        if(node == selectedObject) {
            float adj= storage.Preferences.NodeColors.SelectedBrightness;
            backgroundColor= new Color(adj*BackgroundColor.r, adj*BackgroundColor.g, adj*BackgroundColor.b);
        }
        return backgroundColor;        
    }
    // ----------------------------------------------------------------------
    public void DrawMinimizedNode(iCS_EditorObject node, iCS_IStorage storage) {        
        if(!IsMinimized(node, storage)) return;
        
        // Draw minimized node.
        Rect position= GetDisplayPosition(node, storage);
        Texture icon= GetMaximizeIcon(node, storage);
        if(position.width < 12f || position.height < 12f) return;  // Don't show if too small.
        Rect texturePos= new Rect(position.x, position.y, icon.width, icon.height);                
        if(node.IsTransitionModule) {
            DrawMinimizedTransitionModule(storage.GetTransitionModuleVector(node), texturePos, Color.white);
        } else {
            GUI_DrawTexture(texturePos, icon);                                       
        }
        EditorGUIUtility_AddCursorRect (texturePos, MouseCursor.Link);
        GUI_Label(texturePos, new GUIContent("", node.ToolTip), LabelStyle);
		ShowTitleOver(texturePos, node, storage);
    }
    // ----------------------------------------------------------------------
	void ShowTitleOver(Rect pos, iCS_EditorObject node, iCS_IStorage storage) {
        if(!ShouldShowTitle()) return;
        string title= ObjectNames.NicifyVariableName(storage.Preferences.HiddenPrefixes.GetName(node.Name));
        Vector2 labelSize= LabelStyle.CalcSize(new GUIContent(title));
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
            Handles.DrawSolidRectangleWithOutline(vectors, GetBackgroundColor(node, storage), GetNodeColor(node, storage));
        }
        GUI.Label(labelRect, new GUIContent(title, node.ToolTip), LabelStyle);		
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
        if(node.IsClassModule) {
            return storage.Preferences.NodeColors.ClassColor;
        }
        if(node.IsModule) {
            return storage.Preferences.NodeColors.ModuleColor;
        }
        if(node.IsConstructor) {
            return storage.Preferences.NodeColors.ConstructorColor;
        }
        if(node.IsFunction) {
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
        if(node.IsFunction) {
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
        if(port == null || storage == null) return;
        // Update display position.
        Rect position= GetDisplayPosition(port, storage);

        // Only draw visible data ports.
        if(IsInvisible(port, storage) || IsMinimized(port, storage)) return;
        
        // Get port primary information.
        iCS_EditorObject portParent= storage.GetParent(port);         
        Vector2 center= Math3D.ToVector2(position);
        Type portValueType= port.RuntimeType;
        if(portValueType == null) return;
		// Determine port colors
        Color portColor= storage.Preferences.TypeColors.GetColor(portValueType);
        Color nodeColor= GetNodeColor(portParent, storage);
        // Determine if port is selected.
        bool isSelectedPort= port == selectedObject || (selectedObject != null && selectedObject.IsDataPort && port == storage.GetParent(selectedObject));
        // Determine if port is a static port (a port that feeds information into the graph).
        bool isStaticPort= port.IsInDataPort && storage.GetSource(port) == null;
		// Compute port radius (radius is increased if port is selected).
		float portRadius= iCS_Config.PortRadius;
		if(isSelectedPort) {
			portRadius= 1.67f*iCS_Config.PortRadius;			
		}
		object portValue= null;
        if(port.IsDataPort) {
    		if(Application.isPlaying && storage.Preferences.DisplayOptions.PlayingPortValues) portValue= storage.GetPortValue(port);
			Vector2 portCenter= center;
			if(port.IsOutMuxPort) {
				DrawMuxPort(portCenter, portColor, nodeColor, portRadius);
			} else {
				if(isStaticPort) {
					if(!Application.isPlaying && storage.Preferences.DisplayOptions.EditorPortValues) portValue= storage.GetPortValue(port);
					if(portValue != null) {
		            	DrawSquarePort(portCenter, portColor, nodeColor, portRadius);
					} else {
			            DrawCircularPort(portCenter, portColor, nodeColor, portRadius);									
					}
				} else {
	    	    	DrawCircularPort(portCenter, portColor, nodeColor, portRadius);							        
				}				
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
            // TO BE VERIFIED.
            GUI_Label(portPos, new GUIContent("", port.ToolTip), LabelStyle);            
        }
        
        // Show port label.
        if(port.IsStatePort) return;        // State transition name is handle by DrawConnection. 
        if(!ShouldShowLabel()) return;      // Don't show label & values if scale does not permit.
        string name= portValueType.IsArray ? "["+port.Name+"]" : port.Name;
		string valueAsStr= portValue != null ? GetValueAsString(portValue) : null;
        Vector2 labelSize= LabelStyle.CalcSize(new GUIContent(name));
		Vector2 valueSize= (valueAsStr != null && valueAsStr != "") ? ValueStyle.CalcSize(new GUIContent(valueAsStr)) : Vector2.zero;
		Vector2 valuePos= center;
		Vector2 labelPos= center;
        switch(port.Edge) {
            case iCS_EditorObject.EdgeEnum.Left:
                labelPos.x+= 1 + iCS_Config.PortSize;
                labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
				valuePos.x-= 1 + valueSize.x/Scale + iCS_Config.PortSize;
				valuePos.y-= 1 + 0.5f * valueSize.y/Scale;
                break;
            case iCS_EditorObject.EdgeEnum.Right:
                labelPos.x-= 1 + labelSize.x/Scale + iCS_Config.PortSize;
                labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
				valuePos.x+= 1 + iCS_Config.PortSize;
				valuePos.y-= 1 + 0.5f * valueSize.y/Scale;
                break;
            case iCS_EditorObject.EdgeEnum.Top:            
                labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                labelPos.y-= iCS_Config.PortSize+0.8f*(labelSize.y/Scale)*(1+TopBottomLabelOffset(port, storage));
				valueAsStr= null;
                break;
            case iCS_EditorObject.EdgeEnum.Bottom:
                labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                labelPos.y+= iCS_Config.PortSize+0.8f*(labelSize.y/Scale)*TopBottomLabelOffset(port, storage)-0.2f*labelSize.y/Scale;
				valueAsStr= null;
                break;
        }
        labelPos= TranslateAndScale(labelPos);
        valuePos= TranslateAndScale(valuePos);
        GUI.Label(new Rect(labelPos.x, labelPos.y, labelSize.x, labelSize.y), new GUIContent(name, port.ToolTip), LabelStyle);
        if(!port.IsFloating) {
    		if(valueAsStr != null) {
    			GUI.Label(new Rect(valuePos.x, valuePos.y, valueSize.x, valueSize.y), valueAsStr, ValueStyle);			
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
    static Rect GetDisplayPosition(iCS_EditorObject edObj, iCS_IStorage storage) {
        Rect layoutPosition= GetLayoutPosition(edObj, storage);
        Rect displayPosition= storage.GetDisplayPosition(edObj);
        if(IsAnimationCompleted(edObj, storage)) {
            if((GetAnimationRatio(edObj, storage)-1f)*storage.Preferences.ControlOptions.AnimationTime > 0.2f) {  // 200ms
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
        storage.IsAnimationPlaying= true;
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
        float time= storage.Preferences.ControlOptions.AnimationTime;
        float invTime= Math3D.IsZero(time) ? 10000f : 1f/time;
        return invTime*(storage.GetAnimTime(edObj));        
    }
	// ----------------------------------------------------------------------
    // Returns true if the animation ratio >= 1.
    static bool IsAnimationCompleted(iCS_EditorObject edObj, iCS_IStorage storage) {
        return GetAnimationRatio(edObj, storage) >= 0.99f;
    }

   	// ----------------------------------------------------------------------
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
		if(parent.IsDataPort) return false;
        return IsVisible(parent, storage) && !IsMinimized(parent, storage);
    }
    
}
