using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamForm.Models
{
    public class ExamTemp   
    {
        public List<ExamQuestoin> Ans { get; set; }

        //indexer XML file needed 
        //public List<Answers> Ans { get; set; }
        //public List<Questions> Qus { get; set; }
    }
}