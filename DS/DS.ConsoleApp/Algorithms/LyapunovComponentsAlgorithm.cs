using DS.Helpers;

namespace DS.ConsoleApp.Algorithms
{
    public class LyapunovComponentsAlgorithm : BaseAlgorithm<LyapunovComponentsParameters>
    {
        public override AlgorithmType Type => AlgorithmType.LyapunovComponents;

        public override void Execute(LyapunovComponentsParameters parameters)
        {
            var model = GetModel(parameters);
            var diagramParameters = new DParams(parameters.ParameterInterval, parameters.ParameterRow,
                parameters.ParameterColumn, parameters.PointCount, parameters.SavePreviousPoint);

            var points = Lyapunov.Get(model, diagramParameters, parameters.Start, parameters.Eps, parameters.T);

            points.SaveToFile($"{Type}.txt", GetSaveDir());
        }
    }
}
