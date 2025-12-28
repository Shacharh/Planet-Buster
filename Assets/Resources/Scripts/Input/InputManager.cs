using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private Type _type;

        public enum Type { DESKTOP, MOBILE }

        private Dictionary<Type, System.Type> _componentAssociation;
        public InputController Controller { get; private set; }
        public event System.Action ControllerInitialized;

        private void Awake()
        {
            _componentAssociation = new Dictionary<Type, System.Type>();
            _componentAssociation.Add(Type.DESKTOP, typeof(DesktopInputController));
            _componentAssociation.Add(Type.MOBILE, typeof(MobileInputController));

            // Auto-switch to Mobile on Android
#if UNITY_ANDROID && !UNITY_EDITOR
                _type = Type.MOBILE;
#endif

            // Create the controller immediately
            Controller = gameObject.AddComponent(_componentAssociation[_type]) as InputController;
        }

        private void Start()
        {
            // FIX: We wait until Start() to tell other scripts we are ready.
            // This ensures TurretShooter has finished its OnEnable() and is listening.
            ControllerInitialized?.Invoke();
        }
    }
}