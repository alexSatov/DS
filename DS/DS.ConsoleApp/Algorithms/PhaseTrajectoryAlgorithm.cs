using DS.Helpers;
using DS.Models;

namespace DS.ConsoleApp.Algorithms
{
    public class PhaseTrajectoryAlgorithm : BaseAlgorithm<PhaseTrajectoryParameters>
    {
        public override AlgorithmType Type => AlgorithmType.PhaseTrajectory;

        public override void Execute(PhaseTrajectoryParameters parameters)
        {
            var model = GetModel(parameters);
            var points = PhaseTrajectory.Get(model, parameters.Start, parameters.Skip, parameters.Get);

            points.SaveToFile($"{Type}.txt", GetSaveDir());
        }
    }
}
