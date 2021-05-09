using DS.MathStructures;

namespace DS.ConsoleApp.Algorithms
{
    public class BifurcationDiagramParameters : PhaseTrajectoryParameters
    {
        public Interval<double> ParameterInterval { get; set; }
        public int ParameterRow { get; set; }
        public int ParameterColumn { get; set; }
        public int PointCount { get; set; }
        public bool SavePreviousPoint { get; set; }
    }
}
