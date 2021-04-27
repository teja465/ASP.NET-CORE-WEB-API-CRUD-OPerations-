using JWTAuthenticationRestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JWTAuthenticationRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static Context _context;
        public UsersController( Context context)
        {
            _context = context;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult Get()
        {
            
            var users = _context.Users.ToList<User>();
            _context.SaveChanges();
            // var jsonResponse = JsonConvert.SerializeObject(users);
            return  new JsonResult(users);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
           if(id<=0){
               Response.StatusCode=404;
               return new JsonResult("Invalid user id");
           }
           var user = _context.Users.Find(id);
           if (user ==null) {
               Response.StatusCode=404;
               return new JsonResult("User with given ID not found may be deleted :)");
           }
           return  new JsonResult(user);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public IActionResult Post([FromBody] User userf)
        {
            if (userf.city == null || userf.email == null || userf.password == null || userf.username == null) 
            { 
                Response.StatusCode=400;
                return  new JsonResult("city,email,password & username  cant be NULL");
            }
            if (userf.city == "" || userf.email == "" || userf.password == "" || userf.username == "") 
            { 
                Response.StatusCode=400;
                return  new JsonResult("city,email,password & username  cant be EMPTY ");
            }
            var is_there = _context.Users.Where(u => u.email == userf.email).Any();
            if(is_there)
            {
                return  new JsonResult("email already registered please use anothre email and try again");
            }
             is_there = _context.Users.Where(u => u.username == userf.username).Any();
            if (is_there)
            {
                return  new JsonResult("Username  already registered please use another username and  try again");
            }
            _context.Users.Add(userf);
            var resp = _context.SaveChanges();
            if(resp > 0)
            {
                return new JsonResult("added successfully");
            }
            Response.StatusCode=404;
            return new JsonResult("unable to add user to database please try again");
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public Object Delete(long id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                Response.StatusCode=404;
                return "No user found with given ID";
            }
            _context.Users.Remove(user);
            if (_context.SaveChanges() > 0)
            {
                return "User with given ID successfully deleted ";
            }
            return "Unable to  delete  User try again ";


        }
    }
}
