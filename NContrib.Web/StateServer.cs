using System;
using System.Text.RegularExpressions;

namespace NContrib.Web {

    public class StateServer {

        public string Address { get; protected set; }
        public string Host { get; protected set; }
        public int Port { get; protected set; }

        public DateTime LastKnownAlive { get; set; }
        public DateTime LastStatusCheck { get; set; }

        public bool IsAlive { get; set; }

        public StateServer(string host, int port) {
            Host = host;
            Port = port;
            Address = "tcpip=" + host + ":" + port;
        }

        public static StateServer FromAddress(string address) {
            var m = Regex.Match(address, @"^tcpip=([^:]+):(\d+)$");
            if (!m.Success)
                throw new ArgumentException("Invalid format. Expected: 'tcpip=0.0.0.0:42424'", address);

            return new StateServer(m.Groups[1].Value, int.Parse(m.Groups[2].Value));
        }

        public bool NeedsUpdate(TimeSpan heartbeatInterval) {
            return LastStatusCheck == default(DateTime) || DateTime.UtcNow - LastStatusCheck > heartbeatInterval;
        }
    }
}