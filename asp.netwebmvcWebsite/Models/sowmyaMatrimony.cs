using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.netwebmvcWebsite.Models
{
    
    public class sowmyaMatrimony
    {
            //[Required(ErrorMessage = "Email is Requried")]
            //[RegularExpression(@"[a-zA-Z0-9.-]+(.[a-zA-Z]{2,})+", ErrorMessage = "Enter a valid Email.")]
            public string RegisteredEmailId { get; set; }

            
            public string Password { get; set; }

            
            public string ConformPassword { get; set; }
          
            [Required(ErrorMessage = "Name is Requried")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Surname is Requried")]
            public string Surname { get; set; }

            [Required(ErrorMessage = "Age is Requried")]
            public string Age { get; set; }

            [Required(ErrorMessage = "Gender is Requried")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "DOB is Requried")]
            public DateTime DOB { get; set; }

            [Required(ErrorMessage = "TOB is Requried")]
            public TimeSpan TOB { get; set; }

            [Required(ErrorMessage = "Place of Birth is Requried")]
            public string PlaceOfBirth { get; set; }

            [Required(ErrorMessage = "Candiate Place is Requried")]
            public string CandiatePlace { get; set; }

            [Required(ErrorMessage = "Raasi  is Requried")]
            public string Raasi { get; set; }

            [Required(ErrorMessage = "Star  is Requried")]
            public string Star { get; set; }

            [Required(ErrorMessage = "Occupation is Requried")]
            public string Occupation { get; set; }

            [Required(ErrorMessage = "Candiate Contact  is Requried")]
            public string CandiateContact { get; set; }

            [Required(ErrorMessage = "Mother Name  is Requried")]
            public string MotherName { get; set; }

            [Required(ErrorMessage = "Mother  Occupation  is Requried")]
            public string MotherOccupation { get; set; }

            [Required(ErrorMessage = "Mother Place is Requried")]
            public string MotherPlace { get; set; }

            [Required(ErrorMessage = "Mother Contact is Requried")]
            public string MotherContact { get; set; }

            [Required(ErrorMessage = "Father Name is Requried")]
            public string FatherName { get; set; }

            [Required(ErrorMessage = "Father Occupation is Requried")]
            public string FatherOccupation { get; set; }

            [Required(ErrorMessage = "Father Contact is Requried")]
            public string FatherContact { get; set; }

            [Required(ErrorMessage = "Father Place is Requried")]
            public string FatherPlace { get; set; }
            public int ViewAmount { get; set; }
            public string ViewAmountStatus { get; set; }
            public int ProfileAmount { get; set; }
            public string ProfileAmountStatus { get; set; }

           public byte[] MainImage1 { get; set; }

            public byte[] Image2 { get; set; }

          
            public byte[] Image3 { get; set; }
            public DateTimeOffset ProfileAddDateTime { get; set; }

            [Required(ErrorMessage = "Candiate EmailID is Requried")]
            //[Unique(ErrorMessage="Email Id Already Exist")]
            
            
            public string CandiateEmailID { get; set; }

            [Required(ErrorMessage = "Saakha is Requried")]
            public string Saakha { get; set; }

        [Required(ErrorMessage ="File1 is Mandatory upload image only from AddDetails model")]
            public string MainImage1FilePath { get; set; }

        [Required(ErrorMessage = "File2 is Mandatory upload image only from AddDetails model")]
        public string Image2FilePath { get; set; }
        [Required(ErrorMessage = "File3 is Mandatory upload image only from AddDetails model")]
        public string Image3FilePath { get; set; }

        }
    }
