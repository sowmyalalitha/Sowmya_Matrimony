using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace asp.netwebmvcWebsite.Models
{
    public class DetailedView
    {
        public string RegisteredEmailId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Age { get; set; }

        public string Gender { get; set; }

        public DateTime DOB { get; set; }

        public TimeSpan TOB { get; set; }

        public string PlaceOfBirth { get; set; }

        public string CandiatePlace { get; set; }

        public string Raasi { get; set; }

        public string Star { get; set; }

        public string Occupation { get; set; }

        public string CandiateContact { get; set; }

        public string MotherName { get; set; }

        public string MotherOccupation { get; set; }

        public string MotherPlace { get; set; }

       
        public string MotherContact { get; set; }

       
        public string FatherName { get; set; }

        
        public string FatherOccupation { get; set; }

       
        public string FatherContact { get; set; }

       
        public string FatherPlace { get; set; }
       
        public byte[] MainImage1 { get; set; }

        public byte[] Image2 { get; set; }


        public byte[] Image3 { get; set; }
       


        public string CandiateEmailID { get; set; }

       
        public string Saakha { get; set; }

       
        public string MainImage1FilePath { get; set; }

       
        public string Image2FilePath { get; set; }
       

        public string Image3FilePath { get; set; }
    }
}