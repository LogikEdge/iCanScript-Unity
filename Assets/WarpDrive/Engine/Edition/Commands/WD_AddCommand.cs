using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_AddCommand : WD_Command {
    public WD_AddCommand() {
        CommandType= WD_Command.CommandTypeEnum.Add;
    }
}
