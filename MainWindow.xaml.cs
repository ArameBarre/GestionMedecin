using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Medecins
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private SqlConnection conDB;
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
        // assurez vous de changer la valeur de Source et mettre celle de votre Serveur SQL
        private SqlConnection conDB = new SqlConnection(ConnectionString);
        public MainWindow()
        {
        
            InitializeComponent();
            charger_Liste_Medecins();

        }


        public void charger_Liste_Medecins()
        {
            // pour charger la table medecin dans la grille dgMedecin
            // gerer les exceptions
          
                // Verifier si une connexion a la base de donnees est deja ouverte

                if (conDB.State != ConnectionState.Open)
                {
                    conDB.Open();

                }
                // Requete va chercher ce qui dans la table Medecin
                string requete = "SELECT * FROM Medecin";
                SqlCommand cmd = new SqlCommand(requete, conDB);
                cmd.CommandType = CommandType.Text;

            //  lire tous les enregistrements de la table Medecin dans la base de donnees voitures.
            SqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);

            dgMedecin.ItemsSource = dt.DefaultView;
            //fermer la connexion
                conDB.Close();
            
        }

     private void Ligne_Selectionnee(object sender, SelectionChangedEventArgs e)
        {
            // charger la ligne selectionnee dans la dgMedecin vers les controles  TextBox de la page Medecin.
            //Methode qui permet de verifier si les champs sont vides
            /* DataGrid magrille = sender as DataGrid;
             DataRowView ligne_choisi = magrille.SelectedItem as DataRowView;

             if (ligne_choisi != null)
             {
                 tbIDMedecin.Text = ligne_choisi[0].ToString();
                 tbPrenom.Text = ligne_choisi[1].ToString();
                 tbnom.Text = ligne_choisi[2].ToString();
                 tbNumeroContact.Text = ligne_choisi[3].ToString();
                 tbCourriel.Text = ligne_choisi[4].ToString();
                 tbSalaire.Text = ligne_choisi[5].ToString();
                 tbSpecialite.Text = ligne_choisi[6].ToString();
                 tbHopital.Text = ligne_choisi[6].ToString();

             }*/
            if (dgMedecin.SelectedItem != null)
            {
                DataRowView dr = (DataRowView)dgMedecin.SelectedItem;
                tbIDMedecin.Text = dr["Id"].ToString();
                tbPrenom.Text = dr["Prenom"].ToString();
                tbnom.Text = dr["Nom"].ToString();
                tbSpecialite.Text = dr["Specialite"].ToString();
                tbSalaire.Text = dr["Salaire"].ToString();
                tbHopital.Text = dr["Hopital"].ToString();
                tbNumeroContact.Text = dr["NumeroContact"].ToString();
                tbCourriel.Text = dr["Courriel"].ToString();
            }



        }

        //Methode qui permet de verifier si les champs sont vides
        public bool Verifier_champ()
        {

            return (!string.IsNullOrEmpty(tbPrenom.Text) && !string.IsNullOrEmpty(tbnom.Text) && !string.IsNullOrEmpty(tbNumeroContact.Text) && !string.IsNullOrEmpty(tbCourriel.Text)
                && !string.IsNullOrEmpty(tbSalaire.Text) && !string.IsNullOrEmpty(tbSpecialite.Text) && !string.IsNullOrEmpty(tbHopital.Text));
        }

    //Methode qui permet de verifier si un Medecin existe ou non
    public bool Medecin_Existant(int MedID)
    {
        // verifier si medecin existe
        //1. Si oui return true sinon return false;
        //2. gestion des excpetions try catch

        bool medecin_existe = false;
        string requette = "SELECT COUNT(*) AS TOTAL FROM Medecin WHERE MedID = '" + @MedID + "'";
        // Verifier si une connexion a la base de donnees est deja ouverte
        if (conDB.State != ConnectionState.Open)
        {
            conDB.Open();
        }
        SqlCommand cmd = new SqlCommand(requette, conDB);
            cmd.Parameters.AddWithValue("@MedID ", MedID);


            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            if (sqlDataReader.Read())
            {
                if (sqlDataReader["TOTAL"].ToString() != "0")
                {
                    medecin_existe = true;
                    return true;
                }

            }
            conDB.Close();
            return medecin_existe;

    }

        private void btnAjouteur_Med(object sender, RoutedEventArgs e)
        {
            int MedID = int.Parse(tbIDMedecin.Text);
            // Ajouter un medecin de la BBD,
            // verifier:
            //1. Medecin n'existe pas d'abord
            //2. tous les champs sont saisis
            //3. gestion des excpetions try catch
            //4. Messages dans le status Bar
            // Verifier si un medecin existe dans la base de donnees par son ID
            if (!Medecin_Existant(MedID))
            {
                // verifier si tous les champs sont remplis
                if (Verifier_champ())
                {
                    //verifions si la connexion est ouverte
                    if (conDB.State != ConnectionState.Open)
                    {
                        conDB.Open();
                    }

              
                    SqlCommand cmd = new SqlCommand("INSERT INTO Medecin(MedID,Prénom,Nom,Spécialité,Hopital,Téléphone,Salaire,Courriel) VALUES(@tbIDMedecin,@tbPrenom,@tbnom,@tbNumeroContact,@tbCourriel,@tbSalaire,@tbSpecialite,@tbHopital)", conDB);

                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MedID", MedID);
                    cmd.Parameters.AddWithValue("@tbPrenom", tbPrenom.Text);
                    cmd.Parameters.AddWithValue("@tbnom", tbnom.Text);
                    cmd.Parameters.AddWithValue("@tbSpecialite", tbSpecialite.Text);
                    cmd.Parameters.AddWithValue("@tbHopital", tbHopital.Text);
                    cmd.Parameters.AddWithValue("@tbNumeroContact", tbNumeroContact.Text);
                    cmd.Parameters.AddWithValue("@tbSalaire", tbSalaire.Text);
                    cmd.Parameters.AddWithValue("@tbCourriel", tbCourriel.Text);

                    cmd.ExecuteNonQuery();

                    charger_Liste_Medecins();

                    lbMessage.Content = "Nouvelle Medecin saisi!...";

                }
                else
                    lbMessage.Content = "veuillez saisir tous les champs!...";

            }
            else lbMessage.Content = "Ce Medecin existe dans l'inventaire";

        }

        private void btnsuprimer_Med(object sender, RoutedEventArgs e)
        {
            int MedID=int.Parse(tbIDMedecin.Text);
            // Votre code ici pour retirer un medecin de la base de donnees (BDD).
            //1. verifier si le medecin existe si oui retirez le de la BDD, sinon message pour dire qu'il n'existe pas
            // assurez vous de rafraichir la grille apres avoir retire le medecin.
            if (Medecin_Existant(MedID) && !string.IsNullOrEmpty(tbIDMedecin.Text))
            {
                if (conDB.State != ConnectionState.Open) { conDB.Open(); }

                string requete = "DELETE FROM Medecin WHERE MedID=@MedID";


                SqlCommand cmd = new SqlCommand(requete, conDB);
                cmd.Parameters.AddWithValue("@MedID", tbIDMedecin.Text);

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                charger_Liste_Medecins();

                conDB.Close();




                lbMessage.Content = "Medecin supprimee de la Base de donnees";



            }
            else
            {
                lbMessage.Content = "Aucun Medecin avec ce MedID dans la Base de donnees";
            }
        }

        private void Superieur(object sender, RoutedEventArgs e)
        {
            if (Salaire_Superieur_A.IsChecked == true) Salaire_Inferieur_A.IsChecked = false;
        }

        private void inferieur(object sender, RoutedEventArgs e)
        {
            if (Salaire_Inferieur_A.IsChecked == true) Salaire_Superieur_A.IsChecked = false;
        }

        private void Salaire_Consulter(object sender, RoutedEventArgs e)
        {

            // votre code pour  creer la requete en fonction du salaire
            // gerer les exceptions try catch
            // gerer les options : salaire superieur a - salaire inferieur a et salaire egale a:
            SqlCommand cmd = new SqlCommand("SELECT * FROM Medecin WHERE Salaire > @Salaire", conDB);
            cmd.Parameters.AddWithValue("@Salaire", decimal.Parse(ctbSalaire.Text));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grille_consulter.ItemsSource = dt.DefaultView;



        }

        private void consulter_Nom_Prenom(object sender, RoutedEventArgs e)
        {
            // ici implementer la requete pour consultation par Nom et/ou Prenom
            // gestion des excpetions try catch
            // assurez vous d'implementer les cas possibles Nom / Prenom / Nom et Prenom / rien
            SqlCommand cmd = new SqlCommand("SELECT * FROM Medecin WHERE Nom = @Nom OR Prenom = @Prenom", conDB);
            cmd.Parameters.AddWithValue("@Nom", ctbNom.Text);
            cmd.Parameters.AddWithValue("@Prenom", ctbPrenom.Text);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grille_consulter.ItemsSource = dt.DefaultView;

        }

        private void tbnModifier_Med(object sender, RoutedEventArgs e)
        {
            int MedID = int.Parse(tbIDMedecin.Text);
            // Verifier si un existe dans la base de donnees par son MedID
            if (Medecin_Existant(MedID))
            {

                // verifier si tous les champs sont remplis
                if (Verifier_champ())
                {
                    if (conDB.State != ConnectionState.Open) { conDB.Open(); }

                    string requete = "UPDATE Medecin SET MedID=@tbIDMedecin,Prénom=@Prénom,Nom=@Nom,Spécialité=@Spécialité,Hopital=@Hopital,Téléphone=@Téléphone,Salaire=@Salaire,Courriel=@Courriel WHERE MedID=@tbIDMedecin";

                    SqlCommand cmd = new SqlCommand(requete, conDB);


                    cmd.Parameters.AddWithValue("@MedID", MedID);
                    cmd.Parameters.AddWithValue("@tbPrenom", tbPrenom.Text);
                    cmd.Parameters.AddWithValue("@tbnom", tbnom.Text);
                    cmd.Parameters.AddWithValue("@tbSpecialite", tbSpecialite.Text);
                    cmd.Parameters.AddWithValue("@tbHopital", tbHopital.Text);
                    cmd.Parameters.AddWithValue("@tbNumeroContact", tbNumeroContact.Text);
                    cmd.Parameters.AddWithValue("@tbSalaire", tbSalaire.Text);
                    cmd.Parameters.AddWithValue("@tbCourriel", tbCourriel.Text);

                    cmd.ExecuteNonQuery();

                    charger_Liste_Medecins();

                    conDB.Close();


                    lbMessage.Content = "Information de Medecin mise a jour!...";


                }
                else
                    lbMessage.Content = "veuillez saisir tous les champs!...";


            }
            else lbMessage.Content = "Ce Medecin n'existe pas dans l'inventaire";

        }



    }
}
 
