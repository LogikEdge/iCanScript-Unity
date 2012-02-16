using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_ClassWizardProxy : iCS_ClassWizard {
    public static iCS_ClassWizard GetClassWizard() {
        return GetWindow(typeof(iCS_ClassWizardProxy), false, "Class Wizard") as iCS_ClassWizardProxy;
    }
}
