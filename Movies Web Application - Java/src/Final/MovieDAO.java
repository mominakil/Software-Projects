/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Final;
//imports

import java.sql.Connection; // This import is used to create a connection with SQL Server.
import java.sql.Statement; // This used to execute query based on the sql.connection created.
import java.sql.DriverManager; // This is used to create connection with database.
import java.sql.ResultSet; // This import will Store the SELECT Statement results. The data is stored in table format. 
import java.sql.SQLException; // This import is used to detect SQL Exception errors in catch block.
import java.util.ArrayList; // This import is used to create and store data in ArrayList.


/**
 * MovieDAO
 * Date: April 22, 2021
 * @author Akil Momin.
 */
public class MovieDAO {

    public Connection getConnection() {

        Connection conn = null; // Initializing Connection variable.

        try {
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

            String connectionUrl = "jdbc:sqlserver://localhost:1434;DatabaseName=Movies; "
                    + "User=javaApps; Password=java"; // Connection string used to connect to the database.

            conn = DriverManager.getConnection(connectionUrl); // Connecting to database using connection string.

        } catch (Exception e) {
            e.printStackTrace(); // Displays an error if try block fails
            conn = null;
        }

        return conn;
    }

    public ArrayList getMovies() {

        ArrayList movies = new ArrayList(); // Creating an ArrayList
        Connection conn = getConnection(); // Creating a Connection variable

        ResultSet rs = null;

        try { // The try statement executes, runs and stores data from sql database in ArrayList.
            Statement stmt = conn.createStatement();
            rs = stmt.executeQuery("SELECT id, title, director, description FROM Movies");
            while (rs.next()) {
                Movie m = new Movie(Integer.parseInt(rs.getString(1)),
                        rs.getString(2), rs.getString(3),
                        rs.getString(4));
                movies.add(m);
            }
            conn.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return movies;
    }

    public Movie getMovieById(int itsId) { // This method retrieves data from database using Id.
        Connection conn = getConnection(); // Creates a connection with SQL Server database.
        Movie result = null;
        ResultSet rs = null;

        try { // This try block runs and retrieves data from database based on Id and stores it in ArrayList.
            Statement stmt = conn.createStatement();
            rs = stmt.executeQuery("SELECT id, title, director, description FROM Movies WHERE id=" + itsId);

            if (rs.next()) {
                result = new Movie(Integer.parseInt(rs.getString(1)),
                        rs.getString(2), rs.getString(3),
                        rs.getString(4));
            }
            conn.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return result;
    }

    public void updateMovie(Movie m) { // This method updated the database.

        Connection conn = getConnection(); //Creates connection with SQL Server database.

        try { //This try block Updates the database.
            Statement stmt = conn.createStatement();
            String updateStatement = "UPDATE Movies SET Title='" + m.getTitle()
                    + "'," + "Director = '" + m.getDirector()
                    + "'," + "Description = '" + m.getDescription()
                    + "' WHERE id=" + m.getId();
            stmt.executeUpdate(updateStatement);
            conn.close();
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    // This Method accepts 2 parameters and uses it to search a SELECT Query in Database.
    public ArrayList getMovieByCriteria(String field, String criteria) {
        Connection conn = getConnection(); // Creates a connection with SQL Server database.
        ArrayList movies = new ArrayList(); // Creating an ArrayList for movies.
        ResultSet rs = null; 
        
        try { // This try statement will execute the query based on field and criteria and record the result in loop under ArrayList.
            Statement stmt = conn.createStatement();
            rs = stmt.executeQuery("SELECT * FROM Movies WHERE "+field+" = '"+ criteria +"'");
            while (rs.next()) { // This while loop will loop through each resutset and will record it in ArrayList.
                Movie m = new Movie(Integer.parseInt(rs.getString(1)),
                        rs.getString(2), rs.getString(3),
                        rs.getString(4), rs.getString(5),
                        rs.getString(6), rs.getString(7));
                movies.add(m);
            }
            conn.close();

        }catch (SQLException sqlex){
            sqlex.printStackTrace(); // Print error in case of exception from SQL.
        }
        catch (Exception e) {
            e.printStackTrace(); // Prints errors in case if the try statement fails.
        }
        return movies;
    }

}
