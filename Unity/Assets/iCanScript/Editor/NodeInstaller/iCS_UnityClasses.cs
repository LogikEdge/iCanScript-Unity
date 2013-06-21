using UnityEngine;
using System;
using System.Collections;

// ======================================================================
// This file installs predefined Unity3d classes and events.
//
// You may augment the Unity library by invoking the functions:
//      DecodeUnityClassInfo(...) and
//      InstallUnityEvent(...)
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
    // Use this function to register events on a Unity class.
    // Events are invoked using dynamic (runtime) lookup and and not
    // present in .NET assembly.  For that reason, events are not
    // automatically installed when decoding the Unity class. 
    //
    // This function can be called by the iCanScript user to add to the
    // existing Unity library.
    // 
    public static void InstallUnityEvent(Type classType, string eventName, iCS_StorageClass storageClass,
                                         iCS_Parameter[] parameters, iCS_FunctionReturn functionReturn,
                                         string iconPath= null, string description= null) {
        if(iconPath == null) iconPath= kUnityIcon;
        if(description == null) description= "Event: "+eventName+" on "+classType.Name;
        iCS_LibraryDatabase.AddEvent(classType, eventName, storageClass, parameters, functionReturn, iconPath, description);
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
            
            // Install Events on MonoBehaviour
            var noParameters= new iCS_Parameter[0];
            var voidReturn= new iCS_FunctionReturn("", typeof(void));
            InstallUnityEvent(typeof(MonoBehaviour), "OnMouseEnter", iCS_StorageClass.Instance, noParameters, voidReturn);
            InstallUnityEvent(typeof(MonoBehaviour), "OnMouseOver", iCS_StorageClass.Instance, noParameters, voidReturn);
            InstallUnityEvent(typeof(MonoBehaviour), "OnMouseExit", iCS_StorageClass.Instance, noParameters, voidReturn);
            InstallUnityEvent(typeof(MonoBehaviour), "OnMouseDown", iCS_StorageClass.Instance, noParameters, voidReturn);
            InstallUnityEvent(typeof(MonoBehaviour), "OnMouseUp", iCS_StorageClass.Instance, noParameters, voidReturn);
    }

}
