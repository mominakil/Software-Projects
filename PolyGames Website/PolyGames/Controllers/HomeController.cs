using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyGames.Models;
using PolyGames.DataAccessObjects;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;


namespace PolyGames.Controllers
{
    //Controls all of the websites actions
    public class HomeController : Controller
    {
        //Home page
        public ActionResult Index()
        {
            //Create Database Access Object in order to access the SQL query
            GameDAO dAO = new GameDAO();
            Games g = dAO.getGamesOrderedByMostRecentlyAdded();

            //Returns the list of games to the Index/Home Page view (Index.cshtml)
            return View(g);
        }

        public ActionResult AddGame(Game g, string Save, int? GroupId)
        {
            if (GroupId != 0)
            {
                int groupId = GroupId ?? default(int);
                g.GroupId = groupId;
            }

            //If user is logged in
            if (Session["Email"] != null)
            {
                //Save is the name of the Value on the Add a Game page Button
                if (Save == "Save")
                {
                    //check for uploaded pictures and save them to the appropriate folder
                    if (g.picturesUpload != null)
                    {
                        string pictureFileName;
                        int picCounter = 0;

                        //loop through the object that holds all of the pictures the user uploaded
                        foreach (var item in g.picturesUpload)
                        {
                            //Extract file name and save the picture to the PictureFileUploaded folder in the website Home directory
                            pictureFileName = item.FileName;
                            item.SaveAs(Server.MapPath("~/PictureFileUpload/" + pictureFileName));
                            picCounter++;
                            //Only want to allow 5 pictures max to be saved to the DB so stop adding pictures once you hit 5, even if user selected more
                            if (picCounter == 5) break;
                        }
                    }

                    //check for uploaded video and save it to the appropriate folder
                    if (g.videoUpload != null)
                    {
                        //Extract file name and save the video to the VideoFileUploaded folder in the website Home directory
                        string videoFileName = Path.GetFileName(g.videoUpload.FileName);
                        g.videoUpload.SaveAs(Server.MapPath("~/VideoFileUpload/" + videoFileName));
                    }

                    //check for uploaded executable file and save it to the appropriate folder
                    if (g.executableUpload != null)
                    {
                        //Extract file name and save the executable to the ExecutableFileUploaded folder in the website Home directory
                        string executableFileName = Path.GetFileName(g.executableUpload.FileName);
                        g.executableUpload.SaveAs(Server.MapPath("~/ExecutableFileUpload/" + executableFileName));
                    }

                    //Create Database Access Object in order to access the addGame method which adds the game to the database
                    GameDAO dAO = new GameDAO();
                    //dAO.addGame(g);
                    dAO.AddAGame(g);

                    ViewBag.Message = "  Game Uploaded Successfully!";
                }
            }
            //If user is not logged in, redirect them to the login page
            else
            {
                return RedirectToAction("Login");
            }

            //Clear the form fields after form has been submitted
            ModelState.Clear();

            return View("AddGame", g);
        }

        //Main Game page where you can view and edit game details
        public ActionResult Game(Game gm, string Save)
        {

            GameDAO dAO = new GameDAO();

            //this means the user is saving edits to the game
            if (Save == "Save")
            {
                //check if they uploaded a new executable file. If they did then save it to the appropriate folder
                if (gm.executableUpload != null)
                {
                    string executableFileName = Path.GetFileName(gm.executableUpload.FileName);
                    gm.executableUpload.SaveAs(Server.MapPath("~/ExecutableFileUpload/" + executableFileName));
                }

                //check if they uploaded a new video file. If they did then save it to the appropriate folder
                if (gm.videoUpload != null)
                {
                    string videoFileName = Path.GetFileName(gm.videoUpload.FileName);
                    gm.videoUpload.SaveAs(Server.MapPath("~/VideoFileUpload/" + videoFileName));
                }

                //check if they uploaded any new pictures. If they did then save them to the appropriate folder
                if (gm.picturesUpload.Any())
                {
                    string pictureFileName;
                    int picCounter = 0;

                    foreach (var item in gm.picturesUpload)
                    {
                        if (item != null)
                        {
                            pictureFileName = item.FileName;
                            item.SaveAs(Server.MapPath("~/PictureFileUpload/" + pictureFileName));
                            picCounter++;
                            if (picCounter == 5) break;
                        }
                    }
                }

                //call the UpdateAGame method in the GameDAO which saves the edited game details to the database
                dAO.UpdateAGame(gm);

                ViewBag.Message = "Data successfully updated";
            }

            //get the updated game values that you just saved to the database so that you can re-display them
            Game g = dAO.getGameById(gm.Id);

            //set is editable to false so that the Game page shows the correct view
            g.IsEditable = false;

            //re-display the updated game values
            return View(g);
        }

