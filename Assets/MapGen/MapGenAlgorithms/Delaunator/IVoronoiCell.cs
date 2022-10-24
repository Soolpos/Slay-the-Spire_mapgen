using System.Collections.Generic;

namespace DelaunayTriangulation 
{
    public interface IVoronoiCell
    {
        IPoint[] Points { get; }
        int Index { get; }
    }
}
