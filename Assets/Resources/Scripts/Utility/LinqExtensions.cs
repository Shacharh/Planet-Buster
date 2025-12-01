using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo.Utility
{
    /// <summary>
    /// This class extends methods on the current implementation of <see cref="IEnumerable{T}"/> values.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// The MinBy method iterates over all elements and finds the entry with the lowest return value of the <paramref name="selector"/> function.
        /// </summary>
        /// <typeparam name="TSource">The type of elements contained within the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TKey">The type that is returned to compare with from the <paramref name="selector"/> function.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to iterate over.</param>
        /// <param name="selector">The selection function used to return the comparing value for each element.</param>
        /// <returns>The entry where the return value of the <paramref name="selector"/> function is the lowest.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) => source.MinBy(selector, null);

        /// <summary>
        /// The MinBy method iterates over all elements and finds the entry with the lowest return value of the <paramref name="selector"/> function.
        /// </summary>
        /// <typeparam name="TSource">The type of elements contained within the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TKey">The type that is returned to compare with from the <paramref name="selector"/> function.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to iterate over.</param>
        /// <param name="selector">The selection function used to return the comparing value for each element.</param>
        /// <param name="comparer">An optional <see cref="IComparer"/> to determine how comparisons are performed.</param>
        /// <returns>The entry where the return value of the <paramref name="selector"/> function is the lowest.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either the given <see cref="IEnumerable{T}"/> or selection function is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the given <see cref="IEnumerable{T}"/> contains no elements.</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            if (comparer == null)
                comparer = Comparer<TKey>.Default;

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");

                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);

                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }

                return min;
            }
        }
    }
}