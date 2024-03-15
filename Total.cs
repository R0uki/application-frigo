using affichageTitre;
using alimentChoisi;
using ficheRecette;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using recapitulatifRecette;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using texteRecette;

namespace projet_SEILER_TRAN_VOLTZ
{
    public partial class Total : Form
    {
        public Total()
        {
            InitializeComponent();
        }

        public Total(string nom)
        {
            InitializeComponent();
            this.Text = nom;
            this.Name = nom;
            this.BackColor = System.Drawing.Color.FromArgb(143, 143, 143);

            //fin de linitialisation du recap des ingrediant et ustencil
            affTitre titre1 = new affTitre("Ingrédients", 1);
            titre1.Parent = pnlRecapIngrUsten;
            titre1.Location = new Point(12, 10);
            affTitre titre2 = new affTitre("Ingrédients", 1);
            titre2.Parent = pnlRecapIngrUsten;
            titre2.Location = new Point(12, 264);

            //paramétrage de la connexion
            connec.ConnectionString = chcon;
        }

        //connextion de la base de donné
        OleDbConnection connec = new OleDbConnection();
        string chcon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=baseFrigo2.mdb";

        //variable globale
        int numRecette = 0;
        int numrecette;


        //*****************************************
        //generation de la page de la recette Total
        //*****************************************
        private void Total_Load(object sender, EventArgs e)
        {
            //on commence par la requette necésaiire pour cherché nos donne
            string requette = @"select description, nbPersonnes, tempsCuisson, categPrix, imageDesc
                                from Recettes
                                where description = '" + this.Name +"'";
            connec.Open();
            OleDbDataAdapter data = new OleDbDataAdapter(requette, connec);
            DataTable dt = new DataTable();
            data.Fill(dt);
            recapRecette recap = new recapRecette(dt.Rows[0][0].ToString(), dt.Rows[0][2].ToString() + " minutes", dt.Rows[0][1].ToString() + " personnes", (int)dt.Rows[0][3],550,190,2); 
            recap.Location  = new Point(410, 20);
            recap.Parent = this;
            recap.pdf += MethodePDF;
            recap.annotation += MethodeAnnotation;
            recap.home += retour;
            connec.Close();

            //maintenant on appelle toute les fonctiond'affichage
            //on commnence par recupere le code de la recette
            numeroRecette();

            //on recupere les ingrédiant et leur quantité et les ustencil et on les afficher
            afficheAlliment();
            afficherUstencil();

            //ici on recupere les information suir les etape et on les affiche
            afficherEtape();
        }


        private void afficherEtape()
        {
            //on commence par la requette necésaiire pour trouve le nbr d'etape
            string nbetapes = @"SELECT COUNT (*)
                             FROM EtapesRecette
                             WHERE codeRecette = " + numRecette;

            connec.Open();

            OleDbDataAdapter dataEtape = new OleDbDataAdapter(nbetapes, connec);
            DataTable dtEtape = new DataTable();
            dataEtape.Fill(dtEtape);
            
            for(int  i = 1; i < (int)dtEtape.Rows[0][0] + 1; i++)
            {
                //on fait ici le requette pourle texte qar on a besoin de kla variable i
                string img = @"SELECT imageEtape FROM EtapesRecette
                           WHERE codeRecette = " + numRecette + " AND numEtape = " + i;

                string texte = @"SELECT texteEtape FROM EtapesRecette
                                 WHERE numEtape = " + i + "AND codeRecette = " + numRecette;
                OleDbDataAdapter dataTxt = new OleDbDataAdapter(texte, connec);
                OleDbDataAdapter dataImg = new OleDbDataAdapter(img, connec);
                DataTable dtImg = new DataTable();
                DataTable dtTexte = new DataTable();
                dataImg.Fill(dtImg);
                dataTxt.Fill(dtTexte);
                string nomImage = "Images/" + dtImg.Rows[0][0].ToString();
                string texteEtape = dtTexte.Rows[0][0].ToString();
                txtRecette recette = new txtRecette(nomImage, i, texteEtape, 370,480);
                recette.Parent = pnlEtape;
                recette.Location = new Point(20+((i-1)*420),10);
            }


            connec.Close();
        }

