using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DotPulsar.IntegrationTests;

public static class ProcessAsyncHelper
{
    public static async Task ThrowOnFailure(this Task<ProcessResult> resultTask)
    {
        var result = await resultTask;

        if (!result.Completed)
        {
            throw new InvalidOperationException($"Process did not complete correctly, {Environment.NewLine}{result.Output}");
        }
    }

    public static async Task LogFailure(this Task<ProcessResult> resultTask, Action<string> logAction)
    {
        var result = await resultTask;

        if (!result.Completed)
        {
            logAction(result.Output);
        }
    }

    public static async Task<ProcessResult> ExecuteShellCommand(string command, string arguments)
    {
        var result = new ProcessResult();

        using var process = new Process();

        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        var outputBuilder = new StringBuilder();
        var outputCloseEvent = new TaskCompletionSource<bool>();

        process.OutputDataReceived += (s, e) =>
        {
            // The output stream has been closed i.e. the process has terminated
            if (e.Data == null)
            {
                outputCloseEvent.SetResult(true);
            }
            else
            {
                outputBuilder.Append(e.Data);
            }
        };

        var errorBuilder = new StringBuilder();
        var errorCloseEvent = new TaskCompletionSource<bool>();

        process.ErrorDataReceived += (s, e) =>
        {
            // The error stream has been closed i.e. the process has terminated
            if (e.Data == null)
            {
                errorCloseEvent.SetResult(true);
            }
            else
            {
                errorBuilder.Append(e.Data);
            }
        };

        bool isStarted;

        try
        {
            isStarted = process.Start();
        }
        catch (Exception error)
        {
            // Usually it occurs when an executable file is not found or is not executable

            result.Completed = true;
            result.ExitCode = -1;
            result.Output = error.Message;

            isStarted = false;
        }

        if (isStarted)
        {
            // Reads the output stream first and then waits because deadlocks are possible
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Creates task to wait for process exit using timeout
            var waitForExit = WaitForExitAsync(process);

            // Create task to wait for process exit and closing all output streams
            await Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

            result.Completed = waitForExit.Result;
            result.ExitCode = process.ExitCode;
            // Adds process output if it was completed with error
            result.Output = process.ExitCode != 0 ? $"{outputBuilder}{errorBuilder}" : outputBuilder.ToString();
        }

        return result;
    }


    private static async Task<bool> WaitForExitAsync(Process process)
    {
        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }


    public struct ProcessResult
    {
        public bool Completed;
        public int? ExitCode;
        public string Output;
    }
}
