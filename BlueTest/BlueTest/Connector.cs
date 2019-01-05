using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueTest
{
    public interface Connector
    {
        bool connect();

        void Send(string message);

        Task<String> RecieveAsync();
    }
}
