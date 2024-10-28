using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.Sql;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EPN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadConnectionString();
        }

        //private void LoadConnectionStrings()
        //{
        //    comboBoxConnectionStrings.Items.Clear();
        //    var instances = SqlDataSourceEnumerator.Instance.GetDataSources();

        //    // Check if any instances were retrieved
        //    if (instances.Rows.Count == 0)
        //    {
        //        MessageBox.Show("No SQL Server instances found.");
        //        return;
        //    }

        //    foreach (System.Data.DataRow row in instances.Rows)
        //    {
        //        string serverName = row["ServerName"].ToString();
        //        string instanceName = row["InstanceName"].ToString();

        //        // Combine server and instance names if an instance is available
        //        string fullServerName = string.IsNullOrEmpty(instanceName) ? serverName : $"{serverName}\\{instanceName}";

        //        // Add connection string to the ComboBox
        //        string connectionString = $"Data Source={fullServerName};Initial Catalog=delevery;User ID=sa;Password=ahmed";
        //        comboBoxConnectionStrings.Items.Add(connectionString);
        //    }

        //    // Optionally select the first item
        //    if (comboBoxConnectionStrings.Items.Count > 0)
        //    {
        //        comboBoxConnectionStrings.SelectedIndex = 0;
        //    }
        //    else
        //    {
        //        MessageBox.Show("No connection strings were added.");
        //    }
        //}

        //private string GetSelectedConnectionString()
        //{
        //    return comboBoxConnectionStrings.SelectedItem?.ToString();
        //}
        // Parse dates from various formats
        public DateTime? ParseDate(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return null;

            // Arabic month names and their numeric values
            Dictionary<string, int> arabicMonths = new Dictionary<string, int>
            {
                { "يناير", 1 }, { "فبراير", 2 }, { "مارس", 3 },
                { "أبريل", 4 }, { "مايو", 5 }, { "يونيو", 6 },
                { "يوليو", 7 }, { "أغسطس", 8 }, { "سبتمبر", 9 },
                { "أكتوبر", 10 }, { "نوفمبر", 11 }, { "ديسمبر", 12 }
            };

            // Parse Arabic date format: "14 أبريل, 2022"
            var parts = dateString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 3 && arabicMonths.TryGetValue(parts[1], out int month) &&
                int.TryParse(parts[0], out int day) && int.TryParse(parts[2], out int year))
            {
                try { return new DateTime(year, month, day); }
                catch (ArgumentOutOfRangeException) { return null; }
            }
            dataGridView1.Columns[1].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Parse ISO format: "2021/07/17" or "15/06/2023"
            string[] isoFormats = { "yyyy/MM/dd", "dd/MM/yyyy" };
            foreach (var format in isoFormats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    return parsedDate;
                }
            }

            return null;
        }
        private void LoadConnectionString()
        {
            // Use relative path to find the connection string in the program's folder
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connectionString.txt");

            try
            {
                if (File.Exists(filePath))
                {
                    string connectionString = File.ReadAllText(filePath); // Read content
                    textBox2.Text = connectionString; // Display in TextBox
                }
                else
                {
                    MessageBox.Show("Connection string file not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading connection string: {ex.Message}");
            }
        }        // Load data based on the full address
        public void LoadData(string searchTerm, DateTime fromDate, DateTime toDate)
        {
            List<Data> records = new List<Data>();
            string connectionString = textBox2.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT dat4, tel, namcust, ad1, ad2, ad3 FROM [delevery].[dbo].[del]", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string dateString = reader["dat4"].ToString();
                    string nameCust = reader["namcust"].ToString();
                    string address1 = reader["ad1"].ToString();
                    string address2 = reader["ad2"].ToString();
                    string address3 = reader["ad3"].ToString();
                    string phone = reader["tel"].ToString();

                    // Construct the full address
                    string fullAddress = $"{nameCust}, {address1}, {address2}, {address3}";

                    DateTime? parsedDate = ParseDate(dateString);
                    if (parsedDate.HasValue && fullAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        if (parsedDate.Value.Date >= fromDate.Date && parsedDate.Value.Date <= toDate.Date)
                        {
                            records.Add(new Data { Phone = phone, OrderDate = parsedDate.Value, FullAddress = fullAddress });
                        }
                    }
                }
            }

            // Clear existing rows if needed
            dataGridView1.Rows.Clear();

            // Populate the DataGridView with records
            foreach (var record in records)
            {
                dataGridView1.Rows.Add(record.Phone, record.OrderDate, record.FullAddress);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;
            DateTime fromDate = dateTimePicker1.Value; 
            DateTime toDate = dateTimePicker2.Value; 
            LoadData(searchTerm, fromDate, toDate);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "OrderDate" && e.Value is DateTime dateValue)
            {
                e.Value = dateValue.ToString("MM/dd/yyyy");
                e.FormattingApplied = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Create a StringBuilder to store the phone numbers
            var phoneNumbers = new System.Text.StringBuilder();

            // Iterate through the rows of the DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Check if the row is not a new row
                if (!row.IsNewRow)
                {
                    // Append the phone number (assuming it's in the first column)
                    phoneNumbers.AppendLine(row.Cells[0].Value?.ToString());
                }
            }

            // Copy the phone numbers to the clipboard
            Clipboard.SetText(phoneNumbers.ToString());
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (e.KeyChar == (char)Keys.Enter) // Check if Enter key is pressed
                {
                    string searchTerm = textBox1.Text;
                    DateTime fromDate = dateTimePicker1.Value; // Get the selected from date
                    DateTime toDate = dateTimePicker2.Value; // Get the selected to date
                    LoadData(searchTerm, fromDate, toDate); // Load data with the specified date range and search term
                }
            

        }
    }

    // Model class to represent the data
    public class Data
    {
        public Data()
        {
            FullAddress = string.Empty;
            Phone = string.Empty;
        }

        public DateTime? OrderDate { get; set; }
        public string FullAddress { get; set; }
        public string Phone { get; set; }
    }
}
