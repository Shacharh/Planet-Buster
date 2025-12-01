using DTT.MinigameBase.LevelSelect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class sets up the level selection for the Bubble Shooter minigame.
    /// </summary>
    public class BubbleShooterLevelSelectHandler : LevelSelectHandler<BubbleShooterConfig, BubbleShooterResult, BubbleShooterManager>
    {
        /// <summary>
        /// The _levelConfigs field contains the configurable levels for each level for level selection.
        /// </summary>
        [SerializeField]
        private BubbleShooterConfig[] _levelConfigs;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="result"><inheritdoc/></param>
        protected override float CalculateScore(BubbleShooterResult result)
        {
            bool perfectPops = result.amountOfMissedPops == 0;

            if (perfectPops && result.hasWon)
                return 1f;

            if (result.hasWon)
                return 0.6f;

            return 0.3f;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="levelNumber"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        protected override BubbleShooterConfig GetConfig(int levelNumber) => _levelConfigs[Mathf.Clamp(levelNumber, 0, _levelConfigs.Length - 1)];
    }
}