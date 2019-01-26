using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamForm.Models
{
    public class NewUser
    {
        [Required(ErrorMessage = "ID is required")]
        public int NewUserID { get; set; }

        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "UserRole is required")]
        public string UserRole { get; set; }
    }
    public enum RoleDDL
    {
        SuperUser,
        Admin,
        Student
    }
}