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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Xml.Linq;
using System.Configuration;
using System.Drawing.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace movieRental
{
    public partial class prolificEmployee : UserControl
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        // Data
        public List<ReportThreeContainer> employees;

        public prolificEmployee()
        {
            InitializeComponent();

            PopulateMonthComboBox();
        }

        // Data Source
        private List<ReportThreeContainer>? retrieveTopEmployeesOfMonth(string selectedMonth)
        {
            var employees = new List<ReportThreeContainer>();
            int month = ConvertMonthToInt(selectedMonth);
            if (month == -1) return employees;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                String query =
@$"select Employee.EID, Employee.FirstName, Employee.FamilyName, count(Ordered.EmployeeID) as countEmp
from Employee, Ordered
where Employee.EID = Ordered.EmployeeID and Month(CheckOutDate) = '{month}' and year(CheckOutDate) = '2024'
group by EID, FirstName, FamilyName
order by countEmp desc";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    try
                    {
                        SqlDataReader myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            employees.Add(new ReportThreeContainer()
                            {
                                id = myReader.GetInt32(0),
                                FirstName = myReader.GetString(1),
                                LastName = myReader.GetString(2),
                                orders = myReader.GetInt32(3)
                            });

                        }

                        myReader.Close();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
            return employees;
        }

        // Date string to int
        private int ConvertMonthToInt(string month)
        {
            switch (month)
            {
                case "January": return 1;
                case "February": return 2;
                case "March": return 3;
                case "April": return 4;
                case "May": return 5;
                case "June": return 6;
                case "July": return 7;
                case "August": return 8;
                case "September": return 9;
                case "October": return 10;
                case "November": return 11;
                case "December": return 12;
                default: return -1;
            }
        }

        private void prolificEmployee_Load(object sender, EventArgs e)
        {

        }

        // Switch Screen
        private void SwitchToScreen(UserControl newScreen)
        {
            Form parentForm = this.FindForm();

            if (parentForm != null)
            {
                // Dispose of existing controls
                foreach (Control control in parentForm.Controls.OfType<UserControl>().ToList())
                {
                    control.Dispose();
                }

                // Clear and add the new screen
                parentForm.Controls.Clear();
                parentForm.Controls.Add(newScreen);
                newScreen.Dock = DockStyle.Fill;
            }
        }

        // Combo Box

        private void PopulateMonthComboBox()
        {
            var months = DateTimeFormatInfo.CurrentInfo.MonthNames;

            monthComboBox.Items.Clear();

            foreach (var month in months)
            {
                if (!string.IsNullOrEmpty(month))
                {
                    monthComboBox.Items.Add(month);
                }
            }
        }

        private void monthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMonth = monthComboBox.SelectedItem.ToString();

            topEmployeeDataView.DataSource = null;
            topEmployeeDataView.Columns.Clear();

            employees = retrieveTopEmployeesOfMonth(selectedMonth);

            topEmployeeDataView.AutoGenerateColumns = false;
            topEmployeeDataView.DataSource = employees;
            addTopEmployeesAttributeColoumns();
        }

        private void addTopEmployeesAttributeColoumns()
        {
            topEmployeeDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                DataPropertyName = "fullName",
            });

            topEmployeeDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Employee ID",
                DataPropertyName = "id",
            });

            topEmployeeDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Orders Completed",
                DataPropertyName = "orders",
            });
        }

        // Buttons

        private void CustomersButton_Click(object sender, EventArgs e)
        {
            SwitchToScreen(new customerScreen());
        }

        private void MoviesButton_Click(object sender, EventArgs e)
        {
            SwitchToScreen(new moviesScreen());
        }

        private void RentalsButton_Click(object sender, EventArgs e)
        {
            SwitchToScreen(new rentalScreen());
        }

        private void ReportsButton_Click(object sender, EventArgs e)
        {
            SwitchToScreen(new reportScreen());
        }

        private void viewQueueButton_Click(object sender, EventArgs e)
        {
            // Where is this button? Delete?
            //SwitchToScreen(new customerQueue());
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
        }
    }
}
