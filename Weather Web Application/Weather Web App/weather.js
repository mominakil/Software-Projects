
// to be used inside xhttp function to retrieve data from openweatermap 
var Longitude; 
var Latitude;
var City1;
var City2;
// Temperature Preference
var CelsiusRadioButton;
var FahrenheitRadioButton;

// This event will start as soon as initial HTML document has been complete.
document.addEventListener('DOMContentLoaded', function () { 
    PerformSearch();
    LoadFavourites();
});

window.onload = PerformSearch; // Loads the function on every refresh or first page load.

function PerformSearch() {
    //document.getElementById('radio1').checked
     CelsiusRadioButton = document.getElementById('Celsius');
     FahrenheitRadioButton = document.getElementById('Fahrenheit');
    
    //This retrives the element with name of that particular ID.
     City1 = document.getElementById('City1');
     City2 = document.getElementById('City2');

    //This is the apiKey used to search in openweather map.
    var apiKey = "4b28476ebd5cf1a07d111858b5eeaa62";

    // This URL will be used to search in openweatermap to retrieve weather data for each city.
    var url1 = "http://api.openweathermap.org/data/2.5/weather" +
        "?q=" + City1.value + "&APPID=" + apiKey;
    var url2 = "http://api.openweathermap.org/data/2.5/weather" +
        "?q=" + City2.value + "&APPID=" + apiKey;

    // Runs only if city1 is not empty
    if (City1.value != "") {
        loadDoc(url1, SearchResultCity1, "City1");
    }

    // Runs only if city2 is not empty
    if (City2.value != "") {
        loadDoc(url2, SearchResultCity2, "City2");
    }

    //This function will first search latitude and longitude based on user input for - 
    //each city and with that coordinates it will search in one api to get current and forcasted weather.
    function loadDoc(url, cFunction , City) {
        var xhttp = new XMLHttpRequest(); //XMLHttpRequest is used to connect with web server to get and receive information requested.

        //onreadystatechange will execute when the readyState changes.
        xhttp.onreadystatechange = function () {
            //The if statement will execute only when the condition is fulfilled,i.e we receved a good data repsonse.
            if (this.readyState == 4 && this.status == 200) {

                var SearchResponse = this.responseText; //stores the response received.

                var obj = JSON.parse(SearchResponse); //Parse the received response to object.

                //Extracts the coordinates information from the object.
                Longitude = obj["coord"]["lon"]; 
                Latitude = obj["coord"]["lat"];

                SearchForcast(); // This function looks for Currrent and forecasted weather based on coordinates.
                SaveFavouritePreferences(City); //This functon will save the user preference for cities and Temprature unit.
            } else if (this.status == 404) { //The else if will execute when the server sends us 404 error for data not found.
                var CurrentWeather1 = document.getElementById('CurrentWeather1');
                var CurrentWeather2 = document.getElementById('CurrentWeather2');
                var HourlyWeather1 = document.getElementById('HourlyWeather1');
                var HourlyWeather2 = document.getElementById('HourlyWeather2');

                //The if and else statement will display the error and blank out the forms.
                if (City == "City1") {
                    CurrentWeather1.innerHTML = "";
                    HourlyWeather1.innerHTML = "";
                    CurrentWeather1.innerHTML = "City name not found.";
                    City1.value = "";
                } else if (City == "City2") {
                    CurrentWeather2.innerHTML = ""; 
                    HourlyWeather2.innerHTML = "";
                    CurrentWeather2.innerHTML = "City name not found."; 
                    City2.value = "";
                }

               
            }

        }

        xhttp.open("GET", url, true); //Creates a connection with the server.
        xhttp.send(); // Send the request to the server.
        
        //The SearchForecast() function will search the current weather and forcasted weather based on latitude and longitude.
        function SearchForcast() { // for Searching in oneAPI Call.
            
            var xhttp2 = new XMLHttpRequest(); //XMLHttpRequest is used to connect with web server to get and receive information requested.

            //onreadystatechange will execute when the readyState changes.
            xhttp2.onreadystatechange = function () {
                //The if statement will execute only when the condition is fulfilled,i.e we receved a good data repsonse.
                if (this.readyState == 4 && this.status == 200) {
                    cFunction(this);

                } else if (this.status == 404) { //The else if will execute when the server sends us 404 error for data not found.
                    var HourlyWeather1 = document.getElementById('HourlyWeather1');
                    var HourlyWeather2 = document.getElementById('HourlyWeather2');

                    //The if and else statement will display the an error message if the data is not found..
                    if (City == "City1") {
                        HourlyWeather1.innerHTML = "Weather forecast unavailable - Please retry";
                    } else if (City == "City2") {
                         HourlyWeather2.innerHTML = "Weather forecast unavailable - Please retry";
                    }
          
                }

            }

            //This Api key is used to search in open weathermap.
            var MyApiKey = "4b28476ebd5cf1a07d111858b5eeaa62";

            //SearchString contains the server link to retrieve data from openweater map.
            var SearchString = "http://api.openweathermap.org/data/2.5/onecall" +
                "?lat=" + Latitude + "&lon=" + Longitude +
                "&APPID=" + MyApiKey;

            xhttp2.open("GET", SearchString, true); // Creates a connection with opeanweater map server.
            xhttp2.send(); // Send the requests and receives a resonse.
        }


    }

    //This function will extract the received data from openweatermap and output it to user.
    function SearchResultCity1(xhttp) {
        var GetResponse = xhttp.responseText; // Receives the response.

        //Gets the HTML elements by its id.
        var CurrentWeather = document.getElementById('CurrentWeather1');
        var HourlyWeather = document.getElementById('HourlyWeather1');
        var obj = JSON.parse(GetResponse); // Parse the received response data into object.

        //Clear the old innerHTML of Current and Hourly if it exists in order to avoid appending with old data.
        CurrentWeather.innerHTML = "";
        HourlyWeather.innerHTML = "";

        //Extracts the Current Weather data from the object:        
        var CurrentLongitude = obj.lon;
        var CurrentLatitude = obj.lat;
        var CurrentTime = new Date(obj.current.dt * 1000);
        var SunriseTime = new Date(obj.current.sunrise * 1000);
        var SunsetTime = new Date(obj.current.sunset * 1000);
        var Pressure = obj.current.pressure;
        var WindSpeed = obj.current.wind_speed;
        var WindDirection = obj.current.wind_deg;
        var WeatherDescription = obj.current.weather[0].description;
        var WeatherIcon = obj.current.weather[0].icon;

        var TempratureKelvin = obj.current.temp;
        var Temprature;

        // Converts the extracted temprature from Kelvin to user preffered unit.
        if (CelsiusRadioButton.checked) {
            Temprature = TempratureKelvin - 273.15;
        } else if (FahrenheitRadioButton.checked) {
            Temprature = 9 / 5 * (TempratureKelvin - 273.15) + 32;
        }

        //Format the extracted result to be displyed it to the user.
        var SearchResultsHTML = "Longitude: " + CurrentLongitude + "<br />" +
                                "Latitude: " + CurrentLatitude + "<br />" +
                                "Current Time: " + CurrentTime.toLocaleTimeString() + "<br />" +
                                "Sunrise Time: " + SunriseTime.toLocaleTimeString() + "<br />" +
                                "Sunset Time: " + SunsetTime.toLocaleTimeString() + "<br />" +
                                "Temprature: " + Temprature + "<br />" +
                                "Pressure: " + Pressure + "<br />" +
                                "Wind Speed: " + WindSpeed + "<br />" +
                                "Wind Direction: " + WindDirection + "<br />" +
                                "Weather Description: " + WeatherDescription +
                                "<img src='http://openweathermap.org/img/wn/" + WeatherIcon + "@2x.png'>" + "<br \>";

        var setDate = document.getElementById("DateTextBox"); // Gets the element to set date.
        setDate.value = CurrentTime.toLocaleDateString(); // Sets the date in the text box
        CurrentWeather.innerHTML = SearchResultsHTML; // Displays the result to the user under current weather.

        //Extracting the first 3 hours for forcasted Hourly Weather: 
        var Forecasts = [];
        for (var Counter = 0; Counter < 3; Counter++) {
            Forecasts.push(obj.hourly[Counter]);
        }

        //Extracting the required data for each hour at a time and in total of 3 hours.
        for (var counter = 0; counter < 3; counter++) {
            //Extracting each hour data.
            var ForTempratureKelvin = Forecasts[counter].temp;
            var ForPressure = Forecasts[counter].pressure;
            var ForWindSpeed = Forecasts[counter].wind_speed;
            var ForWindDirection = Forecasts[counter].wind_deg;
            var ForWeatherDesc = Forecasts[counter].weather[0].description;
            var ForProbOfPrecipitation = Forecasts[counter].pop;
            var ForWeatherIcon = Forecasts[counter].weather[0].icon;
            var ForeCastDate = new Date(Forecasts[counter].dt * 1000);  
            var ForTemprature;
            
            //Converts the user preffered temprature unit from Kelvin.
            if (CelsiusRadioButton.checked) {
                ForTemprature = ForTempratureKelvin - 273.15;
            } else if (FahrenheitRadioButton.checked) {
                ForTemprature = 9 / 5 * (ForTempratureKelvin - 273.15) + 32;
            }

            // Format the result for displaying it to the user.
            var SearchResultHTML = "Time: " + ForeCastDate.toLocaleTimeString() + "<br />" +
                                   "Temprature: " + ForTemprature + "<br />" +
                                   "Pressure: " + ForPressure + "<br />" +
                                   "Wind Speed: " + ForWindSpeed + "<br />" +
                                   "Wind Direction: " + ForWindDirection + "<br />" +
                                   "Probability of Precipitation: " + ForProbOfPrecipitation + "<br />" +
                                   "Weather Description: " + ForWeatherDesc +
                                   "<img src='http://openweathermap.org/img/wn/" + ForWeatherIcon + "@2x.png'>" + "<br \>" +
                                   "------------------------------------------------------------------------------------------" + "<br /> <br />";

            HourlyWeather.innerHTML += SearchResultHTML; // Displays the appended results after each loop.

            
        }


    }

     //This function will extract the received data from openweatermap and output it to user.
    function SearchResultCity2(xhttp) {
        var GetResponse = xhttp.responseText; //Receives the response.
        var CurrentWeather = document.getElementById('CurrentWeather2');
        var HourlyWeathers = document.getElementById('HourlyWeather2');
        var obj = JSON.parse(GetResponse); // Converts the received data into an object.

        //Clear the old innerHTML of Current and Hourly if it exists in order to avoid appending with old data
        CurrentWeather.innerHTML = "";
        HourlyWeathers.innerHTML = "";

        //Extracts the Current Weather data from the object:  
        var CurrentLongitude = obj.lon;
        var CurrentLatitude = obj.lat;
        var CurrentTime = new Date(obj.current.dt * 1000);
        var SunriseTime = new Date(obj.current.sunrise * 1000);
        var SunsetTime = new Date(obj.current.sunset * 1000);
        var Pressure = obj.current.pressure;
        var WindSpeed = obj.current.wind_speed;
        var WindDirection = obj.current.wind_deg;
        var WeatherDescription = obj.current.weather[0].description;
        var WeatherIcon = obj.current.weather[0].icon;

        var TempratureKelvin = obj.current.temp;
        var Temprature;

        // Converts the extracted temprature from Kelvin to user preffered unit.
        if (CelsiusRadioButton.checked) {
            Temprature = TempratureKelvin - 273.15;
        } else if (FahrenheitRadioButton.checked) {
            Temprature = 9 / 5 * (TempratureKelvin - 273.15) + 32;
        }

        //Format the extracted result to be displyed it to the user.
        var SearchResultsHTML = "Longitude: " + CurrentLongitude + "<br />" +
                                "Latitude: " + CurrentLatitude + "<br />" +
                                "Current Time: " + CurrentTime.toLocaleTimeString() + "<br />" +
                                "Sunrise Time: " + SunriseTime.toLocaleTimeString() + "<br />" +
                                "Sunset Time: " + SunsetTime.toLocaleTimeString() + "<br />" +
                                "Temprature: " + Temprature + "<br />" +
                                "Pressure: " + Pressure + "<br />" +
                                "Wind Speed: " + WindSpeed + "<br />" +
                                "Wind Direction: " + WindDirection + "<br />" +
                                "Weather Description: " + WeatherDescription +
                                "<img src='http://openweathermap.org/img/wn/" + WeatherIcon + "@2x.png'>" + "<br \>";

        var setDate = document.getElementById("DateTextBox"); // Gets the element to set date.
        setDate.value = CurrentTime.toLocaleDateString(); // Sets the date in the text box
        CurrentWeather.innerHTML = SearchResultsHTML;// Displays the result to the user under current weather.

        //Extracting the first 3 hours for forcasted Hourly Weather:
        var Forecasts = [];
        for (var Counter = 0; Counter < 3; Counter++) {
            Forecasts.push(obj.hourly[Counter]);
        }

        //Extracting the required data for each hour at a time and in total of 3 hours.
        for (var counter = 0; counter < 3; counter++) {

            var ForTempratureKelvin = Forecasts[counter].temp;
            var ForPressure = Forecasts[counter].pressure;
            var ForWindSpeed = Forecasts[counter].wind_speed;
            var ForWindDirection = Forecasts[counter].wind_deg;
            var ForWeatherDesc = Forecasts[counter].weather[0].description;
            var ForProbOfPrecipitation = Forecasts[counter].pop;
            var ForWeatherIcon = Forecasts[counter].weather[0].icon;
            var ForeCastDate = new Date(Forecasts[counter].dt * 1000); 
            var ForTemprature;
            
            // Converts the extracted temprature from Kelvin to user preffered unit.
            if (CelsiusRadioButton.checked) {
                ForTemprature = ForTempratureKelvin - 273.15;
            } else if (FahrenheitRadioButton.checked) {
                ForTemprature = 9 / 5 * (ForTempratureKelvin - 273.15) + 32;
            }

            //Fromatting the extracted results for displaying it to user.
            var SearchResultHTML = "Time: " + ForeCastDate.toLocaleTimeString() + "<br />" +
                                   "Temprature: " + ForTemprature + "<br />" +
                                   "Pressure: " + ForPressure + "<br />" +
                                   "Wind Speed: " + ForWindSpeed + "<br />" +
                                   "Wind Direction: " + ForWindDirection + "<br />" +
                                   "Probability of Precipitation: " + ForProbOfPrecipitation + "<br />" +
                                   "Weather Description: " + ForWeatherDesc +
                                   "<img src='http://openweathermap.org/img/wn/" + ForWeatherIcon + "@2x.png'>" + "<br \>" +
                                   "------------------------------------------------------------------------------------------" + "<br /> <br />";

            HourlyWeathers.innerHTML += SearchResultHTML; // Displays the appended results after each loop.
        }

    }
}


