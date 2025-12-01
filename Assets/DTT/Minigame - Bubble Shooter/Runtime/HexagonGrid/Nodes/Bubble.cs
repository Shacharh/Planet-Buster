using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// This class holds properties for a bubble used in the grid of the playing field and as ammunition.
    /// </summary>
    public abstract class Bubble
    {
        /// <summary>
        /// The _matchers field holds predicate functions to determine whether this bubble matches another to
        /// recursively continue to pop.
        /// </summary>
        private Dictionary<Type, Func<Bubble, bool>> _matchers;

        /// <summary>
        /// The _connectMatchers field holds predicate functions to determine whether bubbles should initiate
        /// a pop upon connectin from the source to a destination.
        /// </summary>
        internal Dictionary<Type, Func<IEnumerable<Bubble>, bool>> i_connectMatchers;

        /// <summary>
        /// The ToPop event is invoked and used internally when the bubble has to be popped through the grid.
        /// </summary>
        internal event Action ToPop;

        /// <summary>
        /// The Popped event is invoked upon this bubbles popping in the grid.
        /// </summary>
        public event Action Popped;

        /// <summary>
        /// Initial position of the bubble.
        /// </summary>
        public Vector3 InitialPosition;

        /// <summary>
        /// Constructs a new bubble on a given cell.
        /// </summary>
        public Bubble()
        {
            _matchers = new Dictionary<Type, Func<Bubble, bool>>();
            i_connectMatchers = new Dictionary<Type, Func<IEnumerable<Bubble>, bool>>();
            InitialPosition = new Vector2(0, 0);
        }

        /// <summary>
        /// The AddMatch method adds a predicate function as a match for a type of bubble to match with this bubble
        /// and a predicate function that tests if the bubble should be part of a group and pop.
        /// </summary>
        /// <typeparam name="T">The type of bubble to check for a match.</typeparam>
        /// <param name="predicate">The predicate function that checks whether the bubbles match or not.</param>
        /// <param name="connectPredicate">The predicate function that checks if the bubble should pop the connected group.</param>
        protected void AddMatches<T>(Func<T, bool> predicate, Func<IEnumerable<T>, bool> connectPredicate) where T : Bubble
        {
            _matchers.Add(typeof(T), bubble => predicate.Invoke((T) bubble));
            i_connectMatchers.Add(typeof(T), bubbles => connectPredicate.Invoke(bubbles.Cast<T>()));
        }

        /// <summary>
        /// The InvokeToPop method notifies the listening grid that the bubble needs to be popped from the grid.
        /// </summary>
        public void InvokeToPop() => ToPop?.Invoke();

        /// <summary>
        /// The Pop method notifies listeners for the <see cref="Popped"/> event the bubble has popped.
        /// </summary>
        public void Pop() => Popped?.Invoke();

        /// <summary>
        /// The IsMatch method invokes matching predicate functions to test whether bubbles match with one another.
        /// </summary>
        /// <param name="other">The <see cref="Bubble"/> instance to check if it matches with this bubble.</param>
        /// <returns>True if the bubbles match. False if they do not match.</returns>
        public bool IsMatch(Bubble other) => _matchers[other.GetType()].Invoke(other);

        /// <summary>
        /// The IsConnectMatch method tests if this bubble is a successful poppable bubble in combination with another attached group.
        /// </summary>
        /// <param name="otherGroup">The other attached <see cref="IEnumerable{Bubble}"/>group to test for this bubble.</param>
        /// <returns>True if the bubble matches. False if they do not match.</returns>
        public bool IsConnectMatch(IEnumerable<Bubble> otherGroup) => i_connectMatchers[otherGroup.FirstOrDefault().GetType()].Invoke(otherGroup);

        /// <summary>
        /// The Clone method copies the properties of the instance.
        /// </summary>
        /// <returns>A new instance of <see cref="Bubble"/> with exact identical properties.</returns>
        public abstract Bubble Clone();
    }
}