        //methode qui nous permet d'afficher les ustencil necessaire pour la recette
        private void afficherUstencil()
        {
            //on commence par la requette necésaiire pour cherché nos donne
            string codeUst = @"SELECT libUstensile
                                FROM Ustensiles u RIGHT JOIN BesoinsUstensiles bu 
                                ON u.codeUstensile = bu.codeUstensile WHERE codeRecette = " + numRecette;
            
            connec.Open();

            OleDbDataAdapter dataUst = new OleDbDataAdapter(codeUst, connec);
            DataTable dtUst = new DataTable();
            dataUst.Fill(dtUst);
            int y = 0;
            foreach (DataRow row in dtUst.Rows)
            {
                y++;
            }

            for (int i = 0; i < y; i++)
            {
                alimentChx aliment = new alimentChx(275, 55, dtUst.Rows[i][0].ToString(), 1);
                aliment.Parent = pnlUsten;
                aliment.Location = new Point(0, 5 + (i * 58));
            }
            connec.Close();
        }

        //Methode pour affiche les ingrediant de la recette
        private void afficheAlliment()
        {
            //on commence par toute les requette necésaiire pour cherché nos donne
            string codeI = @"SELECT libIngredient
                                FROM Ingrédients i RIGHT JOIN IngrédientsRecette ir 
                                ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numRecette;
            string codeQ = @"SELECT quantite
                       FROM IngrédientsRecette ir RIGHT JOIN Ingrédients i
                       ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numRecette;
            string codeU = @"SELECT unité
                       FROM IngrédientsRecette ir RIGHT JOIN Ingrédients i
                       ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numRecette;
            connec.Open();

            OleDbDataAdapter dataI = new OleDbDataAdapter(codeI, connec);
            OleDbDataAdapter dataU = new OleDbDataAdapter(codeU, connec);
            OleDbDataAdapter dataQ = new OleDbDataAdapter(codeQ, connec);
            DataTable dtI = new DataTable();
            DataTable dtU = new DataTable();
            DataTable dtQ = new DataTable();
            dataI.Fill(dtI);
            dataQ.Fill(dtQ);
            dataU.Fill(dtU);

            int y = 0;
            foreach (DataRow row in dtI.Rows)
            {
                y++;
            }

            for (int i = 0; i < y; i++)
            {
                alimentChx aliment = new alimentChx(275, 55, dtI.Rows[i][0].ToString() + " " + dtQ.Rows[i][0].ToString() + dtU.Rows[i][0].ToString(), 1);
                aliment.Parent = pnlIngr;
                aliment.Location = new Point(0, 5 + (i * 58));
            }
            connec.Close();
        }

        //methode pour recupérer le code de la recette
        private void numeroRecette()
        {
            string codeR = @"select codeRecette
                                from Recettes
                                where description = '" + this.Name + "'";
            connec.Open();
            OleDbDataAdapter data3 = new OleDbDataAdapter(codeR, connec);
            DataTable dt3 = new DataTable();
            data3.Fill(dt3);
            numRecette = (int)dt3.Rows[0][0];
            connec.Close();
        }

        //*****************************************
        //affichage de la page annotation
        //*****************************************
        private void MethodeAnnotation(object sender, EventArgs e)
        {
            PictureBox pB = (PictureBox)sender;

            Avis fiche = new Avis(pB.Name);
            if (fiche.ShowDialog() == DialogResult.Cancel)
            {
                fiche.Close();
            }
        }


