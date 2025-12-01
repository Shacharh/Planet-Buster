#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.BubbleShooter.Editor
{
    /// <summary>
    /// Class that handles opening the editor window for the Bubble Shooter package.
    /// </summary>
    internal static class ReadMeOpener
    {
        /// <summary>
        /// Opens the readme for this package.
        /// </summary>
        [MenuItem("Tools/DTT/Minigame Bubble Shooter/ReadMe")]
        private static void OpenReadMe() => DTTEditorConfig.OpenReadMe("dtt.bubble-shooter-minigame");
    }
}
#endif