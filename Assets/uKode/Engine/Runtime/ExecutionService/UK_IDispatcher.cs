using UnityEngine;
using System.Collections;

public interface UK_IDispatcher {
    UK_DispatcherBase GetDispatcher();
    bool IsStalled { get; }
    void Execute(int frameId);
    bool IsCurrent(int frameId);
}
