using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // =======================================================================
    // Types
    // -----------------------------------------------------------------------
    class HotZone {
        public Rect     Area;
        public Action   OnMouseClick= null;
        public Action   OnMouseOver = null;
        public Action   OnGUI       = null;
        
        public HotZone(Rect area, Action onGUI, Action onMouseClick, Action onMouseOver= null) {
            Area= area;
            OnGUI       = onGUI;
            OnMouseClick= onMouseClick;
            OnMouseOver = onMouseOver;
        }
    };

    // =======================================================================
    // Fields
    // -----------------------------------------------------------------------
    List<HotZone>   myHotZones= new List<HotZone>();
    
    // =======================================================================
    // HotZone Managememt
    // -----------------------------------------------------------------------
    void HotZoneAdd(Rect area, Action onGUI, Action onMouseClick, Action onMouseOver= null) {
        myHotZones.Add(new HotZone(area, onGUI, onMouseClick, onMouseOver));
    }
    // -----------------------------------------------------------------------
    void HotZoneRemove(Rect area) {
        for(int i= 0; i < myHotZones.Count; ++i) {
            if(Math3D.IsEqual(myHotZones[i].Area, area)) {
                myHotZones.RemoveAt(i);
                return;
            }
        }
    }
    
    // =======================================================================
    // HotZone Processing
    // -----------------------------------------------------------------------
    void HotZoneMouseClick(Vector2 p) {
        foreach(var hz in myHotZones) {
            if(hz.Area.Contains(p) && hz.OnMouseClick != null) {
                hz.OnMouseClick();
            }
        }
    }
    // -----------------------------------------------------------------------    
    void HotZoneMouseOver(Vector2 p) {
        foreach(var hz in myHotZones) {
            if(hz.Area.Contains(p) && hz.OnMouseOver != null) {
                hz.OnMouseOver();
            }
        }        
    }
    // -----------------------------------------------------------------------    
    void HotZoneGUI() {
        foreach(var hz in myHotZones) {
            if(hz.OnGUI != null) {
                hz.OnGUI();
            }
        }                
    }
}
