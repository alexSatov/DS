using CommandLine;

namespace DS.ConsoleApp
{
    public class Options
    {
        #region HelpText

        private const string AlgorithmHelpText = @"";

        private const string InputFilePathHelpText = @"Path to .json file with input parameters for chosen algorithm";

        #endregion

        [Option('a', "algo", Required = true, HelpText = AlgorithmHelpText)]
        public int Algorithm { get; set; }

        [Option('i', "input", Required = true, HelpText = InputFilePathHelpText)]
        public string InputFilePath { get; set; }
    }
}
