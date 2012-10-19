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
    const float kMinimizeSize    = 32f;
    const float kIconicArea      = kMinimizeSize*kMinimizeSize;
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
    float            Scale= 0f;
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
        BuildValueStyle();
        
        // Set font size according to scale.
        LabelStyle.fontSize= (int)(kLabelFontSize*Scale);
        TitleStyle.fontSize= (int)(kTitleFontSize*Scale);
        ValueStyle.fontSize= (int)(kLabelFontSize*Scale);        
    }
    public void End(iCS_IStorage iStorage) {
		var timeRatio= iStorage.AnimationTimeRatio;
		if(timeRatio.IsActive && timeRatio.IsElapsed) {
			timeRatio.Reset();
		}
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
    void GUI_Box(Rect pos, GUIContent content, Color nodeColor, Color backgroundColor, Color shadowColor) {
        Rect adjPos= TranslateAndScale(pos);
        DrawNode(adjPos, nodeColor, backgroundColor, shadowColor, content);
#if SHOW_TOOLTIP
        string tooltip= content.tooltip;
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
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.FoldedIcon, out foldedIcon)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.UnfoldedIcon, out unfoldedIcon)) {
            IsInitialized= false;
            return IsInitialized;            
        }
        // Load maximize/minimize icon.
        minimizeIcon= iCS_BuiltinTextures.MinimizeIcon;
        if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.MaximizeIcon, out maximizeIcon)) {
            IsInitialized= false;
            return IsInitialized;
        }
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
        if(IsInvisible(node, iStorage) || IsMinimized(node, iStorage)) return;
        
        // Draw node box (if visible).
        Rect position= GetDisplayPosition(node, iStorage);
        if(!IsVisible(position)) return;
        
        string title= GetNodeName(node);
        // Change background color if node is selected.
        Color backgroundColor= GetBackgroundColor(node);
        bool isMouseOver= position.Contains(MousePosition);
#if SHOW_TOOLTIP
        string tooltip= isMouseOver ? GetNodeTooltip(node,iStorage) : null;
#else
        string tooltip= null;
