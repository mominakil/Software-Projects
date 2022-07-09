
import java.io.*;
import java.util.ArrayList;

/**
 * This class overrides some of the methods defined in the abstract superclass
 * in order to give more functionality to the client.
 */
public class ChatClient extends AbstractClient {
    //Instance variables **********************************************

    /**
     * The interface type variable. It allows the implementation of the display
     * method in the client.
     */
    ChatIF clientUI;
    TicTacToeConsole ticTacToeConsole;

    //Constructors ****************************************************
    /**
     * Constructs an instance of the chat client.
     *
     * @param host The server to connect to.
     * @param port The port number to connect on.
     * @param clientUI The interface type variable.
     */
    public ChatClient(String host, int port, ChatIF clientUI) throws IOException {
        super(host, port); //Call the superclass constructor
        this.clientUI = clientUI;
        //openConnection();
    }

    //Instance methods ************************************************
    /**
     * This method handles all data that comes in from the server.
     *
     * @param msg The message from the server.
     */
    @Override
    public void handleMessageFromServer(Object msg) {
        if (msg instanceof Envelope) {
            Envelope env = (Envelope) msg;
            handleCommandFromServer(env);
        } else {
            clientUI.display(msg.toString());
        }
    }

    public void handleCommandFromServer(Envelope env) {
        if (env.getId().equals("who")) {
            ArrayList<String> userList = (ArrayList<String>) env.getContents();
            String roomName = env.getArg().toString();

            //After creating chatIF abstract class for displayUserList.
            clientUI.displayUserList(userList, roomName);

        }

        if (env.getId().equals("#ttt")) {
            processTicTacToe(env);
        }
        //This command only uses to display player name on the window.
        if (env.getId().equals("#tttSetup")) {
            TicTacToe ticTacToe = (TicTacToe) env.getContents();
            //Display the board to the user
            ticTacToeConsole = new TicTacToeConsole(clientUI, ticTacToe.getPlayer2());
            try {
                Envelope envl = new Envelope("#tttAccept", "", "");
                this.sendToServer(envl);
            } catch (IOException io) {
                clientUI.display("Error: Failed to send #tttAccept message to server");
            }
        }

    }

    public void processTicTacToe(Envelope env) {
        TicTacToe ticTacToe = (TicTacToe) env.getContents();

        //Received invitation to play
        if (ticTacToe.getGameState() == 1) {
            clientUI.display("You have been invited to play tic tac toe with " + ticTacToe.getPlayer1() + "\n #tttAccept to accept, #tttDecline to decline");
        }

        //decline
        if (ticTacToe.getGameState() == 2) {
            clientUI.display("Your game was declined");
            //System.exit(0); //Not sure if this will work.
            ticTacToeConsole.quit(); // Add an exception that throws if cannot exit
        }

        //playing
        if (ticTacToe.getGameState() == 3) {
            clientUI.display("Your turn to play TicTacToe");
            //TicTacToeConsole ticTacToeConsole = new TicTacToeConsole(clientUI);

            ticTacToeConsole.setGame(ticTacToe);
            ticTacToeConsole.isPlayed = true;
            ticTacToeConsole.updateBoard(ticTacToe.getBoard());

        }

        //won
        if (ticTacToe.getGameState() == 4) {
            clientUI.display("You have lost the game");
            //Hide the board
            ticTacToeConsole.quit();
        }

    }

    /**
     * This method handles all data coming from the UI
     *
     * @param message The message from the UI.
     */
    public void handleMessageFromClientUI(String message) {

        if (message.charAt(0) == '#') {

            handleClientCommand(message);

        } else {
            try {
                sendToServer(message);
            } catch (IOException e) {
                clientUI.display("Could not send message to server.  Terminating client.......");
                quit();
            }
        }
    }

    /**
     * This method terminates the client.
     */
    public void quit() {
        try {
            closeConnection();
        } catch (IOException e) {
        }
        System.exit(0);
    }

