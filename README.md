# EPN - Database Handling System

EPN is a **WinForms-based desktop application** developed in **C#** that interacts with a messy SQL database, extracts data, and presents it in a structured manner. This tool provides search and filtering capabilities for delivery records, processes dates (including Arabic month names), and allows users to copy phone numbers directly from the results. 

The application is designed to **simplify database querying and address formatting issues**, with a focus on user-friendly interaction.

## Features
### 🔍 **Search & Filter Data**
- **Search by Address:** Enter part of the customer's address to filter relevant records.
- **Filter by Date Range:** Use **two DateTimePickers** to select a start and end date for records.

### 📋 **Clipboard Copy Functionality**
- Quickly **copy phone numbers** from the DataGridView to the clipboard with a button click.

### 🗓️ **Date Parsing & Formatting**
- Handles **Arabic month names** and converts various date formats into a consistent structure.
- Displays dates in **US format (MM/dd/yyyy)** within the DataGridView.

### 🔌 **Dynamic Connection String Handling**
- Loads the SQL **connection string from a text file** (`connectionString.txt`) to support different environments without recompilation.
- Ensures smooth data retrieval from the SQL database.

---

