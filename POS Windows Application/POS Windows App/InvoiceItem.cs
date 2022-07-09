using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comp255_FinalProject_AkilMomin
{
    public class InvoiceItem
    {
        private int itemID;
        private int invoiceID;
        private string itemName;
        private string itemDescription;
        private decimal itemPrice;
        private int itemQuantity;
        private decimal eachItemTotal;


        public InvoiceItem()
        {
            //Default Constructor.
        }

        public InvoiceItem(int ItemID, int InvoiceID, string ItemName, string ItemDescription, decimal ItemPrice, int ItemQuantity)
        {
            //This constructor accepts the data inputs while creating an instance of the class.
            itemID = ItemID;
            invoiceID = InvoiceID;
            itemName = ItemName;
            itemDescription = ItemDescription;
            itemPrice = ItemPrice;
            itemQuantity = ItemQuantity;


        }
                        // Getters and Setter methods.
        public int ItemID
        {
            get => itemID;
            set
            {
                itemID = value;
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

        public string ItemDescription
        {
            get => itemDescription;
            set
            {
                itemDescription = value;
            }
        }

        public string ItemName
        {
            get => itemName;
            set
            {
                itemName = value;
            }
        }

        public decimal ItemPrice
        {
            get => itemPrice;
            set
            {
                itemPrice = value;
            }
        }

        public int ItemQuantity
        {
            get => itemQuantity;
            set
            {
                itemQuantity = value;
            }
        }

        public decimal EachItemTotal // This method calculates the price of each item by quantity.
        {
            get
            {
                return eachItemTotal;
            }
            set
            {
                eachItemTotal = itemPrice * itemQuantity;
            }

        }


        public override String ToString() // This method overides the ToString method.
        {
            return $"{itemID,-15}{itemName,-15}{itemDescription,-25}{itemPrice,-20:N2}{itemQuantity,-14}{eachItemTotal:N2}";
        }

        public override bool Equals(object obj) // This override method is used to reselect an object in listbox.
        {
            if (obj == null) return false;
            try
            {
                if (this.invoiceID == ((InvoiceItem)obj).InvoiceID)
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