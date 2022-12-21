using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.ComponentModel.DataAnnotations;
using DevExpress.XtraBars;
using System.Xml.Linq;
using DevExpress.XtraBars.Docking2010;

namespace Enviador_MM
{
    public partial class editar_conexion : DevExpress.XtraBars.ToolbarForm.ToolbarForm
    {
       

        public editar_conexion()
        {
            InitializeComponent();
            XDocument db_xml = XDocument.Load(@"data.xml");
            var conexion = from con in db_xml.Descendants("Conection_Table") select con;
            var dt = new DataTable();
            dt.Columns.Add("Servidor");
            dt.Columns.Add("Usuario");
            dt.Columns.Add("Base de datos");
            dt.Columns.Add("Password");

            windowsUIButtonPanel.ButtonClick += WindowsUIButtonPanel_ButtonClick;


            foreach (XElement datos in conexion.Elements(@"conection"))
            {

                DataRow row = dt.NewRow();

                row["Servidor"] = datos.Element(@"co_name").Value;
                row["Usuario"] = datos.Element(@"co_user").Value;
                row["Base de datos"] = datos.Element(@"co_db_name").Value;
                row["Password"] = datos.Element(@"co_pass").Value;

               dt.Rows.Add(row);


            }
            
            dataGridView1.DataSource = dt;


        }

        private void WindowsUIButtonPanel_ButtonClick(object sender, ButtonEventArgs e)
        {

            string tag = ((WindowsUIButton)e.Button).Tag.ToString();

            switch (tag)
            {
                case ("btn_nueva"):


                    break;

                case ("btn_cerrar"):
                    this.Close();

                    break;

                

            }
        }

      
      

        private void windowsUIButtonPanel_Click(object sender, EventArgs e)
        {
           
           
        }

        private void windowsUIButtonPanel_ButtonClick(object sender, BaseButtonEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


            


        }

        private void editar_conexion_Load(object sender, EventArgs e)
        {

        }
    }
}
