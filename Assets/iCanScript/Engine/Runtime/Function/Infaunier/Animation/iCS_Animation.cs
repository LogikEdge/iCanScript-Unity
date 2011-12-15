using UnityEngine;
using System.Collections;

[iCS_Class(Company="Infaunier", Package="Animation")]
public class iCS_Animation {
    [iCS_Function(ToolTip="Blends two animtions using the given ratio.")]
    public static AnimationClip AnimBlend(AnimationClip anim1, AnimationClip anim2, float ratio, bool sync) {
        return anim1;
    }

    [iCS_Function(ToolTip="Selects one or two animation clip determined by boolean selection.")]
    public static AnimationClip AnimSelect(AnimationClip anim1, AnimationClip anim2, bool selection) {
        return selection ? anim1 : anim2;
    }

    [iCS_Function]
    public static AnimationClip AnimFade(AnimationClip fromAnim, AnimationClip toAnim, float time, bool sync) {
        return toAnim;
    }

    [iCS_Function(ToolTip="Plays the given full body, facial, and addtive animation layers.")]
    public static void  PlayAnimLayers(AnimationClip fullBody, AnimationClip facial, AnimationClip additive) {
    }

    [iCS_Function(ToolTip="Selects the animation from the active state.")]
    public static AnimationClip AnimStateSelect(AnimationClip anim1, AnimationClip anim2, AnimationClip anim3, AnimationClip anim4) {
        return anim1;
    }
}


