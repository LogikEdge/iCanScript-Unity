using UnityEngine;
using System.Collections;

namespace Subspace {
    /// Defines the possible evaluation states of an action for this run cycle.
    public enum SSEvaluationState {
        WAITING_FOR_ENABLE,     ///< Action is waiting for enables to stablize
        WAITING_FOR_INPUT,      ///< Action is waiting for inputs to stabilize
        DISABLED,               ///< Action has been evaluated and is disabled
        READY_TO_RUN,           ///< Action is ready to execute
        RAN                     ///< Action has ran in this run cycle
    }    
}
