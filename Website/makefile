# DEFAULT VALUES IF NOT SET IN ENVIRONMENT
ICANSCRIPT_DEV_ROOT    := ..
ICANSCRIPT_DEV_BIN     := $(ICANSCRIPT_DEV_ROOT)/bin
LOCAL_SERVER_DATA_ROOT ?= /Volumes/Service-Data/Library/Server

# BUILD USED FOLDER VARIABLES
LOCAL_SERVER_WEB_ROOT           := $(LOCAL_SERVER_DATA_ROOT)/Web/Data/Sites
LOCAL_SERVER_WEBSITE            := $(LOCAL_SERVER_WEB_ROOT)/www.icanscript.com
WEBSITE                    := www.icanscript.com

publish_to_local_web_server:
ifeq "$(wildcard $(LOCAL_SERVER_WEBSITE) )" ""
	@echo "Skipping publishing to local web server => not accessible"
else
	@echo "Publishing website to local web server"
	@rsync -r $(WEBSITE)/ $(LOCAL_SERVER_WEBSITE) >/dev/null
endif

