using recapitulatifRecette;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projet_SEILER_TRAN_VOLTZ
{
    public partial class Avis : Form
    {
        public Avis()
        {
            InitializeComponent();
        }

        public Avis(string nom)
        {
            InitializeComponent();
            this.Name = nom;
        }
        //connextion de la base de donné
        OleDbConnection connec = new OleDbConnection();
        string chcon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=baseFrigo2.mdb";

        private void Avis_Load(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.FromArgb(143, 143, 143);
            //paramétrage de la connexion
            connec.ConnectionString = chcon;
            afficherRecap();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Vous avez supprimer votre avis");
            DialogResult = DialogResult.Cancel;
        }

        private void afficherRecap()
        {
            //on commence par la requette necésaiire pour cherché nos donne
            string requette = @"select description, nbPersonnes, tempsCuisson, categPrix, imageDesc
                                from Recettes
                                where description = '" + this.Name + "'";
            connec.Open();
            OleDbDataAdapter data = new OleDbDataAdapter(requette, connec);
            DataTable dt = new DataTable();
            data.Fill(dt);
            recapRecette recap = new recapRecette(dt.Rows[0][0].ToString(), dt.Rows[0][2].ToString() + " minutes", dt.Rows[0][1].ToString() + " personnes", (int)dt.Rows[0][3], 550, 190, 3);
            recap.Location = new Point(260, 50);
            recap.Parent = this;
            connec.Close();
        }

        

        private void label2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Vous avez enregistere votre avis, qui est : ");
            DialogResult = DialogResult.Cancel;
        }
    }
}
