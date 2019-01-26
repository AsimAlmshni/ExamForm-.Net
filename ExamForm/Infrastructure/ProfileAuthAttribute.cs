using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamForm.Infrastructure
{
    public class ProfileAuthAttribute : AuthorizeAttribute
    {
        public string Role { get; set; }
        StudentsDBEntities1 DBEntities1 = new StudentsDBEntities1();


        public ProfileAuthAttribute() : base()
        {
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string ro = null;
            SqlDataAdapter adapter = new SqlDataAdapter();
            var iUserID = Convert.ToInt32(HttpContext.Current.Session["User"].ToString());

            try
            {
                var tbl = from t in DBEntities1.SLoginInfoes
                          join t2 in DBEntities1.Users
                          on t.StudentID equals t2.UserID
                          where (t.StudentID == iUserID)
                          select new { userRole = t2.UserRole};


                var ListedData = tbl.ToList().First();
                ro = ListedData.userRole.ToString().Trim();

            }
            catch (Exception)
            {
                // Exception happen 
            }

            if (httpContext.Request.IsLocal)
            {
                if (Role.Equals(ro))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}