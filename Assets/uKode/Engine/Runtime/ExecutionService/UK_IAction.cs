using UnityEngine;
using System.Collections;

public interface UK_IAction {
    void Execute(int frameId);
    void ForceExecute(int frameId);
    bool IsCurrent(int frameId);
    void MarkAsCurrent(int frameId);
}
