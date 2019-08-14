﻿using dao;
using metier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSB
{
    public partial class frmGestFiche : Form
    {
        FicheFrais ficheFrais;
        public frmGestFiche()
        {
            InitializeComponent();
            /**********************************************************************************
            * Désactivation de l'évènement SelectionChanged du datagridview dgvLignesFiches
            * ********************************************************************************/
            dgvLignesFiches.SelectionChanged -= dgvLignesFiches_SelectionChanged;
            /*************************************************************************
            * Mise en place du datadridview - lignes de la fiche de frais sélectionnée
            *************************************************************************/
            dgvLignesFiches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLignesFiches.MultiSelect = false;
            dgvLignesFiches.RowHeadersVisible = false;
            dgvLignesFiches.ColumnHeadersVisible = true;
            dgvLignesFiches.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLignesFiches.ReadOnly = true;
            dgvLignesFiches.AllowUserToAddRows = false;
            dgvLignesFiches.ScrollBars = ScrollBars.Vertical;
            dgvLignesFiches.AllowUserToResizeRows = false;

            dgvLignesFiches.ColumnCount = 6;

            dgvLignesFiches.Columns[0].Width = 150;
            dgvLignesFiches.Columns[1].Width = 80;
            dgvLignesFiches.Columns[2].Width = 80;
            dgvLignesFiches.Columns[3].Width = 130;


            dgvLignesFiches.Width = 650;

            dgvLignesFiches.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvLignesFiches.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLignesFiches.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            dgvLignesFiches.Columns[0].HeaderText = "Forfait";
            dgvLignesFiches.Columns[1].HeaderText = "Tarif";
            dgvLignesFiches.Columns[2].HeaderText = "Quantité Déclarée";
            dgvLignesFiches.Columns[3].HeaderText = "Total Déclaré";
        }

        private void btnQuitter_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmGestFiche_Load(object sender, EventArgs e)
        {

            /**********************************************************************************
             * Récupération de l'id de la fiche sélectionnée 
             * *******************************************************************************/
            int idFicheFrais = (this.Owner as frmGestListeFiches).getIdFicheFraisSelectionne();


            /**********************************************************************************
            * Recherche de l'objet fichefrais et de l'objet visiteur sélectionnés
            * *******************************************************************************/
            ficheFrais = LesFichesFrais.chercherFicheFrais(idFicheFrais);
            Utilisateur visiteur = LesUtilisateurs.chercherUtilisateur(ficheFrais.getUtilisateur().getId());


            /**********************************************************************************
            * Chargement des lignes concernant la fiche de frais et du visiteur sélectionnée
            * *******************************************************************************/
            List<LigneFrais> lignesFrais = LignesFraisDAO.chargerLignesFrais(ficheFrais.getId());
            LesLignesFrais.remplirLesLignesFrais(lignesFrais);

            ficheFrais.setLignesFrais(LesLignesFrais.obtenirLesLignesFrais());




            /*************************************************************************
            * Affichage des informations dans le formulaire.
            *************************************************************************/
            CultureInfo culture = new CultureInfo("fr-Fr");
            lblTitre.Text = "Fiche de frais n° " + ficheFrais.getId() + " - " + culture.DateTimeFormat.GetMonthName(ficheFrais.getMois()) + " " + ficheFrais.getAnnee();
            lblNomPrenom.Text = visiteur.getNomComplet();
            lblAdresse.Text = visiteur.getAdresse();
            lblCPVille.Text = visiteur.getCPVille();
            lblDateEmbauche.Text = visiteur.getDateEmbauche().ToString("dd-MM-yyyy");

            lblDateCloture.Text = "";
            if (ficheFrais.getDateCloture() != null)
            {
                lblDateCloture.Text = ficheFrais.getDateCloture().Value.ToString("dd-MM-yyyy");
            }

            lblEtat.Text = ficheFrais.getEtatLong();


            Decimal? montantDeclare = ficheFrais.getMontantDeclare();

            lblMontantDeclare2.Text = (montantDeclare != null ? montantDeclare.Value.ToString("C") : "");



            /*************************************************************************
            * Remplissage du datagridview
            *************************************************************************/
            dgvLignesFiches.RowCount = LesLignesFrais.nbLignesFrais();

            Decimal? totalDeclare;
            int ligne = 0;
            foreach (LigneFrais uneLigneFrais in LesLignesFrais.obtenirLesLignesFrais())
            {
                dgvLignesFiches[0, ligne].Value = uneLigneFrais.getTypeFrais().getLibelle();
                dgvLignesFiches[1, ligne].Value = uneLigneFrais.getTypeFrais().getMontant().ToString("C");
                dgvLignesFiches[2, ligne].Value = uneLigneFrais.getQuantiteDeclaree();
                totalDeclare = uneLigneFrais.getQuantiteDeclaree() * uneLigneFrais.getTypeFrais().getMontant();
                dgvLignesFiches[3, ligne].Value = (totalDeclare != null ? totalDeclare.Value.ToString("C") : "");

                ligne++;
            }

            lblMontantDeclare1.Text = (montantDeclare != null ? montantDeclare.Value.ToString("C") : "");

            /**********************************************************************************
             * Activation/désactivation des boutons selon l'état actuel
            **********************************************************************************/
            if (ficheFrais.getEtat() != "EC")
            {
                btnCloturer.Enabled = false;
            }
            if (ficheFrais.getEtat() != "VA")
            {
                btnMEP.Enabled = false;
            }
            if (ficheFrais.getEtat() != "MP")
            {
                btnRembourse.Enabled = false;
            }

            /**********************************************************************************
            * Activation de l'évènement SelectionChanged du datagridview dgvLignesFiches
            **********************************************************************************/
            dgvLignesFiches.SelectionChanged += dgvLignesFiches_SelectionChanged;
            if (dgvLignesFiches.Rows.Count > 0)
            {
                dgvLignesFiches.Rows[0].Selected = false;
                dgvLignesFiches.Rows[0].Selected = true;
            }
        }
        private void dgvLignesFiches_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void btnCloturer_Click(object sender, EventArgs e)
        {
                try
                {
                    ficheFrais.changerEtatFiche("CL");
                    if (FicheFraisDAO.enregistrerNouvelEtat(ficheFrais) == 1)
                    {
                        MessageBox.Show("La fiche de frais a bien été clôturée");
                    }
                    /**********************************************************************
                     * Mise à jour de la liste des fiches de frais
                     **********************************************************************/
                    LesFichesFrais.remplirLesFichesFrais(FicheFraisDAO.chargerFichesFrais());

                    /**********************************************************************
                    * Mise à jour du datagridview Liste des fiches de frais
                     **********************************************************************/
                    (this.Owner as frmGestListeFiches).remplirDgvFiches();

                    /**********************************************************************************
                    * Désactivation du bouton "Valider la fiche de frais"
                    **********************************************************************************/
                    btnCloturer.Enabled = false;
                }
                catch
                {
                    MessageBox.Show("Clôturation impossible");
                }
        }

        private void btnMEP_Click(object sender, EventArgs e)
        {
            try
            {
                ficheFrais.changerEtatFiche("MP");
                if (FicheFraisDAO.enregistrerNouvelEtat(ficheFrais) == 1)
                {
                    MessageBox.Show("La fiche de frais a bien été mise en paiement");
                }
                /**********************************************************************
                 * Mise à jour de la liste des fiches de frais
                 **********************************************************************/
                LesFichesFrais.remplirLesFichesFrais(FicheFraisDAO.chargerFichesFrais());

                /**********************************************************************
                * Mise à jour du datagridview Liste des fiches de frais
                 **********************************************************************/
                (this.Owner as frmGestListeFiches).remplirDgvFiches();

                /**********************************************************************************
                * Désactivation du bouton "Valider la fiche de frais"
                **********************************************************************************/
                btnCloturer.Enabled = false;
            }
            catch
            {
                MessageBox.Show("Mise en paiement impossible");
            }
        }

        private void btnRembourse_Click(object sender, EventArgs e)
        {
            try
            {
                ficheFrais.changerEtatFiche("RE");
                if (FicheFraisDAO.enregistrerNouvelEtat(ficheFrais) == 1)
                {
                    MessageBox.Show("La fiche de frais a bien été remboursée");
                }
                /**********************************************************************
                 * Mise à jour de la liste des fiches de frais
                 **********************************************************************/
                LesFichesFrais.remplirLesFichesFrais(FicheFraisDAO.chargerFichesFrais());

                /**********************************************************************
                * Mise à jour du datagridview Liste des fiches de frais
                 **********************************************************************/
                (this.Owner as frmGestListeFiches).remplirDgvFiches();

                /**********************************************************************************
                * Désactivation du bouton "Valider la fiche de frais"
                **********************************************************************************/
                btnCloturer.Enabled = false;
            }
            catch
            {
                MessageBox.Show("Remboursement impossible");
            }
        }
    }
}

