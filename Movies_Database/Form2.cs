using Google.Protobuf;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Movies_Database
{
    public partial class Form2 : Form
    {

        public static string connection = "Server=SERVER_HOST;Database=DATABASE_NAME;Uid=LOGINID;pwd=PASSWORD;charset=utf8mb4";
        public Form2()
        {
            InitializeComponent();


        }


        public string filename;
        private void button1_Click(object sender, EventArgs e)
        {
            string movie_title = textBox1.Text;
            string movie_year = dateTimePicker1.Value.ToString();
            string imdb_link = textBox2.Text;
            string imdb_id = textBox3.Text;
            string movie_genre = textBox4.Text;
            string movie_rating = textBox5.Text;
            string movie_description = textBox7.Text;
            string name = System.IO.Path.GetFileNameWithoutExtension(filename);
            string ext = System.IO.Path.GetExtension(filename);
            string movie_photo = "filmai/assets/images/" + name + ext;

            if (movie_title == "" || movie_year == "" || imdb_link == "" || imdb_id == "" || movie_genre == "" || movie_rating == "" || movie_description == "")
            {
                MessageBox.Show("Užpildykite visus laukus");
                return;
            }
            else
            {

                // Executes query to database
                MySqlConnection conn = new MySqlConnection(connection);
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT INTO movies(title, photo, year, text, tags, rating, imdb_link, imdb_id) VALUES (@title, @photo, @year, @text, @tags, @rating, @imdb_link, @imdb_id)";
                command.Parameters.AddWithValue("@title", movie_title);
                command.Parameters.AddWithValue("@photo", movie_photo);
                command.Parameters.AddWithValue("@year", movie_year);
                command.Parameters.AddWithValue("@text", movie_description);
                command.Parameters.AddWithValue("@tags", movie_genre);
                command.Parameters.AddWithValue("@rating", movie_rating);
                command.Parameters.AddWithValue("@imdb_link", imdb_link);
                command.Parameters.AddWithValue("@imdb_id", imdb_id);
                command.ExecuteNonQuery();
                conn.Close();

                // Uploads a photo to a web server
                System.Net.WebClient Client = new System.Net.WebClient();
                Client.Headers.Add("Content-Type", "binary/octet-stream");
                byte[] bandom = Client.UploadFile("http://arturasm.lt/UploadTestingforC.php", "POST", filename);
                string s = System.Text.Encoding.UTF8.GetString(bandom, 0, bandom.Length);

            }
        }


        private void textBox6_Click(object sender, EventArgs e)
        {
            // Configure open file dialog box
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "Images"; // Default file name
            // dialog.DefaultExt = ".jpg", ".png";  Default file extension
            dialog.Filter = "Images (.jpg)|*.jpg;*.JPG;*.jpeg;*.JPEG;*.png;*.PNG"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog() == DialogResult.OK;

            // Process open file dialog box results
            if (result == true)
            {
                filename = dialog.FileName;
                textBox6.Text = filename;
            }
        }

    }
}
