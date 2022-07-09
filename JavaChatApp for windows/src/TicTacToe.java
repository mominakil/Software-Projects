
import java.io.Serializable;

/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
/**
 *
 * @author akilm
 */
public class TicTacToe implements Serializable {

    private String Player1;
    private String Player2;
    private int activePlayer; // 1 or 2 // 1=X 2=O
    private int gameState; // 1=invite, 2=decline, 3=playing, 4=won  
    private char[][] board = new char[3][3];

    public void checkwin() {
        //Horizontal checking
        if (board[0][0] == board[0][1] && board[0][0] == board[0][2]) {
            System.out.println(activePlayer + " won the game");
        } else if (board[1][0] == board[1][1] && board[1][0] == board[1][2]) {
            System.out.println(activePlayer + " won the game");
        } else if (board[2][0] == board[2][1] && board[2][0] == board[2][2]) {
            System.out.println(activePlayer + " won the game");
        }
        //Vertical checking
        if (board[0][0] == board[1][0] && board[0][0] == board[2][0]) {
            System.out.println(activePlayer + " won the game");
        } else if (board[0][1] == board[1][1] && board[0][1] == board[2][1]) {
            System.out.println(activePlayer + " won the game");
        } else if (board[0][2] == board[1][2] && board[0][2] == board[2][2]) {
            System.out.println(activePlayer + " won the game");
        }

        //Diagonal checking
        if (board[0][2] == board[1][1] && board[0][2] == board[2][0]) {
            System.out.println(activePlayer + " won the game");
        } else if (board[0][0] == board[1][1] && board[0][0] == board[2][2]) {
            System.out.println(activePlayer + " won the game");
        }

    }

    public void updateBoard(int move) {
        //move is 1 to 9 based on board square
        if (activePlayer == 1) {
            board[(move - 1) / 3][(move - 1) % 3] = 'X';
        } else {
            board[(move - 1) / 3][(move - 1) % 3] = 'O';
        }
    }
    
    
    //Getters and Setters
    public String getPlayer1() {
        return Player1;
    }

    public void setPlayer1(String Player1) {
        this.Player1 = Player1;
    }

    public String getPlayer2() {
        return Player2;
    }

    public void setPlayer2(String Player2) {
        this.Player2 = Player2;
    }

    public int getActivePlayer() {
        return activePlayer;
    }

    public void setActivePlayer(int activePlayer) {
        this.activePlayer = activePlayer;
    }

    public int getGameState() {
        return gameState;
    }

    public void setGameState(int gameState) {
        this.gameState = gameState;
    }

    public char[][] getBoard() {
        return board;
    }

    public void setBoard(char[][] board) {
        this.board = board;
    }

}
