using DTT.PublishingTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DTT.BubbleShooter.Editor
{
    [DTTHeader("dtt.bubble-shooter-minigame", "Bubble Shooter settings")]
    [CustomEditor(typeof(BubbleShooterConfig), true)]
    internal class BubbleShooterEditor : DTTInspector
    {
        /// <summary>
		/// Holds caches information of the fields in the associated <see cref="BubbleShooterConfig"/>.
		/// </summary>
		private BubbleShooterConfigCache _configCache;

        /// <summary>
		/// Represents the value whether the foldout of the general header is opened or not.
		/// </summary>
        private bool _generalFoldout;

        /// <summary>
		/// Represents the value whether the foldout of the grid header is opened or not.
		/// </summary>
        private bool _gridFoldout;

        /// <summary>
		/// Represents the value whether the foldout of the educative mode header is opened or not.
		/// </summary>
        private bool _educativeModeFoldout;

        /// <summary>
		/// Represents the value whether the foldout of the pool header is opened or not.
		/// </summary>
        private bool _poolFoldout;

		/// <summary>
		/// The OnEnable method initializes a property cache for the associated config and opens all foldout headers.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			_configCache = new BubbleShooterConfigCache(serializedObject);
			_generalFoldout = _gridFoldout = _educativeModeFoldout = _poolFoldout = true;
		}

		public override void OnInspectorGUI()
        {
			base.OnInspectorGUI();

			EditorGUI.BeginChangeCheck();

			_generalFoldout = EditorGUILayout.Foldout(_generalFoldout, "General");
			if (_generalFoldout)
				DrawUI(DrawGeneralUI);

			_gridFoldout = EditorGUILayout.Foldout(_gridFoldout, "Grid");
			if (_gridFoldout)
				DrawUI(DrawGridUI);

			_educativeModeFoldout = EditorGUILayout.Foldout(_educativeModeFoldout, "Educative mode");
			if (_educativeModeFoldout)
				DrawUI(DrawEducativeModeUI);

			_poolFoldout = EditorGUILayout.Foldout(_poolFoldout, "Pool");
			if (_poolFoldout)
				DrawUI(DrawPoolUI);

			if (EditorGUI.EndChangeCheck())
				_configCache.ApplyChanges();
        }

		private void DrawGeneralUI()
		{
			EditorGUILayout.PropertyField(_configCache.colorConfiguration, true);
			EditorGUILayout.PropertyField(_configCache.missedShotsTillNewRow, true);
			EditorGUILayout.PropertyField(_configCache.shotsTillNewRow, true);
		}

		private void DrawGridUI()
		{
			EditorGUILayout.PropertyField(_configCache.relativityMode, true);
			EditorGUILayout.PropertyField(_configCache.gridWidth, true);
			EditorGUILayout.PropertyField(_configCache.initialGridHeight, true);
			EditorGUILayout.PropertyField(_configCache.gridHeightThreshold, true);
		}

		private void DrawEducativeModeUI()
		{
			EditorGUILayout.PropertyField(_configCache.useEducativeElement, true);

			EditorGUI.BeginDisabledGroup(!_configCache.useEducativeElement.boolValue);
			EditorGUILayout.PropertyField(_configCache.minimumBubbleNumber, true);
			EditorGUILayout.PropertyField(_configCache.maximumBubbleNumber, true);
			EditorGUI.EndDisabledGroup();
		}

		private void DrawPoolUI() => EditorGUILayout.PropertyField(_configCache.chanceToPopThreshold, true);
		

		private void DrawUI(System.Action @delegate)
        {
			EditorGUI.indentLevel++;
			@delegate();
			EditorGUI.indentLevel--;
		}
	}
}