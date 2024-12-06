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
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

namespace movieRental
{
    public partial class editCustomer : UserControl
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        // Data
        public List<Movie> Movies;

        private Customer customerToEdit;

        public editCustomer(Customer customer)
        {
            InitializeComponent();

            this.customerToEdit = customer;

            loadInfo();
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

        private void editCustomer_Load(object sender, EventArgs e)
        {

        }

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

        private void LogOutButton_Click(object sender, EventArgs e)
        {
        }

        private void PhoneLabel_Click(object sender, EventArgs e)
        {

        }

        // Incomplete but framework in place to edit customer details in database
        private void saveChangesClick(object sender, EventArgs e)
        {
            if (fieldValidation())
            {

                try
                {
                    object cid = null;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string updateCustomer = "UPDATE Customer SET FirstName=@firstName, FamilyName=@lastName, Address=@address, City=@city, Province=@province, PostalCode=@postalCode, EmailAddress=@emailAddress, CreditCardNum=@creditCard OUTPUT inserted.CID where Customer.AccountNumber=@accountNumber";


                        using (SqlCommand cmd = new SqlCommand(updateCustomer, connection))
                        {
                            cmd.Parameters.AddWithValue("@firstName", FirstNameInput.Text);
                            cmd.Parameters.AddWithValue("@lastName", LastNameInput.Text);
                            cmd.Parameters.AddWithValue("@address", AddressInput.Text);
                            cmd.Parameters.AddWithValue("@city", CityInput.Text);
                            cmd.Parameters.AddWithValue("@province", ProvinceInput.Text);
                            cmd.Parameters.AddWithValue("@postalCode", PostalCodeInput.Text);
                            cmd.Parameters.AddWithValue("@emailAddress", EmailInput.Text);
                            cmd.Parameters.AddWithValue("@creditCard", creditCardInput.Text);
                            cmd.Parameters.AddWithValue("@accountNumber", customerToEdit.accountNumber);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    cid = reader[0]; // Get the first column (CID) from the result
                                    MessageBox.Show($"Customer CID: {cid}");
                                }
                                else
                                {
                                    MessageBox.Show("Customer ID not found!");
                                }
                            }
                        }

                        //MessageBox.Show($"{cid}");
                        // Query to delete all phone numbers for the customer
                        string deleteAllQuery = "DELETE FROM CustomerPhone WHERE CustomerPhone.CustomerID = @CID";

                        // Delete all old phone numbers
                        using (SqlCommand deleteAllCmd = new SqlCommand(deleteAllQuery, connection))
                        {
                            deleteAllCmd.Parameters.AddWithValue("@CID", cid);
                            deleteAllCmd.ExecuteNonQuery();
                        }

                        // Query to insert a new phone numbers
                        string insertQuery = "INSERT INTO CustomerPhone (CustomerID, PhoneNumber) VALUES (@CID, @newNumber)";

                        List<string> newPhoneNumbers = new List<string>
                                                {
                                                    PhoneNumberInput1.Text,
                                                    PhoneNumberInput2.Text,
                                                    PhoneNumberInput3.Text
                                                };
                        //MessageBox.Show($"{PhoneNumberInput1.Text}, {PhoneNumberInput2.Text}, {PhoneNumberInput3}");
                        // Insert each new phone number
                        foreach (string newNum in newPhoneNumbers)
                        {
                            //MessageBox.Show($"neew num = {newNum}");
                            if (!string.IsNullOrEmpty(newNum))
                            {
                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                                {
                                    insertCmd.Parameters.AddWithValue("@CID", cid);
                                    insertCmd.Parameters.AddWithValue("@newNumber", newNum);

                                    // Execute the insert command
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        MessageBox.Show("Customer updated succesfully !");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
        }

        private bool fieldValidation()
        {
            if (!string.IsNullOrEmpty(FirstNameInput.Text) &&
                !string.IsNullOrEmpty(LastNameInput.Text) &&
                !string.IsNullOrEmpty(EmailInput.Text))
            {
                return true;
            }

            return false;
        }

        private void loadInfo()
        {
            try
            {
                Customer? customer = null;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Customer WHERE AccountNumber = @AccountNumber;SELECT SCOPE_IDENTITY();";
                    object cid;
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.AddWithValue("@AccountNumber", customerToEdit.accountNumber);
                        cid = cmd.ExecuteScalar();

                        SqlDataReader myReader = cmd.ExecuteReader();

                        if (myReader.Read())
                        {
                            customer = new Customer()
                            {
                                firstName = myReader.GetString(1),
                                lastName = myReader.GetString(2),
                                address = myReader.IsDBNull(3) ? "" : myReader.GetString(3),
                                city = myReader.IsDBNull(4) ? "" : myReader.GetString(4),
                                province = myReader.IsDBNull(5) ? "" : myReader.GetString(5),
                                postalCode = myReader.IsDBNull(6) ? "" : myReader.GetString(6),
                                email = myReader.GetString(8),
                                creditCard = myReader.GetString(9),
                                CreationDate = myReader.GetDateTime(10)
                            };
                        }

                        myReader.Close();

                    }
                    // Using cid to obtain phone #'s
                    if (cid != null)
                    {
                        string query2 = "SELECT PhoneNumber FROM CustomerPhone WHERE CustomerID = @CID";
                        using (SqlCommand cmd2 = new SqlCommand(query2, connection))
                        {
                            cmd2.Parameters.AddWithValue("@CID", cid);

                            SqlDataReader myReader2 = cmd2.ExecuteReader();
                            List<string> phoneNumbers = new List<string>();

                            while (myReader2.Read())
                            {
                                phoneNumbers.Add(myReader2.GetString(0));
                            }
                            if (phoneNumbers.Count > 0)
                            {
                                PhoneNumberInput1.Text = phoneNumbers.Count > 0 ? phoneNumbers[0] : "";
                                PhoneNumberInput2.Text = phoneNumbers.Count > 1 ? phoneNumbers[1] : "";
                                PhoneNumberInput3.Text = phoneNumbers.Count > 2 ? phoneNumbers[2] : "";
                            }
                            else
                            {
                                PhoneNumberInput1.Text = "";
                                PhoneNumberInput2.Text = "";
                                PhoneNumberInput3.Text = "";
                            }



                            myReader2.Close();

                        }

                        // Set the form fields
                        FirstNameInput.Text = customer.firstName;
                        LastNameInput.Text = customer.lastName;
                        AddressInput.Text = customer.address;
                        CityInput.Text = customer.city;
                        ProvinceInput.Text = customer.province;
                        PostalCodeInput.Text = customer.postalCode;
                        EmailInput.Text = customer.email;
                        creditCardInput.Text = customer.creditCard;

                        // store current phone #'s in customer
                        customerToEdit.phoneNumber1 = !string.IsNullOrEmpty(PhoneNumberInput1.Text) ? PhoneNumberInput1.Text : "";
                        customerToEdit.phoneNumber2 = !string.IsNullOrEmpty(PhoneNumberInput2.Text) ? PhoneNumberInput2.Text : "";
                        customerToEdit.phoneNumber3 = !string.IsNullOrEmpty(PhoneNumberInput3.Text) ? PhoneNumberInput3.Text : "";
                    }
                    else
                    {
                        MessageBox.Show("Customer not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteCustomerClick(object sender, EventArgs e)
        {
            // Message Box with yes and no button options
            DialogResult result = MessageBox.Show(
            "Are you sure you want to delete this customer?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection

                    string query = "DELETE FROM Customer WHERE AccountNumber = @accountNumber;SELECT SCOPE_IDENTITY();";
                    object cid;
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@accountNumber", customerToEdit.accountNumber);
                        cmd.ExecuteNonQuery(); // Execute the delete command
                                               // Execute and retrieve the CID
                        cid = cmd.ExecuteScalar();
                    }

                }
                ClearFormFields();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFormFields()
        {
            FirstNameInput.Text = string.Empty;
            LastNameInput.Text = string.Empty;
            AddressInput.Text = string.Empty;
            CityInput.Text = string.Empty;
            ProvinceInput.Text = string.Empty;
            PostalCodeInput.Text = string.Empty;
            EmailInput.Text = string.Empty;
            creditCardInput.Text = string.Empty;
            PhoneNumberInput1.Text = string.Empty;
            PhoneNumberInput2.Text = string.Empty;
            PhoneNumberInput3.Text = string.Empty;
        }

        private void creditCardKeyPress(object sender, KeyPressEventArgs e)
        {

            // Allow only digits
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject non-digit characters
            }

            // Restrict character length
            if (creditCardInput.Text.Length >= 16 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void phoneNumber1Press(object sender, KeyPressEventArgs e)
        {
            // Allow only digits
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject non-digit characters
            }

            // Restrict character length
            if (PhoneNumberInput1.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void phoneNumber2Press(object sender, KeyPressEventArgs e)
        {
            // Allow only digits
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject non-digit characters
            }

            // Restrict character length
            if (PhoneNumberInput2.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void phoneNumber3Press(object sender, KeyPressEventArgs e)
        {
            // Allow only digits
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject non-digit characters
            }

            // Restrict character length
            if (PhoneNumberInput3.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void emailPress(object sender, KeyPressEventArgs e)
        {
            // Restrict character length
            if (EmailInput.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void emailValidation(object sender, CancelEventArgs e)
        {
            // Define the regex for basic email format
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(EmailInput.Text, emailPattern) && !string.IsNullOrEmpty(EmailInput.Text))
            {
                e.Cancel = true; // Prevent focus from leaving the textbox
                MessageBox.Show("Invalid email format: format ex. someone@gmail.com");
            }
        }

        private void postalCodePress(object sender, KeyPressEventArgs e)
        {

            // Restrict character length
            if (PostalCodeInput.Text.Length >= 6 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void postalCodeValidation(object sender, CancelEventArgs e)
        {
            // Define the regex for postal code format
            string postalCodePattern = @"^[A-Za-z]\d[A-Za-z]\d[A-Za-z]\d$";

            if (!Regex.IsMatch(PostalCodeInput.Text, postalCodePattern) && !string.IsNullOrEmpty(PostalCodeInput.Text))
            {
                e.Cancel = true; // Prevent focus from leaving the textbox
                MessageBox.Show("Invalid postal code format: format ex. RK46L2.");
            }
        }

        private void provincePress(object sender, KeyPressEventArgs e)
        {
            // Allow only chars
            if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject digit characters
            }

            // Restrict character length
            if (ProvinceInput.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void cityPress(object sender, KeyPressEventArgs e)
        {
            // Allow only chars
            if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject digit characters
            }

            // Restrict character length
            if (CityInput.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void addressPress(object sender, KeyPressEventArgs e)
        {
            // Restrict character length
            if (AddressInput.Text.Length >= 40 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void lastNamePress(object sender, KeyPressEventArgs e)
        {
            // Allow only chars
            if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject digit characters
            }

            // Restrict character length
            if (LastNameInput.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void firstNamePress(object sender, KeyPressEventArgs e)
        {
            // Allow only chars
            if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject digit characters
            }

            // Restrict character length
            if (FirstNameInput.Text.Length >= 40 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Reject further input if max length is reached
            }
        }

        private void logOutClick(object sender, EventArgs e)
        {
            // Dispose of current controls if needed
            foreach (Control control in this.Controls.OfType<UserControl>().ToList())
            {
                control.Dispose();
            }

            // Clear all controls on the current form
            this.Controls.Clear();

            // Create and add the Login control back to the form
            LoginUserControl loginScreen = new LoginUserControl();
            this.Controls.Add(loginScreen);
            loginScreen.Dock = DockStyle.Fill;
        }
    }
}
