using EMS.Domain.DTO;
using EMS.Domain.Entities;
using EMS.Domain.RepositoryInterface;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
      

        public UsersController(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
           
        }

        //GET: Get all users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            //Get data from database - Domain Models(using DbContext)
            var usersDomain =  dbContext.Users.ToList();

            //using Repository Pattern
            //var usersDomain = await userRepository.GetAllUsersAsync();

            //Map Domain Models to DTOs
            var usersDto = new List<UsersDTO>();

            //Return DTOs back to client
           foreach(var userDomain in usersDomain)
            {
                var userDto = new UsersDTO
                {
                    UserID = userDomain.UserID,
                    UserName = userDomain.UserName,
                    EmailID = userDomain.EmailID
                  
                };
                usersDto.Add(userDto);
            }

            return Ok(usersDto);
        }

        //GET: Get user by id
        [HttpGet]
        [Route("{id}")]

        public IActionResult GetUserById([FromRoute] int id)
        {
            var user = dbContext.Users.Find(id);  
            //Find method only works with primary key so for other fields use FirstOrDefault
            //var user = dbContext.Users.FirstOrDefault(u => u.Id == id);

            var userDomain = dbContext.Users.FirstOrDefault(u => u.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            //Map/Convert User Domain model to UserDTO

            var userDto = new UsersDTO
            {
                UserID = userDomain.UserID,
                UserName = userDomain.UserName,
                EmailID = userDomain.EmailID
            };
            return Ok(userDto);
        }

        //POST: To create a new user
        [HttpPost]
        
        public IActionResult CreateUser([FromBody] AddUserRequestDTO userDto)
        {
            //Map/Convert UserDTO to User Domain model
            var userDomain = new Users
            {
                UserName = userDto.UserName,
                EmailID = userDto.EmailID
            };

            //Use Domain model to create & save data to database
            dbContext.Users.Add(userDomain);
            dbContext.SaveChanges();

            //Mapping Domain model back to DTO
            var newuserDto = new AddUserRequestDTO
            {
                UserID = userDomain.UserID,
                UserName = userDomain.UserName,
                EmailID = userDomain.EmailID
            };

            return CreatedAtAction(nameof(GetUserById), new { id = newuserDto.UserID }, newuserDto);
        }

        //PUT: Update the resource
        [HttpPut]
        [Route("{id}")]

        public IActionResult UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequestDTO updateUserRequestDto)
        {
            var userDomain = dbContext.Users.FirstOrDefault(u => u.UserID == id);

            //Check if region exists
            if (userDomain == null)
            {
                return NotFound();
            }
            //Map DTO to Domin Model
            userDomain.UserName = updateUserRequestDto.UserName;
            userDomain.EmailID = updateUserRequestDto.EmailID;
           
            dbContext.SaveChanges();

            //Convert DomainModel to DTO
            var userDto = new UsersDTO
            {
                UserID = userDomain.UserID,
                EmailID = userDomain.EmailID
            };
            return Ok(userDto);
        }

        //DELETE: Delete a user
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteUser([FromRoute] int id)
        {
            var userDomain = dbContext.Users.FirstOrDefault(u => u.UserID == id);
            //Check if user exists
            if (userDomain == null)
            {
                return NotFound();
            }
            //Delete the user
            dbContext.Users.Remove(userDomain);
            dbContext.SaveChanges();
            //returning deleted user back after Converting DomainModel to DTO
            var userDto = new UsersDTO
            {
                UserID = userDomain.UserID,
                UserName = userDomain.UserName,
                EmailID = userDomain.EmailID
            };
            return Ok(userDto);
        }

    }
}
