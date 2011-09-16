using UnityEngine;
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
        public Color    StateColor= Color.cyan;
        public Color    ModuleColor= Color.yellow;
        public Color    FunctionColor= Color.green;
        public Color    SelectedColor= Color.white;            
    }
    public UserNodeColors   NodeColors= new UserNodeColors();
}
