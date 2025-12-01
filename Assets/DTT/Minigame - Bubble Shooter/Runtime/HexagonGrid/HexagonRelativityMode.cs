using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// This enum contains modes on how the relativity of neighbouring cells in a hexagon grid relate.
    /// See also <see href="https://www.redblobgames.com/grids/hexagons/#coordinates-offset"/>.
    /// </summary>
    public enum HexagonRelativityMode
    {
        [InspectorName("Odd-r")]
        ODD_R,
        [InspectorName("Even-r")]
        EVEN_R,
        [InspectorName("Odd-q")]
        ODD_Q,
        [InspectorName("Even-q")]
        EVEN_Q
    }

    /// <summary>
    /// This class extends on the standard <see cref="HexagonRelativityMode"/> enum functionality.
    /// </summary>
    public static class HexagonRelativityModeExtensions
    {
        /// <summary>
        /// The GetOpposite method gives the altered mode of its main relativity.
        /// </summary>
        /// <param name="mode">The <see cref="HexagonRelativityMode"/> to get the opposite mode of.</param>
        /// <returns>A <see cref="HexagonRelativityMode"/> opposite of the given mode.</returns>
        /// <exception cref="System.NotImplementedException">Thrown if the given mode does not have an opposite mode.</exception>
        public static HexagonRelativityMode GetOpposite(this HexagonRelativityMode mode)
        {
            switch(mode)
            {
                case HexagonRelativityMode.ODD_R:
                    return HexagonRelativityMode.EVEN_R;
                case HexagonRelativityMode.EVEN_R:
                    return HexagonRelativityMode.ODD_R;
                case HexagonRelativityMode.ODD_Q:
                    return HexagonRelativityMode.EVEN_Q;
                case HexagonRelativityMode.EVEN_Q:
                    return HexagonRelativityMode.ODD_Q;
            }

            throw new System.NotImplementedException();
        }
    }
}