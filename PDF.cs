using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Corte_General.Configuracion;
using iText.Forms.Util;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Horizontal_Alignment = iText.Layout.Properties.HorizontalAlignment;

namespace Corte_General.Publico
{
    public enum font
    {
        titulo, subtitulo, informacion, texto, columna, datos,
        grantitulo
    }
    public class PDF
    {
        string fechaDeReporte = DateTime.Now.ToString("yyyy-MM-dd");                    //Fecha en que se ejecuta el analisis del reporte
        string exportfolder;
        string exportFile;
        Document doc;
        PdfWriter writer;
        PdfDocument pdf;
        float marginleft = 30;                                                         //Margen izquierdo para cada elemento que se agregue.
        string path_logo = @"http://192.168.1.1//Logo.PNG";

        public Usuario usr = new Usuario();

        string OC = "C.P.";
        public PDF()
        {
            setFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                System.IO.Path.Combine(exportfolder ?? "", "test.pdf"));
            OC = Datoscambiantes._var.Oficial_de_Cumplimiento;
        }
        public PDF create()
        {
            try
            {
                if (MessageBox.Show("Elegir carpeta para guardar?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    exportFile = Extensiones.askFilePath("pdf") ?? "";
                }
                if (exportFile == "") { exportFile = exportfolder + @"\Doc.pdf"; }

                if (File.Exists(exportFile) && MessageBox.Show("Ya existe el archivo, desea sobrescribir.", "Confirma", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try { File.Delete(exportFile); } catch { return this; }
                }

                writer = new PdfWriter(exportFile);
                pdf = new PdfDocument(writer);
                doc = new Document(pdf);
            }
            catch (IOException ex)
            {
                System.Windows.Forms.MessageBox.Show("No se pudo abrir el archivo.");
            }
            return this;
        }
        public PDF setDate(DateTime dt, DateTime dt2)
        {
            usr.setDate(dt, dt2);
            return this;
        }
        public PDF setDate(DateTime dt)
        {
            usr.setDate(dt);
            usr.Descripcion = "Con Fecha " + DateTime.Now.ToString("");
            return this;
        }
        public PDF setFolder(string path, string file)
        {
            exportfolder = path;
            exportFile = file;
            return this;
        }
        public PDF LoadUser(string cuc, string fechaDReporte)
        {
            fechaDeReporte = fechaDReporte;
            LoadUser(cuc);
            return this;
        }
        public PDF LoadUser(string cuc)
        {
            usr.cargarUsuario(cuc);
            return this;
        }
        public string getFileName() { return exportFile; }
        private Image getLogo()
        {
            ImageData imageData = ImageDataFactory.Create(@"http://192.168.1.1/Logo.PNG"); ;
            Image logo = new Image(imageData).ScaleAbsolute(100, 120).SetMarginLeft(20);//SetFixedPosition(1, 25, 25);   .SetMarginTop(25)   
            return logo;
        }
        private Paragraph getfirma(int Margin)
        {
            Paragraph line = new Paragraph("______________________\n" + OC + "\nOficial de Cumplimiento").SetMarginTop(Margin);
            line.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
            return line;
        }
        public void AbrirUltimoPDF()
        {
            if (File.Exists(getFileName()) && MessageBox.Show("¿Abrir?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(getFileName());
            }
        }
    }
    
    public static class PDF_extensiones
    {
        static string path_default = @"http://192.168.1.1/Logo.PNG";
        public static Paragraph P(this string p)
        {
            return new Paragraph(p ?? "");
        }
        public static Image I(this byte[] i)
        {
            if (i == null)
            {
                ImageData imageData = ImageDataFactory.Create(path_default);
                return new Image(imageData).ScaleAbsolute(100, 120).SetMarginLeft(25);// 
            } //Imagen Default
            else
            {
                ImageData imageData = ImageDataFactory.Create(i, true);
                return new Image(imageData).ScaleAbsolute(300, 150);//.SetMarginTop(25).SetMarginLeft(25)
            }
        }
        public static Image I(this byte[] i, string path_logo)
        {
            ImageData imageData = ImageDataFactory.Create(path_logo);
            return new Image(imageData).ScaleAbsolute(100, 120).SetMarginLeft(25);//
        }
        public static Image I(string path_logo)
        {
            ImageData imageData = ImageDataFactory.Create(path_logo);
            return new Image(imageData).ScaleAbsolute(100, 120).SetMarginLeft(25);//
        }
        public static Cell SetFont(this Cell c, font f)
        {
            return c.SetFont(GetFont(f));
        }
        public static Paragraph SetFont(this Paragraph c, font f)
        {
            switch (f)
            {
                case font.grantitulo:
                    c.SetFontSize(17);
                    c.SetTextAlignment(TextAlignment.CENTER);
                    break;
                case font.titulo:
                    c.SetFontSize(12);
                    break;
                case font.informacion:
                    c.SetFontSize(9);
                    break;
                case font.subtitulo:
                    c.SetFontSize(12);
                    break;
                case font.texto:
                    c.SetFontSize(10);
                    break;
                case font.columna:
                    c.SetFontSize(8);
                    break;
                case font.datos:
                    c.SetFontSize(9);
                    break;
                default:
                    c.SetFontSize(10);
                    break;
            }
            return c.SetFont(GetFont(f));
        }
        public static PdfFont GetFont(font f)
        {
            PdfFont fuente;
            fuente = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            switch (f)
            {
                case font.grantitulo:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    break;
                case font.titulo:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                    break;
                case font.subtitulo:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                    break;
                case font.informacion:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                    break;
                case font.texto:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    break;
                case font.columna:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    break;
                case font.datos:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    break;
                default:
                    fuente = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    break;
            }
            // new Font(FontFamily.HELVETICA, 13, Font.NORMAL, GrayColor.GRAYWHITE);
            //.Add(new Paragraph("This is a header"))
            //   .SetFont(f)
            //   .SetFontSize(13)
            //   .SetFontColor(DeviceGray.WHITE)
            //   .SetBackgroundColor(DeviceGray.BLACK)
            //   .SetTextAlignment(TextAlignment.CENTER);
            return fuente;
        }

        public static Table ToTable(this DataTable dt)
        {
            float[] columnWidths = new float[dt.Columns.Count > 0 ? dt.Columns.Count : 1];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columnWidths[i] = 1;
            }
            Table table = new Table(UnitValue.CreatePercentArray(columnWidths));

            /*Set Columns*/
            foreach (System.Data.DataColumn column in dt.Columns)
            {
                Cell cell = new Cell().Add(column.ColumnName.P().SetFont(font.columna));
                table.AddCell(cell);
            }
            /*Set Rows*/
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                System.Data.DataRow row = dt.Rows[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cel = row[j] != null ? row[j].ToString() : "";
                    Cell cell = new Cell().Add(cel.P().SetFont(font.datos));
                    table.AddCell(cell);
                }
            }

            return table;
        }
        public static Table ToTable(this DataTable dt, float[] columnWidths)
        {
            if (dt.Columns.Count != columnWidths.Count()) { return dt.ToTable(); }

            Table table = new Table(UnitValue.CreatePercentArray(columnWidths));

            /*Set Columns*/
            foreach (System.Data.DataColumn column in dt.Columns)
            {
                Cell cell = new Cell().Add(column.ColumnName.P().SetFont(font.columna));
                table.AddCell(cell);
            }
            /*Set Rows*/
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                System.Data.DataRow row = dt.Rows[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cel = row[j] != null ? row[j].ToString() : "";
                    Cell cell = new Cell().Add(cel.P().SetFont(font.datos));
                    table.AddCell(cell);
                }
            }

            return table;
        }
        public static Table SetBorderCells(this Table tb, Border b)
        {

            for (int i = 0; i < tb.GetNumberOfRows(); i++)
            {
                for (int j = 0; j < tb.GetNumberOfColumns(); j++)
                {
                    if (tb.GetCell(i, j) == null) { }
                    else
                    {
                        tb.GetCell(i, j).SetBorder(b);
                    }
                }
            }
            return tb;
        }
        public static Table CenterAll(this Table tb)
        {
            for (int i = 0; i < tb.GetNumberOfRows(); i++)
            {
                for (int j = 0; j < tb.GetNumberOfColumns(); j++)
                {
                    if (tb.GetCell(i, j) == null) { }
                    else
                    { tb.GetCell(i, j).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER); }
                }

            }
            return tb;
        }
        public static Table CenterIn(this Table tb, iText.Layout.Document doc)
        {
            tb.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
            return tb;
        }
        public static Table CenterColumn(this Table tb, int columnIndex)
        {

            for (int i = 0; i < tb.GetNumberOfRows(); i++)
            {
                int j = columnIndex;
                if (tb.GetCell(i, j) == null) { }
                else
                { tb.GetCell(i, j).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER); }

            }
            return tb;
        }
        public static Table TitleRowColor(this Table tb, string color = "#D3D3D3")
        {
            for (int i = 0; i < tb.GetNumberOfRows(); i++)
            {
                for (int j = 0; j < tb.GetNumberOfColumns(); j++)
                {
                    if (tb.GetCell(i, j) == null) { }
                    else
                    {
                        tb.GetCell(i, j).SetBackgroundColor(WebColors.GetRGBColor(color));
                    }
                }
                break;
            }

            return tb;
        }

        public static string LeaveSpacesFirst(this string str, int numberOfSpaces)
        {
            string ret = "";
            for (int i = 0; i < numberOfSpaces; i++)
            {
                ret += "\u00A0";
            }
            return ret + str;
        }
    }
}
