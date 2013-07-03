using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public static class iCS_SystemEvents {
    // ======================================================================
    // Callbacks
    // ----------------------------------------------------------------------
    public static Action    OnEditorStarted   = null;
    public static Action    OnSceneChanged    = null;
    public static Action    OnProjectChanged  = null;
    public static Action    OnCompileStarted  = null;
    public static Action    OnCompileCompleted= null;
    public static Action    OnEngineStarted   = null;
    public static Action    OnEngineStopped   = null;
    public static Action    OnEnginePaused    = null;
    
    // ======================================================================
    // State fields
    // ----------------------------------------------------------------------
    static bool    myIsUpdateSeen= false;
    static string  myCurrentScene= null;
    static bool    myIsCompiling = false;
    static bool    myIsPlaying   = false;
    static bool    myIsPaused    = false;
    
    // ======================================================================
    // Persistant States
    // ----------------------------------------------------------------------
    public static bool PersistantIsCompiling {
        get { return EditorPrefs.GetBool("iCS_IsCompilingState", false); }
        set { EditorPrefs.SetBool("iCS_IsCompilingState", value); }        
    }

    // ======================================================================
    // Trap Unity events
    // ----------------------------------------------------------------------
    static iCS_SystemEvents() {
        // Install callbacks
        EditorApplication.hierarchyWindowChanged+= HierarchyChanged;
        EditorApplication.projectWindowChanged+= ProjectChanged;
        EditorApplication.playmodeStateChanged+= PlaymodeChanged;
        EditorApplication.searchChanged+= SearchChanged;
        EditorApplication.update+= Update;
        // Initial state values.
        myCurrentScene= EditorApplication.currentScene;
        myIsCompiling = PersistantIsCompiling;
        CheckCompileState();
    }
    // ----------------------------------------------------------------------
    // Used to start the system events services.
    public static void Start() {}
    
    // ----------------------------------------------------------------------
    // This method attempts to determine what has changed in the hierarchy.
    static void HierarchyChanged() {
        Debug.Log("iCanScript: hierarchy has changed");
        var visualScripts= Resources.FindObjectsOfTypeAll(typeof(iCS_VisualScript));
        Debug.Log("iCanScript: # of Visual Scripts: "+visualScripts.Length);
        foreach(var vs in visualScripts) {
            Debug.Log("GameObject with Visual Script: "+(vs as iCS_VisualScript).gameObject.name);
        }
        Invoke(OnSceneChanged);
    }
    // ----------------------------------------------------------------------
    // This method attempts to determine what has changed in the project.
    static void ProjectChanged() {
        Debug.Log("iCanScript: project has changed");
        Invoke(OnProjectChanged);
    }
    // ----------------------------------------------------------------------
    // This method attempts to determine what has changed in the playmode.
    static void PlaymodeChanged() {
        CheckEnginePlayingState();
        CheckEnginePausedState();
    }
    // ----------------------------------------------------------------------
    // This method attempts to determine what has changed in the search.
    static void SearchChanged() {
        Debug.Log("iCanScript: search has changed");        
    }
    // ----------------------------------------------------------------------
    // This method attempts to determine what has changed.
    static void Update() {
        // Detect if it's the first time update.
        if(!myIsUpdateSeen) {
            Debug.Log("iCanScript: Editor started");
            Invoke(OnEditorStarted);
            myIsUpdateSeen= true;
        }
        // Detect if scene has changed.
        CheckCurrentSceneChange();
        // Detect compiling
        CheckCompileState();
        // Detect engine activity state.
        CheckEnginePlayingState();
        CheckEnginePausedState();
    }
    // ----------------------------------------------------------------------
    static void CheckCompileState() {
        var compiling= EditorApplication.isCompiling;
        if(compiling != myIsCompiling) {
            if(compiling) {
                Debug.Log("iCanScript: compilation started.");
                Invoke(OnCompileStarted);
            } else {
                Debug.Log("iCanScript: compilation ended.");
                Invoke(OnCompileCompleted);
            }
            myIsCompiling= compiling;
            PersistantIsCompiling= myIsCompiling;
        }        
    }
    // ----------------------------------------------------------------------
    static void CheckCurrentSceneChange() {
        var currentScene= EditorApplication.currentScene;
        if(currentScene != myCurrentScene) {
            Debug.Log("iCanScript: scene has changed.");
            Invoke(OnSceneChanged);
            myCurrentScene= currentScene;
        }        
    }
    // ----------------------------------------------------------------------
    static void CheckEnginePlayingState() {
        var isPlaying= EditorApplication.isPlaying;
        if(isPlaying != myIsPlaying) {
            if(isPlaying) {
                Debug.Log("iCanScript: engine started.");
                Invoke(OnEngineStarted);
            } else {
                Debug.Log("iCanScript: engine stopped.");
                Invoke(OnEngineStopped);
            }
            myIsPlaying= isPlaying;
        }        
    }
    // ----------------------------------------------------------------------
    static void CheckEnginePausedState() {
        var isPaused = EditorApplication.isPaused;
        if(isPaused != myIsPaused) {
            if(isPaused) {
                Debug.Log("iCanScript: engine paused.");
                Invoke(OnEnginePaused);
            } else {
                Debug.Log("iCanScript: engine started.");
                Invoke(OnEngineStarted);
            }
            myIsPaused= isPaused;
        }        
    }

    // ======================================================================
    // Utilities
    // ----------------------------------------------------------------------
    static void Invoke(Action action) {
        if(action != null) {
            foreach(Action handler in action.GetInvocationList()) {
                try {
                    handler();
                }
                catch(Exception) {}
            }                       
        }                        
    }
}
