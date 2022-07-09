using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Comp255_FinalProject_AkilMomin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Invoice> CustomerRecords = new List<Invoice>();  // Declaring List Box as Global.
        List<InvoiceItem> CustomerItems = new List<InvoiceItem>(); // Declaring List Box as Global.
        int CurrentInvoicesIndex;    // Declaring variable to record current index of invoice.
        int CurrentItemIndex;        // Declaring variable to record current index of item.
        Invoice CurrentInvoiceRecord;  // Declaring variable to record the current selected object from Invoice.
        InvoiceItem CurrentItemRecord; // Declaring variable to record the current selected object from Item.
        decimal GST, PST, Subtotal, TotalPrice;  // Declaring variables.

        public MainWindow()
        {
            InitializeComponent();

            LoadInvoices();  // Load the data from the LoadInvoice() Method.
        }

        void LoadInvoices()    // This methods loads the invoice data from SQL Database. 
        {
            InvoicesListbox.Items.Clear();
            CustomerRecords.Clear();
            InvoicesListbox.Items.Add($"{"Invoice ID",-20}{"Customer",-20}{"Email",-28}{"Shipped"}");

            using (SqlConnection Connection = new SqlConnection())  // Connection creates an instance of SqlConnection Method under Using Scope.
            {
                Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True";  // Directory of SQL database.
                Connection.Open();

                string Sql = "SELECT * FROM Invoices;"; // Sql Command query.

                SqlCommand MyCommand = new SqlCommand(Sql, Connection); // This creates an instance of SqlCommand with parameters of Sql and Connection.

                using (SqlDataReader Reader = MyCommand.ExecuteReader()) // Reader instance of SqlDataReader reades the Sql Database.
                {

                    while (Reader.Read())    // This Loops through the data from Sql Database and records it in Newinvoices object. 
                    {

                        Invoice NewInvoices = new Invoice((int)Reader["InvoiceID"],
                                                          (DateTime)Reader["InvoiceDate"],
                                                          (bool)Reader["Shipped"],
                                                          (string)Reader["CustomerName"],
                                                          (string)Reader["CustomerAddress"],
                                                          (string)Reader["CustomerEmail"]);
                        NewInvoices.SetShipped(NewInvoices.Shipped);  // This method converts the true or false to Yes or No for shipping.
                        CustomerRecords.Add(NewInvoices);             // This adds the records to CustomerRecords List.
                        InvoicesListbox.Items.Add(NewInvoices);       // This adds the records to Listbox
                    }
                }
            }
        }

        void LoadInvoiceItems() // This methods loads the invoice items data from SQL Database.
        {
            InvoiceItemsListbox.Items.Clear();
            CustomerItems.Clear();
            Subtotal = 0;
            PST = 0;
            GST = 0;
            TotalPrice = 0;

            if (CurrentInvoiceRecord == null)   // This statement will execute if the current selected object is null.
            {
                InvoiceItemsListbox.Items.Clear();
                return;
            }
            else  // The else will execute if the CurrentInvoiceRecord object is not null.
            {
                InvoiceItemsListbox.Items.Add($"{"Item ID",-15}{"Item Name",-15}{"Item Description",-25}{"Item Price",-15}{"Item Quantity",-19}{"Price"}");

                using (SqlConnection Connection = new SqlConnection())   // Connection creates an instance of SqlConnection Method under Using Scope.
                {
                    Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True";  // Directory of SQL database.
                    Connection.Open();

                    string Sql = $"SELECT * FROM InvoiceItems WHERE InvoiceID = {CurrentInvoiceRecord.InvoiceID};";   // Sql Command query.

                    SqlCommand MyCommand = new SqlCommand(Sql, Connection); // This creates an instance of SqlCommand with parameters of Sql and Connection.

                    using (SqlDataReader Reader = MyCommand.ExecuteReader()) // Reader instance of SqlDataReader executes the Sql Database functions.
                    {

                        while (Reader.Read()) // This Loops through the data from Sql Database and records it in NewinvoiceItems object.
                        {

                            InvoiceItem NewInvoiceItems = new InvoiceItem((int)Reader["ItemID"],
                                                          (int)Reader["InvoiceID"],
                                                          (string)Reader["ItemName"],
                                                          (string)Reader["ItemDescription"],
                                                          (decimal)Reader["ItemPrice"],
                                                          (int)Reader["ItemQuantity"]);

                            NewInvoiceItems.EachItemTotal = NewInvoiceItems.ItemPrice; // This calculates the total of each item.
                            PST += NewInvoiceItems.EachItemTotal * 6 / 100;   // Calculation of PST.
                            GST += NewInvoiceItems.EachItemTotal * 5 / 100;   // Calculation of GST
                            Subtotal += NewInvoiceItems.EachItemTotal;        // Adding subtotal to subtotal variable.
                            TotalPrice = Subtotal + PST + GST;                // Adding Subtotal PST and GST to TotalPrice.
                            DisplayCalculated();                              // Displays the records in the textbox of Subtotal,PST,GST and Total.
                            CustomerItems.Add(NewInvoiceItems);               // This adds the NewInvoiceItems to CustomerItems List.
                            InvoiceItemsListbox.Items.Add(NewInvoiceItems);   // This adds the NewInvoiceItems to InvoiceItems Listbox.

                        }
                    }

                    Connection.Close();
                }
            }
        }

        private void InvoicesListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;

            CurrentInvoicesIndex = InvoicesListbox.SelectedIndex;  // Recording the index of current selected item.

            try   // Using try and catch to see if the Current Selected record is null or not.
            {
                CurrentInvoiceRecord = (Invoice)InvoicesListbox.SelectedItem;

            }
            catch (InvalidCastException)
            {
                InvoiceItemsListbox.Items.Clear();
                InvoiceIDTextbox.Text = "";
                InvoiceDateTextbox.Text = "";
                CustomerNameTextbox.Text = "";
                CustomerAddressTextbox.Text = "";
                CustomerEmailTextbox.Text = "";
                ShippedCheckbox.IsChecked = false;
                CurrentInvoiceRecord = null;
                SubtotalTextbox.Text = "";
                PSTTextbox.Text = "";
                GSTTextbox.Text = "";
                TotalTextbox.Text = "";
                return;
            }

            DisplayInvoices();   // This Displays the records in text box of invoice.
            LoadInvoiceItems();  // This loads the InvoiceItems
            DisplayItems();      // This updates all the invoice TextBox
            DisplayCalculated(); // This updates Subtotal,GST,PST and Total.
        }

        void DisplayInvoices()   // This method displays the values to their particular text box if the object is not null.
        {
            if (CurrentInvoiceRecord != null)
            {
                InvoiceIDTextbox.Text = Convert.ToString(CurrentInvoiceRecord.InvoiceID);
                InvoiceDateTextbox.Text = CurrentInvoiceRecord.InvoiceDate.ToShortDateString();
                CustomerNameTextbox.Text = Convert.ToString(CurrentInvoiceRecord.CustomerName);
                CustomerAddressTextbox.Text = Convert.ToString(CurrentInvoiceRecord.CustomerAddress);
                CustomerEmailTextbox.Text = Convert.ToString(CurrentInvoiceRecord.CustomerEmail);
                ShippedCheckbox.IsChecked = Convert.ToBoolean(CurrentInvoiceRecord.Shipped);

            }
            else
            {
                InvoiceIDTextbox.Text = "";
                InvoiceDateTextbox.Text = "";
                CustomerNameTextbox.Text = "";
                CustomerAddressTextbox.Text = "";
                CustomerEmailTextbox.Text = "";
                ShippedCheckbox.IsChecked = false;

            }
        }

        void DisplayCalculated()  // This method displays the value to their particular textbox if the object is not null. 
        {
            if (CurrentInvoiceRecord != null)
            {
                SubtotalTextbox.Text = $"{Subtotal:N2}";
                PSTTextbox.Text = $"{PST:N2}";
                GSTTextbox.Text = $"{GST:N2}";
                TotalTextbox.Text = $"{TotalPrice:N2}";
            }
            else
            {
                SubtotalTextbox.Text = "";
                PSTTextbox.Text = "";
                GSTTextbox.Text = "";
                TotalTextbox.Text = "";
            }
        }

        void DisplayItems() // This method displays the record into the textbox under invoices items if the object is not null.
        {
            if (CurrentItemRecord != null)
            {
                ItemIDTextbox.Text = Convert.ToString(CurrentItemRecord.ItemID);
                ItemNameTextbox.Text = CurrentItemRecord.ItemName;
                ItemDescriptionTextbox.Text = CurrentItemRecord.ItemDescription;
                ItemPriceTextbox.Text = $"{CurrentItemRecord.ItemPrice:N2}";
                ItemQuantitytextbox.Text = Convert.ToString(CurrentItemRecord.ItemQuantity);

            }
            else
            {
                ItemIDTextbox.Text = "";
                ItemNameTextbox.Text = "";
                ItemDescriptionTextbox.Text = "";
                ItemPriceTextbox.Text = "";
                ItemQuantitytextbox.Text = "";
            }
        }

        bool IsDataInvoiceValid()  // This method is used in validating the data entered in thier particular textbox.
        {
            if (InvoiceDateTextbox.Text == "")
            {
                ErrorMessageLabel.Content = "Invoice Date cannot be blank";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            if (CustomerNameTextbox.Text == "")
            {
                ErrorMessageLabel.Content = "Customer Name cannot be blank";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            if (CustomerEmailTextbox.Text == "")
            {
                ErrorMessageLabel.Content = "Customer Email cannot be blank";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            else
            {
                return true;
            }
        }

        bool IsDataInvoicesItemValid()      // This method is used in validating the data entered in thier particular textbox.
        {
            if (ItemNameTextbox.Text == "")
            {
                ErrorMessageLabel.Content = "Item Name cannot be blank";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            if (ItemDescriptionTextbox.Text == "")
            {
                ErrorMessageLabel.Content = "Item Description cannot be blank";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            if (ItemPriceTextbox.Text == "")
            {
                ErrorMessageLabel.Content = "Item Price cannot be blank";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            if (ItemQuantitytextbox.Text == "")
            {
                ErrorMessageLabel.Content = "Item Quantity cannot be blank";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            if (Convert.ToDecimal(ItemPriceTextbox.Text) < 0)
            {
                ErrorMessageLabel.Content = "Item Price cannot be negative";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            if (Convert.ToInt32(ItemQuantitytextbox.Text) < 0)
            {
                ErrorMessageLabel.Content = "Item Quantity cannot be negative";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void InvoiceItemsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;

            CurrentItemIndex = InvoiceItemsListbox.SelectedIndex;   // Recording the index of current selected item in listbox.

            try  // Using try and catch to see if the Current Selected record is null or not.
            {
                CurrentItemRecord = (InvoiceItem)InvoiceItemsListbox.SelectedItem;
            }
            catch (InvalidCastException)
            {
                ItemIDTextbox.Text = "";
                ItemNameTextbox.Text = "";
                ItemDescriptionTextbox.Text = "";
                ItemPriceTextbox.Text = "";
                ItemQuantitytextbox.Text = "";

                SubtotalTextbox.Text = "";
                PSTTextbox.Text = "";
                GSTTextbox.Text = "";
                TotalTextbox.Text = "";
                return;
            }

            DisplayItems();  // This updates the record of items on every selection.
            DisplayCalculated(); // This updates the calculation on every selection.
        }

        private void SaveInvoiceItemButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;


            if (CurrentItemRecord == null)  // If statement will execute if the Current selected object is null. 
            {
                ErrorMessageLabel.Content = "Cannot save this record.";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return;
            }

            if (IsDataInvoicesItemValid() == false)   // This statement validates the data entered in textbox.
            {
                return;
            }
            else   // else will execute if the data validation returns true.
            {
                CurrentItemRecord.ItemName = Convert.ToString(ItemNameTextbox.Text);
                CurrentItemRecord.ItemDescription = Convert.ToString(ItemDescriptionTextbox.Text);
                CurrentItemRecord.ItemPrice = Convert.ToDecimal(ItemPriceTextbox.Text);
                CurrentItemRecord.ItemQuantity = Convert.ToInt32(ItemQuantitytextbox.Text);

                using (SqlConnection Connection = new SqlConnection())  // Creating an instance of SqlConnection to perform Sql Database functions.
                {
                    Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True";  //Sql Database file directory
                    Connection.Open();

                    string Sql = "UPDATE InvoiceItems SET " +  // This SQL syntax will be used to update the data into Sql database
                                                         $"InvoiceID = '{CurrentItemRecord.InvoiceID}', " +
                                                         $"ItemName = '{CurrentItemRecord.ItemName}'," +
                                                         $"ItemDescription = '{CurrentItemRecord.ItemDescription}', " +
                                                         $"ItemPrice = '{CurrentItemRecord.ItemPrice}'," +
                                                         $"ItemQuantity = '{CurrentItemRecord.ItemQuantity}' " +
                                                         $"WHERE ItemID = {CurrentItemRecord.ItemID};";

                    SqlCommand MyCommand = new SqlCommand(Sql, Connection); // This creates an instance of SqlCommand with parameters of Sql and Connection.

                    MyCommand.ExecuteNonQuery(); // This updates the record into Sql database.
                    Connection.Close();

                }

                LoadInvoiceItems(); // This reloads the data and display it to the user.
            }
        }

        private void NewInvoiceItemButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;

            if (IsDataInvoicesItemValid() == false) // if statement validates the data from the text box and executes if its false.
            {
                return;
            }
            else  // else will execute if the data validation returns true.
            {
                string Sql;
                InvoiceItem NewInvoice = new InvoiceItem();   // Creating an instance of invoice item.

                // This saves the record enterd from textbox.
                NewInvoice.InvoiceID = CurrentInvoiceRecord.InvoiceID;
                NewInvoice.ItemName = ItemNameTextbox.Text;
                NewInvoice.ItemDescription = ItemDescriptionTextbox.Text;
                NewInvoice.ItemPrice = Convert.ToDecimal(ItemPriceTextbox.Text);
                NewInvoice.ItemQuantity = Convert.ToInt32(ItemQuantitytextbox.Text);

                using (SqlConnection Connection = new SqlConnection())  // Creating an instance of SqlConnection under using statement.
                {
                    Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True"; // Sql Database directory.
                    Connection.Open();

                    Sql = "SELECT MAX(ItemID) FROM InvoiceItems;";  // Sql Command query.
                    int NewItemIDNumber;

                    using (SqlCommand SelectCommand = new SqlCommand(Sql, Connection)) // Creating an instance of Sql Command to be used under using statement.
                    {
                        NewItemIDNumber = (int)SelectCommand.ExecuteScalar() + 1;    // This finds the max ItemID number and adds it but 1. 
                    }

                    NewInvoice.ItemID = NewItemIDNumber;

                    Sql = "INSERT INTO InvoiceItems " +  // Sql statement to insert the data into the datbase.
                          "(ItemID, InvoiceID, ItemName, ItemDescription, ItemPrice, ItemQuantity)" +
                          "VALUES" +
                          $"({NewInvoice.ItemID}, " +
                          $"'{NewInvoice.InvoiceID}', " +
                          $"'{NewInvoice.ItemName}', " +
                          $"'{NewInvoice.ItemDescription}', " +
                          $"'{NewInvoice.ItemPrice}', " +
                          $"'{NewInvoice.ItemQuantity}');";

                    using (SqlCommand InsertCommand = new SqlCommand(Sql, Connection)) // This creates an instance of SqlCommand with parameters of Sql and Connection.
                    {
                        InsertCommand.ExecuteNonQuery(); // This statement executes the command into the Sql Database.
                    }

                    LoadInvoiceItems();  // This reloads the data and display it to the user.

                }
            }
        }

        private void DeleteInvoiceItemButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;

            if (CurrentItemRecord == null)  // If statement will execute if the current selected object is null.
            {
                ErrorMessageLabel.Content = "Cannot delete a blank record.";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return;
            }
            else // else statement will execute if the current item selected is not null.
            {
                string Sql;

                using (SqlConnection Connection = new SqlConnection()) // Creating an instance of Sql Connection to perform Sql database functions.
                {
                    Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True";  // Sql Database directory.
                    Connection.Open();

                    Sql = $"DELETE FROM InvoiceItems WHERE ItemID = {CurrentItemRecord.ItemID};"; //  Sql database syantax to be used to delete the record.

                    using (SqlCommand DeleteInvoice = new SqlCommand(Sql, Connection))  // This creates an instance of SqlCommand with parameters of Sql and Connection.
                    {

                        DeleteInvoice.ExecuteNonQuery();    // This executes the command into the database.

                        int IndexToRemove = CurrentItemIndex;

                        LoadInvoiceItems();  // This reloads the data and display it to the user.

                        // The if and else statement reselects the item in listbox if its a last record or not.
                        if (IndexToRemove == CustomerItems.Count)
                        {
                            CurrentItemIndex = CustomerItems.Count - 1;
                        }
                        else
                        {
                            CurrentItemIndex = IndexToRemove;
                        }

                        InvoiceItemsListbox.SelectedIndex = CurrentItemIndex; // Selects the item into the listbox.

                    }

                    Connection.Close();
                }

                DisplayCalculated(); // This Updates the record of calculated textbox(Subtotal,GST,PST and Total)
            }
        }

        private void SaveInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;

            if (CurrentInvoiceRecord == null)  // If statement will execute if the current selected object is null.
            {
                ErrorMessageLabel.Content = "Cannot save this record.";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return;
            }

            if (IsDataInvoiceValid() == false)  // This validates the record and it returns false, the if statement will execute.
            {
                return;
            }
            else  // The else statement will execute if the data validation returns false.
            {
                // This will save the record from the user to CurrentInvoiceRecord object.
                CurrentInvoiceRecord.InvoiceID = Convert.ToInt32(InvoiceIDTextbox.Text);
                CurrentInvoiceRecord.CustomerName = CustomerNameTextbox.Text;
                CurrentInvoiceRecord.CustomerAddress = CustomerAddressTextbox.Text;
                CurrentInvoiceRecord.CustomerEmail = CustomerEmailTextbox.Text;
                CurrentInvoiceRecord.Shipped = Convert.ToBoolean(ShippedCheckbox.IsChecked);

                try // Try and catch statement is used to force the user to enter the date in correct format.
                {
                    CurrentInvoiceRecord.InvoiceDate = Convert.ToDateTime(InvoiceDateTextbox.Text);
                }
                catch (FormatException)
                {
                    ErrorMessageLabel.Content = "Invoice date is incorrect";
                    ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                    ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                    return;
                }


                using (SqlConnection Connection = new SqlConnection()) // Creating an instance of Sql Connection to be used to perform Sql database function.
                {
                    Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True"; // Sql Database directory.
                    Connection.Open();

                    string Sql = "UPDATE Invoices SET " +   // This statement is used to update the records into the database.
                                                         $"InvoiceDate = '{CurrentInvoiceRecord.InvoiceDate}', " +
                                                         $"Shipped = '{CurrentInvoiceRecord.Shipped}'," +
                                                         $"CustomerName = '{CurrentInvoiceRecord.CustomerName}', " +
                                                         $"CustomerAddress = '{CurrentInvoiceRecord.CustomerAddress}'," +
                                                         $"CustomerEmail = '{CurrentInvoiceRecord.CustomerEmail}' " +
                                                         $"WHERE InvoiceID = {CurrentInvoiceRecord.InvoiceID};";

                    SqlCommand MyCommand = new SqlCommand(Sql, Connection);  // This creates an instance of SqlCommand with parameters of Sql and Connection.

                    MyCommand.ExecuteNonQuery(); // This executes the command into the Sql database.

                    Connection.Close();

                }

                LoadInvoices(); // Update and reload the data and display it to the user.

            }
        }

        private void NewInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;

            if (IsDataInvoiceValid() == false) // The if statement validates the data entered. This if statement will execute if it returns false.
            {
                return;
            }
            else // else statement will execute if the data validation returns true.
            {

                string Sql;
                Invoice NewInvoice = new Invoice(); // Creating an instance of Invoice class.

                // This Records the date from that particular textbox. 
                NewInvoice.InvoiceDate = Convert.ToDateTime(InvoiceDateTextbox.Text);
                NewInvoice.CustomerName = CustomerNameTextbox.Text;
                NewInvoice.CustomerAddress = CustomerAddressTextbox.Text;
                NewInvoice.CustomerEmail = CustomerEmailTextbox.Text;
                NewInvoice.Shipped = Convert.ToBoolean(ShippedCheckbox.IsChecked);

                using (SqlConnection Connection = new SqlConnection())  // Creating an instance of Sql Connection to be used to perform Sql database functions under using statement.
                {
                    Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True";  // Sql database directory.
                    Connection.Open();

                    Sql = "SELECT MAX(InvoiceID) FROM Invoices;"; // Sql Query statement.
                    int NewInvoiceIDNumber;

                    using (SqlCommand SelectCommand = new SqlCommand(Sql, Connection)) // This creates an instance of SqlCommand with parameters of Sql and Connection.
                    {
                        NewInvoiceIDNumber = (int)SelectCommand.ExecuteScalar() + 1;  // This returns the number of max invoiceID and adds it by 1.
                    }

                    NewInvoice.InvoiceID = NewInvoiceIDNumber;

                    Sql = "INSERT INTO Invoices " +  // Sql statement to be used to insert new data into Sql database.
                          "(InvoiceID, InvoiceDate, Shipped, CustomerName, CustomerAddress, CustomerEmail)" +
                          "VALUES" +
                          $"({NewInvoice.InvoiceID}, " +
                          $"'{NewInvoice.InvoiceDate}', " +
                          $"'{NewInvoice.Shipped}', " +
                          $"'{NewInvoice.CustomerName}', " +
                          $"'{NewInvoice.CustomerAddress}', " +
                          $"'{NewInvoice.CustomerEmail}');";

                    using (SqlCommand InsertCommand = new SqlCommand(Sql, Connection)) // // This creates an instance of SqlCommand with parameters of Sql and Connection.
                    {
                        InsertCommand.ExecuteNonQuery();  // This executes the command into the database. 
                    }

                    LoadInvoices(); // This updates the invoice and displays it to the user.

                    // This reselects the item and scroll the listbox to view.
                    int NewRecordedIndex = InvoicesListbox.Items.IndexOf(CurrentInvoiceRecord);
                    InvoicesListbox.SelectedIndex = NewRecordedIndex;
                    InvoicesListbox.ScrollIntoView(NewInvoice);

                    Connection.Close();

                }
            }
        }

        private void DeleteInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            ErrorMessageLabel.Foreground = null;
            ErrorMessageLabel.Background = null;

            if (CurrentInvoiceRecord == null) // The if statement will execute if the current selected object is null.
            {
                ErrorMessageLabel.Content = "Cannot delete a blank record.";
                ErrorMessageLabel.Foreground = new SolidColorBrush(Colors.White);
                ErrorMessageLabel.Background = new SolidColorBrush(Colors.Black);
                return;
            }
            else // else statement will execute if the current selected object is not null.
            {
                string SqlInvoice;
                string SqlItem;

                using (SqlConnection Connection = new SqlConnection()) // Creating an instance of Sql Connection to be used for performing Sql database functions.
                {
                    Connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|ShoppingCart.mdf; Integrated Security = True";  // Sql database directory.
                    Connection.Open();

                    SqlInvoice = $"DELETE FROM Invoices WHERE InvoiceID = {CurrentInvoiceRecord.InvoiceID};"; // This Sql statement is used to delete an Invoice record.
                    SqlItem = $"DELETE FROM InvoiceItems WHERE InvoiceID = {CurrentInvoiceRecord.InvoiceID};"; // This Sql statement is used to delete  an Invoice item.

                    using (SqlCommand DeleteInvoice = new SqlCommand(SqlInvoice, Connection)) // This creates an instance of SqlCommand with parameters of SqlInvoice and Connection.
                    {

                        DeleteInvoice.ExecuteNonQuery(); // This statement executes the Sql database command.

                        using (SqlCommand DeleteItem = new SqlCommand(SqlItem, Connection))  // This creates an instance of SqlCommand with parameters of SqlItem and Connection.
                        {
                            DeleteItem.ExecuteNonQuery(); // This statement executes the Sql database command.
                        }

                        int IndexToRemove = CurrentInvoicesIndex;

                        LoadInvoices(); // This updates the record and display it to the user.

                        // The if and else statement is used to reselect the index into the textbox.
                        if (IndexToRemove == CustomerRecords.Count)
                        {
                            CurrentInvoicesIndex = CustomerRecords.Count - 1;
                        }
                        else
                        {
                            CurrentInvoicesIndex = IndexToRemove;
                        }

                        InvoicesListbox.SelectedIndex = CurrentInvoicesIndex;

                    }

                    Connection.Close();

                }

                DisplayCalculated(); // This updates the calculate fields(Subtotal,GST,PST and Total)
            }
        }
    }
}
