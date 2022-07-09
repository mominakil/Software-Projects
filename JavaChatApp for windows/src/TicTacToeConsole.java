/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

import java.awt.GridLayout;
import java.awt.LayoutManager;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.IOException;
import java.util.Arrays;
import javax.swing.*;

/**
 *
 * @author gavin
 */
public class TicTacToeConsole extends JFrame {

    private JButton b1 = new JButton("-");
    private JButton b2 = new JButton("-");
    private JButton b3 = new JButton("-");
    private JButton b4 = new JButton("-");
    private JButton b5 = new JButton("-");
    private JButton b6 = new JButton("-");
    private JButton b7 = new JButton("-");
    private JButton b8 = new JButton("-");
    private JButton b9 = new JButton("-");

    ChatIF clientUI;
    TicTacToe game;
    char t;
    boolean isPlayed;

    public TicTacToeConsole(ChatIF clientUI,String playerName) {
        //Added an variable in this class for displaying play name on the window.
        super("TicTacToe: " + playerName);
        isPlayed = false;
        this.setLayout(new GridLayout(3, 3, 3, 3));
        add(b1);
        add(b2);
        add(b3);
        add(b4);
        add(b5);
        add(b6);
        add(b7);
        add(b8);
        add(b9);
        setSize(300, 300);
        setVisible(true);

        this.clientUI = clientUI;
        game = new TicTacToe();

        //Click event handler for all buttons in TicTacToeGUI
        b1.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                //If the boarder spot has been already placed (ether X or O)
                //Or player has played
                //Then return without any actions.
                if (game.getBoard()[0][0] == 'X' || game.getBoard()[0][0] == 'O' || !isPlayed) {
                    return;
                }
                
                game.updateBoard(1); //Update the board with the buttion clicked
                isWinningMove(game.getBoard()); //Check if this is a winning move
                sendToServer(); // sending it to server
            }
        });
        b2.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[0][1] == 'X' || game.getBoard()[0][1] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(2);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });
        b3.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[0][2] == 'X' || game.getBoard()[0][2] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(3);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });
        b4.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[1][0] == 'X' || game.getBoard()[1][0] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(4);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });
        b5.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[1][1] == 'X' || game.getBoard()[1][1] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(5);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });
        b6.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[1][2] == 'X' || game.getBoard()[1][2] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(6);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });
        b7.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[2][0] == 'X' || game.getBoard()[2][0] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(7);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });

        b8.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[2][1] == 'X' || game.getBoard()[2][1] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(8);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });
        b9.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if (game.getBoard()[2][2] == 'X' || game.getBoard()[2][2] == 'O' || !isPlayed) {
                    return;
                }
                game.updateBoard(9);
                isWinningMove(game.getBoard());
                sendToServer();
            }
        });

    }


    public void quit() {
        setVisible(false);
    }

    public TicTacToe getGame() {
        return game;
    }

    public void setGame(TicTacToe game) {
        this.game = game;
    }

    //This method will be called on every move of the players to update the board
    public void updateBoard(char[][] board) {
        b1.setText(board[0][0] + "");
        b2.setText(board[0][1] + "");
        b3.setText(board[0][2] + "");
        b4.setText(board[1][0] + "");
        b5.setText(board[1][1] + "");
        b6.setText(board[1][2] + "");
        b7.setText(board[2][0] + "");
        b8.setText(board[2][1] + "");
        b9.setText(board[2][2] + "");

    }

    public boolean checkWin(char[][] board) {
        //Horizontal
        if (board[0][0] == board[0][1] && board[0][0] == board[0][2] && board[0][0] != 0) {
            return true;
        } else if (board[1][0] == board[1][1] && board[1][0] == board[1][2] && board[1][0] != 0) {
            return true;
        } else if (board[2][0] == board[2][1] && board[2][0] == board[2][2] && board[2][0] != 0) {
            return true;
        }
        //Vertical
        else if (board[0][0] == board[1][0] && board[0][0] == board[2][0] && board[0][0] != 0) {
            return true;
        } else if (board[0][1] == board[1][1] && board[0][1] == board[2][1] && board[0][1] != 0) {
            return true;
        } else if (board[0][2] == board[1][2] && board[0][2] == board[2][2] && board[0][2] != 0) {
            return true;
        }

        //Diagonal
        else if (board[0][2] == board[1][1] && board[0][2] == board[2][0] && board[0][2] != 0) {
            return true;
        } else if (board[0][0] == board[1][1] && board[0][0] == board[2][2] && board[0][0] != 0) {
            return true;
        }

        //return false if no condition is satisfied
        return false;
    }

    public void isWinningMove(char[][] board) {
        if (checkWin(board) == false) {
            return;
        }
        game.setGameState(4);
        clientUI.display("Congratulations!! You have won the game");
        setVisible(false);
    }

    public void sendToServer() {
        //update the latest board
        updateBoard(game.getBoard());
        
        //Checking if the instance if the player is from ChatClient or from GUIConsole
        //Because Player1's instance has been declared in GUIConsole and Player2's instance has been delcared in ChatClient 
        if (clientUI instanceof ChatClient) {
            try {
                Envelope env = new Envelope("#ttt", "", game);
                ((ChatClient) clientUI).sendToServer(env);
            } catch (IOException io) {
                clientUI.display("Error: Failed to send it to server from TicTacToeConsole");
            }
        } else if (clientUI instanceof GUIConsole) {
            try {
                Envelope env = new Envelope("#ttt", "", game);
                ((GUIConsole) clientUI).client.sendToServer(env);
            } catch (IOException io) {
                clientUI.display("Error: Failed to send it to server after winning the game");
            }
        }

        isPlayed = false;
    }

    public static void main(String[] args) {
        TicTacToeConsole ttt = new TicTacToeConsole(null,"Tic Tac Toe");

    }

}
