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
using System.Security.Cryptography;

namespace movieRental
{
    public partial class highestRatedGenreMovie : UserControl
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        //Data
        public List<ReportFiveContainer> actionMovies;
        public List<ReportFiveContainer> comedyMovies;
        public List<ReportFiveContainer> dramaMovies;
        public List<ReportFiveContainer> foreignMovies;

        public highestRatedGenreMovie()
        {
            InitializeComponent();

            actionMovies = retrieveMovies("Action");
            comedyMovies = retrieveMovies("Comedy");
            dramaMovies = retrieveMovies("Drama");
            foreignMovies = retrieveMovies("Foreign");

            topActionDataView.AutoGenerateColumns = false;
            topComedyDataView.AutoGenerateColumns = false;
            topDramaDataView.AutoGenerateColumns = false;
            topForeignDataView.AutoGenerateColumns = false;

            topActionDataView.DataSource = actionMovies;
            topComedyDataView.DataSource = comedyMovies;
            topDramaDataView.DataSource = dramaMovies;
            topForeignDataView.DataSource = foreignMovies;

            addMovieAttributeColoumns();
        }

        private void highestRatedGenreMovie_Load(object sender, EventArgs e)
        {

        }

        // Data Source
        private List<ReportFiveContainer>? retrieveMovies(string genre)
        {
            var movies = new List<ReportFiveContainer>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                String query =
@$"select Name, Type, count(*) as reviewCount, AVG(CAST(MovieRating AS FLOAT)) as avgRating
from Movie, Ordered
where Movie.MID = Ordered.MovieID and Type = '{genre}'
group by Name, Type
order by avgRating desc, count(*) desc;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    try
                    {
                        SqlDataReader myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            movies.Add(new ReportFiveContainer()
                            {
                                title = myReader.GetString(0),
                                reviewCount = myReader.GetInt32(2),
                                avgRating = myReader.GetDouble(3),
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

        // Add certain attribute coloumns manually
        private void addMovieAttributeColoumns()
        {
            topActionDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Rank",
                Name = "rank",
            });

            topComedyDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Rank",
                Name = "rank",
            });

            topDramaDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Rank",
                Name = "rank",
            });

            topForeignDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Rank",
                Name = "rank",
            });

            topActionDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Title",
                DataPropertyName = "title",
            });

            topComedyDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Title",
                DataPropertyName = "title",
            });

            topDramaDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Title",
                DataPropertyName = "title",
            });

            topForeignDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Title",
                DataPropertyName = "title",
            });
            
            topActionDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Review Count",
                DataPropertyName = "reviewCount",
            });

            topComedyDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Review Count",
                DataPropertyName = "reviewCount",
            });

            topDramaDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Review Count",
                DataPropertyName = "reviewCount",
            });

            topForeignDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Review Count",
                DataPropertyName = "reviewCount",
            });
            //
            topActionDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Average Rating",
                DataPropertyName = "avgRating",
            });

            topComedyDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Average Rating",
                DataPropertyName = "avgRating",
            });

            topDramaDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Average Rating",
                DataPropertyName = "avgRating",
            });

            topForeignDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Average Rating",
                DataPropertyName = "avgRating",
            });
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

        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var dataGridView = sender as DataGridView;

            if (dataGridView != null)
            {
                // Iterate through the rows and assign the index
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    dataGridView.Rows[i].Cells["Rank"].Value = i + 1;
                }
            }
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
