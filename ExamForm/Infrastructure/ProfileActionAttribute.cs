using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamForm.Infrastructure
{
    public class ProfileActionAttribute : FilterAttribute, IActionFilter
    {
        StudentsDBEntities1 DBEntities1 = new StudentsDBEntities1();

        private Stopwatch timer;
        public string Discriptor { get; set; }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            var iUserID = Convert.ToInt32(HttpContext.Current.Session["User"].ToString());

            try
            {
                var tbl = from t in DBEntities1.SLoginInfoes
                          join t2 in DBEntities1.Users
                          on t.StudentID equals t2.UserID
                          join t3 in DBEntities1.RelationURs
                          on t2.UserID equals t3.UserID
                          join t4 in DBEntities1.Roles
                          on t3.RoleID equals t4.RoleID
                          where (t.StudentID == iUserID)
                          select new { role = t4.RoleDir };


                var ListedData = tbl.ToList().First();
                string ro = ListedData.role.ToString().Trim();
                if (filterContext.HttpContext.Request.IsLocal)
                {
                    if (Discriptor.Equals(ro))
                    {
                        //greeteing
                    }
                    else
                    {
                        filterContext.Result = new HttpNotFoundResult();
                    }
                }
            }
            catch (Exception)
            {
                // Exception happen 
            }
            timer = Stopwatch.StartNew();
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            timer.Stop();
            if (filterContext.Exception == null)
            {
                filterContext.HttpContext.Response.Write(
                string.Format("<div>Action method elapsed time: {0:F6}</div>",
                timer.Elapsed.TotalSeconds));
            }
        }
    }
}