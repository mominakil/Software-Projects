using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyGames.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;

namespace PolyGames.DataAccessObjects
{
    
    public class GameDAO
    {
        //Use this connection string to connect to local desktop database
        string conString = "Data Source=localhost;Initial Catalog=PolyGames;Integrated Security=True";

        //Use this connection string to connect to the dev server (VDI) database
        //string conString = "Data Source=BISIISDEV;Initial Catalog=PolyGames;User ID=bisstudent;Password=bobby2013;Integrated Security=False";

        //Use this connection string to connect to the test server (VDI) database
        //string conString = "Data Source=BISIISTEST;Initial Catalog=PolyGames;User ID=bisstudent;Password=bobby2013;Integrated Security=False";

        //Query used for allYears page
        public Games getAllYears()
        {
            //Create connection to the backend database
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            //Set your SQL command
            cmd.CommandText = "SELECT DISTINCT Year FROM Games";

            //Create list that will hold the list of years returned
            List<Game> gs = new List<Game>();

            //Open connection to database
            con.Open();

            //Execute query
            SqlDataReader reader = cmd.ExecuteReader();

            //Read through the results of the query
            while (reader.Read())
            {
                Game game = new Game(Convert.ToInt32(reader["Year"]));
                gs.Add(game);
            }
            //Close database connection
            con.Close();

            //Adding the years to a list and returning it to the Home Controller
            Games allYears = new Games();
            allYears.Items = gs;
            return allYears;
        }

        public User Login(User UTC)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string bruh = UTC.Email;
                string pw = UTC.Password;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT Email, Password, IsAdmin FROM UserLogin WHERE Password=@Password AND Email=@Email";
                cmd.Parameters.AddWithValue("@Email", bruh);
                cmd.Parameters.AddWithValue("@Password", pw);

