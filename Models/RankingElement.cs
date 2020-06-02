using System;
using System.Collections.Generic;

namespace Lab2.Models
{
    class RankingElement<T>
    {
        public T Value { get; set; }
        public decimal Rank { get; set; }

        public override bool Equals(object obj)
        {
            return obj is RankingElement<T> element &&
                   EqualityComparer<T>.Default.Equals(Value, element.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(RankingElement<T> left, RankingElement<T> right)
        {
            return EqualityComparer<RankingElement<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(RankingElement<T> left, RankingElement<T> right)
        {
            return !(left == right);
        }
    }
}