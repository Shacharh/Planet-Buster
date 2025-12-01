using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// The class that handles the logic flow for turret actions.
    /// </summary>
    public class Turret
    {
        /// <summary>
        /// The _bubble field holds the currently loaded <see cref="BubbleShooter.Bubble"/> instances that acts as ammunition.
        /// </summary>
        private Bubble _bubble;

        /// <summary>
        /// The Bubble property holds the currently loaded <see cref="BubbleShooter.Bubble"/> instance that acts as ammunition.
        /// </summary>
        public Bubble Bubble => _bubble;
        
        /// <summary>
        /// The canShoot property indicates whether the turret can be fired in its current state.
        /// </summary>
        public bool canShoot;

        /// <summary>
        /// The Shot event is invoked upon the turret shooting a bubble into a given direction.
        /// </summary>
        public event Action<Bubble, Vector2> Shot;

        /// <summary>
        /// The Reloaded event is invoked once a bubble is put in the turret as the next shot.
        /// </summary>
        public event Action<Bubble> Reloaded;

        /// <summary>
        /// The i_Shots property indicates the amount of shots the turret has fired.
        /// </summary>
        internal int i_Shots { get; private set; }

        /// <summary>
        /// The i_missedShots property is the amount of shots the turret missed before adding a new row.
        /// </summary>
        internal int i_missedShots { get; set; }

        /// <summary>
        /// The i_totalMissedShots indicates the amount of total shots the turret missed.
        /// </summary>
        internal int i_totalMissedShots { get; set; }

        /// <summary>
        /// Constructs a new turret.
        /// </summary>
        public Turret() => canShoot = true;

        /// <summary>
        /// The Shoot method shoots a bubble in a certain direction and consumes the current <see cref="BubbleShooter.Bubble"/> ammunition.
        /// </summary>
        /// <param name="direction">The direction the bubble is shot into.</param>
        internal void Shoot(Vector2 direction)
        {
            i_Shots++;

            Shot?.Invoke(Bubble, direction);
            _bubble = null;

            canShoot = false;
        }

        /// <summary>
        /// The Reload method grabs a new <see cref="BubbleShooter.Bubble"/> instance and sets it as ammunition to the turret.
        /// </summary>
        /// <param name="bubble">The <see cref="BubbleShooter.Bubble"/> to be used as ammunition.</param>
        public void Reload(Bubble bubble)
        {
            _bubble = bubble;
            Reloaded?.Invoke(bubble);
        }
    }
}