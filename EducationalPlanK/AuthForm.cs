using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace EducationalPlanK
{
    public partial class AuthForm : Form
    {
        string connectionString;
        public AuthForm()
        {
            InitializeComponent();
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder["Data Source"] = @".\SQLEXPRESS";
            connectionStringBuilder["Initial Catalog"] = "EducationalPlan";
            connectionStringBuilder["Integrated Security"] = true;
            connectionString = connectionStringBuilder.ConnectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Username = UsernameText.Text;
            string Password = PasswordText.Text;
            
            try
            {
                string query = "SELECT Role FROM Users WHERE Username = @Username AND Password = @Password";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        string Role = result.ToString();
                        
                        if (Role == "Адмін") // Адмін
                        {
                            AdminForm form = new AdminForm(connectionString);
                            form.Show();
                            this.Hide();
                        }
                        else if (Role == "Студент") // Студент
                        {
                            StudentForm form = new StudentForm(connectionString);
                            form.Show();
                            this.Hide();
                        }
                        else if (Role == "Вчитель") // Вчитель
                        {
                            int teacherID = GetTeacherID(Username, Password);
                            TeacherForm form = new TeacherForm(connectionString, teacherID);
                            form.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Невірний логін або пароль");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private int GetTeacherID(string u, string p)
        {
            string query = $"SELECT TeacherID FROM Users WHERE Username = '{u}' AND Password = '{p}';";
            int teacherId = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    teacherId = Convert.ToInt32(result);
                }
            }

            return teacherId;
        }

    }
}
