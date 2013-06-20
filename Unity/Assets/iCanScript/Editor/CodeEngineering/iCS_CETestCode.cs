using UnityEngine;
using System.Collections;

public static class iCS_CETestCode {

	public static void TestCodeEngineering() {
		// TODO: Remove this test code.
        if(!iCS_CETextFile.Exists("CodeTest.cs")) {
            string code= "using UnityEngine;\n\npublic class TestAsset : MonoBehaviour {\n\tvoid Update() {\n\t\tDebug.Log(\"Hello World!\");\n\t}\n}\n";
            iCS_CETextFile.WriteFile("CodeTest.cs", code);
			iCS_CETextFile.EditFile("CodeTest.cs");
        }		
	}
}
