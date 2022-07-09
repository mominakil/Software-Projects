
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;

public class EchoServer extends AbstractServer {
    //Class variables *************************************************

    /**
     * The default port to listen on.
     */
    final public static int DEFAULT_PORT = 5555;

    ChatIF serverUI;
    ConnectionToClient Gclient;

    //Constructors ****************************************************
    /**
     * Constructs an instance of the echo server.
     *
     * @param port The port number to connect on.
     */
    public EchoServer(int port, ChatIF serverUI) {

        super(port);
        this.serverUI = serverUI;

    }

    public EchoServer(int port) {
        super(port);
        try {
            this.listen(); //Start listening for connections
        } catch (Exception ex) {
            System.out.println("ERROR - Could not listen for clients!");
        }
    }

    //Instance methods ************************************************
    /**
     * This method handles any messages received from the client.
     *
     * @param msg The message received from the client.
     * @param client The connection from which the message originated.
     */
    @Override
    public void handleMessageFromClient(Object msg, ConnectionToClient client) {

        if (msg instanceof Envelope) {
            Envelope env = (Envelope) msg;
            handleCommandFromClient(env, client);

        } else {
            System.out.println("Message received: " + msg + " from " + client);
            this.sendToAllClientsInRoom(msg, client);
        }
    }

    public void handleCommandFromClient(Envelope env, ConnectionToClient client) {
        if (env.getId().equals("login")) {
            String userId = env.getContents().toString();
            client.setInfo("userid", userId);
            client.setInfo("room", "lobby");
        }
        if (env.getId().equals("join")) {
            String roomName = env.getContents().toString();
            client.setInfo("room", roomName);
        }
        if (env.getId().equals("pm")) {
            String targetUserId = env.getArg();
            String privateMessage = env.getContents().toString();
            sendToAClient(privateMessage, targetUserId, client);
        }
        if (env.getId().equals("yell")) {
            String yellMessage = env.getContents().toString();
            String userId = client.getInfo("userid").toString();
            this.sendToAllClients(userId + " yells: " + yellMessage);
        }
        if (env.getId().equals("who")) {
            this.sendRoomListToClient(client);
        }
        if (env.getId().equals("#ttt")) {
            TicTacToe ticTacToe = (TicTacToe) env.getContents();
            processTicTacToe(ticTacToe, client);
        }
        if (env.getId().equals("#tttDecline")) {
            TicTacToe ticTacToe = (TicTacToe) Gclient.getInfo("ttt");
            ticTacToe.setGameState(2);
            Envelope envl = new Envelope("#ttt", "", ticTacToe);
            sendToPlayer1(envl, ticTacToe, client);
        }
        if (env.getId().equals("#tttAccept")) {
            TicTacToe ticTacToe = (TicTacToe) Gclient.getInfo("ttt");
            ticTacToe.setGameState(3);
            Envelope envl = new Envelope("#ttt", "", ticTacToe);
            sendToPlayer1(envl, ticTacToe, client);
        }
        //This command in envelope is used to display player name on the window.
        if (env.getId().equals("#tttSetup")) {
            TicTacToe ticTacToe = (TicTacToe) Gclient.getInfo("ttt");
            Envelope envl = new Envelope("#tttSetup", "", ticTacToe);
            sendToPlayer2(envl, ticTacToe, client);
        }
    }

