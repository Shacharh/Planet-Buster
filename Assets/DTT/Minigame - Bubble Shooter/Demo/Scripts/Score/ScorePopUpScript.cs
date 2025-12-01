using UnityEngine;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// Handle the score popUp object.
    /// </summary>
    public class ScorePopUpScript : MonoBehaviour
    {
        /// <summary>
        /// Text element of the game object
        /// </summary>
        private Text _text;

        /// <summary>
        /// Time for the object to start disapearing
        /// </summary>
        private float _lifeTime;

        /// <summary>
        /// How fast the object start disapearing once he lived for disapearTIme.
        /// </summary>
        private float _fadingSpeed;

        /// <summary>
        /// How fast the object is going up.
        /// </summary>
        private float _moveSpeed;

        /// <summary>
        /// Get the text component
        /// </summary>
        private void Awake() =>
        _text = GetComponent<Text>();

        /// <summary>
        /// Set up the score to show in the popup.
        /// </summary>
        /// <param name="scoreAmount"></param>
        public void Setup(int scoreAmount, float fadingSpeed, float lifeTime, float moveSpeed)
        {
            _text.text = "+"+scoreAmount.ToString();
            _fadingSpeed = fadingSpeed;
            _lifeTime = lifeTime;
            _moveSpeed = moveSpeed;
        }

        /// <summary>
        /// Make the object move upward and disapear after a certain period of time.
        /// </summary>
        private void Update()
        {
            transform.position += new Vector3(0, _moveSpeed) * Time.deltaTime;
            _lifeTime -= Time.deltaTime;
            if (_lifeTime < 0)
            {
                //start disapear
                Color tempAlpha = _text.color;
                tempAlpha.a = _text.color.a - _fadingSpeed * Time.deltaTime;
                _text.color = tempAlpha;
                if (_text.color.a <= 0)
                    Destroy(this);
            }
        }
        
    }
}
