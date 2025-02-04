using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using asp.netwebmvcWebsite.Models;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web.UI;
using ZXing;
using Razorpay.Api;


namespace asp.netwebmvcWebsite.Controllers
{
      //[EnableCors("AllowSpecificOrigins")]
    public class HomeController : Controller
    {
        // private readonly Logger<HomeController> _logger;
        private readonly string _connectionString;

        public HomeController(String connectionString) { _connectionString = connectionString; }

        public HomeController() { }

        string connectionString = ConfigurationManager.ConnectionStrings["Sowmyadb"].ConnectionString;

        [HttpGet]
        public ActionResult Index() { return View(); }

        [HttpPost]
        public ActionResult logoutbtn()
        {
            Session.Clear();   // Removes all keys and values from the session
            Session.Abandon();       // Terminates the session
            return  RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Privacy() { return View(); }

        [HttpGet]
        public ActionResult WelcomePage() { return View(); }

        [HttpGet]
        public ActionResult Registration() { return View(); }


        [HttpPost]
        public ActionResult Registration(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {  // Check if the email already exists
                        string checkQuery = "SELECT COUNT(*) FROM sowmyaMatrimony_temp WHERE RegisteredEmailId = @RegEmail";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@RegEmail", model.RegisteredEmailId);
                        conn.Open();
                        int emailExists = (int)checkCmd.ExecuteScalar();
                        conn.Close();
                        if (emailExists > 0)
                        {
                            ModelState.AddModelError("", "The email address is already registered."); // Email already exists
                            return View(model);
                        }

                        string query = @"INSERT INTO sowmyaMatrimony(RegisteredEmailId,Password,ConformPassword, RegisteredDateTime) 
                        VALUES (@RegEmail,@Password,@ConformPassword, @RegDateTime)";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@RegEmail", model.RegisteredEmailId);
                        cmd.Parameters.AddWithValue("@Password", model.Password);
                        cmd.Parameters.AddWithValue("@ConformPassword", model.ConformPassword);
                        cmd.Parameters.AddWithValue("@RegDateTime", DateTime.Now);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                    }
                    return RedirectToAction("LoginPage");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the data." + ex);
                    return View();
                }
            }
            return View(model); //return the same view for errors
        }

        [HttpGet]
        public ActionResult LoginPage(string Emailtxt, string Password, LoginModel model)
        {
            /// Emailtxt. = "";
            // Password =  Passwordtxt.text = "";
            model.RegisteredEmailId = "";
            model.Password = "";
            ModelState.Clear();
                   
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(LoginModel model) {
            string connectionString = ConfigurationManager.ConnectionStrings["Sowmyadb"].ConnectionString;
            try {
                if (ModelState.IsValid) {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = @"SELECT  RegisteredEmailId,Password FROM sowmyaMatrimony WHERE RegisteredEmailId = @Email AND Password = @Password";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Email", model.RegisteredEmailId);
                        command.Parameters.AddWithValue("@Password", model.Password);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {    // Store email and password in session
                            Session["email"] = reader["RegisteredEmailId"].ToString();
                            Session["password"] = reader["Password"].ToString();
                            ViewData["Success"] = "Login Credentials are Valid";
                            return RedirectToAction("Payment");
                        }
                        else { ModelState.AddModelError("", "Invalid email or password."); return View(model); }
                    }
                }
                if (!ModelState.IsValid){
                    // Capture errors
                    var errors = ModelState.Values
                                         .SelectMany(v => v.Errors)
                                         .Select(e => e.ErrorMessage)
                                         .ToList();
                    foreach (var error in errors) { ModelState.AddModelError("", error); }
                    return View(model);
                }
                return View(model);
            }
            catch (Exception ex) { ModelState.AddModelError("", "An error occurred while saving the data." + ex);    return View(); }
        }

        [HttpGet]
        public ActionResult Header() { return View(); }  
        
           
          [HttpGet]
        public ActionResult Payment(string payment_id,string Email , string Phone,string Name)
        {
            string regmail = (string)Session["Email"];
            if (string.IsNullOrEmpty(regmail)) {
                string errorMessage = "Session has expired  You Didnt Login. Please Login";
                TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                return RedirectToAction("ErrorPage");
            }
            if (string.IsNullOrEmpty(payment_id))
            {
                TempData["BridePaymentStatusss"] = "Failure";  
                TempData["BridePaymentId"] = null;
                TempData["err"] = "PaymentId is null.";
                
                using (SqlConnection con = new SqlConnection(connectionString))  {
                    con.Open();
                    string query = "SELECT RegisteredEmailId,Name,Gender,BrideAmount,BrideStatus,PaymentId,PaymentDateTime  FROM SowmyaMatrimony " +
                    "where Gender = 'Female' and RegisteredEmailId = '" + regmail + "' and BrideStatus='success' and BrideAmount='500'  ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {  ViewBag.Brideview = "ToviewBride";  }
                    con.Close();
                }
           
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT RegisteredEmailId, Name, gender, BroomAmount, BroomStatus, PaymentId, PaymentDateTime "+
                   "FROM SowmyaMatrimony where Gender = 'Male' and RegisteredEmailId = '" + regmail + "' and BroomStatus='success' and BroomAmount='500'   ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) { ViewBag.Broomview = "ToviewBroom"; }
                    con.Close();
                }
          

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = " SELECT RegisteredEmailId, Name, gender, ProfileAmount, ProfileAmountStatus, AddProfilePaymentId, AddProfilePaymentDateTime "+
                    "FROM SowmyaMatrimony where RegisteredEmailId = '"+ regmail + "'  and ProfileAmountStatus='success' and ProfileAmount='1000'  ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) { ViewBag.AddProfileview = "ToviewAddProfile"; }
                    con.Close();
                }
                return View();
            }
            else if(!string.IsNullOrEmpty(payment_id))
            {
                ViewBag.bride = "Viewbride"; ViewBag.broom = "Viewbroom"; ViewBag.addDetails = "addDetails";
                 return View();
            }   
            return View();
        }

        [HttpPost]
        public ActionResult Payment(PaymentModel model, string payment_id)   //, string orderId, string signature)
        {
            ViewBag.Post = "<script>alert('Post operation');</script>";
            // Your Razorpay API Key and Secret
            var key = ConfigurationManager.AppSettings["RazorPayKey"].ToString();
                var secret = ConfigurationManager.AppSettings["RazorPaySecret"].ToString();
                RazorpayClient cilent = new RazorpayClient(key, secret);

            // Verify payment signature
            var options = new Dictionary<string, object> { { "razorpay_payment_id", payment_id }   };  // , { "razorpay_order_id", orderId },//  { "razorpay_signature", signature }

             try
             {
                if (TempData["BridePaymentStatusss"] == "Failure" || TempData["BridePaymentId"] == null 
                    || TempData["BroomPaymentStatusss"] == "Failure" || TempData["BroomPaymentId"] == null
                    || TempData["AddDetailsPaymentStatus"] == "Failure" || TempData["AddDetailsPaymentId"] == null)
                {
                    TempData["BridePaymentStatusss"] = "Failure";
                    TempData["err"] = "PaymentId is null.";
                    return RedirectToAction("Payment");     // return BadRequest("Invalid payment ID");
                }
                else
                {
                    if (TempData["BridePaymentStatusss"] == "Success" && TempData["GenderFemale"] == "Female")
                    {
                        TempData.Keep("BridePaymentId");
                        string regmail = (string)Session["Email"];

                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            var queryy = "UPDATE SowmyaMatrimony SET PaymentId = @PaymentId, BrideStatus = @BrideStatus, BrideAmount = @BrideAmount," +
                                " PaymentDateTime = @PaymentDateTime " + "WHERE RegisteredEmailId = @RegisteredEmailId AND Gender = @Gender";

                            using (SqlCommand cmd = new SqlCommand(queryy, con))
                            {
                                cmd.Parameters.AddWithValue("@PaymentId", TempData["BridePaymentId"]);    //.ToString());
                                cmd.Parameters.AddWithValue("@BrideStatus", "Success");
                                cmd.Parameters.AddWithValue("@BrideAmount", 500); // Assuming BrideAmount is numeric
                                cmd.Parameters.AddWithValue("@PaymentDateTime", DateTime.Now);
                                cmd.Parameters.AddWithValue("@RegisteredEmailId", regmail);
                                cmd.Parameters.AddWithValue("@Gender", "Female");

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                       // TempData["BridePayment"] = "Paid";
                        return RedirectToAction("Payment");
                    }
                   
                }
             }
             catch (Exception ex)
             {
                    TempData["BridePaymentStatusss"] = "Failure";
                    @ViewBag.err = "Error is  " + ex;
                    return RedirectToAction("Payment");
             }
            
            return RedirectToAction("Payment");
        }

      
        public ActionResult Failure() { ViewBag.Message = "Payment failed. Please try again.";  return View(); }

      
        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session values   
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult BridePage(PhotoViewModel model ,string payment_id)
        {
            string regmail = (string)Session["Email"];
            if (string.IsNullOrEmpty(regmail))
            {
                string errorMessage = "Session has expired  You Didnt Login. Please Login";
                TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                return RedirectToAction("ErrorPage");
            }

            if (!string.IsNullOrEmpty(payment_id))
            {
                TempData["GenderFemale"] = "Female"; TempData["BridePaymentStatusss"] = "Success";
                TempData["BridePaymentId"] = payment_id;
                TempData.Keep("BridePaymentId"); TempData.Keep("BridePaymentStatusss"); TempData.Keep("GenderFemale");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    var queryy = "UPDATE SowmyaMatrimony SET PaymentId = @PaymentId, BrideStatus = @BrideStatus, BrideAmount = @BrideAmount," +
                        " PaymentDateTime = @PaymentDateTime " + "WHERE RegisteredEmailId = @RegisteredEmailId AND Gender = @Gender";

                    using (SqlCommand cmd = new SqlCommand(queryy, con))
                    {
                        cmd.Parameters.AddWithValue("@PaymentId", TempData["BridePaymentId"]);    //.ToString());
                        cmd.Parameters.AddWithValue("@BrideStatus", "Success");
                        cmd.Parameters.AddWithValue("@BrideAmount", 500); // Assuming BrideAmount is numeric
                        cmd.Parameters.AddWithValue("@PaymentDateTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RegisteredEmailId", regmail);
                        cmd.Parameters.AddWithValue("@Gender", "Female");

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    TempData["BridePaymentSuccess"] = " Payment Success with Payment Id= " + payment_id + " to View Bride and Payment valid for 2 Months.";
                    return RedirectToAction("payment");
                }
                        //return RedirectToAction("Payment");              
               // return RedirectToAction("Payment");
            }
            List<PhotoViewModel> photo = new List<PhotoViewModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT Name, MainImage1,MainImage1FilePath FROM SowmyaMatrimony where Gender = 'Female' ";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    photo.Add(new PhotoViewModel
                    {
                        Name = reader["Name"].ToString(),
                        MainImage1FilePath = reader["MainImage1FilePath"].ToString()
                    });
                }
            }
            return View(photo);
        }
    
        [HttpGet]
        public ActionResult ErrorPage()
        {
            ViewBag.Error =TempData["ErrorMessage"];
            return View();
        }

       [HttpGet]
        public ActionResult PersonalProfiles(PhotoViewModel model)
        {
            string regmail = (string)Session["Email"];

            if (string.IsNullOrEmpty(regmail))
            {
                string errorMessage = "Session has expired  You Didnt Login. Please Login";
                TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                return RedirectToAction("ErrorPage");
            }

            List<PhotoViewModel> photo = new List<PhotoViewModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT RegisteredEmailId, Name, MainImage1FilePath FROM SowmyaMatrimony WHERE RegisteredEmailId = @RegisteredEmailId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@RegisteredEmailId", regmail);
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows) // No rows found for the given email
                {
                     ModelState.AddModelError("", "No profile exists for the provided email.");
                    return View();
                }

                while (reader.Read())
                {
                    photo.Add(new PhotoViewModel
                    {
                        RegisteredEmailId = reader["RegisteredEmailId"].ToString(),
                        Name = reader["Name"].ToString(),
                        MainImage1FilePath = reader["MainImage1FilePath"].ToString()
                    });
                }
                con.Close();
            }
            return View(photo);
        }

        [HttpGet]
        public ActionResult BroomPage(string payment_id)
        {
            string regmail = (string)Session["Email"];
            if (string.IsNullOrEmpty(regmail))
            {
                string errorMessage = "Session has expired  You Didnt Login. Please Login";
                TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                return RedirectToAction("ErrorPage");
            }

            if (!string.IsNullOrEmpty(payment_id))
            {
                TempData["BroomPaymentId"] = payment_id; TempData["GenderMale"] = "Male";
                TempData["BroomPaymentStatus"] = "Success"; 
                TempData.Keep("BroomPaymentId"); TempData.Keep("BroomPaymentStatusss"); TempData.Keep("GenderMale");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    var queryy = "UPDATE SowmyaMatrimony SET PaymentId = @PaymentId, BroomStatus = @BroomStatus, BroomAmount = @BroomAmount," +
                        " PaymentDateTime = @PaymentDateTime " + "WHERE RegisteredEmailId = @RegisteredEmailId AND Gender = @Gender";

                    using (SqlCommand cmd = new SqlCommand(queryy, con))
                    {
                        cmd.Parameters.AddWithValue("@PaymentId", TempData["BroomPaymentId"]);    //.ToString());
                        cmd.Parameters.AddWithValue("@BroomStatus", "Success");
                        cmd.Parameters.AddWithValue("@BroomAmount", 500); // Assuming BrideAmount is numeric
                        cmd.Parameters.AddWithValue("@PaymentDateTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RegisteredEmailId", regmail);
                        cmd.Parameters.AddWithValue("@Gender", "Male");

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    TempData["BroomPaymentSuccess"] = "Payment Success with Payment Id= " + payment_id + " To View Broom and Payment valid for 2 Months.";
                    return RedirectToAction("payment");
                }
               
               
            }

            List<PhotoViewModel> photo = new List<PhotoViewModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT Name, MainImage1,MainImage1FilePath FROM SowmyaMatrimony where Gender = 'Male' ";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    photo.Add(new PhotoViewModel
                    {
                        Name = reader["Name"].ToString(),
                        MainImage1FilePath = reader["MainImage1FilePath"].ToString()
                    });
                }
            }
            return View(photo);
        }

        [HttpGet]
        public ActionResult DetailView(string Name, DetailedView modell)
        {
            try
            {
                DetailedView model = null;
                //List<DetailedView>   model = new List<DetailedView>();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT Name ,Surname ,Age,[Gender] ,[DOB],[TOB],[PlaceOfBirth],[CandiatePlace],[Raasi],[Star],[Occupation] " +
                         ",[CandiateContact] ,[MotherName] ,[MotherOccupation] ,[MotherPlace],[MotherContact],[FatherName],[FatherOccupation]," +
                         "[FatherContact] ,[FatherPlace] ,[CandiateEmailID],[Saakha] ,[MainImage1FilePath] ,[Image2FilePath],[Image3FilePath]," +
                         "[MainImage1],[Image2] ,[Image3] FROM [SowmyaMatrimony] where Name= '" + Name + "' ";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TimeSpan tob = reader["TOB"] != DBNull.Value ? (TimeSpan)reader["TOB"] : TimeSpan.Zero;

                        model = new DetailedView
                        {
                            Name = reader["Name"].ToString(),
                            Surname = reader["Surname"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Age = reader["Age"] != DBNull.Value ? reader["Age"].ToString() : null,
                            DOB = (DateTime)(reader["DOB"] != DBNull.Value ? (DateTime?)reader["DOB"] : null),
                            TOB = tob,

                            // Output the formatted value
                            // Console.WriteLine("Time of Birth: " + tob.ToString(@"hh\:mm"));

                            PlaceOfBirth = reader["PlaceOfBirth"]?.ToString(),
                            CandiateContact = reader["CandiateContact"]?.ToString(),
                            CandiateEmailID = reader["CandiateEmailID"]?.ToString(),
                            CandiatePlace = reader["CandiatePlace"]?.ToString(),
                            Raasi = reader["Raasi"]?.ToString(),
                            Star = reader["Star"]?.ToString(),
                            Occupation = reader["Occupation"]?.ToString(),
                            MotherName = reader["MotherName"]?.ToString(),
                            MotherContact = reader["MotherContact"]?.ToString(),
                            MotherOccupation = reader["MotherOccupation"]?.ToString(),
                            MotherPlace = reader["MotherPlace"]?.ToString(),
                            FatherName = reader["FatherName"]?.ToString(),
                            FatherOccupation = reader["FatherOccupation"]?.ToString(),
                            FatherContact = reader["FatherContact"]?.ToString(),
                            FatherPlace = reader["FatherPlace"]?.ToString(),
                            Saakha = reader["Saakha"]?.ToString(),
                            MainImage1FilePath = reader["MainImage1FilePath"]?.ToString(),
                            Image2FilePath = reader["Image2FilePath"]?.ToString(),
                            Image3FilePath = reader["Image3FilePath"]?.ToString(),
                        };

                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ModelState.AddModelError("", "An error occurred while saving the data." + ex + "   and " + error);
                return View();
            }
            // return View(modell);
        }

        [HttpGet]
        public ActionResult Delete(string Name, DetailedView model)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string deleteQuery = "DELETE FROM SowmyaMatrimony WHERE Name = @Name and RegisteredEmailId=@Email";
                    using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@Name", Name);
                        deleteCommand.Parameters.AddWithValue("@Email", Session["Email"]);
                        connection.Open();
                        deleteCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                string folderPath = Server.MapPath("~/images/" + model.Name);
                if (Directory.Exists(folderPath)) { Directory.Delete(folderPath, true); }
                // Set success message in TempData
                TempData["SuccessMessage"] = "The record has been deleted successfully!";
                return RedirectToAction("PersonalProfiles");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("PersonalProfiles");
        }

        [HttpGet]
        public ActionResult forgot()  { return View();  }


        [HttpPost]
        public ActionResult Forgot(ForgotModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                  string query = "update SowmyaMatrimony set  Password='"+model.Password+"',ConformPassword='"+model.ConformPassword+"' where RegisteredEmailId = '"+model.RegisteredEmailId+"' ";
                  SqlCommand cmd = new SqlCommand(query, con);
                  con.Open();
                  cmd.ExecuteNonQuery();
                  con.Close();
                  TempData["PasswordChangedSuccess"] = "Password Changed Successfully";
                }
                return RedirectToAction("LoginPage");
            }
            else
            { 
                ViewBag.Notupdated = "Password not Updated Check Errors";
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult UpdateProfile(string Name)
        {
          // try
          //{
                string regmail = (string)Session["Email"];
                if (string.IsNullOrEmpty(regmail))
                {
                    string errorMessage = "Session has expired  You Didnt Login. Please Login";
                    TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                    return RedirectToAction("ErrorPage");
                }
                Updateprofile model = null;
                //List<DetailedView>   model = new List<DetailedView>();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT RegisteredEmailId, Name ,Surname ,Age,[Gender] ,[DOB],[TOB],[PlaceOfBirth],[CandiatePlace],[Raasi],[Star],[Occupation] " +
                         ",[CandiateContact] ,[MotherName] ,[MotherOccupation] ,[MotherPlace],[MotherContact],[FatherName],[FatherOccupation]," +
                         "[FatherContact] ,[FatherPlace] ,[CandiateEmailID],[Saakha] ,[MainImage1FilePath] ,[Image2FilePath],[Image3FilePath]," +
                         "[MainImage1],[Image2] ,[Image3] FROM [SowmyaMatrimony] where Name= '" + Name + "' and RegisteredEmailId= '"+regmail+"' ";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TimeSpan tob = reader["TOB"] != DBNull.Value ? (TimeSpan)reader["TOB"] : TimeSpan.Zero;

                        model = new Updateprofile
                        {
                            RegisteredEmailId = regmail,
                            Name = reader["Name"].ToString(),
                            Surname = reader["Surname"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Age = reader["Age"] != DBNull.Value ? reader["Age"].ToString() : null,
                            DOB = (DateTime)(reader["DOB"] != DBNull.Value ? (DateTime?)reader["DOB"] : null),
                            TOB = tob,

                            // Output the formatted value
                            // Console.WriteLine("Time of Birth: " + tob.ToString(@"hh\:mm"));
                            PlaceOfBirth = reader["PlaceOfBirth"]?.ToString(),
                            CandiateContact = reader["CandiateContact"]?.ToString(),
                            CandiateEmailID = reader["CandiateEmailID"]?.ToString(),
                            CandiatePlace = reader["CandiatePlace"]?.ToString(),
                            Raasi = reader["Raasi"]?.ToString(),
                            Star = reader["Star"]?.ToString(),
                            Occupation = reader["Occupation"]?.ToString(),
                            MotherName = reader["MotherName"]?.ToString(),
                            MotherContact = reader["MotherContact"]?.ToString(),
                            MotherOccupation = reader["MotherOccupation"]?.ToString(),
                            MotherPlace = reader["MotherPlace"]?.ToString(),
                            FatherName = reader["FatherName"]?.ToString(),
                            FatherOccupation = reader["FatherOccupation"]?.ToString(),
                            FatherContact = reader["FatherContact"]?.ToString(),
                            FatherPlace = reader["FatherPlace"]?.ToString(),
                            Saakha = reader["Saakha"]?.ToString(),
                            MainImage1FilePath = reader["MainImage1FilePath"].ToString(),
                            Image2FilePath = reader["Image2FilePath"].ToString(),
                            Image3FilePath = reader["Image3FilePath"].ToString()
                        };
                    }
                }
                return View(model);
            //}
            //catch (Exception ex)
            //{
            //    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //    ModelState.AddModelError("", "An error occurred while saving the data." + ex + "   and " + error);
            //    return View();
            //}
        }

        [HttpPost] 
        public ActionResult UpdateProfile(Updateprofile model, HttpPostedFileBase MainImage1FilePath, HttpPostedFileBase Image2FilePath, HttpPostedFileBase Image3FilePath)    
        {
            if (!ModelState.IsValid) { return View(model); } //return the same view for error
            try
            {
                 string connectionString = ConfigurationManager.ConnectionStrings["Sowmyadb"].ConnectionString;
                 string regmail = (string)Session["Email"];

                    if (string.IsNullOrEmpty(regmail))
                    {
                        string errorMessage = "Session has expired  You Didnt Login. Please Login";
                        TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                        return RedirectToAction("ErrorPage");
                    }

                    if (MainImage1FilePath == null || MainImage1FilePath.ContentLength == 0)
                    { ModelState.AddModelError("", "File 1 not uploaded ,upload image file"); }
                    else if (MainImage1FilePath != null && MainImage1FilePath.ContentLength > 0)
                    {
                       string filename1 = Path.GetFileName(MainImage1FilePath.FileName);
                       string fileName1 = Path.GetFileName(MainImage1FilePath.FileName);
                       string filepath = Path.GetExtension(MainImage1FilePath.FileName);
                    var baseFolder = Server.MapPath("~/Images");
                    var folderName = model.Name;
                    var subFolder = Path.Combine(baseFolder, folderName);


                        // Create the directory if it doesn't exist
                        if (!Directory.Exists(subFolder)) { Directory.CreateDirectory(subFolder); }

                        if (filepath != ".jpg" && filepath != ".png" && filepath != ".jpeg")
                        { ViewBag.Message = "only .jpeg , .jpg ,.png files are allowed"; }
                        else
                        {
                          //  var Model1Filepath = model.MainImage1FilePath;
                            string fullfilepath = Path.Combine(subFolder, fileName1);
                            fullfilepath = GenerateUniqueFilePath(fullfilepath);
                            MainImage1FilePath.SaveAs(fullfilepath);
                            ViewBag.Message1 = "file 1 image  successfully saved to folder ";

                            string relativePath1 =  $"~/Images/{folderName}/{fileName1}";
                           model.MainImage1FilePath = relativePath1;
                            ViewBag.filepath1 = model.MainImage1FilePath;

                            model.MainImage1 = System.IO.File.ReadAllBytes(fullfilepath);

                            // Convert byte array to Base64 string
                            string base64Image = Convert.ToBase64String(model.MainImage1);
                            ViewBag.Image1 = $"data:image/jpeg;base64,{base64Image}";

                            //ViewBag.Image1 = model.MainImage1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(Request.Form["ExistingMainImage1"]))
                    { model.MainImage1 = Convert.FromBase64String(Request.Form["ExistingMainImage1"]); }


                if (Image2FilePath == null || Image2FilePath.ContentLength == 0)
                { ModelState.AddModelError("", "File 2 not uploaded ,upload image file"); }
                else if (Image2FilePath != null && Image2FilePath.ContentLength > 0)
                {
                    string filename2 = Path.GetFileName(Image2FilePath.FileName);
                    string fileName2 = Path.GetFileName(Image2FilePath.FileName);
                    string filepath = Path.GetExtension(Image2FilePath.FileName);

                    var baseFolder = Server.MapPath("~/Images");
                    var folderName = model.Name;
                    var subFolder = Path.Combine(baseFolder, folderName);

                   // var subFolder = Server.MapPath($"~/Images/{model.Name}");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(subFolder)) { Directory.CreateDirectory(subFolder); }

                    if (filepath != ".jpg" && filepath != ".png" &&  filepath != ".jpeg")
                    { ViewBag.Message = "only .jpeg , .jpg ,.png files are allowed"; }
                    else
                    {
                        //var Image2FilePath = model.Image2FilePath;
                        string fullfilepath2 = Path.Combine(subFolder, fileName2);
                        fullfilepath2 = GenerateUniqueFilePath(fullfilepath2);
                        Image2FilePath.SaveAs(fullfilepath2);
                        ViewBag.Message2 = "file 2 image  successfully saved to folder";

                        string relativePath2 = $"~/Images/{folderName}//{fileName2}";
                        model.Image2FilePath = relativePath2;
                        ViewBag.filepath2 = model.Image2FilePath;

                        model.Image2 = System.IO.File.ReadAllBytes(fullfilepath2);

                        //using (var binaryReader = new BinaryReader(Image2FilePath.InputStream))
                        //{ model.Image2 = binaryReader.ReadBytes(Image2FilePath.ContentLength); }
                        // TempData["Image2"] = pp2;

                        // Convert byte array to Base64 string
                        string base64Image = Convert.ToBase64String(model.Image2);
                        ViewBag.Image2 = $"data:image/jpeg;base64,{base64Image}";

                        // ViewBag.Image2= model.Image2;
                        // Resolve the relative path to an absolute path
                        //  string fullPath = Server.MapPath(relativePath2);
                    }
                }
                else if (!string.IsNullOrEmpty(Request.Form["ExistingMainImage2"]))
                { model.Image2 = Convert.FromBase64String(Request.Form["ExistingMainImage2"]); }




                if (Image3FilePath == null || Image3FilePath.ContentLength == 0)
                { ModelState.AddModelError("", "File 3 not uploaded ,upload image file"); }
                else if (Image3FilePath != null && Image3FilePath.ContentLength > 0)
                {
                   
                    // var baseFolder = Server.MapPath("~/Images"); var folderName = model.Name; var subFolder = Path.Combine(baseFolder, folderName);
                   string filename3 = Path.GetFileName(Image3FilePath.FileName);
                   string fileName3 = Path.GetFileName(Image3FilePath.FileName);
                   string filepath = Path.GetExtension(fileName3);

                    var baseFolder = Server.MapPath("~/Images");
                    var folderName = model.Name;
                    var subFolder = Path.Combine(baseFolder, folderName);
                    // var Image3 = model.Image3FilePath;

                   // var subFolder = Server.MapPath($"~/Images/{model.Name}");
                    if (!Directory.Exists(subFolder)) { Directory.CreateDirectory(subFolder); }

                    if (filepath != ".jpg" && filepath != ".png" && filepath != ".jpeg")
                    { ViewBag.Message = "only .jpeg , .jpg ,.png files are allowed"; }
                    else
                    {
                        string fullfilepath3 = Path.Combine(subFolder, fileName3);
                        fullfilepath3 = GenerateUniqueFilePath(fullfilepath3);
                        Image3FilePath.SaveAs(fullfilepath3);
                        ViewBag.Message3 = "file 3 image  successfully saved to folder";

                        string relativePath3 = $"~/Images/{folderName}/{fileName3}";
                        model.Image3FilePath = relativePath3;
                        ViewBag.filepath3 = model.Image3FilePath;
                        model.Image3 = System.IO.File.ReadAllBytes(fullfilepath3);

                        // Convert byte array to Base64 string
                        string base64Image = Convert.ToBase64String(model.Image3);
                        ViewBag.Image3 = $"data:image/jpeg;base64,{base64Image}";
                    }
                }
                else if (!string.IsNullOrEmpty(Request.Form["ExistingMainImage3"]))
                { model.Image3 = Convert.FromBase64String(Request.Form["ExistingMainImage3"]); }

                  using (SqlConnection conn = new SqlConnection(connectionString))
                  {
                        string updateQuery = @" UPDATE SowmyaMatrimony SET  [Name] = @Name,  [Surname] = @Surname, [Age] = @Age, [Gender] = @Gender, 
                              [DOB] = @DOB, [TOB] = @TOB, [PlaceOfBirth] = @PlaceOfBirth, [CandiatePlace] = @CandiatePlace,  [Raasi] = @Raasi, [Star] = @Star, 
                               [Occupation] = @Occupation,[CandiateContact] = @CandiateContact, [MotherName] = @MotherName, [MotherOccupation] = @MotherOccupation, 
                              [MotherPlace] = @MotherPlace, [MotherContact] = @MotherContact, [FatherName] = @FatherName, [FatherOccupation] = @FatherOccupation, 
                              [FatherContact] = @FatherContact, [FatherPlace] = @FatherPlace, [MainImage1] = @MainImage1, [Image2] = @Image2, [Image3] = @Image3, 
                              [ProfileAddDateTime] = @ProfileAddDateTime,[CandiateEmailID] = @CandiateEmailID, [Saakha] = @Saakha ,[MainImage1FilePath]=@MainImage1FilePath ,
                              [Image2FilePath]=@Image2FilePath ,[Image3FilePath]=@Image3FilePath  WHERE  [RegisteredEmailId] = @RegisteredEmailId  and Name= @Name ";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@RegisteredEmailId", regmail);
                            cmd.Parameters.AddWithValue("@Name", model.Name);
                            cmd.Parameters.AddWithValue("@Surname", model.Surname);
                            cmd.Parameters.AddWithValue("@Age", model.Age);
                            cmd.Parameters.AddWithValue("@Gender", model.Gender);
                            cmd.Parameters.AddWithValue("@DOB", model.DOB);
                            cmd.Parameters.AddWithValue("@TOB", model.TOB);
                            cmd.Parameters.AddWithValue("@PlaceOfBirth", model.PlaceOfBirth);
                            cmd.Parameters.AddWithValue("@CandiatePlace", model.CandiatePlace);
                            cmd.Parameters.AddWithValue("@Raasi", model.Raasi);
                            cmd.Parameters.AddWithValue("@Star", model.Star);
                            cmd.Parameters.AddWithValue("@Occupation", model.Occupation);
                            cmd.Parameters.AddWithValue("@CandiateContact", model.CandiateContact);
                            cmd.Parameters.AddWithValue("@MotherName", model.MotherName);
                            cmd.Parameters.AddWithValue("@MotherOccupation", model.MotherOccupation);
                            cmd.Parameters.AddWithValue("@MotherPlace", model.MotherPlace);
                            cmd.Parameters.AddWithValue("@MotherContact", model.MotherContact);
                            cmd.Parameters.AddWithValue("@FatherName", model.FatherName);
                            cmd.Parameters.AddWithValue("@FatherOccupation", model.FatherOccupation);
                            cmd.Parameters.AddWithValue("@FatherContact", model.FatherContact);
                            cmd.Parameters.AddWithValue("@FatherPlace", model.FatherPlace);
                            cmd.Parameters.AddWithValue("@MainImage1", @ViewBag.Image1);
                            cmd.Parameters.AddWithValue("@Image2", @ViewBag.Image2);
                            cmd.Parameters.AddWithValue("@Image3", @ViewBag.Image3);
                            cmd.Parameters.AddWithValue("@CandiateEmailID", model.CandiateEmailID);
                            cmd.Parameters.AddWithValue("@Saakha", model.Saakha);
                            cmd.Parameters.AddWithValue("@ProfileAddDateTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@MainImage1FilePath", model.MainImage1FilePath);
                            cmd.Parameters.AddWithValue("@Image2FilePath", model.Image2FilePath);
                            cmd.Parameters.AddWithValue("@Image3FilePath", model.Image3FilePath);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }


                  }
                    ViewBag.Failure = "Profile  updated Successfully";
                    TempData["Success"] = "Profile updated Successfully";
                    return RedirectToAction("Payment");
               
              }
            catch (Exception ex)
              {
                ViewBag.Failure = "Profile Not Updated";
                var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ModelState.AddModelError("", "An error occurred while saving the data." + ex + "   and " + error);
                return View();
              }
            
        }


        [HttpGet]
        public ActionResult AddDetailsPage( string payment_id)  // AddDetails model)
        {
            var model = new sowmyaMatrimony
            {
                MainImage1FilePath = string.Empty,
                Image2FilePath = string.Empty,
                Image3FilePath = string.Empty,
            };

            string regmail = (string)Session["Email"];
            if (string.IsNullOrEmpty(regmail))
            {
                string errorMessage = "Session has expired  You Didnt Login. Please Login";
                TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                return RedirectToAction("ErrorPage");
            }

            if (!string.IsNullOrEmpty(payment_id))
            {
                TempData["GenderAddDetails"] = "DetailsAdded";
                TempData["AddDetailsPaymentStatus"] = "Success"; TempData["AddDetailsPaymentId"] = payment_id;
                TempData.Keep("AddDetailsPaymentId"); TempData.Keep("AddDetailsPaymentStatus"); TempData.Keep("GenderAddDetails");
        
               // string regmail = (string)Session["Email"];
               using (SqlConnection con = new SqlConnection(connectionString))
               {
                 var queryy = "UPDATE SowmyaMatrimony SET AddProfilePaymentId = @AddProfilePaymentId, ProfileAmountStatus = @ProfileAmountStatus, ProfileAmount = @ProfileAmount," +
                 " AddProfilePaymentDateTime = @AddProfilePaymentDateTime " + "WHERE RegisteredEmailId = @RegisteredEmailId ";

                        using (SqlCommand cmd = new SqlCommand(queryy, con))
                        {
                            cmd.Parameters.AddWithValue("@AddProfilePaymentId", TempData["AddDetailsPaymentId"]);
                            cmd.Parameters.AddWithValue("@ProfileAmountStatus", "Success");
                            cmd.Parameters.AddWithValue("@ProfileAmount", 1000); // Assuming BrideAmount is numeric
                            cmd.Parameters.AddWithValue("@AddProfilePaymentDateTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@RegisteredEmailId", regmail);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    TempData["AddDetailsPaymentSuccess"] = " Payment Success with Payment Id= " + payment_id + "  to Add Details and Payment valid for 2 Months.";
                    return RedirectToAction("payment");
                }
                
            }
            return View(model);
        }


        [HttpPost]
        public ActionResult AddDetailsPage(sowmyaMatrimony model, HttpPostedFileBase MainImage1FilePath, HttpPostedFileBase Image2FilePath, HttpPostedFileBase Image3FilePath)     //, Image MainImage1, IFormFile Image2, IFormFile Image3)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["Sowmyadb"].ConnectionString;
                    string regmail = (string)Session["Email"];

                    if (string.IsNullOrEmpty(regmail))
                    {
                        string errorMessage = "Session has expired  You Didnt Login. Please Login";
                        TempData["ErrorMessage"] = errorMessage; // Store the message in ViewBag
                        return RedirectToAction("ErrorPage");
                    }

                    if (MainImage1FilePath == null || MainImage1FilePath.ContentLength == 0)
                    { ModelState.AddModelError("", "File 1 not uploaded ,upload image file"); }
                    else if (MainImage1FilePath != null && MainImage1FilePath.ContentLength > 0)
                    {
                        string filename1 = Path.GetFileName(MainImage1FilePath.FileName);
                        var baseFolder = Server.MapPath("~/Images");
                        var folderName = model.Name;
                        var subFolder = Path.Combine(baseFolder, folderName);
                        string fileName1 = Path.GetFileName(MainImage1FilePath.FileName);
                        string filepath = Path.GetExtension(MainImage1FilePath.FileName);

                        // Create the directory if it doesn't exist
                        if (!Directory.Exists(subFolder)) { Directory.CreateDirectory(subFolder); }

                        if (filepath != ".jpg" && filepath != ".png" && filepath != ".jpeg")
                        { ViewBag.Message = "only .jpeg , .jpg ,.png files are allowed"; }
                        else
                        {
                            string fullfilepath = Path.Combine(subFolder, fileName1);
                            fullfilepath = GenerateUniqueFilePath(fullfilepath);
                            MainImage1FilePath.SaveAs(fullfilepath);
                            ViewBag.Message1 = "file 1 image  successfully saved to folder ";

                            string relativePath = $"~/Images/{folderName}/{fileName1}";
                            model.MainImage1FilePath = relativePath;
                            ViewBag.filepath1 = model.MainImage1FilePath;

                            model.MainImage1 = System.IO.File.ReadAllBytes(fullfilepath);

                            // Convert byte array to Base64 string
                            string base64Image = Convert.ToBase64String(model.MainImage1);
                            ViewBag.Image1 = $"data:image/jpeg;base64,{base64Image}";

                            //ViewBag.Image1 = model.MainImage1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(Request.Form["ExistingMainImage1"]))
                    { model.MainImage1 = Convert.FromBase64String(Request.Form["ExistingMainImage1"]); }


                    if (Image2FilePath == null || Image2FilePath.ContentLength == 0)
                    { ModelState.AddModelError("", "File 2 not uploaded ,upload image file"); }
                    else if (Image2FilePath != null && Image2FilePath.ContentLength > 0)
                    {
                        string filename2 = Path.GetFileName(Image2FilePath.FileName);
                        var baseFolder = Server.MapPath("~/Images");
                        var folderName = model.Name;
                        var subFolder = Path.Combine(baseFolder, folderName);
                        string fileName2 = Path.GetFileName(Image2FilePath.FileName);
                        string filepath = Path.GetExtension(Image2FilePath.FileName);

                        if (!Directory.Exists(subFolder)) { Directory.CreateDirectory(subFolder); }

                        if (filepath != ".jpg" && filepath != ".png" && filepath != ".jpeg")
                        { ViewBag.Message = "only .jpeg , .jpg ,.png files are allowed"; }
                        else
                        {

                            string fullfilepath2 = Path.Combine(subFolder, fileName2);
                            fullfilepath2 = GenerateUniqueFilePath(fullfilepath2);
                            Image2FilePath.SaveAs(fullfilepath2);
                            ViewBag.Message2 = "file 2 image  successfully saved to folder";

                            string relativePath = $"~/Images/{folderName}/{fileName2}";
                            model.Image2FilePath = relativePath;
                            ViewBag.filepath2 = model.Image2FilePath;
                            var pp2 = System.IO.File.ReadAllBytes(fullfilepath2);

                            using (var binaryReader = new BinaryReader(Image2FilePath.InputStream))
                            { model.Image2 = binaryReader.ReadBytes(Image2FilePath.ContentLength); }


                            TempData["Image2"] = pp2;

                            // Convert byte array to Base64 string
                            string base64Image = Convert.ToBase64String(pp2);
                            ViewBag.Image2 = $"data:image/jpeg;base64,{base64Image}";
                            //model.Image2 = ViewBag.Image2;


                            string fullPath = Server.MapPath(relativePath); // Resolve the relative path to an absolute path

                        }
                    }
                    else if (!string.IsNullOrEmpty(Request.Form["ExistingMainImage2"]))
                    { model.Image2 = Convert.FromBase64String(Request.Form["ExistingMainImage2"]); }

                    if (Image3FilePath == null || Image3FilePath.ContentLength == 0)
                    { ModelState.AddModelError("", "File 3 not uploaded ,upload image file"); }
                    else if (Image3FilePath != null && Image3FilePath.ContentLength > 0)
                    {
                        string filename3 = Path.GetFileName(Image3FilePath.FileName);

                        var baseFolder = Server.MapPath("~/Images");
                        var folderName = model.Name;
                        var subFolder = Path.Combine(baseFolder, folderName);
                        string fileName3 = Path.GetFileName(Image3FilePath.FileName);
                        string filepath = Path.GetExtension(Image3FilePath.FileName);

                        if (!Directory.Exists(subFolder)) { Directory.CreateDirectory(subFolder); }

                        if (filepath != ".jpg" && filepath != ".png" && filepath != ".jpeg")
                        { ViewBag.Message = "only .jpeg , .jpg ,.png files are allowed"; }
                        else
                        {
                            string fullfilepath3 = Path.Combine(subFolder, fileName3);
                            fullfilepath3 = GenerateUniqueFilePath(fullfilepath3);
                            Image3FilePath.SaveAs(fullfilepath3);
                            ViewBag.Message3 = "file 3 image  successfully saved to folder";

                            string relativePath = $"~/Images/{folderName}/{fileName3}";
                            model.Image3FilePath = relativePath;
                            ViewBag.filepath3 = model.Image3FilePath;

                            model.Image3 = System.IO.File.ReadAllBytes(fullfilepath3);

                            // Convert byte array to Base64 string
                            string base64Image = Convert.ToBase64String(model.Image3);
                            ViewBag.Image3 = $"data:image/jpeg;base64,{base64Image}";

                            //ViewBag.Image1 = model.MainImage1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(Request.Form["ExistingMainImage3"]))
                    { model.Image3 = Convert.FromBase64String(Request.Form["ExistingMainImage3"]); }

                    //Check if the email already exists
                    string checkQuery = "SELECT count(*) FROM SowmyaMatrimony WHERE RegisteredEmailId = '" + regmail + "' and Name= '" + model.Name + "' and Surname= '" + model.Surname + "'  ";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            conn.Open();
                            int countt = (int)checkCmd.ExecuteScalar();
                            if (countt > 0)
                            {
                                ModelState.AddModelError("", "A profile with the same email and name already exists.");
                                return View(model);
                            }
                            conn.Close();
                        }
                    }

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string checkQuery2 = "SELECT count(*) FROM SowmyaMatrimony WHERE RegisteredEmailId = '" + regmail + "' ";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery2, conn))
                        {
                            conn.Open();
                            int countt = (int)checkCmd.ExecuteScalar();
                            if (countt > 5)
                            {
                                ModelState.AddModelError("", "Cant Insert Profile With this Email id ,It is used for 5 times ");
                                return View(model);
                            }
                        }
                        conn.Close();
                    }

                    string checkQueryy = @"SELECT COUNT(*) FROM SowmyaMatrimony WHERE RegisteredEmailId = @RegisteredEmailId and Password IS NOT NULL AND Password != 'Pending' and Name='Pending' ";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand checkCmdd = new SqlCommand(checkQueryy, conn))
                        {
                            conn.Open();
                            checkCmdd.Parameters.AddWithValue("@RegisteredEmailId", regmail);
                            int count = (int)checkCmdd.ExecuteScalar(); // Get the count of matching rows
                            conn.Close();

                            string insertQuery;
                            if (count > 0)     //count > 0)// Query when RegisteredEmailId exists and Name is 'Pending'
                            {

                                string updateQuery = @" UPDATE SowmyaMatrimony SET  [Name] = @Name,  [Surname] = @Surname, [Age] = @Age, [Gender] = @Gender, 
                              [DOB] = @DOB, [TOB] = @TOB, [PlaceOfBirth] = @PlaceOfBirth, [CandiatePlace] = @CandiatePlace,  [Raasi] = @Raasi, [Star] = @Star, 
                               [Occupation] = @Occupation,[CandiateContact] = @CandiateContact, [MotherName] = @MotherName, [MotherOccupation] = @MotherOccupation, 
                              [MotherPlace] = @MotherPlace, [MotherContact] = @MotherContact, [FatherName] = @FatherName, [FatherOccupation] = @FatherOccupation, 
                              [FatherContact] = @FatherContact, [FatherPlace] = @FatherPlace, [MainImage1] = @MainImage1, [Image2] = @Image2, [Image3] = @Image3, 
                              [ProfileAddDateTime] = @ProfileAddDateTime,[CandiateEmailID] = @CandiateEmailID, [Saakha] = @Saakha ,[MainImage1FilePath]=@MainImage1FilePath ,
                              [Image2FilePath]=@Image2FilePath ,[Image3FilePath]=@Image3FilePath  WHERE  [RegisteredEmailId] = @RegisteredEmailId  and Password!='Pending'
                              and Password=ConformPassword and Name='Pending' ";

                                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@RegisteredEmailId", regmail);
                                    cmd.Parameters.AddWithValue("@Name", model.Name);
                                    cmd.Parameters.AddWithValue("@Surname", model.Surname);
                                    cmd.Parameters.AddWithValue("@Age", model.Age);
                                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                                    cmd.Parameters.AddWithValue("@DOB", model.DOB);
                                    cmd.Parameters.AddWithValue("@TOB", model.TOB);
                                    cmd.Parameters.AddWithValue("@PlaceOfBirth", model.PlaceOfBirth);
                                    cmd.Parameters.AddWithValue("@CandiatePlace", model.CandiatePlace);
                                    cmd.Parameters.AddWithValue("@Raasi", model.Raasi);
                                    cmd.Parameters.AddWithValue("@Star", model.Star);
                                    cmd.Parameters.AddWithValue("@Occupation", model.Occupation);
                                    cmd.Parameters.AddWithValue("@CandiateContact", model.CandiateContact);
                                    cmd.Parameters.AddWithValue("@MotherName", model.MotherName);
                                    cmd.Parameters.AddWithValue("@MotherOccupation", model.MotherOccupation);
                                    cmd.Parameters.AddWithValue("@MotherPlace", model.MotherPlace);
                                    cmd.Parameters.AddWithValue("@MotherContact", model.MotherContact);
                                    cmd.Parameters.AddWithValue("@FatherName", model.FatherName);
                                    cmd.Parameters.AddWithValue("@FatherOccupation", model.FatherOccupation);
                                    cmd.Parameters.AddWithValue("@FatherContact", model.FatherContact);
                                    cmd.Parameters.AddWithValue("@FatherPlace", model.FatherPlace);
                                    cmd.Parameters.AddWithValue("@MainImage1", @ViewBag.Image1);
                                    cmd.Parameters.AddWithValue("@Image2", @ViewBag.Image2);
                                    cmd.Parameters.AddWithValue("@Image3", @ViewBag.Image3);
                                    cmd.Parameters.AddWithValue("@CandiateEmailID", model.CandiateEmailID);
                                    cmd.Parameters.AddWithValue("@Saakha", model.Saakha);
                                    cmd.Parameters.AddWithValue("@ProfileAddDateTime", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@MainImage1FilePath", model.MainImage1FilePath);
                                    cmd.Parameters.AddWithValue("@Image2FilePath", model.Image2FilePath);
                                    cmd.Parameters.AddWithValue("@Image3FilePath", model.Image3FilePath);

                                    conn.Open();
                                    cmd.ExecuteNonQuery();

                                    conn.Close();
                                }
                            }
                            else
                            {  // Query when RegisteredEmailId does not exist
                                insertQuery = @"INSERT INTO SowmyaMatrimony  ([RegisteredEmailId], [Name], [Surname], [Age], [Gender], [DOB], [TOB], [PlaceOfBirth], 
                       [CandiatePlace],[Raasi], [Star], [Occupation], [CandiateContact], [MotherName], [MotherOccupation], [MotherPlace], [MotherContact], 
                        [FatherName], [FatherOccupation], [FatherContact], [FatherPlace], [MainImage1], [Image2], [Image3], [ProfileAddDateTime], [CandiateEmailID], 
                         [Saakha] ,[MainImage1FilePath] ,[Image2FilePath],  [Image3FilePath])   VALUES 
                      (@RegEmail, @Name, @Surname, @Age, @Gender, @DOB, @TOB, @PlaceOfBirth, @CandiatePlace, @Raasi, @Star, @Occupation, @CandiateContact, 
                      @MotherName, @MotherOccupation, @MotherPlace, @MotherContact, @FatherName, @FatherOccupation, @FatherContact, @FatherPlace, @MainImage1, 
                      @Image2, @Image3, @ProfileAddDateTime, @CandiateEmailID, @Saakha ,@MainImage1FilePath,@Image2FilePath ,@Image3FilePath) ";

                                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@RegEmail", regmail);
                                    cmd.Parameters.AddWithValue("@Name", model.Name);
                                    cmd.Parameters.AddWithValue("@Surname", model.Surname);
                                    cmd.Parameters.AddWithValue("@Age", model.Age);
                                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                                    cmd.Parameters.AddWithValue("@DOB", model.DOB);
                                    cmd.Parameters.AddWithValue("@TOB", model.TOB);
                                    cmd.Parameters.AddWithValue("@PlaceOfBirth", model.PlaceOfBirth);
                                    cmd.Parameters.AddWithValue("@CandiatePlace", model.CandiatePlace);
                                    cmd.Parameters.AddWithValue("@Raasi", model.Raasi);
                                    cmd.Parameters.AddWithValue("@Star", model.Star);
                                    cmd.Parameters.AddWithValue("@Occupation", model.Occupation);
                                    cmd.Parameters.AddWithValue("@CandiateContact", model.CandiateContact);
                                    cmd.Parameters.AddWithValue("@MotherName", model.MotherName);
                                    cmd.Parameters.AddWithValue("@MotherOccupation", model.MotherOccupation);
                                    cmd.Parameters.AddWithValue("@MotherPlace", model.MotherPlace);
                                    cmd.Parameters.AddWithValue("@MotherContact", model.MotherContact);
                                    cmd.Parameters.AddWithValue("@FatherName", model.FatherName);
                                    cmd.Parameters.AddWithValue("@FatherOccupation", model.FatherOccupation);
                                    cmd.Parameters.AddWithValue("@FatherContact", model.FatherContact);
                                    cmd.Parameters.AddWithValue("@FatherPlace", model.FatherPlace);
                                    cmd.Parameters.AddWithValue("@MainImage1", @ViewBag.Image1);
                                    cmd.Parameters.AddWithValue("@Image2", @ViewBag.Image2);
                                    cmd.Parameters.AddWithValue("@Image3", @ViewBag.Image3);
                                    cmd.Parameters.AddWithValue("@CandiateEmailID", model.CandiateEmailID);
                                    cmd.Parameters.AddWithValue("@Saakha", model.Saakha);
                                    cmd.Parameters.AddWithValue("@ProfileAddDateTime", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@MainImage1FilePath", model.MainImage1FilePath);
                                    cmd.Parameters.AddWithValue("@Image2FilePath", model.Image2FilePath);
                                    cmd.Parameters.AddWithValue("@Image3FilePath", model.Image3FilePath);

                                    conn.Open();
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                            }
                        }
                    }
                    ViewBag.Failure = "Profile Saved Successfully";
                    TempData["Success"] = "Profile Saved Successfully";
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert(' Data inserted successfully');", true);
                    return RedirectToAction("Payment");
                }

                catch (Exception ex)
                {
                    ViewBag.Failure = "Profile Not Saved";
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    ModelState.AddModelError("", "An error occurred while saving the data." + ex + "   and " + error);
                    return View();
                }
            }
            return View(model); //return the same view for errors
        }

        private string GenerateUniqueFilePath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            int counter = 1;

            // Ensure the file name is unique
            while (System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(directory, $"{fileNameWithoutExtension}_{counter}{extension}");
                counter++;
            }

            return filePath;
        }


       
    }
}