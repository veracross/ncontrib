using System;

namespace NContrib {

    /// <summary>
    /// Represents a Range (min &amp; max) of <see cref="IComparable"/> values that
    /// optionally include the min and max values.
    /// </summary>
    /// <typeparam name="T">Type that implements IComparable</typeparam>
    public struct Range<T> where T : IComparable {

        public T Min { get; private set; }
        public T Max { get; private set; }

        /// <summary>
        /// Indicates that a value is considered by <see cref="Includes(T)"/>
        /// to be in the Range if it is the minimum specified value of the range
        /// </summary>
        public bool IsMinInclusive { get; private set; }

        /// <summary>
        /// Indicates that a value is considered by <see cref="Includes(T)"/>
        /// to be in the Range if it is the maximum value of this range
        /// </summary>
        public bool IsMaxInclusive { get; private set; }

        /// <summary>
        /// Creates a Range which is optionally min and max inclusive
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Minimum value</param>
        /// <param name="minInclusive">Are values that match the Min are considered to be in the range?</param>
        /// <param name="maxInclusive">Are values that match the Max are considered to be in the range?</param>
        public Range(T min, T max, bool minInclusive = true, bool maxInclusive = true)
            : this() {
            Min = min;
            Max = max;
            IsMinInclusive = minInclusive;
            IsMaxInclusive = maxInclusive;
        }

        /// <summary>
        /// Tells if the value exists in this range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Includes(T value) {
            if (IsMinInclusive && value.CompareTo(Min) == 0)
                return true;

            if (IsMaxInclusive && value.CompareTo(Max) == 0)
                return true;

            // value is between the upper or lower limit
            return value.CompareTo(Min) == 1 && value.CompareTo(Max) == -1;
        }

        /// <summary>Formats the range in an easy to read way.</summary>
        /// <example>5 ≤ x &lt; 10</example>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0} {1} x {2} {3}",
                Min,
                IsMinInclusive ? '≤' : '<',
                IsMaxInclusive ? '≤' : '<',
                Max);
        }
    }
}
