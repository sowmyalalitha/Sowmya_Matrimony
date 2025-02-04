using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace asp.netwebmvcWebsite.Models
{
    public class PaymentModel
    {
        public string RegisteredEamilId { get; set; }
        public string PaymentId { get; set; }
     
        public string BrideOrderId { get; set; }
         public string BrideStatus { get; set; }
        public decimal BrideAmount { get; set; }
     
        public string BroomOrderId { get; set; }
        public string BroomStatus { get; set; }
       
        // public int PaymentId { get; set; }
 
        public string ProlileOrderId { get; set; }
        public string ProfileAmountStatus { get; set; }
        public int ProfileAmount { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public string Gender { get; set; }

        public string Name { get; set; }
        



    }
}