using System.Diagnostics;

namespace KubernetesPodsAPI.Helpers
{
    public class RunProcess
    {
        public static void Make(string arg1, string arg2)
        {
            var processInfo = new ProcessStartInfo(arg1, arg2)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            int exitCode;
            using var process = new Process();

            process.StartInfo = processInfo;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit(1200000);
            if (!process.HasExited)
            {
                process.Kill();
            }

            exitCode = process.ExitCode;
        }
    }
}
