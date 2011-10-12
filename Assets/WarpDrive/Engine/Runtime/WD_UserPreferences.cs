using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class WD_UserPreferences {
    [System.Serializable]
    public class UserBackgroundGrid {
        public Color    BackgroundColor= new Color(0.169f,0.188f,0.243f,1.0f);
        public Color    GridColor= new Color(0.25f,0.25f,0.25f,1.0f);
        public float    GridSpacing= 20.0f;
    }
    public UserBackgroundGrid   Grid= new UserBackgroundGrid();

    [System.Serializable]
    public class UserNodeColors {
        public Color    StateColor   = Color.cyan;
        public Color    ModuleColor  = Color.yellow;
        public Color    ClassColor   = new Color(1.0f,0.5f,0.0f,1.0f);
        public Color    FunctionColor= Color.green;
        public Color    SelectedColor= Color.white;            
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
            if(t.Name == BoolType.TypeName)       return BoolType.TypeColor;
            if(t.Name == IntType.TypeName)        return IntType.TypeColor;
            if(t.Name == FloatType.TypeName)      return FloatType.TypeColor;
            if(t.Name == Vector2Type.TypeName)    return Vector2Type.TypeColor;
            if(t.Name == Vector3Type.TypeName)    return Vector3Type.TypeColor;
            if(t.Name == Vector4Type.TypeName)    return Vector4Type.TypeColor;
            if(t.Name == StringType.TypeName)     return StringType.TypeColor;
            if(t.Name == GameObjectType.TypeName) return GameObjectType.TypeColor;
            foreach(var tc in CustomColors) {
                if(t.Name == tc.TypeName) return tc.TypeColor;
            }
            return Color.white;
        }
    }
    public UserTypeColors   TypeColors= new UserTypeColors();

    [System.Serializable]
    public class UserHiddenPrefixes {
        public const string WarpDrivePrefix= WD_EditorConfig.TypePrefix;
        public string[]     CustomPrefixes= new string[0]; 

        public string GetTypeName(Type t)  { return GetName(t.Name); }
        public string GetName(string name) {
            int prefixLen= WarpDrivePrefix.Length;
            if(name.Substring(0, prefixLen) == WarpDrivePrefix) return name.Substring(prefixLen, name.Length-prefixLen);
            foreach(var prefix in CustomPrefixes) {
                prefixLen= prefix.Length;
                if(name.Substring(0, prefixLen) == prefix) return name.Substring(prefixLen, name.Length-prefixLen);                
            }
            return name;
        }
    }
    public UserHiddenPrefixes   HiddenPrefixes= new UserHiddenPrefixes();
}
