using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public class iCS_Layout {
        // ======================================================================
        // CONSTANTS
        // ----------------------------------------------------------------------
        const int   kLabelFontSize= iCS_EditorConfig.kLabelFontSize;
        const int   kTitleFontSize= iCS_EditorConfig.kTitleFontSize;

        // ======================================================================
        // FIELDS
        // -------------------------------------------------------------------
        // These are all the GUI styles needed to layout a node
        public static GUIStyle  DefaultTitleStyle   = null;
        public static GUIStyle  DefaultSubTitleStyle= null;
        public static GUIStyle  DefaultLabelStyle   = null;
        public static GUIStyle  DefaultValueStyle   = null;
        
        public GUIStyle  DynamicTitleStyle   = null;
        public GUIStyle  DynamicSubTitleStyle= null;
        public GUIStyle  DynamicLabelStyle   = null;
        public GUIStyle  DynamicValueStyle   = null;

        // ======================================================================
        // FUNCTIONS
        // ----------------------------------------------------------------------
        /// Initializes the iCanScript Layout objects.
        static iCS_Layout() {
            Init();
        }
        // ----------------------------------------------------------------------
        /// Initializes the iCanScript Layout objects.
        public static void Init() {
            // Build default GUI Styles.
            InitTitleStyle();
            InitSubTitleStyle();
            InitLabelStyle();
            InitValueStyle();
        }
        // ----------------------------------------------------------------------
        /// Refreshes the dynamic layout objects.
        public void Refresh(float scale= 1f) {
            // Build default GUI Styles.
            RefreshLabelStyle(scale);
            RefreshTitleStyle(scale);
            RefreshValueStyle(scale);
            RefreshSubTitleStyle(scale);
        }
        
        // ======================================================================
        // Size utility
        // ----------------------------------------------------------------------
        public static Vector2 DefaultLabelSize(string str) {
            return DefaultLabelStyle.CalcSize(new GUIContent(str));
        }
        
        // ======================================================================
        // Build and update GUI styles
        // ----------------------------------------------------------------------
        /// Initializes the default _'node'_ title GUI style.
        public static void InitTitleStyle() {
            // Allocate on first initialization.
            if(DefaultTitleStyle == null) {
                DefaultTitleStyle= new GUIStyle();
            }
            // Initialise all attribute of the style.
            Color titleColor= Prefs.NodeTitleColor;
            DefaultTitleStyle.normal.textColor= titleColor;
            DefaultTitleStyle.hover.textColor= titleColor;
            DefaultTitleStyle.focused.textColor= titleColor;
            DefaultTitleStyle.active.textColor= titleColor;
            DefaultTitleStyle.onNormal.textColor= titleColor;
            DefaultTitleStyle.onHover.textColor= titleColor;
            DefaultTitleStyle.onFocused.textColor= titleColor;
            DefaultTitleStyle.onActive.textColor= titleColor;
            DefaultTitleStyle.fontStyle= FontStyle.Bold;
            DefaultTitleStyle.fontSize= kTitleFontSize;
        }
        // ----------------------------------------------------------------------
        /// Refreshes the dynamic _'node'_ title GUI style.
        public void RefreshTitleStyle(float scale) {
            // Allocate on first initialization.
            if(DynamicTitleStyle == null) {
                DynamicTitleStyle= new GUIStyle();
            }
            // Refresh color attributes.
            Color titleColor= Prefs.NodeTitleColor;
            if(titleColor != DynamicTitleStyle.normal.textColor) {
                DynamicTitleStyle.normal.textColor= titleColor;
                DynamicTitleStyle.hover.textColor= titleColor;
                DynamicTitleStyle.focused.textColor= titleColor;
                DynamicTitleStyle.active.textColor= titleColor;
                DynamicTitleStyle.onNormal.textColor= titleColor;
                DynamicTitleStyle.onHover.textColor= titleColor;
                DynamicTitleStyle.onFocused.textColor= titleColor;
                DynamicTitleStyle.onActive.textColor= titleColor;                
            }
            // Refresh font style.
            DynamicTitleStyle.fontStyle= FontStyle.Bold;
            // Refresh font size.
            DynamicTitleStyle.fontSize= (int)(kTitleFontSize*scale);
        }
        // ----------------------------------------------------------------------
        /// Initializes the default _'node'_ sub-title GUI style.
        public static void InitSubTitleStyle() {
            // Allocate on first initialization.
            if(DefaultSubTitleStyle == null) {
                DefaultSubTitleStyle= new GUIStyle(DefaultTitleStyle);
            }
            // Initialise all attribute of the style.
            Color subTitleColor= Prefs.NodeTitleColor;
            subTitleColor.a= 0.7f;
            DefaultSubTitleStyle.normal.textColor   = subTitleColor;
            DefaultSubTitleStyle.hover.textColor    = subTitleColor;
            DefaultSubTitleStyle.focused.textColor  = subTitleColor;
            DefaultSubTitleStyle.active.textColor   = subTitleColor;
            DefaultSubTitleStyle.onNormal.textColor = subTitleColor;
            DefaultSubTitleStyle.onHover.textColor  = subTitleColor;
            DefaultSubTitleStyle.onFocused.textColor= subTitleColor;
            DefaultSubTitleStyle.onActive.textColor = subTitleColor;
            DefaultSubTitleStyle.fontStyle= FontStyle.Italic;
            DefaultSubTitleStyle.fontSize= (int)(0.8f*kTitleFontSize);
        }
        // ----------------------------------------------------------------------
        /// Refreshes the dynamic _'node'_ sub-title GUI style.
        public void RefreshSubTitleStyle(float scale) {
            // Allocate on first initialization.
            if(DynamicSubTitleStyle == null) {
                DynamicSubTitleStyle= new GUIStyle(DynamicTitleStyle);
            }
            // Refresh color attributes.
            Color subTitleColor= Prefs.NodeTitleColor;
            subTitleColor.a= 0.7f;
            if(subTitleColor != DynamicSubTitleStyle.normal.textColor) {
                DynamicSubTitleStyle.normal.textColor   = subTitleColor;
                DynamicSubTitleStyle.hover.textColor    = subTitleColor;
                DynamicSubTitleStyle.focused.textColor  = subTitleColor;
                DynamicSubTitleStyle.active.textColor   = subTitleColor;
                DynamicSubTitleStyle.onNormal.textColor = subTitleColor;
                DynamicSubTitleStyle.onHover.textColor  = subTitleColor;
                DynamicSubTitleStyle.onFocused.textColor= subTitleColor;
                DynamicSubTitleStyle.onActive.textColor = subTitleColor;                
            }
            DynamicSubTitleStyle.fontStyle= FontStyle.Italic;
            DynamicSubTitleStyle.fontSize= (int)(scale*0.8f*kTitleFontSize);
        }
        // ----------------------------------------------------------------------
        /// Initializes the default _'port'_ label GUI style.
        public static void InitLabelStyle() {
            // Allocate on first initialization.
            if(DefaultLabelStyle == null) {
                DefaultLabelStyle= new GUIStyle();
            }
            // Initialise all attribute of the style.
            Color labelColor= Prefs.NodeLabelColor;
            DefaultLabelStyle.normal.textColor= labelColor;
            DefaultLabelStyle.hover.textColor= labelColor;
            DefaultLabelStyle.focused.textColor= labelColor;
            DefaultLabelStyle.active.textColor= labelColor;
            DefaultLabelStyle.onNormal.textColor= labelColor;
            DefaultLabelStyle.onHover.textColor= labelColor;
            DefaultLabelStyle.onFocused.textColor= labelColor;
            DefaultLabelStyle.onActive.textColor= labelColor;
            DefaultLabelStyle.fontStyle= FontStyle.Bold;
            DefaultLabelStyle.fontSize= (int)(kLabelFontSize);
        }
        // ----------------------------------------------------------------------
        /// Refreshed the dynamic _'port'_ label GUI style.
        public void RefreshLabelStyle(float scale) {
            // Allocate on first initialization.
            if(DynamicLabelStyle == null) {
                DynamicLabelStyle= new GUIStyle(DefaultLabelStyle);
            }
            // Refresh color attributes.
            Color labelColor= Prefs.NodeLabelColor;
            if(labelColor != DynamicLabelStyle.normal.textColor) {
                DynamicLabelStyle.normal.textColor= labelColor;
                DynamicLabelStyle.hover.textColor= labelColor;
                DynamicLabelStyle.focused.textColor= labelColor;
                DynamicLabelStyle.active.textColor= labelColor;
                DynamicLabelStyle.onNormal.textColor= labelColor;
                DynamicLabelStyle.onHover.textColor= labelColor;
                DynamicLabelStyle.onFocused.textColor= labelColor;
                DynamicLabelStyle.onActive.textColor= labelColor;                
            }
            // Refresh font style.
            DynamicLabelStyle.fontStyle= FontStyle.Bold;
            // Refresh font size.
            DynamicLabelStyle.fontSize= (int)(kLabelFontSize*scale);
        }
        // ----------------------------------------------------------------------
        /// Initializes the default _'port'_ value GUI style.
        public static void InitValueStyle() {
            // Allocate on first initialization.
            if(DefaultValueStyle == null) {
                DefaultValueStyle= new GUIStyle();
            }
            // Initialise all attribute of the style.
            Color valueColor= Prefs.NodeValueColor;
            DefaultValueStyle.normal.textColor= valueColor;
            DefaultValueStyle.hover.textColor= valueColor;
            DefaultValueStyle.focused.textColor= valueColor;
            DefaultValueStyle.active.textColor= valueColor;
            DefaultValueStyle.onNormal.textColor= valueColor;
            DefaultValueStyle.onHover.textColor= valueColor;
            DefaultValueStyle.onFocused.textColor= valueColor;
            DefaultValueStyle.onActive.textColor= valueColor;
            DefaultValueStyle.fontStyle= FontStyle.Bold;
            DefaultValueStyle.fontSize= (int)(kLabelFontSize);
        }
        // ----------------------------------------------------------------------
        /// Initializes the default _'port'_ value GUI style.
        public void RefreshValueStyle(float scale) {
            // Allocate on first initialization.
            if(DynamicValueStyle == null) {
                DynamicValueStyle= new GUIStyle();
            }
            // Refresh color attributes.
            Color valueColor= Prefs.NodeValueColor;
            if(valueColor != DynamicValueStyle.normal.textColor) {
                DynamicValueStyle.normal.textColor= valueColor;
                DynamicValueStyle.hover.textColor= valueColor;
                DynamicValueStyle.focused.textColor= valueColor;
                DynamicValueStyle.active.textColor= valueColor;
                DynamicValueStyle.onNormal.textColor= valueColor;
                DynamicValueStyle.onHover.textColor= valueColor;
                DynamicValueStyle.onFocused.textColor= valueColor;
                DynamicValueStyle.onActive.textColor= valueColor;                
            }
            // Refresh font style.
            DynamicValueStyle.fontStyle= FontStyle.Bold;
            // Refresh font size.
            DynamicValueStyle.fontSize= (int)(kLabelFontSize*scale);
        }
        
    }
    
}
