using UnityEngine;
using System.Collections.Generic;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Engine {

    [AddComponentMenu("")]
    public class iCS_MonoBehaviourImp : MonoBehaviour, iCS_IVisualScriptData {
        // ======================================================================
        // Storage
        // ----------------------------------------------------------------------
        [HideInInspector] public bool                     IsEditorScript        = false;
        [HideInInspector] public string                   CSharpFileName        = null;
        [HideInInspector] public bool                     BaseTypeOverride      = false;
        [HideInInspector] public string                   BaseType              = "UnityEngine.MonoBehaviour";
        [HideInInspector] public string                   SourceFileGUID        = null;
        [HideInInspector] public int			          MajorVersion          = iCS_Config.MajorVersion;
        [HideInInspector] public int    		          MinorVersion          = iCS_Config.MinorVersion;
        [HideInInspector] public int    		          BugFixVersion         = iCS_Config.BugFixVersion;
        [HideInInspector] public int                      DisplayRoot           = -1;	
    	[HideInInspector] public int    		          SelectedObject        = -1;
        [HideInInspector] public Vector2                  SelectedObjectPosition= Vector2.zero;
    	[HideInInspector] public bool                     ShowDisplayRootNode   = true;
    	[HideInInspector] public float  		          GuiScale              = 1f;	
    	[HideInInspector] public Vector2		          ScrollPosition        = Vector2.zero;
        [HideInInspector] public int                      UndoRedoId            = 0;
        [HideInInspector] public List<iCS_EngineObject>   EngineObjects         = new List<iCS_EngineObject>();
        [HideInInspector] public iCS_NavigationHistory    NavigationHistory     = new iCS_NavigationHistory();
        [HideInInspector] public iCS_EngineObject         EngineObject          = null;

        // ======================================================================
        // Visual Script Data Interface Implementation
        // ----------------------------------------------------------------------
        bool iCS_IVisualScriptData.IsEditorScript {
            get { return IsEditorScript; }
            set { IsEditorScript= value; }
        }
        string iCS_IVisualScriptData.CSharpFileName {
            get { return CSharpFileName; }
            set { CSharpFileName= value; }
        }
        bool iCS_IVisualScriptData.BaseTypeOverride {
            get { return BaseTypeOverride; }
            set { BaseTypeOverride= value; }
        }
        string iCS_IVisualScriptData.BaseType {
            get { return BaseType; }
            set { BaseType= value; }
        }
        string iCS_IVisualScriptData.SourceFileGUID {
            get { return SourceFileGUID; }
            set { SourceFileGUID= value; }
        }
        int iCS_IVisualScriptData.MajorVersion {
            get { return MajorVersion; }
            set { MajorVersion= value; }
        }
        int iCS_IVisualScriptData.MinorVersion {
            get { return MinorVersion; }
            set { MinorVersion= value; }
        }
        int iCS_IVisualScriptData.BugFixVersion {
            get { return BugFixVersion; }
            set { BugFixVersion= value; }
        }
        List<iCS_EngineObject>  iCS_IVisualScriptData.EngineObjects {
            get { return EngineObjects; }
        }
        int iCS_IVisualScriptData.UndoRedoId {
            get { return UndoRedoId; }
            set { UndoRedoId= value; }
        }
    
    
        // ======================================================================
        // Visual Script Editor Data Interface Implementation
        // ----------------------------------------------------------------------
        int iCS_IVisualScriptData.DisplayRoot {
            get { return DisplayRoot; }
            set { DisplayRoot= value; }
        }
        int iCS_IVisualScriptData.SelectedObject {
            get { return SelectedObject; }
            set { SelectedObject= value; }
        }
        Vector2 iCS_IVisualScriptData.SelectedObjectPosition {
            get { return SelectedObjectPosition; }
            set { SelectedObjectPosition= value; }
        }
        bool iCS_IVisualScriptData.ShowDisplayRootNode {
            get { return ShowDisplayRootNode; }
            set { ShowDisplayRootNode= value; }
        }
        float iCS_IVisualScriptData.GuiScale {
            get { return GuiScale; }
            set { GuiScale= value; }
        }
        Vector2 iCS_IVisualScriptData.ScrollPosition {
            get { return ScrollPosition; }
            set { ScrollPosition= value; }
        }
        iCS_NavigationHistory iCS_IVisualScriptData.NavigationHistory {
            get { return NavigationHistory; }
        }
    
     }
    
}
