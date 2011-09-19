using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_ReplaceCommand : WD_Command {
    public WD_ReplaceCommand(WD_Object obj) {
        CommandType= WD_Command.CommandTypeEnum.Replace;
        InstanceId= obj.InstanceId;
    }
}
