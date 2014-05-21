using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_DemoMenus {
    [MenuItem("Help/iCanScript/Purchase...", false, 71)]
    public static void Purchase() {
        Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/16872");
    }
}