        //AllGames.cshtml page action
        public ActionResult AllGames()
        {
            //Create Database Access Object in order to access the SQL query that returns a list of all the games
            GameDAO dao = new GameDAO();
            Games g = dao.getAllGames();

            //Returns the list of games to the AllGames page
            return View(g);
        }

        //GamesByYear.cshtml page action
        public ActionResult GamesByYear(int year)
        {
            //Create Database Access Object in order to access the SQL query that returns a list of all the games in a certain year
            GameDAO dao = new GameDAO();
            Games g = dao.getGamesByYear(year);

            //Returns the list of games to the GamesByYear page
            return View(g);
        }

        //Game.cshtml page - action when user clicks the delete link
        public ActionResult GameDelete(int id, int groupId)
        {
            //If user is logged in
            if (Session["Email"] != null)
            {
                GameDAO dAO = new GameDAO();
                dAO.deleteGame(id, groupId);

                //Since you've just deleted the game for the page you're on, you are redirected to the Home/Index page
                Games g = dAO.getGamesOrderedByMostRecentlyAdded();
                return View("Index", g);
            }
            else
            {
                //If you're not logged in, you're taken to the login page
                return RedirectToAction("Login");
            }
        }

        //Game.cshtml page - action when user clicks the edit link
        public ActionResult GameEdit(int id)
        {
            if (Session["Email"] != null)
            {
                //Getting the game details
                GameDAO dAO = new GameDAO();
                Game g = dAO.getGameById(id);

                //Setting the game attribute to editable so that the editable view is shown on the Game.cshtml
                g.IsEditable = true;

                //Returning to the Game.cshtml page passing the original game details but as the editable view
                return View("Game", g);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //Game.cshtml page - action when user clicks the download link
        public ActionResult DownloadGame(string path)
        {
            //Saves the executable file to the users Download directory
            return File(path, "application", Path.GetFileName(path));
        }

        public ActionResult Login(User u, string Login, String PLI)
        {
            GameDAO dao = new GameDAO();
            if (Session["Email"] != null && Session["Password"] != null)
            {
                if ((bool)Session["IsAdmin"] == true)
                {
                    //Returning allUsersModel to populate table
                    ALLInOneModel users = new ALLInOneModel();
                    users = getAllUser();
                    users.games = dao.getAllGamesList();
                    return View("AdminPage", users);
                }
                else if ((bool)Session["IsAdmin"] == false)
                {
                    //Returning allUsersModel to populate table
                    ALLInOneModel user = new ALLInOneModel();
                    //users = getAllUser();

                    user = getUserDataById((int)Session["MemberId"]);

                    return View("UserPage", user);
                }

            }

            if (Login == "Login")
            {
                u = dao.IsAdmin(u.Email, u.Password);
                if (u.Email != null || u.Password != null) // needs to check database to validate login...
                {
                    //GameDAO dao = new GameDAO();
                    //dao.Login(u);
                    //ViewBag.Message = "Logged In";

                    //u = dao.IsAdmin(u.Email, u.Password);
                    if (u.IsAdmin == true)
                    {
                        Session["Email"] = u.Email;
                        Session["Name"] = u.Name;
                        Session["Password"] = u.Password;
                        Session["IsAdmin"] = u.IsAdmin;
                        Session["IsActive"] = u.IsActive;
                        Session["MemberId"] = u.MemberID;

                        //Returning allUsersModel to populate table
                        ALLInOneModel users = new ALLInOneModel();
                        users = getAllUser();
                        users.games = dao.getAllGamesList();
                        return View("AdminPage", users);
                    }
                    else
                    {
                        if (u.IsActive == true)
                        {
                            Session["Email"] = u.Email;
                            Session["Name"] = u.Name;
                            Session["Password"] = u.Password;
                            Session["IsAdmin"] = u.IsAdmin;
                            Session["IsActive"] = u.IsActive;
                            Session["MemberId"] = u.MemberID;
                            //Returning allUsersModel to populate table
                            ALLInOneModel user = new ALLInOneModel();
                            //users = getAllUser();

                            user = getUserDataById(u.MemberID);

                            return View("UserPage", user);
                        }
                        else
                        {
                            ViewBag.LogOutMessage = "Your Account is inactive, please contact your instructor";
                            return View("Login");
                        }

                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Email or Password";
                    return View();
                }

            }
            if (PLI == "PLI")
            {
                ViewBag.Message("Please log in to add a game");
            }

            return View("Login");

        }

        public ActionResult LogOut()
        {
            Session["Email"] = null;
            Session["Name"] = null;
            Session["Password"] = null;
            Session["IsAdmin"] = null;
            Session["MemberId"] = null;
            Session["IsActive"] = null;

            ViewBag.LogOutMessage = "Successfully Logged Out";

            return View("Login");
        }

        public ActionResult AdminPage()
        {
            GameDAO dao = new GameDAO();
            if (Session["Email"] != null && Session["Password"] != null)
            {
                //Returning allUsersModel to populate table
                ALLInOneModel users = new ALLInOneModel();
                users = getAllUser();
                users.games = dao.getAllGamesList();
                return View("AdminPage", users);
            }
            return View("Login");
        }

        public ActionResult UserPage()
        {
            if (Session["Email"] != null && Session["Password"] != null)
            {
                if ((bool)Session["IsActive"] == true)
                {
                    if ((bool)Session["IsAdmin"] == false)
                    {
                        //Returning allUsersModel to populate table
                        ALLInOneModel user = new ALLInOneModel();
                        //users = getAllUser();
                        user = getUserDataById((int)Session["MemberId"]);
                        return View("UserPage", user);
                    }
                }
                else
                {
                    ViewBag.LogOutMessage = "Your Account is inactive, please contact your instructor";
                    return View("Login");
                }

            }
            return View("Login");
        }

        //allYears.cshtml action
        public ActionResult allYears()
        {
            //Gets and returns a list of distinct years
            GameDAO dao = new GameDAO();
            Games y = dao.getAllYears();
            return View(y);
        }

        public ActionResult addUser(ALLInOneModel users, String Save)
        {
            //ModelState will clear all textboxes on All a new Student after any student has been added.
            ModelState.Clear();

            if (Save == "Save")
            {
                if (users.user.Email != null && users.user.Name != null && users.user.Password != null)
                {
                    GameDAO dao = new GameDAO();
                    dao.addUser(users.user);

                    MemberToGroup mToG = new MemberToGroup();
                    mToG.GroupId = users.groups.GroupId;
                    dao.InsertMemberToGroup(mToG, users.user);

                    ViewBag.Message = "Student Added Succesfully!";
                    return View("AdminPage", getAllUser());
                }
                else
                {
                    ViewBag.Message = "Invalid Email, Password, Name(Cannot have numerics)";
                    return View("AdminPage", getAllUser());
                }
            }
            return View("AdminPage", getAllUser());
        }

        public ActionResult addGroup(ALLInOneModel group, String Create)
        {
            if (Create == "Create")
            {
                GameDAO dao = new GameDAO();
                dao.addGroup(group.groups);
                ViewBag.MessageAddGroup = "Group Created";
                return View("AdminPage", getAllUser());
            }
            else
            {
                ViewBag.MessageAddGroup = "Failed to create the Group";
                return View("AdminPage", getAllUser());
            }
        }



        public ActionResult UserAccountEdit(int? id, ALLInOneModel users)
        {
            int id2 = id ?? default(int);
            GameDAO dao = new GameDAO();
            //users.user = getAllUser().user;

            int registrationYear = 0;
            int groupId = 0;



            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                //Extracting sessions
                registrationYear = (int)Session["RegistrationYear"];
                groupId = (int)Session["GroupId"];
            }


            users = FilteredResult(registrationYear, groupId);

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                users.filters = new Filters();
                users.filters.RegistrationYear = registrationYear;
                users.filters.GroupId = groupId;
            }

            users.user.EditIndex = dao.setUserNameToEditMode(users.user.Items, id2);

            if (id2 != 0)
            {
                Session["MemberIdEdit"] = id2;
                Session["UserAccountEdit"] = users;
            }

            //ViewBag.Message
            return View("AdminPage", users);
        }

        // FOR HIDDEN A USER FROM THE PUBLIC ON THEIR GAME PAGE
        //public ActionResult HideUserName(int? id, ALLInOneModel users)
        //{
        //    int id2 = id ?? default(int);
        //    GameDAO dao = new GameDAO();

        //    users.user.EditIndex = dao.setUserHidden(users.user, id2);
        //}

        public ActionResult DeleteUserAccount(int? id, ALLInOneModel users)
        {

            int id2 = id ?? default(int);
            GameDAO dao = new GameDAO();
            dao.DeleteGroupByMemberId(id2);
            dao.DeleteUserByMemberId(id2);

            users = getAllUser();

            return View("AdminPage", users);

        }


        public ActionResult getAllUsers(ALLInOneModel model, String Save)
        {
            GameDAO dao = new GameDAO();
            if (Save == "Save")
            {
                User user = model.user.Items[model.user.EditIndex];

                //Member to Group
                //MemberToGroup mToG = new MemberToGroup();
                //mToG.GroupId = model.groups.GroupId;
                //mToG.MemberId = user.MemberID;

                //MemberToGroup memberToGroup = dao.GetMemberToGroupIds();
                //Boolean isIdentical = false;

                //for (int i = 0; i < memberToGroup.items.Count; i++)
                //{
                //    if (memberToGroup.items[i].MemberId == mToG.MemberId && memberToGroup.items[i].GroupId == mToG.GroupId)
                //    {
                //        isIdentical = true;
                //    }
                //}

                //if (isIdentical == false)
                //{
                //    dao.InsertMemberToGroup(mToG);
                //}

                dao.UpdateUser(user);

                model.user.IsEditable = false;
                model.user.EditIndex = -1;
            }

            ALLInOneModel FilteredModel = new ALLInOneModel();


            Session["RegistrationYear"] = model.filters.RegistrationYear;
            Session["GroupId"] = model.filters.GroupId;
            

            int registrationYear = 0;
            int groupId = 0;

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                //Extracting sessions
                registrationYear = (int)Session["RegistrationYear"];
                groupId = (int)Session["GroupId"];
            }

            FilteredModel = FilteredResult(registrationYear, groupId);

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                FilteredModel.filters = new Filters();
                FilteredModel.filters.RegistrationYear = (int)Session["RegistrationYear"];
                FilteredModel.filters.GroupId = (int)Session["GroupId"];
            }

