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
using System.Windows.Forms;
using System.Security.Cryptography;

namespace movieRental
{
    public partial class grossingMovies : UserControl
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        // Data
        public List<ReportTwoContainer> Movies;

        public grossingMovies()
        {
            InitializeComponent();

            PopulateMonthComboBox();
        }

        private void grossingMovies_Load(object sender, EventArgs e)
        {
        }

        // Data Source
        private List<ReportTwoContainer>? retrieveTopMoviesOfMonth(string selectedMonth)
        {
            var movies = new List<ReportTwoContainer>();
            int month = ConvertMonthToInt(selectedMonth);
            if (month == -1) return movies;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                String query = 
                @$"
with profits as (
    select count(Movie.MID) * DistributionFee as profit, Movie.MID, Name
    from Ordered, Movie
    where Ordered.MovieID = Movie.MID and month(CheckOutDate) = '{month}' and year(CheckOutDate) = '2024'
group by Movie.MID, DistributionFee, Name
)
select Name, profit
from Ordered, profits
where profits.MID = Ordered.MovieID--replace month and year
group by Name, profit
order by profit desc";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    try
                    {
                        SqlDataReader myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            movies.Add(new ReportTwoContainer()
                            {
                                Name = myReader.GetString(0),
                                Profit = myReader.GetDecimal(1)
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
            return movies;

        }

        // Date string to int
        private int ConvertMonthToInt(string month)
        {
            switch(month)
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

            topMovieDataView.DataSource = null;
            topMovieDataView.Columns.Clear();

            Movies = retrieveTopMoviesOfMonth(selectedMonth);

            topMovieDataView.AutoGenerateColumns = false;
            topMovieDataView.DataSource = Movies;
            addTopMovieAttributeColoumns();
        }

        private void addTopMovieAttributeColoumns()
        {
            topMovieDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Title",
                DataPropertyName = "Name",
            });

            topMovieDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Profit",
                DataPropertyName = "Profit",
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
