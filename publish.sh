rm -r ../iCanScriptPublish/Assets/Nodeling
mkdir ../iCanScriptPublish/Assets/Nodeling
mkdir ../iCanScriptPublish/Assets/Nodeling/Editor
mkdir ../iCanScriptPublish/Assets/Nodeling/Engine
cp -r Assets/Nodeling/Editor/Resources ../iCanScriptPublish/Assets/Nodeling/Editor/Resources
cp Library/ScriptAssemblies/Assembly-CSharp-Editor.dll ../iCanScriptPublish/Assets/Nodeling/Editor/NodelingEditor.dll
cp Library/ScriptAssemblies/Assembly-CSharp.dll ../iCanScriptPublish/Assets/Nodeling/Engine/NodelingEngine.dll