            return View("AdminPage", FilteredModel);

        }

        public ActionResult saveGamesData(ALLInOneModel model, String Save)
        {
            GameDAO dao = new GameDAO();

            if (Save == "Save")
            {
                if (Session["MemberId"] != null)
                {
                    model.user.MemberID = (int)Session["MemberId"];
                    dao.UpdateIsHidden(model);
                    ViewBag.SaveMessage = "Saved Successfully";
                }
                else
                {
                    ViewBag.SaveMessage = "Failed to Save";
                }
            }

            //Returning allUsersModel to populate table
            model = getUserDataById(model.user.MemberID);

            return View("UserPage", model);
        }


        public ALLInOneModel FilteredResult(int RegistrationYear, int GroupId)
        {
            ALLInOneModel FilteredModel = new ALLInOneModel();
            GameDAO dao = new GameDAO();

            if (GroupId == 0 && RegistrationYear == 0)
            {
                FilteredModel = getAllUser();

            }
            else
            {
                FilteredModel.user = dao.getUserbyFilters(RegistrationYear, GroupId);
                FilteredModel = getAllViewBags(FilteredModel);
            }

            return FilteredModel;
        }


        public ALLInOneModel getAllUser()
        {
            GameDAO dao = new GameDAO();
            ALLInOneModel UsersModel = new ALLInOneModel();
            UsersModel.user = dao.getAllUsers();

            UsersModel = getAllViewBags(UsersModel);

            return UsersModel;

        }

