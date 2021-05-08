using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using DS.ConsoleApp.Algorithms;
using Newtonsoft.Json;

namespace DS.ConsoleApp
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Console.WriteLine($"Processor count: {Environment.ProcessorCount}");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var algorithmType = (AlgorithmType) o.Algorithm;
                    var watch = Stopwatch.StartNew();
                    var exitCode = 0;

                    try
                    {
                        Execute(algorithmType, o.InputFilePath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unexpected error: {e.Message}");
                        exitCode = -1;
                    }
                    finally
                    {
                        Console.WriteLine($"Finished in {watch.Elapsed}");
                        Environment.Exit(exitCode);
                    }
                });
        }

        private static void Execute(AlgorithmType algorithmType, string inputFilePath)
        {
            switch (algorithmType)
            {
                case AlgorithmType.PhaseTrajectory:
                    var ptParameters = DeserializeFromFile<PhaseTrajectoryParameters>(inputFilePath);
                    var ptAlgorithm = new PhaseTrajectoryAlgorithm();
                    ptAlgorithm.Execute(ptParameters);
                    break;
                case AlgorithmType.BifurcationDiagram:
                    var bdParameters = DeserializeFromFile<BifurcationDiagramParameters>(inputFilePath);
                    var bdAlgorithm = new BifurcationDiagramAlgorithm();
                    bdAlgorithm.Execute(bdParameters);
                    break;
                case AlgorithmType.ModeMap:
                    var mmParameters = DeserializeFromFile<ModeMapParameters>(inputFilePath);
                    var mmAlgorithm = new ModeMapAlgorithm();
                    mmAlgorithm.Execute(mmParameters);
                    break;
                case AlgorithmType.LyapunovComponents:
                    var lcParameters = DeserializeFromFile<LyapunovComponentsParameters>(inputFilePath);
                    var lcAlgorithm = new LyapunovComponentsAlgorithm();
                    lcAlgorithm.Execute(lcParameters);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithmType), algorithmType,
                        $"Invalid algorithm type");
            }
        }

        private static T DeserializeFromFile<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
