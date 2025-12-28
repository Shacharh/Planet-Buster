using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.BubbleShooter.Demo
{
    public class MobileInputController : InputController
    {
        private void Update()
        {
            // UNITY TRICK: Input.mousePosition works on Android too!
            // It returns the position of the first finger touch.
            Vector3 inputPos = Input.mousePosition;

            // 1. HANDLE HOVER (Aiming)
            // On Android, we only want to aim if the user is actually touching the screen.
            // (Input.GetMouseButton(0) returns true if 1 or more fingers are touching)
            if (Input.GetMouseButton(0))
            {
                if (!IsTouchingUI())
                {
                    InvokeHover(inputPos);
                }
            }

            // 2. HANDLE PERFORM (Shooting)
            // Input.GetMouseButtonUp(0) returns true when the finger is lifted.
            if (Input.GetMouseButtonUp(0))
            {
                if (!IsTouchingUI())
                {
                    InvokePerform(inputPos);
                }
            }
        }

        /// <summary>
        /// This is the MAGIC FIX. 
        /// Instead of checking "currentSelectedGameObject" (which gets stuck),
        /// we check if the pointer is physically over UI right now.
        /// </summary>
        private bool IsTouchingUI()
        {
            if (EventSystem.current == null) return false;

            // Check if the mouse/finger is currently hovering over a UI element
            return EventSystem.current.IsPointerOverGameObject();
        }

        // We force this to true because we handle the safety checks manually above.
        protected override bool AllowInput() => true;
    }
}