        public ALLInOneModel getAllViewBags(ALLInOneModel UsersModel)
        {
            GameDAO dao = new GameDAO();
            Groups groups = dao.getAllGroups();
            UsersModel.groups = new Groups();
            UsersModel.groups.GroupList = groups.GroupList;

            List<int> RegistrationYears = dao.getDistinctRegistrationYear();

            //For Filter By Registration Year on Admin Page
            ViewBag.registrationYears = RegistrationYears;

            //For Group dropdownlist on AdminPage
            ViewBag.Grouplist = groups.GroupList;

            //For Games Information Table
            UsersModel.games = dao.getAllGamesList();

            return UsersModel;
        }

        public ActionResult getAllGames(ALLInOneModel model, String Save)
        {
            GameDAO dao = new GameDAO();

            ALLInOneModel FilteredModel = new ALLInOneModel();
            FilteredModel = FilteredResult((int)Session["RegistrationYear"], (int)Session["GroupId"]);
            FilteredModel.games = dao.getAllGamesList();

            return View("AdminPage", FilteredModel);

        }

        // might not be using this 
        public ALLInOneModel getUserDataById(int id)
        {
            GameDAO dao = new GameDAO();
            ALLInOneModel UsersModel = new ALLInOneModel();
            //UsersModel.user = getAllUser().user;
            UsersModel.games = dao.getUserDataById(id);

            return UsersModel;
        }

