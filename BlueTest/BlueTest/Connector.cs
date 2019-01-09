using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueTest
{
    public interface Connector
    {
        Task<bool> ConnectAsync();

        Task SendAsync(string message);

        Task<String> RecieveAsync();

        bool Disconnect();

        bool IsConnected { get; }
    }
}
