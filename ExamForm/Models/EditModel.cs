using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamForm.Models
{
    public class EditModel
    {
        [Key]
        public int UserID { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "User Result")]
        public string UserResult { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

    }   
}