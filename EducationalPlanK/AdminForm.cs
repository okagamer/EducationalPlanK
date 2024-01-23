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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace EducationalPlanK
{
    public partial class AdminForm : Form
    {
        string connectionString;
        int groupID;
        int disciplineID;
        int disciplineID1;
        int specialtyID;
        int semester;
        public AdminForm(string str)
        {
            connectionString = str;
            InitializeComponent();
            LoadStudentsToDataGridView();
            LoadTeachersToDataGridView();
            LoadUsersToDataGridView();
            LoadDisciplinesToDataGridView();
            DisplayCurriculumDetailsInDataGridView();
            LoadSpecialtyList();
            LoadDisciplinesList();

        }
        private void AddStudentButton_Click(object sender, EventArgs e)
        {
            int maxID = GetLastUserID();
            string role = "Студент";
            string firstname = StudentFirstNameText.Text;
            string lastname = StudentLastNameText.Text;
            string username = StudentUsernameText.Text;
            string password = StudentPasswordText.Text;
            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand insertStudentCommand = connection.CreateCommand())
                    using (SqlCommand insertUserCommand = connection.CreateCommand())
                    {
                        insertStudentCommand.Transaction = transaction;
                        insertUserCommand.Transaction = transaction;

                        try
                        {
                            
                            insertStudentCommand.CommandText = "INSERT INTO Student (StudentID, GroupID, FirstName, LastName) VALUES (@StudentID, @GroupID, @FirstName, @LastName);";
                            insertStudentCommand.Parameters.AddWithValue("@StudentID", maxID);
                            insertStudentCommand.Parameters.AddWithValue("@GroupID", groupID);
                            insertStudentCommand.Parameters.AddWithValue("@FirstName", firstname);
                            insertStudentCommand.Parameters.AddWithValue("@LastName", lastname);

                            insertStudentCommand.ExecuteNonQuery();

                            insertUserCommand.CommandText = "INSERT INTO Users (UserID, Username, Password, Role, StudentID) VALUES (@UserID, @Username, @Password, @Role, @StudentID);";
                            insertUserCommand.Parameters.AddWithValue("@UserID", maxID);
                            insertUserCommand.Parameters.AddWithValue("@Username", username);
                            insertUserCommand.Parameters.AddWithValue("@Password", password);
                            insertUserCommand.Parameters.AddWithValue("@Role", role);
                            insertUserCommand.Parameters.AddWithValue("@StudentID", maxID);

                            insertUserCommand.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                LoadStudentsToDataGridView();
            }
        }

        private void DeleteStudentButton_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                int selectedStudentID = Convert.ToInt32(dataGridViewStudents.SelectedRows[0].Cells["StudentID"].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand deleteStudentCommand = connection.CreateCommand())
                        using (SqlCommand deleteUserCommand = connection.CreateCommand())
                        {
                            deleteStudentCommand.Transaction = transaction;
                            deleteUserCommand.Transaction = transaction;

                            try
                            {
                                deleteUserCommand.CommandText = "DELETE FROM Users WHERE StudentID = @StudentID";
                                deleteUserCommand.Parameters.AddWithValue("@StudentID", selectedStudentID);
                                deleteUserCommand.ExecuteNonQuery();

                                deleteStudentCommand.CommandText = "DELETE FROM Student WHERE StudentID = @StudentID";
                                deleteStudentCommand.Parameters.AddWithValue("@StudentID", selectedStudentID);
                                deleteStudentCommand.ExecuteNonQuery();
                                
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                LoadStudentsToDataGridView();
            }
            else
            {
                MessageBox.Show("Виберіть рядок для видалення.");
            }
        }

        private void AddTeacherButton_Click(object sender, EventArgs e)
        {
            LoadDisciplinesList();
            int maxID = GetLastUserID();
            string role = "Вчитель";
            string firstname = TeacherFirstNameText.Text;
            string lastname = TeacherLastNameText.Text;
            string username = TeacherUserNameText.Text;
            string password = TeacherPasswordText.Text;
            string qualification = TeacherQualificationText.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand insertTeacherCommand = connection.CreateCommand())
                    using (SqlCommand insertUserCommand = connection.CreateCommand())
                    {
                        insertTeacherCommand.Transaction = transaction;
                        insertUserCommand.Transaction = transaction;
                        try
                        {
                            insertTeacherCommand.CommandText = "INSERT INTO Teacher (TeacherID, DisciplineID, FirstName, LastName, Qualification) VALUES (@TeacherID, @DisciplineID, @FirstName, @LastName, @Qualification);";
                            insertTeacherCommand.Parameters.AddWithValue("@TeacherID", maxID);
                            insertTeacherCommand.Parameters.AddWithValue("@DisciplineID", disciplineID);
                            insertTeacherCommand.Parameters.AddWithValue("@FirstName", firstname);
                            insertTeacherCommand.Parameters.AddWithValue("@LastName", lastname);
                            insertTeacherCommand.Parameters.AddWithValue("@Qualification", qualification);

                            insertTeacherCommand.ExecuteNonQuery();

                            insertUserCommand.CommandText = "INSERT INTO Users (UserID, Username, Password, Role, StudentID, TeacherID) VALUES (@UserID, @Username, @Password, @Role, NULL, @TeacherID);";
                            insertUserCommand.Parameters.AddWithValue("@UserID", maxID);
                            insertUserCommand.Parameters.AddWithValue("@Username", username);
                            insertUserCommand.Parameters.AddWithValue("@Password", password);
                            insertUserCommand.Parameters.AddWithValue("@Role", role);
                            insertUserCommand.Parameters.AddWithValue("@TeacherID", maxID);

                            insertUserCommand.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            LoadTeachersToDataGridView();
        }

        private void DeleteTeacherButton_Click(object sender, EventArgs e)
        {
            if (dataGridViewTeacher.SelectedRows.Count > 0)
            {
                int teacherID = Convert.ToInt32(dataGridViewTeacher.SelectedRows[0].Cells["TeacherID"].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand deleteTeacherCommand = connection.CreateCommand())
                        using (SqlCommand deleteUserCommand = connection.CreateCommand())
                        {
                            deleteTeacherCommand.Transaction = transaction;
                            deleteUserCommand.Transaction = transaction;

                            try
                            {
                                deleteUserCommand.CommandText = "DELETE FROM Users WHERE TeacherID = @TeacherID";
                                deleteUserCommand.Parameters.AddWithValue("@TeacherID", teacherID);
                                deleteUserCommand.ExecuteNonQuery();

                                deleteTeacherCommand.CommandText = "DELETE FROM Teacher WHERE TeacherID = @TeacherID";
                                deleteTeacherCommand.Parameters.AddWithValue("@TeacherID", teacherID);
                                deleteTeacherCommand.ExecuteNonQuery();

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                LoadTeachersToDataGridView();
            }
            else
            {
                MessageBox.Show("Виберіть рядок для видалення.");
            }
            

        }

        private void AddAdminButton_Click(object sender, EventArgs e)
        {
            string username = AdminUserNameText.Text;
            string password = AdminPasswordText.Text;
            string role = "Адмін";
            int userID = GetLastUserID();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand insertUserCommand = connection.CreateCommand())
                    {
                        insertUserCommand.Transaction = transaction;

                        try
                        {
                            insertUserCommand.CommandText = "INSERT INTO Users (UserID, Username, Password, Role, TeacherID, StudentID) VALUES (@UserID, @Username, @Password, @Role, NULL, NULL)";
                            insertUserCommand.Parameters.AddWithValue("@UserID", userID);
                            insertUserCommand.Parameters.AddWithValue("@Username", username);
                            insertUserCommand.Parameters.AddWithValue("@Password", password);
                            insertUserCommand.Parameters.AddWithValue("@Role", role);
                            insertUserCommand.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            LoadUsersToDataGridView();
        }

        private void DeleteAdminButton_Click(object sender, EventArgs e)
        {
            if (dataGridViewAdmin.SelectedRows.Count > 0)
            {
                int userID = Convert.ToInt32(dataGridViewAdmin.SelectedRows[0].Cells["UserID"].Value);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand deleteUserCommand = connection.CreateCommand())
                        {
                            deleteUserCommand.Transaction = transaction;

                            try
                            {
                                deleteUserCommand.CommandText = "DELETE FROM Users WHERE UserID = @UserID";
                                deleteUserCommand.Parameters.AddWithValue("@UserID", userID);
                                deleteUserCommand.ExecuteNonQuery();

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                LoadUsersToDataGridView();
            }
            else
            {
                MessageBox.Show("Виберіть рядок для видалення.");
            }
        }

        private void AddDisciplineButton_Click(object sender, EventArgs e)
        {
            string disciplinename = DisciplineNameText.Text;
            string descriprion = DisciplineDescriptionText.Text;
            int disciplineid = GetLastDisciplineID();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand insertCommand = connection.CreateCommand())
                    {
                        insertCommand.Transaction = transaction;

                        try
                        {
                            insertCommand.CommandText = "INSERT INTO Discipline (DisciplineID, DisciplineName, Description) VALUES (@DisciplineID, @DisciplineName, @Description)";
                            insertCommand.Parameters.AddWithValue("@DisciplineID", disciplineid);
                            insertCommand.Parameters.AddWithValue("@DisciplineName", disciplinename);
                            insertCommand.Parameters.AddWithValue("@Description", descriprion);
                            insertCommand.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            LoadDisciplinesToDataGridView();
        }

        private void DeleteDisciplineButton_Click(object sender, EventArgs e)
        {
            if (dataGridViewDisciplines.SelectedRows.Count > 0)
            {
                int disciplineID = Convert.ToInt32(dataGridViewDisciplines.SelectedRows[0].Cells["DisciplineID"].Value);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand deleteCommand = connection.CreateCommand())
                        {
                            deleteCommand.Transaction = transaction;

                            try
                            {
                                deleteCommand.CommandText = "DELETE FROM Discipline WHERE DisciplineID = @DisciplineID";
                                deleteCommand.Parameters.AddWithValue("@DisciplineID", disciplineID);
                                deleteCommand.ExecuteNonQuery();

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show("Error: " + ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                LoadDisciplinesToDataGridView();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть рядок для видалення.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateCurriculumButton_Click(object sender, EventArgs e)
        {
            int curriculumID = GetLastCurriculumID();
            int hours = int.Parse(HoursText.Text);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand insertCommand = connection.CreateCommand())
                    {
                        insertCommand.Transaction = transaction;

                        try
                        {
                            insertCommand.CommandText = "INSERT INTO Curriculum (CurriculumID, SpecialtyID, DisciplineID, Hours, Semester) VALUES (@CurriculumID, @SpecialtyID, @DisciplineID, @Hours, @Semester)";
                            insertCommand.Parameters.AddWithValue("@CurriculumID", curriculumID);
                            insertCommand.Parameters.AddWithValue("@SpecialtyID", specialtyID);
                            insertCommand.Parameters.AddWithValue("@DisciplineID", disciplineID1);
                            insertCommand.Parameters.AddWithValue("@Hours", hours);
                            insertCommand.Parameters.AddWithValue("@Semester", semester);
                            insertCommand.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            DisplayCurriculumDetailsInDataGridView();
        }

        private void DeleteCurriculumButton_Click(object sender, EventArgs e)
        {
            if (dataGridViewCurriculum.SelectedRows.Count > 0)
            {
                int curriculumID = Convert.ToInt32(dataGridViewCurriculum.SelectedRows[0].Cells["CurriculumID"].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand deleteCommand = connection.CreateCommand())
                        {
                            deleteCommand.Transaction = transaction;

                            try
                            {
                                deleteCommand.CommandText = "DELETE FROM Curriculum WHERE CurriculumID = @CurriculumID";
                                deleteCommand.Parameters.AddWithValue("@CurriculumID", curriculumID);
                                deleteCommand.ExecuteNonQuery();

                                transaction.Commit();
                                MessageBox.Show("Рядок видалено успішно.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Помилка видалення рядка: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                DisplayCurriculumDetailsInDataGridView();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть рядок для видалення.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentsToDataGridView()
        {
            
            string query = "SELECT * FROM Student;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridViewStudents.DataSource = dataTable;
                    }
                }
            }
            using (SqlConnection connection1 = new SqlConnection(connectionString))
            {
                connection1.Open();

                string query1 = "\r\nSELECT * FROM Users\r\nWHERE Users.Role = 'Студент';";
                using (SqlCommand command1 = new SqlCommand(query1, connection1))
                {
                    SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                    DataTable dataTable1 = new DataTable();

                    adapter1.Fill(dataTable1);

                    dataGridViewUsers.DataSource = dataTable1;
                }
            }
            dataGridViewStudents.Update();
            dataGridViewUsers.Update();
        }

        private void LoadTeachersToDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Teacher";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    dataGridViewTeacher.DataSource = dataTable;
                }
            }
            using (SqlConnection connection1 = new SqlConnection(connectionString))
            {
                connection1.Open();

                string query1 = "\r\nSELECT * FROM Users\r\nWHERE Users.Role = 'Вчитель';";
                using (SqlCommand command1 = new SqlCommand(query1, connection1))
                {
                    SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                    DataTable dataTable1 = new DataTable();

                    adapter1.Fill(dataTable1);

                    dataGridViewTeacherUsers.DataSource = dataTable1;
                }
            }
            dataGridViewTeacher.Update();
            dataGridViewTeacherUsers.Update();
        }

        private void LoadUsersToDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "\r\nSELECT * FROM Users\r\nWHERE Users.Role = 'Адмін';";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    dataGridViewAdmin.DataSource = dataTable;
                }
            }
            dataGridViewAdmin.Update();
        }

        private void LoadDisciplinesToDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Discipline";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    dataGridViewDisciplines.DataSource = dataTable;
                }
                dataGridViewDisciplines.Update();
            }
        }

        private int GetLastUserID()
        {
            string query = "SELECT MAX(UserID) FROM Users;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result) + 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }
        
        private int GetLastDisciplineID()
        {
            string query = "SELECT MAX(DisciplineID) FROM Discipline;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result) + 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        private int GetLastCurriculumID()
        {
            string query = "SELECT MAX(CurriculumID) FROM Curriculum;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result) + 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        private void LoadDisciplinesList()
        {
            List<string> disciplinesList = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DisciplineName FROM Discipline";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string disciplineName = reader["DisciplineName"].ToString();
                            disciplinesList.Add(disciplineName);
                        }
                    }
                }
            }
            
            DisciplineComboBox.Items.Clear();

            DisciplineComboBox.Items.AddRange(disciplinesList.ToArray());

            DisciplineComboBox1.Items.Clear();

            DisciplineComboBox1.Items.AddRange(disciplinesList.ToArray());
        }

        private void LoadSpecialtyList()
        {
            List<string> specialtyList = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT SpecialtyName FROM Specialty";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string specialtyName = reader["SpecialtyName"].ToString();
                            specialtyList.Add(specialtyName);
                        }
                    }
                }
            }

            SpecialtyComboBox.Items.Clear();

            SpecialtyComboBox.Items.AddRange(specialtyList.ToArray());
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

        private void GroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupID = GroupComboBox.SelectedIndex+1;
        }

        private void DisciplineComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            disciplineID = DisciplineComboBox.SelectedIndex+1;
        }

        private void DisciplineComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            disciplineID1 = (DisciplineComboBox.SelectedIndex) + 2;
        }

        private void SpecialtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            specialtyID = SpecialtyComboBox.SelectedIndex + 1;
        }

        private void SemesterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            semester = int.Parse(SemesterComboBox.Text);
        }

        private DataTable GetCurriculumDetails()
        {
            DataTable curriculumDetailsTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                C.CurriculumID,
                S.SpecialtyName,
                D.DisciplineName,
                C.Hours,
                C.Semester
            FROM Curriculum C
            INNER JOIN Specialty S ON C.SpecialtyID = S.SpecialtyID
            INNER JOIN Discipline D ON C.DisciplineID = D.DisciplineID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(curriculumDetailsTable);
                    }
                }
            }

            return curriculumDetailsTable;
        }

        private void DisplayCurriculumDetailsInDataGridView()
        {

            DataTable curriculumDetailsData = GetCurriculumDetails();

            if (curriculumDetailsData.Rows.Count > 0)
            {
                dataGridViewCurriculum.DataSource = curriculumDetailsData;
            }
            else
            {
                MessageBox.Show("Немає даних для відображення.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            dataGridViewCurriculum.Update();
        }

    }
}
