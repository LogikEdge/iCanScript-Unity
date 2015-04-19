# DEFAULT VALUES IF NOT SET IN ENVIRONMENT
ICANSCRIPT_DEV_ROOT    := ../../..
ICANSCRIPT_DEV_BIN     := $(ICANSCRIPT_DEV_ROOT)/bin
LOCAL_SERVER_DATA_ROOT ?= /Volumes/Service-Data/Library/Server

# BUILD USED FOLDER VARIABLES
LOCAL_SERVER_WEB_ROOT           := $(LOCAL_SERVER_DATA_ROOT)/Web/Data/Sites
LOCAL_SERVER_WEBSITE            := $(LOCAL_SERVER_WEB_ROOT)/www.icanscript.com
LOCAL_SERVER_WEBSITE_USER_GUIDE := $(LOCAL_SERVER_WEBSITE)/user-guide
DEV_WEBSITE                     := $(ICANSCRIPT_DEV_ROOT)/Website/www.icanscript.com
DEV_WEBSITE_USER_GUIDE          := $(DEV_WEBSITE)/user-guide

# TOOLS DEFINITIONS
MULTIMARKDOWN = /usr/local/bin/multimarkdown
MMD_TOC       = $(ICANSCRIPT_DEV_BIN)/mmd_toc.rb

# OBJECT DEFINITIONS
MD_SOURCE    := $(shell find . -type f -name "*.md")
TXT_SOURCE   := $(shell find . -type f -name "*.txt")
HTML_TARGET  := $(sort $(addsuffix .html, $(basename $(MD_SOURCE) $(TXT_SOURCE))))
DOC_SECTIONS := $(filter-out .,$(shell find . -type d -depth 1))

all: build_html publish_to_local_web_server

build_html: $(HTML_TARGET)

.PHONY: sync_website
publish_to_local_web_server:
ifeq "$(wildcard $(LOCAL_SERVER_WEBSITE_USER_GUIDE) )" ""
	@echo "Skipping publishing to local web server => not accessible"
else
	@echo "Publishing website documentation to local web server"
	$(foreach SECTION,$(DOC_SECTIONS),$(shell rsync -a -h $(SECTION)/ $(LOCAL_SERVER_WEBSITE_USER_GUIDE)/$(basename $(SECTION))))
endif

%.html: %.md
	@echo "Rebuilding =>" $@
	@$(MMD_TOC) $< | $(MULTIMARKDOWN) -o $@

%.html: %.txt
	@echo "Rebuilding =>" $@
	@$(MMD_TOC) $< | $(MULTIMARKDOWN) -o $@

