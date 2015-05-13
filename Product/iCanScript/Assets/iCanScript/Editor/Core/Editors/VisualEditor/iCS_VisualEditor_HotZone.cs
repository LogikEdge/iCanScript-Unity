using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
    // =======================================================================
    // Types
    // -----------------------------------------------------------------------
    class HotZone {
        public Rect     Area;
        public Action   OnMouseClick= null;
        public Action   OnMouseOver = null;
        public Action   OnGUI       = null;
        public bool     IsForeground= false;
        
        public HotZone(Rect area, Action onGUI, Action onMouseClick, Action onMouseOver= null, bool isForground= false) {
            Area= area;
            OnGUI       = onGUI;
            OnMouseClick= onMouseClick;
            OnMouseOver = onMouseOver;
            IsForeground= isForground;
        }
    };

    // =======================================================================
    // Fields
    // -----------------------------------------------------------------------
    Dictionary<string,HotZone>   myHotZones= new Dictionary<string,HotZone>();
    
    // =======================================================================
    // HotZone Managememt
    // -----------------------------------------------------------------------
    void HotZoneAdd(string key, Rect area, Action onGUI, Action onMouseClick, Action onMouseOver= null, bool isForground= false) {
        myHotZones.Add(key, new HotZone(area, onGUI, onMouseClick, onMouseOver, isForground));
    }
    // -----------------------------------------------------------------------
    void HotZoneRemove(string key) {
        myHotZones.Remove(key);
    }
    
    // =======================================================================
    // HotZone Processing
    // -----------------------------------------------------------------------
    bool HotZoneMouseClick(Vector2 p, iCS_PickInfo pickInfo) {
        bool isUsed= false;
        HotZone[] hotZones= new HotZone[myHotZones.Values.Count];
        myHotZones.Values.CopyTo(hotZones, 0);
        foreach(var hz in hotZones) {
            if(hz.IsForeground == true || (pickInfo == null || pickInfo.PickedObject.IsBehaviour)) {
                if(hz.Area.Contains(p) && hz.OnMouseClick != null) {
                    hz.OnMouseClick();
                    isUsed= true;
                }
            }                
        }
        return isUsed;
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
}