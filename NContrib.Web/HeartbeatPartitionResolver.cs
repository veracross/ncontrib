using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using NLog;

namespace NContrib.Web {

    public class HeartbeatPartitionResolver : IPartitionResolver {

        protected static StateServer[] Providers;
        protected const int Timeout = 2000;
        protected readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(60);
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void Initialize() {

            // seems like IIS re-creates this object every time a session-requiring request is made
            // we don't want to overwrite the providers list if it's already populated
            // Providers is a class variable so it will persist
            if (Providers != null)
                return;

            var stateConfig = WebConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;

            Providers = stateConfig.StateConnectionString.Split(';').Select(StateServer.FromAddress).ToArray();

            Log.Info("Initialized HeartbeatPartitionProvider with {0} provider(s)", Providers.Length);
        }

        public string ResolvePartition(object key) {

            if (Providers.Length == 1) {
                Log.Debug("Only one state server is configured. Using " + Providers[0].Address);
                return Providers[0].Address;
            }

            var provider = Providers.FirstOrDefault(StateServerIsAlive);

            if (provider == null)
                throw new HttpUnhandledException("No alive state servers are available.");

            return provider.Address;
        }

        protected bool StateServerIsAlive(StateServer server) {

            if (server.NeedsUpdate(HeartbeatInterval)) {
                
                lock (Providers) {

                    Log.Debug("Checking availability of state server " + server.Address);

                    var result = Utilities.Network.CanConnect(server.Host, server.Port, TimeSpan.FromMilliseconds(Timeout));
                    server.IsAlive = result;
                    server.LastStatusCheck = DateTime.UtcNow;

                    if (result) {
                        Log.Debug("State server {0} is UP", server.Address);
                        server.LastKnownAlive = DateTime.UtcNow;
                    }
                    else {
                        Log.Warn("State server {0} is DOWN. Was last alive at {1}", server.Address, server.LastKnownAlive);
                    }
                }
            }

            return server.IsAlive;
        }
    }
}