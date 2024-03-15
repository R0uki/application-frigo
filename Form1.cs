using affichageTitre;
using alimentChoisi;
using boutonChoixAliment;
using nomAliment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using testeUC;
using Budget;
using PdfSharp.Charting;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using Image = System.Drawing.Image;
using ficheRecette;
using Application = System.Windows.Forms.Application;
using Point = System.Drawing.Point;

namespace projet_SEILER_TRAN_VOLTZ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "A Table !";
        }
        
        //connextion de la base de donné
        OleDbConnection connec = new OleDbConnection();
        string chcon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=baseFrigo2.mdb";


        //***************************
        //Initialisation des delegate
        //***************************
        //pour le nom de categorie choisit
        public delegate void methode(object sender, EventArgs e, string vlu);
        private event methode titreCate;



        //*******************************************
        //Initialisation des Controls/variable global
        //*******************************************
        DataSet dataSet = new DataSet();
        List<string> alimentChoisit = new List<string>();
        int nbAliment = 0;
        bool baseCharge = false;
        bool charger = false;
        int budget = 0;
        int numRecette;



        private void Form1_Load(object sender, EventArgs e)
        {
            //initialisation du tabControl
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage t in tabControl1.TabPages)
            {
                t.BackColor = System.Drawing.Color.FromArgb(143, 143, 143);
            }

            //Initialisation deds fond de panel qui n'ont pas d'image
            pnlRecap.BackColor = System.Drawing.Color.FromArgb(155, 199, 84);
            pnlCritSupplementaire.BackColor = System.Drawing.Color.FromArgb(239,159,51);
            pnlCategorieChoix.BackColor = System.Drawing.Color.FromArgb(218,114,22);

            //paramétrage de la connexion
            connec.ConnectionString = chcon;                        
        }

        //chargement de la base pour le mode deconnecté (utilisé sur tabPage2/3)
        private void chargementBase()
        {
            connec.Open();
            DataTable schemaTable = connec.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            string nomTable, requete;

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                nomTable = schemaTable.Rows[i][2].ToString();
                requete = "select * from " + nomTable;
                OleDbDataAdapter da = new OleDbDataAdapter(requete, connec);
                da.Fill(dataSet, nomTable);
            }
            connec.Close();
        }


        //**********************************************************
        //Initialisation/utilisation de la page de choix des aliment
        //**********************************************************

        //affichage de la page au click du bouton : trouver recette
        private void btnAffichageChoixAliment(object sender, EventArgs e)
        {
            butonCategorie[] bt = pnlCategorieChoix.Controls.OfType<butonCategorie>().ToArray();

            //changement de page du tabControls
            tabControl1.SelectTab(tabControl1.SelectedIndex + 1);

            //Initialisation des variable et autre
            if(!baseCharge)
            {
                chargementBase();
                baseCharge = true;
            }

            //initialisationd des composant dynamique de le page 2 (tabP 2)
            affTitre categorie = new affTitre("Catégories", 2);
            categorie.Parent = pnlCategorie;
            categorie.Location = new Point(10, 5);

            //appelle des fonction d'initialisation
            chargementCategories(sender, e);
        }

        //charge les usercontrol de choix de categorie
        private void chargementCategories(object sender, EventArgs e)
        {
            //Initialisation des variable
            int nbCate = 0;

            //Initialisation des categorie
            foreach (DataRow ligne in dataSet.Tables[5].Rows)
            {
                butonCategorie btnCate = new butonCategorie(ligne[1].ToString(), 370, 70);
                btnCate.Parent = pnlCategorieChoix;
                btnCate.Name = ligne[1].ToString();
                btnCate.Location = new System.Drawing.Point(2, (70 * nbCate) + 5);
                
                nbCate++;
            }

            butonCategorie[] b = pnlCategorieChoix.Controls.OfType<butonCategorie>().ToArray();
            System.Windows.Forms.Label clickedButon = (System.Windows.Forms.Label)sender;


            foreach (butonCategorie btn in b)
            {
                titreCate += new methode(cate_clik);
                btn.Value = titreCate;
                btn.methodeEvent += chargementAliment;
            }
        }

        //charge les usercontrol de choix des aliment
        private void chargementAliment(object sender, EventArgs e)
        {
            btnChoixAliment[] bt = pnlChoixAliment.Controls.OfType<btnChoixAliment>().ToArray();

            //Initialisation des variable
            int code = 0;
            int x = 0, y = 0;

            //on reinitialise le panel de choix d'aliment
            foreach (btnChoixAliment btnChoixAliment in bt)
            {
                pnlChoixAliment.Controls.Remove(btnChoixAliment);
            }

            //on recherche le code de la categories et du coup ou est sont nom dans la base
            for (int i = 0; i < dataSet.Tables[5].Rows.Count; i++)
            {
                if (lblNomCatégorie.Text == dataSet.Tables[5].Rows[i][1].ToString())
                {
                    if(i >= 8)
                    {
                        code = i + 2;
                    }
                    else { code = i + 1; }
                }
            }
            //MessageBox.Show(code.ToString());

            //Initialisation des aliment
            foreach (DataRow ligne in dataSet.Tables[6].Rows)
            {
                if (ligne[2].ToString() == code.ToString())
                {
                    btnChoixAliment aliment = new btnChoixAliment(365, 75, ligne[1].ToString());
                    aliment.Location = new System.Drawing.Point(20 + ((x % 2) * 370), 10 + (85 * y));
                    aliment.Parent = pnlChoixAliment;
                    aliment.Name = ligne[1].ToString();
                    aliment.methodeEvent += Ingr_Clik;
                    x++;
                    if (x % 2 == 0)
                    {
                        y++;
                    }
                }
            }
        }

        //methode qui traite lorsque l'on click sur un ingrediant
        private void Ingr_Clik(object sender, EventArgs e, string texte, bool cocher)
        {
            btnChoixAliment[] b = pnlChoixAliment.Controls.OfType<btnChoixAliment>().ToArray();

            foreach (btnChoixAliment bt in b)
            {
                bt.Total = nbAliment;
               
            }

            //affichage des UC qui montre quelle aliment on a selectionner
            if (nbAliment < 3 && !cocher)
            {
                if (texte != "")
                {
                    alimentChoisit.Add(texte);
                }

                pnlAlimentChoisit.Controls.Clear();
                nbAliment++;

                for (int i = 0; i < alimentChoisit.Count; i++)
                {
                    txtAliment ali = new txtAliment(alimentChoisit[i], 255, 95);
                    ali.Parent = pnlAlimentChoisit;
                    ali.Name = alimentChoisit[i];
                    ali.BackColor = Color.Transparent;
                    ali.Location = new System.Drawing.Point(10+(i * 268), 18);
                    ali.methodeEvent += methodeSuprAliment;
                }
            }
        }

        //methode qui traite le click qur la crois d'un aliment choisit
        private void methodeSuprAliment(object sender, EventArgs e)
        {
            txtAliment[] l = pnlAlimentChoisit.Controls.OfType<txtAliment>().ToArray();
            btnChoixAliment[] b = pnlChoixAliment.Controls.OfType<btnChoixAliment>().ToArray();
            int nb = l.Length;

            PictureBox clickedButon = (PictureBox)sender;


            foreach (txtAliment ali in l)
            {
                if (clickedButon.Name == ali.Name)
                {
                    ali.Parent.Controls.Remove(ali);
                    foreach (btnChoixAliment buton in b)
                    {
                        if (buton.Name == clickedButon.Name)
                        {

                            buton.Image = Image.FromFile("Image/rectChx.png");
                        }
                    }
                    nbAliment -= 1;

                    alimentChoisit.Remove(ali.Name);
                }
            }
        }

        //gere les UC des nom de categorie avec gestion des couleur et du du label general
        private void cate_clik(object sender, EventArgs e, string vlu)
        {
            butonCategorie[] b = pnlCategorieChoix.Controls.OfType<butonCategorie>().ToArray();
            btnChoixAliment[] bt = pnlChoixAliment.Controls.OfType<btnChoixAliment>().ToArray();
            System.Windows.Forms.Label clickedButon = (System.Windows.Forms.Label)sender;

            lblNomCatégorie.Text = vlu;

            foreach (butonCategorie btn in b)
            {
                if (btn.Name == clickedButon.Text)
                {
                    //ici on gere l'image du bouton
                    if (btn.Clik == true)
                    {
                        lblNomCatégorie.Text = "";
                        lblAttente.Visible = true;
                        btn.Image = Image.FromFile("Image/rectCate.png");

                        foreach(btnChoixAliment btnChoixAliment in bt)
                        {
                            pnlChoixAliment.Controls.Remove(btnChoixAliment);
                        }
                    }
                    else
                    {
                        btn.Clik = false;
                        btn.Image = Image.FromFile("Image/rectCate2.png");
                        //chargementAliment(sender, e);
                    }

                    //ici on gere la label d'attente
                    if (lblNomCatégorie.Text != "")
                    {
                        lblAttente.Visible = false;
                    }
                }
                else
                {
                    btn.Image = Image.FromFile("Image/rectCate.png");
                    btn.Clik = false;
                }
            }
        }


        //************************************************************
        //Initialisation/utilisation de la page de choix de la recette
        //************************************************************

        //affichage au click du bouton fleche suivante
        private void btnAffichageChoixReccete(object sender, EventArgs e)
        {
            //changement de page du tabControls
            tabControl1.SelectTab(tabControl1.SelectedIndex + 1);

            //initialisation du UC de budget
            bdg bdg1 = new bdg();
            bdg1.Parent = panel15;
            bdg1.Location = new Point(73, 43);
            bdg1.methodeClik += methodeBudget;
            
            //affichage des aliment selectioner
            affichageRecapAliment();

            //chargement des comboBox
            chargementCBO(sender, e);

            //affichage des recette avec les paratre deja rentrer
            afficherRecette(sender, e);
        }

        //cette methode nosu permet d'afficher les recette possible suivant les paramettre choisit
        private void afficherRecette(object sender, EventArgs e)
        {
            //on supprime toute le sfiche recette de l'ancienne recherche
            ficheRcte[] f = pnlFicheRecette.Controls.OfType<ficheRcte>().ToArray();
            foreach(ficheRcte fiche in f)
            {
                pnlFicheRecette.Controls.Remove(fiche);
            }

            //initialisation des variable
            bool debut = false;
            int y = 0;

            //base de la requette pour trouver les bonne recette
            string requette = @"SELECT r.description, r.tempsCuisson, r.categPrix, r.imageDesc
                                FROM Recettes r, IngrédientsRecette ir, Ingrédients i, CatégoriesRecette cr, Catégories c
                                Where r.codeRecette = ir.codeRecette
                                and ir.codeIngredient = i.codeIngredient";

            //ajout des choix des aliment dans la requette
            for (int i = 0; i < alimentChoisit.Count; i++)
            {
                if(!debut) //si c'est le premier aliment on doit alors mettre une (
                {
                    debut = true;
                    requette += " and (i.libIngredient= \"" + alimentChoisit[i] + "\"";
                }
                else
                {
                    requette += " or i.libIngredient= \"" + alimentChoisit[i] + "\"";
                }
                if(i  == alimentChoisit.Count -1) //si dernier aliment on referme la paranthese
                {
                    requette += ")";
                }
            }

            //ajout du type dans les paramtre de recherche
            if (cboType.SelectedIndex != -1)
            {
                int index = cboType.SelectedIndex+1; //on fait +1 car les codeType commence a 1 et non 0
                MessageBox.Show(index.ToString());
                requette += @" and r.codeRecette = cr.codeRecette
                            and c.codeCategorie = cr.codeCategorie
                            and c.codeCategorie = " + index.ToString();
            }

            //ajout du temps dans les paramettre de recherche
            if (cboTemps.SelectedIndex != -1)
            {
                requette += " and r.tempsCuisson <= " + cboTemps.Items[cboTemps.SelectedIndex].ToString();
            }

            //ajout du budget dans les paramettre de recherche
            if(budget != 0)
            {
                requette += " and r.categPrix <= " + budget.ToString();
            }

            //on les regroupe est met par ordre alphabetique
            requette += @" GROUP BY r.description, r.tempsCuisson, r.categPrix, r.imageDesc
                        ORDER BY r.description";

            //ouverture de la connexion
            connec.Open();

            //on rentre toute les recette dans trouve avec le requette crer dans un dataTable
            OleDbDataAdapter data = new OleDbDataAdapter(requette, connec);
            DataTable dt = new DataTable();
            data.Fill(dt);

            //on affiche toute les recette presente dans le dataTables
            foreach (DataRow dr in dt.Rows)
            {
                string nomImage = "Images/" + dr[3].ToString();
                ficheRcte fiche = new ficheRcte(nomImage, dr[0].ToString(), dr[1].ToString() + " minutes", (int)dr[2], 285, 440);
                fiche.Location = new Point(40 + (330 * y), 30);
                fiche.Name = dr[0].ToString();
                fiche.Parent = pnlFicheRecette;
                fiche.methodeTotal += MethodeTotal;
                fiche.methodeEtape += MethodeEtape;
                fiche.methodePDF += MethodePDF;
                y++;
            }

            //fermeture de la connexion
            connec.Close();
        }

        //methode qui permet de recupere le paramtre buget pour la recherche de recette 
        private void methodeBudget(object sender, EventArgs e)
        {
            PictureBox bg = (PictureBox)sender;
            //la variable est définie globalement pour plus de facilité pour la recupérer
            budget = (int)bg.Tag;
            afficherRecette(sender, e);
        }

        //affichage des recette si on change les information dans les comboBox
        private void cboTemps_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (cbo.Name == "cboType" && charger)
            {
                afficherRecette(sender, e);
            }

            if (cbo.Name == "cboTemps" && cboTemps.SelectedIndex != -1)
            {
                afficherRecette(sender, e);
            }
        }

        //methode qui regarde dans la liste alimentChoisit et affiche les element si il en a
        private void affichageRecapAliment()
        {
            //cree un tableau avec tous les contols de type alimentchx de pnlRecapAliment
            alimentChx[] a = pnlRecapAliment.Controls.OfType<alimentChx>().ToArray();

            //enleve les controls que l'on ne veut plus
            foreach(alimentChx alimentAncien in a)
            {
                pnlRecapAliment.Controls.Remove(alimentAncien);
            }

            //affiche les nouveau controls
            for (int i = 0; i< alimentChoisit.Count; i ++)
            {
                alimentChx aliment = new alimentChx(175, 60,alimentChoisit[i],2);
                aliment.Parent = pnlRecapAliment;
                aliment.BackColor = Color.Transparent;
                aliment.Location = new Point(210+(180*i),9);
                aliment.Name = alimentChoisit[i];
            }
        }

        //permete de remplir les comboBox de la page si elle sont vide
        private void chargementCBO(object sender, EventArgs e)
        {
            if (cboType.Items.Count == 0)
            {
                //chargement de la cbo du type de recette
                cboType.DataSource = dataSet.Tables[2];
                cboType.ValueMember = "libCategorie";
                cboType.DisplayMember = "codeCategories";
                cboType.SelectedIndex = -1;
                charger = true;

                //chargement de la cbo du temps
                cboTemps.Items.Add(15);
                cboTemps.Items.Add(30);
                cboTemps.Items.Add(45);
                cboTemps.Items.Add(60);
                cboTemps.Items.Add(120);
                cboTemps.Items.Add(180);
            }
            
        }

        //quand on clik on mais réinitilise tous les critere
        private void resetCritere(object sender, EventArgs e)
        {
            //on commence avec les comboBox
            cboTemps.SelectedIndex = -1;
            cboType.SelectedIndex = -1;

            //et après on a juste amettre le variable budget à 0
            budget = 0;

            //on suprime le UC de budget et on le recréé (pour plus de simplicité, et manque de temps)
            bdg[] b = panel15.Controls.OfType<bdg>().ToArray();
            foreach (bdg bdg in b)
            {
                panel15.Controls.Remove(bdg);
            }
            bdg bdg1 = new bdg();
            bdg1.Parent = panel15;
            bdg1.Location = new Point(73, 43);
            bdg1.methodeClik += methodeBudget;

            //on finit par reafficher les recette
            afficherRecette(sender,e);
        }

        //***************************************
        //affichage de la page de recette entiere
        //***************************************
        private void MethodeTotal(object sender, EventArgs e)
        {
            PictureBox pB= (PictureBox)sender;

            Total fiche = new Total(pB.Name);
            if(fiche.ShowDialog() == DialogResult.Cancel)
            {
                fiche.Close();
            }
        }


        //*****************************************
        //affichage de la page de recette par etape
        //*****************************************
        private void MethodeEtape(object sender, EventArgs e)
        {
            PictureBox pB = (PictureBox)sender;

            Etape fiche = new Etape(pB.Name);
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
            numRecette = (int)dt3.Rows[0][0];
            connec.Close();
        }
        private void MethodePDF(object sender, EventArgs e)
        {
            PictureBox clickedButon = (PictureBox)sender;
            numeroRecette(sender,e);
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
                             WHERE codeRecette = " + numRecette;
            OleDbCommand cdtitre = new OleDbCommand(titre, connec);
            string nomRecette = (string)cdtitre.ExecuteScalar();
            gfx.DrawString(nomRecette, fontTitre, XBrushes.Black, new XRect(0, 0, page1.Width, page1.Height - 500), XStringFormats.TopCenter);

            //nombre d'etapes, d'ingrédients et d'ustencile
            string etapes = @"SELECT COUNT (*)
                             FROM EtapesRecette
                             WHERE codeRecette = " + numRecette;
            OleDbCommand cdetape = new OleDbCommand(etapes, connec);
            int nbetape = (int)cdetape.ExecuteScalar();
            string ingre = @"SELECT COUNT(*) 
                             FROM IngrédientsRecette 
                             WHERE codeRecette = " + numRecette;
            OleDbCommand cdingre = new OleDbCommand(ingre, connec);
            int nbingre = (int)cdingre.ExecuteScalar();
            string ust = @"SELECT COUNT(*)
                           FROM BesoinsUstensiles 
                           WHERE codeRecette = " + numRecette;
            OleDbCommand usten = new OleDbCommand(ust, connec);
            int nbUstencile = (int)usten.ExecuteScalar();


            //image de la recette
            string img = @"SELECT imageDesc FROM Recettes
                           WHERE codeRecette = " + numRecette;
            OleDbCommand cdimg = new OleDbCommand(img, connec);
            string lienimg = cdimg.ExecuteScalar().ToString();
            string chemin = "../../Images/" + lienimg;

            XImage image;

            try
            {
                image = XImage.FromFile(chemin);
            }
            catch { 
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
                                             WHERE numEtape = " + i + "AND codeRecette = " + numRecette;
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
                                ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = "+ numRecette;
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
                       ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numRecette;
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
                       ON i.codeIngredient = ir.codeIngredient WHERE codeRecette = " + numRecette;
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
                gfx.DrawString(tabingre[i] +" : " + tabquabtiteingre[i].ToString() + tabuniteingre[i], font, XBrushes.Black,
                new XRect(0, 0, page1.Width, page1.Height - decal), XStringFormats.CenterLeft);
                decal -= 50;
            }
            
            //ustensile
            decal = 300;
            //recuperation des donnée
            //nom des ustensile
            string codeus = @"SELECT libUstensile
                                FROM Ustensiles u RIGHT JOIN BesoinsUstensiles bu 
                                ON u.codeUstensile = bu.codeUstensile WHERE codeRecette = " + numRecette;
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
        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void avantClick(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabControl1.SelectedIndex + 1);
        }

        private void arriereClick(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabControl1.SelectedIndex - 1);
        }

        
    }
}