// This function loads the previously saved user data from Local Storage and displays it to the user. 
function LoadFavourites() {
    var FavoutitesStored = localStorage.getItem('Favourites'); // Gets the data from local storage.

    //The if statement will execute if it has previously stored data in Local Storage.
    if (FavoutitesStored) {
        var Favourite = JSON.parse(FavoutitesStored); // Converts the stored data to an Object.

        //Set the city to favourites
        City1.value = Favourite.city1;
        City2.value = Favourite.city2;

        //Set the temprature to favourites
        CelsiusRadioButton.checked = Favourite.celsius;
        FahrenheitRadioButton.checked = Favourite.fahrenheit;

    }

}

// This function saves the users favourite preferences into local storage.
function SaveFavouritePreferences(City) {
    var Favourite = new Object();

    // retrieve and parse localStorage
    var FavoutitesStored = localStorage.getItem('Favourites');
    if (FavoutitesStored) {
        var Favourite = JSON.parse(FavoutitesStored);
    }
        
         // change only the Favorite setting.
        if (City == "City1") {
            Favourite.city1 = City1.value;
        } else if (City == "City2") {
            Favourite.city2 = City2.value;
        }    

    //Checks for Favourite Temprature preference of user.
    Favourite.celsius = CelsiusRadioButton.checked;
    Favourite.fahrenheit = FahrenheitRadioButton.checked;

    //Store it in local storage.
    localStorage.setItem('Favourites', JSON.stringify(Favourite));

}
