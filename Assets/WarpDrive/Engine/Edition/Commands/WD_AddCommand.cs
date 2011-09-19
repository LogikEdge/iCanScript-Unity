using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_AddCommand : WD_Command {
    public WD_AddCommand(WD_Object obj) : base(obj) {
        CommandType= WD_Command.CommandTypeEnum.Add;
    }
}
