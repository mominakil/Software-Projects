
import javax.swing.*;
import java.awt.event.*;
import java.awt.*;
import java.io.IOException;
import java.util.ArrayList;

/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
/**
 *
 * @author akilm
 */
public class GUIConsole extends JFrame implements ChatIF {

    final public static int DEFAULT_PORT = 5555;
    ChatClient client;

    //Creating buttons
    private JButton closeB = new JButton("Logoff");
    private JButton openB = new JButton("Login");
    private JButton sendB = new JButton("Send");
    private JButton quitB = new JButton("Quit");
    private JButton whoB = new JButton("User List");
    private JButton privateMessageB = new JButton("Private Message");
    private JButton ticTacToeB = new JButton("Tic Tac Toe");

    //Creating JComboBox, it is similar to dropdrown list
    private JComboBox whoCB = new JComboBox();

    //Creating Text field
    private JTextField portTxF = new JTextField("5555");
    private JTextField hostTxF = new JTextField("127.0.0.1");
    private JTextField messageTxF = new JTextField("");
    private JTextField userTxF = new JTextField("");

    //Creating Labels
    private JLabel portLB = new JLabel("Port: ", JLabel.RIGHT);
    private JLabel hostLB = new JLabel("Host: ", JLabel.RIGHT);
    private JLabel messageLB = new JLabel("Message: ", JLabel.RIGHT);
    private JLabel userLB = new JLabel("User Id: ", JLabel.RIGHT);

    //Creating TextArea to display chat messages.
    private JTextArea messageList = new JTextArea();

    //Constructor
    public GUIConsole(String host, int port, String userId) {
        //The super class JFrame will put the text in the title bar of the window
        super("Simple Chat GUI");

        //Set the dimensions of the window frame. 300 Wide 400 Height
        setSize(300, 400);

        setLayout(new BorderLayout(5, 5)); //5by5 window
        JPanel bottom = new JPanel();
        add("Center", messageList); //This area to display chat messages
        add("South", bottom); // This area will be in the south for controls.

        //Layout is set to 7 rows 2 columns 5px of padding Horizontal and vertical
        bottom.setLayout(new GridLayout(8, 2, 5, 5));
        //JLabel and Text Fields
        bottom.add(hostLB);
        bottom.add(hostTxF);
        bottom.add(portLB);
        bottom.add(portTxF);
        bottom.add(userLB);
        bottom.add(userTxF);
        bottom.add(messageLB);
        bottom.add(messageTxF);
        //Buttons
        bottom.add(whoB);
        bottom.add(whoCB);
        bottom.add(privateMessageB);
        bottom.add(sendB);
        bottom.add(openB);
        bottom.add(closeB);
        bottom.add(ticTacToeB);
        bottom.add(quitB);

        //Setting the Host Ip address, Port and userId received from constructor.
        portTxF.setText(port + ""); // "" converts its into string
        hostTxF.setText(host);
        userTxF.setText(userId);

        setVisible(true);//To make the window appear

        sendB.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //Message from message textbox
                send(messageTxF.getText());
            }
        });

        openB.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //Content from userId textbox Login
                login(hostTxF.getText(), portTxF.getText(), userTxF.getText());

            }
        });

        closeB.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //Content from userId textbox Login
                logoff();

            }
        });

        quitB.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //quits the chat application and end the connection 
                quit();

            }
        });

        whoB.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //Calling send method and passing #who command in it.
                send("#who");
            }
        });

        privateMessageB.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //Calling send method and passing #who command in it.
                send("#pm " + whoCB.getSelectedItem() + " " + messageTxF.getText());
            }
        });

        ticTacToeB.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //Calling ticTacToe method below and passing userId in it to play the game.
                ticTacToe(userId);
            }
        });

        try {
            client = new ChatClient(host, port, this);
        } catch (IOException exception) {
            System.out.println("Error: Can't setup connection!!!!"
                    + " Terminating client.");
            System.exit(1);
        }
    }

    @Override
    public void display(String message) {
        messageList.insert(message + "\n", 0);
    }

    @Override
    public void displayUserList(ArrayList<String> userList, String roomName) {
        //Clear the ComboBox before re-populating it again
        whoCB.removeAllItems();

        //Populate the ComboBox for userlist from who passed in the whoB click event
        for (String user : userList) {
            whoCB.addItem(user);
        }
    }

    //Send method is executed when user clicks send button or private message or when when user has clicked user list button
    public void send(String message) {
        client.handleMessageFromClientUI(message);
    }
    
    //This method will be executed when user clicks login button
    public void login(String host, String port, String userId) {
        client.handleMessageFromClientUI("#setHost " + host);
        client.handleMessageFromClientUI("#setPort " + port);
        client.handleMessageFromClientUI("#login " + userId);
    }

    //This method will be executed when user clicks logoff button 
    public void logoff() {
        client.handleMessageFromClientUI("#logoff");
    }
    
    //This method will execute when quit is clicked
    public void quit() {
        client.handleMessageFromClientUI("#quit");
    }
    
    //This method will execute when user wants to play TicTacToe and it sends invitation to another player selected in user list
    public void ticTacToe(String userId) {
        TicTacToe ttt = new TicTacToe();
        ttt.setPlayer1(userTxF.getText());
        if ((whoCB.getSelectedItem()) != null) {
            ttt.setPlayer2(whoCB.getSelectedItem() + ""); //user selected in combobox
            ttt.setActivePlayer(1);
            ttt.setGameState(1);
            
            //If you are inviting yourself to play. It will display error and return
            if(ttt.getPlayer1().equals(ttt.getPlayer2())){
                display("Error: Cannot invite yourself to play TicTacToe. Please try again");
                return;
            }
            
            //instance of TicTacToe console
            TicTacToeConsole tttc = new TicTacToeConsole(this,ttt.getPlayer1());
            Envelope env = new Envelope("#ttt", "", ttt);
            
            //Send it to server.
            try {
                client.sendToServer(env);
                client.ticTacToeConsole = tttc;
            } catch (IOException io) {
                display("Error: Cannont send the tictactoe object to server");
            }
        } else {
            display("Error: Please select a user in UserList and try again");
        }
    }
    
    //main method
    public static void main(String[] args) {

        String host = "";
        int port = 0;  //The port number
        String userId = "";

        try {
            host = args[0];
            port = Integer.parseInt(args[1]);
        } catch (ArrayIndexOutOfBoundsException e) {
            host = "localhost";
            port = DEFAULT_PORT;
        }

        try {
            userId = args[2];
        } catch (ArrayIndexOutOfBoundsException e) {
            userId = "guest";
        }

        GUIConsole console = new GUIConsole(host, port, userId);

    }

}
