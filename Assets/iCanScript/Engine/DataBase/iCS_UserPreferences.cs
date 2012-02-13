using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class iCS_UserPreferences {
    [System.Serializable]
    public class UserControlOptions {
        public float AnimationTime= 0.35f;
        public float ScrollSpeed  = 3f;
		public bool  InverseZoom  = false;
    }
    public UserControlOptions ControlOptions= new UserControlOptions();

    [System.Serializable]
    public class UserDisplayOptions {
        public bool     EditorPortValues= true;
        public bool     PlayingPortValues= true;
    }
    public UserDisplayOptions DisplayOptions= new UserDisplayOptions();

    [System.Serializable]
    public class UserBackgroundGrid {
        public Color    BackgroundColor= new Color(0.169f,0.188f,0.243f,1.0f);
        public Color    GridColor= new Color(0.25f,0.25f,0.25f,1.0f);
        public float    GridSpacing= 20.0f;
    }
    public UserBackgroundGrid   Grid= new UserBackgroundGrid();

    [System.Serializable]
    public class UserNodeColors {
        public float    SelectedBrightness= 1.75f;
        public Color    TitleColor        = Color.black;
        public Color    LabelColor        = Color.white;
        public Color    ValueColor        = new Color(1f,0.8f,0.4f);
        public Color    EntryStateColor   = new Color(1f,0.5f,0.25f);
        public Color    StateColor        = Color.cyan;
        public Color    ModuleColor       = Color.yellow;
        public Color    ClassColor        = Color.yellow;
        public Color    ConstructorColor  = new Color(1f,0.25f,0.5f);
        public Color    FunctionColor     = Color.green;
        public Color    SelectedColor     = Color.white;            
    }
    public UserNodeColors   NodeColors= new UserNodeColors();

    [System.Serializable]
    public class UserTypeColors {
        [System.Serializable]
        public class UserTypeColor {
            public string  TypeName;
            public Color   TypeColor;
            public UserTypeColor(Type type, Color color) {
                TypeName= type.Name;
                TypeColor= color;
            }
        }
        public UserTypeColor    BoolType      = new UserTypeColor(typeof(bool),       Color.red);
        public UserTypeColor    IntType       = new UserTypeColor(typeof(int),        Color.magenta);
        public UserTypeColor    FloatType     = new UserTypeColor(typeof(float),      Color.cyan);
        public UserTypeColor    Vector2Type   = new UserTypeColor(typeof(Vector2),    Color.yellow);
        public UserTypeColor    Vector3Type   = new UserTypeColor(typeof(Vector3),    Color.green);
        public UserTypeColor    Vector4Type   = new UserTypeColor(typeof(Vector4),    Color.blue);
        public UserTypeColor    StringType    = new UserTypeColor(typeof(string),     Color.red);
        public UserTypeColor    GameObjectType= new UserTypeColor(typeof(GameObject), Color.blue);
        public UserTypeColor[]  CustomColors  = new UserTypeColor[0];

        public Color GetColor(Type t) {
            if(t == null) return Color.white;
            string typeName= t.HasElementType ? t.GetElementType().Name : t.Name;
            if(typeName == BoolType.TypeName)       return BoolType.TypeColor;
            if(typeName == IntType.TypeName)        return IntType.TypeColor;
            if(typeName == FloatType.TypeName)      return FloatType.TypeColor;
            if(typeName == Vector2Type.TypeName)    return Vector2Type.TypeColor;
            if(typeName == Vector3Type.TypeName)    return Vector3Type.TypeColor;
            if(typeName == Vector4Type.TypeName)    return Vector4Type.TypeColor;
            if(typeName == StringType.TypeName)     return StringType.TypeColor;
            if(typeName == GameObjectType.TypeName) return GameObjectType.TypeColor;
            foreach(var tc in CustomColors) {
                if(typeName == tc.TypeName) return tc.TypeColor;
            }
            return Color.white;
        }
    }
    public UserTypeColors   TypeColors= new UserTypeColors();

    [System.Serializable]
    public class UserHiddenPrefixes {
        public const string uCodePrefix= iCS_EditorConfig.TypePrefix;
        public string[]     CustomPrefixes= new string[0]; 

        public string GetTypeName(Type t)  { return GetName(t.Name); }
        public string GetName(string name) {
            int prefixLen= uCodePrefix.Length;
            int nameLen= name.Length;
            if(nameLen > prefixLen && name.Substring(0, prefixLen) == uCodePrefix) return name.Substring(prefixLen, nameLen-prefixLen);
            foreach(var prefix in CustomPrefixes) {
                prefixLen= prefix.Length;
                if(nameLen > prefixLen && name.Substring(0, prefixLen) == prefix) return name.Substring(prefixLen, name.Length-prefixLen);                
            }
            return name;
        }
    }
    public UserHiddenPrefixes   HiddenPrefixes= new UserHiddenPrefixes();

    [System.Serializable]
    public class UserIcons {
        public bool         EnableMinimizedIcons= true;
        public const string uCodeIconPath= iCS_EditorConfig.GuiAssetPath;
        public string[]     CustomIconPaths= new string[0];
    }
    public UserIcons     Icons= new UserIcons();
    
    [System.Serializable]
    public class UserOnCreateClassModule {
        public bool OutputInstanceVariables = true;
        public bool OutputClassVariables    = false;
        public bool OutputInstanceProperties= false;
        public bool OutputClassProperties   = false;
        public bool InputInstanceVariables  = false;
        public bool InputClassVariables     = false;
        public bool InputInstanceProperties = false;
        public bool InputClassProperties    = false;
    }
    public UserOnCreateClassModule    OnCreateClassModule= new UserOnCreateClassModule();

}
