/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Final;

import java.io.*; //Importing all the package for Java.Io and we will use it for Input and Output communication.
import java.net.*; // This import allows us to do networking tasks, and used for ServerSocket and Socket in our program.
import java.text.*; // This import is used to format time in our program.
import java.util.Calendar; // This import is used to get current time in our program.
import java.util.Scanner; //This import is used to record incoming requests from webbrowser and to read files.
import javax.swing.*; // Used for Creating GUI panel


/**
 * Responder
 * Date: April 22, 2021
 * @author Akil Momin
 */
public class Responder extends JFrame implements Runnable {

    // Declaring Variables.
    private Socket requestHandler; // Used for accepting connection passed in by the caller of the class.
    private Scanner requestReader; // Used to store requests from client - web browser.
    private Scanner pageReader; // Used to read txt files.
    private DataOutputStream pageWriter; // Used to write data to client - web browser.
    private String HTTPMessage; // It is used to record message received from the webbrowser
    private String requestedFile; // It is used from HTTPMessage and filter out requied information.
    private String WriteRecord; // This string contains information of page requests, time, and error message if any.
    private FileWriter fw; // This is used to write information to txt file.
    private String FileNotFoundError = ""; // Stores the file not found exception as string to be used for saving in FileWriter file.
    private JTextArea input; // This is used to write web requests to JTextArea of the program.


    public Responder(Socket requestHandler, JTextArea input) { //Constructor
        this.requestHandler = requestHandler;
        this.input = input;
    }

    public void run() {

        try {

            System.out.println("Page Requested: Requested Header: ");

            //Receives a request from client Web.
            requestReader = new Scanner(new InputStreamReader(requestHandler.getInputStream()));
            
            //do while loop is used to filter out HTTP Message received from web browser.
            int lineCount = 0;
            do {
                lineCount++; //This will be used later and this notes down the lines in the HTTP Message.
                HTTPMessage = requestReader.nextLine(); // Gets the received message for requestReader and stores it.
                if (lineCount == 1) {
                    requestedFile = "WebRoot\\" + HTTPMessage.substring(5, HTTPMessage.indexOf("HTTP/1.1") - 1);
                    //doService = HTTPMessage.substring(5, 15);
                }
                System.out.println(HTTPMessage); //Prints out the received message
                input.append(HTTPMessage + "\n"); // Prints it to JTextArea of Window Layout of Server.
            } while (HTTPMessage.length() != 0); // This reades through the last line if the last line is empty then it exits.

            requestedFile = requestedFile.trim(); // Trims the extra leading space.
                
            // This try and catch block will read the requested file as per request type from web browser. This will not process doService requests.
            try {
                if (requestedFile.equals("WebRoot\\subdir") || requestedFile.equals("WebRoot\\subdir/")) {
                    requestedFile = "WebRoot\\Default.htm";
                    pageReader = new Scanner(new File(requestedFile)); // Reads the file from Poject directory and stores it in the scanner pageReader
                } else if (requestedFile.equals("WebRoot\\")) {
                    requestedFile = "WebRoot\\Default.htm";
                    pageReader = new Scanner(new File(requestedFile)); // Reads the file from Poject directory and stores it in the scanner pageReader
                } else if (requestedFile.indexOf("doSERVICE") == -1) {
                    pageReader = new Scanner(new File(requestedFile)); // Reads the file from Poject directory and stores it in the scanner pageReader
                }

            } catch (FileNotFoundException fnfe) { // If file is not found then it will display custom Error html page.
                requestedFile = "WebRoot\\Util\\Error404.htm";
                pageReader = new Scanner(new File(requestedFile)); // Reads the file in case fnfe.
                FileNotFoundError = fnfe.toString(); // This is later used in file writer to record errors in txt file.
            }
            
            // This if statement will process only doService requests and will use SQLSelectService to perform SQL SELECT Query based on request. 
            if (requestedFile.indexOf("doSERVICE") > -1) {
                pageWriter = new DataOutputStream(requestHandler.getOutputStream());
                pageWriter.flush();

                SQLSelectService s = new SQLSelectService(pageWriter, requestedFile); // Creating an instance of SQLSelectService class.
                s.doWork(); //This instance method of SQLSelectService will process the SQL request from query.htm and display the result to user.

            } else { // This else statement will execute if the request is other than doSERVICE.
                requestedFile = requestedFile.trim(); // trim leading space if any.
                pageWriter = new DataOutputStream(requestHandler.getOutputStream()); // Creates an IO path for output.
                pageWriter.flush(); // Clears out the DataOutputStream if any.
                while (pageReader.hasNext()) { // It will loop unit it contains next line.
                    String s = pageReader.nextLine(); // Reads line by line.
                    pageWriter.writeBytes(s); // Sends the data to the client line by line.
                }

                //Tells the browser that we are done sending.
                pageReader.close();
                pageWriter.close();
                requestHandler.close();

            }
            
            // Converts current time in specified format.
            String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(Calendar.getInstance().getTime());
            fw = new FileWriter(new File("WebRequests.txt"), true); // Creating an instance of FileWriter and selecting the txt file.
            // This String contains the name of the file requested, FileNotFoundError if any and time stamp. 
            WriteRecord = "\nVisited Page: " + requestedFile.substring(8) + "\n" + FileNotFoundError + "\nVisited time: " + timeStamp + "\n\n";
            fw.write(WriteRecord); //Writes the String to the txt file.
            fw.write("---------------------------------------------------------------------------------------------------------");
            fw.close(); // This saves the txt file.

        } catch (Exception e) {
            System.out.println(e.toString()); // Prints the Exception e in case of error.
            System.out.print("\n");
            e.printStackTrace(); // Prints the Exception e in PrintStackTrace in case of error.
        }

    }

}
