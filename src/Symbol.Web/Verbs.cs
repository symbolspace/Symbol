/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
namespace Symbol.Web {
    /// <summary>
    /// Http Methods
    /// </summary>
    [Flags]
    [Const("Http谓词")]
    public enum Verbs {
        /// <summary>
        /// Get Http Method
        /// </summary>
        [Const("GET")]
        Get = 1,
        /// <summary>
        /// Post Http Method
        /// </summary>
        [Const("POST")]
        Post = 2,
        /// <summary>
        /// Head Http Method
        /// </summary>
        [Const("HEAD")]
        Head = 4,
        /// <summary>
        /// Delete Http Method
        /// </summary>
        [Const("DELETE")]
        Delete = 8,
        /// <summary>
        /// Put Http Method
        /// </summary>
        [Const("PUT")]
        Put = 16,
        /// <summary>
        /// Options Http Method
        /// </summary>
        [Const("OPTIONS")]
        Options = 32,
        /// <summary>
        /// Copy Http Method
        /// </summary>
        [Const("COPY")]
        Copy = 64,
        /// <summary>
        /// Move Http Method
        /// </summary>
        [Const("MOVE")]
        Move = 128,
        /// <summary>
        /// All
        /// </summary>
        [Const("All")]
        All = Get | Post | Head | Delete | Put | Options | Copy | Move,
    }


}