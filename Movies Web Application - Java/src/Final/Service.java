/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Final;

import java.io.DataOutputStream; //This import will be used to send data to client - Web Browser.
import java.util.ArrayList; // This import will be used to record the data as Array List.

/**
 * Service
 * Date: April 22, 2021
 * @author Akil Momin.
 */
public abstract class Service {
    //Creating variable.
    DataOutputStream responseWriter;

    public Service(DataOutputStream responseWriter) { //Constructor
        this.responseWriter = responseWriter;
    }

    public abstract void doWork(); // Creating an abstract class that will be compulsorily used in class which extends this class.
}

class SQLSelectService extends Service { 
    //Creating variables.
    private String requestString;
    private MovieDAO dbLayer;
    private String field;
    private String criteria;
    private ArrayList movies;

    public SQLSelectService(DataOutputStream responseWriter, String requestString) { //Constructor.
        super(responseWriter);
        this.requestString = requestString;
    }

    public void doWork() {

        try {
            responseWriter.writeBytes("<html><head><title>test"); // Writes it to client - Web Browser. 
            responseWriter.writeBytes("</title></head><body>"); // Writes it to client - Web Browser.
            
            // Filtering out the Fields and Criteria to be used in SELECT statement.
            field = requestString.substring(requestString.indexOf("Field=") + 6, requestString.indexOf("&Submit"));
            criteria = requestString.substring(27, requestString.indexOf("&Field"));
            criteria = criteria.replaceAll("\\+", " "); // Removes + and add spaces.
            
            dbLayer = new MovieDAO(); //Creating an instance of MovieDAO class.
            movies = dbLayer.getMovieByCriteria(field, criteria); //Passing a field and criteria which returns results from SQL SELECT Statement.

            // This for loop writes the extracted data from database to Client - Web Browser.
            for (int Counter = 0; Counter < movies.size(); Counter++) {  
                Movie m;
                m = (Movie) movies.get(Counter); // Returns an element specified in ArrayList and cast it to Movie.
                responseWriter.writeBytes("ID: " + m.getId() + "<br />" +
                                           "Title: " + m.getTitle() + "<br />" +
                                           "Director: " + m.getDirector() + "<br />" +
                                           "Date Released: " + m.getDateReleased() + "<br />" +
                                           "Description: " + m.getDescription() + "<br />" +
                                           "CategoryID: " + m.getCategoryID() + "<br />" +
                                           "In Theaters: " + m.getInTheaters() + "<br /><br />");
            }
            responseWriter.writeBytes("</body></html>"); 
            responseWriter.close(); //This tells the browser that we are done sending.

        } catch (Exception e) {
            e.printStackTrace(); //Prints the exception error in case if try statement fails
        }

    }

}
