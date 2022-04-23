using System;

namespace ozgurtek.framework.core.Data
{
    /// <summary>
    /// Specifies spatial geometric relations between two geometries.
    /// </summary>
    [Flags]
    public enum GdSpatialRelation
    {
        /// <summary>
        /// Test for disjointness.
        /// </summary>
        Disjoint = 0x0001,

        /// <summary>
        /// Test for intersection.
        /// </summary>
        Intersects = 0x0002,

        /// <summary>
        /// Test for touching.
        /// </summary>
        Touches = 0x0004,

        /// <summary>
        /// Test for crossing.
        /// </summary>
        Crosses = 0x0008,

        /// <summary>
        /// Test for containment.
        /// </summary>
        Within = 0x0010,

        /// <summary>
        /// Test for containment.
        /// </summary>
        Contains = 0x0020,

        /// <summary>
        /// Test for overlapping.
        /// </summary>
        Overlaps = 0x0040,

        /// <summary>
        /// Test for equal.
        /// </summary>
        Equals = 0x0080
    }
}
