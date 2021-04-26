using JWTAuthenticationRestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAuthenticationRestAPI.Models;
using System;
using System.Collections.Generic;
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
        public Object Get()
        {
            var users = _context.Users.ToList<User>();
            _context.SaveChanges();
            var jsonResponse = JsonConvert.SerializeObject(users);

            return jsonResponse;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public string Post([FromBody] User userf)
        {
            if (userf.city == null || userf.email == null || userf.password == null || userf.username == null) 
            { 
                return "city,email,password & username  cant be NULL";
            }
            var is_there = _context.Users.Where(u => u.email == userf.email).Any();
            if(is_there)
            {
                return "email already registered please use anothee email and try again";
            }
             is_there = _context.Users.Where(u => u.username == userf.username).Any();
            if (is_there)
            {
                return "Username  already registered please use another username and  try again";
            }
            _context.Users.Add(userf);
            var resp = _context.SaveChanges();
            if(resp > 0)
            {
                return "added successfully";
            }
            return "unable to add user  to database";
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
