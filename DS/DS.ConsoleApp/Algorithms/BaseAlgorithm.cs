using System;
using DS.Models;

namespace DS.ConsoleApp.Algorithms
{
    public abstract class BaseAlgorithm<T>
        where T: AlgorithmParameters
    {
        public abstract AlgorithmType Type { get; }

        public abstract void Execute(T parameters);

        protected DeterministicNModel1 GetModel(AlgorithmParameters parameters)
        {
            return new(parameters.N, parameters.I, parameters.Alpha, parameters.Px, parameters.Py);
        }

        protected string GetSaveDir()
        {
            return $"{Type}_{DateTime.Now:yyyy-MM-ddTHH-mm-ss}";
        }
    }
}