    public void processTicTacToe(TicTacToe ticTacToe, ConnectionToClient client) {
        //TicTacToe ticTacToe = (TicTacToe) env.getContents();

        //invite
        if (ticTacToe.getGameState() == 1) {
            client.setInfo("ttt", ticTacToe);
            Gclient = client;
            Envelope env = new Envelope("#ttt", "", ticTacToe);
            sendToPlayer2(env, ticTacToe, client);
        }

        //decline
        if (ticTacToe.getGameState() == 2) {
            Envelope env = new Envelope("#ttt", "", ticTacToe);
            sendToPlayer1(env, ticTacToe, client);
        }

        //playing
        if (ticTacToe.getGameState() == 3) {
            //Save the instance state.
            client.setInfo("ttt", ticTacToe);
            Gclient = client;
            //Swap activePlayer
            if (ticTacToe.getActivePlayer() == 1) {
                ticTacToe.setActivePlayer(2);
            } else if (ticTacToe.getActivePlayer() == 2) {
                ticTacToe.setActivePlayer(1);
            }

            Envelope env = new Envelope("#ttt", "", ticTacToe);

            //Send envelope to activePlayer
            if (ticTacToe.getActivePlayer() == 1) {
                sendToPlayer1(env, ticTacToe, client);
            } else if (ticTacToe.getActivePlayer() == 2) {
                sendToPlayer2(env, ticTacToe, client);
            }
        }

        //won
        if (ticTacToe.getGameState() == 4) {

            //Swap activePlayer
            if (ticTacToe.getActivePlayer() == 1) {
                ticTacToe.setActivePlayer(2);
            } else if (ticTacToe.getActivePlayer() == 2) {
                ticTacToe.setActivePlayer(1);
            }

            Envelope env = new Envelope("#ttt", "", ticTacToe);

            //Send envelope to activePlayer
            if (ticTacToe.getActivePlayer() == 1) {
                sendToPlayer1(env, ticTacToe, client);
            } else if (ticTacToe.getActivePlayer() == 2) {
                sendToPlayer2(env, ticTacToe, client);
            }
        }

    }
    
    //Sends the envelope to player1
    public void sendToPlayer1(Envelope env, TicTacToe ticTacToe, ConnectionToClient client) {

        Thread[] clientThreadList = getClientConnections();
        for (int i = 0; i < clientThreadList.length; i++) {

            ConnectionToClient target = (ConnectionToClient) clientThreadList[i];

            if (target.getInfo("userid").equals(ticTacToe.getPlayer1())) {
                try {
                    target.sendToClient(env);
                } catch (IOException ie) {
                    System.out.println("Error: Failed to send it to client");
                }
            }
        }

    }
    
    
    //Sends the envelope to player2
    public void sendToPlayer2(Envelope env, TicTacToe ticTacToe, ConnectionToClient client) {
        Thread[] clientThreadList = getClientConnections();
        for (int i = 0; i < clientThreadList.length; i++) {
            ConnectionToClient target = (ConnectionToClient) clientThreadList[i];
            if (target.getInfo("userid").equals(ticTacToe.getPlayer2())) {
                try {
                    target.sendToClient(env);
                } catch (IOException ie) {
                    System.out.println("Error: Failed to send it to client");
                }
            }
        }
    }
    
    //Sends the roomlist to the client who requested it.
    public void sendRoomListToClient(ConnectionToClient client) {
        Envelope e = new Envelope();
        e.setId("who");

        //Create an ArrayList to store the list of names or userId
        ArrayList<String> userList = new ArrayList<String>();
        String room = client.getInfo("room").toString();

        //Gets the list of all the active threas connected
        Thread[] clientThreadList = getClientConnections();

        //Loops through the list of Threadlist
        for (int i = 0; i < clientThreadList.length; i++) {
            ConnectionToClient target = (ConnectionToClient) clientThreadList[i];

            //Check for all the user in the room of the user who requested the list
            //The populate the ArrayList with the userid.
            if (target.getInfo("room").equals(room)) {
                userList.add(target.getInfo("userid").toString());
            }
        }

        //Set the content of Envelope e as userList
        e.setContents(userList);
        e.setArg(room); //pass the room name of the user who requested it in the arguments

        //Sends the list to client
        try {
            client.sendToClient(e);
        } catch (Exception ex) {
            System.out.println("Failed to send userList to client");
        }
    }
    
    //Sends private message to a client
    public void sendToAClient(String privateMessage, String targetUserId, ConnectionToClient client) {
        Thread[] clientThreadList = getClientConnections();

        for (int i = 0; i < clientThreadList.length; i++) {
            ConnectionToClient target = (ConnectionToClient) clientThreadList[i];

            //Find the user with same userid
            if (target.getInfo("userid").equals(targetUserId)) {
                try {
                    target.sendToClient(client.getInfo("userid") + ": " + privateMessage);
                } catch (Exception ex) {
                    System.out.println("Failed to send to client");
                }
            }

        }

    }
    
    //Sends messages to clients in that room.
    public void sendToAllClientsInRoom(Object msg, ConnectionToClient client) {
        Thread[] clientThreadList = getClientConnections();
        String room = client.getInfo("room").toString();

        for (int i = 0; i < clientThreadList.length; i++) {
            ConnectionToClient target = (ConnectionToClient) clientThreadList[i];
            if (target.getInfo("room").equals(room)) {
                try {
                    target.sendToClient(client.getInfo("userid") + ": " + msg);
                } catch (Exception ex) {
                    System.out.println("Failed to send to client");
                }
            }
        }
    }

