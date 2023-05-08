using Microsoft.CodeAnalysis.Tools.Utilities;

internal class Program
{
    private static int Main(string[] args)
    {

        try
        {
            throw new ExecutionEngineException();
        }
        catch
        {
            Console.WriteLine("Huch!");
        }



        // TryGetDotNetCliVersion(out string a);
        // Console.WriteLine(a);

        return 0;
    }


    internal static bool TryGetDotNetCliVersion(out string dotnetVersion)
    {
        var processInfo = ProcessRunner.CreateProcess("dotnet", "--version", captureOutput: true, displayWindow: false);
        var versionResult = processInfo.Result.GetAwaiter().GetResult();

        dotnetVersion = versionResult.OutputLines[0].Trim();
        return true;
    }
}
