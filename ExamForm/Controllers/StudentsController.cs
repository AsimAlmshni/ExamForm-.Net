using ExamForm.Models;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data.Entity;
using PagedList;
using ExamForm.Infrastructure;
using ExamForm.Security;

namespace ExamForm.Controllers
{
    public class StudentsController : Controller
    {
        private StudentsDBEntities1 DBEntities1 = new StudentsDBEntities1();
        private List<SLoginInfo> listedData = new List<SLoginInfo>();

        // GET: Students
        public ActionResult QuizView()
        {
            return View();
        }
        // POST: Students
        [HttpPost]
        public ActionResult QuizView(Users model)
        {
            StudentsDBEntities1 cbe = new StudentsDBEntities1();
            var s = cbe.GetSLoginInfo(model.UserName, model.Password);

            var tbl = from t in DBEntities1.SLoginInfoes
                      where (t.UserName == model.UserName) && (t.Password == model.Password)
                      select t.StudentID;
            try
            {
                Session["User"] = tbl.ToList().First();

            }
            catch (Exception)
            {
                //Exception Happen
            }
                var item = s.FirstOrDefault();
                if (ModelState.IsValid)
                {
                    if (item == "SuccessAdmin")
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        return RedirectToAction("AdminView", "Home");
                    }
                    else if (item == "Success")
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (item == "User Does not Exists")
                    {
                        ViewBag.NotValidUser = item;
                    }
                    else if (item == "Account been deleted")
                    {
                        ViewBag.NotValidUser = item;
                    }
                    else
                    {
                        ViewBag.Failedcount = item;
                    }
                }
            return View("QuizView");
        }


        public ActionResult UserLandingView()
        {
            return View();
        }

        [ProfileAuth(Role = "Admin")]
        public ActionResult Edit(int id)
        {
            var tbl = from t in DBEntities1.SLoginInfoes select t;
            listedData = tbl.ToList();

            var std = listedData.Where(s => s.StudentID == id).FirstOrDefault();

            return View(std);
        }

        [ProfileAuth(Role = "Admin")]
        [ProfileAction (Discriptor = "Students/Edit")]
        [HttpPost]
        public ActionResult Edit(SLoginInfo model)
        {
            if (ModelState.IsValid)
            {
                DBEntities1.Entry(model).State = EntityState.Modified;
                DBEntities1.SaveChanges();
                return RedirectToAction("AdminView", "Home");
            }

            return View("Edit");
        }

        
        [Authorize ]
        public ActionResult DeleteStudent(int id)
        {
            string connetionString = null;
            string sql = null;
            SqlCommand command;
            SqlConnection cnn;
            SqlDataAdapter adapter = new SqlDataAdapter();

            connetionString = "Server = DESKTOP-NUBO06T; Database = StudentsDB; Integrated Security = SSPI;";
            sql = "update SLoginInfo set Deleted=1 where StudentID="+id;

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();

                command = new SqlCommand(sql, cnn);
                adapter.DeleteCommand = new SqlCommand(sql, cnn);
                adapter.DeleteCommand.ExecuteNonQuery();

                command.Dispose();
                cnn.Close();
            }
            catch (Exception)
            {
                //Exception happen
            }
            return RedirectToAction("AdminView", "Home");
        }

        //[HttpGet]
        //public ActionResult Search(string searchString)
        //{
        //    var data = from m in DBEntities1.SLoginInfoes
        //               select m;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        data = data.Where(s => s.UserName.Contains(searchString));
        //    }
        //    ViewBag.StudentsData = data.ToList();
        //    return RedirectToAction("AdminView","Home");
        //}

        // Http Get
        //public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        //{
        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }

        //    ViewBag.CurrentFilter = searchString;

        //    var students = from s in DBEntities1.SLoginInfoes
        //                   select s;
        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        students = students.Where(s => s.UserName.Contains(searchString));
        //    }
        //    ViewBag.StudentsData = students.ToList();

        //    int pageSize = 3;
        //    int pageNumber = (page ?? 1);
        //    //return View(students.ToPagedList(pageNumber, pageSize));
        //    return RedirectToAction("AdminView", "Home");
        //}
        //private bool ValidateUser(string Email, string Password)
        //{
        //    bool isValid = false;
        //    using (var db = new StudentsDBEntities1())
        //    {
        //        var Usr = db.SLoginInfoes.FirstOrDefault(a => a.UserName == Email);

        //        if (Usr != null)
        //        {
        //            if (Usr.Password == Password)
        //            {
        //                Session["UserName"] = Usr.UserName;
        //                isValid = true;
        //            }
        //        }
        //    }
        //    return isValid;
        //}

        [HttpGet]
        [ProfileAuth(Role = "Admin")]
        public ActionResult Admin()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            try
            {
                // simple way to get the whole data from table in db
                var tbl = from t in DBEntities1.SLoginInfoes select t;
                listedData = tbl.ToList();
                ViewBag.StudentsData = listedData;

                var iUserID = Convert.ToInt32(HttpContext.Session["User"].ToString());

                var tbl2 = from x in DBEntities1.SLoginInfoes
                           where x.StudentID == iUserID
                           select x.UserName;
                ViewBag.User_Name = tbl2.FirstOrDefault().ToString();
            }
            catch (Exception)
            {
                // Exception happen 
            }
            return View();
        }

        [HttpPost]
        [ProfileAuth (Role = "Admin")]
        public ActionResult Search(SLoginInfo searchText)
        {
            if (searchText.UserName == null)
            {
                var tbl = from t in DBEntities1.SLoginInfoes select t;
                listedData = tbl.ToList();
            }
            else
            {
                var tbl = from t in DBEntities1.SLoginInfoes
                          where t.UserName.Contains(searchText.UserName)
                          select t;
                listedData = tbl.ToList();
            }
            ViewBag.StudentsData = listedData;
            return View("Admin");
        }

        [HttpGet]
        [ProfileAuth (Role = "Admin")]
        public ActionResult Info(int id)
        {
            var tbl = from t in DBEntities1.SLoginInfoes
                      join t2 in DBEntities1.Users
                      on t.StudentID equals t2.UserID
                      where t.StudentID == id
                      select t;
            listedData = tbl.ToList();

            return View(tbl.FirstOrDefault());
        }
    }
}
