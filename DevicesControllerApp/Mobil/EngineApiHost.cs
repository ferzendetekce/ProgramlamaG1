using System;
using System.Diagnostics;
using System.IO;

namespace RehabilitationSystem.Mobile
{
    /// <summary>
    /// EngineAPI ASP.NET Core servisinin dış süreç olarak başlatılmasını yönetir.
    /// </summary>
    internal class EngineApiHost : IDisposable
    {
        private readonly int _port;
        private Process _process;

        public string LastError { get; private set; } = string.Empty;
        public bool IsRunning => _process != null && !_process.HasExited;
        public int Port => _port;

        public EngineApiHost(int port = 5086)
        {
            _port = port;
        }

        public bool Start()
        {
            if (IsRunning)
            {
                LastError = string.Empty;
                return true;
            }

            var dllPath = FindDllPath();

            if (dllPath == null)
            {
                LastError = "EngineAPI.dll bulunamadı. Önce EngineAPI projesini build edin.";
                return false;
            }

            var apiDir = Path.GetDirectoryName(dllPath);

            try
            {
                var psi = new ProcessStartInfo("dotnet", $"\"{dllPath}\" --urls http://*:{_port}")
                {
                    WorkingDirectory = apiDir,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                _process = Process.Start(psi);
                LastError = _process != null ? string.Empty : "EngineAPI süreci başlatılamadı.";
                return _process != null;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                _process.Kill();
                _process.WaitForExit(3000);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        public void Dispose()
        {
            Stop();
            _process?.Dispose();
        }

        private string FindDllPath()
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // Önce doğrudan baseDir altındaki olası yolları dene
                var directCandidates = new[]
                {
                    Path.Combine(baseDir, "Mobil", "MobilUygulama", "EngineAPI", "EngineAPI.dll"),
                    Path.Combine(baseDir, "Mobil", "MobilUygulama", "EngineAPI", "bin", "Debug", "net9.0", "EngineAPI.dll"),
                    Path.Combine(baseDir, "Mobil", "MobilUygulama", "EngineAPI", "bin", "Release", "net9.0", "EngineAPI.dll"),
                };

                foreach (var candidate in directCandidates)
                {
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }

                // Klasörde yukarı doğru 4 seviye gezip arama
                var current = new DirectoryInfo(baseDir);
                for (int i = 0; i < 5 && current != null; i++)
                {
                    var rootPath = Path.Combine(current.FullName, "Mobil", "MobilUygulama", "EngineAPI");
                    if (Directory.Exists(rootPath))
                    {
                        var found = Directory.GetFiles(rootPath, "EngineAPI.dll", SearchOption.AllDirectories);
                        if (found.Length > 0)
                        {
                            return found[0];
                        }
                    }
                    current = current.Parent;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