    @Override
    public void connectionClosed() {

        System.out.println("Connection closed");

    }

    @Override
    protected void connectionException(Exception exception) {

        System.out.println("Server has shut down");

    }

    public void handleClientCommand(String message) {

        if (message.equals("#quit")) {
            clientUI.display("Shutting Down Client");
            quit();

        }

        if (message.equals("#logoff")) {
            clientUI.display("Disconnecting from server");
            try {
                closeConnection();
            } catch (IOException e) {
            };

        }

        if (message.indexOf("#setHost") >= 0) {

            if (isConnected()) {
                clientUI.display("Cannot change host while connected");
            } else {
                setHost(message.substring(8, message.length()).trim());
            }

        }

        if (message.indexOf("#setPort") >= 0) {
            //#setPort 5556
            if (isConnected()) {
                clientUI.display("Cannot change port while connected");
            } else {
                setPort(Integer.parseInt(message.substring(8, message.length()).trim()));
            }
        }

        if (message.indexOf("#login") == 0) {
            //eg. #login akil
            if (isConnected()) {
                clientUI.display("already connected");
            } else {

                try {
                    //Extracting userName = akil from #login akil
                    String userName = message.substring(6, message.length()).trim();
                    openConnection();
                    clientUI.display("Logging in as " + userName);
                    Envelope env = new Envelope("login", "", userName);
                    this.sendToServer(env);

                } catch (IOException e) {
                    clientUI.display("failed to connect to server.");
                }
            }
        }

        if (message.indexOf("#join") == 0) {
            //#join roomName
            //create an envelope

            try {
                String roomName = message.substring(5, message.length()).trim();
                Envelope env = new Envelope("join", "", roomName);
                this.sendToServer(env);
            } catch (IOException e) {
                clientUI.display("failed to join a room.");
            }
        }

        if (message.indexOf("#pm") == 0) {
            //#pm userid privatemessage

            try {
                //#pm Bob Hi Bob

                //Extracting targetUserIdAndMessage = Bob Hi Bob
                String targetUserIdAndMessage = message.substring(3, message.length()).trim();

                //Extracting targetUserId = Bob
                String targetUserId = targetUserIdAndMessage.substring(0, targetUserIdAndMessage.indexOf(" ")).trim();

                //Extracting privateMessage = Hi Bob 
                String privateMessage = targetUserIdAndMessage.substring(targetUserIdAndMessage.indexOf(" "), targetUserIdAndMessage.length()).trim();

                Envelope env = new Envelope("pm", targetUserId, privateMessage);
                this.sendToServer(env);
            } catch (IOException e) {
                clientUI.display("Failed to send private message");
            }
        }

        if (message.indexOf("#yell") == 0) {
            //#yell message
            try {
                //#yell hurry up everyone

                // Extracting yellMessage = hurry up everyone
                String yellMessage = message.substring(5, message.length()).trim();

                Envelope env = new Envelope("yell", "", yellMessage);
                this.sendToServer(env);
            } catch (IOException e) {
                clientUI.display("Failed to yell");
            }
        }

        if (message.equals("#who")) {
            //who command will display the list of users in that room to the requested user
            try {
                Envelope env = new Envelope("who", "", "");
                this.sendToServer(env);
            } catch (IOException e) {
                clientUI.display("Failed to aquire user list");
            }
        }
        
        //When user the declines the TicTacToe request
        if (message.equals("#tttDecline")) {

            try {
                Envelope env = new Envelope("#tttDecline", "", "");
                this.sendToServer(env);
            } catch (IOException io) {
                clientUI.display("Error: Failed to send #tttDecline message to server");
            }
        }
        
        //When user the accepts the TicTacToe request
        if (message.equals("#tttAccept")) {
            try {
                Envelope env = new Envelope("#tttSetup", "", "");
                this.sendToServer(env);
            } catch (IOException io) {
                clientUI.display("Error: Failed to send #tttAccept message to server");
            }
        }

    }
}
//End of ChatClient class
