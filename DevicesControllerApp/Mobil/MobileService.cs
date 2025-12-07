using System;

namespace RehabilitationSystem.Mobile
{
    /// <summary>
    /// EngineAPI süreci, mobil komut sunucusu ve keşif servisinin tek elden yönetimi.
    /// </summary>
    internal class MobileService : IDisposable
    {
        private readonly EngineApiHost _apiHost;
        private readonly MobileCommandServer _commandServer;
        private readonly DiscoveryServer _discoveryServer;

        public EngineApiHost ApiHost => _apiHost;
        public MobileCommandServer CommandServer => _commandServer;
        public DiscoveryServer DiscoveryServer => _discoveryServer;

        public MobileService(int apiPort = 5086, int commandPort = 9000, int discoveryPort = 50500)
        {
            _apiHost = new EngineApiHost(apiPort);
            _commandServer = new MobileCommandServer(commandPort);
            _discoveryServer = new DiscoveryServer(discoveryPort, apiPort);
        }

        public bool StartAll(out string error)
        {
            error = string.Empty;
            if (!_apiHost.Start())
            {
                error = _apiHost.LastError;
                return false;
            }

            _commandServer.Start();
            _discoveryServer.Start();
            return true;
        }

        public void StopAll()
        {
            _commandServer.Stop();
            _discoveryServer.Stop();
            _apiHost.Stop();
        }

        public void Dispose()
        {
            StopAll();
            _apiHost.Dispose();
            _commandServer.Dispose();
            _discoveryServer.Dispose();
        }
    }
}