        public ActionResult updatePassword(string Submit, ALLInOneModel model)
        {
            if (Submit == "Submit")
            {
                GameDAO dao = new GameDAO();
                Boolean isMatched = dao.isPasswordMatched(model.user.Password);
                if (isMatched == true && model.passwordCheck[0] == model.passwordCheck[1])
                {
                    dao.updatePassword(model.passwordCheck[0], model.user.Password);
                    ViewBag.PasswordUpdated = "Password succesfully changed";
                }
                else
                {
                    ViewBag.PasswordUpdated = "Password Did Not Matched!";
                }
            }
            ALLInOneModel user = new ALLInOneModel();
            if (Session["MemberId"] != null)
            {
                user = getUserDataById((int)Session["MemberId"]);
            }

            return View("UserPage", user);

        }

        //Updating MemberToGroup based on Admin page dropdownlist under User Information
        public ActionResult UpdateTeam(int? id, int? id2)
        {
            GameDAO dao = new GameDAO();
            int NewGroupId = id ?? default(int);
            int OldGroupId = id2 ?? default(int);
            int MemberId = 0;
            bool IsMemberToGroupExist = false;
            int EditIndex = 0;
            ALLInOneModel model = new ALLInOneModel();
            int registrationYear = 0;
            int groupId =  0;

            //Extracting Sessions for ALLInOneModel and MemberId
            if (Session["MemberIdEdit"] != null && Session["UserAccountEdit"] != null)
            {
                MemberId = (int)Session["MemberIdEdit"];
                model = (ALLInOneModel)Session["UserAccountEdit"];

                //Update Group
                // model.user.Items[model.user.EditIndex].CurrentTeamName = dao.getGroupByMemberId(MemberId);
                EditIndex = model.user.EditIndex;
            }

            //Checking if the MemberId and GroupId exist or not.
            if (MemberId != 0)
            {
                IsMemberToGroupExist = dao.IsMemberToGroupExist(MemberId, OldGroupId);
            }

            if (IsMemberToGroupExist == true)
            {
                dao.updateMemberToGroup(MemberId, NewGroupId, OldGroupId);
            }


            //Retriving all Data for Model

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                //Extracting sessions
                registrationYear = (int)Session["RegistrationYear"];
                groupId = (int)Session["GroupId"];
            }


