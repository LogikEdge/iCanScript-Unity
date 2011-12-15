using UnityEngine;
using System;
using System.Collections;

public static class iCS_UnityClasses {
    public static void PopulateDataBase() {
        DecodeUnityClassInfo(typeof(AccelerationEvent));
        
        DecodeUnityClassInfo(typeof(AnimationCurve));
        DecodeUnityClassInfo(typeof(AnimationEvent));
        DecodeUnityClassInfo(typeof(AnimationState));
        DecodeUnityClassInfo(typeof(Application));
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
    public static void DecodeUnityClassInfo(Type type) {
        iCS_Reflection.DecodeClassInfo(type, "Unity", type.Name, type.Name, "Unity class "+type.Name, null, true);
    }
}
