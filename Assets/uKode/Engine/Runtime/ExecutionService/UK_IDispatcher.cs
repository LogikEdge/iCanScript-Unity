using UnityEngine;
using System.Collections;

public interface UK_IDispatcher : UK_IAction {
    bool IsStalled { get; }
}
