using System.Net.Sockets;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource
{
    public class ExecutionTriggerSourceOpenPort : IExecutionTriggerSource
    {
        public bool CheckTrigger(string parameter)
        {
            using TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect("127.0.0.1", int.Parse(parameter));
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
