using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;


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
        // FIELDS
        // ----------------------------------------------------------------------
        public static bool  IsInitialized= false;    
        static Texture2D    lineTexture       = null;
        static Texture2D    maximizeIcon      = null;
        static Texture2D    upArrowHeadIcon   = null;
        static Texture2D    downArrowHeadIcon = null;
        static Texture2D    leftArrowHeadIcon = null;
        static Texture2D    rightArrowHeadIcon= null;
    	
        // ----------------------------------------------------------------------
        public Texture2D   StateMaximizeIcon       = null;
        public Texture2D   ModuleMaximizeIcon      = null;
        public Texture2D   EntryStateMaximizeIcon  = null;
        public Texture2D   ConstructionMaximizeIcon= null;
        public Texture2D   FunctionMaximizeIcon    = null;
        public Texture2D   DefaultMaximizeIcon     = null;
    
        // ----------------------------------------------------------------------
        public iCS_Layout  Layout= new iCS_Layout();
    
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
            Layout.Refresh(scale);
            
            // Special case for asset store images
            if(iCS_DevToolsConfig.ShowBoldImage) {
                Layout.DynamicLabelStyle.fontSize= (int)(Layout.DynamicLabelStyle.fontSize*1.2f);
                Layout.DynamicValueStyle.fontSize= (int)(Layout.DynamicValueStyle.fontSize*1.2f);
                Layout.DynamicLabelStyle.fontStyle= FontStyle.Bold;
                Layout.DynamicValueStyle.fontStyle= FontStyle.Bold;
            }                
        }
        public void End(iCS_IStorage iStorage) {
        }
        
        // ======================================================================
        // GUI Warppers
    	// ----------------------------------------------------------------------
        bool IsVisibleInViewport(Vector2 point, float radius= 0f) {
            return IsVisibleInViewport(new Rect(point.x-radius, point.y-radius, 2f*radius, 2f*radius));
        }
    	// ----------------------------------------------------------------------
        bool IsVisibleInViewport(Rect r) {
            Rect intersection= Clip(r);
            return Math3D.IsNotZero(intersection.width) && Math3D.IsNotZero(intersection.height);        
        }
    	// ----------------------------------------------------------------------
        bool IsFullyVisibleInViewport(Rect r) {
            return (IsVisibleInViewport(new Vector2(r.x, r.y)) && IsVisibleInViewport(new Vector2(r.xMax, r.yMax)));
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
        public Rect TranslateAndScale(Rect r) {
            Vector2 pos= TranslateAndScale(new Vector2(r.x, r.y));
            return new Rect(pos.x, pos.y, Scale*r.width, Scale*r.height);
        }
    	// ----------------------------------------------------------------------
        Vector2 TranslateAndScale(float x, float y) {
            return new Vector2(Scale*(x-Translation.x), Scale*(y-Translation.y));
        }
        // ----------------------------------------------------------------------
        void GUI_Box(Rect pos, iCS_EditorObject node, Color nodeColor, Color backgroundColor, Color shadowColor) {
            Rect adjPos= TranslateAndScale(pos);
            DrawNode(adjPos, node, nodeColor, backgroundColor, shadowColor);
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
            DrawArrowHead(dir, p, nodeColor, 9f, new Color(0,0,0));
    	}
        // ----------------------------------------------------------------------
        void DrawArrowHead(Vector2 dir, Vector2 p, Color nodeColor, float arrowSize, Color outlineColor) {
            Vector3 center= TranslateAndScale(p);
            Vector3 tangent= Vector3.Cross(dir, Vector3.forward);
            float size= arrowSize*Scale;
            Vector3 head= size*dir;
            Vector3 bottom= size*tangent;
            
            Vector3[] vectors= new Vector3[4];
            vectors[0]= center+head;
            vectors[1]= center-head+bottom;
            vectors[2]= center-0.6f*head;
            vectors[3]= center-head-bottom;
            Handles.color= GUI.color;
            Handles.DrawSolidRectangleWithOutline(vectors, nodeColor, outlineColor);
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
                if(!TextureCache.GetTexture(iCS_EditorStrings.AALineTexture, out lineTexture)) {
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
            if(!TextureCache.GetIcon(iCS_EditorStrings.UpArrowHeadIcon, out upArrowHeadIcon)) {
                IsInitialized= false;
                return IsInitialized;
            }        
            if(!TextureCache.GetIcon(iCS_EditorStrings.DownArrowHeadIcon, out downArrowHeadIcon)) {
                IsInitialized= false;
                return IsInitialized;
            }        
            if(!TextureCache.GetIcon(iCS_EditorStrings.LeftArrowHeadIcon, out leftArrowHeadIcon)) {
                IsInitialized= false;
                return IsInitialized;
            }        
            if(!TextureCache.GetIcon(iCS_EditorStrings.RightArrowHeadIcon, out rightArrowHeadIcon)) {
                IsInitialized= false;
                return IsInitialized;
            }        
            // Graphic resources properly initialized.
            IsInitialized= true;
            return IsInitialized;
        }
        // ----------------------------------------------------------------------
        public void DrawIconCenteredAt(Vector2 point, Texture2D icon) {
            if(icon == null) return;
            GUI_DrawTexture(new Rect(point.x-0.5f*icon.width,point.y-0.5f*icon.height, icon.width, icon.height), icon);
        }
        // ----------------------------------------------------------------------
        void DrawLabelBackground(Rect r, float alpha, Color backgroundColor, Color outlineColor) {
            Vector3[] vectors= new Vector3[4];
            vectors[0]= new Vector3(r.x-2, r.y+2, 0);
            vectors[1]= new Vector3(r.xMax+2, r.y+2, 0);
            vectors[2]= new Vector3(r.xMax+2, r.yMax+2, 0);
            vectors[3]= new Vector3(r.x-2, r.yMax+2, 0);
            Handles.color= new Color(1f, 1f, 1f, alpha);
            Handles.DrawSolidRectangleWithOutline(vectors, backgroundColor, outlineColor);
        }
        
        // ======================================================================
        //  GRID
        // ----------------------------------------------------------------------
        public void DrawGrid(Rect screenArea, Vector2 offset) {
            var backgroundColor = Prefs.CanvasBackgroundColor;
            var minorGridColor  = Prefs.MinorGridColor;
            var majorGridColor  = Prefs.MajorGridColor;
            var minorGridSpacing= Prefs.GridSpacing;
            var screenWidth     = screenArea.width;
            var screenHeight    = screenArea.height;
            if(iCS_DevToolsConfig.UseBackgroundImage) {
                Texture2D background= TextureCache.GetTexture("Assets/DevTools/Editor/resources/background.png");
                var backgroundRect= new Rect(0,0,background.width, background.height);
                GUI.DrawTexture(backgroundRect, background);            
            }
            else {
                // Draw background.
                Vector3[] vect= { new Vector3(0,0,0),
                                  new Vector3(screenWidth, 0, 0),
                                  new Vector3(screenWidth,screenHeight,0),
                                  new Vector3(0,screenHeight,0)};
                Handles.color= Color.white;
                Handles.DrawSolidRectangleWithOutline(vect, backgroundColor, backgroundColor);
    
                // Draw grid lines.
                if(minorGridSpacing*Scale >= 2) {        
                    float xOffset= -Translation.x-offset.x;
                    float yOffset= -Translation.y-offset.y;
                    float majorGridSpacing= 10f*minorGridSpacing;
                    float x= (xOffset)-minorGridSpacing*Mathf.Floor((xOffset)/minorGridSpacing);
                    float y= (yOffset)-minorGridSpacing*Mathf.Floor((yOffset)/minorGridSpacing);
                    float x10= (xOffset)-majorGridSpacing*Mathf.Floor((xOffset)/majorGridSpacing);
                    float y10= (yOffset)-majorGridSpacing*Mathf.Floor((yOffset)/majorGridSpacing);
                
                    // Scale grid
                    x*= Scale;
                    y*= Scale;
                    x10*= Scale;
                    y10*= Scale;
                    minorGridSpacing*= Scale;
                    majorGridSpacing*= Scale;
                
                    if(Scale < 1f) {
                        minorGridColor.a *= Scale;
                        majorGridColor.a *= Scale;
                    }
                    minorGridColor= new Color(minorGridColor.r, minorGridColor.g, minorGridColor.b, 0.5f*minorGridColor.a);
                    var startPoint= new Vector3(0,0,0);
                    var endPoint  = new Vector3(0,screenHeight,0);
                    Handles.color= minorGridColor;
                    for(; x < screenWidth; x+= minorGridSpacing) {
                        startPoint.x= x;
                        endPoint.x  = x;
                        if(Mathf.Abs(x-x10) < 1f) {
                            x10+= majorGridSpacing;
                            Handles.color= majorGridColor;
                            Handles.DrawLine(startPoint, endPoint);
                            Handles.color= minorGridColor;
                        } else {
                            Handles.DrawLine(startPoint, endPoint);
                        }
                    }
                    startPoint.x= 0;
                    endPoint.x  = screenWidth;
                    Handles.color= minorGridColor;
                    for(; y < screenHeight; y+= minorGridSpacing) {
                        startPoint.y= y;
                        endPoint.y  = y;
                        if(Mathf.Abs(y-y10) < 1f) {
                            y10+= majorGridSpacing;
                            Handles.color= majorGridColor;
                            Handles.DrawLine(startPoint, endPoint);
                            Handles.color= minorGridColor;
                        } else {
                            Handles.DrawLine(startPoint, endPoint);
                        }
                    }            
                }
            }
            // Show iCanScript backdrop.
            var backdropStyle= new GUIStyle();
            backdropStyle.fontSize= (int)(screenWidth/20);
            backdropStyle.fontStyle= FontStyle.Bold;
            var backdropColor= Color.grey;
            backdropColor.a= 0.3f;
            backdropStyle.normal.textColor= backdropColor;
            var backdrop= new GUIContent("iCanScript2");
            var backdropSize= backdropStyle.CalcSize(backdrop);
            var spacer= 20f;
            var backdropRect= new Rect(screenWidth-(backdropSize.x+spacer),
                                        screenHeight-(backdropSize.y+spacer),
                                        backdropSize.x,
                                        backdropSize.y);
    //        GUI.color= new Color(1f, 1f, 1f, 0.3f);
            GUI.Label(backdropRect, backdrop, backdropStyle);
            
            // Draw guides for asset store big image
            if(iCS_DevToolsConfig.ShowAssetStoreBigImageFrame) {
                Rect liveRect;
                Rect r= iCS_DevToolsConfig.GetAssetStoreBigImageRect(new Vector2(screenArea.width, screenArea.height), out liveRect);
                if(!iCS_DevToolsConfig.IsSnapshotActive) {
                    Handles.color= Color.red;
                    Handles.DrawLine(new Vector3(r.x, r.y, 0), new Vector3(r.xMax, r.y, 0));
                    Handles.DrawLine(new Vector3(r.x, r.yMax, 0), new Vector3(r.xMax, r.yMax, 0));
                    Handles.DrawLine(new Vector3(r.x, r.y, 0), new Vector3(r.x, r.yMax, 0));
                    Handles.DrawLine(new Vector3(r.xMax, r.y, 0), new Vector3(r.xMax, r.yMax, 0));
                    // Live area
                    Handles.DrawLine(new Vector3(liveRect.x, liveRect.y, 0), new Vector3(liveRect.xMax, liveRect.y, 0));
                    Handles.DrawLine(new Vector3(liveRect.x, liveRect.yMax, 0), new Vector3(liveRect.xMax, liveRect.yMax, 0));
                    Handles.DrawLine(new Vector3(liveRect.x, liveRect.y, 0), new Vector3(liveRect.x, liveRect.yMax, 0));
                    Handles.DrawLine(new Vector3(liveRect.xMax, liveRect.y, 0), new Vector3(liveRect.xMax, liveRect.yMax, 0));                            
                }
                    // Show iCanScript Title
                    Texture2D title= TextureCache.GetTexture("Assets/DevTools/Editor/resources/iCanScript.png");
                    if(title != null) {
                        float titleRatio= 0.65f;
                        float reservedTitleWidth= titleRatio*liveRect.width;
                        var titleWidth= Mathf.Min(title.width, reservedTitleWidth);
                        var titleHeight= title.height*(titleWidth/title.width);
                        var yMin= 0.5f*(liveRect.y + r.y);
                        var titleRect= new Rect(liveRect.xMax-titleWidth, yMin, titleWidth, titleHeight);    
                        GUI.DrawTexture(titleRect, title);
                    }                
            }
            // Draw guides for asset store big image
            if(iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame && !iCS_DevToolsConfig.IsSnapshotActive) {
                Rect liveRect;
                Rect r= iCS_DevToolsConfig.GetAssetStoreSmallImageRect(new Vector2(screenArea.width, screenArea.height), out liveRect);
                Handles.color= Color.red;
                Handles.DrawLine(new Vector3(r.x, r.y, 0), new Vector3(r.xMax, r.y, 0));
                Handles.DrawLine(new Vector3(r.x, r.yMax, 0), new Vector3(r.xMax, r.yMax, 0));
                Handles.DrawLine(new Vector3(r.x, r.y, 0), new Vector3(r.x, r.yMax, 0));
                Handles.DrawLine(new Vector3(r.xMax, r.y, 0), new Vector3(r.xMax, r.yMax, 0));
                // Live area
                Handles.DrawLine(new Vector3(liveRect.x, liveRect.y, 0), new Vector3(liveRect.xMax, liveRect.y, 0));
                Handles.DrawLine(new Vector3(liveRect.x, liveRect.yMax, 0), new Vector3(liveRect.xMax, liveRect.yMax, 0));
                Handles.DrawLine(new Vector3(liveRect.x, liveRect.y, 0), new Vector3(liveRect.x, liveRect.yMax, 0));
                Handles.DrawLine(new Vector3(liveRect.xMax, liveRect.y, 0), new Vector3(liveRect.xMax, liveRect.yMax, 0));            
                // Show iCanScript Title
                Texture2D title= TextureCache.GetTexture("Assets/DevTools/Editor/resources/iCanScript.png");
                if(title != null) {
                    var titleWidth= Mathf.Min(title.width, liveRect.width);
                    var titleHeight= title.height*(titleWidth/title.width);
                    var titleRect= new Rect(liveRect.x+0.5f*(liveRect.width-titleWidth), liveRect.y, titleWidth, titleHeight);    
                    GUI.DrawTexture(titleRect, title);
                }                                
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
            if(!IsVisibleInViewport(position)) return;
    
            // Draw node since all draw conditions are valid.
    		float alpha= node.DisplayAlpha;
    		if(ControlFlow.IsDisabledInEditorMode(node)) {
    			alpha*= 0.5f;
    		}
            GUI.color= new Color(1f, 1f, 1f, alpha);
            // Change background color if node is selected.
            Color backgroundColor= GetBackgroundColor(node);
            bool isMouseOver= position.Contains(MousePosition);
    		
            // Determine title style
            var shadowColor= isMouseOver || iStorage.IsSelectedOrMultiSelected(node) ? WhiteShadowColor : BlackShadowColor;
            GUI_Box(position, node, GetNodeColor(node), backgroundColor, shadowColor);
            if(isMouseOver) {
                EditorGUIUtility_AddCursorRect (new Rect(position.x,  position.y, position.width, kNodeTitleHeight), MouseCursor.Link);            
            }
            // Fold/Unfold icon
            if(ShouldDisplayFoldIcon(node)) {
                var icon= node.IsFoldedInLayout ? iCS_BuiltinTextures.FoldIcon(Scale) : iCS_BuiltinTextures.UnfoldIcon(Scale);
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
            if(node.IStorage.IsSelectedOrMultiSelected(node)) {
                return Prefs.SelectedBackgroundColor;
            }
            return Prefs.BackgroundColor;        
        }
        // ----------------------------------------------------------------------
        public void DrawMinimizedNode(iCS_EditorObject node, iCS_IStorage iStorage) {        
            if(!node.IsIconizedOnDisplay) return;
            
            // Draw minimized node (if visible).
            Rect displayRect= node.AnimatedRect;
            Rect displayArea= new Rect(displayRect.x-100f, displayRect.y-16f, displayRect.width+200f, displayRect.height+16f);
            if(!IsVisibleInViewport(displayArea)) return;
    
    		float alpha= node.DisplayAlpha;
    		if(ControlFlow.IsDisabledInEditorMode(node)) {
    			alpha*= 0.5f;
    		}
    		Color alphaWhite= new Color(1f, 1f, 1f, alpha);
            GUI.color= alphaWhite;
            Texture icon= Icons.GetIconFor(node);
    		var position= Math3D.Middle(displayRect);
            var iconWidth= icon.width;
            var iconHeight= icon.height;
            // Limit icon to 64x64
            if(iconWidth > 64 || iconHeight > 64) {
                var maxLen= Mathf.Max(iconWidth, iconHeight);
                var factor= 64f/maxLen;
                iconWidth = (int)((iconWidth *factor)+0.1f);
                iconHeight= (int)((iconHeight*factor)+0.1f);
            }
            Rect textureRect= new Rect(position.x-0.5f*iconWidth, position.y-0.5f*iconHeight, iconWidth, iconHeight);                
            if(node.IsTransitionPackage) {
                DrawMinimizedTransitionModule(iStorage.GetTransitionPackageVector(node), position, alphaWhite);
            } else {
                GUI_DrawTexture(textureRect, icon);                                       
            }
            if(textureRect.Contains(MousePosition)) {
                EditorGUIUtility_AddCursorRect (textureRect, MouseCursor.Link);
            }
    		ShowTitleOver(textureRect, node);
            GUI.color= Color.white;
        }
        // ----------------------------------------------------------------------
    	void ShowTitleOver(Rect pos, iCS_EditorObject node) {
            if(!ShouldShowTitle()) return;
            string title= GetNodeTitle(node);
    //        string title= GetNodeName(node); // Name too long with stereotype
            Vector2 labelSize= GetNodeNameSize(node);
    		pos.y-=5f;	// Put title a bit higher.
            pos= TranslateAndScale(pos);
            Rect labelRect= new Rect(0.5f*(pos.x+pos.xMax-labelSize.x), pos.y-labelSize.y, labelSize.x, labelSize.y);
            var boxAlpha= 0.10f;
            var outlineColor= Color.black;
            var backgroundColor= Color.black;
            if(node.IStorage.IsSelectedOrMultiSelected(node)) {
                boxAlpha= 1f;
                outlineColor= GetNodeColor(node);
                backgroundColor= GetBackgroundColor(node);
            }
            DrawLabelBackground(labelRect, boxAlpha, backgroundColor, outlineColor);
            GUI.Label(labelRect, new GUIContent(title), Layout.DynamicLabelStyle);		
    	}
    	
        // ======================================================================
        // Node style functionality
        // ----------------------------------------------------------------------
        // Returns the display color of the given node.
        static Color GetNodeColor(iCS_EditorObject node) {
            if(node.IsBehaviour) {
                return new Color(0.75f, 0.75f, 0.75f);
            }
            if(node.IsEventHandler) {
                return Prefs.MessageNodeColor;
            }
            if(node.IsFunctionDefinition) {
                return Prefs.UserFunctionNodeColor;
            }
            if(node.IsEntryState) {
                return Prefs.EntryStateNodeColor;
            }
            if(node.IsState || node.IsStateChart) {
                return Prefs.StateNodeColor;
            }
            if(node.IsInstanceNode) {
                return Prefs.InstanceNodeColor;
            }
            if(node.IsConstructor) {
                return Prefs.ConstructorNodeColor;
            }
            if(node.IsKindOfFunction) {
                return Prefs.FunctionNodeColor;
            }
            if(node.IsKindOfPackage) {
                return Prefs.PackageNodeColor;
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
            if(node.IsKindOfPackage) {
                return BuildMaximizeIcon(node, ref ModuleMaximizeIcon);
            }
            if(node.IsConstructor) {
                return BuildMaximizeIcon(node, ref ConstructionMaximizeIcon);
            }
            if(node.IsKindOfFunction) {
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
            if(!IsVisibleInViewport(displayArea)) return;
            
            // Determine if port is selected.
            bool isSelectedPort= iStorage.IsSelectedOrMultiSelected(port) ||
                                 (selectedObject != null && selectedObject.IsDataOrControlPort && port == selectedObject.Parent);
            
            // Special case for asset store images
    		// Compute port radius (radius is increased if port is selected).
            bool useLargePort= false;
    		if(isSelectedPort || iCS_DevToolsConfig.ShowBoldImage) {
    			portRadius= iCS_EditorConfig.PortRadius*iCS_EditorConfig.SelectedPortFactor;			
                useLargePort= true;
    		}
    
            // Get port type information.
            Type portValueType= GetPortValueType(port);
            if(portValueType == null) return;
    
            // Port alpha
            var alpha= port.DisplayAlpha;
            if(parent.IsIconizedInLayout && parent.IsAnimated) {
                alpha= 1f-parent.AnimationTimeRatio;
            }
    		// Reduce alpha if port is disabled
    		if(ControlFlow.IsDisabledInEditorMode(parent)) {
    			alpha*= 0.5f;
    		}
            GUI.color= new Color(1f,1f,1f,alpha);
            
    		// Determine port colors
            Color portColor= Prefs.GetTypeColor(portValueType);
            Color nodeColor= GetNodeColor(port.Parent);
    
            // Draw port icon
    		DrawPortIcon(port, portCenter, useLargePort, portColor, nodeColor, portRadius, iStorage);
    
            // Configure move cursor for port.
            Rect portPos= new Rect(portCenter.x-portRadius, portCenter.y-portRadius, 2f*portRadius, 2f*portRadius);
            if(portPos.Contains(MousePosition)) {
                if(!port.IsTransitionPort) {
                    EditorGUIUtility_AddCursorRect (portPos, MouseCursor.Link);            
                }        
            }            
    		
            // State transition name is handle by DrawConnection.
            if(port.IsStatePort || port.IsTransitionPort) return;         
    
            // Display port name.
            if(ShouldDisplayPortName(port)) {
    	        Rect portNamePos= GetPortNameGUIPosition(port);
                var boxAlpha= 0.35f;
                var backgroundColor= Color.clear;
                var outlineColor= Color.clear;
                if(isSelectedPort) {
                    boxAlpha= 1f;
                    outlineColor= portColor;
                }
                if(parent.IsKindOfPackage && parent.IsUnfoldedInLayout) {
                    backgroundColor= Color.black;
                }
                DrawLabelBackground(portNamePos, boxAlpha, backgroundColor, outlineColor);
    	        string name= GetPortName(port);
    	        GUI.Label(portNamePos, name, Layout.DynamicLabelStyle);                                            	
            }
    
            // Display port value (if applicable).
            if(ShouldDisplayPortValue(port)) {    
                if(!port.IsFloating && !port.IsEnablePort) {
        			EditorGUIUtility.LookLikeControls();
                    Rect portValuePos= GetPortValueGUIPosition(port);
            		if(Math3D.IsNotZero(portValuePos.width)) {
                        DrawLabelBackground(portValuePos, 0.35f, Color.black, Color.black);
                		string valueAsStr= GetPortValueAsString(port);
            			GUI.Label(portValuePos, valueAsStr, Layout.DynamicValueStyle);			
            		}            				
        
    //                /*
    //                    CHANGED: ==> Experimental <==
    //                */
    //    			// Bring up port editor for selected static ports.
    //                object portValue= port.PortValue;
    //    			if(isStaticPort && portValue != null && Scale > 0.75f) {
    //    				EditorGUIUtility.LookLikeControls();
    //    				if(portValueType == typeof(bool) && !port.IsEnablePort) {
    //    					Vector2 togglePos= TranslateAndScale(portCenter);
    //                        var savedBackgroundColor= GUI.backgroundColor;
    //    					GUI.backgroundColor= portColor;
    //    					GUI.changed= false;
    //                        bool currentValue= (bool)portValue;
    //    					bool newValue= GUI.Toggle(new Rect(togglePos.x-7, togglePos.y-9, 16, 16), currentValue, "");					
    //                        GUI.backgroundColor= savedBackgroundColor;
    //    					if(GUI.changed) {
    //    						port.PortValue= newValue;
    //    					}
    //    				}
    //    			}
               }
           }
           // Reset GUI alpha.
           GUI.color= Color.white;
        }
    
    	// ----------------------------------------------------------------------
        public void DrawPortIcon(iCS_EditorObject port, Vector2 portCenter, bool isSelected,
                                 Color portColor, Color nodeColor, float portRadius, iCS_IStorage iStorage) {
            // Draw port icon.
            if(port.IsDataOrControlPort) {
                // Don't display mux input ports.
                if(port.IsChildMuxPort) return;
                // Data ports.
    			if(port.IsOutParentMuxPort) {
                    DrawOutMuxPort(portCenter, portColor, isSelected, port.Edge);
    			} else if(port.IsInParentMuxPort) {
                    DrawInMuxPort(portCenter, portColor, isSelected, port.Edge);
    			} else if(port.IsControlPort) {
    	    	    DrawControlPort(port, portCenter, portColor, isSelected);							        			    
    			} else {
    	    	    DrawDataPort(port, portCenter, portColor, isSelected);							        
    			}
            } else if(port.IsStatePort) {
                // State ports.
                if(port.IsOutStatePort) {
    				var color= GUI.color;
    				color.a*= 0.8f;
                    Handles.color= color;
                    Handles.DrawSolidDisc(TranslateAndScale(portCenter), FacingNormal, 0.65f*portRadius*Scale);
                }
            } else if(port.IsTransitionPort) {
                // Transition ports.
                if(port.IsOutTransitionPort) {
    				var color= GUI.color;
    				color.a*= 0.8f;
                    Handles.color= color;
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
                switch(port.PortSpec) {
                    case PortSpecification.Constant: {
        				DrawConstantPort(_center, _fillColor, isSelected);                        
                        break;
                    }
					case PortSpecification.Owner:
                    case PortSpecification.PublicVariable: {
        				DrawInPublicVariablePort(_center, _fillColor, isSelected);                        
                        break;
                    }
                    case PortSpecification.PrivateVariable: {
        				DrawInPrivateVariablePort(_center, _fillColor, isSelected);                        
                        break;
                    }
                    case PortSpecification.StaticPublicVariable: {
        				DrawInStaticPublicVariablePort(_center, _fillColor, isSelected);                        
                        break;
                    }
                    case PortSpecification.StaticPrivateVariable: {
        				DrawInStaticPrivateVariablePort(_center, _fillColor, isSelected);                        
                        break;
                    }
                    case PortSpecification.Parameter: {
        				DrawInParameterPort(_center, _fillColor, isSelected);                        
                        break;
                    }
                    default: {
        				DrawInLocalVariablePort(_center, _fillColor, isSelected);
                        break;                        
                    }
                }
    		} else {
                switch(port.PortSpec) {
					case PortSpecification.Owner:
                    case PortSpecification.PublicVariable: {
        				DrawOutPublicVariablePort(_center, _fillColor, isSelected);
                        break;
                    }
                    case PortSpecification.PrivateVariable: {
        				DrawOutPrivateVariablePort(_center, _fillColor, isSelected);
                        break;
                    }
                    case PortSpecification.StaticPublicVariable: {
        				DrawOutStaticPublicVariablePort(_center, _fillColor, isSelected);
                        break;
                    }
                    case PortSpecification.StaticPrivateVariable: {
        				DrawOutStaticPrivateVariablePort(_center, _fillColor, isSelected);
                        break;
                    }
                    case PortSpecification.Parameter: {
        				DrawOutParameterPort(_center, _fillColor, isSelected);                        
                        break;
                    }
                    default: {
        				DrawOutLocalVariablePort(_center, _fillColor, isSelected);
                        break;                        
                    }
                }
    		}
        }
    	// ----------------------------------------------------------------------
        void DrawControlPort(iCS_EditorObject port, Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= null;
    		if(port.IsInputPort) {
    			portIcon= isSelected ? iCS_PortIcons.GetSelectedEnablePortIcon(_fillColor) :
    			                       iCS_PortIcons.GetEnablePortIcon(_fillColor);
    		} else {
    			portIcon= isSelected ? iCS_PortIcons.GetSelectedTriggerPortIcon(_fillColor) :
    			                       iCS_PortIcons.GetTriggerPortIcon(_fillColor);			
    		}
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawInLocalVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInLocalVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetInLocalVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawOutLocalVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutLocalVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetOutLocalVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawInPublicVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInPublicVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetInPublicVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawOutPublicVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutPublicVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetOutPublicVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawInPrivateVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInPrivateVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetInPrivateVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawOutPrivateVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutPrivateVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetOutPrivateVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawInStaticPublicVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInStaticPublicVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetInStaticPublicVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawOutStaticPublicVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutStaticPublicVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetOutStaticPublicVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawInStaticPrivateVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInStaticPrivateVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetInStaticPrivateVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawOutStaticPrivateVariablePort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutStaticPrivateVariablePortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetOutStaticPrivateVariablePortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawInParameterPort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedInParameterPortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetInParameterPortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawOutParameterPort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedOutParameterPortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetOutParameterPortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawConstantPort(Vector3 _center, Color _fillColor, bool isSelected) {
    		Texture2D portIcon= isSelected ? iCS_PortIcons.GetSelectedConstantPortIcon(_fillColor) :
    		                                 iCS_PortIcons.GetConstantPortIcon(_fillColor);
            DrawSymetricalPort(_center, portIcon);
        }
    	// ----------------------------------------------------------------------
        void DrawSymetricalPort(Vector3 _center, Texture2D portIcon) {
    		Vector3 center= TranslateAndScale(_center);
    		Rect pos= new Rect(center.x-0.5f*portIcon.width,
    						   center.y-0.5f*portIcon.height,
    						   portIcon.width,
    						   portIcon.height);
    		GUI.DrawTexture(pos, portIcon);
        }
    
    
    	// ----------------------------------------------------------------------
        void DrawInMuxPort(Vector3 _center, Color _fillColor, bool isSelected, iCS_EdgeEnum edge) {
    		Vector3 center= TranslateAndScale(_center);
    		Texture2D portIcon= null; 
            switch(edge) {
                case iCS_EdgeEnum.Top:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedInMuxPortTopIcon(_fillColor) :
        		                           iCS_PortIcons.GetInMuxPortTopIcon(_fillColor);
        		    break;
                case iCS_EdgeEnum.Bottom:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedInMuxPortBottomIcon(_fillColor) :
        		                           iCS_PortIcons.GetInMuxPortBottomIcon(_fillColor);
        		    break;
                case iCS_EdgeEnum.Left:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedInMuxPortLeftIcon(_fillColor) :
        		                           iCS_PortIcons.GetInMuxPortLeftIcon(_fillColor);
        		    break;
                case iCS_EdgeEnum.Right:
                default:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedInMuxPortRightIcon(_fillColor) :
        		                           iCS_PortIcons.GetInMuxPortRightIcon(_fillColor);
        		    break;
            }
    		Rect pos= new Rect(center.x-0.5f*portIcon.width,
    						   center.y-0.5f*portIcon.height,
    						   portIcon.width,
    						   portIcon.height);
    		GUI.DrawTexture(pos, portIcon);
        }   
    	// ----------------------------------------------------------------------
        void DrawOutMuxPort(Vector3 _center, Color _fillColor, bool isSelected, iCS_EdgeEnum edge) {
    		Vector3 center= TranslateAndScale(_center);
    		Texture2D portIcon= null; 
            switch(edge) {
                case iCS_EdgeEnum.Top:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedOutMuxPortTopIcon(_fillColor) :
        		                           iCS_PortIcons.GetOutMuxPortTopIcon(_fillColor);
        		    break;
                case iCS_EdgeEnum.Bottom:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedOutMuxPortBottomIcon(_fillColor) :
        		                           iCS_PortIcons.GetOutMuxPortBottomIcon(_fillColor);
        		    break;
                case iCS_EdgeEnum.Left:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedOutMuxPortLeftIcon(_fillColor) :
        		                           iCS_PortIcons.GetOutMuxPortLeftIcon(_fillColor);
        		    break;
                case iCS_EdgeEnum.Right:
                default:
        		    portIcon= isSelected ? iCS_PortIcons.GetSelectedOutMuxPortRightIcon(_fillColor) :
        		                           iCS_PortIcons.GetOutMuxPortRightIcon(_fillColor);
        		    break;
            }
    		Rect pos= new Rect(center.x-0.5f*portIcon.width,
    						   center.y-0.5f*portIcon.height,
    						   portIcon.width,
    						   portIcon.height);
    		GUI.DrawTexture(pos, portIcon);
        }   
        
        // ======================================================================
        //  CONNECTION
        // ----------------------------------------------------------------------
        public void DrawBinding(iCS_EditorObject port, iCS_IStorage iStorage, bool highlight= false, float lineWidth= 1.5f) {
            // No connection to draw if no valid source.
            if(!port.IsSourceValid) return;
            iCS_EditorObject portParent= port.ParentNode;
    
            // No connection to draw if the port is not visible.
            bool isPortVisible= port.IsVisibleOnDisplay || port.IsFloating;
            bool isShowInvisiblePort= false;
            if(port.IsStatePort || port.IsTransitionPort) {
                // For state ports, we draw them if the transition module is visible.
                var transitionPackage= port.IStorage.GetTransitionPackage(port);
                if(transitionPackage != null) {
                    if(!transitionPackage.IsVisibleOnDisplay) return;
                    isShowInvisiblePort= true;
                }
                else {
                    if(isPortVisible == false) return;                
                }
            } else {
                if(isPortVisible == false) return;
            }
    
            // No connection to draw if source port is not visible.
            iCS_EditorObject source= port.VisibleProducerPort;
    		if(source == null) return;
            iCS_EditorObject sourceParent= source.Parent;
            bool isSourceVisible= source.IsVisibleOnDisplay;
            if(isSourceVisible == false && isShowInvisiblePort == false) return;
            if(port.IsOutStatePort) return;
            
            // No connection to draw if outside clipping area.
            var portPos = port.AnimatedPosition;
            var sourcePos= source.AnimatedPosition;
            if(!isPortVisible) {
                var visibleNode= portParent.GetLeafVisibleNode();
                var parentRect= visibleNode.GlobalRect;
                portPos= ClosestPointOnRectOnEdge(parentRect, sourcePos, port.Edge);
                Math3D.LineSegmentAndRectEdgeIntersection(sourcePos, portPos, parentRect, out portPos);
            }
            if(!isSourceVisible) {
                var visibleNode= sourceParent.GetLeafVisibleNode();
                var parentRect= visibleNode.GlobalRect;
                sourcePos= Math3D.Middle(parentRect);
                sourcePos= ClosestPointOnRectOnEdge(parentRect, portPos, source.Edge);
                Math3D.LineSegmentAndRectEdgeIntersection(portPos, sourcePos, parentRect, out sourcePos);
            }
            Rect displayArea= Math3D.Union(new Rect(portPos.x, portPos.y, 1f, 1f), new Rect(sourcePos.x, sourcePos.y, 1f, 1f));
            if(!IsVisibleInViewport(displayArea)) return;
    
            // Set connection alpha according to port alpha.
            var alpha= 1f;
            if(isShowInvisiblePort) {
                alpha= Mathf.Max(port.DisplayAlpha, source.DisplayAlpha);
            }
            else {
                alpha= port.DisplayAlpha*source.DisplayAlpha;
            }
            
    		// Reduce alpha if consumer port is disable
    		if(ControlFlow.IsDisabledInEditorMode(sourceParent) ||
               ControlFlow.IsDisabledInEditorMode(portParent)) {
    			alpha*= 0.5f;
    		}
    		
            // Determine line color.
            Color sourceColor= Color.white;
            Color portColor  = Color.white;
            if(port.IsStatePort || port.IsTransitionPort) {
                if(isPortVisible == false || isSourceVisible == false) {
                    sourceColor= Color.yellow;
                    portColor  = Color.yellow;
                }
            }
            else {
                sourceColor= Prefs.GetTypeColor(source.RuntimeType);            
                portColor  = Prefs.GetTypeColor(port.RuntimeType);            
            }
            sourceColor.a*= alpha;
            portColor.a  *= alpha;
            // Determine if this connection is part of the selected object.
            float highlightWidth= 2f;
            Color highlightColor= new Color(0.67f, 0.67f, 0.67f, alpha);
            if(iStorage.IsSelectedOrMultiSelected(port) ||
               iStorage.IsSelectedOrMultiSelected(source)) {
               highlight= true;
            }
            // Special case for asset store images.
            if(iCS_DevToolsConfig.ShowBoldImage) {
                highlight= true;
                highlightColor= sourceColor;
            }
            // Determine if this connection is part of a drag.
            iCS_BindingParams cp= new iCS_BindingParams(port, portPos, source, sourcePos, iStorage);
            if(highlight) {
                DrawBezier(cp, sourceColor, portColor, highlightColor, lineWidth, highlightWidth);
            } else {
                sourceColor.a= 0.85f*sourceColor.a;
                portColor.a  = 0.85f*portColor.a;
                DrawBezier(cp, sourceColor, portColor, highlightColor, lineWidth, 0f);
            }
            // Show transition name for state connections.
    		if(port.IsInTransitionPort && portParent.IsIconizedInLayout) return;
            if(port.IsInStatePort || port.IsInTransitionPort) {
                var arrowColor= new Color(1f,1f,1f,alpha);
                DirectionEnum dir= DirectionEnum.Up;
                // Reposition the end to make space for the arrow
                Vector2 normalizedEndTangent= new Vector2(cp.EndTangent.x-cp.End.x, cp.EndTangent.y-cp.End.y);
                normalizedEndTangent.Normalize();
                // Show transition input port.
                if(Mathf.Abs(normalizedEndTangent.x) > Mathf.Abs(normalizedEndTangent.y)) {
                    if(normalizedEndTangent.x > 0) {
                        dir= DirectionEnum.Left;
                    } else {
                        dir= DirectionEnum.Right;
                    }
                } else {
                    if(normalizedEndTangent.y > 0) {
                        dir= DirectionEnum.Up;
                    } else {
                        dir= DirectionEnum.Down;
                    }
                }                 
                ShowArrowCenterOn(cp.End, arrowColor, dir);
            }
            else {
                // Show binding direction
                if(ShouldShowPort()) {
                    DrawArrowMiddleBezier(cp, sourceColor, highlight);
                }
            }
        }
        // ----------------------------------------------------------------------
        void DrawBezier(iCS_BindingParams cp,
                        Color startColor, Color endColor, Color highlightColor,
                        float lineWidth, float highlightWidth) {
    
            // Adjust to canvas settings
            Vector3 startPos= TranslateAndScale(cp.Start);
            Vector3 endPos= TranslateAndScale(cp.End);
            Vector3 startTangent= TranslateAndScale(cp.StartTangent);
            Vector3 endTangent= TranslateAndScale(cp.EndTangent);
            lineWidth*= Scale; if(lineWidth < 1f) lineWidth= 1f;
    
            // Simple case where the start and end point are of the same color.
            if(startColor == endColor) {
                if(highlightWidth != 0) {
                    highlightWidth*= Scale; if(highlightWidth < 1f) highlightWidth= 1f;
            		Handles.DrawBezier(startPos, endPos, startTangent, endTangent, highlightColor, lineTexture, lineWidth+highlightWidth);
                }
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, startColor, lineTexture, lineWidth);
            }
    
            // Different colors for start & end points.  Draw two bezier curves of different colors attached in the center.
            else {
                Vector3 center= TranslateAndScale(cp.Center);
                // Adjust tangent strength to half since we are creating two beziers.
                float tangentMagnitude= 0.25f*(cp.StartTangent-cp.Start).magnitude;
                Vector3 centerStartTangent=  TranslateAndScale(cp.Center+(cp.CenterDirection * tangentMagnitude));
                Vector3 centerEndTangent  =  TranslateAndScale(cp.Center-(cp.CenterDirection * tangentMagnitude));
                startTangent= startPos + 0.5f*(startTangent-startPos);
                endTangent  = endPos   + 0.5f*(endTangent-endPos);
                if(highlightWidth != 0) {
                    highlightWidth*= Scale; if(highlightWidth < 1f) highlightWidth= 1f;
                    Handles.DrawBezier(startPos, center, startTangent, centerEndTangent, highlightColor, lineTexture, lineWidth+highlightWidth);
                    Handles.DrawBezier(center, endPos, centerStartTangent, endTangent, highlightColor, lineTexture, lineWidth+highlightWidth);                
                }
                Handles.DrawBezier(startPos, center, startTangent, centerEndTangent, startColor, lineTexture, lineWidth);
                Handles.DrawBezier(center, endPos, centerStartTangent, endTangent, endColor, lineTexture, lineWidth);
            }
        }
        // ----------------------------------------------------------------------
        public void ShowBindingArrow(iCS_EditorObject port, Vector2 pos, Vector2 tangent, Color bindingColor) {
            DirectionEnum dir= DirectionEnum.Up;
            switch(port.Edge) {
                case iCS_EdgeEnum.Top:
                case iCS_EdgeEnum.Bottom: {
                    if(tangent.y < 0) {
                        dir= DirectionEnum.Down;                                        
                    }
                    else {
                        dir= DirectionEnum.Up;                    
                    }
                    break;
                }
                case iCS_EdgeEnum.Left:
                case iCS_EdgeEnum.Right: {
                    if(tangent.x < 0) {
                        dir= DirectionEnum.Right;
                    }
                    else {
                        dir= DirectionEnum.Left;
                    }
                    break;
                }
            }
            ShowArrowCenterOn(pos, bindingColor, dir);
        }
        // ----------------------------------------------------------------------
        public Vector2 GetBindingEndPosition(iCS_EditorObject port, Vector2 endPos, Vector2 tangent) {
            switch(port.Edge) {
                case iCS_EdgeEnum.Top:
                case iCS_EdgeEnum.Bottom: {
                    if(tangent.y < 0) {
                        endPos.y-= iCS_EditorConfig.PortDiameter;
                    }
                    else {
                        endPos.y+= iCS_EditorConfig.PortDiameter;
                    }
                    break;
                }
                case iCS_EdgeEnum.Left:
                case iCS_EdgeEnum.Right: {
                    if(tangent.x < 0) {
                        endPos.x-= iCS_EditorConfig.PortDiameter;
                    }
                    else {
                        endPos.x+= iCS_EditorConfig.PortDiameter;
                    }
                    break;
                }
            }
            return endPos;
        }
        // ----------------------------------------------------------------------
        public void ShowArrowCenterOn(Vector2 graphPos, Color arrowColor, DirectionEnum dir) {
    		var savedColor= GUI.color;
            GUI.color= arrowColor;
            switch(dir) {
                case DirectionEnum.Left:  DrawIconCenteredAt(graphPos, leftArrowHeadIcon); break;
                case DirectionEnum.Right: DrawIconCenteredAt(graphPos, rightArrowHeadIcon); break;
                case DirectionEnum.Up:    DrawIconCenteredAt(graphPos, upArrowHeadIcon); break;
                case DirectionEnum.Down:  DrawIconCenteredAt(graphPos, downArrowHeadIcon); break;
            }
            // Reset GUI alpha.
            GUI.color= savedColor;
        }
        // ----------------------------------------------------------------------
        public void DrawArrowMiddleBezier(iCS_BindingParams cp, Color color, bool highlight) {
            color.a*= 0.8f;
            float size= highlight ? 6f : 4f;
            DrawArrowHead(cp.CenterDirection, cp.Center, color, size, color);
        }
        // ----------------------------------------------------------------------
    	bool IsDisable(iCS_EditorObject obj) { return !IsEnable(obj); }
    	bool IsEnable(iCS_EditorObject obj) {
    		if(obj.IsBehaviour) return true;
    		if(UnityUtility.IsPrefab(obj.IStorage.VSMonoBehaviour.gameObject)) return true;
    		if(obj.IsInStatePort) {
    			var transitionPackage= obj.IStorage.GetTransitionPackage(obj);
    			if(transitionPackage == null) {
    				return true;
    			}
    			return IsEnable(transitionPackage);
    		}
    		if(!IsEnable(obj.ParentNode)) return false;
    		if(obj.IsPort) return !obj.IsPortDisabled;
    		bool isPlaying= Application.isPlaying;
    		if(obj.IsState) {
    			return isPlaying ? IsActiveState(obj) : true;
    		}
    		if(obj.IsTransitionPackage) {
    			var fromStatePort= obj.IStorage.GetFromStatePort(obj);
    			if(fromStatePort == null) return true;
    			var fromState= fromStatePort.ParentNode;
    			return IsEnable(fromState);
    		}
            bool isEnabled= true;
    		obj.ForEachChildPort(
                p=> {
        			if(p.IsEnablePort && (isPlaying || p.ProducerPort == null)) {
                        var portValue= p.Value;
        				if(portValue != null && (bool)(portValue) == false) {
        					isEnabled= false;
        				}				
        			}
                }
            );
    		return isEnabled;
    	}
        // ----------------------------------------------------------------------
        public static bool IsActiveState(iCS_EditorObject state) {
            // TODO: Determine state chart (runtime) active state.
            return true;
        }
        
        // ----------------------------------------------------------------------
        /// Returns the closest point on the given edge
        Vector2 ClosestPointOnRectOnEdge(Rect r, Vector2 p, iCS_EdgeEnum edge) {
            Vector2 result= Vector2.zero;
            switch(edge) {
                case iCS_EdgeEnum.Left: {
                    result= Math3D.ClosestPointOnLineSegmentToPoint(Math3D.TopLeftCorner(r), Math3D.BottomLeftCorner(r), p);
                    break;
                }
                case iCS_EdgeEnum.Right: {
                    result= Math3D.ClosestPointOnLineSegmentToPoint(Math3D.TopRightCorner(r), Math3D.BottomRightCorner(r), p);
                    break;                    
                }
                case iCS_EdgeEnum.Top: {
                    result= Math3D.ClosestPointOnLineSegmentToPoint(Math3D.TopLeftCorner(r), Math3D.TopRightCorner(r), p);
                    break;
                }
                case iCS_EdgeEnum.Bottom: {
                    result= Math3D.ClosestPointOnLineSegmentToPoint(Math3D.BottomLeftCorner(r), Math3D.BottomRightCorner(r), p);
                    break;
                }
            }
            return result;
        }
    }

}