        //*************
        //    PDF
        //*************
        private void numeroRecette(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            string codeR = @"select codeRecette
                                from Recettes
                                where description = '" + pb.Name + "'";
            connec.Open();
            OleDbDataAdapter data3 = new OleDbDataAdapter(codeR, connec);
            DataTable dt3 = new DataTable();
            data3.Fill(dt3);
            numrecette = (int)dt3.Rows[0][0];
            connec.Close();
        }
        private void MethodePDF(object sender, EventArgs e)
        {
            PictureBox clickedButon = (PictureBox)sender;
            numeroRecette(sender, e);
            connec.Open();

            //creation du PDF
            PdfDocument doc = new PdfDocument();
            //creation d'une page
            PdfPage page1 = doc.AddPage();
            //taille de la page
            page1.Size = PdfSharp.PageSize.Folio;


            //police, taille, stile pour le nom de la recette
            XFont fontTitre = new XFont("Ingrid Darling", 20, XFontStyle.Bold);
            //gfx permet d'ecrire dans le PDF et on l'ajoute a le premierre page
            XGraphics gfx = XGraphics.FromPdfPage(page1);
            //titre de la recette
            string titre = @"SELECT description 
                             FROM Recettes
                             WHERE codeRecette = " + numrecette;
            OleDbCommand cdtitre = new OleDbCommand(titre, connec);
            string nomRecette = (string)cdtitre.ExecuteScalar();
            gfx.DrawString(nomRecette, fontTitre, XBrushes.Black, new XRect(0, 0, page1.Width, page1.Height - 500), XStringFormats.TopCenter);

            //nombre d'etapes, d'ingrédients et d'ustencile
            string etapes = @"SELECT COUNT (*)
                             FROM EtapesRecette
                             WHERE codeRecette = " + numrecette;
            OleDbCommand cdetape = new OleDbCommand(etapes, connec);
            int nbetape = (int)cdetape.ExecuteScalar();
            string ingre = @"SELECT COUNT(*) 
                             FROM IngrédientsRecette 
                             WHERE codeRecette = " + numrecette;
            OleDbCommand cdingre = new OleDbCommand(ingre, connec);
            int nbingre = (int)cdingre.ExecuteScalar();
            string ust = @"SELECT COUNT(*)
                           FROM BesoinsUstensiles 
                           WHERE codeRecette = " + numrecette;
            OleDbCommand usten = new OleDbCommand(ust, connec);
            int nbUstencile = (int)usten.ExecuteScalar();


            //image de la recette
            string img = @"SELECT imageDesc FROM Recettes
                           WHERE codeRecette = " + numrecette;
            OleDbCommand cdimg = new OleDbCommand(img, connec);
            string lienimg = cdimg.ExecuteScalar().ToString();
            string chemin = "../../Images/" + lienimg;

            XImage image;

            try
            {
                image = XImage.FromFile(chemin);
            }
            catch
            {
                image = XImage.FromFile("Images/imgDefaut.png");
            }

            double moit = image.Width / 2;
            double moi = page1.Width / 2;
            double x = (page1.Width / 2) - (image.Width / 2);
            double y = (page1.Height / 2) - (image.Height / 2);
            gfx.DrawImage(image, x, y);

            double decal = 5;

            //sous titre : police, taille, stile pour les sous titre(etapes, ingredient et ustencile)
            XFont font = new XFont("Kalam", 17, XFontStyle.Bold);
            PdfPage page = doc.AddPage();
            page1.Size = PdfSharp.PageSize.Folio;
            gfx = XGraphics.FromPdfPage(page);

            for (int i = 1; i < nbetape + 1; i++)
            {
                if (0 + decal < page1.Height * 2)
                {

                    //ecrie Etape 1,Etape 2, ect...
                    gfx.DrawString("Etape" + i, font, XBrushes.Black,
                    new XRect(0, decal, page1.Width, decal), XStringFormats.TopLeft);
                    decal += 15;
                    //texte etapes
                    string etape = @"SELECT texteEtape FROM EtapesRecette
                                             WHERE numEtape = " + i + "AND codeRecette = " + numrecette;
                    OleDbCommand cdeta = new OleDbCommand(etape, connec);
                    string etapeune = cdeta.ExecuteScalar().ToString();
                    XTextFormatter tf = new XTextFormatter(gfx);

                    double tailletxt = GetTextHeight(gfx, etapeune, page1.Width, font);
                    XRect newRect = new XRect(0, decal, page1.Width, tailletxt);
                    tf.DrawString(etapeune, font, XBrushes.Black, newRect, XStringFormats.TopLeft);
                    decal += newRect.Height;
                }

            }

            //ingredient ustencile
            PdfPage page4 = doc.AddPage();
            page4.Size = PdfSharp.PageSize.Folio;
            gfx = XGraphics.FromPdfPage(page4);
            decal = 300;
            //sous titre
            // text
            gfx.DrawString("Ingrédients", font, XBrushes.Black,
            new XRect(0, 0, page1.Width, page1.Height - 350), XStringFormats.CenterLeft);
            gfx.DrawString("Ustensiles", font, XBrushes.Black,
            new XRect(0, 0, page1.Width, page1.Height - 350), XStringFormats.CenterRight);

            //texte
            //police, taille, stile pour le texte
            font = new XFont("Kalam", 17, XFontStyle.Regular);

            //recuperation des donnée
            //nom des ingrédient
            string codein = @"SELECT libIngredient
                                FROM Ingrédients i RIGHT JOIN IngrédientsRecette ir 
                                ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numrecette;
            OleDbCommand cd = new OleDbCommand(codein, connec);
            OleDbDataReader dr = cd.ExecuteReader();

            string[] tabingre = new string[nbingre];
            int n = 0;
            while (dr.Read())
            {
                tabingre[n] = dr[0].ToString();
                n++;
            }

            //les quantité des ingrédients
            codein = @"SELECT quantite
                       FROM IngrédientsRecette ir RIGHT JOIN Ingrédients i
                       ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numrecette;
            cd = new OleDbCommand(codein, connec);
            dr = cd.ExecuteReader();

            int[] tabquabtiteingre = new int[nbingre];
            n = 0;
            while (dr.Read())
            {
                tabquabtiteingre[n] = Convert.ToInt32(dr[0]);
                n++;
            }

            //les unité des quantité des ingrédients
            codein = @"SELECT unité
                       FROM IngrédientsRecette ir RIGHT JOIN Ingrédients i
                       ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numrecette;
            cd = new OleDbCommand(codein, connec);
            dr = cd.ExecuteReader();

            string[] tabuniteingre = new string[nbingre];
            n = 0;
            while (dr.Read())
            {
                tabuniteingre[n] = dr[0].ToString();
                n++;
            }

            //affichage des donnée
            for (int i = 0; i < tabingre.Length; i++)
            {
                // text
                gfx.DrawString(tabingre[i] + " : " + tabquabtiteingre[i].ToString() + tabuniteingre[i], font, XBrushes.Black,
                new XRect(0, 0, page1.Width, page1.Height - decal), XStringFormats.CenterLeft);
                decal -= 50;
            }

            //ustensile
            decal = 300;
            //recuperation des donnée
            //nom des ustensile
            string codeus = @"SELECT libUstensile
                                FROM Ustensiles u RIGHT JOIN BesoinsUstensiles bu 
                                ON u.codeUstensile = bu.codeUstensile WHERE codeRecette = " + numrecette;
            OleDbCommand cdu = new OleDbCommand(codeus, connec);
            OleDbDataReader dru = cdu.ExecuteReader();

            string[] tabusten = new string[nbUstencile];
            int nb = 0;
            while (dru.Read())
            {
                tabusten[nb] = dru[0].ToString();
                nb++;
            }

            //affichage des donnée
            for (int i = 0; i < tabusten.Length; i++)
            {
                // text
                gfx.DrawString(tabusten[i], font, XBrushes.Black,
                    new XRect(0, 0, page1.Width, page1.Height - decal), XStringFormats.CenterRight);
                decal -= 50;
            }

            //Specify file name of the PDF file
            string filename = "../../telechargement_de_PDF/" + nomRecette + ".pdf";
            //Save PDF File

            doc.Save(filename);

            connec.Close();
            MessageBox.Show("Téléchargement de votre fichier PDF réussi");
        }

        private double GetTextHeight(XGraphics gfx, string text, double rectWidth, XFont Font)
        {
            double fontHeight = Font.GetHeight();
            double absoluteTextHeight = gfx.MeasureString(text, Font).Height;
            double absoluteTextWidth = gfx.MeasureString(text, Font).Width;

            if (absoluteTextWidth > rectWidth)
            {
                double linesToAdd = (int)Math.Ceiling(absoluteTextWidth / 290) - 1;
                return absoluteTextHeight + linesToAdd * (fontHeight);
            }
            return absoluteTextHeight;
        }



        //*************
        //   Autre
        //*************
        private void sortirPage(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        private void retour(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
