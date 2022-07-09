/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Final;

/**
 * Movie
 * Date: April 22, 2021
 * @author Akil Momin
 */
public class Movie {

    //Declaring variables.
    private int id;
    private String title;
    private String director;
    private String description;
    private String dateReleased;
    private String categoryID;
    private String inTheaters;

    public Movie() {
        //Default Constructor
    }

    //Constructor accepting 3 arguments
    public Movie(int itsId, String itsTitle, String itsDirector) { 
        setId(itsId);
        setTitle(itsTitle);
        setDirector(itsDirector);
    }
    
    //Constructor accepting 4 arguments.
    public Movie(int itsId, String itsTitle, String itsDirector, String itsDescription) { 
        setId(itsId);
        setTitle(itsTitle);
        setDirector(itsDirector);
        setDescription(itsDescription);
    }

    //Constructor accepting All arguments.
    public Movie(int itsId, String itsTitle, String itsDirector, String itsDateReleased, String itsDescription, String itsCategoryID, String itsInTheaters) { 
        setId(itsId);
        setTitle(itsTitle);
        setDirector(itsDirector);
        setDescription(itsDescription);
        setDateReleased(itsDateReleased);
        setCategoryID(itsCategoryID);
        setInTheaters(itsInTheaters);
    }

    public int getId() { //Getter method for Id.
        return this.id;
    }

    public void setId(int itsId) { //Setter method for Id.
        this.id = itsId;
    }

    public String getTitle() { //getter method for Title.
        return this.title;
    }

    public void setTitle(String itsTitle) { //Setter method for Title.
        this.title = itsTitle;
    }

    public String getDirector() { //Getter method for Director.
        return this.director;
    }

    public void setDirector(String itsDirector) { //Setter method for Director.
        this.director = itsDirector;
    }

    public String getDescription() { //Getter method for Description.
        return this.description;
    }

    public void setDescription(String itsDescription) { //Setter method for Description.
        this.description = itsDescription;
    }

    public String getDateReleased() { //Getter method for DataReleased.
        return this.dateReleased;
    }

    public void setDateReleased(String itsDateReleased) { //Setter method for DateReleased.
        this.dateReleased = itsDateReleased;
    }

    public String getCategoryID() { //Getter method for CategoryID.
        return this.categoryID;
    }

    public void setCategoryID(String itsCategoryID) { //Setter method for CategoryID.
        this.categoryID = itsCategoryID;
    }

    public String getInTheaters() { //Getter method for InTheaters.
        return this.inTheaters;
    }

    public void setInTheaters(String itsInTheaters) { //Setter method for InTheaters.
        this.inTheaters = itsInTheaters;
    }
}
