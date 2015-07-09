using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript;

namespace iCanScript.Internal.Editor {
    
    public static class UnityUtility {
        // -----------------------------------------------------------------------
        //! Returns true if the given object is a GameObject
        public static bool IsGameObject(UnityEngine.Object theObject) {
            return iCS_Types.IsA<GameObject>(theObject.GetType());
        }
        // -----------------------------------------------------------------------
        /// Returns true if the given GameObject is a Prefab.
        public static bool IsPrefab(GameObject go) {
            if(go == null) return false;
            var prefabType= PrefabUtility.GetPrefabType(go);
            if(prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
                return true;
            }
            return false;
        }
        // -----------------------------------------------------------------------
        /// Returns true if the given GameObject is an instance of a Prefab.
        public static bool IsPrefabInstance(GameObject go) {
            if(go == null) return false;
            var prefabType= PrefabUtility.GetPrefabType(go);
            if( prefabType == PrefabType.PrefabInstance ||
                prefabType == PrefabType.ModelPrefabInstance) {
                    return true;
            }
            return false;
        }
        // -----------------------------------------------------------------------
        /// Returns true if the given GameObject is in the current scene.
        public static bool IsSceneGameObject(GameObject go) {
            if(go == null) return false;
            return !IsPrefab(go);
        }

        // =======================================================================
        /// Retreives the Unity project name.
        ///
        /// @return The unity project name.
        ///
        public static string GetProjectName()
        {
            string[] s = Application.dataPath.Split('/');
            string projectName = s[s.Length - 2];
            return projectName;
        }
    }

}
