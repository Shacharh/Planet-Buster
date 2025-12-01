using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This interface can be used to invoke render responsibility of a <see cref="Bubble"/> and <see cref="BubbleController"/>.
    /// </summary>
    public interface IBubbleRenderer
    {
        /// <summary>
        /// The Render method renders the given <see cref="Bubble"/> instance on the <see cref="BubbleController"/>.
        /// </summary>
        /// <param name="bubble">The bubble to render on the given controller.</param>
        /// <param name="controller">The controller to render the given bubble for.</param>
        void Render(Bubble bubble, BubbleController controller);
    }
}