    //Handelling message by server
    public void handleMessageFromServerUI(String message) {
        if (message.charAt(0) == '#') {

            handleServerCommand(message);

        } else {
            try {
                sendToAllClients("<ADMIN>: " + message);
            } catch (Exception ex) {
                serverUI.display("Could not send message to client.  Terminating server.......");
            }
        }
    }
    
    //This method is called in handleMessageFromServerUI
    public void handleServerCommand(String message) {
        if (message.indexOf("#setPort") >= 0) {
            //#setPort 5556
            if (isListening()) {
                serverUI.display("Cannot change port while listening");
            } else {
                setPort(Integer.parseInt(message.substring(8, message.length()).trim()));
            }
        }

        if (message.equals("#start")) {
            if (isListening()) {
                serverUI.display("already listening");
            } else {
                try {
                    this.listen(); //Start listening for connections
                } catch (Exception ex) {
                    serverUI.display("ERROR - Could not listen for clients!");
                }
            }
        }

        if (message.equals("#stop")) {
            try { //stops listening for connections
                close();
            } catch (IOException ioe) {
                serverUI.display("ERROR: failed to stop the server");
            }
        }

        if (message.equals("#quit")) {
            try { // stops listening and exits the program
                close();
                System.exit(0);
            } catch (IOException ioe) {
                serverUI.display("ERROR: failed to stop the server");
            }

        }

        if (message.indexOf("#ison") == 0) {
            boolean isUserPresent = false;
            try {
                String userId = message.substring(5, message.length()).trim();
                //Gets the list of all the active threads connected
                Thread[] clientThreadList = getClientConnections();

                for (int i = 0; i < clientThreadList.length; i++) {
                    ConnectionToClient target = (ConnectionToClient) clientThreadList[i];
                    //Find the user
                    if (target.getInfo("userid").equals(userId)) {
                        String roomName = target.getInfo("room").toString();
                        isUserPresent = true;
                        serverUI.display(userId + " is on the room " + roomName);
                    }
                }
                if (isUserPresent == false) {
                    serverUI.display(userId + " is not logged in");
                }
            } catch (Exception e) {
                serverUI.display("Error: failed to execute #ison command");
            }
        }

        if (message.equals("#userstatus")) {

            try {
                //Gets the list of all active threads connected
                Thread[] clientThreadList = getClientConnections();

                for (int i = 0; i < clientThreadList.length; i++) {
                    ConnectionToClient target = (ConnectionToClient) clientThreadList[i];
                    String userId = target.getInfo("userid").toString();
                    String roomName = target.getInfo("room").toString();

                    serverUI.display(userId + " - " + roomName);
                }
            } catch (Exception e) {
                serverUI.display("Error: failed to execute #userstatus command");
            }

        }

    }

    /**
     * This method overrides the one in the superclass. Called when the server
     * starts listening for connections.
     */
    @Override
    protected void serverStarted() {
        System.out.println("Server listening for connections on port " + getPort());
    }

    /**
     * This method overrides the one in the superclass. Called when the server
     * stops listening for connections.
     */
    @Override
    protected void serverStopped() {
        System.out.println("Server has stopped listening for connections.");
    }

    //Class methods ***************************************************
    /**
     * This method is responsible for the creation of the server instance (there
     * is no UI in this phase).
     *
     * @param args[0] The port number to listen on. Defaults to 5555 if no
     * argument is entered.
     */
    public static void main(String[] args) {
        int port = 0; //Port to listen on

        try {
            port = Integer.parseInt(args[0]);
        } catch (ArrayIndexOutOfBoundsException oob) {
            port = DEFAULT_PORT; //Set port to 5555
        }

        EchoServer sv = new EchoServer(port);

        try {
            sv.listen(); //Start listening for connections
        } catch (Exception ex) {
            System.out.println("ERROR - Could not listen for clients!");
        }
    }

    @Override
    protected void clientConnected(ConnectionToClient client) {

        System.out.println("<Client Connected:" + client + ">");

    }

    @Override
    synchronized protected void clientException(
            ConnectionToClient client, Throwable exception) {
        System.out.println("Client Shut down");

    }

}
//End of EchoServer class
