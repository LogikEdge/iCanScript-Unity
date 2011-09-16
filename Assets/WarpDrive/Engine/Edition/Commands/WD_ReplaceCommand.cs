using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_ReplaceCommand : WD_Command {
    public WD_ReplaceCommand(string theObjectId) {
        CommandType= WD_Command.CommandTypeEnum.Replace;
        ObjectId= theObjectId;
    }
}
