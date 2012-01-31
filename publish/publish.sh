rm -r ../../iCanScriptPublish/Assets/iCanScript
mkdir ../../iCanScriptPublish/Assets/iCanScript
mkdir ../../iCanScriptPublish/Assets/iCanScript/Editor
mkdir ../../iCanScriptPublish/Assets/iCanScript/Engine
cp -r ../Assets/iCanScript/readme.txt ../../iCanScriptPublish/Assets/iCanScript/readme.txt
cp -r ../Assets/iCanScript/AssetStore ../../iCanScriptPublish/Assets/iCanScript/AssetStore
cp -r ../Assets/iCanScript/Editor/Resources ../../iCanScriptPublish/Assets/iCanScript/Editor/Resources


#cp /Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll Obfuscar_Input
#cp /Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll Obfuscar_Input
#cp ../Library/ScriptAssemblies/Assembly-CSharp-Editor.dll Obfuscar_Input
#cp ../Library/ScriptAssemblies/Assembly-CSharp.dll Obfuscar_Input
#mono Obfuscar.exe obfuscar.xml
#cp Obfuscar_Output/Assembly-CSharp-Editor.dll ../../iCanScriptPublish/Assets/iCanScript/Editor/iCanScriptEditor.dll
#cp Obfuscar_Output/Assembly-CSharp.dll ../../iCanScriptPublish/Assets/iCanScript/Engine/iCanScriptEngine.dll

cp ../Library/ScriptAssemblies/Assembly-CSharp-Editor.dll ../../iCanScriptPublish/Assets/iCanScript/Editor/iCanScriptEditor.dll
cp ../Library/ScriptAssemblies/Assembly-CSharp.dll ../../iCanScriptPublish/Assets/iCanScript/Engine/iCanScriptEngine.dll
