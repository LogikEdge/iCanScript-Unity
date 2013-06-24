using UnityEngine;
using System;
using System.Collections;

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
    const string kUnityEnginePackage= "UnityEngine";
    const string kResourcesPath     = iCS_Config.ResourcePath;
    const string kJoystickIcon      = kResourcesPath+"/iCS_JoystickIcon.psd";
    const string kUnityIcon         = kResourcesPath+"/iCS_UnityLogo_32x32.png";
    const string kOutMailIcon       = kResourcesPath+"/iCS_OutMail_32x32.png";
    const string kInMailIcon        = kResourcesPath+"/iCS_InMail_32x32.png";
    
    // ======================================================================
    // The following are helper functions to register Unity3D classes
    // ----------------------------------------------------------------------
    // Use this function to register Unity3d classes.
    // All public fields/properties and methods will be registered.
    //
    // This function can be called by the iCanScript user to add to the
    // existing Unity library.
    // 
    public static void DecodeUnityClassInfo(Type classType, string package= "UnityEngine", string iconPath= null, string description= null) {
        string                  company               = "Unity";
        bool                    decodeAllPublicMembers= true;
        if(package == null)     package               = kUnityEnginePackage;
        if(description == null) description           = "Unity class "+classType.Name;
        if(iconPath == null)    iconPath              = kUnityIcon;
        iCS_Reflection.DecodeClassInfo(classType, company, package, description, iconPath, decodeAllPublicMembers);
    }
    // ----------------------------------------------------------------------
    // Use this function to register a Unity message on a class.
    // Messages are invoked using dynamic (runtime) lookup and are not
    // present in .NET assembly.  For that reason, messages are not
    // automatically installed when decoding the Unity class. 
    //
    // This function can be called by the iCanScript user to add to the
    // existing Unity library.
    // 
    public static void InstallUnityMessage(Type classType, string messageName, iCS_StorageClass storageClass,
                                           iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                           string iconPath= null, string description= null) {
        if(iconPath == null) iconPath= kUnityIcon;
        if(description == null) description= "Event: "+messageName+" on "+classType.Name;
        iCS_LibraryDatabase.AddMessage(classType, messageName, storageClass, parameters, functionReturn, description, iconPath);
    }
    // ----------------------------------------------------------------------
    // Use this function to register a Unity message on the
    // MonoBehaviour class.
    //
    // This function can be called by the iCanScript user to add to the
    // existing Unity library.
    // 
    public static void InstallMonoBehaviourMessage(string messageName, iCS_Parameter[] parameters, 
                                           string iconPath= null, string description= null) {
        var voidReturn= new iCS_FunctionReturn("", typeof(void));                                       
        InstallUnityMessage(typeof(MonoBehaviour), messageName, iCS_StorageClass.Instance, parameters, voidReturn, description, iconPath);
    }
    
        
    // ======================================================================
    // The following is the list of preinstalled Unity classes.
    // ----------------------------------------------------------------------
    public static void PopulateDataBase() {
        DecodeUnityClassInfo(typeof(AccelerationEvent));
        
        DecodeUnityClassInfo(typeof(AnimationCurve));
        DecodeUnityClassInfo(typeof(AnimationEvent));
        DecodeUnityClassInfo(typeof(AnimationState));
        DecodeUnityClassInfo(typeof(Application));
        DecodeUnityClassInfo(typeof(Array));
        DecodeUnityClassInfo(typeof(AudioSettings));
        DecodeUnityClassInfo(typeof(BitStream));
        DecodeUnityClassInfo(typeof(BoneWeight));
        DecodeUnityClassInfo(typeof(Bounds));
        DecodeUnityClassInfo(typeof(Caching));
        DecodeUnityClassInfo(typeof(ClothSkinningCoefficient));
        DecodeUnityClassInfo(typeof(Collision));
        DecodeUnityClassInfo(typeof(Color32));
        DecodeUnityClassInfo(typeof(Color));
        DecodeUnityClassInfo(typeof(CombineInstance));
        DecodeUnityClassInfo(typeof(ContactPoint));
        DecodeUnityClassInfo(typeof(ControllerColliderHit));
        DecodeUnityClassInfo(typeof(Debug));
        DecodeUnityClassInfo(typeof(DetailPrototype));
        DecodeUnityClassInfo(typeof(Event));
        DecodeUnityClassInfo(typeof(GL));
        DecodeUnityClassInfo(typeof(GUIContent));
        DecodeUnityClassInfo(typeof(GUILayoutOption));
        DecodeUnityClassInfo(typeof(GUILayoutUtility));
        DecodeUnityClassInfo(typeof(GUILayout));
        DecodeUnityClassInfo(typeof(GUISettings));
        DecodeUnityClassInfo(typeof(GUIStyleState));
        DecodeUnityClassInfo(typeof(GUIStyle));
        DecodeUnityClassInfo(typeof(GUIUtility));
        DecodeUnityClassInfo(typeof(GUI));
        DecodeUnityClassInfo(typeof(GeometryUtility));
        DecodeUnityClassInfo(typeof(Gizmos));
        DecodeUnityClassInfo(typeof(Graphics));
        DecodeUnityClassInfo(typeof(Gyroscope));
        DecodeUnityClassInfo(typeof(Hashtable));
        DecodeUnityClassInfo(typeof(HostData));
        DecodeUnityClassInfo(typeof(Input), kUnityEnginePackage, kJoystickIcon);
        DecodeUnityClassInfo(typeof(JointDrive));
        DecodeUnityClassInfo(typeof(JointLimits));
        DecodeUnityClassInfo(typeof(JointMotor));
        DecodeUnityClassInfo(typeof(JointSpring));
        DecodeUnityClassInfo(typeof(Keyframe));
        DecodeUnityClassInfo(typeof(LayerMask));
        DecodeUnityClassInfo(typeof(LightmapData));
        DecodeUnityClassInfo(typeof(LightmapSettings));
        DecodeUnityClassInfo(typeof(LocationInfo));
        DecodeUnityClassInfo(typeof(MasterServer));
        DecodeUnityClassInfo(typeof(MaterialPropertyBlock));
        DecodeUnityClassInfo(typeof(Mathf));
        DecodeUnityClassInfo(typeof(Matrix4x4));
        DecodeUnityClassInfo(typeof(NetworkMessageInfo));
        DecodeUnityClassInfo(typeof(NetworkPlayer));
        DecodeUnityClassInfo(typeof(NetworkViewID));
        DecodeUnityClassInfo(typeof(Network));
        
        DecodeUnityClassInfo(typeof(UnityEngine.Object));        
            DecodeUnityClassInfo(typeof(AnimationClip));        
            DecodeUnityClassInfo(typeof(AssetBundle));        
            DecodeUnityClassInfo(typeof(AudioClip));        
            DecodeUnityClassInfo(typeof(Component));
                DecodeUnityClassInfo(typeof(Behaviour));    
                    DecodeUnityClassInfo(typeof(Animation));        
                    DecodeUnityClassInfo(typeof(AudioChorusFilter));        
                    DecodeUnityClassInfo(typeof(AudioDistortionFilter));        
                    DecodeUnityClassInfo(typeof(AudioEchoFilter));        
                    DecodeUnityClassInfo(typeof(AudioHighPassFilter));        
                    DecodeUnityClassInfo(typeof(AudioListener));        
                    DecodeUnityClassInfo(typeof(AudioLowPassFilter));        
                    DecodeUnityClassInfo(typeof(AudioReverbFilter));        
                    DecodeUnityClassInfo(typeof(AudioReverbZone));        
                    DecodeUnityClassInfo(typeof(AudioSource));        
                    DecodeUnityClassInfo(typeof(Camera));        
                    DecodeUnityClassInfo(typeof(ConstantForce));        
                    DecodeUnityClassInfo(typeof(GUIElement));        
                        DecodeUnityClassInfo(typeof(GUIText));        
                        DecodeUnityClassInfo(typeof(GUITexture));        
                    DecodeUnityClassInfo(typeof(GUILayer));        
                    DecodeUnityClassInfo(typeof(LensFlare));        
                    DecodeUnityClassInfo(typeof(Light));        
                    DecodeUnityClassInfo(typeof(MonoBehaviour));        
//        DecodeUnityClassInfo(typeof(Terrain));        // This is a special behaviour (not supported yet)
//                    DecodeUnityClassInfo(typeof(NavMeshAgent));        
                    DecodeUnityClassInfo(typeof(NetworkView));        
                    DecodeUnityClassInfo(typeof(Projector));        
                    DecodeUnityClassInfo(typeof(Skybox));        
                DecodeUnityClassInfo(typeof(Cloth));        
                    DecodeUnityClassInfo(typeof(InteractiveCloth));        
                    DecodeUnityClassInfo(typeof(SkinnedCloth));        
                DecodeUnityClassInfo(typeof(Collider));        
                    DecodeUnityClassInfo(typeof(BoxCollider));        
                    DecodeUnityClassInfo(typeof(CapsuleCollider));        
                    DecodeUnityClassInfo(typeof(CharacterController));        
                    DecodeUnityClassInfo(typeof(MeshCollider));        
                    DecodeUnityClassInfo(typeof(SphereCollider));        
                    DecodeUnityClassInfo(typeof(TerrainCollider));        
                    DecodeUnityClassInfo(typeof(WheelCollider));        
                DecodeUnityClassInfo(typeof(Joint));        
                    DecodeUnityClassInfo(typeof(CharacterJoint));        
                    DecodeUnityClassInfo(typeof(ConfigurableJoint));        
                    DecodeUnityClassInfo(typeof(FixedJoint));        
                    DecodeUnityClassInfo(typeof(HingeJoint));        
                    DecodeUnityClassInfo(typeof(SpringJoint));        
//                DecodeUnityClassInfo(typeof(LODGroup));        
//                DecodeUnityClassInfo(typeof(LightProbeGroup));        
                DecodeUnityClassInfo(typeof(MeshFilter));        
                DecodeUnityClassInfo(typeof(OcclusionArea));        
                DecodeUnityClassInfo(typeof(ParticleAnimator));        
                DecodeUnityClassInfo(typeof(ParticleEmitter));        
                DecodeUnityClassInfo(typeof(Renderer));        
                    DecodeUnityClassInfo(typeof(ClothRenderer));        
                    DecodeUnityClassInfo(typeof(LineRenderer));        
                    DecodeUnityClassInfo(typeof(MeshRenderer));        
                    DecodeUnityClassInfo(typeof(ParticleRenderer));        
                    DecodeUnityClassInfo(typeof(SkinnedMeshRenderer));        
                    DecodeUnityClassInfo(typeof(TrailRenderer));        
                DecodeUnityClassInfo(typeof(Rigidbody));        
                DecodeUnityClassInfo(typeof(TextMesh));        
                DecodeUnityClassInfo(typeof(Transform));        
                DecodeUnityClassInfo(typeof(Tree));        
            DecodeUnityClassInfo(typeof(Flare));        
            DecodeUnityClassInfo(typeof(Font));        
            DecodeUnityClassInfo(typeof(GameObject));        
//            DecodeUnityClassInfo(typeof(LightProbes));        
            DecodeUnityClassInfo(typeof(Material));        
                DecodeUnityClassInfo(typeof(ProceduralMaterial));        
            DecodeUnityClassInfo(typeof(Mesh));        
//            DecodeUnityClassInfo(typeof(NavMesh));        
            DecodeUnityClassInfo(typeof(PhysicMaterial));        
            DecodeUnityClassInfo(typeof(QualitySettings));        
            DecodeUnityClassInfo(typeof(ScriptableObject));        
                DecodeUnityClassInfo(typeof(GUISkin));        
            DecodeUnityClassInfo(typeof(Shader));        
            DecodeUnityClassInfo(typeof(TerrainData));        
            DecodeUnityClassInfo(typeof(TextAsset));        
            DecodeUnityClassInfo(typeof(Texture));        
                DecodeUnityClassInfo(typeof(Cubemap));        
                DecodeUnityClassInfo(typeof(MovieTexture));        
                DecodeUnityClassInfo(typeof(RenderTexture));        
                DecodeUnityClassInfo(typeof(Texture2D));        
//                DecodeUnityClassInfo(typeof(WebCamTexture));        
//            DecodeUnityClassInfo(typeof(OffMeshLinkData));
//            DecodeUnityClassInfo(typeof(ParticleSystem.Particle));
            DecodeUnityClassInfo(typeof(Particle));
//            DecodeUnityClassInfo(typeof(Path));
            DecodeUnityClassInfo(typeof(Physics));
            DecodeUnityClassInfo(typeof(Ping));
            DecodeUnityClassInfo(typeof(Plane));
            DecodeUnityClassInfo(typeof(PlayerPrefsException));
            DecodeUnityClassInfo(typeof(PlayerPrefs));
            DecodeUnityClassInfo(typeof(ProceduralPropertyDescription));
            DecodeUnityClassInfo(typeof(Profiler));
            DecodeUnityClassInfo(typeof(Quaternion));
            DecodeUnityClassInfo(typeof(UnityEngine.Random));
//            DecodeUnityClassInfo(typeof(Range));
            DecodeUnityClassInfo(typeof(Ray));
            DecodeUnityClassInfo(typeof(RaycastHit));
            DecodeUnityClassInfo(typeof(RectOffset));
            DecodeUnityClassInfo(typeof(Rect));
#if UNITY_IPHONE
            DecodeUnityClassInfo(typeof(RemoteNotification));
#endif
//            DecodeUnityClassInfo(typeof(RenderBuffer));
            DecodeUnityClassInfo(typeof(RenderSettings));
            DecodeUnityClassInfo(typeof(Resolution));
            DecodeUnityClassInfo(typeof(Resources));
            DecodeUnityClassInfo(typeof(Screen));
            DecodeUnityClassInfo(typeof(Security));
            DecodeUnityClassInfo(typeof(SoftJointLimit));
            DecodeUnityClassInfo(typeof(SplatPrototype));
            DecodeUnityClassInfo(typeof(StaticBatchingUtility));
            DecodeUnityClassInfo(typeof(String));
            DecodeUnityClassInfo(typeof(SystemInfo));
            DecodeUnityClassInfo(typeof(Time));
        
            DecodeUnityClassInfo(typeof(Vector2));    
            DecodeUnityClassInfo(typeof(Vector3));    
            DecodeUnityClassInfo(typeof(Vector4));
            
            // Install MonoBehaviour messages with no parameters.
            var noParameters= new iCS_Parameter[0];
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
            InstallMonoBehaviourMessage("OnRenderImage"       , noParameters, kInMailIcon);
            InstallMonoBehaviourMessage("OnDrawGizmosSelected", noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnDrawGizmos"        , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnServerInitialized" , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnConnectedToServer" , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnAudioFilterRead"   , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnAnimatorMove"      , noParameters, kInMailIcon);         
            InstallMonoBehaviourMessage("OnApplicationQuit"   , noParameters, kInMailIcon);         

            // Trigger messages
            var triggerParameters= new iCS_Parameter[1]{new iCS_Parameter("other", typeof(Collider))};
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
    }

}