            model = FilteredResult(registrationYear, groupId);

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                model.filters = new Filters();
                model.filters.RegistrationYear = registrationYear;
                model.filters.GroupId = groupId;
            }

            model.user.EditIndex = dao.setUserNameToEditMode(model.user.Items, MemberId);
            ///////////////////////////


            //Return Data
            return View("AdminPage", model);
        }

        //Assign New Team into MemberToGroup based on Admin page dropdownlist under User Information
        public ActionResult AssignNewTeam(int? id)
        {
            GameDAO dao = new GameDAO();
            int NewGroupId = id ?? default(int);
            int MemberId = 0;
            bool IsMemberToGroupExist = false;
            ALLInOneModel model = new ALLInOneModel();
            int registrationYear = 0;
            int groupId = 0;



            if (Session["MemberIdEdit"] != null && Session["UserAccountEdit"] != null)
            {
                MemberId = (int)Session["MemberIdEdit"];
                model = (ALLInOneModel)Session["UserAccountEdit"];

                //Update Group
                model.user.Items[model.user.EditIndex].CurrentTeamName = dao.getGroupByMemberId(MemberId);
            }

            IsMemberToGroupExist = dao.IsMemberToGroupExist(MemberId, NewGroupId);

            if(IsMemberToGroupExist == false && NewGroupId !=0)
            {
                MemberToGroup memberToGroup = new MemberToGroup(MemberId, NewGroupId);
                dao.InsertMemberToGroup(memberToGroup);
            }

            //Return Data
            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                //Extracting sessions
                registrationYear = (int)Session["RegistrationYear"];
                groupId = (int)Session["GroupId"];
            }


            model = FilteredResult(registrationYear, groupId);

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                model.filters = new Filters();
                model.filters.RegistrationYear = registrationYear;
                model.filters.GroupId = groupId;
            }

            model.user.EditIndex = dao.setUserNameToEditMode(model.user.Items, MemberId);


            return View("AdminPage", model);
        }

        public ActionResult DeleteAssignedGroup(int? MemberId, int? GroupId)
        {
            GameDAO dao = new GameDAO();
            int memberId = MemberId ?? default(int);
            int groupId = GroupId ?? default(int);
            ALLInOneModel model = new ALLInOneModel();
            int registrationYear = 0;
            int filterGroupId = 0;
            int MemberIdEdit = 0;
            //

            if (MemberId != 0 && GroupId != 0)
            {
                dao.DeleteMemberToGroupByIds(memberId, groupId);
            }

            //Retrieve ALLInOneModel Data

            if (Session["MemberIdEdit"] != null && Session["UserAccountEdit"] != null)
            {
                MemberIdEdit = (int)Session["MemberIdEdit"];
                model = (ALLInOneModel)Session["UserAccountEdit"];

                //Update Group
                model.user.Items[model.user.EditIndex].CurrentTeamName = dao.getGroupByMemberId(MemberIdEdit);
            }

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                //Extracting sessions
                registrationYear = (int)Session["RegistrationYear"];
                filterGroupId = (int)Session["GroupId"];
            }


            model = FilteredResult(registrationYear, filterGroupId);

            if (Session["RegistrationYear"] != null && Session["GroupId"] != null)
            {
                model.filters = new Filters();
                model.filters.RegistrationYear = registrationYear;
                model.filters.GroupId = filterGroupId;
            }

            model.user.EditIndex = dao.setUserNameToEditMode(model.user.Items, MemberIdEdit);


            return View("AdminPage", model);
        }

    }
}