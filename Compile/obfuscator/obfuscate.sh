cp /Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll Obfuscar_Input
cp /Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll Obfuscar_Input
cp ../iCanScriptEngine.dll Obfuscar_Input
cp ../iCanScriptEditor.dll Obfuscar_Input
mono Obfuscar.exe obfuscar.xml
cp Obfuscar_Output/iCanScriptEditor.dll ../iCanScriptEditor.dll
cp Obfuscar_Output/iCanScriptEngine.dll ../iCanScriptEngine.dll
