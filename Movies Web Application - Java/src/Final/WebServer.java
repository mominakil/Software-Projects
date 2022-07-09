/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Final;

import java.io.*; // Importing all the package for Input and Output communication. 
import java.net.*; // This import allows us to do networking tasks, and used for ServerSocket and Socket in our program and  Eg. Read and write data using input and output stream and Socket Server. 
import java.util.*; // This imports all libraries for util.
import java.util.concurrent.*; // This import is used to run Executor service for threads.
import javax.swing.*; // This import is used to run JFrame - JTextArea, JButton, JPanel etc.
import java.awt.*; // This import is used for Border Layout to assign JFrame features into the window layout.
import java.awt.event.*; //This import is used for Action Listener in our program to set Start and Stop Server.
import static java.lang.Integer.parseInt; // This import is used for Converting data to int, In our program its for converting HTTP Port input to int from string.

/**
 * WebServer
 * Date: April 22, 2021
 * @author Akil momin
 */
public class WebServer extends JFrame {

    private static ServerSocket requestListener; // This variable is used to Create Server and accepting connection request.
    public static int HTTP_PORT; //This Variable is used to record HTTP PORT and used to accept connections under this port.
    private ExecutorService responses; // This will be used for Executing a runnable class. Responder class in out Program.

    private JTextArea input; // This is used for text area in the layout to receive the page requests.
    private JButton startServer; // This button stats accepting the connection and its requests. 
    private JButton stopServer; // This button stops the server from accepting the requests.
    private JPanel buttonInputContainer; // This Jpanel is used to store both buttons and stores in as a container.
    private boolean startOrStopServer = true; // This will be used in Event listener.

    public WebServer() {

        //set title bar
        super("GUI Console");

        //set dimensions.
        setSize(500, 500);

        //determines layout
        BorderLayout Layout = new BorderLayout();
        setLayout(Layout);

        //add some controls
        input = new JTextArea();
        startServer = new JButton("Start Server");
        stopServer = new JButton("Stop Server");
        

        //Create Button container for Start and Stop
        buttonInputContainer = new JPanel();
        buttonInputContainer.add(startServer);
        buttonInputContainer.add(stopServer);

        //BorderLayout.CENTRE
        add(input, BorderLayout.CENTER);

        //BorderLayout.SOUTH
        add(buttonInputContainer, BorderLayout.SOUTH);

        //Start Button action listener.
        startServer.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent ae) {
                startOrStopServer = true; // Turns the boolean to true in case of startServer button press.
            }
        }
        );

        //Stop Button action listener.
        stopServer.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent ae) {
                startOrStopServer = false; // Turns the boolean to false in case of stopServer button press. 
            }
        }
        );

        setVisible(true); //This makes the window layout visible.

        try {
            HTTP_PORT = parseInt(JOptionPane.showInputDialog("Enter Server Port number")); // Pops up a JOptionPane.
            requestListener = new ServerSocket(HTTP_PORT); // Similar to serverSocket in lab09 to enter Port.
            System.out.println("Waiting For IE to request a page:"); 
            responses = Executors.newFixedThreadPool(100); //Creates a Executor serveice to run 100 threads.
            start(); // This calls the start() function.
        } catch (Exception e) {
            e.printStackTrace(); // Prints an error message in case of Exception.
        }

    }

    public void start() { //This method runs the server 

        try {
            while (true) {
                Thread.sleep(500); // lets it sleep for 0.5 seconds for it to update the boolean in the while loop.
                while (startOrStopServer == true) {
                    Responder r = new Responder(requestListener.accept(), input); // Creating an instance of Responder and passing ServerScoket Variable to for accepting connection.
                    responses.execute(r); // Executes thread of instance variable of class Responder which implements runnable.
                }
            }

        } catch (Exception e) {
            e.printStackTrace(); //Prints an error message in case of Exception.
        }
    }

    public static void main(String[] args) {
        WebServer WS = new WebServer(); // Creating an instance and running it.
    }
}
