
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;

/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
/**
 *
 * @author akilm
 */
public class ServerConsole implements ChatIF {

    //Default port to connect to
    final public static int DEFAULT_PORT = 5555;

    //Instance of EchoServer
    EchoServer echoServer;

    //Constructor 
    public ServerConsole(int port) {
        try {
            echoServer = new EchoServer(port, this);
        } catch (Exception ex) {
            System.out.println("Error: Can't setup server!!!! Terminating Connection!!");
            System.exit(1);
        }
    }
   
    
    public static void main(String[] args) {
        
        int port = 0; //The port number
        
        //The port from args[1] or from DEFAULT_PORT
        try {
            port = Integer.parseInt(args[1]);
        } catch (ArrayIndexOutOfBoundsException e) {
            port = DEFAULT_PORT;
        }

        ServerConsole console = new ServerConsole(port);
        console.accept(); //Waits for console data from server UI
    }

    //accepts the command or message from the serverUI
    public void accept() {
        try {
            BufferedReader fromConsole
                    = new BufferedReader(new InputStreamReader(System.in));
            String message;

            while (true) {
                message = fromConsole.readLine();
                echoServer.handleMessageFromServerUI(message);
            }
        } catch (Exception ex) {
            System.out.println("Unexpected error while reading from console!");
        }
    }

    @Override
    public void display(String message) {
        System.out.println("> " + message);
    }

    @Override
    public void displayUserList(ArrayList<String> userList, String roomName) {
        //foreach loop: for each string s in userList, display s
        this.display("Users in " + roomName + ":");
        for (String s : userList) {
            this.display(s);
        }
    }

}
