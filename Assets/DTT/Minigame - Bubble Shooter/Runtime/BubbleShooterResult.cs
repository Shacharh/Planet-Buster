using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// This container holds information of game results.
    /// </summary>
    public struct BubbleShooterResult
    {
        /// <summary>
        /// The timeTaken field indicates the time in seconds it took to finish a game.
        /// </summary>
        public readonly float timeTaken;

        /// <summary>
        /// The shotsFired field is the amount of times a bubble has been shot from the turret.
        /// </summary>
        public readonly int shotsFired;

        /// <summary>
        /// The amountOfMissedPops indicates the amount of times a bubble was shot, but made no bubbles pop.
        /// </summary>
        public readonly int amountOfMissedPops;

        /// <summary>
        /// The hasWon field indicates whether the game was won by the player or not.
        /// </summary>
        public readonly bool hasWon;

        /// <summary>
        /// Score of the game.
        /// </summary>
        public readonly int score;

        /// <summary>
        /// Constructs a new container with information about the game's results.
        /// </summary>
        /// <param name="timeTaken">The time it took to complete the game.</param>
        /// <param name="shotsFired">The amount of shots fired from the turret.</param>
        /// <param name="amountOfMissedPops">The amount of times a bubble was shot, but no bubbles popped.</param>
        /// <param name="hasWon">Whether the game was won by the player or not.</param>
        public BubbleShooterResult(float timeTaken, int shotsFired, int amountOfMissedPops, bool hasWon, int score)
        {
            this.timeTaken = timeTaken;
            this.shotsFired = shotsFired;
            this.amountOfMissedPops = amountOfMissedPops;
            this.hasWon = hasWon;
            this.score = score;
        }
    }
}