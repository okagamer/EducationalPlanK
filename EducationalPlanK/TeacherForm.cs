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
    public partial class TeacherForm : Form
    {
        string connectionString;
        int TeacherID;
        public TeacherForm(string str, int tID)
        {
            connectionString = str;
            TeacherID = tID;
            
            InitializeComponent();
            GetStudentsByTeacherID(TeacherID);
            ScheduleDataGridView(TeacherID);

        }

        private void GetStudentsByTeacherID(int teacherID)
        {
            string query = $@"
        SELECT Student.FirstName, Student.LastName
        FROM Student
        JOIN Schedule ON Student.GroupID = Schedule.GroupID
        JOIN Discipline ON Schedule.DisciplineID = Discipline.DisciplineID
        JOIN Teacher ON Discipline.DisciplineID = Teacher.DisciplineID
        WHERE Teacher.TeacherID = {teacherID};
        ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TeacherID", teacherID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();

                            StudentListDataGrid.Rows.Add(firstName, lastName);
                        }
                    }
                }
            }
        }
        private void ScheduleDataGridView(int teacherID)
        {
            string query = $@"
        SELECT Schedule.LessonNumber, Schedule.DayOfWeek, Discipline.DisciplineName
        FROM Schedule
        JOIN Teacher ON Schedule.DisciplineID = Teacher.DisciplineID
        JOIN Discipline ON Schedule.DisciplineID = Discipline.DisciplineID
        WHERE Teacher.TeacherID = {teacherID};
        ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string lessonNumber = reader["LessonNumber"].ToString();
                            string dayofweek = reader["DayOfWeek"].ToString();
                            string disciplineName = reader["DisciplineName"].ToString();

                            TeacherScheduleGridView.Rows.Add(lessonNumber, dayofweek, disciplineName);

                        }
                    }
                }
            }
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
