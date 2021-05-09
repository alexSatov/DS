using DS.MathStructures;

namespace DS.ConsoleApp.Algorithms
{
    public class ModeMapParameters : PhaseTrajectoryParameters
    {
        public Interval<double> ParameterXInterval { get; set; }
        public int ParameterXRow { get; set; }
        public int ParameterXColumn { get; set; }
        public int PointCountX { get; set; }
        public Interval<double> ParameterYInterval { get; set; }
        public int ParameterYRow { get; set; }
        public int ParameterYColumn { get; set; }
        public int PointCountY { get; set; }
        public ByPreviousType SavePreviousPointType { get; set; }
        public double Eps { get; set; }
        public double PolarCenterX { get; set; }
        public double PolarCenterY { get; set; }
        public int PolarRayCount { get; set; }
    }
}
