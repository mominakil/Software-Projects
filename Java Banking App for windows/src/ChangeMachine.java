/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * ChangeMachine Class
 * @author Akil Akbar Momin
 * 6th December,2020
 */
public class ChangeMachine {

    private int loonies;         // Declaring private variable.
    private int toonies;         // Declaring private variable.
    private boolean status;      // Declaring private variable.
    
    

    public ChangeMachine(int loonies, int toonies) { // This constructor accepts loonies and toonies and check status.
        setLoonies(loonies);
        setToonies(toonies);

        checkStatus();

    }

    public int getLoonies() { // This getter method returns loonies.
        return loonies;
    }

    public int getToonies() { // This getter method returns toonies.
        return toonies;
    }

    public boolean getStatus() { //  This getter method returns status of the machine.
        return status;
    }

    public void setLoonies(int loonies) { // This setter method uses if statement to validates the amount passed in through method and checks the status.
        if (loonies < 0) {
            System.out.println("You cannot have negative number of coins!");
        }
        checkStatus(); // Checks if the machine has enough coins to continue.

        if (loonies > 0) {
            this.loonies += loonies;
        }

        checkStatus();

    }

    public void setToonies(int toonies) { // This setter method uses if statement to validates the amount passed in through method and checks the status.
        if (toonies < 0) {
            System.out.println("You cannot have negative number of coins!");

        }
        checkStatus(); // Checks if the machine has enough coins to continue.

        if (toonies > 0) {
            this.toonies += toonies;
        }

        checkStatus();

    }

    private void setStatus(boolean status) {  // This setter method sets the status of the machine.
        this.status = status;
    }

    public void acceptMoney(int amount) { // This method accepts the money after validation under if and else statement and checks the status of machine.
        checkStatus();

        if (status == false) {
            System.out.println("The machine is out of order! Here is your bill back");
        } 
        else if (amount == 5 || amount == 10 || amount == 20) {
           
            makeChange(amount);
        }
        else{
            System.out.println("Invalid! You must insert 5$ or 10$ or 20$ bill. Try again.");
        }

        checkStatus();

    }

    public void checkStatus() { // This method sets the status of the machine as boolean after validate under if and else statement.
        
        if (this.loonies < 1 || this.toonies < 17) {
            setStatus(false);
        } 
        else {
            setStatus(true);
        }
    }

    private void makeChange(int amount) { // This method calculates the change and deducts the amount from loonies and toonies and display the result.
        int LoonieChange;
        int ToonieChange;

        LoonieChange = amount % 2;
        ToonieChange = amount / 2;

        this.loonies = this.loonies - LoonieChange;
        this.toonies = this.toonies - ToonieChange;

       System.out.println("The number of loonies you will get is: " + LoonieChange + "\nThe number of tooneies you will get is: " + ToonieChange);

    }

}
