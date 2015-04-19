# TOOLS DEFINITIONS
SED= sed

# OBJECT DEFINITIONS
INPUT= iCanScript-User-Guide.tex/iCanScript-User-Guide.tex
OUTPUT= iCanScript-User-Guide.tex/iCanScript-User-Guide-out.tex

.PHONY: $(OUTPUT)
$(OUTPUT):
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{Drive-Download_48x48.png}/[keepaspectratio,width=48px,height=48px]{Drive-Download_48x48.png}/g" <$(INPUT) | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{icanscript-ui-tips_32x32x32.png}/[keepaspectratio,width=32px,height=32px]{icanscript-ui-tips_32x32x32.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{PostitNote_32x32.png}/[keepaspectratio,width=32px,height=32px]{PostitNote_32x32.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-1-orange.png}/[keepaspectratio,width=32px,height=32px]{stamp-1-orange.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-2-orange.png}/[keepaspectratio,width=32px,height=32px]{stamp-2-orange.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-3-orange.png}/[keepaspectratio,width=32px,height=32px]{stamp-3-orange.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-4-orange.png}/[keepaspectratio,width=32px,height=32px]{stamp-4-orange.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-1-green.png}/[keepaspectratio,width=32px,height=32px]{stamp-1-green.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-2-green.png}/[keepaspectratio,width=32px,height=32px]{stamp-2-green.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-3-green.png}/[keepaspectratio,width=32px,height=32px]{stamp-3-green.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{stamp-4-green.png}/[keepaspectratio,width=32px,height=32px]{stamp-4-green.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{UnityLogo_32x32.png}/[keepaspectratio,width=32px,height=32px]{UnityLogo_32x32.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{Chalkboard-ABCs_32x32.png}/[keepaspectratio,width=32px,height=32px]{Chalkboard-ABCs_32x32.png}/g" | \
	$(SED) "s/\[keepaspectratio\,width=\\\textwidth\,height=0.75\\\textheight\]{iCanScriptLogo_32x32x32.png}/[keepaspectratio,width=32px,height=32px]{iCanScriptLogo_32x32x32.png}/g" >$@


