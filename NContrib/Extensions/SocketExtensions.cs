using System;
using System.Net.Sockets;

namespace NContrib.Extensions {

    public static class SocketExtensions {

        /// <summary>
        /// Attempts connecting to the given host and port with a timeout. If the timeout limit is reached,
        /// the connection is dropped.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool CanConnect(this Socket socket, string host, int port, TimeSpan timeout) {

            var result = socket.BeginConnect(host, port, null, null);
            result.AsyncWaitHandle.WaitOne(timeout, true);

            var connected = socket.Connected;

            if (connected)
                socket.BeginDisconnect(true, ar => {}, null);

            return connected;
        }
    }
}
