# Mono Variables
APPLICATION_ROOT=/Applications
UNITY_APP_DIR=$APPLICATION_ROOT/Unity/Unity.app
MONO_ROOT=$UNITY_APP_DIR/Contents/Frameworks/Mono
MONO_BIN_DIR=$MONO_ROOT/bin
MONO_LIB_DIR=$MONO_ROOT/lib
MONO_ETC_DIR=$MONO_ROOT/etc
# Tools
GMCS=$MONO_BIN_DIR/gmcs
# Constants
ROOT_DIR=../..
UNITY_DIR=$ROOT_DIR/Unity
ASSETS_DIR=$UNITY_DIR/Assets
PRODUCT_DIR=$ASSETS_DIR/iCanScript
EDITOR_DIR=$PRODUCT_DIR/Editor
ENGINE_DIR=$PRODUCT_DIR/Engine
EDITOR_PUBLIC_SOURCES_DIR=$EDITOR_DIR/NodeInstaller
ENGINE_PUBLIC_SOURCES_DIR=$ENGINE_DIR/Nodes
PUBLISH_ROOT=$ROOT_DIR/../Published
PUBLISH_ASSETS_DIR=$PUBLISH_ROOT/Assets
PUBLISH_PRODUCT_DIR=$PUBLISH_ASSETS_DIR/iCanScript
PUBLISH_EDITOR_DIR=$PUBLISH_PRODUCT_DIR/Editor
PUBLISH_ENGINE_DIR=$PUBLISH_PRODUCT_DIR/Engine
# Generate file list.
echo "Generating file list ..."
find $EDITOR_PUBLIC_SOURCES_DIR -name "*.cs" >editorFilesToExclude
find $ENGINE_PUBLIC_SOURCES_DIR -name "*.cs" >engineFilesToExclude
find $EDITOR_DIR -name "*.cs" >_editorFiles
find $PRODUCT_DIR -name "*.cs" | grep -v -f _editorFiles - >_engineFiles
grep -v -f editorFilesToExclude _editorFiles >editorFiles
grep -v -f engineFilesToExclude _engineFiles >engineFiles
# Create response files.
cat EditorCommands editorFiles >iCanScriptEditor.rsp
cat EngineCommands engineFiles >iCanScriptEngine.rsp
# Compile libraries.
echo "Compiling engine code..."
$GMCS @iCanScriptEngine.rsp
echo "Compiling editor code..."
$GMCS @iCanScriptEditor.rsp
# Run obfuscator.
#echo "Running obfuscator..."
#./obfuscate.sh
# Install libraries inside the publish directory.
echo "Installing into" $PUBLISH_PRODUCT_DIR "..."
rm -r -f $PUBLISH_ROOT
mkdir $PUBLISH_ROOT
mkdir $PUBLISH_ASSETS_DIR
mkdir $PUBLISH_PRODUCT_DIR
mkdir $PUBLISH_ENGINE_DIR
mkdir $PUBLISH_EDITOR_DIR
cp iCanScriptEngine.dll $PUBLISH_ENGINE_DIR
cp iCanScriptEditor.dll $PUBLISH_EDITOR_DIR
# Install visible source files.
cp $PRODUCT_DIR/readme.txt $PUBLISH_PRODUCT_DIR
#cp -r $PRODUCT_DIR/AssetStore $DESTINATION_DIR
cp -r $EDITOR_DIR/Resources $PUBLISH_EDITOR_DIR
rsync -av --exclude=*/*.meta $EDITOR_PUBLIC_SOURCES_DIR $PUBLISH_EDITOR_DIR >/dev/null
rsync -av --exclude=*/*.meta $ENGINE_PUBLIC_SOURCES_DIR $PUBLISH_ENGINE_DIR >/dev/null

