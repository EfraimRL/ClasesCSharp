using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Corte_General.Publico;
using WIA;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;

namespace Corte_General.Publico
{
    public static class Extensiones
    {
        /* String */
        public static int[] SplitToInt(this string ar, char car)
        {
            string[] arr = ar.Split(car);
            int[] arr2 = new int[arr.Count()];
            int i = 0;
            foreach (string valor in arr)
            {
                try
                {
                    arr2[i] = int.Parse(valor);
                }
                catch (Exception)
                {

                    throw new Exception("No se puede convertir el valor a entero. Valor{" + valor + "}");
                }
                i++;
            }
            return arr2;
        }

        public static string Clean(this string str)
        {
            str.Replace("\\", "");
            str.Replace("\'", "");
            return str;
        }
        public static string askFilePath(params string[] formats)
        {
            if (formats.Count() < 1)
            {
                MessageBox.Show("Se necesita al menos un parametro de formato.");
                return null;
            }
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            string format = formats[0];
            saveFileDialog1.Filter = format + " File|*." + format;
            saveFileDialog1.Title = "Save an File of " + format;
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                return saveFileDialog1.FileName;
            }
            else
            {
                return null;
            }
        }
        public static string setLength(this string str, int longitud, char charToFil)
        {
            string resultado = str ?? "";
            while (resultado.Length < longitud)
            {
                resultado += charToFil;
            }
            return resultado;
        }
        public static string NewLine = Environment.NewLine;
        public static string AND(this string str, string condition) { str = str + (str.Replace(" ", "") == "" || condition == "" ? " " : " AND ") + condition; return str; }
        /* Arreglos Arrays */
        public static object[] Add(this object[] arr, params object[] arr2)
        {
            object[] arrAux = new object[arr.Count() + arr2.Count()];
            int i = 0;
            foreach (object valor in arr)
            {
                try
                {
                    arrAux[i] = valor;
                }
                catch (Exception)
                {
                    throw new Exception("No se puede agregar el valor al arreglo. Valor{" + valor + "}");
                }
                i++;
            }
            foreach (object valor in arr2)
            {
                try
                {
                    arrAux[i] = valor;
                }
                catch (Exception)
                {
                    throw new Exception("No se puede agregar el valor al arreglo. Valor{" + valor + "}");
                }
                i++;
            }
            return arrAux;
        }
        public static object CastTo(this object[] arr, Type type)
        {
            dynamic resultado = Array.CreateInstance(type, arr.Count());
            for (int i = 0; i < arr.Count(); i++)
            {
                dynamic val;
                val = Convert.ChangeType(arr[i], type);
                resultado[i] = val;
            }
            return resultado;
        }
        public static bool In(this string str, params string[] args)
        {
            bool encontrado = false;
            foreach (string item in args)
            {
                if (str.Equals(item)) { return true; }
            }
            return encontrado;
        }
        /*public static object[] ToArray(this DataRow row)
        {
            object[] arr = new object[row.ItemArray.Count()];
            int i = 0;
            foreach (object item in row.ItemArray)
            {
                arr[i++] = item.ToString();
            }
            return arr;
        }*/


        /* DataGridView */
        public static void OrdenarDGV(this DataGridView dgv, params int[] arr)
        {
            DataGridView dgv2 = dgv;//FALTA
            foreach (DataGridViewRow row in dgv.Rows)
            {

            }
            for (int i = 0; i < arr.Length; i++)
            {

            }
        }

        public static void Fill(this DataGridView dgv, DataTable dt)
        {
            try
            {
                dgv.Rows.Clear();
                dgv.Columns.Clear();

            }
            catch (Exception)
            {

            }
            foreach (DataColumn column in dt.Columns)
            {
                dgv.Columns.Add(column.ColumnName, column.ColumnName);
            }
            foreach (DataRow dr in dt.Rows)
            {
                //string prueba = dr[5].ToString();
                dgv.Rows.Add(Array.ConvertAll(dr.ItemArray, ele => ele.ToString())); ;
            }
        }

