lastReport=`ls releaseNotes | sort | sed "s/\.releaseNotes//" | tail -n 1`
latestTag=`git tag | tail -n 1`

# Generate written report if a new Tag exists
if [ -n "$1" ]; then
    echo Generating Release Notes from Head.
    git log ${lastReport}.. --oneline | grep "PUBLIC:" | sed "s/[0-9a-fA-F]* PUBLIC:[ ]*//"
else
    echo Last Report: ${lastReport}
    echo Latest Tag: ${latestTag}
    if [ ${lastReport} != ${latestTag} ]; then
        echo Generating a new written relase note for ${latestTag}.
        git log ${lastReport}..${latestTag} --oneline | grep "PUBLIC:" | sed "s/[0-9a-fA-F]* PUBLIC:[ ]*//" | tee releaseNotes/$2.releaseNotes
    else
        echo Report already generated for ${latestTag}.
    fi
fi
