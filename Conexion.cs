using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Corte_General.Publico
{
    public class Conexion
    {
        /* Esta clase se usa en:
         * reimpresionTickets
         * */
        /* MySQL */
        public MySqlCommand MySqlCmd = new MySqlCommand();

        public string getCadenaConexion()
        {
            return _MySqlCnn.ConnectionString;
        }

        public static string Usr_Default = DatosPublicosGenerales.uidServer, Psw_Default = DatosPublicosGenerales.pswConsolidado, Db_Default = "dbo";
        //public static object Port_Default = 3306;
        static public MySqlConnection MySqlCnn_st { get; set; }
        public MySqlConnection _MySqlCnn { get; set; }
        public dynamic Sqlite { get; private set; }
        static bool Ejecutar_Query(Querys Consulta_ejecutar)
        {/* //Ver este metodo por el Transaction.
            bool Estado = false;
            using (MySqlConnection con = Consulta_ejecutar.Conexion())
            {
                try
                {
                    con.Open();
                    using (MySql.Data.MySqlClient.MySqlTransaction trans = con.BeginTransaction())
                    {

                        try
                        {
                            //command to executive query
                            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySqlCommand(Consulta_ejecutar.Consulta, con, trans))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();
                            Consultas.RemoveAt(0);
                        }
                        catch
                        {
                            trans.Rollback();
                            ErrConsultas.Add(Consultas[0]);
                            Consultas.RemoveAt(0);
                        }
                    }
                }*/
            /*catch (MySqlException ex)
            {

                switch (ex.Number)
                {
                    case 1042:
                        {
                            Consultas.Add(Consultas[0]);
                            Consultas.RemoveAt(0);
                        }
                        break;
                    case 0:
                        {
                            ErrConsultas.Add(Consultas[0]);
                            Consultas.RemoveAt(0);
                        }
                        break;
                }
            }
        }
        return Estado;*/
            return false;
        }
        public Conexion(bool connect = true)
        {
            if (connect)
            {
                ObtenerConexion();
                Sqlite = DatosPublicosGenerales.ClasesRey_Exchange.getSqlite();
            }
        }
        /* Obtiene la conexion y la asigna a la estatica */
        public MySqlConnection ObtenerConexion()
        {
            //MySqlCnn = DatosCajeroSucursalOperacion.MySqlCnn;
            _MySqlCnn = ObtenerConexion(Db_Default);
            return _MySqlCnn;
        }
        public MySqlConnection ObtenerConexion(string dbo)
        {
            //MySqlCnn = DatosCajeroSucursalOperacion.MySqlCnn;
            _MySqlCnn = ObtenerConexionTrySSH(DatosPublicosGenerales.IPServer, DatosPublicosGenerales.uidServer, DatosPublicosGenerales.pswServer, dbo);
            return _MySqlCnn;
        }
        public MySqlConnection ObtenerConexionConsolidado()
        {
            _MySqlCnn = ObtenerConexionConsolidado(Db_Default);
            return _MySqlCnn;
        }
        public MySqlConnection ObtenerConexionConsolidado(string dbo)
        {
            _MySqlCnn = ObtenerConexionTrySSH(DatosPublicosGenerales.IPConsolidado, DatosPublicosGenerales.uidConsolidado, DatosPublicosGenerales.pswConsolidado, dbo);
            return _MySqlCnn;
        }
        public MySqlConnection ObtenerConexionTrySSH(string IP, string USR, string PSW, string dbo)
        {
            //MySqlCnn = DatosCajeroSucursalOperacion.MySqlCnn;
            _MySqlCnn = new MySqlConnection(
                "Server=" + (DatosPublicosGenerales.SSH == "S" ? "127.0.0.1" : IP) +
                "; " + "Port=" + (DatosPublicosGenerales.SSH == "S" ? DatosPublicosGenerales.portFwld.BoundPort + ";" : DatosPublicosGenerales.Port + ";") +
                "Database=" + dbo + ";" + "Uid=" + USR + ";" + "Password=" + PSW + "; Convert Zero Datetime=True; Command Timeout=1288000;");
            MySqlCnn_st = _MySqlCnn;
            return _MySqlCnn;
        }
        /* Obtiene la conexion y solo la usa en la instancia */
        public MySqlConnection ObtenerConexionWith(string IP)
        {
            //Añadir case para un usuario, contraseña, o puerto distinto
            _MySqlCnn = ObtenerConexionWith(IP, Usr_Default, Psw_Default, Db_Default, DatosPublicosGenerales.Port);

            return _MySqlCnn;
        }
        public MySqlConnection ObtenerConexionWith(string IP, string dbo)
        {
            //Añadir case para un usuario, contraseña, o puerto distinto
            _MySqlCnn = ObtenerConexionWith(IP, Usr_Default, Psw_Default, dbo, DatosPublicosGenerales.Port);

            return _MySqlCnn;
        }
        public MySqlConnection ObtenerConexionWith(string IP, string USR, string PSW, string DB, object PORT)
        {
            _MySqlCnn = new MySqlConnection("Server=" + IP + "; " + "Port=" + PORT + ";" + "Database=" + DB + ";" + "Uid=" + USR + ";" + "Password=" + PSW + "; Convert Zero Datetime=True; Command Timeout=1288000;");

            return _MySqlCnn;
        }

        public static bool RevisarConexiones()
        {
            bool conectado = false;
            foreach (string ip in DatosPublicosGenerales.IPs)
            {
                conectado = RevisarConexion(ip);
                if (conectado)
                {
                    DatosPublicosGenerales.IPServer = ip;
                    return conectado;
                }
            }
            return conectado;
        }
        public static bool RevisarConexion(string ip)
        {
            return RevisarConexion(ip, Usr_Default, Psw_Default, Db_Default);
        }
        public static bool RevisarConexion(string ip, string usr, string pas, string dba)
        {
            bool conectado = false;
            try
            {
                string ip_temp = ip.Contains(":") ? ip.Substring(0, ip.IndexOf(":")) : ip;
                uint port = 3306; try { port = uint.Parse(ip.Substring(ip.IndexOf(":") + 1)); } catch (Exception) { }
                MySqlConnection MySqlCnn_Temp = new MySqlConnection("Server=" + ip_temp + "; " + "Port=" + port + ";" + "Database=" + dba + ";" + "Uid=" + usr + ";" + "Password=" + pas + "; Convert Zero Datetime=True; Command Timeout=128800;");
                MySqlCnn_Temp.Open();
                if (MySqlCnn_Temp.State == ConnectionState.Open)
                {
                    MySqlCnn_Temp.Close();
                    conectado = true;
                }
            }
            catch (Exception ex)
            {
                ;
            }
            return conectado;
        }
        public static bool RevisarConexion()
        {
            bool conectado = false;
            try
            {
                MySqlCnn_st.TryConnect();
                if (MySqlCnn_st.State == ConnectionState.Open)
                {
                    MySqlCnn_st.Close();
                    conectado = true;
                }
            }
            catch { }
            finally { try { MySqlCnn_st.Close(); } catch { } }
            return conectado;
        }
        public bool HayConexion()
        {
            bool conectado = false;
            try
            {
                _MySqlCnn.TryConnect();
                if (_MySqlCnn.State == ConnectionState.Open)
                {
                    _MySqlCnn.Close();
                    conectado = true;
                }
            }
            catch { }
            return conectado;
        }
        public DataSet CargarSelect(string CON)
        {
            return
               CargarSelect(CON, true, false);
        }
        public DataSet CargarSelect(string CON, bool mostrarMensaje, bool soloMySQL = false)
        {
            DataSet dt = new DataSet();
            bool consultaNoExitosa = true;
            if (!DatosPublicosGenerales.SinConexion || soloMySQL)
            {
                try
                {
                    string ConsultaProductos = CON;
                    MySqlCmd = new MySqlCommand(ConsultaProductos, _MySqlCnn);
                    MySqlDataAdapter da = new MySqlDataAdapter(MySqlCmd);
                    da.Fill(dt);
                    consultaNoExitosa = false;
                }
                catch (Exception ex)
                {
                    if (!esErrorPorConexion_sincroniza(ex))
                    {
                        if (mostrarMensaje) { MessageBox.Show(ex.Message); }
                    }
                }
                finally { _MySqlCnn.Close(); }
            }
            if (DatosPublicosGenerales.SinConexion || consultaNoExitosa && !soloMySQL)
            { try { dt = Sqlite.ExecuteDS(CON); consultaNoExitosa = false; } catch { } }
            if (consultaNoExitosa && mostrarMensaje) { MessageBox.Show("La operación no se puede llevar a cabo, Reportarlo al departamento de ingenieria", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            return dt;
        }
        public int InsertarUpdate(string CON)
        {
            int resultado = -1;
            try
            {
                _MySqlCnn.TryConnect();
                string ConsultaProductos = CON; 
                MySqlCmd = new MySqlCommand(ConsultaProductos, _MySqlCnn);
                resultado = MySqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                esErrorPorConexion_sincroniza(ex);

            }
            return resultado;
        }

        public int NewCommand(string sql, object[,] args)
        {
            int resultado = -1;
            _MySqlCnn.TryConnect();
            try
            {
                MySqlCmd = _MySqlCnn.CreateCommand();
                MySqlCmd.CommandText = sql;
                for (int i = 0; i < args.Length / 2; i++)
                //foreach (var parametro in args)
                {
                    dynamic p = new object[] { args[i, 0], args[i, 1] };
                    //dynamic p = parametro;
                    MySqlCmd.Parameters.AddWithValue(p[0], p[1]);
                }
                resultado = MySqlCmd.ExecuteNonQuery();
            }
            catch (Exception x)
            {
                esErrorPorConexion_sincroniza(x);

                MessageBox.Show(x.Message);
            }
            finally
            {
                _MySqlCnn.Close();
            }
            return resultado;
        }
        public static DateTime TryGetDBDateTime()
        {
            try
            {
                foreach (DataRow r in new Conexion().CargarSelect("Select now();").Tables.First().Rows)
                {
                    return DateTime.Parse(r.ItemArray.First().ToString());
                }
            }
            catch
            { }
            return DateTime.Now;
        }
        public static bool esErrorPorConexion_sincroniza(Exception ex)
        {
            if (ex is MySqlException)
            {
                MySqlException mySqlException = (MySqlException)ex;
                MySqlErrorCode errorCode;
                if (Enum.TryParse(mySqlException.Number.ToString(), false, out errorCode))
                {
                    if (errorCode == MySqlErrorCode.DatabaseAccessDenied || errorCode == MySqlErrorCode.UnableToConnectToHost)
                    {
                        DatosPublicosGenerales.SetSinConexion(true); return true;
                    }
                }
            }
            return false;
            //if (mostrarMensaje) { MessageBox.Show(mySqlException.Message); }
        }

        /* Imagenes//Archivos */
        public static Image CargarImagen(string query)
        {
            try
            {

                using (MySqlConnection conn = new Conexion().ObtenerConexionConsolidado())
                {
                    try { if (conn.State != ConnectionState.Open) { conn.Open(); } } catch { }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = query;
                        byte[] imgArr = (byte[])cmd.ExecuteScalar();
                        imgArr = (byte[])cmd.ExecuteScalar();
                        using (var stream = new MemoryStream(imgArr))
                        {
                            Image img = Image.FromStream(stream);
                            return img;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("No se pudo obtener la imagen");
                return null;
            }
        }
        public static byte[] CargarByteImagen(string query)
        {
            try
            {

                using (MySqlConnection conn = new Conexion().ObtenerConexionConsolidado())
                {
                    try { if (conn.State != ConnectionState.Open) { conn.Open(); } } catch { }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = query;
                        byte[] imgArr = (byte[])cmd.ExecuteScalar();

                        return imgArr;

                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("No se pudo obtener la imagen");
                return null;
            }
        }
        public void GuardarArchivo(string Nombre, byte[] Archivo, string Query)
        {
            _MySqlCnn.TryConnect();
            MySqlCommand comm = _MySqlCnn.CreateCommand();
            comm.CommandText = Query;
            comm.Parameters.Add("@" + Nombre, MySqlDbType.LongBlob).Value = Archivo;
            int columnasAfectadas = comm.ExecuteNonQuery();
            _MySqlCnn.Close();
            if (columnasAfectadas > 0)
            {
                MessageBox.Show("Exito");
            }
            else
            {
                MessageBox.Show("No se actualizao");
            }
        }

        /* Verificaciones telefonica y dommiciliaria */
        public bool GuardarMapa(byte[] Map, string Cuc)
        {
            ObtenerConexion();
            _MySqlCnn.TryConnect();
            MySqlCommand comm = _MySqlCnn.CreateCommand();
            comm.CommandText = "INSERT INTO verificaciones   " +
                " (cuc, motivo_llamada, Resultado_llamada, Recado_A, fecha_llamada, Descripcion_llamada, Fecha_visita, Motivo_visita, Coordenadas, Descripcion_Inmueble, Resultado_visita, Map_Dom)" +
                " VALUES(@cuc, null, 'PENDIENTE', null, null, null, null, null, null, null, 'PENDIENTE', @map)" +
                " ON DUPLICATE KEY UPDATE Map_Dom = @Map;";
            comm.Parameters.Add("@map", MySqlDbType.LongBlob).Value = Map;
            comm.Parameters.Add("@cuc", MySqlDbType.VarChar).Value = Cuc;
            int columnasAfectadas = comm.ExecuteNonQuery();
            _MySqlCnn.Close();
            if (columnasAfectadas > 0)
            {
                MessageBox.Show("Exito"); return true;

            }
            else
            {
                MessageBox.Show("No se actualizao"); return false;
            }
        }

        #region Estructura Accionaria
        public static void GuardarImagen(Image imagen, string cuc)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imagen.Save(ms, ImageFormat.Png);
                byte[] imgArr = ms.ToArray();
                using (MySqlConnection conn = new Conexion().ObtenerConexionConsolidado())
                {
                    try { if (conn.State != ConnectionState.Open) { conn.Open(); } } catch { }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO moral_EstructuraOrg(CUC, Image) VALUES (@CUC, @Image) " +
                            "ON DUPLICATE KEY UPDATE Image=@Image;";
                        cmd.Parameters.AddWithValue("@CUC", cuc);
                        cmd.Parameters.AddWithValue("@Image", imgArr);
                        cmd.ExecuteNonQuery();
                        int resultado = cmd.ExecuteNonQuery();
                        if (resultado > 0)
                        {
                            MessageBox.Show("Guardado");
                        }
                        else
                        {
                            MessageBox.Show("No guardado.");
                        }
                    }
                }
            }
        }
        public static void GuardarArchivo(Image imagen, string cuc)
        {//FALTA MODIFCARLO PARA GUARDAR ARCHIVO
            using (MemoryStream ms = new MemoryStream())
            {
                imagen.Save(ms, ImageFormat.Gif);
                byte[] imgArr = ms.ToArray();
                using (MySqlConnection conn = new Conexion().ObtenerConexionConsolidado())
                {

                    try { if (conn.State != ConnectionState.Open) { conn.Open(); } } catch { }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO moral_EstructuraOrg(CUC, Image) VALUES (@CUC, @Image) " +
                            "ON DUPLICATE KEY UPDATE Image=@Image;";
                        cmd.Parameters.AddWithValue("@CUC", cuc);
                        cmd.Parameters.AddWithValue("@Image", imgArr);
                        cmd.ExecuteNonQuery();
                        int resultado = cmd.ExecuteNonQuery();
                        if (resultado > 0)
                        {
                            MessageBox.Show("Guardado");
                        }
                        else
                        {
                            MessageBox.Show("No guardado.");
                        }
                    }
                }
            }
        }
        public static void EliminarImagen(string cuc)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (MySqlConnection conn = new Conexion().ObtenerConexionConsolidado())
                {
                    try { if (conn.State != ConnectionState.Open) { conn.Open(); } } catch { }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "DELETE FROM moral_EstructuraOrg WHERE CUC = @CUC;";
                        cmd.Parameters.AddWithValue("@CUC", cuc);
                        int resultado = cmd.ExecuteNonQuery();
                        if (resultado > 0)
                        {
                            MessageBox.Show("Borrado");
                        }
                        else
                        {
                            MessageBox.Show("No se borro.");
                        }
                    }
                }
            }
        }
        public static Image CargarImagenEstructura(string cuc)
        {
            try
            {

                using (MySqlConnection conn = new Conexion().ObtenerConexionConsolidado())
                {
                    try { if (conn.State != ConnectionState.Open) { conn.Open(); } } catch { }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT Image FROM moral_EstructuraOrg WHERE CUC = @CUC";
                        cmd.Parameters.AddWithValue("@CUC", cuc);
                        byte[] imgArr = (byte[])cmd.ExecuteScalar();
                        imgArr = (byte[])cmd.ExecuteScalar();
                        using (var stream = new MemoryStream(imgArr))
                        {
                            Image img = Image.FromStream(stream);
                            return img;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("No se pudo obtener la imagen");
                return null;
            }
        }
        #endregion Estructura Accionaria

        /* Abrir una aplicacion */
        public static string lanzaProceso(string Proceso)
        {
            System.Diagnostics.Process.Start(Proceso);
            return "";
        }
        public static string lanzaProceso(string Proceso, string Parametros)
        {
            //System.Diagnostics.Process.Start(@"file path");
            string ending = "";
            if (ending == "csv")
            {
                Proceso = "writer.exe";
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(Proceso);
            if (Parametros != "")
            {
                startInfo = new ProcessStartInfo(Proceso, Parametros);
            }
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false; //No utiliza RunDLL32 para lanzarlo
                                               //Redirige las salidas y los errores
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process proc = Process.Start(startInfo); //Ejecuta el proceso
            proc.WaitForExit(); // Espera a que termine el proceso
            string error = proc.StandardError.ReadToEnd();
            string resultado = proc.StandardOutput.ReadToEnd();
            if (error != null && error != "") //Error
                throw new Exception("Se ha producido un error al ejecutar el proceso '" + Proceso + "'\n" + "Detalles:\n" + "Error: " + error);
            else //Éxito
                return resultado;//Devuelve el resultado 
        }

        /* Hilos */
        public static void EnSegundoPlano1(ParameterizedThreadStart metodo, params object[] arg)
        {
            Thread thread = new Thread(metodo);
            thread.Start(arg);
        }
        public static void EnSegundoPlano(ThreadStart metodo, params object[] arg)
        {
            Thread thread = new Thread(metodo);
            thread.Start();
        }
    }
    public class XML
    {/*No se usa*/
        private XElement xmlContactos;
        string IPServidor;
        private bool conectado = false;
        public bool ConectarXML()
        {
            try
            {
                xmlContactos = XElement.Load("c://Datos.xml");//Cmabiar ruta...ya no es valida
                conectado = true;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n\n\nVerifica el archivo C://Datos.xml");
            }
            conectado = false;
            return false;
        }
        public void LeerXML()
        {
            try
            {
                string path = DatosPublicosGenerales.RutaDatos;
                string Datos = path + DatosPublicosGenerales.DatosXML;
                if (!File.Exists(Datos))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    Corte_General.Encriptar encriptar = new Corte_General.Encriptar();
                    encriptar.ShowDialog();
                }
                XmlDocument xDoc = new XmlDocument();
                //La ruta del documento XML permite rutas relativas 
                xDoc.LoadXml(Encriptar.DesCifrarFrom(Datos));
                XmlNodeList configuracion = xDoc.GetElementsByTagName("configuraciones");
                XmlNodeList lista = ((XmlElement)configuracion[0]).GetElementsByTagName("configuracion");
                foreach (XmlElement nodo in lista)
                {
                    //Caja
                    /*DatosCajeroSucursalOperacion.ipserver = nodo["servidor"].InnerText;//Se usa en CorteGeneral
                    DatosCajeroSucursalOperacion.usuario = nodo["usuario"].InnerText;
                    DatosCajeroSucursalOperacion.contraseña = nodo["password"].InnerText;
                    DatosCajeroSucursalOperacion.espacios = nodo["espacios_ticket"].InnerText;
                    DatosCajeroSucursalOperacion.impresorav = nodo["caracteres_ticket"].InnerText;
                    DatosCajeroSucursalOperacion.aux = nodo["extra"].InnerText;
                    DatosCajeroSucursalOperacion.Digitales = nodo["digitales"].InnerText;
                    DatosCajeroSucursalOperacion.AvanDB = nodo["AvanDB"].InnerText;
                    DatosCajeroSucursalOperacion.COM = nodo["COM"].InnerText;

                    //SSCO
                    DatosCajeroSucursalOperacion.ipConsolidado = nodo["servidorC"].InnerText;
                    DatosCajeroSucursalOperacion.usrConsolidado = nodo["usuarioC"].InnerText;
                    DatosCajeroSucursalOperacion.conConsolidado = nodo["passwordC"].InnerText;

                    SSCO_TPOS.Variables.IPConsolidado = DatosCajeroSucursalOperacion.ipConsolidado;
                    SSCO_TPOS.Variables.uidConsolidado = DatosCajeroSucursalOperacion.usrConsolidado;
                    SSCO_TPOS.Variables.pswConsolidado = DatosCajeroSucursalOperacion.conConsolidado;
                    DatosCajeroSucursalOperacion.CodigoActualizaciones = nodo["CodigoActualizaciones"].InnerText;

                    //Corte General
                    Corte_General.DatosPublicosGenerales.dirCortesSucursales = @nodo["CortesSucursales"].InnerText;
                    Corte_General.DatosPublicosGenerales.IPConsolidado = nodo["servidorC"].InnerText;
                    Corte_General.DatosPublicosGenerales.uidConsolidado = nodo["usuarioC"].InnerText;
                    Corte_General.DatosPublicosGenerales.pswConsolidado = nodo["passwordC"].InnerText;
                    Corte_General.DatosPublicosGenerales.IPServer = nodo["servidor"].InnerText;
                    Corte_General.DatosPublicosGenerales.uidServer = nodo["usuario"].InnerText;
                    Corte_General.DatosPublicosGenerales.pswServer = nodo["password"].InnerText;
*/

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public string BuscarIP()
        {
            try
            {
                return DatosPublicosGenerales.IPServer;
                //if (!conectado)
                //{
                //    ConectarXML();
                //}
                //if (conectado)
                //{

                //    var estadosAll =
                //      from c in xmlContactos.Descendants("configuracion")
                //      select c.Element("servidor").Value;
                //    IPServidor = estadosAll.First();


                //}
                //return IPServidor;
            }
            catch (Exception)
            {
                return "";// IPServidor;
            }
        }
    }
}
