# ============================================================================
# Mono Variables
APPLICATION_ROOT=/Applications
UNITY_APP_DIR=$APPLICATION_ROOT/Unity/Unity.app
MONO_ROOT=$UNITY_APP_DIR/Contents/Frameworks/Mono
MONO_BIN_DIR=$MONO_ROOT/bin
MONO_LIB_DIR=$MONO_ROOT/lib
MONO_ETC_DIR=$MONO_ROOT/etc

# ============================================================================
# Tools
GMCS=$MONO_BIN_DIR/gmcs
# Build Info Constants
BUILD_INFO_GENERATOR=./createBuildInfo
BUILD_INFO_CLASS=iCS_BuildInfo
BUILD_DATE_VAR=kBuildDateStr

# ============================================================================
# Constants
ROOT_DIR=../..
UNITY_DIR=$ROOT_DIR/iCanScript
ASSETS_DIR=$UNITY_DIR/Assets
ASSET_STORE_TOOLS_DIR=$ASSETS_DIR/AssetStoreTools
PRODUCT_DIR=$ASSETS_DIR/iCanScript
EDITOR_DIR=$PRODUCT_DIR/Editor
ENGINE_DIR=$PRODUCT_DIR/Engine
EDITOR_PUBLIC_EDITOR_WINDOWS_DIR=$EDITOR_DIR/EditorWindows
ENGINE_PUBLIC_COMPONENTS_DIR=$ENGINE_DIR/Components
STAGING_ROOT=$ROOT_DIR/../Staging
STAGING_ASSETS_DIR=$STAGING_ROOT/Assets
STAGING_PRODUCT_DIR=$STAGING_ASSETS_DIR/iCanScript
STAGING_EDITOR_DIR=$STAGING_PRODUCT_DIR/Editor
STAGING_ENGINE_DIR=$STAGING_PRODUCT_DIR/Engine
BUILD_INFO_FILE=$EDITOR_DIR/iCS_BuildInfo.cs

# ============================================================================
# Generate file list.
echo "Generating file list ..."
# Get a list of all files in the editor space
find $EDITOR_DIR -name "*.cs" >_editorFiles
# The engine files are all files outside the editor files.
find $PRODUCT_DIR -name "*.cs" | grep -v -f _editorFiles - >_engineFiles
# Build list of files to exclude from the editor space (compile)
find $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR -name "*.cs" >editorFilesToExclude
# Build list of files to exclude from the engine space (compile)
find $ENGINE_PUBLIC_COMPONENTS_DIR -name "*.cs" >engineFilesToExclude
# Exclude editor & engine files from compile
grep -v -f editorFilesToExclude _editorFiles >editorFiles
grep -v -f engineFilesToExclude _engineFiles >engineFiles

# ============================================================================
# Create compiler response files.
cat EditorCommandsCommunity editorFiles >iCanScriptEditorCommunity.rsp
cat EditorCommandsPro editorFiles >iCanScriptEditorPro.rsp
cat EngineCommands engineFiles >iCanScriptEngine.rsp

# ============================================================================
# Compile libraries.
echo "Creating Build Info file..."
$BUILD_INFO_GENERATOR $ENGINE_DIR $BUILD_INFO_CLASS $BUILD_DATE_VAR
echo "Compiling engine code..."
$GMCS @iCanScriptEngine.rsp
echo "Compiling editor code..."
$GMCS -d:COMMUNITY_EDITION @iCanScriptEditorCommunity.rsp
$GMCS -d:PRO_EDITION @iCanScriptEditorPro.rsp

# ============================================================================
# Run obfuscator.
#echo "Running obfuscator..."
#./obfuscate.sh

# ============================================================================
# Build for specific edition
function build_edition {
    local STAGING_ROOT=$ROOT_DIR/../Staging$1
    local STAGING_ASSETS_DIR=$STAGING_ROOT/Assets
    local STAGING_PRODUCT_DIR=$STAGING_ASSETS_DIR/iCanScript
    local STAGING_EDITOR_DIR=$STAGING_PRODUCT_DIR/Editor
    local STAGING_ENGINE_DIR=$STAGING_PRODUCT_DIR/Engine
    echo "Building edition " $1 " into " $STAGING_ROOT

    # ========================================================================
    # Create folders
    #echo "Creating folders for " $STAGING_ROOT
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
    
    # ========================================================================
    # Install dlls
    #echo "Installing into" $STAGING_PRODUCT_DIR "..."
    cp iCanScriptEngine.dll $STAGING_ENGINE_DIR
    cp iCanScriptEditor$1.dll $STAGING_EDITOR_DIR

    # ========================================================================
    # Install documentation
    if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.txt' -print -quit)"; then
    	#echo "Copying *.txt files into" $STAGING_PRODUCT_DIR
    	cp $PRODUCT_DIR/*.txt $STAGING_PRODUCT_DIR
    fi
    if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.pdf' -print -quit)"; then
    	#echo "Copying *.pdf files into" $STAGING_PRODUCT_DIR
    	cp $PRODUCT_DIR/*.pdf $STAGING_PRODUCT_DIR
    fi
    if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.md' -print -quit)"; then
    	#echo "Copying *.md files into" $STAGING_PRODUCT_DIR
    	cp $PRODUCT_DIR/*.md $STAGING_PRODUCT_DIR
    fi
    if test -n "$(find $PRODUCT_DIR -maxdepth 1 -name '*.html' -print -quit)"; then
    	#echo "Copying *.html files into" $STAGING_PRODUCT_DIR
    	cp $PRODUCT_DIR/*.html $STAGING_PRODUCT_DIR
    fi

    # ========================================================================
    # Install visible source files.
    cp -r $EDITOR_DIR/Images $STAGING_EDITOR_DIR
    rsync -av $EDITOR_PUBLIC_NODE_INSTALLER_DIR $STAGING_EDITOR_DIR >/dev/null
    rsync -av $EDITOR_PUBLIC_EDITOR_WINDOWS_DIR $STAGING_EDITOR_DIR >/dev/null
    rsync -av $ENGINE_PUBLIC_COMPONENTS_DIR $STAGING_ENGINE_DIR >/dev/null
    rsync -av $ENGINE_PUBLIC_NODES_DIR $STAGING_ENGINE_DIR >/dev/null
}
build_edition Community
build_edition Pro

# ============================================================================
# install Asset Store Tools
function install_asset_store_tools {
    local STAGING_ROOT=$ROOT_DIR/../Staging$1
    local STAGING_ASSETS_DIR=$STAGING_ROOT/Assets

    echo "Installing Unity Asset Store Tools into " $STAGING_ASSETS_DIR
    rsync -av $ASSET_STORE_TOOLS_DIR $STAGING_ASSETS_DIR >/dev/null
}
install_asset_store_tools Pro
install_asset_store_tools Community
