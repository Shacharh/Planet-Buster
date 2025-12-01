using DTT.Utils.EditorUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DTT.BubbleShooter.Editor
{
    /// <summary>
	/// The PropertyCache class holds caches information of the fields in the associated <see cref="BubbleShooterConfig"/>.
	/// </summary>
    public class BubbleShooterConfigCache : SerializedPropertyCache
    {

	    /// <summary>
	    /// A color configuration for the game, holding a color and a spawning weight.
	    /// </summary>
	    public SerializedProperty colorConfiguration => base[nameof(colorConfiguration)];

        /// <summary>
        /// The relativityMode field is the mode used to properly get adjacent cells in a specific offset hexagon grid.
        /// </summary>
        public SerializedProperty relativityMode => base[nameof(relativityMode)];

        /// <summary>
        /// The gridWidth field represents the amount of bubbles each row will have.
        /// </summary>
        public SerializedProperty gridWidth => base[nameof(gridWidth)];

        /// <summary>
        /// The initialGridHeight is the height of rows the grid of bubbles will be instantiated with.
        /// </summary>
        public SerializedProperty initialGridHeight => base[nameof(initialGridHeight)];

        /// <summary>
        /// The gridHeightThreshold is the height the grid has to reach for the game to be game over.
        /// </summary>
        public SerializedProperty gridHeightThreshold => base[nameof(gridHeightThreshold)];

        /// <summary>
        /// The missedShotsTillNewRow field indicates the amount of missed shots the turret has to shot until a new row appears.
        /// </summary>
        public SerializedProperty missedShotsTillNewRow => base[nameof(missedShotsTillNewRow)];

        /// <summary>
        /// THe shotsTillNewRow field indicates the amount of shots the turret has to shot until a new row appears.
        /// </summary>
        public SerializedProperty shotsTillNewRow => base[nameof(shotsTillNewRow)];
        
        /// <summary>
        /// The useEducativeElement field indicates whether to use numbers for an educative way to play the game.
        /// </summary>
        public SerializedProperty useEducativeElement => base[nameof(useEducativeElement)];

        /// <summary>
        /// The minimumBubbleNumber field is the minimum value a numbered bubble can have upon initalization of the grid.
        /// </summary>
        public SerializedProperty minimumBubbleNumber => base[nameof(minimumBubbleNumber)];

        /// <summary>
        /// The maximumBubbleNumber field is the maximum value a numbered bubble can have upon initalization of the grid.
        /// </summary>
        public SerializedProperty maximumBubbleNumber => base[nameof(maximumBubbleNumber)];

        /// <summary>
        /// The chanceToPopThreshold is the minimum chance a bubble pair has to have from the bubble pool to be generated in the turret.
        /// </summary>
        public SerializedProperty chanceToPopThreshold => base[nameof(chanceToPopThreshold)];

        /// <summary>
		/// Constructs a new cache and allows access to associated fields.
		/// </summary>
		/// <param name="serializedObject">The serialized object that is being drawn in the editor.</param>
        public BubbleShooterConfigCache(SerializedObject serializedObject) : base(serializedObject) { }
    }
}