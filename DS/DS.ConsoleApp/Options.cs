using CommandLine;

namespace DS.ConsoleApp
{
    public class Options
    {
        #region HelpText

        private const string AlgorithmHelpText = @"Algorithm number to execute:
1) Phase trajecory. Algorithm calculates attractor with fixed parameters. Synchronous.
Input parameters:
* N - model size;
* I - array of model parameters (size of N);
* Alpha - matrix of model parameters (size of NxN);
* Px - model parameter;
* Py - model parameter;
* Start - start point of attractor calculation;
* Skip - point count to skip (exclude from result attractor);
* Get - point count to get (include to result attractor).
2) Bifurcation diagram. Algorithm calculates attractors depending on the parameter (from Alpha matrix) change.
Input parameters:
* All parameters from p.1;
* ParameterInterval (Start, End): parameter value range for iteration;
* ParameterRow: row index (counting from zero) of parameter in Alpha matrix;
* ParameterColumn: column index (counting from zero) of parameter in Alpha matrix;
* PointCount: count of interval split points;
* SavePreviousPoint: flag which defines start point of next iteration (default start point or last point of previous attractor).
If true, algorithm will run synchronously, otherwise - parallel.
3) ModeMap. Algorithm calculates attractor types depending on the two parameter (X and Y, from Alpha matrix) change. Parallel.
Input parameters:
* All parameters from p.1;
* ParameterXInterval (Start, End): parameter value range for iteration (X parameter);
* ParameterXRow: row index (counting from zero) of parameter in Alpha matrix (X parameter);
* ParameterXColumn: column index (counting from zero) of parameter in Alpha matrix (X parameter);
* PointXCount: count of interval split points (X parameter);
* ParameterYInterval (Start, End): parameter value range for iteration (Y parameter);
* ParameterYRow: row index (counting from zero) of parameter in Alpha matrix (Y parameter);
* ParameterYColumn: column index (counting from zero) of parameter in Alpha matrix (Y parameter);
* PointYCount: count of interval split points (Y parameter);
* SavePreviousPointType: number which defines start point of next iteration
(0 - default, 1 - along X parameter, 2 - along Y parameter, 3 - polar, previous point will save along ray from fixed center point);
* Eps (epsilon): accuracy of determining the type of attractor;
* PolarCenterX (for SavePreviousPointType = 3): X component of fixed center point;
* PolarCenterY (for SavePreviousPointType = 3): Y component of fixed center point;
* PolarRayCount (for SavePreviousPointType = 3): ray count along an angle of 360 degrees.
4) LyapunovComponents. Algorithm calculates Lyapunov components depending on the parameter (from Alpha matrix) change.
Input parameters:
* All parameters from p.1 and p.2, excluding Skip and Get;
* Eps (epsilon): vector component shift;
* T: iteration count.

Examples of input parameters for each algorithm can be found here:
https://github.com/alexSatov/DS/tree/master/DS/DS.ConsoleApp
";

        private const string InputFilePathHelpText = @"Path to .json file with input parameters for chosen algorithm.";

        #endregion

        [Option('a', "algo", Required = true, HelpText = AlgorithmHelpText)]
        public int Algorithm { get; set; }

        [Option('i', "input", Required = true, HelpText = InputFilePathHelpText)]
        public string InputFilePath { get; set; }
    }
}
