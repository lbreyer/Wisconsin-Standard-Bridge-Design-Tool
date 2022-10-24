using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WisDot.Bos.StandardPlans.Data;

namespace WisDot.Bos.StandardPlans.Models
{
    public class User
    {
        public string wamsID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNum { get; set; }
        public bool isInactive { get; set; }
        public DateTime inactiveDate { get; set; }

        //public DateTime inactiveDate { get; set; }

        public User(string wamsID, string firstName, string lastName, string email, string phoneNum)
        {
            this.wamsID = wamsID;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.phoneNum = phoneNum;
            isInactive = false;
            inactiveDate = DateTime.MaxValue;
            //this.inactiveDate = inactiveDate;
        }

        public User()
        {
            Initialize();
        }

        public void Initialize()
        {
            wamsID = "";
            firstName = "";
            lastName = "";
            email = "";
            phoneNum = "";
        }

        public void DeactivateUser(DateTime inactiveDate)
        {

        }

        public void DeactivateUser()
        {

        }
    }
}