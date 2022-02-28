using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Corte_General.Publico
{
    public partial class SplashScreen : Form
    {
        public SplashScreen() { InitializeComponent(); }
        public SplashScreen(object param)
        {
            InitializeComponent();
            object[] arr = (object[])param;
            if (arr.Length < 1) { return; }
            switch ((int)arr[0])
            {
                case 1: pictureBox1.Image = Corte_General.Properties.Resources.patricio; break;
                case 2: pictureBox1.Image = Corte_General.Properties.Resources.lineas_en_ciruculos; break;
                default:
                    break;
            }

        }
        /* Seccion de movimiento de Form */
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void SplashScreen_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        /* Seccion para ejecutarlo */
        private static Thread splash;
        public static void ShowSplash()
        {
            splash = new Thread(new ParameterizedThreadStart(MostrarSplashScreen));
            object arr = new object[0];
            splash.Start(arr);
        }
        public static void ShowSplash(params object[] param)
        {
            /* El primer parametro se usa para elegir la imagen (Index) */
            splash = new Thread(new ParameterizedThreadStart(MostrarSplashScreen));
            object arr = param;
            splash.Start(arr);
        }
        public static void CloseSplash() { try { Cerrar("", 1); } catch { } }

        /* Metodos privados */

        private static SplashScreen form;
        private static void MostrarSplashScreen(object Configuraciones)
        {
            try
            {
                form = new SplashScreen(Configuraciones);
                form.CenterToScreen();
                form.ShowDialog();
                Console.WriteLine(Thread.CurrentThread.ThreadState);
            }
            catch (ThreadAbortException) { }
            catch { }
        }

        delegate void CambiarProgresoDelegado(string texto, int valor);
        static private void Cerrar(string texto, int valor)
        {
            try
            {
                if (form.InvokeRequired) //preguntamos si la llamada se hace desde un hilo 
                {
                    //si es así entonces volvemos a llamar a CambiarProgreso pero esta vez a través del delegado 
                    //instanciamos el delegado indicandole el método que va a ejecutar 
                    CambiarProgresoDelegado delegado = new CambiarProgresoDelegado(Cerrar);
                    //ya que el delegado invocará a CambiarProgreso debemos indicarle los parámetros 
                    object[] parametros = new object[] { texto, valor };
                    //invocamos el método a través del mismo contexto del formulario (this) y enviamos los parámetros 
                    form.Invoke(delegado, parametros);
                }
                else
                {
                    //en caso contrario, se realiza el llamado a los controles 
                    form.Close();
                }
            }
            catch (Exception) { }
        }
    }
}
