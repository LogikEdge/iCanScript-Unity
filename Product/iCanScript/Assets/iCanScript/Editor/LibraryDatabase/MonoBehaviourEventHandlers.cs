using UnityEngine;
using System;
using System.Collections;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    // ======================================================================
    // This file installs predefined Unity event handlers.
    //
    // Please augment the Unity library using your own source file as
    // this source file may be changed in future releases.
    public static class MonoBehaviourEventHandlers {
        // ======================================================================
        // The following are helper functions to register Unity3D message handlers
        // ----------------------------------------------------------------------
        // Use this function to register a Unity message on the
        // MonoBehaviour class.
        //
        // This function can be called by the iCanScript user to add to the
        // existing Unity library.
        // 
        static void InstallMonoBehaviourMessage(string messageName, Type[] parameterTypes, string[] parameterNames) {
    		var declaringType= typeof(MonoBehaviour);
    		LibraryController.AddEventHandler(messageName, declaringType, parameterTypes, parameterNames);
        }
    
        
        // ======================================================================
        // The following is the list of preinstalled Unity classes.
        // ----------------------------------------------------------------------
        public static void Install() {                        
            // Install MonoBehaviour messages with no parameters.
            var noParameterTypes= new Type[0];
            var noParameterNames= new string[0];
            InstallMonoBehaviourMessage("Awake"               , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("Update"              , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("LateUpdate"          , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("FixedUpdate"         , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("Start"               , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("Reset"               , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnMouseEnter"        , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnMouseOver"         , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnMouseExit"         , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnMouseDown"         , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnMouseUp"           , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnMouseUpAsButton"   , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnMouseDrag"         , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnBecameVisible"     , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnBecameInvisible"   , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnEnable"            , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnDisable"           , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnDestroy"           , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnPreCull"           , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnPreRender"         , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnPostRender"        , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnRenderObject"      , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnWillRenderObject"  , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnGUI"               , noParameterTypes, noParameterNames);
            InstallMonoBehaviourMessage("OnDrawGizmosSelected", noParameterTypes, noParameterNames);         
            InstallMonoBehaviourMessage("OnDrawGizmos"        , noParameterTypes, noParameterNames);         
            InstallMonoBehaviourMessage("OnServerInitialized" , noParameterTypes, noParameterNames);         
            InstallMonoBehaviourMessage("OnConnectedToServer" , noParameterTypes, noParameterNames);         
            InstallMonoBehaviourMessage("OnAnimatorMove"      , noParameterTypes, noParameterNames);         
            InstallMonoBehaviourMessage("OnApplicationQuit"   , noParameterTypes, noParameterNames);         

            // Trigger messages
            var triggerParameterTypes= new Type[1]  {typeof(Collider)};
            var triggerParameterNames= new string[1]{"colliderInfo"};
            InstallMonoBehaviourMessage("OnTriggerEnter", triggerParameterTypes, triggerParameterNames);
            InstallMonoBehaviourMessage("OnTriggerExit" , triggerParameterTypes, triggerParameterNames);
            InstallMonoBehaviourMessage("OnTriggerStay" , triggerParameterTypes, triggerParameterNames);

            // Collision messages
            var collisionParameterTypes= new Type[1]  {typeof(Collision)};
            var collisionParameterNames= new string[1]{"collisionInfo"};
            InstallMonoBehaviourMessage("OnCollisionEnter", collisionParameterTypes, collisionParameterNames);
            InstallMonoBehaviourMessage("OnCollisionExit" , collisionParameterTypes, collisionParameterNames);
            InstallMonoBehaviourMessage("OnCollisionStay" , collisionParameterTypes, collisionParameterNames);

            var controllerColliderParamTypes= new Type[1]  {typeof(ControllerColliderHit)};
            var controllerColliderParamNames= new string[1]{"hit"};
            InstallMonoBehaviourMessage("OnControllerColliderHit", controllerColliderParamTypes, controllerColliderParamNames);

            var jointBreakParameterTypes= new Type[1]  {typeof(float)};
            var jointBreakParameterNames= new string[1]{"breakForce"};
            InstallMonoBehaviourMessage("OnJointBreak", jointBreakParameterTypes, jointBreakParameterNames);

            var gameObjectParameterTypes= new Type[1]  {typeof(GameObject)};
            var gameObjectParameterNames= new string[1]{"gameObject"};
            InstallMonoBehaviourMessage("ParticleCollision", gameObjectParameterTypes, gameObjectParameterNames);

            var levelLoadedParameterTypes= new Type[1]  {typeof(int)};
            var levelLoadedParameterNames= new string[1]{"level"};
            InstallMonoBehaviourMessage("OnLevelWasLoaded", levelLoadedParameterTypes, levelLoadedParameterNames);

            var pauseParameterTypes= new Type[1]  {typeof(bool)};
            var pauseParameterNames= new string[1]{"pause"};
            InstallMonoBehaviourMessage("OnApplicationPause", pauseParameterTypes, pauseParameterNames);         

            var focusParameterTypes= new Type[1]  {typeof(bool)};
            var focusParameterNames= new string[1]{"focus"};
            InstallMonoBehaviourMessage("OnApplicationFocus", focusParameterTypes, focusParameterNames);         

            var animatorIKParameterTypes= new Type[1]  {typeof(int)};
            var animatorIKParameterNames= new string[1]{"layerIndex"};
            InstallMonoBehaviourMessage("OnAnimatorIK", animatorIKParameterTypes, animatorIKParameterNames);         

            var renderImageParameterTypes= new Type[2]  {typeof(RenderTexture), typeof(RenderTexture)};
            var renderImageParameterNames= new string[2]{"source",              "destination"};
            InstallMonoBehaviourMessage("OnRenderImage", renderImageParameterTypes, renderImageParameterNames);

            var audioFilterReadParameterTypes= new Type[2]  {typeof(float[]),typeof(int)};
            var audioFilterReadParameterNames= new string[2]{"data",         "channels"};
            InstallMonoBehaviourMessage("OnAudioFilterRead", audioFilterReadParameterTypes, audioFilterReadParameterNames);         
        }

    }

}
