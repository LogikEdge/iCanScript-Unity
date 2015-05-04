using UnityEngine;
using System;
using System.Collections;
using iCanScript.Engine;
using P=iCanScript.Prelude;

namespace iCanScript.Editor {
    
    // ======================================================================
    // This file installs predefined Unity3d classes and messages.
    //
    // You may augment the Unity library by invoking the functions:
    //      DecodeUnityClassInfo(...) and
    //      InstallUnityMessage(...)
    //
    // Please augment the Unity library using your own source file as
    // this source file may be changed in future releases.
    public static class iCS_UnityClasses {
        // ======================================================================
        // Constants
        // ----------------------------------------------------------------------
        const string kResourcesPath     = iCS_Config.ImagePath;
        const string kInMailIcon        = kResourcesPath+"/iCS_InMail_32x32.png";
    
        // ======================================================================
        // The following are helper functions to register Unity3D message handlers
        // ----------------------------------------------------------------------
        // Use this function to register a Unity message on the
        // MonoBehaviour class.
        //
        // This function can be called by the iCanScript user to add to the
        // existing Unity library.
        // 
        public static void InstallMonoBehaviourMessage(string messageName, iCS_Parameter[] parameters, 
                                               string iconPath= null, string description= null) {
    		var declaringType= typeof(MonoBehaviour);
    		var parameterTypes= P.map(p=> p.type, parameters);
    		var parametersNames= P.map(p=> p.name, parameters);
    		LibraryController.AddEventHandler(messageName, declaringType, parameterTypes, parametersNames);
        }
    
        
        // ======================================================================
        // The following is the list of preinstalled Unity classes.
        // ----------------------------------------------------------------------
        public static void PopulateDataBase() {                        
            // Install MonoBehaviour messages with no parameters.
            var noParameters= new iCS_Parameter[0];
            InstallMonoBehaviourMessage("Awake"               , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("Update"              , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("LateUpdate"          , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("FixedUpdate"         , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("Start"               , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("Reset"               , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnMouseEnter"        , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnMouseOver"         , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnMouseExit"         , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnMouseDown"         , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnMouseUp"           , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnMouseUpAsButton"   , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnMouseDrag"         , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnBecameVisible"     , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnBecameInvisible"   , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnEnable"            , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnDisable"           , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnDestroy"           , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnPreCull"           , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnPreRender"         , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnPostRender"        , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnRenderObject"      , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnWillRenderObject"  , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnGUI"               , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnDrawGizmosSelected", noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnDrawGizmos"        , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnServerInitialized" , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnConnectedToServer" , noParameters, kInMailIcon);         
            //InstallMonoBehaviourMessage("OnAnimatorMove"      , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnApplicationQuit"   , noParameters, kInMailIcon);         

            // Trigger messages
            var triggerParameters= new iCS_Parameter[1]{new iCS_Parameter("colliderInfo", typeof(Collider))};
            InstallMonoBehaviourMessage("OnTriggerEnter", triggerParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnTriggerExit" , triggerParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnTriggerStay" , triggerParameters, kInMailIcon);

            // Collision messages
            var collisionParameters= new iCS_Parameter[1]{new iCS_Parameter("collisionInfo", typeof(Collision))};
            InstallMonoBehaviourMessage("OnCollisionEnter", collisionParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnCollisionExit" , collisionParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnCollisionStay" , collisionParameters, kInMailIcon);

            var controllerColliderParams= new iCS_Parameter[1]{new iCS_Parameter("hit", typeof(ControllerColliderHit))};
            InstallMonoBehaviourMessage("OnControllerColliderHit", controllerColliderParams, kInMailIcon);

            var jointBreakParameters= new iCS_Parameter[1]{new iCS_Parameter("breakForce", typeof(float))};
            InstallMonoBehaviourMessage("OnJointBreak", jointBreakParameters, kInMailIcon);

            var gameObjectParameters= new iCS_Parameter[1]{new iCS_Parameter("gameObject", typeof(GameObject))};
            InstallMonoBehaviourMessage("ParticleCollision", gameObjectParameters, kInMailIcon);

            var levelLoadedParameters= new iCS_Parameter[1]{new iCS_Parameter("level", typeof(int), 0)};
            InstallMonoBehaviourMessage("OnLevelWasLoaded", levelLoadedParameters, kInMailIcon);

            var pauseParameters= new iCS_Parameter[1]{new iCS_Parameter("pause", typeof(bool), false)};
            InstallMonoBehaviourMessage("OnApplicationPause", pauseParameters, kInMailIcon);         

            var focusParameters= new iCS_Parameter[1]{new iCS_Parameter("focus", typeof(bool), false)};
            InstallMonoBehaviourMessage("OnApplicationFocus", focusParameters, kInMailIcon);         

            var playerParameters= new iCS_Parameter[1]{new iCS_Parameter("player", typeof(NetworkPlayer))};
            InstallMonoBehaviourMessage("OnPlayerConnected"   , playerParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnPlayerDisconnected", playerParameters, kInMailIcon);         

            var disconnectParameters= new iCS_Parameter[1]{new iCS_Parameter("mode", typeof(NetworkDisconnection))};
            InstallMonoBehaviourMessage("OnDisconnectedFromServer", disconnectParameters, kInMailIcon);             

            var networkErrorParameters= new iCS_Parameter[1]{new iCS_Parameter("error", typeof(NetworkConnectionError))};
            InstallMonoBehaviourMessage("OnFailedToConnect"              , networkErrorParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnFailedToConnectToMasterServer", networkErrorParameters, kInMailIcon);

            var msEventParameters= new iCS_Parameter[1]{new iCS_Parameter("msEvent", typeof(MasterServerEvent))};
            InstallMonoBehaviourMessage("OnMasterServerEvent", msEventParameters, kInMailIcon);

            var networkInfoParameters= new iCS_Parameter[1]{new iCS_Parameter("info", typeof(NetworkMessageInfo))};
            InstallMonoBehaviourMessage("OnNetworkInstantiate", networkInfoParameters, kInMailIcon);         

            var serializeViewParameters= new iCS_Parameter[2]{
                new iCS_Parameter("stream", typeof(BitStream)),
                new iCS_Parameter("info", typeof(NetworkMessageInfo))
            };
            InstallMonoBehaviourMessage("OnSerializeNetworkView", serializeViewParameters, kInMailIcon);         

            var animatorIKParameters= new iCS_Parameter[1]{new iCS_Parameter("layerIndex", typeof(int), 0)};
            InstallMonoBehaviourMessage("OnAnimatorIK", animatorIKParameters, kInMailIcon);         

            var renderImageParameters= new iCS_Parameter[2]{
                new iCS_Parameter("source", typeof(RenderTexture)),
                new iCS_Parameter("destination", typeof(RenderTexture))
            };
            InstallMonoBehaviourMessage("OnRenderImage", renderImageParameters, kInMailIcon);

            var audioFilterReadParameters= new iCS_Parameter[2]{
                new iCS_Parameter("data", typeof(float[])),
                new iCS_Parameter("channels", typeof(int))
            };
            InstallMonoBehaviourMessage("OnAudioFilterRead", audioFilterReadParameters, kInMailIcon);         
        }

    }

}
