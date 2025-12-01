using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class DesktopInputController : InputController
    {
        /// <summary>
        /// The Update method continuously checks for input on the desktop device.
        /// </summary>
        private void Update()
        {
            Vector3 mousePosition = Input.mousePosition;

            InvokeHover(mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                InvokePerform(mousePosition);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected override bool AllowInput() => EventSystem.current.currentSelectedGameObject == null;
    }
}