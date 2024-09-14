# Timesheet-Management
GUI for Timesheet Management using C# Windows Forms

<img width="437" alt="image" src="https://github.com/user-attachments/assets/ba98ddee-8e5a-40da-8a8c-3cd5eda91efc">

# C# Timesheet Application: Key Features

## Login Functionality:
The application includes a basic login system where the username is set to user and the password to password. No database is used for authentication but if the credentials are entered correctly, the user is directed to the next panel for timesheet entry. If incorrect credentials are provided, a message box will display an error alert.

## Time Validation:
If the exit time entered is earlier than the entry time, the system will display an error message: "Exit time cannot be earlier than entry time." <br>
<img width="227" alt="image" src="https://github.com/user-attachments/assets/dc2aee88-6e8c-4af1-b2fb-1dc7933dc617">


## Consistent Month Entries:
The application ensures that all time entries are from the same month. Navigation to other months is blocked to maintain consistency.<br>
<img width="425" alt="image" src="https://github.com/user-attachments/assets/7d3a3ee0-0046-43c5-b581-830162124e7d">


## Edit Functionality:
The exit button allows users to select and edit the existing row, enabling modifications to the time entries as needed.<br>
<img width="301" alt="image" src="https://github.com/user-attachments/assets/4a9387b2-9854-4e6f-abac-6c7b608e2e1d">


## CSV File Storage:
All timesheet data, including entry and exit times is saved to a CSV file for record-keeping and further analysis.<br>
<img width="271" alt="image" src="https://github.com/user-attachments/assets/b084e292-59ea-42ff-9b30-8f4057a19ff4">


## Daily Total Hours Calculation:
The application continuously calculates the total working hours for each day and updates them as entries are made.<br>
<img width="136" alt="image" src="https://github.com/user-attachments/assets/f1d1c397-2b29-405d-acb8-a7e4f24baee9">

