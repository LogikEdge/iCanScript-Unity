using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Performs all sanity checks.
    void SanityCheck() {
//        SanityCheck_Animation();
    }
    // ----------------------------------------------------------------------
    // Validates the editor/engine container coherency.
    void SanityCheck_EditorEngineContainers() {
        if(EditorObjects.Count != EngineObjects.Count) {
            Debug.LogWarning("iCanScript: Editor/Engine contains are not of same size !!!\nEditorObjects size: "+
                             EditorObjects.Count+" EngineObjects size: "+EngineObjects.Count);
        }
    }
    // ----------------------------------------------------------------------
    // Validates that animation has been properly applied.
    void SanityCheck_Animation() {
        ForEach(
            obj=> {
                var animation= obj.AnimatedPosition;
                if(obj.IsVisible) {
                    if(Math3D.IsNotEqual(obj.GlobalRect, animation.TargetValue)) {
                        Debug.LogWarning("iCanScript: Animation was not properly applied for: "+obj.Name+" with id: "+obj.InstanceId);                        
                    }
                } else {
                    float area= animation.TargetValue.width*animation.TargetValue.height;
                    if(Math3D.IsNotZero(area)) {
                        Debug.LogWarning("iCanScript: Shrink animation was not properly applied for: "+obj.Name+" with id: "+obj.InstanceId);
                    }
                }
            }
        );
    }
}
