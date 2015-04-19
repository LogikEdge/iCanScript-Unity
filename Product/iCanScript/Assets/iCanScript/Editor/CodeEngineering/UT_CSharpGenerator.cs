using UnityEngine;
using System;
using System.Text;
using System.Collections;

public class UnitTest {}

namespace iCanScript.Editor.CodeEngineering {

    public class UT_CSharpGenerator : UnitTest {
        
		// -------------------------------------------------------------------------
        public static void GenerateTestCSharpFile(iCS_IStorage iStorage) {
            var codeGenerator= new CodeGenerator();
            codeGenerator.GenerateCodeFor(iStorage);
        }
    }

}

