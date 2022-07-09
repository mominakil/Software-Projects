using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comp255_FinalProject_AkilMomin
{
    public class Invoice
    {
        private int invoiceID;
        private DateTime invoiceDate;
        private bool shipped;
        private string customerName;
        private string customerAddress;
        private string customerEmail;
        private string isShipped;


        public Invoice()
        {
            //Default Constructor.
        }

        public Invoice(int InvoiceID, DateTime InvoiceDate, bool Shipped, string CustomerName, string CustomerAddress, string CustomerEmail)
        {
            //This constructor accepts the data inputs while creating an instance of the class.
            invoiceID = InvoiceID;
            invoiceDate = InvoiceDate;
            shipped = Shipped;
            customerName = CustomerName;
            customerAddress = CustomerAddress;
            customerEmail = CustomerEmail;

        }
                        // Getter and Setter methods
        public string CustomerAddress
        {
            get => customerAddress;
            set
            {
                customerAddress = value;
            }
        }

        public string CustomerEmail
        {
            get => customerEmail;
            set
            {
                customerEmail = value;
            }
        }

        public string CustomerName
        {
            get => customerName;
            set
            {
                customerName = value;
            }
        }

        public System.DateTime InvoiceDate
        {
            get => invoiceDate;
            set
            {
                invoiceDate = value;
            }
        }

        public int InvoiceID
        {
            get => invoiceID;
            set
            {
                invoiceID = value;
            }
        }

        public bool Shipped
        {
            get => shipped;
            set
            {
                shipped = value;
            }
        }


        public void SetShipped(bool shipping)  // This method sets the shipping to Yes or No with input parameter of bool shipping.
        {
            if (shipped == true)
            {
                isShipped = "Yes";
            }
            else
            {
                isShipped = "No";
            }
        }


        public override String ToString() // This method overides the ToString method.
        {
            return $"{invoiceID,-20}{customerName,-20}{customerEmail,-30}{isShipped}";
        }

        public override bool Equals(object obj) // This override method is used to reselect an object in listbox.
        {
            if (obj == null) return false;

            try
            {
                if (this.invoiceID == ((Invoice)obj).InvoiceID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (InvalidCastException)
            {
                return false;
            }


        }
        public override int GetHashCode() // This override method returns the matching invoiceID when reselection an object in listbox. 
        {
            return this.invoiceID;
        }
    }
}