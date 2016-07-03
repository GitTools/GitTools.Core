namespace GitTools
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    public static class ProcessHelper
    {
        static volatile object lockObject = new object();

        // http://social.msdn.microsoft.com/Forums/en/netfxbcl/thread/f6069441-4ab1-4299-ad6a-b8bb9ed36be3
        public static Process Start(ProcessStartInfo startInfo)
        {
            Process process;

            lock (lockObject)
            {
#if !NETSTANDARD
                using (new ChangeErrorMode(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox))
                {
#endif
                    try
                    {
                        process = Process.Start(startInfo);
                        process.PriorityClass = ProcessPriorityClass.Idle;
                    }
                    catch (Win32Exception exception)
                    {
                        // NOTE: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681382%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396 @asbjornu
                        if (exception.NativeErrorCode == 2)
                        {
                            throw new FileNotFoundException(string.Format("The executable file '{0}' could not be found.",
                                                                          startInfo.FileName),
                                                            startInfo.FileName,
                                                            exception);
                        }

                        throw;
                    }
#if !NETSTANDARD
                }
#endif
            }

            return process;
        }

        // http://csharptest.net/532/using-processstart-to-capture-console-output/
        public static int Run(Action<string> output, Action<string> errorOutput, TextReader input, string exe, string args, string workingDirectory, params KeyValuePair<string, string>[] environmentalVariables)
        {
            if (String.IsNullOrEmpty(exe))
                throw new ArgumentNullException("exe");
            if (output == null)
                throw new ArgumentNullException("output");

            workingDirectory = workingDirectory ?? Directory.GetCurrentDirectory();

            var psi = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
#if !NETSTANDARD
                WindowStyle = ProcessWindowStyle.Hidden,
                ErrorDialog = false,
#endif
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
                FileName = exe,
                Arguments = args
            };
            foreach (var environmentalVariable in environmentalVariables)
            {
#if NETSTANDARD
                if (psi.Environment.ContainsKey(environmentalVariable.Key))
                {
                    psi.Environment[environmentalVariable.Key] = environmentalVariable.Value;
                }
                else
                {
                    psi.Environment.Add(environmentalVariable.Key, environmentalVariable.Value);
                }
                if (psi.Environment.ContainsKey(environmentalVariable.Key) && environmentalVariable.Value == null)
                    psi.Environment.Remove(environmentalVariable.Key);
#else
                if (psi.EnvironmentVariables.ContainsKey(environmentalVariable.Key))
                {
                    psi.EnvironmentVariables[environmentalVariable.Key] = environmentalVariable.Value;
                }
                else
                {
                    psi.EnvironmentVariables.Add(environmentalVariable.Key, environmentalVariable.Value);
                }
                if (psi.EnvironmentVariables.ContainsKey(environmentalVariable.Key) && environmentalVariable.Value == null)
                    psi.EnvironmentVariables.Remove(environmentalVariable.Key);
#endif
            }

            using (var process = Start(psi))
            using (var mreOut = new ManualResetEvent(false))
            using (var mreErr = new ManualResetEvent(false))
            {
                process.EnableRaisingEvents = true;
                process.OutputDataReceived += (o, e) =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    if (e.Data == null)
                        mreOut.Set();
                    else
                        output(e.Data);
                };
                process.BeginOutputReadLine();
                process.ErrorDataReceived += (o, e) =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    if (e.Data == null)
                        mreErr.Set();
                    else
                        errorOutput(e.Data);
                };
                process.BeginErrorReadLine();

                string line;
                while (input != null && null != (line = input.ReadLine()))
                    process.StandardInput.WriteLine(line);

                process.StandardInput.Dispose();
                process.WaitForExit();

                mreOut.WaitOne();
                mreErr.WaitOne();

                return process.ExitCode;
            }
        }

        [Flags]
        public enum ErrorModes
        {
            Default = 0x0,
            FailCriticalErrors = 0x1,
            NoGpFaultErrorBox = 0x2,
            NoAlignmentFaultExcept = 0x4,
            NoOpenFileErrorBox = 0x8000
        }

#if !NETSTANDARD
        public struct ChangeErrorMode : IDisposable
        {
            readonly int oldMode;

            public ChangeErrorMode(ErrorModes mode)
            {
                try
                {
                    oldMode = SetErrorMode((int)mode);
                }
                catch (EntryPointNotFoundException)
                {
                    oldMode = (int)mode;
                }
            }


            void IDisposable.Dispose()
            {
                try
                {
                    SetErrorMode(oldMode);
                }
                catch (EntryPointNotFoundException)
                {
                    // NOTE: Mono doesn't support DllImport("kernel32.dll") and its SetErrorMode method, obviously. @asbjornu
                }
            }

            [DllImport("kernel32.dll")]
            static extern int SetErrorMode(int newMode);
        }
#endif
    }
}