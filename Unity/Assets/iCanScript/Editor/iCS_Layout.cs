using UnityEngine;
using System.Collections;
using Prefs= iCS_PreferencesController;

namespace iCanScript.Editor {
    public static class iCS_Layout {
        // ======================================================================
        // CONSTANTS
        // ----------------------------------------------------------------------
        const int   kLabelFontSize= iCS_EditorConfig.kLabelFontSize;
        const int   kTitleFontSize= iCS_EditorConfig.kTitleFontSize;

        // ======================================================================
        // FIELDS
        // -------------------------------------------------------------------
        // These are all the GUI styles needed to layout a node
        public static GUIStyle  LabelStyle        = null;
        public static GUIStyle  TitleStyle        = null;
        public static GUIStyle  MessageTitleStyle = null;
        public static GUIStyle  ValueStyle        = null;
        public static GUIStyle  SubTitleStyle     = null;
        public static GUIStyle  UnscaledTitleStyle= null;

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
            InitLabelStyle();
            InitTitleStyle();
            InitMessageTitleStyle();
            InitValueStyle();
            InitSubTitleStyle();
            InitUnscaledTitleStyle();
        }
        // ----------------------------------------------------------------------
        /// Adjusts all layout elements that depend on the visual scale.
        public static void AdjustForScale(float scale) {
            LabelStyle.fontSize       = (int)(kLabelFontSize*scale);
            TitleStyle.fontSize       = (int)(kTitleFontSize*scale);
            MessageTitleStyle.fontSize= (int)(kTitleFontSize*scale);
            ValueStyle.fontSize       = (int)(kLabelFontSize*scale);
            SubTitleStyle.fontSize    = (int)(0.8f*kTitleFontSize*scale);
        }
        
