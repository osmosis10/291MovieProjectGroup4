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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using skeleton;

namespace movieRental
{
    public partial class addCustomer : UserControl
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public addCustomer()
        {
            InitializeComponent();
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

        private void addCustomer_Load(object sender, EventArgs e)
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

        private void addCustomerClick(object sender, EventArgs e)
        {
            if (fieldValidation())
            {

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"INSERT INTO Customer (FirstName, FamilyName, Address, City, Province, PostalCode, AccountNumber, EmailAddress, CreditCardNum) 
                        VALUES (@firstName, @lastName, @address, @city, @province, @postalCode, NEXT VALUE FOR Customer_AccountNumber, @emailAddress, @creditCardNum);
                        SELECT SCOPE_IDENTITY();";
                        object cid;

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            // Add parameters to prevent SQL injection
                            cmd.Parameters.AddWithValue("@firstName", FirstNameInput.Text);
                            cmd.Parameters.AddWithValue("@lastName", LastNameInput.Text);
                            cmd.Parameters.AddWithValue("@address", AddressInput.Text);
                            cmd.Parameters.AddWithValue("@city", CityInput.Text);
                            cmd.Parameters.AddWithValue("@province", ProvinceInput.Text);
                            cmd.Parameters.AddWithValue("@postalCode", PostalCodeInput.Text);
                            cmd.Parameters.AddWithValue("@emailAddress", EmailInput.Text);
                            cmd.Parameters.AddWithValue("@creditCardNum", creditCardInput.Text);

                            // Execute and retrieve the CID
                            cid = cmd.ExecuteScalar();
                        }

                        if (cid != null)
                        {
                            string query2 = "INSERT INTO CustomerPhone(CustomerID, PhoneNumber) VALUES (@CID, @phoneNumber)";

                            // List to store numbers
                            List<string> phoneNumbers = new List<string>
                                                {
                                                    PhoneNumberInput1.Text,
                                                    PhoneNumberInput2.Text,
                                                    PhoneNumberInput3.Text
                                                };

                            foreach (string curNum in phoneNumbers)
                            {
                                if (!string.IsNullOrEmpty(curNum))
                                {
                                    using (SqlCommand cmd = new SqlCommand(query2, connection))
                                    {
                                        // Add the @CID and @phoneNumber parameters
                                        cmd.Parameters.AddWithValue("@CID", cid);
                                        cmd.Parameters.AddWithValue("@phoneNumber", curNum);

                                        int rowsAffected = cmd.ExecuteNonQuery();

                                        if (rowsAffected == 0)
                                        {
                                            MessageBox.Show("Error: Record not inserted correctly");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    MessageBox.Show("Customer inserted succesfully !");
                    ClearFormFields();
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
