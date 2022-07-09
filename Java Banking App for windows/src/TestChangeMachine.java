/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Test ChangeMachine
 * @author Akil Akbar Momin 
 * 6th December,2020.
 */
import java.util.Scanner; // importing scanner to be used for keyboard input.

public class TestChangeMachine {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        int Bill;                         // Declaring varibles.
        int Counter = 1;                  // Declaring varibles.
        boolean Status = true;            // Declaring varibles.
        int Loonies;                      // Declaring varibles.
        int Toonies;                      // Declaring varibles.
        int InputPin;                     // Declaring varibles.
        int Pin = 333;                    // Declaring varibles.
        Scanner input = new Scanner(System.in); // Declaring scanner to be used for keybaoed input.

        ChangeMachine MyChange1 = new ChangeMachine(11, 17); // Using constructor to initialize.

        if (MyChange1.getStatus() == false) { // if the status of the manchine is false,  if statement execultes.
            System.out.println("Out Of Order! Cannot accept ANY bills. Please call technician to repair the machine");
        } else { // If the status of the machine is true, the else statement executes.
            while (Status == true) { // While statement will loop unit the status is false.
                if (MyChange1.getStatus() == false) { // if statement will execute as long as the statement is false.
                    System.out.println("Out Of Order! Cannot accept any more bills. Please call technician to refill the machine");
                    System.out.print("If you are a technician please enter the PIN number: ");
                    InputPin = input.nextInt();
                    if (InputPin == Pin) {  // If the prompted pin is equal to the actual pin, this statement will execute.
                        System.out.print("Please add $1 Coins to the machine: ");
                        Loonies = input.nextInt();
                        System.out.print("Please add $2 Coins to the machine: ");
                        Toonies = input.nextInt();
                        MyChange1.setLoonies(Loonies);
                        MyChange1.setToonies(Toonies);

                    } else if (InputPin != Pin) { // if the prompted pin is wrong, this statement will execute and this will end the program.
                        System.out.println("Invalid Pin");
                        Status = false;
                        return;
                    }

                } else {
                    System.out.print("Please insert a single $5 or $10 or $20 bill for No. " + Counter + " transaction: "); // This statement will execute if the status of the machine is true.
                    Bill = input.nextInt();
                    MyChange1.acceptMoney(Bill);

                }

                Counter++; // increments the counter by 1 on each loop.
            }
        } 

    } // end of main class.

} // end of public class.
