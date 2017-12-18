using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
    public class EmployeeController : ApiController
    {
        public IEnumerable<Employee> Get()
        {
            using (WebApiDBEntities entity = new WebApiDBEntities())
            {
                return entity.Employees.ToList();
            }
        }

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
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }         
        }

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
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
