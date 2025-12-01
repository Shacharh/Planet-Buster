using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class MobileInputController : InputController
    {
        /// <summary>
        /// The Update method continuously checks for input on the mobile device.
        /// </summary>
        private void Update()
        {
            if (Input.touchCount == 0)
                return;

            Touch touch = Input.touches[0];
            Vector2 touchPosition = touch.position;

            InvokeHover(touchPosition);

            if (touch.phase == TouchPhase.Ended)
                InvokePerform(touchPosition);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected override bool AllowInput() => EventSystem.current.currentSelectedGameObject == null;
    }
}