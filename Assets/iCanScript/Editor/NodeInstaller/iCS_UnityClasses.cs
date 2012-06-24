using UnityEngine;
using System;
using System.Collections;

public static class iCS_UnityClasses {
    // ----------------------------------------------------------------------
    // Install the desired .NET classes
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
        DecodeUnityClassInfo(typeof(Input));
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
        
        DecodeUnityClassInfo(typeof(AnimationClip));        
        DecodeUnityClassInfo(typeof(AssetBundle));        
        DecodeUnityClassInfo(typeof(AudioClip));        
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
        DecodeUnityClassInfo(typeof(GUIText));        
        DecodeUnityClassInfo(typeof(GUITexture));        
        DecodeUnityClassInfo(typeof(GUILayer));        
        DecodeUnityClassInfo(typeof(LensFlare));        
        DecodeUnityClassInfo(typeof(Light));        
//        DecodeUnityClassInfo(typeof(Terrain));        // This is a special behaviour (not supported yet)
        DecodeUnityClassInfo(typeof(NetworkView));        
        DecodeUnityClassInfo(typeof(Projector));        
        DecodeUnityClassInfo(typeof(Skybox));        
        DecodeUnityClassInfo(typeof(InteractiveCloth));        
        DecodeUnityClassInfo(typeof(SkinnedCloth));        
        DecodeUnityClassInfo(typeof(BoxCollider));        
        DecodeUnityClassInfo(typeof(CapsuleCollider));        
        DecodeUnityClassInfo(typeof(CharacterController));        
        DecodeUnityClassInfo(typeof(MeshCollider));        
        DecodeUnityClassInfo(typeof(SphereCollider));        
        DecodeUnityClassInfo(typeof(TerrainCollider));        
        DecodeUnityClassInfo(typeof(WheelCollider));        
        DecodeUnityClassInfo(typeof(CharacterJoint));        
        DecodeUnityClassInfo(typeof(ConfigurableJoint));        
        DecodeUnityClassInfo(typeof(FixedJoint));        
        DecodeUnityClassInfo(typeof(HingeJoint));        
        DecodeUnityClassInfo(typeof(SpringJoint));        
        DecodeUnityClassInfo(typeof(MeshFilter));        
        DecodeUnityClassInfo(typeof(OcclusionArea));        
        DecodeUnityClassInfo(typeof(ParticleAnimator));        
        DecodeUnityClassInfo(typeof(ParticleEmitter));        
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
        DecodeUnityClassInfo(typeof(ProceduralMaterial));        
        DecodeUnityClassInfo(typeof(Mesh));        
        DecodeUnityClassInfo(typeof(PhysicMaterial));        
        DecodeUnityClassInfo(typeof(GUISkin));        
        DecodeUnityClassInfo(typeof(Shader));        
        DecodeUnityClassInfo(typeof(TerrainData));        
        DecodeUnityClassInfo(typeof(TextAsset));        
        DecodeUnityClassInfo(typeof(Cubemap));        
        DecodeUnityClassInfo(typeof(MovieTexture));        
        DecodeUnityClassInfo(typeof(RenderTexture));        
        DecodeUnityClassInfo(typeof(Texture2D));        
        
        DecodeUnityClassInfo(typeof(Particle));
        DecodeUnityClassInfo(typeof(Physics));
        DecodeUnityClassInfo(typeof(Ping));
        DecodeUnityClassInfo(typeof(Plane));
        DecodeUnityClassInfo(typeof(PlayerPrefsException));
        DecodeUnityClassInfo(typeof(PlayerPrefs));
        DecodeUnityClassInfo(typeof(ProceduralPropertyDescription));
        DecodeUnityClassInfo(typeof(Profiler));
        DecodeUnityClassInfo(typeof(QualitySettings));
        DecodeUnityClassInfo(typeof(Quaternion));
        DecodeUnityClassInfo(typeof(UnityEngine.Random));
        DecodeUnityClassInfo(typeof(Ray));
        DecodeUnityClassInfo(typeof(RaycastHit));
        DecodeUnityClassInfo(typeof(RectOffset));
        DecodeUnityClassInfo(typeof(Rect));
        DecodeUnityClassInfo(typeof(RenderSettings));
        DecodeUnityClassInfo(typeof(Resolution));
        DecodeUnityClassInfo(typeof(Resources));
        DecodeUnityClassInfo(typeof(Screen));
        DecodeUnityClassInfo(typeof(Security));
        DecodeUnityClassInfo(typeof(SoftJointLimit));
        
        DecodeUnityClassInfo(typeof(SystemInfo));
        DecodeUnityClassInfo(typeof(Time));
        
        DecodeUnityClassInfo(typeof(Vector2));    
        DecodeUnityClassInfo(typeof(Vector3));    
        DecodeUnityClassInfo(typeof(Vector4));
    }

    // ----------------------------------------------------------------------
    // Helper function to simplify .NET class decoding.
    public static void DecodeUnityClassInfo(Type type) {
        iCS_Reflection.DecodeClassInfo(type, "Unity", "UnityEngine", "Unity class "+type.Name, null, true);
    }
}