        public static void FillOnlyColumns(this DataGridView dgv, DataTable dt, params int[] columnas)
        {
            try
            {
                dgv.Rows.Clear();
                dgv.Columns.Clear();

            }
            catch (Exception)
            {

            }
            foreach (int columnNum in columnas)
            {
                dgv.Columns.Add(dt.Columns[columnNum].ColumnName, dt.Columns[columnNum].ColumnName);
            }
            foreach (DataRow dr in dt.Rows)
            {
                string[] arr = new string[columnas.Count()];
                int i = 0;
                foreach (int columnNum in columnas)
                {
                    arr[i] = dr[columnNum].ToString();
                }
                dgv.Rows.Add(arr); ;
            }
        }

        public static void FillColumns(this DataGridView dgv, DataTable dt)
        {
            try
            {
                dgv.Rows.Clear();
                dgv.Columns.Clear();
            }
            catch (Exception) { }
            foreach (DataColumn column in dt.Columns)
            {
                dgv.Columns.Add(column.ColumnName, column.ColumnName);
            }
        }

        public static void RowsAutoReSizeFill(this DataGridView dgv)
        {
            dgv.RowsAdded += new DataGridViewRowsAddedEventHandler(delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                //
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    row.Height = (dgv.Height - dgv.ColumnHeadersHeight) / dgv.Rows.Count;
                }
            });
        }
        public static void RowsSizeFill(this DataGridView dgv)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Height = (dgv.Height - dgv.ColumnHeadersHeight) / dgv.Rows.Count;
            }
        }
        /* DataTableCollection */
        public static DataTable First(this DataTableCollection dtc)
        {
            if (dtc.Count < 1)
            {
                return new DataTable();
            }
            else
            {
                if (dtc[0].Rows.Count < 1)
                {
                    DataTable dt = new DataTable();
                    foreach (DataColumn column in dtc[0].Columns)
                    {
                        dt.Columns.Add(column.ColumnName);
                    }
                    return dt;
                }
                return dtc[0];
            }
        }
        public static bool HasTables(this DataTableCollection dtc)
        {
            if (dtc.Count < 1)
            {
                return false;
            }
            else
            {
                //if (dtc[0].Rows.Count < 1)
                //{
                //    return false;
                //}
                return true;
            }
        }

        /* DataRow */
        public static object[] ToArray(this DataRow dr)
        {
            object[] arr = new object[dr.ItemArray.Count()];
            for (int i = 0; i < arr.Count(); i++)
            {
                arr[i] = dr.ItemArray[i];
            }
            return arr;
        }
        /* RadioButton */
        public static string rdbToStringS(this RadioButton rdb)
        {
            if (rdb.Checked)
            {
                return "S";
            }
            else
            {
                return "N"; ;
            }
        }

        /* RadioButton */
        public static void rdbSIoNO(string pertipo, RadioButton rdbS, RadioButton rdbN)
        {
            if (pertipo == "S")
            {
                rdbS.Checked = true;
                rdbN.Checked = false;
            }
            else
            {
                rdbS.Checked = false;
                rdbN.Checked = true;
            }
        }


        public static string rdbTipo(this RadioButton rdb, string pertipo)
        {
            string tipo = (pertipo == "FISICO") ? ((rdb.Checked == true) ? "F" : "M") : "NA";
            return tipo;
        }


        /* CheckBox */
        public static string ToStringS(this CheckBox rdb)
        {
            if (rdb.Checked)
            {
                return "S";
            }
            else
            {
                return "N"; ;
            }
        }
        public static void CheckedText(this CheckBox chk, Form th)
        {
            try
            {
                string nombre = chk.Name.Substring(3);
                Control ds = th.Controls.Find("txt" + nombre, true)?[0];

                if (chk.Checked)
                {
                    ((TextBox)ds).PasswordChar = '\0';
                    chk.CheckState = CheckState.Indeterminate;
                }
                else
                {
                    ((TextBox)ds).PasswordChar = '*';
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /* List */
        /*public static Elemento GetElementoByID(this List<Elemento> list, int id)
        {
            Elemento elemento = null;
            foreach (Elemento item in list)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return elemento;
        }
        public static Elemento GetElementoByName(this List<Elemento> list, string nombre)
        {
            Elemento elemento = null;
            foreach (Elemento item in list)
            {
                if (item.Nombre == nombre)
                {
                    return item;
                }
            }
            return elemento;
        }
        public static int GetIndexBy(this List<Elemento> list, object objCompare)
        {
            foreach (Elemento item in list)
            {
                if ((objCompare is string && item.Nombre == (string)objCompare) || (objCompare is int && item.ID == (int)objCompare))
                {
                    return item.index;
                }
            }
            return -1;
        }
        */
        /* DataTable */
        public static Dictionary<string, int> ToDictionary(this DataTable dt, string key, string value)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                try { dict.Add((string)row[key], (int)row[value]); } catch (Exception) { }
            }
            return dict;
        }

        /* ESCAPE */
        public static void cerrarventana(this Control ctrl)
        {
            Func<double, double> square = (double x) => { return x * x; };
            ctrl.KeyPress += new KeyPressEventHandler(delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar == Convert.ToChar(Keys.Escape))
                {
                    ((Form)ctrl).Close();
                }
            });
        }

        /* Control */

        /* Evento de TAB con enter*/

        public static void OnlyEnter(this Control ctrl)
        {
            Func<double, double> square = (double x) => { return x * x; };
            ctrl.KeyPress += new KeyPressEventHandler(delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar == 13)
                {
                    SendKeys.Send("{TAB}");
                }
            });

        }

        public static void OnlyClic(this Control txt)
        {
            ((TextBox)txt).Click += new EventHandler(delegate (object sender, EventArgs e)
            {
                ((TextBox)txt).SelectAll();
            });
        }

        public static void OnlyLeave(this Control ctrl)
        {
            Func<double, double> square = (double x) => { return x * x; };
            ctrl.Leave += new EventHandler(delegate (object sender, EventArgs e)
            {
                if (String.IsNullOrEmpty(ctrl.Text))
                {
                    ctrl.BackColor = Color.DarkRed;
                    ctrl.ForeColor = Color.White;
                }
                else
                {
                    ctrl.BackColor = Color.White;
                    ctrl.ForeColor = Color.Black;
                }
            });
        }

        public static void AlmenosUno_Leave(this Control ctrl, TextBox t2, TextBox t3)
        {

            Func<double, double> square = (double x) => { return x * x; };
            ctrl.Leave += new EventHandler(delegate (object sender, EventArgs e)
            {
                if (String.IsNullOrEmpty(ctrl.Text) && String.IsNullOrEmpty(t2.Text) && (String.IsNullOrEmpty(t3.Text)))
                {
                    ctrl.BackColor = Color.Yellow;
                    t2.BackColor = Color.Yellow;
                    t3.BackColor = Color.Yellow;
                }
                else
                {
                    ctrl.BackColor = Color.White;
                    t2.BackColor = Color.White;
                    t3.BackColor = Color.White;
                }
            });

        }

        public static void OnlyNumbers(this Control ctrl)
        {
            Func<double, double> square = (double x) => { return x * x; };
            ctrl.KeyPress += new KeyPressEventHandler(delegate (object sender, KeyPressEventArgs e)
            {
                try
                {
                    if (Char.IsDigit(e.KeyChar))
                    {
                        e.Handled = false;
                    }
                    else if (Char.IsControl(e.KeyChar))
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                catch { }
            });

        }

        public static void OnlyLetters(this Control ctrl)
        {
            Func<double, double> square = (double x) => { return x * x; };
            ctrl.KeyPress += new KeyPressEventHandler(delegate (object sender, KeyPressEventArgs e)
            {
                if (Char.IsLetter(e.KeyChar))
                {
                    e.Handled = false;
                }
                else if (e.KeyChar == '-')
                {
                    e.Handled = false;
                }
                else if (e.KeyChar == Convert.ToChar("'"))
                {
                    e.Handled = false;
                }
                else if (Char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                }
                else if (Char.IsSeparator(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            });
        }

        public static void OnlyNumbersNLetters(this Control ctrl)
        {
            Func<double, double> square = (double x) =>
            {
                return x * x;
            };
            ctrl.KeyPress += new KeyPressEventHandler(delegate (object sender, KeyPressEventArgs e)
            {
                if (Char.IsLetter(e.KeyChar))
                {
                    e.Handled = false;
                }
                else if (e.KeyChar == '-')
                {
                    e.Handled = false;
                }
                else if (e.KeyChar == Convert.ToChar("'"))
                {
                    e.Handled = false;
                }
                else if (Char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else if (Char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                }
                else if (Char.IsSeparator(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            });
        }
        public static void OnEnterDo(this Control txt, object btn)
        {
            txt.KeyDown += new KeyEventHandler(delegate (object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ((Button)btn).PerformClick();
                }
            });
        }

        /* Controls */
        public static Control FindPanel(this Control.ControlCollection Controles, string nombre)
        {
            foreach (Control control in Controles)
            {
                if (control.Name == nombre)
                {
                    return control;
                }
            }
            return null;
        }
        public static Control[] FindLike(this Control.ControlCollection Controles, string nombre, bool reiterar)
        {
            List<Control> controls = new List<Control>();
            foreach (Control control in Controles)
            {
                if (control.Controls.Count > 0) foreach (Control c in control.Controls.FindLike(nombre, reiterar)) { controls.Add(c); }

                if (control.Name.ToUpper().Contains(nombre.ToUpper())) { controls.Add(control); }
            }
            return controls.ToArray();
        }
        /* TextBox */
        public static string text(this TextBox txt)
        {
            try
            {
                return txt.Text;
            }
            catch (Exception)
            {
            }
            return "";
        }

        /* object */
        public static int ToIntTry(this object obj)
        {
            int _return = 0;
            if (obj is null) { return 0; }
            else if (obj is string)
            {
                if (obj.ToString() == "")
                {
                    obj = "0";
                }
                else if (obj.ToString().IndexOf(".") != -1)
                {
                    obj = obj.ToString().Split('.').First();
                }
                return int.Parse(obj.ToString());

            }
            else if (obj is bool) { return (bool)obj == true ? 1 : 0; }
            else
            {
                try
                {

                    return int.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo convertir el valor de '" + obj + "'.");
                }
            }
            return _return;
        }
        public static Int64 ToInt64Try(this object obj)
        {
            int _return = 0;
            if (obj is null) { return 0; }
            else if (obj is string)
            {
                if (obj.ToString() == "")
                {
                    obj = "0";
                }
                else if (obj.ToString().IndexOf(".") != -1)
                {
                    obj = obj.ToString().Split('.').First();
                }
                return Int64.Parse(obj.ToString());

            }
            else if (obj is bool) { return (bool)obj == true ? 1 : 0; }
            else
            {
                return Int64.Parse(obj.ToString());
            }
            return _return;
        }
        public static double ToDouble(this object obj)
        {
            double _return = 0;
            if (obj is null)
            {
                return 0.0;
            }
            if (obj is string)
            {
                if (obj.ToString() == "") { obj = "0"; }
                return double.Parse(obj.ToString());
            }
            else if (obj is bool) { return (bool)obj == true ? 1.0 : 0.0; }
            else
            {
                return double.Parse(obj.ToString());
            }
            return _return;
        }
        public static int ToInt(this object obj)
        {
            int _return = 0;
            if (obj is null) { return 0; }
            else if (obj is string)
            {
                if (obj.ToString() == "")
                {
                    obj = "0";
                }
                else if (obj.ToString().IndexOf(".") != -1)
                {
                    obj = obj.ToString().Split('.').First();
                }
                return int.Parse(obj.ToString());

            }
            else if (obj is bool) { return (bool)obj == true ? 1 : 0; }
            else
            {
                return int.Parse(obj.ToString());
            }
            return _return;
        }
        public static double ToDoubleTry(this object obj)
        {
            double _return = 0;
            if (obj is null)
            {
                return 0.0;
            }
            if (obj is string)
            {
                if (obj.ToString() == "") { obj = "0"; }
                return double.Parse(obj.ToString());
            }
            else if (obj is bool) { return (bool)obj == true ? 1.0 : 0.0; }
            else
            {
                try
                {

                    return double.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo convertir el valor de '" + obj + "'.");
                }
            }
            return _return;
        }

        /* Form */
        public static string Preguntar(this Form fr, params object[] args)
        {
            //Argumento 0 es el mensaje
            //Argumento 1 es titulo del formulario
            //Argumento 2 si es password los caracteres son '*'
            //      Si es lista en lugar del textbox se llena una lista con 
            try
            {
                string pregunta_descripcion = (string)args.First();
                string titulo = args.Count() >= 2 ? (string)args[1] : "Pregunta";
                Control resultado = null;
                Form pregunta = new Form(); pregunta.Size = new Size(400, 200); pregunta.StartPosition = FormStartPosition.CenterScreen; pregunta.Text = titulo;
                Label lbl = new Label(); lbl.Size = new Size(300, 30); lbl.Location = new Point(44, 18); lbl.Text = pregunta_descripcion;
                pregunta.Controls.Add(lbl);
                Button btn = new Button(); btn.Size = new Size(100, 30); btn.Location = new Point(230, 110); btn.Text = "OK";
                btn.Click += new EventHandler(delegate (object send, EventArgs e1)
                {
                    if (resultado is null) { return; }
                    else if (resultado is ListBox && ((ListBox)resultado).SelectedIndex == -1)
                    {
                        MessageBox.Show("Selecciona un elemento de la lista.");
                        return;
                    }
                    pregunta.DialogResult = DialogResult.OK;
                });
                if (args.Count() >= 3 && ((string)args[2]).ToLower() == "lista")
                {
                    ListBox xtt = new ListBox(); xtt.Size = new Size(300, 30); xtt.Location = new Point(44, 50);

                    int i = 0; foreach (object arg in args)
                    {
                        if (i < 3) { i++; continue; }
                        var type = arg?.GetType();
                        try
                        {
                            type = arg?.GetType().GetGenericTypeDefinition();
                        }
                        catch { };
                        if (type == typeof(List<>))
                        {
                            foreach (var itemList in (dynamic)arg)
                            {
                                xtt.Items.Add(itemList);
                            }
                        }
                        else if (type.IsArray)
                        {
                            foreach (var itemList in (object[])arg)
                            {
                                xtt.Items.Add(itemList);
                            }
                        }
                        else if (arg is DataTable)
                        {
                            foreach (DataRow itemList in ((DataTable)arg).Rows)
                            {
                                xtt.Items.Add(itemList[0]);
                            }
                        }
                        else
                        {
                            xtt.Items.Add(arg);
                        }
                        i++;
                    }
                    xtt.OnEnterDo(btn);
                    pregunta.Controls.Add(xtt);
                    resultado = xtt;
                }
                else
                {
                    TextBox xtt = new TextBox(); xtt.Size = new Size(300, 20); xtt.Location = new Point(44, 50);
                    xtt.OnEnterDo(btn);
                    pregunta.Controls.Add(xtt);
                    resultado = xtt;
                    if (args.Count() >= 3 && ((string)args[2]).ToLower() == "password")
                    {
                        xtt.Name = "txtPwd";
                        xtt.PasswordChar = '*';
                        CheckBox chk = new CheckBox(); chk.Text = ""; chk.Location = new Point(xtt.Location.X + xtt.Size.Width + 4, xtt.Location.Y);
                        chk.Name = "chkPwd"; chk.Size = new Size(20, 20); pregunta.Controls.Add(chk);
                        chk.CheckedChanged += new EventHandler(delegate (object sender, EventArgs e) { chk.CheckedText(pregunta); });
                    }
                }
                pregunta.Controls.Add(btn);
                pregunta.BringToFront();
                pregunta.ShowDialog();
                return resultado.Text;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static void Mostrar(params object[] args)
        {
            Form f1 = new Form();
            f1.AutoScroll = true;
            f1.BackColor = Color.FromArgb(1, 45, 108);
            f1.Size = new Size(600, 712);
            int location = 25, i = 0;
            foreach (object obj in args)
            {
                TextBox txt = new TextBox();
                txt.Multiline = true;
                txt.Size = new Size(550, 100);
                txt.Location = new Point(location, (100 * i) + (location * (++i)));
                txt.Text = obj.ToString();
                f1.Controls.Add(txt);
                txt.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left);
            }
            f1.Show();
        }

        /* OpenFileDialog */

        public static object OpenFileD(params object[] args)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Image Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "jpg",
                Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|gif files (*.gif)|*.gif",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName != "")
            {
                return openFileDialog1.FileName;
            }
            return "";
        }
        public static object OpenFileExtensions(string extension, params object[] args)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Image Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = extension,
                Filter = string.Format("{0} files (*.{0})|*.{0}", extension),
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName != "")
            {
                return openFileDialog1.FileName;
            }
            return "";
        }
        public static object OpenFilePathExtensions(string extension, params object[] args)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Image Files",

                CheckFileExists = false,
                CheckPathExists = true,

                DefaultExt = extension,
                Filter = string.Format("{0} files (*.{0})|*.{0}", extension),
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName != "")
            {
                return openFileDialog1.FileName;
            }
            return "";
        }
        public static object FindPath(params object[] args)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog
            {
                //InitialDirectory = @"C:\",
                Title = "Buscar carpeta",

                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                // Always default to Folder Selection.
                FileName = "Carpeta",

                RestoreDirectory = true,

                ReadOnlyChecked = true
            };

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                return Path.GetDirectoryName(folderBrowser.FileName);
            }
            return "";
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            catch (Exception ex)
            {
                ;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        /* MysqlConnection */
        public static MySqlConnection TryConnect(this MySqlConnection my)
        {
            if (my is null)
            {
                return my;
            }
            try { my.Close(); } catch { }
            if (my.State != ConnectionState.Open)
            {
                try { my.Open(); } catch { }
            }
            return my;
        }

        /* ListBox.ObjectCollectio */
        public static object[] ToArray(this ListBox.ObjectCollection lst)
        {
            object[] resultado = new object[lst.Count];
            for (int i = 0; i < lst.Count; i++)
            {
                resultado[i] = lst[i];
            }
            return resultado;
        }

        /* Array */
        public static List<object> ToList(this ListBox.ObjectCollection lst)
        {
            List<object> resultado = new List<object>();
            for (int i = 0; i < lst.Count; i++)
            {
                resultado.Add(lst[i]);
            }
            return resultado;
        }

        /* Funciones */
        public static Dupla Elegir(params object[] args)
        {
            /* Devuelve el index y valor en una Dupla */
            Dupla res = new Dupla(-1, "");
            Form fr = new Form();
            fr.Size = new Size(112 * args.Count(), 170);
            fr.Text = "Elegir";
            fr.BackColor = Color.DarkBlue;
            for (int i = 1; i <= args.Count(); i++)
            {
                object arg = args[i - 1];
                int index = i;
                Button btn = new Button(); btn.BackColor = Color.White;
                btn.Text = arg.ToString(); btn.Size = new Size(90, 100); btn.Location = new Point(i * 100 - 90, 10);
                fr.Controls.Add(btn);
                btn.Click += new EventHandler(delegate (object sender, EventArgs e) { try { Cursor.Current = Cursors.WaitCursor; res = new Dupla(index, arg); fr.DialogResult = DialogResult.OK; } catch { } finally { Cursor.Current = Cursors.Default; } });
            }
            fr.ShowDialog();
            return res;
        }
        public static string ToString2(this Dictionary<string, object> d)
        {
            string res = "{";
            foreach (KeyValuePair<string, object> dupla in d)
                res += "[" + dupla.Key + "||" + dupla.Value + "]";
            res += "}";
            return res;
        }
    }

    public class CursorWait : IDisposable
    {
        public CursorWait(bool appStarting = false, bool applicationCursor = false)
        {
            // Wait
            Cursor.Current = appStarting ? Cursors.AppStarting : Cursors.WaitCursor;
            if (applicationCursor) Application.UseWaitCursor = true;
        }

        public void Dispose()
        {
            // Reset
            Cursor.Current = Cursors.Default;
            Application.UseWaitCursor = false;
        }
    }
}
