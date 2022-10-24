using System.Collections.Generic;

namespace DelaunayTriangulation 
{
    public interface ITriangle
    {
        IEnumerable<IPoint> Points { get; }
        int Index { get; }
    }
}
