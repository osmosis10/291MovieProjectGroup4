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
    public partial class biggestFan : UserControl
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        //Data
        public List<Customer> Customers;
        public List<Actor> Actors;

        public biggestFan()
        {
            InitializeComponent();

            Actors = retrieveActor();

            actorDataView.AutoGenerateColumns = false;
            actorDataView.DataSource = Actors;
            addActorAttributeColoumns();
            addEditButtonColumn();
        }

        private void biggestFan_Load(object sender, EventArgs e)
        {

        }

        // Data Source
        private List<Actor>? retrieveActor()
        {
            var actors = new List<Actor>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                String query = "SELECT * FROM Actor";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    try
                    {
                        SqlDataReader myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            actors.Add(new Actor()
                            {
                                id = myReader.GetInt32(0),
                                firstName = myReader.GetString(1),
                                lastName = myReader.GetString(2),
                                gender = myReader.GetString(3),
                                dateOfBirth = myReader.GetDateTime(4)
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
            return actors;
        }

        private List<ReportFourContainer>? retrieveActorFans(int actorID)
        {
            var customers = new List<ReportFourContainer>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                String query =
@$"select count(*) as reviewAmt, avg(CAST(Rating AS FLOAT)) as avgRating, CID, concat(Customer.FirstName, ' ', Customer.FamilyName) as fullName
from ActorReview, Customer, Ordered
where ActorID = {actorID} and ActorReview.OrderID = Ordered.OID and Ordered.CustomerID = Customer.CID
group by CID, Customer.FirstName, Customer.FamilyName
order by avg(Rating) desc, count(*) desc;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    try
                    {
                        SqlDataReader myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            customers.Add(new ReportFourContainer()
                            {
                                reviewAmount = myReader.GetInt32(0),
                                avgRating = myReader.GetDouble(1),
                                id = myReader.GetInt32(2),
                                fullName = myReader.GetString(3)
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
            return customers;
        }

        // Add certain attribute coloumns manually
        private void addActorAttributeColoumns()
        {

            actorDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                DataPropertyName = "fullName",
            });

            actorDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date of Birth",
                DataPropertyName = "dateOfBirth",
            });
        }

        private void addFanAttributeColoumns()
        {

            biggestFanDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                DataPropertyName = "id",
            });

            biggestFanDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                DataPropertyName = "fullName",
            });

            biggestFanDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Average Rating",
                DataPropertyName = "avgRating",
            });

            biggestFanDataView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Review Amount",
                DataPropertyName = "reviewAmount",
            });
        }

        // Add a button column for editing
        private void addEditButtonColumn()
        {
            DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();

            editButtonColumn.Name = "Select";
            editButtonColumn.HeaderText = "Select";
            editButtonColumn.Text = "Select";
            editButtonColumn.UseColumnTextForButtonValue = true;

            actorDataView.Columns.Add(editButtonColumn);
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

        private void actorDataView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && actorDataView.Columns[e.ColumnIndex].Name == "Select")
            {
                var selectedActor = actorDataView.Rows[e.RowIndex].DataBoundItem as Actor;

                if (selectedActor != null)
                {
                    // Retrieve customers for the selected actor
                    var customers = retrieveActorFans(selectedActor.id);

                    // Clear existing data and columns
                    biggestFanDataView.DataSource = null;
                    biggestFanDataView.Columns.Clear();

                    // Configure columns and bind data
                    biggestFanDataView.AutoGenerateColumns = false;
                    addFanAttributeColoumns();
                    biggestFanDataView.DataSource = customers;

                    // Update label
                    biggestFanLabel.Text = $"{selectedActor.fullName}'s Biggest Fans:";
                }
            }
        }

        private void actorSearch__TextChanged(object sender, EventArgs e)
        {
            var temp = Actors.Where(
            c => c.fullName.Contains(actorSearch.Text))
            .ToList();
            actorDataView.DataSource = temp;
        }
    }
}