        // ======================================================================
        // Build and update GUI styles
        // ----------------------------------------------------------------------
        /// Initializes the _'port'_ label GUI style.
        public static void InitLabelStyle() {
            Color labelColor= Prefs.NodeLabelColor;
            // Allocate on first initialization.
            if(LabelStyle == null) {
                LabelStyle= new GUIStyle();
            }
            // Return if no change detected.
            else if(labelColor == LabelStyle.normal.textColor) {
                return;
            }
            // Initialise all attribute of the style.
            LabelStyle.normal.textColor= labelColor;
            LabelStyle.hover.textColor= labelColor;
            LabelStyle.focused.textColor= labelColor;
            LabelStyle.active.textColor= labelColor;
            LabelStyle.onNormal.textColor= labelColor;
            LabelStyle.onHover.textColor= labelColor;
            LabelStyle.onFocused.textColor= labelColor;
            LabelStyle.onActive.textColor= labelColor;
            LabelStyle.fontStyle= FontStyle.Bold;
            LabelStyle.fontSize= kLabelFontSize;
        }
        // ----------------------------------------------------------------------
        /// Initializes the _'node'_ title GUI style.
        public static void InitTitleStyle() {
            Color titleColor= Prefs.NodeTitleColor;
            // Allocate on first initialization.
            if(TitleStyle == null) {
                TitleStyle= new GUIStyle();
            }
            // Return if no change detected.
            else if(titleColor == TitleStyle.normal.textColor) {
                return;
            }
            TitleStyle.normal.textColor= titleColor;
            TitleStyle.hover.textColor= titleColor;
            TitleStyle.focused.textColor= titleColor;
            TitleStyle.active.textColor= titleColor;
            TitleStyle.onNormal.textColor= titleColor;
            TitleStyle.onHover.textColor= titleColor;
            TitleStyle.onFocused.textColor= titleColor;
            TitleStyle.onActive.textColor= titleColor;
            TitleStyle.fontStyle= FontStyle.Bold;
            TitleStyle.fontSize= kTitleFontSize;
        }
        // ----------------------------------------------------------------------
        /// Initializes the _'node'_ title GUI style.
        public static void InitUnscaledTitleStyle() {
            Color titleColor= Prefs.NodeTitleColor;
            // Allocate on first initialization.
            if(UnscaledTitleStyle == null) {
                UnscaledTitleStyle= new GUIStyle();
            }
            // Return if no change detected.
            else if(titleColor == UnscaledTitleStyle.normal.textColor) {
                return;
            }
            UnscaledTitleStyle.normal.textColor= titleColor;
            UnscaledTitleStyle.hover.textColor= titleColor;
            UnscaledTitleStyle.focused.textColor= titleColor;
            UnscaledTitleStyle.active.textColor= titleColor;
            UnscaledTitleStyle.onNormal.textColor= titleColor;
            UnscaledTitleStyle.onHover.textColor= titleColor;
            UnscaledTitleStyle.onFocused.textColor= titleColor;
            UnscaledTitleStyle.onActive.textColor= titleColor;
            UnscaledTitleStyle.fontStyle= FontStyle.Bold;
            UnscaledTitleStyle.fontSize= kTitleFontSize;
        }
        // ----------------------------------------------------------------------
        /// Initializes the event handler node title GUI style.
        public static void InitMessageTitleStyle() {
            Color messageTitleColor= Color.red;
            // Allocate on first initialization.
            if(MessageTitleStyle == null) {
                MessageTitleStyle= new GUIStyle(TitleStyle);
            }
            MessageTitleStyle.normal.textColor= messageTitleColor;
        }
        // ----------------------------------------------------------------------
        /// Initializes the _'port'_ value GUI style.
        public static void InitValueStyle() {
            Color valueColor= Prefs.NodeValueColor;
            // Allocate on first initialization.
            if(ValueStyle == null) {
                ValueStyle= new GUIStyle();
            }
            // Return if no change detected.
            else if(valueColor == ValueStyle.normal.textColor) {
                return;
            }
            ValueStyle.normal.textColor= valueColor;
            ValueStyle.hover.textColor= valueColor;
            ValueStyle.focused.textColor= valueColor;
            ValueStyle.active.textColor= valueColor;
            ValueStyle.onNormal.textColor= valueColor;
            ValueStyle.onHover.textColor= valueColor;
            ValueStyle.onFocused.textColor= valueColor;
            ValueStyle.onActive.textColor= valueColor;
            ValueStyle.fontStyle= FontStyle.Bold;
            ValueStyle.fontSize= 11;
        }
        // ----------------------------------------------------------------------
        /// Initializes the _'node'_ sub-title GUI style.
        public static void InitSubTitleStyle() {
//            Color c= Prefs.NodeTitleColor;
//            Color subTitleColor= new Color(Mathf.Abs(c.r-0.2f),
//                                           Mathf.Abs(c.g-0.2f),
//                                           Mathf.Abs(c.b-0.2f));
            Color subTitleColor= Prefs.NodeTitleColor;
            subTitleColor.a= 0.7f;
            // Allocate on first initialization.
            if(SubTitleStyle == null) {
                SubTitleStyle= new GUIStyle(TitleStyle);
            }
            // Return if no change detected.
            else if(subTitleColor == SubTitleStyle.normal.textColor) {
                return;
            }
            SubTitleStyle.normal.textColor   = subTitleColor;
            SubTitleStyle.hover.textColor    = subTitleColor;
            SubTitleStyle.focused.textColor  = subTitleColor;
            SubTitleStyle.active.textColor   = subTitleColor;
            SubTitleStyle.onNormal.textColor = subTitleColor;
            SubTitleStyle.onHover.textColor  = subTitleColor;
            SubTitleStyle.onFocused.textColor= subTitleColor;
            SubTitleStyle.onActive.textColor = subTitleColor;
            SubTitleStyle.fontStyle= FontStyle.Italic;
            SubTitleStyle.fontSize= (int)(0.8f*kTitleFontSize);
        }
        
    }
    
}
