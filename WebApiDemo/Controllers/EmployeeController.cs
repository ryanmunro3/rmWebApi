using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiDemo.Models;
using WebApiDemo.Security;

namespace WebApiDemo.Controllers
{

    //[FromUri] - can be used to route complex types to the uri // default on complex types is [FromBody]
    // simple types default to the URI, can be forced to body with [FromBody]
    //if you want to disable a specific action you can use [DisableCors] on the function/action

    //you can add min/max constraits as well (min, max, length, range, minlength, maxlength)
    
    [EnableCorsAttribute("http://localhost:57042", "*", "*")]
    [RoutePrefix("api/employee")]  // you can use ~ in the route attribute to override this functionality
    public class EmployeeController : ApiController
    {
        [HttpGet]
        [BasicAuthentication]
        [Route("")]
        public IHttpActionResult Get(string gender="All")
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            using (WebApiDBEntities entity = new WebApiDBEntities())
            {
                switch(username.ToLower())
                {
                    case "all":
                        return Ok(entity.Employees.ToList());
                    case "ryan":
                        return Ok(entity.Employees.Where(e => e.Gender.ToLower() == "male").ToList());
                    case "jenn":
                        return Ok(entity.Employees.Where(e => e.Gender.ToLower() == "female").ToList());
                    default:
                        return Unauthorized();
                }
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetEmployeeByID(int id)
        {
            using (WebApiDBEntities entity = new WebApiDBEntities())
            {
                Employee emp = entity.Employees.FirstOrDefault(e => e.ID == id);
                if (emp != null)
                    return Ok(emp);
                else
                    return NotFound();
            }
        }

        [HttpGet]
        [Route("{name:alpha}")]
        public IHttpActionResult GetEmployeeByID(string name)
        {
            using (WebApiDBEntities entity = new WebApiDBEntities())
            {
                Employee emp = entity.Employees.FirstOrDefault(e => e.FirstName == name);
                if (emp != null)
                    return Ok(emp);
                else
                    return NotFound();
            }
        }

        [HttpGet]
        [Route("{id}/salary")]
        public IHttpActionResult GetEmployeeSalary(int id)
        {
            using (WebApiDBEntities entity = new WebApiDBEntities())
            {
                Employee emp = entity.Employees.FirstOrDefault(e => e.ID == id);
                if (emp != null)
                    return Ok(emp.Salary);
                else
                    return NotFound();
            }
        }

        [HttpPost]
        [Route("{id}")]
        public IHttpActionResult Post([FromBody]Employee emp)
        {
            try
            {
                using (WebApiDBEntities entity = new WebApiDBEntities())
                {
                    entity.Employees.Add(emp);
                    entity.SaveChanges();
                    return Created(Request.RequestUri + "/" + emp.ID.ToString(), emp);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                using (WebApiDBEntities entity = new WebApiDBEntities())
                {
                    var emp = entity.Employees.FirstOrDefault(e => e.ID == id);
                    if (emp == null)
                        return NotFound();
                    else
                    {
                        entity.Employees.Remove(emp);
                        entity.SaveChanges();
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody]Employee emp)
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
                        return Ok(updateEmp);
                    }
                    else
                        return NotFound();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
