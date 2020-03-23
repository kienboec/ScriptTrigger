using System.Net.Sockets;

namespace ScriptTrigger.BusinessLogic.ExecutionTriggerSource
{
    public static class ExecutionTriggerSourceOpenPort
    {
        public static bool CheckTrigger(string parameter)
        {
            using TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect("127.0.0.1", int.Parse(parameter));
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
