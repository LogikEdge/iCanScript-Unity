# Mono Variables
APPLICATION_ROOT=/Applications
UNITY_APP_DIR=$APPLICATION_ROOT/Unity/Unity.app
MONO_ROOT=$UNITY_APP_DIR/Contents/Frameworks/Mono
MONO_BIN_DIR=$MONO_ROOT/bin
MONO_LIB_DIR=$MONO_ROOT/lib
MONO_ETC_DIR=$MONO_ROOT/etc
# Tools
GMCS=$MONO_BIN_DIR/gmcs
# Build Info Constants
BUILD_INFO_GENERATOR=./createBuildInfo
BUILD_INFO_CLASS=iCS_BuildInfo
BUILD_DATE_VAR=kBuildDateStr
# Constants
ROOT_DIR=../..
UNITY_DIR=$ROOT_DIR/Unity
ASSETS_DIR=$UNITY_DIR/Assets
ASSET_STORE_TOOLS_DIR=$ASSETS_DIR/AssetStoreTools
PRODUCT_DIR=$ASSETS_DIR/iCanScript
EDITOR_DIR=$PRODUCT_DIR/Editor
ENGINE_DIR=$PRODUCT_DIR/Engine
EDITOR_EDITIONS_ROOT=$EDITOR_DIR/Editions
EDITOR_DEV_EDITION_DIR=$EDITOR_EDITIONS_ROOT/Dev
EDITOR_DEMO_EDITION_DIR=$EDITOR_EDITIONS_ROOT/Demo
EDITOR_UNITY_STORE_EDITION_DIR=$EDITOR_EDITIONS_ROOT/UnityStore
EDITOR_DEVTOOLS_DIR=$EDITOR_DIR/DevTools
EDITOR_PUBLIC_NODE_INSTALLER_DIR=$EDITOR_DIR/NodeInstaller
EDITOR_PUBLIC_EDITOR_WINDOWS_DIR=$EDITOR_DIR/EditorWindows
ENGINE_PUBLIC_COMPONENTS_DIR=$ENGINE_DIR/Components
ENGINE_PUBLIC_NODES_DIR=$ENGINE_DIR/Nodes
DEMO_SCENES_DIR=$PRODUCT_DIR/Demo_Scenes
STAGING_ROOT=$ROOT_DIR/../Staging
STAGING_ASSETS_DIR=$STAGING_ROOT/Assets
STAGING_PRODUCT_DIR=$STAGING_ASSETS_DIR/iCanScript
STAGING_EDITOR_DIR=$STAGING_PRODUCT_DIR/Editor
STAGING_ENGINE_DIR=$STAGING_PRODUCT_DIR/Engine
BUILD_INFO_FILE=$EDITOR_DIR/iCS_BuildInfo.cs
# Generate file list.
echo "Generating file list ..."
find $EDITOR_PUBLIC_NODE_INSTALLER_DIR -name "*.cs" >editorFilesToExclude
find $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR -name "*.cs" >>editorFilesToExclude
find $EDITOR_DEVTOOLS_DIR -name "*.cs" >>editorFilesToExclude
find $EDITOR_DEV_EDITION_DIR -name "*.cs" >>editorFilesToExclude
find $EDITOR_DEMO_EDITION_DIR -name "*.cs" >> editorFilesToExclude
find $EDITOR_UNITY_STORE_EDITION_DIR -name "*.cs" >>editorFilesToExclude
find $ENGINE_PUBLIC_COMPONENTS_DIR -name "*.cs" >engineFilesToExclude
find $ENGINE_PUBLIC_NODES_DIR -name "*.cs" >>engineFilesToExclude
find $DEMO_SCENES_DIR >>engineFilesToExclude
find $EDITOR_DIR -name "*.cs" >_editorFiles
find $PRODUCT_DIR -name "*.cs" | grep -v -f _editorFiles - >_engineFiles
grep -v -f editorFilesToExclude _editorFiles >editorFiles
grep -v -f engineFilesToExclude _engineFiles >engineFiles
# Create response files.
cat EditorCommands editorFiles >iCanScriptEditor.rsp
cat EngineCommands engineFiles >iCanScriptEngine.rsp
# Compile libraries.
echo "Creating Build Info file..."
$BUILD_INFO_GENERATOR $ENGINE_DIR $BUILD_INFO_CLASS $BUILD_DATE_VAR
echo "Compiling engine code..."
$GMCS @iCanScriptEngine.rsp
echo "Compiling editor code..."
$GMCS @iCanScriptEditor.rsp
# Run obfuscator.
#echo "Running obfuscator..."
#./obfuscate.sh
# Install libraries inside the publish directory.
echo "Installing into" $STAGING_PRODUCT_DIR "..."
if [ ! -d $STAGING_ROOT ]; then
    mkdir $STAGING_ROOT
