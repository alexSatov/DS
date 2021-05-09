using DS.Helpers;

namespace DS.ConsoleApp.Algorithms
{
    public class BifurcationDiagramAlgorithm : BaseAlgorithm<BifurcationDiagramParameters>
    {
        public override AlgorithmType Type => AlgorithmType.BifurcationDiagram;

        public override void Execute(BifurcationDiagramParameters parameters)
        {
            var model = GetModel(parameters);
            var diagramParameters = new DParams(parameters.ParameterInterval, parameters.ParameterRow,
                parameters.ParameterColumn, parameters.PointCount, parameters.SavePreviousPoint);

            var points = BifurcationDiagram.Get(model, diagramParameters, parameters.Start, parameters.Skip,
                parameters.Get);

            points.SaveToFile($"{Type}.txt", GetSaveDir());
        }
    }
}
