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
EDITOR_PUBLIC_NODE_INSTALLER_DIR=$EDITOR_DIR/NodeInstaller
EDITOR_PUBLIC_EDITOR_WINDOWS_DIR=$EDITOR_DIR/EditorWindows
ENGINE_PUBLIC_COMPONENTS_DIR=$ENGINE_DIR/Components
ENGINE_PUBLIC_NODES_DIR=$ENGINE_DIR/Nodes
PUBLISH_ROOT=$ROOT_DIR/../Published
PUBLISH_ASSETS_DIR=$PUBLISH_ROOT/Assets
PUBLISH_PRODUCT_DIR=$PUBLISH_ASSETS_DIR/iCanScript
PUBLISH_EDITOR_DIR=$PUBLISH_PRODUCT_DIR/Editor
PUBLISH_ENGINE_DIR=$PUBLISH_PRODUCT_DIR/Engine
# Generate file list.
echo "Generating file list ..."
find $EDITOR_PUBLIC_NODE_INSTALLER_DIR -name "*.cs" >editorFilesToExclude
find $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR -name "*.cs" >>editorFilesToExclude
find $ENGINE_PUBLIC_COMPONENTS_DIR -name "*.cs" >engineFilesToExclude
find $ENGINE_PUBLIC_NODES_DIR -name "*.cs" >>engineFilesToExclude
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
if [ ! -d $PUBLISH_ROOT ]; then
    mkdir $PUBLISH_ROOT
fi
if [ ! -d $PUBLISH_ASSETS_DIR ]; then
    mkdir $PUBLISH_ASSETS_DIR
fi
if [ -d $PUBLISH_PRODUCT_DIR ]; then
    rm -r -f $PUBLISH_PRODUCT_DIR
fi
mkdir $PUBLISH_PRODUCT_DIR
mkdir $PUBLISH_ENGINE_DIR
mkdir $PUBLISH_EDITOR_DIR
cp iCanScriptEngine.dll $PUBLISH_ENGINE_DIR
cp iCanScriptEditor.dll $PUBLISH_EDITOR_DIR
# Install visible source files.
cp $PRODUCT_DIR/readme.txt $PUBLISH_PRODUCT_DIR
#cp -r $PRODUCT_DIR/AssetStore $DESTINATION_DIR
cp -r $EDITOR_DIR/Resources $PUBLISH_EDITOR_DIR
rsync -av $EDITOR_PUBLIC_NODE_INSTALLER_DIR $PUBLISH_EDITOR_DIR >/dev/null
rsync -av $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR $PUBLISH_EDITOR_DIR >/dev/null
rsync -av $ENGINE_PUBLIC_COMPONENTS_DIR $PUBLISH_ENGINE_DIR >/dev/null
rsync -av $ENGINE_PUBLIC_NODES_DIR $PUBLISH_ENGINE_DIR >/dev/null
#rsync -av --exclude=*/*.meta $EDITOR_PUBLIC_NODE_INSTALLER_DIR $PUBLISH_EDITOR_DIR >/dev/null
#rsync -av --exclude=*/*.meta $ENGINE_PUBLIC_NODES_DIR $PUBLISH_ENGINE_DIR >/dev/null

# ==================================================================================
# Example demo project
DEMO_PROJECT_DIR=$HOME/Workspaces/ZaZoo
DEMO_INSTALL_DIR=$DEMO_PROJECT_DIR/Assets/iCanScript
DEMO_EDITOR_DIR=$DEMO_INSTALL_DIR/Editor
DEMO_ENGINE_DIR=$DEMO_INSTALL_DIR/Engine
echo "Installing into" $DEMO_PROJECT_DIR 
if [ -d $DEMO_INSTALL_DIR ]; then
    rm -r -f $DEMO_INSTALL_DIR
fi
mkdir $DEMO_INSTALL_DIR
mkdir $DEMO_EDITOR_DIR
mkdir $DEMO_ENGINE_DIR
cp iCanScriptEditor.dll $DEMO_EDITOR_DIR
cp iCanScriptEngine.dll $DEMO_ENGINE_DIR
cp $PRODUCT_DIR/readme.txt $DEMO_INSTALL_DIR
cp -r $EDITOR_DIR/Resources $DEMO_EDITOR_DIR
rsync -av $EDITOR_PUBLIC_NODE_INSTALLER_DIR $DEMO_EDITOR_DIR >/dev/null
rsync -av $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR $DEMO_EDITOR_DIR >/dev/null
rsync -av $ENGINE_PUBLIC_COMPONENTS_DIR $DEMO_ENGINE_DIR >/dev/null
