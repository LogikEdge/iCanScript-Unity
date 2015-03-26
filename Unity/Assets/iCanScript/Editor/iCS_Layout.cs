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
        public static GUIStyle  LabelStyle       = null;
        public static GUIStyle  TitleStyle       = null;
        public static GUIStyle  MessageTitleStyle= null;
        public static GUIStyle  ValueStyle       = null;
        public static GUIStyle  SubTitleStyle    = null;

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
            LabelStyle       = null;
            TitleStyle       = null;
            MessageTitleStyle= null;
            ValueStyle       = null;
            SubTitleStyle    = null;
            InitLabelStyle();
            InitTitleStyle();
            InitMessageTitleStyle();
            InitValueStyle();
            InitSubTitleStyle();
        }
        // ----------------------------------------------------------------------
        public static void AdjustForScale(float scale) {
            LabelStyle.fontSize       = (int)(kLabelFontSize*scale);
            TitleStyle.fontSize       = (int)(kTitleFontSize*scale);
            MessageTitleStyle.fontSize= (int)(kTitleFontSize*scale);
            ValueStyle.fontSize       = (int)(kLabelFontSize*scale);
            SubTitleStyle.fontSize    = (int)(kTitleFontSize*0.8f*scale);
        }
        
        // ======================================================================
        // Build and update GUI styles
        // ----------------------------------------------------------------------
        /// Initializes the _'Label'_ style with its default values.
        public static void InitLabelStyle() {
            Color labelColor= Prefs.NodeLabelColor;
            if(LabelStyle == null) LabelStyle= new GUIStyle();
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
        public static void InitTitleStyle() {
            Color titleColor= Prefs.NodeTitleColor;
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
            TitleStyle.fontSize= kTitleFontSize;
        }
        // ----------------------------------------------------------------------
        public static void InitMessageTitleStyle() {
            Color messageTitleColor= Color.red;
            if(MessageTitleStyle == null) {
                MessageTitleStyle= new GUIStyle(TitleStyle);
            }
            MessageTitleStyle.normal.textColor= messageTitleColor;
        }
        // ----------------------------------------------------------------------
        public static void InitValueStyle() {
            Color valueColor= Prefs.NodeValueColor;
            if(ValueStyle == null) ValueStyle= new GUIStyle();
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
        public static void InitSubTitleStyle() {
            Color subTitleColor= Prefs.NodeTitleColor;
            subTitleColor.a= 0.75f;
            if(SubTitleStyle == null) {
                SubTitleStyle= new GUIStyle(TitleStyle);
            }
            SubTitleStyle.normal.textColor= subTitleColor;
            SubTitleStyle.fontStyle= FontStyle.Italic;
            SubTitleStyle.fontSize= (int)(kTitleFontSize*0.8f);
        }
        
    }
    
}
