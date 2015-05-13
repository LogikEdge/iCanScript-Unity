using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    using ActionVisualScript= System.Action<iCS_IStorage>;
    using ActionEditorObject= System.Action<iCS_EditorObject>;
    
    public static class SystemEvents {
        // ======================================================================
        // Callbacks
        // ----------------------------------------------------------------------
        // Events related to Unity operating states.
        public static Action    OnEditorStarted         = null;   ///< Event: Editor has started
        public static Action    OnSceneChanged          = null;   ///< Event: Scene has changed
        public static Action    OnHierarchyChanged      = null;   ///< Event: Hierarchy has changed
        public static Action    OnProjectChanged        = null;   ///< Event: Project has changed
        public static Action    OnCompileStarted        = null;   ///< Event: Start compiling
        public static Action    OnCompileCompleted      = null;   ///< Event: Compile completed
        public static Action    OnEngineStarted         = null;   ///< Event: Engine starting
        public static Action    OnEngineStopped         = null;   ///< Event: Engine stopping
        public static Action    OnEnginePaused          = null;   ///< Event: Engine paused
        public static Action    OnWindowUnderMouseChange= null;   ///< Event: Mouse is hovering a new window
        // Events related to visual script edition.
        public static ActionVisualScript OnVisualScriptSaved               = null;  ///< Event: Changes to the Visual Script where saved
        public static ActionVisualScript OnVisualScriptReloaded            = null;  ///< Event: Visual Script was reloaded
        public static ActionVisualScript OnVisualScriptUndo                = null;  ///< Event: Undo performed on Visual Script
        public static ActionEditorObject OnVisualScriptElementAdded        = null;  ///< Event: Element added to visual script
        public static ActionEditorObject OnVisualScriptElementWillBeRemoved= null;  ///< Event: Element removed from visual script
        public static ActionEditorObject OnVisualScriptElementNameChanged  = null;  ///< Event: Visual script element name change
        // Events related to scene changes.
        public static Action    OnSceneVisualScriptListChange= null;    ///< Event: Additon or removal of a visual script in scene
        
        // ======================================================================
        // State fields
        // ----------------------------------------------------------------------
        static bool         myIsUpdateSeen    = false;
        static string       myCurrentScene    = null;
        static bool         myIsCompiling     = false;
        static bool         myIsPlaying       = false;
        static bool         myIsPaused        = false;
    	static EditorWindow myWindowUnderMouse= null;
        
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
        static SystemEvents() {
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
        // Used to start & shutdown the system events services.
        public static void Start() {}
        public static void Shutdown() {}
        
        // ======================================================================
        // Event Announcements
        // ----------------------------------------------------------------------
        public static void AnnouceVisualScriptSaved(iCS_IStorage iStorage) {
            Invoke(OnVisualScriptSaved, iStorage);
        }
        public static void AnnouceVisualScriptReloaded(iCS_IStorage iStorage) {
            Invoke(OnVisualScriptReloaded, iStorage);
        }
        public static void AnnouceVisualScriptUndo(iCS_IStorage iStorage) {
            Invoke(OnVisualScriptUndo, iStorage);
        }
        public static void AnnounceVisualScriptElementAdded(iCS_EditorObject element) {
            Invoke(OnVisualScriptElementAdded, element);
        }
        public static void AnnounceVisualScriptElementWillBeRemoved(iCS_EditorObject element) {
            Invoke(OnVisualScriptElementWillBeRemoved, element);
        }
        public static void AnnounceVisualScriptElementNameChanged(iCS_EditorObject element) {
            Invoke(OnVisualScriptElementNameChanged, element);
        }
        public static void AnnounceSceneVisualScriptListChange() {
            Invoke(OnSceneVisualScriptListChange);
        }
        
        // ======================================================================
        // Process Unity events
        // ----------------------------------------------------------------------
        // This method attempts to determine what has changed in the hierarchy.
        static void HierarchyChanged() {
    //        Debug.Log("iCanScript: hierarchy has changed");
            Invoke(OnHierarchyChanged);
        }
        // ----------------------------------------------------------------------
        // This method attempts to determine what has changed in the project.
        static void ProjectChanged() {
    //        Debug.Log("iCanScript: project has changed");
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
    //        Debug.Log("iCanScript: search has changed");        
        }
    	
        // ----------------------------------------------------------------------
        // This method attempts to determine what has changed.
        static void Update() {
            // Detect if it's the first time update.
            if(!myIsUpdateSeen) {
    //            Debug.Log("iCanScript: Editor started");
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
    		// Detect for window under mouse changed.
    		CheckWindowUnderMouse();
        }
    	
        // ----------------------------------------------------------------------
        static void CheckWindowUnderMouse() {
    		// If current moused over window changes, Update help string and display
    		var windowUnderMouse= EditorWindow.mouseOverWindow;
    		if(windowUnderMouse !=  myWindowUnderMouse) {
    			myWindowUnderMouse= windowUnderMouse;
                Invoke(OnWindowUnderMouseChange);
    		}	
        }
    	
        // ----------------------------------------------------------------------
        static void CheckCompileState() {
            var compiling= EditorApplication.isCompiling;
            if(compiling != myIsCompiling) {
                if(compiling) {
    //                Debug.Log("iCanScript: compilation started.");
                    Invoke(OnCompileStarted);
                } else {
    //                Debug.Log("iCanScript: compilation ended.");
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
    //            Debug.Log("iCanScript: scene has changed. New path is: "+currentScene+"; New GUID is: "+AssetDatabase.AssetPathToGUID(currentScene));
                Invoke(OnSceneChanged);
                myCurrentScene= currentScene;
            }        
        }
        // ----------------------------------------------------------------------
        static void CheckEnginePlayingState() {
            var isPlaying= EditorApplication.isPlaying;
            if(isPlaying != myIsPlaying) {
                if(isPlaying) {
    //                Debug.Log("iCanScript: engine started.");
                    Invoke(OnEngineStarted);
                } else {
    //                Debug.Log("iCanScript: engine stopped.");
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
    //                Debug.Log("iCanScript: engine paused.");
                    Invoke(OnEnginePaused);
                } else {
    //                Debug.Log("iCanScript: engine started.");
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
        // ----------------------------------------------------------------------
        static void Invoke<T>(Action<T> action, T p) {
            if(action != null) {
                foreach(Action<T> handler in action.GetInvocationList()) {
                    try {
                        handler(p);
                    }
                    catch(Exception) {}
                }                       
            }                        
        }
        // ----------------------------------------------------------------------
        static void Invoke<T1,T2>(Action<T1,T2> action, T1 p1, T2 p2) {
            if(action != null) {
                foreach(Action<T1,T2> handler in action.GetInvocationList()) {
                    try {
                        handler(p1, p2);
                    }
                    catch(Exception) {}
                }                       
            }                        
        }
    }

}