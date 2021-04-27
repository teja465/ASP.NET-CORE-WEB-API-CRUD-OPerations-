using JWTAuthenticationRestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JWTAuthenticationRestAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static Context _context;
        private IConfiguration _config;
        private IJWTAuthenticationManager _auth;

        public UsersController( Context context, IConfiguration config, IJWTAuthenticationManager authenticationManager)
        {
            _context = context;
            _config = config;
            _auth = authenticationManager;
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
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            //if(id<=0){
            //    Response.StatusCode=404;
            //    return new JsonResult("Invalid user id");
            //}
            //var user = _context.Users.Find(id);
            //if (user ==null) {
            //    Response.StatusCode=404;
            //    return new JsonResult("User with given ID not found may be deleted :)");
            //}
            //return  new JsonResult(user);

            var token = _auth.Authenticate("ravi", "Pass@123");
            return new JsonResult( token);
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

        private string GenerateJSONWebToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
