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
    Dictionary<string,HotZone>   myHotZones= new Dictionary<string,HotZone>();
    
    // =======================================================================
    // HotZone Managememt
    // -----------------------------------------------------------------------
    void HotZoneAdd(string key, Rect area, Action onGUI, Action onMouseClick, Action onMouseOver= null) {
        myHotZones.Add(key, new HotZone(area, onGUI, onMouseClick, onMouseOver));
    }
    // -----------------------------------------------------------------------
    void HotZoneRemove(string key) {
        myHotZones.Remove(key);
    }
    
    // =======================================================================
    // HotZone Processing
    // -----------------------------------------------------------------------
    void HotZoneMouseClick(Vector2 p) {
        HotZone[] hotZones= new HotZone[myHotZones.Values.Count];
        myHotZones.Values.CopyTo(hotZones, 0);
        foreach(var hz in hotZones) {
            if(hz.Area.Contains(p) && hz.OnMouseClick != null) {
                hz.OnMouseClick();
            }
        }
    }
    // -----------------------------------------------------------------------    
    void HotZoneMouseOver(Vector2 p) {
        HotZone[] hotZones= new HotZone[myHotZones.Values.Count];
        myHotZones.Values.CopyTo(hotZones, 0);
        foreach(var hz in hotZones) {
            if(hz.Area.Contains(p) && hz.OnMouseOver != null) {
                hz.OnMouseOver();
            }
        }        
    }
    // -----------------------------------------------------------------------    
    void HotZoneGUI() {
        HotZone[] hotZones= new HotZone[myHotZones.Values.Count];
        myHotZones.Values.CopyTo(hotZones, 0);
        foreach(var hz in hotZones) {
            if(hz.OnGUI != null) {
                hz.OnGUI();
            }
        }                
    }
}
