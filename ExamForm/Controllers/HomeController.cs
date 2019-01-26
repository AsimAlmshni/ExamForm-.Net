using ExamForm.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.Web.Security;
using System.Data.SqlClient;
using ExamForm.Security;
using ExamForm.Infrastructure;

namespace ExamForm.Controllers
{
    public class HomeController : Controller
    {
        private List<Questions> data = new List<Questions>();
        private List<Answers> Ansrs = new List<Answers>();
        private StudentsDBEntities1 DBEntities1 = new StudentsDBEntities1();
        private List<SLoginInfo> listedData = new List<SLoginInfo>();
        private List<ExamQuestoin> EQuestions = new List<ExamQuestoin>();

        private ExamTemp Ex = new ExamTemp();

        private int correctAns;

        string hh;
        string mm;
        string ss;

        public static System.Timers.Timer timer = new System.Timers.Timer(60); // This will raise the event every one minute.
        private int h, m, s, id = 0;

        private string[] ansList = new string[3];

        [HttpGet]
        public ActionResult Index()
        {
                h = Convert.ToInt32("0");
                m = Convert.ToInt32("10");
                s = Convert.ToInt32("10");

                timer.Enabled = true;
                timer.Start();
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

                GetQuestions();
                //FillFun();
                //Ex.Qus = data;
                //Ex.Ans = Ansrs;
            if (User.Identity.IsAuthenticated)
            {
                return View(Ex);
            }
            if (!Session.IsCookieless)
            {
                return View(Ex);
            }
            else
            {
                return RedirectToAction("QuizView", "Students");
            }
        }
        //void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    // Do Your Stuff
        //    s = s - 1;
        //    if (s == -1)
        //    {
        //        m = m - 1;
        //        s = 59;
        //    }
        //    if (m == -1)
        //    {
        //        h = h - 1;
        //        m = 59;
        //    }

        //    if (h == 0 && m == 0 && s == 0)
        //    {
        //        timer.Stop();
        //        ViewBag.tim =  "time out";
        //    }

        //    hh = Convert.ToString(h);
        //    mm = Convert.ToString(m);
        //    ss = Convert.ToString(s);
        //    hh = mm = ss = "";
        //    ViewBag.tim = "" + hh + ":" + mm + ":" + ss + "";


        //}

        public void ExportToExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
            ws.Name = "Exam Report";

            ws.Cells["A1"].Value = "OnlineExam";
            ws.Cells["B1"].Value = "ITG";

            ws.Cells["A2"].Value = "ExamReport";
            ws.Cells["B2"].Value = "Exam";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            ws.Cells["A6"].Value = "Questions";
            ws.Cells["B6"].Value = "Answer";

            FillFun();
            int rowStart = 7;
            foreach (var item in data)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Question;
                ws.Cells[string.Format("B{0}", rowStart)].Value = "Create function to get the correct ansers only";
                rowStart++;
            }
            rowStart++;
            ws.Cells[string.Format("A{0}", rowStart)].Value = "Graded : " + correctAns + " of " + data.Count;

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-diposition","attachment: filenames"+"ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        public PartialViewResult _Ajax(FormCollection fc) 
        {
            
            correctAns = 0;
            ViewBag.Answer1 = fc["Answer1"];
            ViewBag.Answer2 = fc["Answer2"];
            ViewBag.Answer3 = fc["Answer3"];

            if ("True" == fc["Answer1"])
            {
                correctAns++;
            }
            if ("True" == fc["Answer2"])
            {
                correctAns++;
            }
            if ("True" == fc["Answer3"])
            {
                correctAns++;
            }

            string connetionString = null;
            string sql = null;
            //SqlDataReader dataReader;
            SqlCommand command;
            SqlConnection cnn;
            SqlDataAdapter adapter = new SqlDataAdapter();



            connetionString = "Server = DESKTOP-NUBO06T; Database = StudentsDB; Integrated Security = SSPI;";
            //sql = "Select * from dbo.SLoginInfo";
            var iUserID = 0;
            try
            {
                iUserID = Convert.ToInt32(HttpContext.Session["User"].ToString());
            }
            catch (Exception)
            {
            }
            if (iUserID != 0)
            {
                sql = "Update SLoginInfo set UserResult=" + correctAns + "where StudentID=" + iUserID;

                cnn = new SqlConnection(connetionString);

                try
                {
                    cnn.Open();

                    command = new SqlCommand(sql, cnn);
                    adapter.UpdateCommand = new SqlCommand(sql, cnn);
                    adapter.UpdateCommand.ExecuteNonQuery();
                    command.Dispose();
                    cnn.Close();
                }
                catch (Exception)
                {
                }
            }
                ViewBag.CorrectAnswer = "correctAnswer " + correctAns + " of " + 3 + " Questions";
            return PartialView("_Ajax");
        }