fi
if [ ! -d $STAGING_ASSETS_DIR ]; then
    mkdir $STAGING_ASSETS_DIR
fi
if [ -d $STAGING_PRODUCT_DIR ]; then
    rm -r -f $STAGING_PRODUCT_DIR
fi
mkdir $STAGING_PRODUCT_DIR
mkdir $STAGING_ENGINE_DIR
mkdir $STAGING_EDITOR_DIR
cp iCanScriptEngine.dll $STAGING_ENGINE_DIR
cp iCanScriptEditor.dll $STAGING_EDITOR_DIR
# Install documentation.
if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.txt' -print -quit)"; then
	echo "Copying *.txt files into" $STAGING_PRODUCT_DIR
	cp $PRODUCT_DIR/*.txt $STAGING_PRODUCT_DIR
fi
if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.pdf' -print -quit)"; then
	echo "Copying *.pdf files into" $STAGING_PRODUCT_DIR
	cp $PRODUCT_DIR/*.pdf $STAGING_PRODUCT_DIR
fi
if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.md' -print -quit)"; then
	echo "Copying *.md files into" $STAGING_PRODUCT_DIR
	cp $PRODUCT_DIR/*.md $STAGING_PRODUCT_DIR
fi
if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.html' -print -quit)"; then
	echo "Copying *.html files into" $STAGING_PRODUCT_DIR
	cp $PRODUCT_DIR/*.html $STAGING_PRODUCT_DIR
fi
# Install visible source files.
cp -r $EDITOR_DIR/Resources $STAGING_EDITOR_DIR
rsync -av $EDITOR_PUBLIC_NODE_INSTALLER_DIR $STAGING_EDITOR_DIR >/dev/null
rsync -av $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR $STAGING_EDITOR_DIR >/dev/null
rsync -av $ENGINE_PUBLIC_COMPONENTS_DIR $STAGING_ENGINE_DIR >/dev/null
rsync -av $ENGINE_PUBLIC_NODES_DIR $STAGING_ENGINE_DIR >/dev/null
rsync -av $DEMO_SCENES_DIR $STAGING_PRODUCT_DIR >/dev/null
rsync -av $ASSET_STORE_TOOLS_DIR $STAGING_ASSETS_DIR >/dev/null
#rsync -av --exclude=*/*.meta $EDITOR_PUBLIC_NODE_INSTALLER_DIR $STAGING_EDITOR_DIR >/dev/null
#rsync -av --exclude=*/*.meta $ENGINE_PUBLIC_NODES_DIR $STAGING_ENGINE_DIR >/dev/null

# ==================================================================================
# Example demo project
#DEMO_PROJECT_DIR=$HOME/Workspaces/ZaZoo
#DEMO_INSTALL_DIR=$DEMO_PROJECT_DIR/Assets/iCanScript
#DEMO_EDITOR_DIR=$DEMO_INSTALL_DIR/Editor
#DEMO_ENGINE_DIR=$DEMO_INSTALL_DIR/Engine
#echo "Installing into" $DEMO_PROJECT_DIR 
#if [ -d $DEMO_INSTALL_DIR ]; then
#    rm -r -f $DEMO_INSTALL_DIR
#fi
#mkdir $DEMO_INSTALL_DIR
#mkdir $DEMO_EDITOR_DIR
#mkdir $DEMO_ENGINE_DIR
#cp iCanScriptEditor.dll $DEMO_EDITOR_DIR
#cp iCanScriptEngine.dll $DEMO_ENGINE_DIR
#cp -r $EDITOR_DIR/Resources $DEMO_EDITOR_DIR
#rsync -av $EDITOR_PUBLIC_NODE_INSTALLER_DIR $DEMO_EDITOR_DIR >/dev/null
#rsync -av $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR $DEMO_EDITOR_DIR >/dev/null
#rsync -av $ENGINE_PUBLIC_COMPONENTS_DIR $DEMO_ENGINE_DIR >/dev/null
