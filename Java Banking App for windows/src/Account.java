/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Account Class
 * @author Akil Momin
 * Date: 6th December, 2020.
 */
public class Account {

    static private int numberOfAccounts;  // Declaring static private variable
    private int id;                       // Declaring private variables 
    private double balance;               // Declaring private variables 
    private double annualInterestRate;    // Declaring private variables 
    private int accountNumber;            // Declaring private variables 
    private double interestRate;          // Declaring private variables 
    
    
    public Account() {  //This is a Default Constructor 
        balance = 0;
        annualInterestRate = 0;
        id = numberOfAccounts; 
        accountNumber = 0;
        interestRate = 0;
        
        numberOfAccounts++;
    }

    public Account(double balance, double annualInterestRate) { //This is a Customized constructor
        if (balance >= 0) { // Validating if balance is less 0 or not, if not then proceed inside if statement.
            this.balance = balance;
            this.annualInterestRate = annualInterestRate;
            id = numberOfAccounts; 
            accountNumber=0;
            interestRate =0;
            numberOfAccounts++;
        }

        
    }

    public int getId() {  // Getter method for getId
        return id;
    }

    public double getBalance() { // Getter method for getBalance.
        return balance;
    }

    public double getAnnualInterestRate() { // Getter method for getAnnualInterestRate.
        return annualInterestRate;
    }

    public void setId(int id) { // Setter method for setId.
        this.id = numberOfAccounts;
    }

    private void setBalance(double balance) { // Setter method for setBalance.
        if (balance >= 0) {  // Validating if balance is less than 0 or not, if not then proceed inside if statement.
            this.balance = balance;
        }

    }

    public void setAnnualInterestrate(double annualInterestRate) { // Setter method for setAnnualInterestrate.
        this.annualInterestRate = annualInterestRate;
    }

    public void getMonthlyInterestRate() {  // Getter method for monthly interest rate.
        double Monthly;
        Monthly = annualInterestRate/12;
        
    }

    public double withdraw(double amount) { // Withdraw method - deducts amount passed in with balance if it satisfies if condition.
        if (amount<=balance && amount > 0){
        balance = balance - amount;  
        
        }
        
        return balance;
    }

    public double deposit(double amount) { // Deposite method - Adds amount to the balance if the amount passed in satisfy th if condition.
        if (amount > 0){
            balance = balance + amount; 
        }
        return balance;
    }

    public int getNumberOfAccounts() { // Getter method for Numberof Accounts
        return numberOfAccounts;
    }

    public void transfer(double amount, Account otherAccount) { // This method transfers amount from one account to another.
        otherAccount.balance += amount;
        
    }

    public int getAccountNumber() { // This getter method returns the accountNumber.
        return accountNumber;
    }

    public double getInterestrate() { // This getter method returns the interestrate.
        return interestRate;
    }

    public void setAccountNumber(int accountNumber) { // This setter method sets the AccountNumber.
        this.accountNumber = accountNumber;
    }

    public void setInterestRate(double interestRate) { // This method sets the interestrate.
        this.interestRate = interestRate;
    }
   
    public static int getNumberofAccounts(){ // This getter method returns the NumberOfAccounts.
        return numberOfAccounts;
    }
}
