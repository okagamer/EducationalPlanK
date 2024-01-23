using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace EducationalPlanK
{
    
    public partial class StudentForm : Form
    {
        string connectionString;
        public StudentForm(string str)
        {
            connectionString = str;
            InitializeComponent();
        }

        private void GroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string group = GroupComboBox.Text.ToString();
            MondayGridView(group);
            TuesdayGridView(group);
            WednesdayGridView(group);
            ThursdayGridView(group);
            FridayGridView(group);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string group = comboBox1.Text.ToString();
            SpecialityGridView(group);
            CurriculumGridView(group);
        }

        private void MondayGridView (string g)
        {
            MondayGrid.Rows.Clear();

            string dayofweek = "Понеділок";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SheduleQuery(dayofweek, g), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string disciplineName = reader["DisciplineName"].ToString();

                            MondayGrid.Rows.Add(disciplineName);
                        }
                    }
                }
            }
        }

        private void TuesdayGridView(string g)
        {
            TuesdayGrid.Rows.Clear();

            string dayofweek = "Вівторок";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SheduleQuery(dayofweek, g), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string disciplineName = reader["DisciplineName"].ToString();

                            TuesdayGrid.Rows.Add(disciplineName);
                        }
                    }
                }
            }
        }

        private void WednesdayGridView(string g)
        {
            WednesdayGrid.Rows.Clear();

            string dayofweek = "Середа";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SheduleQuery(dayofweek, g), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string disciplineName = reader["DisciplineName"].ToString();

                            WednesdayGrid.Rows.Add(disciplineName);
                        }
                    }
                }
            }
        }

        private void ThursdayGridView(string g)
        {
            ThursdayGrid.Rows.Clear();

            string dayofweek = "Четвер";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SheduleQuery(dayofweek, g), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string disciplineName = reader["DisciplineName"].ToString();

                            ThursdayGrid.Rows.Add(disciplineName);
                        }
                    }
                }
            }
        }

        private void FridayGridView(string g)
        {
            FridayGrid.Rows.Clear();

            string dayofweek = "П`ятниця";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SheduleQuery(dayofweek, g), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string disciplineName = reader["DisciplineName"].ToString();

                            FridayGrid.Rows.Add(disciplineName);
                        }
                    }
                }
            }
        }

        private void SpecialityGridView(string g)
        {
            SpecialityDataGrid.Rows.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SpecialityQuery(g), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string specialityName = reader["SpecialtyName"].ToString();
                            string description = reader["Description"].ToString();

                            SpecialityDataGrid.Rows.Add(specialityName, description);
                        }
                    }
                }
            }
        }

        private void CurriculumGridView(string g)
        {
            CurriculumDataGrid.Rows.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(CurriculumQuery(g), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string disciplineName = reader["DisciplineName"].ToString();
                            string hours = reader["Hours"].ToString();
                            string description = reader["Description"].ToString();

                            CurriculumDataGrid.Rows.Add(disciplineName, hours, description);
                        }
                    }
                }
            }
        }

        private string CurriculumQuery(string g)
        {
            string query = $"SELECT Curriculum.Hours, Discipline.DisciplineName, Discipline.Description\r\nFROM Groups\r\nJOIN Curriculum ON Groups.GroupID = Curriculum.SpecialtyID\r\nJOIN Discipline ON Curriculum.DisciplineID = Discipline.DisciplineID\r\nJOIN Specialty ON Curriculum.SpecialtyID = Specialty.SpecialtyID\r\nWHERE Groups.GroupName = '{g}'; ";
            return query;
        }

        private string SheduleQuery(string day, string g)
        {
            string query = $"SELECT Discipline.DisciplineName\r\nFROM Schedule\r\nJOIN Groups ON Schedule.GroupID = Groups.GroupID\r\nJOIN Discipline ON Schedule.DisciplineID = Discipline.DisciplineID\r\nWHERE Groups.GroupName = '{g}' AND Schedule.DayOfWeek = '{day}' ;";
            return query;
        }

        private string SpecialityQuery(string g)
        {
            string query = $"SELECT DISTINCT Specialty.SpecialtyName, Specialty.Description\r\nFROM Groups\r\nJOIN Student ON Groups.GroupID = Student.GroupID\r\nJOIN Curriculum ON Groups.GroupID = Curriculum.SpecialtyID\r\nJOIN Specialty ON Curriculum.SpecialtyID = Specialty.SpecialtyID\r\nWHERE Groups.GroupName = '{g}'";
            return query;
        }

        private void LogOut_Click(object sender, EventArgs e)
        {
            DialogResult IsExit;
            IsExit = MessageBox.Show("Вийти з акаунту?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (IsExit == DialogResult.Yes)
            {
                AuthForm form = new AuthForm();
                form.Show();
                this.Close();
            }
        }
    }
}
