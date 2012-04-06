using System;
using System.Net.Sockets;
using NContrib.Extensions;

namespace NContrib.Utilities {

    public static class Network {

        public static bool CanConnect(string host, int port, int timeoutMilliseconds, ProtocolType protocolType = ProtocolType.Tcp) {
            return CanConnect(host, port, TimeSpan.FromMilliseconds(timeoutMilliseconds), protocolType);
        }

        public static bool CanConnect(string host, int port, TimeSpan timeout, ProtocolType protocolType = ProtocolType.Tcp) {
            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocolType)) {
                return sock.CanConnect(host, port, timeout);
            }
        }
    }
}
