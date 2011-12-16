rm -r ../iCanScriptPublish/Assets/iCanScript
mkdir ../iCanScriptPublish/Assets/iCanScript
mkdir ../iCanScriptPublish/Assets/iCanScript/Editor
mkdir ../iCanScriptPublish/Assets/iCanScript/Engine
cp -r Assets/iCanScript/Editor/Resources ../iCanScriptPublish/Assets/iCanScript/Editor/Resources
cp Library/ScriptAssemblies/Assembly-CSharp-Editor.dll ../iCanScriptPublish/Assets/iCanScript/Editor/iCanScriptEditor.dll
cp Library/ScriptAssemblies/Assembly-CSharp.dll ../iCanScriptPublish/Assets/iCanScript/Engine/iCanScriptEngine.dll
