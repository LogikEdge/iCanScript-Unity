using UnityEngine;
using System.Collections;

namespace iCanScript.Editor {
    
    /// <summary>
    /// The parameter direction enumeration is used to describe the data flow direction
    /// of a port on a node.  It is also used to describe the direction of
    /// <b>variables</b> and parameters.
    /// 
    /// <ul>
    /// 	<li>bullet 1</li>
    /// 	<li>bullet 2</li>
    /// </ul>
    /// </summary>
    [System.Serializable] public enum iCS_ParamDirection : int {


    	/// <summary>
    	/// Defines a parameter used as input.
    	/// </summary>
    	In,
    	/// <summary>
    	/// Defines a parameter used as output.
    	/// </summary>
    	Out,
    	/// <summary>
    	/// Defines a parameter used as both an input and an output.
    	/// </summary>
    	InOut


    }

}