#endif
        GUI_Box(position, new GUIContent(title, tooltip), GetNodeColor(node), backgroundColor, isMouseOver ? WhiteShadowColor : BlackShadowColor);
        if(isMouseOver) {
            EditorGUIUtility_AddCursorRect (new Rect(position.x,  position.y, position.width, kNodeTitleHeight), MouseCursor.Link);            
        }
        // Fold/Unfold icon
        if(ShouldDisplayFoldIcon(node, iStorage)) {
            if(iStorage.IsFolded(node)) {
                GUI_DrawTexture(new Rect(position.x+6f, position.y-1f, foldedIcon.width, foldedIcon.height), foldedIcon);                           
            } else {
                GUI_DrawTexture(new Rect(position.x+6f, position.y-1f, unfoldedIcon.width, unfoldedIcon.height), unfoldedIcon);               
            }            
        }
        // Minimize Icon
        GUI_DrawTexture(new Rect(position.xMax-2-minimizeIcon.width, position.y, minimizeIcon.width, minimizeIcon.height), minimizeIcon);
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
        if(!IsMinimized(node, iStorage)) return;
        
        // Draw minimized node (if visible).
        Rect position= GetDisplayPosition(node, iStorage);
        if(position.width < 12f || position.height < 12f) return;  // Don't show if too small.
        Rect displayArea= new Rect(position.x-100f, position.y-16f, position.width+200f, position.height+16f);
        if(!IsVisible(displayArea)) return;

        Texture icon= GetMaximizeIcon(node);
        Rect texturePos= new Rect(position.x, position.y, icon.width, icon.height);                
        if(node.IsTransitionModule) {
            DrawMinimizedTransitionModule(iStorage.GetTransitionModuleVector(node), texturePos, Color.white);
        } else {
            GUI_DrawTexture(texturePos, icon);                                       
        }
        if(texturePos.Contains(MousePosition)) {
            EditorGUIUtility_AddCursorRect (texturePos, MouseCursor.Link);
#if SHOW_TOOLTIP
            GUI_Label(texturePos, new GUIContent("", GetNodeTooltip(node,iStorage)), LabelStyle);            
#endif
        }
		ShowTitleOver(texturePos, node, iStorage);
    }
    // ----------------------------------------------------------------------
	void ShowTitleOver(Rect pos, iCS_EditorObject node, iCS_IStorage iStorage) {
        if(!ShouldShowTitle()) return;
        string title= GetNodeName(node);
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
            Handles.DrawSolidRectangleWithOutline(vectors, GetBackgroundColor(node), GetNodeColor(node));
        }
        GUI.Label(labelRect, new GUIContent(title, node.Tooltip), LabelStyle);		
	}
	
    // ======================================================================
    // Node style functionality
    // ----------------------------------------------------------------------
    // Returns the display color of the given node.
    static Color GetNodeColor(iCS_EditorObject node) {
        if(node.IsEntryState) {
            return iCS_PreferencesEditor.EntryStateNodeColor;
        }
        if(node.IsState || node.IsStateChart) {
            return iCS_PreferencesEditor.StateNodeColor;
        }
        if(node.IsClassModule) {
            return iCS_PreferencesEditor.InstanceNodeColor;
        }
        if(node.IsModule) {
            return iCS_PreferencesEditor.PackageNodeColor;
        }
        if(node.IsConstructor) {
            return iCS_PreferencesEditor.ConstructorNodeColor;
        }
        if(node.IsFunction) {
            return iCS_PreferencesEditor.FunctionNodeColor;
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
        if(node.IsModule) {
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
//        if(IsInvisible(port, iStorage) || IsMinimized(port, iStorage)) return;
        if(IsInvisible(port, iStorage)) return;
        
        // Don't display if outside clipping area.
		Vector2 portCenter= GetPortCenter(port, iStorage);
		float portRadius= iCS_Config.PortRadius;
        Rect displayArea= new Rect(portCenter.x-200f, portCenter.y-4f*portRadius, 400f, 8f*portRadius);
        if(!IsVisible(displayArea)) return;
        
        // Determine if port is selected.
        bool isSelectedPort= port == selectedObject || (selectedObject != null && selectedObject.IsDataPort && port == iStorage.GetParent(selectedObject));

		// Compute port radius (radius is increased if port is selected).
		if(isSelectedPort) {
			portRadius= 1.67f*iCS_Config.PortRadius;			
		}

        // Get port type information.
        Type portValueType= GetPortValueType(port);
        if(portValueType == null) return;

		// Determine port colors
        Color portColor= iCS_PreferencesEditor.GetTypeColor(portValueType);
        Color nodeColor= GetNodeColor(iStorage.GetParent(port));

        // Draw port icon
		DrawPortIcon(port, portCenter, isSelectedPort, portColor, nodeColor, portRadius, iStorage);

        // Configure move cursor for port.
        Rect portPos= new Rect(portCenter.x-portRadius*1.5f, portCenter.y-portRadius*1.5f, portRadius*3f, portRadius*3f);
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
        if(port.IsStatePort) return;         

        // Determine if port is a static port (a port that feeds information into the graph).
        bool isStaticPort= port.IsInDataPort && iStorage.GetSource(port) == null;

        // Display port name.
        if(!ShouldDisplayPortName(port, iStorage)) return;
        Rect portNamePos= GetPortNameGUIPosition(port, iStorage);
        string name= GetPortName(port);
        GUI.Label(portNamePos, name, LabelStyle);                                    

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
    					Vector2 togglePos= TranslateAndScale(portCenter);
                        var savedBackgroundColor= GUI.backgroundColor;
    					GUI.backgroundColor= portColor;
    					GUI.changed= false;
    					bool newValue= GUI.Toggle(new Rect(togglePos.x-7, togglePos.y-9, 16, 16), (bool)portValue, "");					
                        GUI.backgroundColor= savedBackgroundColor;
    					if(GUI.changed) {
    						iStorage.SetPortValue(port, newValue);
    					}
    				}
    			}
           }
       }
    }

	// ----------------------------------------------------------------------
    public void DrawPortIcon(iCS_EditorObject port, Vector2 portCenter, bool isSelected, Color portColor, Color nodeColor, float portRadius, iCS_IStorage iStorage) {
        // Determine if port is a static port (a port that feeds information into the graph).
        bool isStaticPort= port.IsInDataPort && iStorage.GetSource(port) == null;
        // Draw port icon.
        if(port.IsDataPort) {
            // Don't display mux input ports.
            if(port.IsInMuxPort) return;
            // Data ports.
			if(port.IsOutMuxPort) {
				DrawMuxPort(portCenter, portColor, nodeColor, portRadius);
			} else {
				if(isStaticPort) {
		            DrawValuePort(portCenter, portColor, nodeColor, isSelected);
				} else {
	    	    	DrawDataPort(portCenter, portColor, nodeColor, isSelected);							        
				}				
			}
        } else if(port.IsStatePort) {
            // State ports.
            if(port.IsOutStatePort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(TranslateAndScale(portCenter), FacingNormal, portRadius*Scale);
            }
        } else if(port.IsTransitionPort) {
            // Transition ports.
            if(port.IsOutTransitionPort) {
                Handles.color= Color.white;
                Handles.DrawSolidDisc(TranslateAndScale(portCenter), FacingNormal, portRadius*Scale);                            
            }
        }
        else {
            // All other types of ports (should not exists).
            DrawDataPort(portCenter, portColor, nodeColor, isSelected);
        }        
    }
	// ----------------------------------------------------------------------
    void DrawDataPort(Vector3 _center, Color _fillColor, Color _borderColor, bool isSelected) {
		Vector3 center= TranslateAndScale(_center);
		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedDataPortIcon(_borderColor, _fillColor) :
		                                 iCS_PortIcons.GetDataPortIcon(_borderColor, _fillColor);
		Rect pos= new Rect(center.x-0.5f*portIcon.width,
						   center.y-0.5f*portIcon.height,
						   portIcon.width,
						   portIcon.height);
		GUI.DrawTexture(pos, portIcon);
    }

	// ----------------------------------------------------------------------
    void DrawValuePort(Vector3 _center, Color _fillColor, Color _borderColor, bool isSelected) {
		Vector3 center= TranslateAndScale(_center);
		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedValuePortIcon(_borderColor, _fillColor) :
		                                 iCS_PortIcons.GetValuePortIcon(_borderColor, _fillColor);
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
        float delta= radius*1.75f;

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
        // No connection to draw if no valid source.
        if(!iStorage.IsValid(port.Source)) return;
        iCS_EditorObject portParent= iStorage.GetParent(port);

        // No connection to draw if the parent is not visible.
        if(!IsVisible(portParent, iStorage)) return;

        // No connection to draw if source parent is not visible.
        iCS_EditorObject source= iStorage.GetSource(port);
        iCS_EditorObject sourceParent= iStorage.GetParent(source);
        if(!(IsVisible(sourceParent, iStorage) && !port.IsOutStatePort)) return;
        
        // No connection to draw if outside clipping area.
        Rect portPos= GetDisplayPosition(port, iStorage);
        Rect sourcePos= GetDisplayPosition(source, iStorage);
        Rect displayArea= new Rect(portPos.x, portPos.y, sourcePos.x-portPos.x, sourcePos.y-portPos.y);
        if(displayArea.width < 0) { displayArea.x= sourcePos.x; displayArea.width= portPos.x-sourcePos.x; }
        if(displayArea.height < 0) { displayArea.y= sourcePos.y; displayArea.height= portPos.y-sourcePos.y; }
        if(!IsVisible(displayArea)) return;
        
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
        Color color= iCS_PreferencesEditor.GetTypeColor(source.RuntimeType);
        iCS_ConnectionParams cp= new iCS_ConnectionParams(port, portPos, source, sourcePos, iStorage);
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
    }
    
    // ======================================================================
    //  Utilities
    // ----------------------------------------------------------------------
    static Rect GetDisplayPosition(iCS_EditorObject edObj, iCS_IStorage iStorage) {
		return iStorage.GetDisplayPosition(edObj);
    }
   	// ----------------------------------------------------------------------
 	bool IsMinimized(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        if(!edObj.IsNode) return false;
		var nodeAnimation= iStorage.GetEditorObjectCache(edObj).AnimatedPosition;
        float area= nodeAnimation.CurrentValue.width*nodeAnimation.CurrentValue.height;
        return (area <= kIconicArea+1f);
    }
   	// ----------------------------------------------------------------------
    static bool IsInvisible(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        return !IsVisible(edObj, iStorage);
    }
   	// ----------------------------------------------------------------------
    static bool IsVisible(iCS_EditorObject edObj, iCS_IStorage iStorage) {
        if(edObj.IsNode) {
    		var nodeAnimation= iStorage.GetEditorObjectCache(edObj).AnimatedPosition;
            float area= nodeAnimation.CurrentValue.width*nodeAnimation.CurrentValue.height;
            return Math3D.IsGreater(area, 0.1f);            
        }
        var parentNode= iStorage.GetParentNode(edObj);
        if(parentNode == null) return false;
		var parentAnimation= iStorage.GetEditorObjectCache(parentNode).AnimatedPosition;
        float parentArea= parentAnimation.CurrentValue.width*parentAnimation.CurrentValue.height;
        return parentArea > kIconicArea+1f;  // Parent is visible and not iconic.
    }
}
