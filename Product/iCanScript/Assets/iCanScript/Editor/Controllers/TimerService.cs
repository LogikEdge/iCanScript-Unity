using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;
using TimedAction= iCanScript.Internal.Prelude.TimerService.TimedAction;

namespace iCanScript.Internal.Editor {
    
    /// @date 2015-04-08    Code review.
    public static class TimerService {
        // ======================================================================
        // FIELD
        // ----------------------------------------------------------------------    
        static P.TimerService myTimerService= new P.TimerService(EditorTime);
    
        //=======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public static float EditorTime() { return (float)EditorApplication.timeSinceStartup; }
        public static float EngineTime() { return Time.realtimeSinceStartup; }
    
        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        static TimerService() {
            EditorApplication.update+= PeriodicUpdate;
        }
        public static void Start() {}
        public static void Shutdown() {
            EditorApplication.update-= PeriodicUpdate;
        }
        
        // ======================================================================
        // PUBLIC TIMED ACTION INTERFACE
        // ----------------------------------------------------------------------    
        /// Creates a timed action.
        ///
        /// @param time The delay before the action is invoked.
        /// @param action The action to be executed once the timer elapses.
        /// @param isLooping Set to _true_ for the timed action to auto-repeat.
        /// @return The created timed action.
        /// @note You must schedule the timed action for it to be activated.
        ///
        public static P.TimerService.TimedAction CreateTimedAction(float time, Action action, bool isLooping= false) {
            return myTimerService.CreateTimedAction(time, action, isLooping);
        }
    
        // ----------------------------------------------------------------------    
        /// Schedule a timed action into the timer service.
        ///
        /// @param timedAction The timed action to be scheduled.
        ///
        public static void Schedule(TimedAction timedAction) {
            myTimerService.Schedule(timedAction);
        }
        // ----------------------------------------------------------------------    
        /// Restart and reschedule the given timed action.
        ///
        /// @param timedAction The timed action to be restart and rescheduled.
        ///
        public static void Restart(TimedAction timedAction) {
            myTimerService.Restart(timedAction);
        }
    
        // ----------------------------------------------------------------------    
        /// Stops and removed a timed action from the timr service.
        ///
        /// @param timedAction The timed action to be stopped.
        ///
        public static void Stop(TimedAction timedAction) {
            myTimerService.Stop(timedAction);
        }
        
        // ----------------------------------------------------------------------    
        /// Returns the active status of the timed action.
        ///
        /// @param timedAction The timed action analysed.
        /// @return _true_ if the timed action is scheduled.  _false_ otherwise.
        ///
        public static bool IsActive(TimedAction timedAction) {
            return myTimerService.IsActive(timedAction);
        }
        
        // ======================================================================
        // TIMED ACTION UTILITIES
        // ----------------------------------------------------------------------
        /// Periodically verify for timers that have elapsed and run their
        /// associated action.
        static void PeriodicUpdate() {
            myTimerService.Update();
        }
    }
    
}