/*
    TODO: re-examine tooltip implementation since the current one triples the frame rate.
*/
//#define SHOW_TOOLTIP

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= Prelude;

public partial class iCS_Graphics {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const float kInitialScale    = iCS_EditorConfig.kInitialScale;
    const float kIconicSize      = iCS_EditorConfig.kIconicSize;
    const float kIconicArea      = iCS_EditorConfig.kIconicArea;
    const float kNodeCornerRadius= iCS_EditorConfig.kNodeCornerRadius;
    const float kNodeTitleHeight = iCS_EditorConfig.kNodeTitleHeight;
    const int   kLabelFontSize   = iCS_EditorConfig.kLabelFontSize;
    const int   kTitleFontSize   = iCS_EditorConfig.kTitleFontSize;
    
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    static public bool  IsInitialized= false;    
    static Texture2D    lineTexture       = null;
    static Texture2D    maximizeIcon      = null;
    static Texture2D    upArrowHeadIcon   = null;
    static Texture2D    downArrowHeadIcon = null;
    static Texture2D    leftArrowHeadIcon = null;
    static Texture2D    rightArrowHeadIcon= null;
	
    // ----------------------------------------------------------------------
    public GUIStyle    LabelStyle              = null;
    public GUIStyle    TitleStyle              = null;
    public GUIStyle    MessageTitleStyle       = null;
    public GUIStyle    ValueStyle              = null;
    public Texture2D   StateMaximizeIcon       = null;
    public Texture2D   ModuleMaximizeIcon      = null;
    public Texture2D   EntryStateMaximizeIcon  = null;
    public Texture2D   ConstructionMaximizeIcon= null;
    public Texture2D   FunctionMaximizeIcon    = null;
    public Texture2D   DefaultMaximizeIcon     = null;

    // ----------------------------------------------------------------------
    iCS_EditorObject selectedObject= null;
    float            Scale= kInitialScale;
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
	       static readonly Color   BlackShadowColor= new Color(0,0,0,0.25f);
	       static readonly Color   WhiteShadowColor= new Color(1f,1f,1f,0.125f);
           
    // ======================================================================
    // Drawing staging
	// ----------------------------------------------------------------------
    public void Begin(Vector2 translation, float scale, Rect clipingRect, iCS_EditorObject selObj, Vector2 mousePos) {
        Translation= translation;
		if(Math3D.IsNotEqual(Scale, scale)) {
			iCS_PortIcons.BuildPortIconTemplates(scale);
			iCS_NodeTextures.BuildNodeTemplate(scale);
		}
        Scale= scale;
        ClipingArea= clipingRect;
        MousePosition= mousePos;
        selectedObject= selObj;

        // Rebuild label style to match user preferences.
        BuildLabelStyle();
        BuildTitleStyle();
        BuildMessageTitleStyle();
        BuildValueStyle();
        
        // Set font size according to scale.
        LabelStyle.fontSize= (int)(kLabelFontSize*Scale);
        TitleStyle.fontSize= (int)(kTitleFontSize*Scale);
        ValueStyle.fontSize= (int)(kLabelFontSize*Scale);        
    }
    public void End(iCS_IStorage iStorage) {
    }
    
