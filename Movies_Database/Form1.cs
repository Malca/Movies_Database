﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace Movies_Database
{
    public partial class Form1 : Form
    {
        public static string connection = "Server=localhost;Database=filmai;Uid=root;pwd=fortesting;charset=utf8mb4";


        public Form1()
        {


            InitializeComponent();

            if (checkDB_Connection() != true)
            {
                if (MessageBox.Show("Something went wrong. Can't connect to Database") == DialogResult.OK)
                {
                    // Done();
                }

            }
            else
            { 
                
            MySqlConnection mySqlconn = new MySqlConnection(connection);
            mySqlconn.Open();
            GetRecords(mySqlconn);
            mySqlconn.Close();

                if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dataGridView1.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView1, true, null);
            }
            


            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateCategoryComboBox();
        }



        public static bool checkDB_Connection()
        {

            bool isConn = false;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connection);
                conn.Open();
                isConn = true;
            }
            catch (ArgumentException a_ex)
            {
                /*
                Console.WriteLine("Check the Connection String.");
                Console.WriteLine(a_ex.Message);
                Console.WriteLine(a_ex.ToString());
                */
            }
            catch (MySqlException ex)
            {
                /*string sqlErrorMessage = "Message: " + ex.Message + "\n" +
                "Source: " + ex.Source + "\n" +
                "Number: " + ex.Number;
                Console.WriteLine(sqlErrorMessage);
                */
                isConn = false;
                switch (ex.Number)
                {
                    //http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        break;
                    case 0: // Access denied (Check DB name,username,password)
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return isConn;
        }

        private void GetRecords(MySqlConnection mySqlconn, [Optional] string selectedItem, [Optional] string keyword)
        {
            dataGridView1.Rows.Clear();
            MySqlCommand dbcommand = mySqlconn.CreateCommand();

            if (selectedItem != null && keyword != null)
            {
                switch (selectedItem)
                {
                    case "title":
                        dbcommand.CommandText = "SELECT * FROM movies WHERE title LIKE @keyword";
                        break;
                    case "year":
                        dbcommand.CommandText = "SELECT * FROM movies WHERE year LIKE @keyword";
                        break;
                    case "tags":
                        dbcommand.CommandText = "SELECT * FROM movies WHERE tags LIKE @keyword";
                        break;

                }
                dbcommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");



            }
            else
            {

                dbcommand.CommandText = "SELECT * FROM movies";

            }
            MySqlDataReader records = dbcommand.ExecuteReader();
            while (records.Read())
            {

                string title = records["title"].ToString();
                int year = (int)records["year"];
                int rating = (int)records["rating"];
                string tags = records["tags"].ToString();
                string imdb = records["imdb_link"].ToString();

                dataGridView1.Rows.Add(title, year, rating, tags, imdb);

            }
            records.Close();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells[4].Value == null) return;
                var url = row.Cells[4].Value.ToString();
                System.Diagnostics.Process.Start(url);
            }
        }

       public void PopulateCategoryComboBox()
        {
            comboBox1.Items.Add(dataGridView1.Columns[0].HeaderText);
            comboBox1.Items.Add(dataGridView1.Columns[1].HeaderText);
            comboBox1.Items.Add(dataGridView1.Columns[3].HeaderText);
    
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MySqlConnection mySqlconn = new MySqlConnection(connection);
            mySqlconn.Open();
            GetRecords(mySqlconn);
            mySqlconn.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] categories = { "title", "year", "tags" };
            string selectedCategorie = categories[comboBox1.SelectedIndex].ToString();
            Console.WriteLine(selectedCategorie);
            string keyword = textBox1.Text;
            MySqlConnection mySqlconn = new MySqlConnection(connection);
            mySqlconn.Open();
            GetRecords(mySqlconn, selectedCategorie, keyword);
            mySqlconn.Close();
        }

    }
}
