using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Corte_General.Publico;
using System.Timers;

namespace project1
{
    static public class DatosPublicosGenerales
    {
        static public List<string> IPs = new List<string>();
        static public string iDConexion { get; set; }
        static public string desarrollo { get; set; }
        static public string dirCortesSucursales { get; set; }
        static public string IPConsolidado { get; set; }
        static public string uidConsolidado { get; set; }
        static public string pswConsolidado { get; set; }
        static public string IPServer { get; set; }
        public static uint Port { get; private set; }
        static public string uidServer { get; set; }
        static public string pswServer { get; set; }
        static public string RutaDatos { get; set; }
        public static string SSH { get; internal set; }
        public static string Caja { get; set; }
        public static string SSHusr { get; set; }
        public static string SSHpass { get; set; }
        public static bool SinConexion { get; set; }
        public static int TiempoDeEsperaParaReconexion = 1000 * 60 * 10;//10 min
        
        
        public static PasswordConnectionInfo connectionInfo;
        public static SshClient client;
        public static bool x;
        public static ForwardedPortLocal portFwld;
        public static void ssh(string IP, string usuario, string pass)
        {
            connectionInfo = new PasswordConnectionInfo(IP, usuario, pass);
            connectionInfo.Timeout = TimeSpan.FromSeconds(30);
            client = new SshClient(connectionInfo);
            client.Connect();
            x = client.IsConnected;
            portFwld = new ForwardedPortLocal("127.0.0.1"/*your computer ip*/, IP /*server ip*/, 3306 /*server mysql port*/);
            client.AddForwardedPort(portFwld);
            portFwld.Start();
        }

        public static bool checkConnection(string IP, string usuario, string pass, string mysqlUsr, string mysqlPas)
        {
            try
            {
                uint port_default = 3306;
                try { port_default = uint.Parse(IP.Substring(IP.IndexOf(":") + 1)); } catch { }
                IP = IP.Contains(":") ? IP.Substring(0, IP.IndexOf(":")) : IP;
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(IP, usuario, pass);
                connectionInfo.Timeout = TimeSpan.FromSeconds(30);
                client = new SshClient(connectionInfo);
                client.Connect();
                x = client.IsConnected;
                portFwld = new ForwardedPortLocal("127.0.0.1"/*your computer ip*/, IP /*server ip*/, port_default /*server mysql port*/);
                client.AddForwardedPort(portFwld);
                portFwld.Start();
                string port = portFwld.BoundPort + "";
                MySqlConnection cone = new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Port=" + port + ";Database=dbo; Uid=" + mysqlUsr + "; Password=" + mysqlPas); //directo

                cone.Open();
                try
                {
                    portFwld.Stop();
                    client.Disconnect();
                }
                catch { }
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    portFwld.Stop();
                    client.Disconnect();
                }
                catch { }
                return false;
            }
        }
        public static bool trySetConnection(string IP, string usuario, string pass, string mysqlUsr, string mysqlPas)
        {

            try
            {
                uint port_default = 3306;
                try { port_default = uint.Parse(IP.Substring(IP.IndexOf(":") + 1)); } catch { }
                IP = IP.Contains(":") ? IP.Substring(0, IP.IndexOf(":")) : IP;
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(IP, usuario, pass);
                connectionInfo.Timeout = TimeSpan.FromSeconds(30);
                /*var*/
                client = new SshClient(connectionInfo);
                client.Connect();
                /*var*/
                x = client.IsConnected;
                /*ForwardedPortLocal*/
                portFwld = new ForwardedPortLocal("127.0.0.1"/*your computer ip*/, IP /*server ip*/, port_default /*server mysql port*/);
                client.AddForwardedPort(portFwld);
                portFwld.Start();
                string port = portFwld.BoundPort + "";
                MySqlConnection cone = new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Port=" + port + ";Database=dbo; Uid=" + mysqlUsr + "; Password=" + mysqlPas); //directo

                cone.Open();

                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    if (portFwld != null) { portFwld.Stop(); }
                    client.Disconnect();
                }
                catch { }
                return false;
            }
        }
    }
}
