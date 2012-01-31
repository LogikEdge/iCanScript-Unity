# Constants
SOURCE_ROOT=../Assets/iCanScript
SOURCE_EDITOR_ROOT=$SOURCE_ROOT/Editor
SOURCE_ENGINE_ROOT=$SOURCE_ROOT/Engine
SOURCE_EDITOR_PUBLIC_SOURCES=$SOURCE_EDITOR_ROOT/PublicSources
SOURCE_ENGINE_PUBLIC_SOURCES=$SOURCE_ENGINE_ROOT/PublicSources
DESTINATION_ROOT=../../iCanScriptPublish/Assets/iCanScript
DESTINATION_EDITOR_ROOT=$DESTINATION_ROOT/Editor
DESTINATION_ENGINE_ROOT=$DESTINATION_ROOT
# Generate file list.
find $SOURCE_EDITOR_PUBLIC_SOURCES -name "*.cs" >editorFilesToExclude
find $SOURCE_ENGINE_PUBLIC_SOURCES -name "*.cs" >engineFilesToExclude
find $SOURCE_EDITOR_ROOT -name "*.cs" >_editorFiles
find $SOURCE_ROOT -name "*.cs" | grep -v -f _editorFiles - >_engineFiles
grep -v -f editorFilesToExclude _editorFiles >editorFiles
grep -v -f engineFilesToExclude _engineFiles >engineFiles
# Create response files.
cat EditorCommands editorFiles >iCanScriptEditor.rsp
cat EngineCommands engineFiles >iCanScriptEngine.rsp
# Compile libraries.
gmcs @iCanScriptEngine.rsp
gmcs @iCanScriptEditor.rsp
# Install libraries inside the publish directory.
rm -r -f $DESTINATION_ROOT
mkdir $DESTINATION_ROOT
cp iCanScriptEngine.dll $DESTINATION_ENGINE_ROOT
mkdir $DESTINATION_EDITOR_ROOT
cp iCanScriptEditor.dll $DESTINATION_EDITOR_ROOT
cp -r $SOURCE_EDITOR_ROOT/Resources $DESTINATION_EDITOR_ROOT
# Install visible source files.
rsync -av --exclude=*/*.meta $SOURCE_EDITOR_PUBLIC_SOURCES $DESTINATION_EDITOR_ROOT
rsync -av --exclude=*/*.meta $SOURCE_ENGINE_PUBLIC_SOURCES $DESTINATION_ENGINE_ROOT

