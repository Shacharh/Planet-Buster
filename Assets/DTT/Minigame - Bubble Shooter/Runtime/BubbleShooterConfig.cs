using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// The class containing settings for a game of Bubble Shooter.
    /// </summary>
    [CreateAssetMenu(fileName = "Bubble Shooter config", menuName = "DTT/Mini Game/Bubble Shooter/Config")]
    public class BubbleShooterConfig : ScriptableObject
    {
        /// <summary>
        /// Color configuration of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Color configuration of the game, containing a color and a spawning weight.")]
        private ColorConfig[] _colorConfiguration;

        /// <summary>
        /// <inheritdoc cref="_colorConfiguration"/>
        /// </summary>
        public ColorConfig[] ColorConfiguration => _colorConfiguration;

        /// <summary>
        /// Indicates the amount of missed shots the turret has to shot until a new row appears.
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of missed shots the turret has to shot until a new row appears.")]
        private int _missedShotsTillNewRow = 5;

        /// <summary>
        /// <inheritdoc cref="_missedShotsTillNewRow"/>
        /// </summary>
        public int MissedShotsTillNewRow => _missedShotsTillNewRow;
        
        /// <summary>
        /// Indicates the amount of shots the turret has to shot until a new row appears.
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of shot the turret has to shot until a new row appears")]
        private int _shotsTillNewRow = 6;
        
        /// <summary>
        /// <inheritdoc cref="_shotsTillNewRow"/>
        /// </summary>
        public int ShotsTillNewRow => _shotsTillNewRow;

        /// <summary>
        /// The mode used to properly get adjacent cells in a specific offset hexagon grid.
        /// </summary>
        [SerializeField]
        [Tooltip("The mode used to properly get adjacent cells in a specific offset hexagon grid.")]
        private HexagonRelativityMode _relativityMode;

        /// <summary>
        /// <inheritdoc cref="_relativityMode"/>
        /// </summary>
        public HexagonRelativityMode RelativityMode => _relativityMode;

        /// <summary>
        /// Represents the amount of bubbles each row will have.
        /// </summary>
        [SerializeField]
        [Tooltip("Represents the amount of bubbles each row will have.")]
        private int _gridWidth = 12;

        /// <summary>
        /// <inheritdoc cref="_gridWidth"/>
        /// </summary>
        public int GridWidth => _gridWidth;

        /// <summary>
        /// The height of rows the grid of bubbles will be instantiated with.
        /// </summary>
        [SerializeField]
        [Tooltip("The height of rows the grid of bubbles will be instantiated with.")]
        private int _initialGridHeight = 8;

        /// <summary>
        /// <inheritdoc cref="_initialGridHeight"/>
        /// </summary>
        public int InitialGridHeight => _initialGridHeight;

        /// <summary>
        /// The height the grid has to reach for the game to be game over.
        /// </summary>
        [SerializeField]
        [Tooltip("The height the grid has to reach for the game to be game over.")]
        private int _gridHeightThreshold = 14;

        /// <summary>
        /// <inheritdoc cref="_gridHeightThreshold"/>
        /// </summary>
        public int GridHeightThreshold => _gridHeightThreshold;

        /// <summary>
        /// Indicates whether to use numbers for an educative way to play the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether to use numbers for an educative way to play the game.")]
        private bool _useEducativeElement = false;

        /// <summary>
        /// <inheritdoc cref="_useEducativeElement"/>
        /// </summary>
        public bool UseEducativeElement => _useEducativeElement;

        /// <summary>
        /// The minimum value a numbered bubble can have upon initalization of the grid.
        /// </summary>
        [SerializeField]
        [Tooltip("The minimum value a numbered bubble can have upon initalization of the grid.")]
        private int _minimumBubbleNumber = 1;

        /// <summary>
        /// <inheritdoc cref="_minimumBubbleNumber"/>
        /// </summary>
        public int MinimumBubbleNumber => _minimumBubbleNumber;

        /// <summary>
        /// The maximum value a numbered bubble can have upon initalization of the grid.
        /// </summary>
        [SerializeField]
        [Tooltip("The maximum value a numbered bubble can have upon initalization of the grid.")]
        private int _maximumBubbleNumber = 6;

        /// <summary>
        /// <inheritdoc cref="_maximumBubbleNumber"/>
        /// </summary>
        public int MaximumBubbleNumber => _maximumBubbleNumber;

        /// <summary>
        /// The minimum chance a bubble pair has to have from the bubble pool to be generated in the turret.
        /// </summary>
        [SerializeField]
        [Tooltip("The minimum chance a bubble pair has to have from the bubble pool to be generated in the turret.")]
        private float _chanceToPopThreshold = 50.0f;

        /// <summary>
        /// <inheritdoc cref="_chanceToPopThreshold"/>
        /// </summary>
        public float ChanceToPopThreshold => _chanceToPopThreshold;
    }
}