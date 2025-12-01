using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class handles input from a specific platform and invoke inputs from the input.
    /// </summary>
    public abstract class InputController : MonoBehaviour
    {
        /// <summary>
        /// The Hover event is invoked when a pointer is moved over the screen.
        /// </summary>
        public event Action<Vector2> Hover;

        /// <summary>
        /// The Perform event is invoked when a pointer performs its main action (clicking/tapping).
        /// </summary>
        public event Action<Vector2> Perform;

        /// <summary>
        /// The InvokeHover method invokes the <see cref="Hover"/> event if allowed by input.
        /// </summary>
        /// <param name="position">The position on the screen in pixels where the action was performed.</param>
        protected void InvokeHover(Vector2 position)
        {
            if (!AllowInput())
                return;

            Hover?.Invoke(position);
        }

        /// <summary>
        /// The InvokePerform method invokes the <see cref="Perform"/> event if allowed by input.
        /// </summary>
        /// <param name="position">The position on the screen in pixels where the action was performed.</param>
        protected void InvokePerform(Vector2 position)
        {
            if (!AllowInput())
                return;

            Perform?.Invoke(position);
        }

        /// <summary>
        /// The AllowInput method checks if input at the moment of calling this method is allowed.
        /// </summary>
        /// <returns>True if input is allowed. False if otherwise.</returns>
        protected abstract bool AllowInput();
    }
}