using UnityEngine;

namespace iCanScript.Internal.Editor {
    
    public enum iCS_PickPartEnum { Unknown, EditorObject, Name, Value, FoldIcon, MinimizeIcon };
    public class iCS_PickInfo {
        public iCS_IStorage         IStorage= null;
        public iCS_EditorObject     PickedObject= null;
        public iCS_PickPartEnum     PickedPart= iCS_PickPartEnum.Unknown;
        public Vector2              PickedPoint= Vector2.zero;
        public Vector2              PickedPointInGUISpace= Vector2.zero;
        public Rect                 PickedPartGraphPosition= new Rect(0,0,0,0);
        public Rect                 PickedPartGUIPosition= new Rect(0,0,0,0);
        public iCS_PickInfo(iCS_IStorage iStorage) { IStorage= iStorage; }
    }

}
