using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
namespace Enviador_MM
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        int venta_id = 0;
        SmtpClient smtp = new SmtpClient();
        MailMessage correo = new MailMessage();
        string valor_opacidad = "";




        public Form1()
        {
            InitializeComponent();

           lista_ventas();
           
            


        }

        public string lista_ventas()
        {
            string name_server = "";
            string user = "";
            string db_name = "";
            string pass = "";
            string query_lista_ventas = "";

            XDocument db_xml = XDocument.Load(@"data.xml");
            var conexion = from con in db_xml.Descendants("Conection_Data") select con;

            foreach (XElement datos in conexion.Elements(@"Conection_Table"))
            {
                name_server = datos.Element(@"co_name").Value;
                user = datos.Element(@"co_user").Value;
                db_name = datos.Element(@"co_db_name").Value;
                pass = datos.Element(@"co_pass").Value;
            }

            
            foreach (XElement datos_querys in conexion.Elements(@"Query_Table"))
            {
               query_lista_ventas = datos_querys.Element(@"query_lista_ventas").Value;
                
            }



            MySqlConnection conexion_db = new MySqlConnection("Server=" + name_server + "; Database=" + db_name + "; Uid=" + user + "; Pwd=" + pass);
         



            try
            {
              conexion_db.Open();

                MessageBox.Show(query_lista_ventas);
                MySqlCommand command = new MySqlCommand(query_lista_ventas, conexion_db);
                MySqlDataReader query1 = command.ExecuteReader();
                var dt = new DataTable();
                dt.Columns.Add("Venta");
                dt.Columns.Add("Usuario");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Email");
                dt.Columns.Add("Envio");
                dt.Columns.Add("Total");


                while (query1.Read())
                {
                    //Console.Write(Convert.ToString(query1[0]));
                    DataRow row = dt.NewRow();
                    row["Venta"] = Convert.ToString(query1[0]);
                    row["Usuario"] = Convert.ToString(query1[1]);

                    if ((Convert.ToString(query1[1])) == "temporal")
                    {
                        row["Nombre"] = Convert.ToString(query1[3]);
                        row["Email"] = Convert.ToString(query1[5]);
                    }
                    else
                    {
                        row["Nombre"] = Convert.ToString(query1[2]);
                        row["Email"] = Convert.ToString(query1[4]);
                    }



                    row["Envio"] = Convert.ToString(query1[6]);
                    row["Total"] = Convert.ToString(query1[7]);
                    dt.Rows.Add(row);


                    GridVCompras.DataSource = dt;
                    GridVCompras.Columns[0].Width = 40;
                    GridVCompras.Columns[4].Width = 80;
                   
                }
                query1.Close();
            }
            catch (Exception e)
            {

                MessageBox.Show("Hay un error en la conexion a la base de datos " + e);
            }
           
           

           

            return "";
        }




        public string envio()
        {
            string smtp_host = "";
            string smtp_correo = "";
            string smtp_pass = "";
            string pass = "";
          



            XDocument db_xml = XDocument.Load(@"data.xml");
            var conexion = from con in db_xml.Descendants("Conection_Data") select con;

            foreach (XElement datos in conexion.Elements(@"email_table"))
            {
                smtp_host = datos.Element(@"em_smtp_host").Value;
                smtp_correo = datos.Element(@"em_correo").Value;
                smtp_pass = datos.Element(@"em_pass").Value;
               
            }
           
            smtp.Host = smtp_host;
             smtp.Port = 465;
            
             smtp.Credentials = new System.Net.NetworkCredential(smtp_correo, smtp_pass);
             smtp.EnableSsl = true;

          
            
            

            correo.From = new MailAddress(smtp_correo, "Muchomaterial", System.Text.Encoding.UTF8);//Correo de salida
            correo.IsBodyHtml = true;
            correo.Body = html();
           
            correo.Subject = (textbox_asunto.Text);

            try
            {
                correo.To.Add(text_box_correo.Text);
                smtp.Send(correo);
                MessageBox.Show("Correo Enviado");
                
               
            }
            catch (Exception e )
            {

                MessageBox.Show("Datos incorrectos" + e);
            }
            

            correo.To.Clear();

            return "";
        }


            public static string[] detalles_venta(string venta_id)
        {
            string name_server = "";
            string user = "";
            string db_name = "";
            string pass = "";
            string[] arr = new string[15];
            string reco = "";
            string query_detalles_ventas = "";
            string query_recomendado = "";
            int contador1 = 2;
            int contador2 = 3;
            List<string> lista_recomendados = new List<string>();
            
            XDocument db_xml = XDocument.Load(@"data.xml");
            var conexion = from con in db_xml.Descendants("Conection_Data") select con;
            foreach (XElement datos in conexion.Elements(@"Conection_Table"))
            {
                name_server = datos.Element(@"co_name").Value;
                user = datos.Element(@"co_user").Value;
                db_name = datos.Element(@"co_db_name").Value;
                pass = datos.Element(@"co_pass").Value;
            }
            MySqlConnection conexion_db = new MySqlConnection("Server=" + name_server + "; Database=" + db_name + "; Uid=" + user + "; Pwd=" + pass);


            foreach (XElement datos_querys in conexion.Elements(@"Query_Table"))
            {
                query_detalles_ventas = datos_querys.Element(@"query_detalles_ventas").Value;
                query_recomendado = datos_querys.Element(@"query_recomendados").Value;
            }
            string query_detalles_venta = query_detalles_ventas + venta_id+" limit 1";
            try
            {
                conexion_db.Open();

                MySqlCommand command = new MySqlCommand(query_detalles_venta, conexion_db);
                MySqlDataReader query1 = command.ExecuteReader();

                string reomendados = "";
                while (query1.Read())
                {
                    //articulo_id
                    arr[0] = Convert.ToString(query1[1]);
                    //nombre
                    arr[1] = Convert.ToString(query1[3]);

                    reco = Convert.ToString(query1[4]);

                }
                query1.Close();

            }
            catch (Exception)
            {

                MessageBox.Show("Hay un problema con la conexion a la base");
            }
          

          

            string query_recomendados = query_recomendado+reco+" order by RAND() limit 4;";
           // conexion_db.Open();
            MySqlCommand command1 = new MySqlCommand(query_recomendados, conexion_db);
            MySqlDataReader query2 = command1.ExecuteReader();

            while (query2.Read())
            {
                //contador1=2
               // contador2 = 3;
                   
                arr[contador1]= Convert.ToString(query2[0]);
                contador1 = contador1 + 2;
                arr[contador2] = Convert.ToString(query2[1]);
                contador2 = contador2 + 2;

            }
            query2.Close();
            return  arr;
            
        }
        public static string generador_ruta(string art)
        {
            string name_server = "";
            string user = "";
            string db_name = "";
            string pass = "";
            string[] arr_ruta = new string[3];
            string rep = "";
            string departamento = "";
            string subdepartamento = "";
            string ruta_query = "";
            //MySqlConnection conexion = new MySqlConnection("Server=192.168.3.213; Database=muchomaterial; Uid=sa; Pwd=A5z8Y1x3!");
            XDocument db_xml = XDocument.Load(@"data.xml");
            var conexion = from con in db_xml.Descendants("Conection_Data") select con;
            foreach (XElement datos in conexion.Elements(@"Conection_Table"))
            {
                name_server = datos.Element(@"co_name").Value;
                user = datos.Element(@"co_user").Value;
                db_name = datos.Element(@"co_db_name").Value;
                pass = datos.Element(@"co_pass").Value;
            }
            MySqlConnection conexion_db = new MySqlConnection("Server=" + name_server + "; Database=" + db_name + "; Uid=" + user + "; Pwd=" + pass);


            foreach (XElement datos_querys in conexion.Elements(@"Query_Table"))
            {
                ruta_query = datos_querys.Element(@"query_ruta").Value;

            }
            string query_ruta = ruta_query + art + ";";
            conexion_db.Open();
            MySqlCommand command = new MySqlCommand(query_ruta, conexion_db);
            MySqlDataReader query_ruta1 = command.ExecuteReader();
            while (query_ruta1.Read())
            {
                //departamento
                arr_ruta[0] = Convert.ToString(query_ruta1[0]);
                //subdepartamento
                arr_ruta[1] = Convert.ToString(query_ruta1[1]);
                //nombre articulo
                arr_ruta[2] = Convert.ToString(query_ruta1[2]);

            }
            query_ruta1.Close();
            rep = arr_ruta[2].Replace(" ", "_");
            rep = rep.Replace("/", "-");
            departamento = arr_ruta[0].Replace(" ", "_");
            subdepartamento = arr_ruta[1].Replace(" ", "_");
            string ruta = "https://muchomaterial.com" + "/" + departamento + "/" + subdepartamento + "/" + rep;
            return ruta;

        }

       

      
        public string plantilla()
        {
            string plantilla1 = System.IO.File.ReadAllText(@"retoma1.html");
            return plantilla1;
        }
        public string plantilla_cupon()
        {
            string plantilla_cupon_1 = System.IO.File.ReadAllText(@"cupon_1.html");
            return plantilla_cupon_1;
        }
        public string Html_cupon()
        {
            string plantilla_cupon_1 = "";
            plantilla_cupon_1 = plantilla_cupon();


            webBrowser1.DocumentText = plantilla_cupon_1;

            return plantilla_cupon_1;
        }

        public string html()
        {
            //funcion para generar html de retoma tu compra
            string plantilla1 = "";
            plantilla1 = plantilla();


            plantilla1 = plantilla1.Replace("#variable_nombre#", text_box_nombre.Text);
            //MessageBox.Show(text_box_nombre.Text);

            string[] detalles = { "" };
            detalles = detalles_venta(Convert.ToString(GridVCompras.CurrentRow.Cells[0].Value));


            plantilla1 = plantilla1.Replace("#variable_articulo_imagen#", detalles[0]);
            plantilla1 = plantilla1.Replace("#variable_nombre_articulo#", detalles[1]);
            plantilla1 = plantilla1.Replace("#variable_articulo_recomendados_1#", detalles[2]);
            plantilla1 = plantilla1.Replace("#variable_nombre_recomendados_1#", detalles[3]);
            plantilla1 = plantilla1.Replace("#variable_articulo_recomendados_2#", detalles[4]);
            plantilla1 = plantilla1.Replace("#variable_nombre_recomendados_2#", detalles[5]);
            plantilla1 = plantilla1.Replace("#variable_articulo_recomendados_3#", detalles[6]);
            plantilla1 = plantilla1.Replace("#variable_nombre_recomendados_3#", detalles[7]);
            plantilla1 = plantilla1.Replace("#variable_articulo_recomendados_4#", detalles[8]);
            plantilla1 = plantilla1.Replace("#variable_nombre_recomendados_4#", detalles[9]);
            plantilla1 = plantilla1.Replace("#variable_texto_1#", textbox_texto_1.Text);
            plantilla1 = plantilla1.Replace("#variable_texto_2#", textbox_texto_2.Text);
            plantilla1 = plantilla1.Replace("#variable_color_1#", Convert.ToString(colorPickEdit1.Text));
            plantilla1 = plantilla1.Replace("#variable_color_2#", Convert.ToString(colorPickEdit2.Text));
            plantilla1 = plantilla1.Replace("#variable_color_3#", Convert.ToString(colorPickEdit3.Text));
            //reemplazo de imagenenes header y footer
            plantilla1 = plantilla1.Replace("#variable-ruta-imagen-header#", textbox_img_head.Text);
            plantilla1 = plantilla1.Replace("#variable-ruta-imagen-footer#", textbox_img_foot.Text);
            //opacidad imagen header
            if (control_opacidad.Value < 10)
            {
                valor_opacidad = '0' + control_opacidad.Value.ToString();
            }
            else
            {
                valor_opacidad = control_opacidad.Value.ToString();
            }
            plantilla1 = plantilla1.Replace("#variable-opacidad-header#", valor_opacidad);


            if (combobox_imagen.SelectedItem.ToString() == "Original")
            {
                plantilla1 = plantilla1.Replace("#variable_imagen_original#", "");
            }
            else
            {
                plantilla1 = plantilla1.Replace("#variable_imagen_original#", combobox_imagen.SelectedItem.ToString());
            }


            webBrowser1.DocumentText = plantilla1;
            return plantilla1;
        }

      

       

        private void GridVCompras_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            text_box_nombre.Text = Convert.ToString(GridVCompras.CurrentRow.Cells[2].Value);
            text_box_correo.Text = Convert.ToString(GridVCompras.CurrentRow.Cells[3].Value);
            html();
        }

      

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
              //  correo.To.Add(text_box_correo.Text);

                DialogResult dr = MessageBox.Show("¿Enviar correo a " + text_box_correo.Text + "?", "Enviar correo",
           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {

                    envio();
                }


            }
            catch
            {
                MessageBox.Show("Correo no valido");
            }
          
        }

        private void control_opacidad_MouseClick(object sender, MouseEventArgs e)
        {
          
            html();
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            editar_conexion editar_conexion_form = new editar_conexion();
            editar_conexion_form.Show();
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            html();
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            lista_ventas();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
               // correo.To.Add(text_box_correo.Text);

                DialogResult dr = MessageBox.Show("¿Enviar correo a " + text_box_correo.Text + "?", "Enviar correo",
           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {

                    envio();
                }


            }
            catch (Exception er)
            {
                MessageBox.Show("Correo no valido" + er);
            }
        }

        private void barListItem1_ListItemClick(object sender, DevExpress.XtraBars.ListItemClickEventArgs e)
        {

        }

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Html_cupon();
        }

        private void ribbonControl1_SelectedPageChanged(object sender, EventArgs e)
        {
            try
            {
                if (ribbonControl1.SelectedPage.Name == "ribbonPage1")
                {
                
                    label2.Text = "Correo";
                    textbox_img_foot.Visible = true;
                }
                if (ribbonControl1.SelectedPage.Name== "ribbonPage2")
                {
                   
                    label2.Text = "Texto 3";
                    textbox_img_foot.Visible = false;
                }
            }
            catch 
            {

                throw;
            }
        }

        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            radialMenu1.ShowPopup(new Point(500,500));
        }

        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
