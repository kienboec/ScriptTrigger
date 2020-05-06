using System.Net.Sockets;
using ScriptTrigger.CLI.BusinessLogic.Infrastructure;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource
{
    public class ExecutionTriggerSourceOpenPort : IExecutionTriggerSource
    {
        public bool CheckTrigger(string parameter)
        {
            using TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect("127.0.0.1", int.Parse(parameter, Internationalization.DefaultCulture));
                tcpClient.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
