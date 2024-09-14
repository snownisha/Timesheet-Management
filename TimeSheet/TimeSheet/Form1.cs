using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TimeSheet
{
    public partial class Form1 : Form
    {
        private HashSet<DateTime> enteredDates = new HashSet<DateTime>();
        private int editingRowIndex = -1; // -1 indicates no row is being edited
        private double runningTotalHours = 0; // Track running total of hours

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";

            dateTimePicker2.Format = DateTimePickerFormat.Time;
            dateTimePicker2.ShowUpDown = true;

            dateTimePicker3.Format = DateTimePickerFormat.Time;
            dateTimePicker3.ShowUpDown = true;

            InitializeDataGridViewColumns();

            panel2.Visible = true;
            panel1.Visible = false;
        }

        private void InitializeDataGridViewColumns()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("Date", "Date");
            dataGridView1.Columns.Add("EntryTime", "Entry Time");
            dataGridView1.Columns.Add("ExitTime", "Exit Time");
            dataGridView1.Columns.Add("TotalHours", "Total Hours");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value.Date;

            // Ensure all dates are in the same month
            if (dataGridView1.Rows.Count > 0)
            {
                // Get the date of the first row to compare months
                string cellValue = dataGridView1.Rows[0].Cells[1].Value?.ToString();
                DateTime initialDate = !string.IsNullOrEmpty(cellValue)
                    ? DateTime.ParseExact(cellValue, "dd/MM/yyyy", null)
                    : DateTime.Now; // Fallback date if cell value is null or empty

                // Check if the selected date is in the same month and year as the initial date
                if (selectedDate.Month != initialDate.Month || selectedDate.Year != initialDate.Year)
                {
                    MessageBox.Show("All entries must be for the same month.", "Month Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Check if date already exists
            if (editingRowIndex == -1)
            {
                if (enteredDates.Contains(selectedDate))
                {
                    MessageBox.Show("You have already entered data for this date. Please select a different date.",
                                    "Duplicate Date",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                enteredDates.Add(selectedDate);
            }

            // Add or update the DataGridView
            if (editingRowIndex == -1)
            {
                // Adding a new entry
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = (n + 1).ToString(); // Entry number
                dataGridView1.Rows[n].Cells[1].Value = selectedDate.ToString("dd/MM/yyyy");

                dataGridView1.Rows[n].Cells[2].Value = dateTimePicker2.Value.TimeOfDay.ToString(@"hh\:mm");
                dataGridView1.Rows[n].Cells[3].Value = dateTimePicker3.Value.TimeOfDay.ToString(@"hh\:mm");

                TimeSpan entryTime = TimeSpan.Parse(dataGridView1.Rows[n].Cells[2].Value.ToString());
                TimeSpan exitTime = TimeSpan.Parse(dataGridView1.Rows[n].Cells[3].Value.ToString());

                if (exitTime < entryTime)
                {
                    MessageBox.Show("Exit time cannot be earlier than entry time.", "Invalid Time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dataGridView1.Rows.RemoveAt(n);
                    return;
                }

                double totalHours = (exitTime - entryTime).TotalHours;
                dataGridView1.Rows[n].Cells[4].Value = totalHours.ToString("F2");

                // Update running total
                runningTotalHours += totalHours;
                UpdateRunningTotal();

                // Display success message after adding a new row
                MessageBox.Show("New entry added successfully.");

                // Check if all days of the month are entered
                if (AreAllDaysEntered())
                {
                    PromptForSubmission();
                }

                SaveToCSV();
            }
            else
            {
                // Editing an existing entry
                double oldTotalHours = double.Parse(dataGridView1.Rows[editingRowIndex].Cells[4].Value.ToString());

                dataGridView1.Rows[editingRowIndex].Cells[1].Value = selectedDate.ToString("dd/MM/yyyy");
                dataGridView1.Rows[editingRowIndex].Cells[2].Value = dateTimePicker2.Value.TimeOfDay.ToString(@"hh\:mm");
                dataGridView1.Rows[editingRowIndex].Cells[3].Value = dateTimePicker3.Value.TimeOfDay.ToString(@"hh\:mm");

                TimeSpan entryTime = TimeSpan.Parse(dataGridView1.Rows[editingRowIndex].Cells[2].Value.ToString());
                TimeSpan exitTime = TimeSpan.Parse(dataGridView1.Rows[editingRowIndex].Cells[3].Value.ToString());

                if (exitTime < entryTime)
                {
                    MessageBox.Show("Exit time cannot be earlier than entry time.", "Invalid Time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double totalHours = (exitTime - entryTime).TotalHours;
                dataGridView1.Rows[editingRowIndex].Cells[4].Value = totalHours.ToString("F2");

                // Update running total
                runningTotalHours = runningTotalHours - oldTotalHours + totalHours;
                UpdateRunningTotal();

                editingRowIndex = -1;
                MessageBox.Show("Entry updated successfully", "Success", MessageBoxButtons.OK);
                SaveToCSV();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                editingRowIndex = dataGridView1.SelectedRows[0].Index;
                dateTimePicker1.Value = DateTime.ParseExact(dataGridView1.Rows[editingRowIndex].Cells[1].Value.ToString(), "dd/MM/yyyy", null);

                TimeSpan entryTime = TimeSpan.Parse(dataGridView1.Rows[editingRowIndex].Cells[2].Value.ToString());
                TimeSpan exitTime = TimeSpan.Parse(dataGridView1.Rows[editingRowIndex].Cells[3].Value.ToString());

                DateTime entryDateTime = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day)
                    .Add(entryTime);
                DateTime exitDateTime = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day)
                    .Add(exitTime);

                dateTimePicker2.Value = entryDateTime;
                dateTimePicker3.Value = exitDateTime;

                MessageBox.Show("You are editing the selected row. Modify the values and click Submit to update.", "Editing Mode", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Please select the row you want to edit.");
            }
        }

        private void PromptForSubmission()
        {
            DialogResult result = MessageBox.Show("You have entered data for all days of the month. Do you want to submit the data?",
                                                  "Submit Data",
                                                  MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                double totalMonthlyHours = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[4].Value != null)
                    {
                        totalMonthlyHours += double.Parse(row.Cells[4].Value.ToString());
                    }
                }
                MessageBox.Show($"Total hours worked for the month: {totalMonthlyHours:F2} hours. Also saved to the CSV file");
                ClearEntries();
            }
            else
            {
                MessageBox.Show("You can continue editing the entries.");
            }
        }

        private void ClearEntries()
        {
            dataGridView1.Rows.Clear();
            enteredDates.Clear();
            editingRowIndex = -1;
            runningTotalHours = 0;
            UpdateRunningTotal();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridView1.Rows.Clear();
            enteredDates.Clear();

            runningTotalHours = 0;
            UpdateRunningTotal();

            int daysInMonth = DateTime.DaysInMonth(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, day);
                dataGridView1.Rows.Add(day.ToString(), date.ToString("dd/MM/yyyy"), "", "", "");
                enteredDates.Add(date);
            }
        }

        private bool AreAllDaysEntered()
        {
            int daysInMonth = DateTime.DaysInMonth(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month);
            return enteredDates.Count == daysInMonth;
        }

        private void UpdateRunningTotal()
        {
            label1.Text = $"Running Total Hours: {runningTotalHours:F2}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text;
            string password = textBox2.Text;
            if (userName == "user" && password == "password")
            {
                panel2.Visible = false;
                panel1.Visible = true;

                panel1.Location = panel2.Location;

                textBox1.Clear();
                textBox2.Clear();

                MessageBox.Show("Login Successful!");
            }
            else
            {
                MessageBox.Show("Error in the User Name or Password!");
            }
        }

        private void SaveToCSV()
        {
            StringBuilder csvContent = new StringBuilder();

            // Adding header row
            csvContent.AppendLine("ID,Date,Entry Time,Exit Time,Total Hours");

            // Loop through each row in the DataGridView and append the values
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow) // Exclude the last empty row
                {
                    var id = row.Cells["Id"].Value?.ToString();
                    var date = row.Cells["Date"].Value?.ToString();
                    var entryTime = row.Cells["EntryTime"].Value?.ToString();
                    var exitTime = row.Cells["ExitTime"].Value?.ToString();
                    var totalHours = row.Cells["TotalHours"].Value?.ToString();

                    csvContent.AppendLine($"{id},{date},{entryTime},{exitTime},{totalHours}");
                }
            }

            // Save the CSV content to a file
            string filePath = "timesheet_data.csv"; // You can modify the path as needed
            File.WriteAllText(filePath, csvContent.ToString());
        }
    }
}