                List<User> user = new List<User>();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                User nu = new User(reader["Email"].ToString(),
                                    reader["Password"].ToString(),
                                    (bool)reader["IsAdmin"]);
                con.Close();
                return nu;
            }
            catch(InvalidOperationException ioe)
            {
            
                return null;
                 
            }
        }

        public void OldaddUser(User u)
        {

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            con.Open();
            cmd.CommandText = "INSERT INTO UserLogin(Email, Password) VALUES(@Email, @Password)";
            if (u.Email == null)
            {
                cmd.Parameters.AddWithValue("@Email", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Email", u.Email);
            }
            if (u.Password == null)
            {
                cmd.Parameters.AddWithValue("@Password", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Password", u.Password);
            }
            cmd.ExecuteNonQuery();
            con.Close();
        }


        //Query to be used for Home page to display recently added games (video with clickable link that takes user to that game's game page)
        public Games getGamesOrderedByMostRecentlyAdded()
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            List<Game> gs = new List<Game>();
            con.Open();

            using (con)
            {
                //Set your SQL command to use the stored procedure in the database
                cmd = new SqlCommand("usp_GetAllGamesOrderedByRecentlyAdded", con);
                //Set the type of command to a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Game g = new Game(
                                    Convert.ToInt32(reader["GameId"]),
                                    reader["GameName"].ToString(),
                                    reader["GameDescription"].ToString()
                                    //reader["pictureFilePath"].ToString(),
                                    //reader["videoFilePath"].ToString()
                               );

                    g.GamePictures = getGamePictures(g.Id);
                    g.GameVideos = getGameVideos(g.Id);

                    gs.Add(g);
                }
            }
            con.Close();

            //Adding the games to a list and returning them to the Home Controller
            Games allGames = new Games();
            allGames.Items = gs;
            return allGames;
            
        }

        //Query to be used for individual game page
        public Game getGameById(int id)
        {
            //Get game details, group details, video, and executable file (all 1:1 relationships)
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd = new SqlCommand("usp_GetGameDetailsOne", con);
            cmd.Parameters.AddWithValue("@GameId", id);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            Game g = new Game(
                                Convert.ToInt32(reader["GameId"]),
                                reader["GameName"].ToString(),                
                                reader["GameDescription"].ToString()   
                                );
                g.GameVideos = getGameVideos(g.Id);
                g.executableId = Convert.ToInt32(reader["executableId"]);
                g.GameName = reader["GameName"].ToString();
                g.GroupName = reader["GroupName"].ToString();
                g.GroupId = Convert.ToInt32(reader["GroupId"]);
                g.Year = Convert.ToInt32(reader["Year"]);
                g.executableFilePath = reader["executableFilePath"].ToString();

            con.Close();

            //Get group member details (1:many relationship, one game can have many group members)
            g.groupMembers = getGroupMembers(g.GroupId);
          
            //Get picture details (1:many relationship, one game can have many pictures)
            g.GamePictures = getGamePictures(id);

            return g;
        }

        
        //Gets a list of group member details - called in the getGameById method
        public List<Student> getGroupMembers(int id)
        {
            List<Student> ss = new List<Student>();
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd = new SqlCommand("SELECT MemberToGroup.MemberId, studentRole, MemberToGroup.IsHidden, Name FROM UserLogin inner join MemberToGroup on MemberToGroup.MemberId = UserLogin.MemberId WHERE MemberToGroup.GroupId = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Student s = new Student(
                                Convert.ToInt32(reader["MemberId"]),
                                id,
                                reader["Name"].ToString(),
                                reader["studentRole"].ToString()
                    );
                s.isHidden = (bool)reader["IsHidden"];
                ss.Add(s);
            }
            con.Close();
            
            return ss;
        }

        //Query to be used for Game page to display all game pictures (called in the getGameByID method)
        public List<PictureFiles> getGamePictures(int id)
        {
            List<PictureFiles> ps = new List<PictureFiles>();
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd = new SqlCommand("SELECT pictureID, pictureFileName, pictureFilePath FROM PictureFiles WHERE gameID=@Id", con);
            //Add the game id value as a parameter in the query
            cmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PictureFiles p = new PictureFiles(
                                Convert.ToInt32(reader["pictureID"]),
                                reader["pictureFileName"].ToString(),
                                reader["pictureFilePath"].ToString(),
                                id
                    );
                ps.Add(p);
            }
            con.Close();

            return ps;
        }

        public List<VideoFiles> getGameVideos(int id)
        {
            List<VideoFiles> vs = new List<VideoFiles>();
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd = new SqlCommand("SELECT videoID, videoFileName, videoFilePath FROM videoFiles WHERE gameID=@Id", con);
            //Add the game id value as a parameter in the query
            cmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                VideoFiles v = new VideoFiles(
                                Convert.ToInt32(reader["videoID"]),
                                reader["videoFileName"].ToString(),
                                reader["videoFilePath"].ToString(),
                                id
                    );
                vs.Add(v);
            }
            con.Close();

            return vs;
        }

        //SQL commands to be used for the AddGame page
        public void addGame(Game g)
        {
            
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            //Insert values into Games table
            con.Open();
            cmd.CommandText = "INSERT INTO Games(Year, GameName, GameDescription) VALUES(@Year, @GameName, @GameDescription)";
            if (g.GameName == null) // need to add a way to implement a group id increment 
            {
                cmd.Parameters.AddWithValue("@GameName", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@GameName", g.GameName);
            }
            if (g.Description == null)
            {
                cmd.Parameters.AddWithValue("@GameDescription", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@GameDescription", g.Description);
            }
            cmd.Parameters.AddWithValue("@Year", g.Year);
            cmd.ExecuteNonQuery();
            con.Close();

            //Lookup the GameId and GroupId based on the GameName and GameDescription to use in the rest of the inserts
            con.Open();
            cmd.CommandText = "SELECT GameId, GroupId FROM Games WHERE GameName=@gName AND GameDescription=@gDesc";
            cmd.Parameters.AddWithValue("@gName", g.GameName);
            cmd.Parameters.AddWithValue("@gDesc", g.Description);
            SqlDataReader readerTwo = cmd.ExecuteReader();
            readerTwo.Read();
            int currentGameId = (Convert.ToInt32(readerTwo["GameId"]));
            int gameGroupId = (Convert.ToInt32(readerTwo["GroupId"]));
            con.Close();

            //Insert values into Group table
            con.Open();
            cmd.CommandText = "INSERT INTO PolyGamesGroups (GroupId, GroupName) VALUES (@grpId, @GroupName)";
            if (g.GroupName == null)
            {
                cmd.Parameters.AddWithValue("@GroupName", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@GroupName", g.GroupName);
            }
            cmd.Parameters.AddWithValue("@grpId", gameGroupId);
            cmd.ExecuteNonQuery();
            con.Close();

            //Insert values into Group Members table
            cmd.CommandText = "INSERT INTO GroupMembers (GroupId, studentRole, studentName) VALUES (@gId, @StudentRole, @StudentName)";
            cmd.Parameters.AddWithValue("@gId", gameGroupId);
            if (g.studentNameOne == null)
            {
                cmd.Parameters.AddWithValue("@StudentName", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@StudentName", g.studentNameOne);
            }
            if (g.studentRoleOne == null)
            {
                cmd.Parameters.AddWithValue("@StudentRole", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@StudentRole", g.studentRoleOne);
            }
            con.Open();
            cmd.ExecuteNonQuery();

            //Had to create individual group members so that each member had it's own text box and unique values to store in the database
            //Check if Group Member Two fields were filled out, execute query to add them to the DB if they were
            if (g.studentNameTwo != null && g.studentRoleTwo != null)
            {
                cmd.CommandText = "INSERT INTO GroupMembers (GroupId, studentRole, studentName) VALUES (@gTwoId, @StudentRoleTwo, @StudentNameTwo)";
                cmd.Parameters.AddWithValue("@gTwoId", gameGroupId);
                cmd.Parameters.AddWithValue("@StudentRoleTwo", g.studentRoleTwo);
                cmd.Parameters.AddWithValue("@StudentNameTwo", g.studentNameTwo);
                cmd.ExecuteNonQuery();
            }

            //Check if Group Member Three fields were filled out, execute query to add them to the DB if they were
            if (g.studentNameThree != null && g.studentRoleThree != null)
            {
                cmd.CommandText = "INSERT INTO GroupMembers (GroupId, studentRole, studentName) VALUES (@gThreeId, @StudentRoleThree, @StudentNameThree)";
                cmd.Parameters.AddWithValue("@gThreeId", gameGroupId);
                cmd.Parameters.AddWithValue("@StudentRoleThree", g.studentRoleThree);
                cmd.Parameters.AddWithValue("@StudentNameThree", g.studentNameThree);
                cmd.ExecuteNonQuery();
            }

            //Check if Group Member Four fields were filled out, execute query to add them to the DB if they were
            if (g.studentNameFour != null && g.studentRoleFour != null)
            {
                cmd.CommandText = "INSERT INTO GroupMembers (GroupId, studentRole, studentName) VALUES (@gFourId, @StudentRoleFour, @StudentNameFour)";
                cmd.Parameters.AddWithValue("@gFourId", gameGroupId);
                cmd.Parameters.AddWithValue("@StudentRoleFour", g.studentRoleFour);
                cmd.Parameters.AddWithValue("@StudentNameFour", g.studentNameFour);
                cmd.ExecuteNonQuery();
            }

            //Check if Group Member Five fields were filled out, execute query to add them to the DB if they were
            if (g.studentNameFive != null && g.studentRoleFive != null)
            {
                cmd.CommandText = "INSERT INTO GroupMembers (GroupId, studentRole, studentName) VALUES (@gFiveId, @StudentRoleFive, @StudentNameFive)";
                cmd.Parameters.AddWithValue("@gFiveId", gameGroupId);
                cmd.Parameters.AddWithValue("@StudentRoleFive", g.studentRoleFive);
                cmd.Parameters.AddWithValue("@StudentNameFive", g.studentNameFive);
                cmd.ExecuteNonQuery();
            }

            //Check if Group Member Six fields were filled out, execute query to add them to the DB if they were
            if (g.studentNameSix != null && g.studentRoleSix != null)
            {
                cmd.CommandText = "INSERT INTO GroupMembers (GroupId, studentRole, studentName) VALUES (@gSixId, @StudentRoleSix, @StudentNameSix)";
                cmd.Parameters.AddWithValue("@gSixId", gameGroupId);
                cmd.Parameters.AddWithValue("@StudentRoleSix", g.studentRoleSix);
                cmd.Parameters.AddWithValue("@StudentNameSix", g.studentNameSix);
                cmd.ExecuteNonQuery();
            }
            con.Close();

            //Insert values into Pictures table
            con.Open();
            if (g.picturesUpload != null)
            {
                string pictureFileName;
                int pictureFileSize = 0;
                int fileSize = 0;
                int picCounter = 0;

                using (con)
                {
                    cmd = new SqlCommand("usp_AddNewPictureFile", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //loop through the object that holds all of the pictures the user uploaded
                    foreach (var item in g.picturesUpload)
                    {
                        //Extract file attributes and save them to the database using the stored procedure
                        pictureFileName = item.FileName;
                        pictureFileSize = item.ContentLength;
                        fileSize = pictureFileSize / 1000;
                        cmd = new SqlCommand("usp_AddNewPictureFile", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pictureFileName", pictureFileName);
                        cmd.Parameters.AddWithValue("@pictureFileSize", fileSize);
                        cmd.Parameters.AddWithValue("@pictureFilePath", "~/PictureFileUpload/" + pictureFileName);
                        cmd.Parameters.AddWithValue("@gameID", currentGameId);
                        cmd.ExecuteNonQuery();
                        
                        picCounter++;
                        if (picCounter == 5) break; //once we've added 5 pictures to the DB, stop
                    }
                }
            }
            con.Close();

            //Insert values into Video table
            string CS = conString;
            SqlConnection conTwo = new SqlConnection(CS);
            SqlCommand cmdTwo = new SqlCommand();
            cmdTwo.Connection = conTwo;
            conTwo.Open();
            if (g.videoUpload != null)
            {
                string videoFileName = Path.GetFileName(g.videoUpload.FileName);
                int videoFileSize = g.videoUpload.ContentLength;
                int fileSize = videoFileSize / 1000;

                using (conTwo)
                {
                    cmdTwo = new SqlCommand("usp_AddNewVideoFile", conTwo);
                    cmdTwo.CommandType = CommandType.StoredProcedure;
                    cmdTwo.Parameters.AddWithValue("@videoFileName", videoFileName);
                    cmdTwo.Parameters.AddWithValue("@videoFileSize", fileSize);
                    cmdTwo.Parameters.AddWithValue("@videoFilePath", "~/VideoFileUpload/" + videoFileName);
                    cmdTwo.Parameters.AddWithValue("@gameID", currentGameId);
                    cmdTwo.ExecuteNonQuery();
                }
                conTwo.Close();
            }


            //Insert values into Executable table
            SqlConnection conThree = new SqlConnection(CS);
            SqlCommand cmdThree = new SqlCommand();
            cmdThree.Connection = conThree;
            conThree.Open();
            if (g.executableUpload != null)
            {
                string executableFileName = Path.GetFileName(g.executableUpload.FileName);
                int executableFileSize = g.executableUpload.ContentLength;
                int fileSize = executableFileSize / 1000;

                using (conThree)
                {
                    cmdThree = new SqlCommand("usp_AddNewExecutableFile", conThree);
                    cmdThree.CommandType = CommandType.StoredProcedure;
                    cmdThree.Parameters.AddWithValue("@executableFileName", executableFileName);
                    cmdThree.Parameters.AddWithValue("@executableFileSize", fileSize);
                    cmdThree.Parameters.AddWithValue("@executableFilePath", "~/ExecutableFileUpload/" + executableFileName);
                    cmdThree.Parameters.AddWithValue("@gameID", currentGameId);
                    cmdThree.ExecuteNonQuery();
                }
                conThree.Close();
            }
            
        }

        public void AddAGame(Game game)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            //For Add data in Game Table
            con.Open();
            cmd.CommandText = "INSERT INTO Games(Year, GameName, GameDescription, GroupId) VALUES(@Year, @GameName, @GameDescription, @GroupId)";
            cmd.Parameters.AddWithValue("@Year", game.Year);
            cmd.Parameters.AddWithValue("@GameName", game.GameName);
            cmd.Parameters.AddWithValue("@GameDescription", game.Description);
            cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();

            //Getting GameId
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "SELECT GameId FROM Games WHERE GroupId=@groupId";
            cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            game.Id = (int)reader["GameId"];
            con.Close();
            con.Dispose();

            //For Adding data to PictureFiles Table
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            if (game.picturesUpload != null)
            {
                string pictureFileName;
                int pictureFileSize = 0;
                int fileSize = 0;
                int picCounter = 0;

                using (con)
                {
                    cmd = new SqlCommand("usp_AddNewPictureFile", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //loop through the object that holds all of the pictures the user uploaded
                    foreach (var item in game.picturesUpload)
                    {
                        //Extract file attributes and save them to the database using the stored procedure
                        pictureFileName = item.FileName;
                        pictureFileSize = item.ContentLength;
                        fileSize = pictureFileSize / 1000;
                        cmd = new SqlCommand("usp_AddNewPictureFile", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pictureFileName", pictureFileName);
                        cmd.Parameters.AddWithValue("@pictureFileSize", fileSize);
                        cmd.Parameters.AddWithValue("@pictureFilePath", "~/PictureFileUpload/" + pictureFileName);
                        cmd.Parameters.AddWithValue("@gameID", game.Id);
                        cmd.ExecuteNonQuery();

                        picCounter++;
                        if (picCounter == 5) break; //once we've added 5 pictures to the DB, Limit 5 pictures.
                    }
                }
            }
            con.Close();
            con.Dispose();

            //For Adding data to VideoFile table
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            if (game.videoUpload != null)
            {
                string videoFileName = Path.GetFileName(game.videoUpload.FileName);
                int videoFileSize = game.videoUpload.ContentLength;
                int fileSize = videoFileSize / 1000;

                using (con)
                {
                    cmd = new SqlCommand("usp_AddNewVideoFile", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@videoFileName", videoFileName);
                    cmd.Parameters.AddWithValue("@videoFileSize", fileSize);
                    cmd.Parameters.AddWithValue("@videoFilePath", "~/VideoFileUpload/" + videoFileName);
                    cmd.Parameters.AddWithValue("@gameID", game.Id);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
                con.Dispose();
            }


            //For Adding data to ExecutableFile table
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            if (game.executableUpload != null)
            {
                string executableFileName = Path.GetFileName(game.executableUpload.FileName);
                int executableFileSize = game.executableUpload.ContentLength;
                int fileSize = executableFileSize / 1000;

                using (con)
                {
                    cmd = new SqlCommand("usp_AddNewExecutableFile", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@executableFileName", executableFileName);
                    cmd.Parameters.AddWithValue("@executableFileSize", fileSize);
                    cmd.Parameters.AddWithValue("@executableFilePath", "~/ExecutableFileUpload/" + executableFileName);
                    cmd.Parameters.AddWithValue("@gameID", game.Id);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
                con.Dispose();
            }
        }

        //Query to be used for Games by Year page
        public  Games getGamesByYear(int year)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            List<Game> gs = new List<Game>();
            con.Open();

            using (con)
            {
                cmd = new SqlCommand("usp_GetAllGamesByYear", con);
                cmd.Parameters.AddWithValue("@GameYear", year);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Game g = new Game(
                                    Convert.ToInt32(reader["GameId"]),
                                    reader["GameName"].ToString(),
                                    reader["GameDescription"].ToString(),
                                    reader["pictureFilePath"].ToString(),
                                    year
                               );
                    gs.Add(g);
                }
            }
            Games allGames = new Games(); 
            allGames.Items = gs;
            con.Close();
            return allGames;
        }

        //Query to be used for All Games page
        public Games getAllGames() 
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            List<Game> gs = new List<Game>();
            con.Open();

            using (con)
            {
                //cmd = new SqlCommand("usp_GetAllGames", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = "SELECT Games.GameId, GameName, GameDescription, pictureFilePath FROM Games INNER JOIN PictureFiles ON Games.GameId = PictureFiles.gameID";
                cmd.CommandText = "SELECT * FROM Games";
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Game g = new Game(
                                    Convert.ToInt32(reader["GameId"]),
                                    reader["GameName"].ToString(),
                                    reader["GameDescription"].ToString());
                    g.GamePictures = getPicturesByGameId(g.Id);

                    gs.Add(g);
                }
            }
            Games allGames = new Games(); 
            allGames.Items = gs;
            con.Close();

            return allGames;
        }

        public List<PictureFiles> getPicturesByGameId(int GameId)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            PictureFiles pictureFile = new PictureFiles();
            List<PictureFiles> pictureFiles = new List<PictureFiles>();

            //Getting pictures associatiated with game
            con.Open();
            cmd.CommandText = "SELECT * FROM PictureFiles WHERE gameID=@gameID";
            cmd.Parameters.AddWithValue("@gameID", GameId);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                pictureFile = new PictureFiles((int)reader["pictureID"],
                                                (string)reader["pictureFileName"],
                                                (string)reader["pictureFilePath"],
                                                GameId);

                pictureFiles.Add(pictureFile);
            }

            return pictureFiles;
        }

        //Command to delete all game data for one game
        public void deleteGame(int id, int groupId)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd = new SqlCommand("usp_DeleteGame", con);
            cmd.Parameters.AddWithValue("@GameId", id);
            cmd.Parameters.AddWithValue("@GroupId", groupId);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return;
        }

        //SQL Commands to update game details
        public Game updateGame(Game g)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();

            //update game table values
            cmd.CommandText = "UPDATE Games SET GameName=@GameName, Year=@Year, GameDescription=@GameDescription WHERE GameId=@GameId";
            cmd.Parameters.AddWithValue("@GameId", g.Id);
            cmd.Parameters.AddWithValue("@GameName", g.GameName);
            cmd.Parameters.AddWithValue("@Year", g.Year);
            cmd.Parameters.AddWithValue("@GameDescription", g.Description);
            cmd.ExecuteNonQuery();

            //update group name
            cmd.CommandText = "UPDATE PolyGamesGroups SET GroupName=@GroupName WHERE GroupId=@GroupId";
            cmd.Parameters.AddWithValue("@GroupId", g.GroupId);
            cmd.Parameters.AddWithValue("@GroupName", g.GroupName);
            cmd.ExecuteNonQuery();

            //update group member one
            cmd.CommandText = "UPDATE GroupMembers SET studentRole=@StudentRoleOne, studentName=@StudentNameOne WHERE MemberId=@oneId";
            cmd.Parameters.AddWithValue("@oneId", g.groupMembers[0].memberId);
            cmd.Parameters.AddWithValue("@StudentRoleOne", g.groupMembers[0].studentRole);
            cmd.Parameters.AddWithValue("@StudentNameOne", g.groupMembers[0].studentName);
            cmd.ExecuteNonQuery();

            //if there is a second group member, update group member two info
            if (g.groupMembers.Count >= 2)
            {
                cmd.CommandText = "UPDATE GroupMembers SET studentRole=@StudentRoleTwo, studentName=@StudentNameTwo WHERE MemberId=@twoId";
                cmd.Parameters.AddWithValue("@twoId", g.groupMembers[1].memberId);
                cmd.Parameters.AddWithValue("@StudentRoleTwo", g.groupMembers[1].studentRole);
                cmd.Parameters.AddWithValue("@StudentNameTwo", g.groupMembers[1].studentName);
                cmd.ExecuteNonQuery();
            }

            //if there is a third group member, update group member three info
            if (g.groupMembers.Count >= 3)
            {
                cmd.CommandText = "UPDATE GroupMembers SET studentRole=@StudentRoleThree, studentName=@StudentNameThree WHERE MemberId=@threeId";
                cmd.Parameters.AddWithValue("@threeId", g.groupMembers[2].memberId);
                cmd.Parameters.AddWithValue("@StudentRoleThree", g.groupMembers[2].studentRole);
                cmd.Parameters.AddWithValue("@StudentNameThree", g.groupMembers[2].studentName);
                cmd.ExecuteNonQuery();
            }

            //if there is a fourth group member, update group member four info
            if (g.groupMembers.Count >= 4)
            {
                cmd.CommandText = "UPDATE GroupMembers SET studentRole=@StudentRoleFour, studentName=@StudentNameFour WHERE MemberId=@fourId";
                cmd.Parameters.AddWithValue("@fourId", g.groupMembers[3].memberId);
                cmd.Parameters.AddWithValue("@StudentRoleFour", g.groupMembers[3].studentRole);
                cmd.Parameters.AddWithValue("@StudentNameFour", g.groupMembers[3].studentName);
                cmd.ExecuteNonQuery();
            }

            //if there is a fifth group member, update group member five info
            if (g.groupMembers.Count >= 5)
            {
                cmd.CommandText = "UPDATE GroupMembers SET studentRole=@StudentRoleFive, studentName=@StudentNameFive WHERE MemberId=@fiveId";
                cmd.Parameters.AddWithValue("@fiveId", g.groupMembers[4].memberId);
                cmd.Parameters.AddWithValue("@StudentRoleFive", g.groupMembers[4].studentRole);
                cmd.Parameters.AddWithValue("@StudentNameFive", g.groupMembers[4].studentName);
                cmd.ExecuteNonQuery();
            }

            //if there is a sixth group member, update group member six info
            if (g.groupMembers.Count >= 6)
            {
                cmd.CommandText = "UPDATE GroupMembers SET studentRole=@StudentRoleSix, studentName=@StudentNameSix WHERE MemberId=@sixId";
                cmd.Parameters.AddWithValue("@sixId", g.groupMembers[5].memberId);
                cmd.Parameters.AddWithValue("@StudentRoleSix", g.groupMembers[5].studentRole);
                cmd.Parameters.AddWithValue("@StudentNameSix", g.groupMembers[5].studentName);
                cmd.ExecuteNonQuery();
            }

            //update executable file
            if (g.executableUpload != null) //IF a new executable file was uploaded
            {
                //extract the file name and size from the upload
                string executableFileName = Path.GetFileName(g.executableUpload.FileName);
                int executableFileSize = g.executableUpload.ContentLength;
                int fileSize = executableFileSize / 1000;

                cmd.CommandText = "UPDATE ExecutableFiles SET executableFileName=@executableFileName, executableFileSize=@executableFileSize, " + 
                    "executableFilePath=@executableFilePath WHERE gameID=@GameId";
                cmd.Parameters.AddWithValue("@executableFileName", executableFileName);
                cmd.Parameters.AddWithValue("@executableFileSize", fileSize);
                cmd.Parameters.AddWithValue("@executableFilePath", "~/ExecutableFileUpload/" + executableFileName);
               // cmd.Parameters.AddWithValue("@GmId", g.Id);
                cmd.ExecuteNonQuery();
            }

            //update video file
            if (g.videoUpload != null) //IF a new video file was uploaded
            {
                //extract the file name and size from the upload
                string videoFileName = Path.GetFileName(g.videoUpload.FileName);
                int videoFileSize = g.videoUpload.ContentLength;
                int fileSize = videoFileSize / 1000;

                cmd.CommandText = "UPDATE VideoFiles SET videoFileName=@videoFileName, videoFileSize=@videoFileSize, " +
                    "videoFilePath=@videoFilePath WHERE gameID=@GameId";
                cmd.Parameters.AddWithValue("@videoFileName", videoFileName);
                cmd.Parameters.AddWithValue("@videoFileSize", fileSize);
                cmd.Parameters.AddWithValue("@videoFilePath", "~/VideoFileUpload/" + videoFileName);
                cmd.ExecuteNonQuery();

            }

            //update pictures - deletes the old ones and replaces them with the new pictures the user selected
            if (g.picturesUpload.Any())
            {
                //extract the file name and size from the upload
                string pictureFileName;
                int pictureFileSize = 0;
                int fileSize = 0;
                int picCounter = 0;

                foreach (var item in g.picturesUpload)
                {
                    if (item != null)
                    {
                        //delete existing pictures for this game
                        cmd.CommandText = "DELETE FROM PictureFiles where gameID=@GameId";
                        cmd.ExecuteNonQuery();
                        break;
                    }
                    break;
                }
                
                //loop through each new picture that user uploaded
                foreach (var item in g.picturesUpload)
                {
                    if (item != null)
                    {
                        pictureFileName = item.FileName;
                        pictureFileSize = item.ContentLength;
                        fileSize = pictureFileSize / 1000;
                        cmd = new SqlCommand("usp_AddNewPictureFile", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pictureFileName", pictureFileName);
                        cmd.Parameters.AddWithValue("@pictureFileSize", fileSize);
                        cmd.Parameters.AddWithValue("@pictureFilePath", "~/PictureFileUpload/" + pictureFileName);
                        cmd.Parameters.AddWithValue("@gameID", g.Id);
                        cmd.ExecuteNonQuery();
                        picCounter++;
                        if (picCounter == 5) break; //once we've added 5 pictures to the DB, stop
                    }
                }  
            }

            con.Close();

            return g;
        }

        public void UpdateAGame(Game game)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            //Updating Polygames Group table
            con.Open();
            cmd.CommandText = "UPDATE PolyGamesGroups SET GroupName=@GroupName WHERE GroupId=@GroupId";
            cmd.Parameters.AddWithValue("@GroupName", game.GroupName);
            cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();

            //Updating Games table
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "UPDATE Games SET GameName=@GameName, GameDescription=@GameDescription, Year=@Year WHERE GroupId=@GroupId";
            cmd.Parameters.AddWithValue("@GameName", game.GameName);
            cmd.Parameters.AddWithValue("@GameDescription", game.Description);
            cmd.Parameters.AddWithValue("@Year", game.Year);
            cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();

            //Update PictureFiles table
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            if (game.picturesUpload.Any())
            {
                //extract the file name and size from the upload
                string pictureFileName;
                int pictureFileSize = 0;
                int fileSize = 0;
                int picCounter = 0;

                foreach (var item in game.picturesUpload)
                {
                    if (item != null)
                    {
                        //delete existing pictures for this game
                        cmd.CommandText = "DELETE FROM PictureFiles where gameID=@GameId";
                        cmd.Parameters.AddWithValue("@GameId", game.Id);
                        cmd.ExecuteNonQuery();
                        break;
                    }
                    break;
                }

                //loop through each new picture that user uploaded
                foreach (var item in game.picturesUpload)
                {
                    if (item != null)
                    {
                        pictureFileName = item.FileName;
                        pictureFileSize = item.ContentLength;
                        fileSize = pictureFileSize / 1000;
                        cmd = new SqlCommand("usp_AddNewPictureFile", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pictureFileName", pictureFileName);
                        cmd.Parameters.AddWithValue("@pictureFileSize", fileSize);
                        cmd.Parameters.AddWithValue("@pictureFilePath", "~/PictureFileUpload/" + pictureFileName);
                        cmd.Parameters.AddWithValue("@gameID", game.Id);
                        cmd.ExecuteNonQuery();
                        picCounter++;
                        if (picCounter == 5) break; //once we've added 5 pictures to the DB, stop
                    }
                }
            }
            con.Close();
            con.Dispose();


            //Update VideoFiles table
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            if (game.videoUpload != null) //IF a new video file was uploaded
            {

                //extract the file name and size from the upload
                string videoFileName = Path.GetFileName(game.videoUpload.FileName);
                int videoFileSize = game.videoUpload.ContentLength;
                int fileSize = videoFileSize / 1000;

                cmd.CommandText = "UPDATE VideoFiles SET videoFileName=@videoFileName, videoFileSize=@videoFileSize, " +
                    "videoFilePath=@videoFilePath WHERE gameID=@GameId";
                cmd.Parameters.AddWithValue("@GameId", game.Id);
                cmd.Parameters.AddWithValue("@videoFileName", videoFileName);
                cmd.Parameters.AddWithValue("@videoFileSize", fileSize);
                cmd.Parameters.AddWithValue("@videoFilePath", "~/VideoFileUpload/" + videoFileName);
                cmd.ExecuteNonQuery();

            }
            con.Close();
            con.Dispose();


            //Update ExecutableFiles table
            con = new SqlConnection(conString);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            if (game.executableUpload != null) //IF a new executable file was uploaded
            {
                //extract the file name and size from the upload
                string executableFileName = Path.GetFileName(game.executableUpload.FileName);
                int executableFileSize = game.executableUpload.ContentLength;
                int fileSize = executableFileSize / 1000;

                cmd.CommandText = "UPDATE ExecutableFiles SET executableFileName=@executableFileName, executableFileSize=@executableFileSize, " +
                    "executableFilePath=@executableFilePath WHERE gameID=@GameId";
                cmd.Parameters.AddWithValue("@executableFileName", executableFileName);
                cmd.Parameters.AddWithValue("@executableFileSize", fileSize);
                cmd.Parameters.AddWithValue("@executableFilePath", "~/ExecutableFileUpload/" + executableFileName);
                cmd.Parameters.AddWithValue("@GameId", game.Id);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            con.Dispose();

            //Update UserLogin table
            for(int i = 0; i < game.groupMembers.Count; i++)
            {
                con = new SqlConnection(conString);
                cmd = new SqlCommand();
                cmd.Connection = con;
                con.Open();

                cmd.CommandText = "UPDATE UserLogin SET Name=@Name WHERE MemberId=@MemberId";
                cmd.Parameters.AddWithValue("@MemberId", game.groupMembers[i].memberId);
                cmd.Parameters.AddWithValue("@Name", game.groupMembers[i].studentName);
                cmd.ExecuteNonQuery();

                con.Close();
                con.Dispose();
            }


            //Update MemberToGroup table
            for (int i = 0; i < game.groupMembers.Count; i++)
            {
                con = new SqlConnection(conString);
                cmd = new SqlCommand();
                cmd.Connection = con;
                con.Open();
                cmd.CommandText = "UPDATE MemberToGroup SET StudentRole=@StudentRole WHERE MemberId=@MemberId AND GroupId=@GroupId";

                cmd.Parameters.AddWithValue("@MemberId", game.groupMembers[i].memberId);
                cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
                cmd.Parameters.AddWithValue("@StudentRole", game.groupMembers[i].studentRole);
                cmd.ExecuteNonQuery();

                con.Close();
                con.Dispose();
            }

        }

        public User getAllUsers()
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT MemberId, Name, Email, Password, IsActive, IsAdmin, RegistrationDate FROM UserLogin";
            List<User> au = new List<User>();
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                User nu = new User( (int)reader["MemberId"],
                                    reader["Name"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Password"].ToString(),
                                    (bool)reader["IsActive"],
                                    (bool)reader["IsAdmin"],
                                    (DateTime)reader["RegistrationDate"]);


                nu.RegistrationYear = nu.RegistrationDate.Value.Year;
                nu.CurrentTeamName = getGroupByMemberId(nu.MemberID);
                au.Add(nu);
                
            }
            User allUsers = new User();
            allUsers.Items = au;
            return allUsers;
        }

        public User getUserById(int id)
        {
            
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT MemberId, Name, Email, Password, IsActive, IsAdmin, RegistrationDate FROM UserLogin WHERE MemberId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            
                        User nu = new User((int)reader["MemberId"],
                                    reader["Name"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Password"].ToString(),
                                    (bool)reader["IsActive"],
                                    (bool)reader["IsAdmin"],
                                    (DateTime)reader["RegistrationDate"]);

                nu.CurrentTeamName = getGroupByMemberId(nu.MemberID);

            return nu;
        }


        public Groups getGroupByMemberId(int memberId)
        {
            List<Groups> ListGroup = new List<Groups>();
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            using (con)
            {
                cmd = new SqlCommand("usp_GetGroupNameByMemberId", con);
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Groups group = new Groups((int)reader["GroupId"],
                                            reader["GroupName"].ToString());

                    ListGroup.Add(group);
                }
                
            }
            con.Close();
            
            Groups Allgroups = new Groups();
            Allgroups.GroupList = ListGroup;

            return Allgroups;
        }

        public Groups getAllGroups()
        {
            List<Groups> ListGroup = new List<Groups>();
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM PolyGamesGroups";
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                   Groups group = new Groups((int)reader["GroupId"],
                                        reader["GroupName"].ToString());
                    ListGroup.Add(group);
                }
            con.Close();

            Groups groups = new Groups();
            groups.GroupList = ListGroup;

            return groups;
        }

        public void DeleteGroupByMemberId(int MemberId)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "DELETE FROM MemberToGroup WHERE MemberId=@MemberId";
            cmd.Parameters.AddWithValue("@MemberId", MemberId);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void DeleteUserByMemberId(int MemberId)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "DELETE FROM UserLogin WHERE MemberId=@MemberId";
            cmd.Parameters.AddWithValue("@MemberId", MemberId);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public User IsAdmin(String email, String password)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT Email, Password, IsAdmin, MemberId, IsActive, Name FROM UserLogin WHERE Password=@Password AND Email=@Email";
            cmd.Parameters.AddWithValue("Email", email);
            cmd.Parameters.AddWithValue("Password", password);
            List<User> au = new List<User>();
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {   // add memberid, name, isactive ???
                User nu = new User(reader["Email"].ToString(),
                                    reader["Password"].ToString(),
                                    reader["Name"].ToString(),
                                    (bool)reader["IsAdmin"],
                                    (int)reader["MemberId"],
                                    (bool)reader["IsActive"]);
                au.Add(nu);
                return nu;
            }
            User allUsers = new User();
            allUsers.Items = au;
            return allUsers;

        }

        public void addUser(User u)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "INSERT INTO UserLogin (Email, Password, Name, IsAdmin, IsActive, RegistrationDate) VALUES (@Email, @Password, @Name, @IsAdmin, @IsActive, @RegistrationDate)";
            //Boolean Active = true;
            //Boolean Admin = false;
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@Name", u.Name);
            cmd.Parameters.AddWithValue("@IsAdmin", u.IsAdmin); /// add a check box later and remove the hard code
            cmd.Parameters.AddWithValue("@IsActive", u.IsActive); /// add a check box later and remove the hard code
            cmd.Parameters.AddWithValue("@RegistrationDate", u.RegistrationDate);
            cmd.ExecuteNonQuery();
            con.Close();
            //addMemeberId(u);
        }

        public void addMemeberId(User u)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "SELECT MemberId FROM UserLogin WHERE Email=@Email AND Password=@Password";
            cmd.Parameters.AddWithValue("Email", u.Email);
            cmd.Parameters.AddWithValue("Password", u.Password);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            int MemberID = (int)reader["MemberId"];
            con.Close();
            con.Open();
            cmd.CommandText = "INSERT INTO MemberToGroup(MemberId, GroupId) VALUES(@MemberId, 1)";
            cmd.Parameters.AddWithValue("MemberId", MemberID);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        

        public void UpdateUser(User u)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "UPDATE UserLogin SET Email=@Email, Password=@Password, IsActive=@IsActive, IsAdmin=@IsAdmin, Name=@Name, RegistrationDate=@RegistrationDate WHERE MemberId = @MemberId";
            cmd.Parameters.AddWithValue("@MemberId", u.MemberID);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@IsActive", u.IsActive);
            cmd.Parameters.AddWithValue("@IsAdmin", u.IsAdmin);
            cmd.Parameters.AddWithValue("@Name", u.Name);
            cmd.Parameters.AddWithValue("@RegistrationDate", u.RegistrationDate);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        
        public void InsertMemberToGroup(MemberToGroup memberToGroup)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            if(memberToGroup.StudentRole != null)
            {
                cmd.CommandText = "INSERT INTO MemberToGroup(MemberId, GroupId, StudentRole, IsHidden) VALUES(@MemberId, @GroupId, @StudentRole, @ISHidden)";
                cmd.Parameters.AddWithValue("@MemberId", memberToGroup.MemberId);
                cmd.Parameters.AddWithValue("@GroupId", memberToGroup.GroupId);
                cmd.Parameters.AddWithValue("@StudentRole", memberToGroup.StudentRole);
                cmd.Parameters.AddWithValue("@ISHidden", false);
            }
            else
            {
                cmd.CommandText = "INSERT INTO MemberToGroup(MemberId, GroupId, IsHidden) VALUES(@MemberId, @GroupId, @ISHidden)";
                cmd.Parameters.AddWithValue("@MemberId", memberToGroup.MemberId);
                cmd.Parameters.AddWithValue("@GroupId", memberToGroup.GroupId);
                cmd.Parameters.AddWithValue("@ISHidden", false);

            }

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void updateMemberToGroup(int memberId, int newGroupId, int oldGroupId)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();

            cmd.CommandText = "UPDATE MemberToGroup SET GroupId = @NewGroupId, IsHidden = @ISHidden WHERE MemberId = @MemberId AND GroupId = @OldGroupId"; 
            cmd.Parameters.AddWithValue("@MemberID", memberId);
            cmd.Parameters.AddWithValue("@NewGroupID", newGroupId);
            cmd.Parameters.AddWithValue("@OldGroupID", oldGroupId);
            cmd.Parameters.AddWithValue("@ISHidden", false);

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void InsertMemberToGroup(MemberToGroup memberToGroup, User u)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            // First Getting member id
            con.Open();
            cmd.CommandText = "SELECT MemberId FROM UserLogin WHERE Email=@Email AND Password=@Password";
            cmd.Parameters.AddWithValue("Email", u.Email);
            cmd.Parameters.AddWithValue("Password", u.Password);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            memberToGroup.MemberId = (int)reader["MemberId"];
            con.Close();

            // Then adding it into MemberToGroup table to assign group to user
            con.Open();
            if (memberToGroup.StudentRole != null)
            {
                cmd.CommandText = "INSERT INTO MemberToGroup(MemberId, GroupId, StudentRole, IsHidden) VALUES(@MemberId, @GroupId, @StudentRole, @IsHidden)";
                cmd.Parameters.AddWithValue("@MemberId", memberToGroup.MemberId);
                cmd.Parameters.AddWithValue("@GroupId", memberToGroup.GroupId);
                cmd.Parameters.AddWithValue("@StudentRole", memberToGroup.StudentRole);
                cmd.Parameters.AddWithValue("@IsHidden", 0); // Default IsHidden property as False 
            }
            else
            {
                cmd.CommandText = "INSERT INTO MemberToGroup(MemberId, GroupId, IsHidden) VALUES(@MemberId, @GroupId, @IsHidden)";
                cmd.Parameters.AddWithValue("@MemberId", memberToGroup.MemberId);
                cmd.Parameters.AddWithValue("@GroupId", memberToGroup.GroupId);
                cmd.Parameters.AddWithValue("@IsHidden", 0); // Default IsHidden property as False
            }

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateIsHidden(ALLInOneModel model)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "UPDATE MemberToGroup SET IsHidden=@IsHidden WHERE MemberId = @MemberId AND GroupId = @GroupId";
            
            for(int i =0; i<model.games.Items.Count; i++)
            {
                cmd.Parameters.AddWithValue("@IsHidden", model.games.Items[i].IsHidden);
                cmd.Parameters.AddWithValue("@MemberId", model.user.MemberID);
                cmd.Parameters.AddWithValue("@GroupId", model.games.Items[i].GroupId);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            con.Close();
        }

        public MemberToGroup GetMemberToGroupIds()
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT MemberId, GroupId FROM MemberToGroup";
            List<MemberToGroup> au = new List<MemberToGroup>();
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                MemberToGroup nu = new MemberToGroup((int)reader["MemberId"],
                                    (int)reader["GroupId"]);
                au.Add(nu);
            }
            MemberToGroup allMemberToGroup = new MemberToGroup();
            allMemberToGroup.items = au;
            return allMemberToGroup;

        }


        public void addGroup(Groups group)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "INSERT INTO PolyGamesGroups(GroupName) VALUES(@GroupName)";
            cmd.Parameters.AddWithValue("@GroupName", group.GroupName);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public int setUserNameToEditMode(List<User> users, int id)
        {
            int editIndex = 0;

            foreach(User u in users)
            {
                if(u.MemberID == id)
                {
                    u.IsEditable = true;
                    return editIndex;
                }
                editIndex++;
            }
            return -1;
        }
        

        public List<int> getDistinctRegistrationYear()
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT DISTINCT YEAR(RegistrationDate)AS 'RegistrationYear' FROM UserLogin ";
            List<int> Years = new List<int>();
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Years.Add(((int)reader["RegistrationYear"]));
                
            }
            return Years;
        }

        public User getUserbyFilters(int RegistrationYear, int GroupId)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();

            List<User> au = new List<User>();

            if (GroupId == 0 && RegistrationYear != 0)
            {
                cmd.CommandText = "SELECT * FROM UserLogin WHERE YEAR(RegistrationDate)=@RegistrationDate";
                cmd.Parameters.AddWithValue("@RegistrationDate", RegistrationYear);
            }
            else if (GroupId != 0 && RegistrationYear == 0)
            {
                cmd.CommandText = "SELECT * FROM UserLogin INNER JOIN MemberToGroup ON UserLogin.MemberId = MemberToGroup.MemberId WHERE MemberToGroup.GroupId=@GroupId";
                cmd.Parameters.AddWithValue("@GroupId", GroupId);
            }
            else if (GroupId != 0 && RegistrationYear != 0)
            {
                cmd.CommandText = "SELECT * FROM UserLogin INNER JOIN MemberToGroup ON UserLogin.MemberId = MemberToGroup.MemberId WHERE MemberToGroup.GroupId=@GroupId AND YEAR(UserLogin.RegistrationDate)=@RegistrationDate";
                cmd.Parameters.AddWithValue("@GroupId", GroupId);
                cmd.Parameters.AddWithValue("@RegistrationDate", RegistrationYear);
            }
            
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                User nu = new User((int)reader["MemberId"],
                                    reader["Name"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Password"].ToString(),
                                    (bool)reader["IsActive"],
                                    (bool)reader["IsAdmin"],
                                    (DateTime)reader["RegistrationDate"]);


                nu.RegistrationYear = nu.RegistrationDate.Value.Year;
                nu.CurrentTeamName = getGroupByMemberId(nu.MemberID);
                au.Add(nu);

            }
            User allUsers = new User();
            allUsers.Items = au;
            return allUsers;


        }

        public Games getAllGamesList()
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            List<Game> gs = new List<Game>();
            con.Open();

            using (con)
            {

                cmd.CommandText = "SELECT Games.GameId ,PolyGamesGroups.GroupId, ExecutableFiles.executableId, PolyGamesGroups.GroupName, Games.GameName, Games.Year, ExecutableFiles.executableFileName, ExecutableFiles.executableFileSize FROM Games INNER JOIN PolyGamesGroups ON Games.GroupId = PolyGamesGroups.GroupId INNER JOIN ExecutableFiles ON Games.GameId = ExecutableFiles.gameID";
                //cmd.CommandText = "SELECT Games.GameId, GameName, GameDescription, pictureFilePath FROM Games INNER JOIN PictureFiles ON Games.GameId = PictureFiles.gameID";
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    Game g = new Game((int)reader["GameId"],
                                      (int)reader["GroupId"],
                                      (int)reader["executableId"],
                                      (String)reader["GroupName"],
                                      (String)reader["GameName"],
                                      (int)reader["Year"],
                                      (String)reader["executableFileName"],
                                      (int)reader["executableFileSize"]);

                    gs.Add(g);
                   
                }
            }
            Games allGames = new Games();
            allGames.Items = gs;
            con.Close();
            return allGames;
        }

        // test for returning a signle user
        public Games getUserDataById(int id)
        {
            List<Game> TeamList = new List<Game>();
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            //cmd.CommandText = "SELECT MemberId, Name, Email, Password, IsActive, IsAdmin FROM UserLogin WHERE MemberId=@id";
            con.Open();

            using (con)
            {
                cmd.CommandText = "SELECT MemberToGroup.GroupId, PolyGamesGroups.GroupName, Games.GameName, Games.GameId, MemberToGroup.IsHidden FROM MemberToGroup INNER JOIN PolyGamesGroups ON MemberToGroup.GroupId = PolyGamesGroups.GroupId INNER JOIN Games ON PolyGamesGroups.GroupId = Games.GroupId WHERE MemberToGroup.MemberId = @MemberID";
                cmd.Parameters.AddWithValue("@MemberID", id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    Game usersInfo = new Game((int)reader["GameId"],
                                    reader["GroupName"].ToString(),
                                    reader["GameName"].ToString(),
                                    (int)reader["GroupId"],
                                    (bool)reader["IsHidden"]);
                    TeamList.Add(usersInfo);
                }

            }
            con.Close();
            SqlConnection con2 = new SqlConnection(conString);
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = con2;
            con2.Open();
            using (con2)
            {
                cmd2.CommandText = "SELECT PolyGamesGroups.GroupName, PolyGamesGroups.GroupId, MemberToGroup.IsHidden FROM PolyGamesGroups INNER JOIN MemberToGroup ON MemberToGroup.GroupId = PolyGamesGroups.GroupId LEFT JOIN Games ON Games.GroupId = PolyGamesGroups.GroupId WHERE Games.GameName is null and MemberToGroup.MemberId = @MemberID";
                cmd2.Parameters.AddWithValue("@MemberID", id);
                SqlDataReader reader = cmd2.ExecuteReader();
                while (reader.Read())
                {
                    Game usersInfo = new Game(
                                    reader["GroupName"].ToString(),
                                    (int)reader["GroupId"],
                                    (bool)reader["IsHidden"]);
                    TeamList.Add(usersInfo);
                }
            }
            con2.Close();

            Games AllTeams = new Games();
            AllTeams.Items = TeamList;

            return AllTeams;
        }

        public Boolean isPasswordMatched(string oldPW)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();

            Boolean isMatched = false;
            cmd.CommandText = "SELECT Password FROM UserLogin WHERE Password = @oldPW";
            cmd.Parameters.AddWithValue("@oldPW", oldPW);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string oldPassword = reader["Password"].ToString();           
                if(oldPW.Equals(oldPassword))
                {
                    isMatched = true;
                }
            }
            con.Close();
            return isMatched;
        }

        public void updatePassword(string newPW, string oldPW)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();

            cmd.CommandText = "UPDATE UserLogin SET Password = @newPW WHERE Password = @oldPW";
            cmd.Parameters.AddWithValue("@oldPw", oldPW);
            cmd.Parameters.AddWithValue("@newPW", newPW);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public bool IsMemberToGroupExist(int MemberId, int GroupId)
        {
            MemberToGroup memberToGroup = new MemberToGroup();
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "SELECT * FROM MemberToGroup WHERE MemberId=@MemberId AND GroupId=@GroupId";
            cmd.Parameters.AddWithValue("@MemberId", MemberId);
            cmd.Parameters.AddWithValue("@GroupId", GroupId);

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                memberToGroup = new MemberToGroup((int)reader["MemberId"],
                                                  (int)reader["GroupId"]);

                if(memberToGroup.MemberId == MemberId && memberToGroup.GroupId == GroupId)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteMemberToGroupByIds(int MemberId, int GroupId)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();

            cmd.CommandText = "DELETE FROM MemberToGroup WHERE MemberId=@MemberId AND GroupId=@GroupId";
            cmd.Parameters.AddWithValue("@MemberId", MemberId);
            cmd.Parameters.AddWithValue("@GroupId", GroupId);

            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}