        public ActionResult Info()   
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            var iUserID = Convert.ToInt32(HttpContext.Session["User"].ToString());

            var tbl = from t in DBEntities1.SLoginInfoes
                      where (t.StudentID == iUserID)
                      select t;

            var ListedData = tbl.ToList().First();
            return View(ListedData);
        }

        [ProfileAuthAttribute (Role = "Admin")]
        public ActionResult AdminView()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            try
            {
                // simple way to get the whole data from table in db
                var tbl = from t in DBEntities1.SLoginInfoes select t;
                listedData = tbl.ToList();
                ViewBag.StudentsData = listedData;

                // anotehr way to get data from data base
                //var type = from p in DBEntities1.SLoginInfoes
                //           select new
                //           {
                //               id = p.StudentID,
                //               name = p.UserName,
                //               pass = p.Password
                //          };
                //var reo = type.ToList();

                //var st = (from tt in context.Teachers
                //          join ss in context.Students
                //          on tt.ID equals ss.teacherID
                //          where tt.TypeID == 2
                //          select new Test
                //          {
                //              Id = tt.ID,
                //              Text = tt.Text,
                //              Students = new List<StudentsDTO>()
                //                        {
                //                            new StudentsDTO()
                //                            {
                //                                Name= ss.Name,
                //                                Id= ss.StudentID
                //                            }
                //                        }.ToList()
                //          }).ToList();
            }
            catch (Exception)
            {
                // Exception happen 
            }
            return RedirectToAction("Admin", "Students");
        }

        public void GetQuestions()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            try
            {
                // simple way to get the whole data from table in db
                var tbl = from t in DBEntities1.ExamQuestoins select t;
                EQuestions = tbl.ToList();
                Ex.Ans = EQuestions;
                ViewBag.DBEQs = EQuestions;
            }
            catch (Exception)
            {
                // Exception happen 
            }
        }

        public void FillFun()
        {
            try
            {
            XmlDocument xdc = new XmlDocument();
            xdc.Load(Server.MapPath("~/App_Data/Questions.xml"));

            XmlNodeList xnlNodes = xdc.SelectNodes("Questions/problem");

                    string response = "";
                    XmlNodeList itemNodes = xdc.GetElementsByTagName("problem");
                    if (itemNodes.Count > 0)
                    {
                        foreach (XmlElement node in itemNodes)
                        {
                            response = node.SelectSingleNode("Q1").InnerXml;
                            data.Add(
                           new Questions
                           {
                               Question = response
                           });
                           response = "";
                        }
                    }

                    XmlNodeList itemNodes3 = xdc.GetElementsByTagName("choicegroup");
                    string response2 = "";
                    if (itemNodes3.Count > 0)
                    {
                        XmlNodeList itemNodes4 = xdc.GetElementsByTagName("choice");

                        foreach (XmlElement node2 in itemNodes4)
                        {                                        
                            response2 = node2.InnerText.ToString();
                            Ansrs.Add(
                                new Answers
                                {
                                    Anser = response2,
                                    Check = node2.GetAttribute("correct")
                        });
                            response2 = "";
                        }
                    }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("FileNotFoundException");
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException source: {0}", e.Source);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("IndexOutOfRangeException");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("\nExecution of the finally block after an unhandled\n");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            Session.Clear();
            Session.RemoveAll();

            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();
            return RedirectToAction("QuizView", "Students");
        }

        [HttpPost]
        [ProfileAuth (Role = "Admin")]
        public ActionResult Registration(NewUser model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StudentsDBEntities1())
                {
                    var encryptedPassword = Security.Encryption.Encrypt(model.NewPassword);
                    var user = db.SLoginInfoes.Create();
                    var role = db.Users.Create();
                    user.StudentID = model.NewUserID;
                    user.UserName = model.UserName;
                    user.Password = model.NewPassword;
                    user.LoginFailedCount = 0;
                    user.UserResult = 0;
                    role.UserID = model.NewUserID;
                    role.UserRole = model.UserRole;
                    db.SLoginInfoes.Add(user);
                    db.Users.Add(role);
                    var xo = db.GetValidationErrors();
                    db.SaveChanges();
                }   
            }
            else
            {
                ModelState.AddModelError("", "One or more fields have been");
            }
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }
    }
}