using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_RemoveCommand : WD_Command {
    public WD_RemoveCommand(WD_Object obj) {
        CommandType= WD_Command.CommandTypeEnum.Remove;
        InstanceId= obj.InstanceId;
    }
}