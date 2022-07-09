/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Test Account
 * @author Akil Akbar Momin.
 * Date: 6th December,2020.
 */
public class TestAccount {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) { 
        
        System.out.println("Number of Accounts: "+ Account.getNumberofAccounts()); // Printing the number of accounts.
        
        Account MyAccount1 = new Account(200.5,10); // calling the constructor MyAccount1 to initialize.
        MyAccount1.deposit(450); // Depositing the amount.
        MyAccount1.withdraw(25.22); // Withdrawing the amount.
        
        System.out.println("Current Balance of Account1 : "+ MyAccount1.getBalance()); // Printing the statement for current balance.
        System.out.println("My Id Number is: "+ MyAccount1.getId()+"\n\n"); // printing the statement for Id Number.
        
        
        
        Account MyAccount2 = new Account(2.25,10); // Calling the constructor for  MyAccount2 to initialize.
        MyAccount2.deposit(45.22); // Making a deposite.
        MyAccount2.withdraw(26.33); // Making a withdawal.
        MyAccount1.transfer(125.666666, MyAccount2); // Transferring the amount from one account to another.
        System.out.println("Current Balance of Account2 : " + MyAccount2.getBalance()); //Printing out current balance.
        System.out.println("My Id Number is: " + MyAccount2.getId());     // Printing out Id Number.
        
        System.out.println("Number of Accounts: " + Account.getNumberofAccounts()); // Printing the Number of account created.
    } // End of main Class.
    
}// End of public Class.
