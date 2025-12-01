using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class handles adding the right behaviour for retrieving input and passing it to an accessible level.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        /// <summary>
        /// The _type field indicates what platform of input is used for a running game.
        /// </summary>
        [SerializeField]
        private Type _type;

        /// <summary>
        /// This enum contains constraints for types of platforms the input manager supports.
        /// </summary>
        public enum Type
        {
            [InspectorName("Desktop")]
            DESKTOP,
            [InspectorName("Mobile")]
            MOBILE
        }

        /// <summary>
        /// The _componentAssociation field is a dictionary of component types associated by a platform type, used to
        /// add the correct component for a given platform type.
        /// </summary>
        private Dictionary<Type, System.Type> _componentAssociation;

        /// <summary>
        /// The Controller property is the currently assigned <see cref="InputController"/> instance for the selected
        /// platform.
        /// </summary>
        public InputController Controller { get; private set; }

        /// <summary>
        /// The ControllerInitialized event is invoked once the <see cref="Controller"/> for input is initialized.
        /// </summary>
        public event System.Action ControllerInitialized;

        /// <summary>
        /// The Awake method sets up component associations and configures the <see cref="Controller"/>.
        /// </summary>
        private void Awake()
        {
            _componentAssociation = new Dictionary<Type, System.Type>();
            _componentAssociation.Add(Type.DESKTOP, typeof(DesktopInputController));
            _componentAssociation.Add(Type.MOBILE, typeof(MobileInputController));

            Controller = gameObject.AddComponent(_componentAssociation[_type]) as InputController;
            ControllerInitialized.Invoke();
        }
    }
}