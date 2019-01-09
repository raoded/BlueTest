using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueTest
{
    public interface IConnector
    {
        Task<bool> ConnectAsync();

        Task SendAsync(string message);

        Task<String> RecieveAsync();

        Task<bool> DisconnectAsync();

        bool IsConnected { get; }
    }
}
