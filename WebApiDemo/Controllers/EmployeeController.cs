using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{

    //[FromUri] - can be used to route complex types to the uri // default on complex types is [FromBody]
    // simple types default to the URI, can be forced to body with [FromBody]
    //if you want to disable a specific action you can use [DisableCors] on the function/action
    
    [EnableCorsAttribute("http://localhost:57042", "*", "*")]
    public class EmployeeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string gender="All")
        {
            using (WebApiDBEntities entity = new WebApiDBEntities())
            {
                switch(gender.ToLower())
                {
                    case "all":
                        return Request.CreateResponse(HttpStatusCode.OK, entity.Employees.ToList());
                    case "male":
                        return Request.CreateResponse(HttpStatusCode.OK, entity.Employees.Where(e => e.Gender.ToLower() == "male").ToList());
                    case "female":
                        return Request.CreateResponse(HttpStatusCode.OK, entity.Employees.Where(e => e.Gender.ToLower() == "female").ToList());
                    default:
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Value for gender must be All, Male, or Female. " + gender + " is invalid.");
                }
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            using (WebApiDBEntities entity = new WebApiDBEntities())
            {
                Employee emp = entity.Employees.FirstOrDefault(e => e.ID == id);
                if (emp != null)
                    return Request.CreateResponse(HttpStatusCode.OK, emp);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with ID " + id + " not found.");
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]Employee emp)
        {
            try
            {
                using (WebApiDBEntities entity = new WebApiDBEntities())
                {
                    entity.Employees.Add(emp);
                    entity.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, emp);
                    message.Headers.Location = new Uri(Request.RequestUri + "/" + emp.ID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (WebApiDBEntities entity = new WebApiDBEntities())
                {
                    var emp = entity.Employees.FirstOrDefault(e => e.ID == id);
                    if (emp == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with ID = " + id.ToString() + " not found!");
                    else
                    {
                        entity.Employees.Remove(emp);
                        entity.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody]Employee emp)
        {
            try
            {
                using (WebApiDBEntities entity = new WebApiDBEntities())
                {
                    var updateEmp = entity.Employees.FirstOrDefault(e => e.ID == id);
                    if (updateEmp != null)
                    {
                        updateEmp.FirstName = emp.FirstName;
                        updateEmp.LastName = emp.LastName;
                        updateEmp.Gender = emp.Gender;
                        updateEmp.Salary = emp.Salary;
                        entity.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, updateEmp);
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with ID = " + id.ToString() + " does not exist!");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
