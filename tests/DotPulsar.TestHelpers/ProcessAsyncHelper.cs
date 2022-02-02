/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.TestHelpers;

using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public static class ProcessAsyncHelper
{
    public static async Task ThrowOnFailure(this Task<ProcessResult> resultTask)
    {
        var result = await resultTask;

        if (!result.Completed)
            throw new InvalidOperationException($"Process did not complete correctly: {result.Output}");
    }

    public static async Task LogFailure(this Task<ProcessResult> resultTask, Action<string> logAction)
    {
        var result = await resultTask;

        if (!result.Completed)
            logAction(result.Output);
    }

    public static async Task<ProcessResult> ExecuteShellCommand(string command, string arguments)
    {
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
            if (e.Data is null)
                outputCloseEvent.SetResult(true);
            else
                outputBuilder.Append(e.Data);
        };

        var errorBuilder = new StringBuilder();
        var errorCloseEvent = new TaskCompletionSource<bool>();

        process.ErrorDataReceived += (s, e) =>
        {
            // The error stream has been closed i.e. the process has terminated
            if (e.Data is null)
                errorCloseEvent.SetResult(true);
            else
                errorBuilder.Append(e.Data);
        };

        var result = new ProcessResult();
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
