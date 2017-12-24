using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiDemo.Models;

namespace WebApiDemo.Security
{
    public class EmployeeSecurity
    {
        public static bool Login(string username, string password)
        {
            using (WebApiDBEntities entities = new WebApiDBEntities())
            {
                return entities.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);
            }
        }
    }
}