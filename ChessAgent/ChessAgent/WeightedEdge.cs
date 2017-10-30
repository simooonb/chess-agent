using System;

namespace VacuumAgent
{
    public class WeightedEdge<T> : IComparable<WeightedEdge<T>>
    {
        public T Source { get; set; }
        public T Destination { get; set; }
        public long Weight { get; set; }

        public WeightedEdge(T src, T dest, long weight)
        {
            Source = src;
            Destination = dest;
            Weight = weight;
        }
        
        /// <summary>
        /// Allow comparaison between edges.
        /// </summary>
        /// <param name="other">The other WeightedEdge instance.</param>
        /// <returns>0 if both instances are equals, -1 if other > this, 1 otherwise.</returns>
        public int CompareTo(WeightedEdge<T> other)
        {
            if (other == null)
                return -1;

            // Check if nodes are equal
            if (Source.Equals(other.Source) && Destination.Equals(other.Destination))
                return Weight.CompareTo(other.Weight);
            
            return -1;
        }
    }
}