using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_ReplaceCommand : WD_Command {
    public WD_ReplaceCommand(int theInstanceId) {
        CommandType= WD_Command.CommandTypeEnum.Replace;
        InstanceId= theInstanceId;
    }
}
