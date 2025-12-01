using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class shows the popup when necessary for when game's events arise.
    /// </summary>
    public class PopupManager : MonoBehaviour
    {
        /// <summary>
        /// The _manager field is a reference to the <see cref="BubbleShooterManager"/> instance used to listen to certain
        /// game flow events.
        /// </summary>
        [SerializeField]
        private BubbleShooterManager _manager;

        /// <summary>
        /// The _popup field references to the popup behaviour used when showing the popup.
        /// </summary>
        [SerializeField]
        private Popup _popup;

        /// <summary>
        /// The _pauseButton field is a <see cref="Button"/> reference used to pause the game and show the pause popup.
        /// </summary>
        [SerializeField]
        private Button _pauseButton;

        /// <summary>
        /// The OnEnable method attaches event listeners on when to show the paused/game results popup.
        /// </summary>
        private void OnEnable()
        {
            _manager.Paused += _popup.ShowPaused;
            _manager.Finish += _popup.ShowGameResults;
            _manager.Fail += _popup.ShowGameResults;

        }

        /// <summary>
        /// The OnDisable method detaches event listeners on when to show the paused/game results popup.
        /// </summary>
        private void OnDisable()
        {
            _manager.Paused -= _popup.ShowPaused;
            _manager.Finish -= _popup.ShowGameResults;
            _manager.Fail -= _popup.ShowGameResults;

        }
    }
}