    // ======================================================================
    // GUI Warppers
	// ----------------------------------------------------------------------
    bool IsVisible(Vector2 point, float radius= 0f) {
        return IsVisible(new Rect(point.x-radius, point.y-radius, 2f*radius, 2f*radius));
    }
	// ----------------------------------------------------------------------
    bool IsVisible(Rect r) {
        Rect intersection= Clip(r);
        return Math3D.IsNotZero(intersection.width) && Math3D.IsNotZero(intersection.height);        
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
    void GUI_Box(Rect pos, GUIContent title, Color nodeColor, Color backgroundColor, Color shadowColor, GUIStyle titleStyle= null) {
        if(titleStyle == null) titleStyle= TitleStyle;
        Rect adjPos= TranslateAndScale(pos);
        DrawNode(adjPos, nodeColor, backgroundColor, shadowColor, title, titleStyle);
#if SHOW_TOOLTIP
        string tooltip= title.tooltip;
        if(tooltip != null && tooltip != "") {
            GUI.Label(adjPos, new GUIContent("", tooltip), LabelStyle);
        }
#endif
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
    public static void DrawBox(Rect rect, Color fillColor, Color outlineColor, Color multiplyColor) {
        Vector3[] vectors= new Vector3[4];
        vectors[0]= new Vector3(rect.x, rect.y, 0);
        vectors[1]= new Vector3(rect.xMax, rect.y, 0);
        vectors[2]= new Vector3(rect.xMax, rect.yMax, 0);
        vectors[3]= new Vector3(rect.x, rect.yMax, 0);        
        Handles.color= multiplyColor;
        Handles.DrawSolidRectangleWithOutline(vectors, fillColor, outlineColor);
    }
    // ----------------------------------------------------------------------
    public static void DrawRect(Rect rect, Color fillColor, Color outlineColor) {
        DrawBox(rect, fillColor, outlineColor, Color.white);
    }

    // ----------------------------------------------------------------------
    void DrawMinimizedTransitionModule(Vector2 dir, Vector2 p, Color nodeColor) {
        Vector3 center= TranslateAndScale(p);
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
        // Build texture temaples.
        iCS_PortIcons.BuildPortIconTemplates(kInitialScale);
		iCS_NodeTextures.BuildNodeTemplate(kInitialScale);
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
        // Load maximize/minimize icon.
        maximizeIcon= iCS_BuiltinTextures.MaximizeIcon(1f);
        // Load line arrow heads.
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.UpArrowHeadIcon, out upArrowHeadIcon)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.DownArrowHeadIcon, out downArrowHeadIcon)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.LeftArrowHeadIcon, out leftArrowHeadIcon)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.RightArrowHeadIcon, out rightArrowHeadIcon)) {
            IsInitialized= false;
            return IsInitialized;
        }        
        // Graphic resources properly initialized.
        IsInitialized= true;
        return IsInitialized;
    }
    // ----------------------------------------------------------------------
    void BuildLabelStyle() {
        Color labelColor= iCS_PreferencesEditor.NodeLabelColor;
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
        Color titleColor= iCS_PreferencesEditor.NodeTitleColor;
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
    void BuildMessageTitleStyle() {
        Color messageTitleColor= Color.red;
        if(MessageTitleStyle == null) {
            MessageTitleStyle= new GUIStyle(TitleStyle);
        }
        MessageTitleStyle.normal.textColor= messageTitleColor;
    }
    // ----------------------------------------------------------------------
    void BuildValueStyle() {
        Color valueColor= iCS_PreferencesEditor.NodeValueColor;
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
        if(!node.IsVisibleOnDisplay || node.IsIconizedOnDisplay) return;
        
        // Don't display if we are outside the clipping area.
        Rect position= node.AnimatedRect;
        if(!IsVisible(position)) return;

        // Draw node since all draw conditions are valid.
        GUI.color= new Color(1f, 1f, 1f, node.DisplayAlpha);
        string title= GetNodeName(node);
        // Change background color if node is selected.
        Color backgroundColor= GetBackgroundColor(node);
        bool isMouseOver= position.Contains(MousePosition);
#if SHOW_TOOLTIP
        string tooltip= isMouseOver ? GetNodeTooltip(node,iStorage) : null;
#else
        string tooltip= null;
#endif
        // Determine title style
        var shadowColor= isMouseOver ? WhiteShadowColor : BlackShadowColor;
        GUI_Box(position, new GUIContent(title, tooltip), GetNodeColor(node), backgroundColor, shadowColor);
        if(isMouseOver) {
            EditorGUIUtility_AddCursorRect (new Rect(position.x,  position.y, position.width, kNodeTitleHeight), MouseCursor.Link);            
        }
        // Fold/Unfold icon
        if(ShouldDisplayFoldIcon(node)) {
            var icon= node.IsFoldedOnDisplay ? iCS_BuiltinTextures.FoldIcon(Scale) : iCS_BuiltinTextures.UnfoldIcon(Scale);
            GUI_DrawTexture(new Rect(position.x+6f, position.y-0.5f, 16, 16), icon);                           
        }
        // Minimize Icon
        if(ShouldDisplayMinimizeIcon(node)) {
            var minimizeIcon= iCS_BuiltinTextures.MinimizeIcon(Scale);
            GUI_DrawTexture(new Rect(position.xMax-2-/*minimizeIcon.width*/16, position.y-0.5f, /*minimizeIcon.width*/16, /*minimizeIcon.height*/16), minimizeIcon);            
        }
        GUI.color= Color.white;
    }
    // ----------------------------------------------------------------------
    Color GetBackgroundColor(iCS_EditorObject node) {
        Color backgroundColor= BackgroundColor;
        if(node == selectedObject) {
            float adj= iCS_PreferencesEditor.SelectedBrightnessGain;
            backgroundColor= new Color(adj*BackgroundColor.r, adj*BackgroundColor.g, adj*BackgroundColor.b);
        }
        return backgroundColor;        
    }
    // ----------------------------------------------------------------------
    public void DrawMinimizedNode(iCS_EditorObject node, iCS_IStorage iStorage) {        
        if(!node.IsIconizedOnDisplay) return;
        
        // Draw minimized node (if visible).
        Rect displayRect= node.AnimatedRect;
        Rect displayArea= new Rect(displayRect.x-100f, displayRect.y-16f, displayRect.width+200f, displayRect.height+16f);
        if(!IsVisible(displayArea)) return;

		Color alphaWhite= new Color(1f, 1f, 1f, node.DisplayAlpha);
        GUI.color= alphaWhite;
        Texture icon= GetMaximizeIcon(node);
		var position= Math3D.Middle(displayRect);
        Rect textureRect= new Rect(position.x-0.5f*icon.width, position.y-0.5f*icon.height, icon.width, icon.height);                
        if(node.IsTransitionModule) {
            DrawMinimizedTransitionModule(iStorage.GetTransitionModuleVector(node), position, alphaWhite);
        } else {
            GUI_DrawTexture(textureRect, icon);                                       
        }
        if(textureRect.Contains(MousePosition)) {
            EditorGUIUtility_AddCursorRect (textureRect, MouseCursor.Link);
#if SHOW_TOOLTIP
            GUI_Label(textureRect, new GUIContent("", GetNodeTooltip(node,iStorage)), LabelStyle);            
#endif
        }
		ShowTitleOver(textureRect, node);
        GUI.color= Color.white;
    }
    // ----------------------------------------------------------------------
	void ShowTitleOver(Rect pos, iCS_EditorObject node) {
        if(!ShouldShowTitle()) return;
        string title= GetNodeName(node);
        Vector2 labelSize= GetNodeNameSize(node);
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
            Handles.DrawSolidRectangleWithOutline(vectors, GetBackgroundColor(node), GetNodeColor(node));
        }
        GUI.Label(labelRect, new GUIContent(title, node.Tooltip), LabelStyle);		
	}
	
    // ======================================================================
    // Node style functionality
    // ----------------------------------------------------------------------
    // Returns the display color of the given node.
    static Color GetNodeColor(iCS_EditorObject node) {
        if(node.IsBehaviour) {
            return new Color(0.75f, 0.75f, 0.75f);
        }
        if(node.IsMessage) {
            return iCS_PreferencesEditor.MessageNodeColor;
        }
        if(node.IsEntryState) {
            return iCS_PreferencesEditor.EntryStateNodeColor;
        }
        if(node.IsState || node.IsStateChart) {
            return iCS_PreferencesEditor.StateNodeColor;
        }
        if(node.IsObjectInstance) {
            return iCS_PreferencesEditor.InstanceNodeColor;
        }
        if(node.IsConstructor) {
            return iCS_PreferencesEditor.ConstructorNodeColor;
        }
        if(node.IsFunction) {
            return iCS_PreferencesEditor.FunctionNodeColor;
        }
        if(node.IsKindOfAggregate) {
            return iCS_PreferencesEditor.PackageNodeColor;
        }
        return Color.gray;
    }
    // ----------------------------------------------------------------------
    // Returns the maximize icon for the given node.
    Texture2D GetNodeDefaultMaximizeIcon(iCS_EditorObject node) {
        if(node.IsEntryState) {
            return BuildMaximizeIcon(node, ref EntryStateMaximizeIcon);
        }
        if(node.IsState || node.IsStateChart) {
            return BuildMaximizeIcon(node, ref StateMaximizeIcon);
        }
        if(node.IsKindOfAggregate) {
            return BuildMaximizeIcon(node, ref ModuleMaximizeIcon);
        }
        if(node.IsConstructor) {
            return BuildMaximizeIcon(node, ref ConstructionMaximizeIcon);
        }
        if(node.IsFunction) {
            return BuildMaximizeIcon(node, ref FunctionMaximizeIcon);
        }
        return BuildMaximizeIcon(node, ref DefaultMaximizeIcon);
    }
    // ----------------------------------------------------------------------
    Texture2D BuildMaximizeIcon(iCS_EditorObject node, ref Texture2D icon) {
        if(icon == null) {
            Color nodeColor= GetNodeColor(node);
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
        // Don't show port if too small
        if(!ShouldShowPort()) return;
        
        // Only draw visible data ports.
        if(port == null || iStorage == null) return;
        if(!port.IsVisibleOnDisplay) return;
        
        // Don't show port if its parent node is iconized.
        var parent= port.ParentNode;
        if(parent.IsIconizedOnDisplay) return;
        
        // Don't display if outside clipping area.
		Vector2 portCenter= GetPortCenter(port);
		if(port.IsOnRightEdge) portCenter.x-= 1f/Scale;   // Small adjustement realign right ports on visual edge.
		float portRadius= iCS_EditorConfig.PortRadius;
        Rect displayArea= new Rect(portCenter.x-200f, portCenter.y-2f*portRadius, 400f, 4f*portRadius);
        if(!IsVisible(displayArea)) return;
        
        // Determine if port is selected.
        bool isSelectedPort= port == selectedObject || (selectedObject != null && selectedObject.IsDataPort && port == selectedObject.Parent);

		// Compute port radius (radius is increased if port is selected).
		if(isSelectedPort) {
			portRadius= iCS_EditorConfig.PortRadius*iCS_EditorConfig.SelectedPortFactor;			
		}

        // Get port type information.
        Type portValueType= GetPortValueType(port);
        if(portValueType == null) return;

        // Port alpha
        var alpha= port.DisplayAlpha;
        if(parent.IsIconizedInLayout && parent.IsAnimated) {
            alpha= 1f-parent.AnimationTimeRatio;
        } 
        GUI.color= new Color(1f,1f,1f,alpha);
        
		// Determine port colors
        Color portColor= iCS_PreferencesEditor.GetTypeColor(portValueType);
        Color nodeColor= GetNodeColor(port.Parent);

        // Draw port icon
		DrawPortIcon(port, portCenter, isSelectedPort, portColor, nodeColor, portRadius, iStorage);

        // Configure move cursor for port.
        Rect portPos= new Rect(portCenter.x-portRadius, portCenter.y-portRadius, 2f*portRadius, 2f*portRadius);
        if(portPos.Contains(MousePosition)) {
            if(!port.IsTransitionPort) {
                EditorGUIUtility_AddCursorRect (portPos, MouseCursor.Link);            
            }
            if(!port.IsFloating) {
#if SHOW_TOOLTIP
        		string tooltip= GetPortTooltip(port, iStorage);
                GUI_Label(portPos, new GUIContent("", tooltip), LabelStyle);            
#endif
            }            
        }            
        
        // State transition name is handle by DrawConnection.
        if(port.IsStatePort || port.IsTransitionPort) return;         

        // Determine if port is a static port (a port that feeds information into the graph).
        bool isStaticPort= port.IsInDataPort && port.Source == null;

        // Display port name.
        if(!ShouldDisplayPortName(port)) return;
        Rect portNamePos= GetPortNameGUIPosition(port, iStorage);
        string name= GetPortName(port);
        GUI.Label(portNamePos, name, LabelStyle);                                    

        // Display port value (if applicable).
        if(ShouldDisplayPortValue(port)) {    
            if(!port.IsFloating) {
    			EditorGUIUtility.LookLikeControls();
                Rect portValuePos= GetPortValueGUIPosition(port);
        		if(Math3D.IsNotZero(portValuePos.width)) {
            		string valueAsStr= GetPortValueAsString(port);
        			GUI.Label(portValuePos, valueAsStr, ValueStyle);			
        		}            				
    
                /*
                    CHANGED: ==> Experimental <==
                */
    			// Bring up port editor for selected static ports.
                object portValue= port.PortValue;
    			if(isStaticPort && portValue != null && Scale > 0.75f) {
    				EditorGUIUtility.LookLikeControls();
    				if(portValueType == typeof(bool)) {
    					Vector2 togglePos= TranslateAndScale(portCenter);
                        var savedBackgroundColor= GUI.backgroundColor;
    					GUI.backgroundColor= portColor;
    					GUI.changed= false;
                        bool currentValue= (bool)portValue;
    					bool newValue= GUI.Toggle(new Rect(togglePos.x-7, togglePos.y-9, 16, 16), currentValue, "");					
                        GUI.backgroundColor= savedBackgroundColor;
    					if(GUI.changed) {
    						port.PortValue= newValue;
    					}
    				}
    			}
           }
       }
       // Reset GUI alpha.
       GUI.color= Color.white;
    }

	// ----------------------------------------------------------------------
    public void DrawPortIcon(iCS_EditorObject port, Vector2 portCenter, bool isSelected, Color portColor, Color nodeColor, float portRadius, iCS_IStorage iStorage) {
        // Draw port icon.
        if(port.IsDataPort) {
            // Don't display mux input ports.
            if(port.IsChildMuxPort) return;
            // Data ports.
			if(port.IsParentMuxPort) {
				DrawMuxPort(portCenter, portColor, nodeColor, portRadius);
			} else {
	    	    DrawDataPort(port, portCenter, portColor, isSelected);							        
			}
        } else if(port.IsStatePort) {
            // State ports.
            if(port.IsOutStatePort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(TranslateAndScale(portCenter), FacingNormal, 0.65f*portRadius*Scale);
            }
        } else if(port.IsTransitionPort) {
            // Transition ports.
            if(port.IsOutTransitionPort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(TranslateAndScale(portCenter), FacingNormal, 0.65f*portRadius*Scale);                            
            }
        }
        else {
            // All other types of ports (should not exists).
            DrawDataPort(port, portCenter, portColor, isSelected);
        }        
    }
	// ----------------------------------------------------------------------
    void DrawDataPort(iCS_EditorObject port, Vector3 _center, Color _fillColor, bool isSelected) {
		if(port.IsInputPort) {
			if(port.IsEndPort) {
				DrawInEndPort(_center, _fillColor, isSelected);
			} else {
				DrawInRelayPort(_center, _fillColor, isSelected);
			}
		} else {
			if(port.IsEndPort) {
				DrawOutEndPort(_center, _fillColor, isSelected);
			} else {
				DrawOutRelayPort(_center, _fillColor, isSelected);
			}
		}
    }
	// ----------------------------------------------------------------------
    void DrawInEndPort(Vector3 _center, Color _fillColor, bool isSelected) {
		Vector3 center= TranslateAndScale(_center);
		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInEndPortIcon(_fillColor) :
		                                 iCS_PortIcons.GetInEndPortIcon(_fillColor);
		Rect pos= new Rect(center.x-0.5f*portIcon.width,
						   center.y-0.5f*portIcon.height,
						   portIcon.width,
						   portIcon.height);
		GUI.DrawTexture(pos, portIcon);
    }
	// ----------------------------------------------------------------------
    void DrawOutEndPort(Vector3 _center, Color _fillColor, bool isSelected) {
		Vector3 center= TranslateAndScale(_center);
		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutEndPortIcon(_fillColor) :
		                                 iCS_PortIcons.GetOutEndPortIcon(_fillColor);
		Rect pos= new Rect(center.x-0.5f*portIcon.width,
						   center.y-0.5f*portIcon.height,
						   portIcon.width,
						   portIcon.height);
		GUI.DrawTexture(pos, portIcon);
    }
	// ----------------------------------------------------------------------
    void DrawInRelayPort(Vector3 _center, Color _fillColor, bool isSelected) {
		Vector3 center= TranslateAndScale(_center);
		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInRelayPortIcon(_fillColor) :
		                                 iCS_PortIcons.GetInRelayPortIcon(_fillColor);
		Rect pos= new Rect(center.x-0.5f*portIcon.width,
						   center.y-0.5f*portIcon.height,
						   portIcon.width,
						   portIcon.height);
		GUI.DrawTexture(pos, portIcon);
    }
	// ----------------------------------------------------------------------
    void DrawOutRelayPort(Vector3 _center, Color _fillColor, bool isSelected) {
		Vector3 center= TranslateAndScale(_center);
		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutRelayPortIcon(_fillColor) :
		                                 iCS_PortIcons.GetOutRelayPortIcon(_fillColor);
		Rect pos= new Rect(center.x-0.5f*portIcon.width,
						   center.y-0.5f*portIcon.height,
						   portIcon.width,
						   portIcon.height);
		GUI.DrawTexture(pos, portIcon);
    }

	// ----------------------------------------------------------------------
    void DrawMuxPort(Vector3 _center, Color _fillColor, Color _borderColor, float radius) {
        Color backgroundColor= Color.black;
        radius*= Scale;
        Vector3 center= TranslateAndScale(_center);
        Vector3[] vectors= new Vector3[4];
        float delta= radius*0.8f;

        vectors[0]= new Vector3(center.x-delta, center.y-2f*delta, 0);
        vectors[1]= new Vector3(center.x-delta, center.y+2f*delta, 0);
        vectors[2]= new Vector3(center.x+delta, center.y+delta, 0);
        vectors[3]= new Vector3(center.x+delta, center.y-delta, 0);
        Handles.color= Color.white;
		Handles.DrawSolidRectangleWithOutline(vectors, backgroundColor, _borderColor);

        delta*= 0.60f;
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
        float ratio= 0.5f+port.PortPositionRatio;
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
    public void DrawBinding(iCS_EditorObject port, iCS_IStorage iStorage, bool highlight= false, float lineWidth= 1.5f) {
        // No connection to draw if no valid source.
        if(!port.IsSourceValid) return;
        iCS_EditorObject portParent= port.Parent;

        // No connection to draw if the port is not visible.
        if(!port.IsVisibleOnDisplay) return;

        // No connection to draw if source port is not visible.
        iCS_EditorObject source= port.Source;
        iCS_EditorObject sourceParent= source.Parent;
        if(!(source.IsVisibleOnDisplay && !port.IsOutStatePort)) return;

        // No connection to draw if outside clipping area.
        var portPos= port.AnimatedPosition;
        var sourcePos= source.AnimatedPosition;
        Rect displayArea= new Rect(portPos.x, portPos.y, sourcePos.x-portPos.x, sourcePos.y-portPos.y);
        if(displayArea.width < 0) { displayArea.x= sourcePos.x; displayArea.width= portPos.x-sourcePos.x; }
        if(displayArea.height < 0) { displayArea.y= sourcePos.y; displayArea.height= portPos.y-sourcePos.y; }
        if(!IsVisible(displayArea)) return;
        
        // Set connection alpha according to port alpha.
        var alpha= port.DisplayAlpha*source.DisplayAlpha;
        
        // Determine if this connection is part of the selected object.
        float highlightWidth= 2f;
        Color highlightColor= new Color(0.67f, 0.67f, 0.67f, alpha);
        if(port == selectedObject || source == selectedObject || portParent == selectedObject || sourceParent == selectedObject) {
            highlight= true;
        }
        // Determine if this connection is part of a drag.
        Color color= iCS_PreferencesEditor.GetTypeColor(source.RuntimeType);
        color.a*= alpha;
        iCS_BindingParams cp= new iCS_BindingParams(port, portPos, source, sourcePos, iStorage);
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
        GUI.color= new Color(1f,1f,1f,alpha);
        if(port.IsInStatePort || port.IsInTransitionPort) {
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
        // Reset GUI alpha.
        GUI.color= Color.white;
    }
}
