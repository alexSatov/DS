using System.Linq;
using DS.MathStructures.Points;
using DS.Helpers;

namespace DS.ConsoleApp.Algorithms
{
    public class ModeMapAlgorithm : BaseAlgorithm<ModeMapParameters>
    {
        public override AlgorithmType Type => AlgorithmType.ModeMap;

        public override void Execute(ModeMapParameters parameters)
        {
            var model = GetModel(parameters);
            var diagramParameters = new D2Params(parameters.ParameterXInterval, parameters.ParameterYInterval,
                parameters.ParameterXRow, parameters.ParameterXColumn,
                parameters.ParameterYRow, parameters.ParameterYColumn,
                parameters.PointCountX, parameters.PointCountY, parameters.SavePreviousPointType);

            var result = parameters.SavePreviousPointType == ByPreviousType.Polar
                ? BifurcationDiagram.GetPolar(model, diagramParameters,
                    new PointD(parameters.PolarCenterX, parameters.PolarCenterY), parameters.PolarRayCount,
                    parameters.Start, parameters.Skip, parameters.Get, parameters.Eps)
                : BifurcationDiagram.Get(model, diagramParameters, parameters.Start, parameters.Skip, parameters.Get,
                    parameters.Eps);

            var attractors = result
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Params).ToList());

            attractors.SaveToDir(GetSaveDir());
        }
    }
}
