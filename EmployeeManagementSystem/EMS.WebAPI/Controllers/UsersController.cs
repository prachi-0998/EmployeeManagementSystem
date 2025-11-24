using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Repository;
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
        private readonly IUserRepository userRepository;


        public UsersController(EMSDbContext dbContext, IUserRepository userRepository)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;

        }

        //GET: Get all users
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            //Get data from database - Domain Models(using DbContext)
           // var usersDomain = await dbContext.Users.ToListAsync();
            //Get data using Repository Pattern
            var usersDomain = await userRepository.GetAllUsersAsync();

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

        public async Task<IActionResult> GetUserByIdAsync([FromRoute] int id)
        {
            // Replace this line in GetUserById method:
            // var user = dbContext.Users.Find(id);  //Find method only works with primary key so for other fields use FirstOrDefault

            // var user = await dbContext.Users.FindAsync(id);  // Use async version for await

            // var userDomain = await dbContext.Users.FirstOrDefaultAsync(u => u.UserID == id);

            //Get data using Repository Pattern
            var userDomain = await userRepository.GetUserByIDAsync(id);

            if (userDomain == null)
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
        public async Task<IActionResult> CreateUserAsync([FromBody] AddUserRequestDTO userDto)
        {
            //Map/Convert UserDTO to User Domain model
            var userDomain = new Users
            {
                UserName = userDto.UserName,
                EmailID = userDto.EmailID
            };

            //Use Domain model to create & save data to database
            // dbContext.Users.Add(userDomain);
            //await dbContext.SaveChangesAsync();

            //Use Repository Pattern to create user
            userDomain = await userRepository.CreateUserAsync(userDomain);

            //Mapping Domain model back to DTO
            var newuserDto = new AddUserRequestDTO
            {
                UserID = userDomain.UserID,
                UserName = userDomain.UserName,
                EmailID = userDomain.EmailID
            };

            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = newuserDto.UserID }, newuserDto);
        }

        //PUT: Update the resource
        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult> UpdateUserAsync([FromRoute] int id, [FromBody] UpdateUserRequestDTO updateUserRequestDto)
        {
            //Map DTO to Domin Model
            var userDomain = new Users
            {
                UserName = updateUserRequestDto.UserName,
                EmailID = updateUserRequestDto.EmailID,
            };
            //var userDomain = await dbContext.Users.FirstOrDefaultAsync(u => u.UserID == id);
            userDomain = await userRepository.UpdateUserAsync(id, userDomain);
           
           if (userDomain == null) {    
                return NotFound();
            }

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
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
        {
            var userDomain = await userRepository.DeleteAsync(id);
            //Check if user exists
            if (userDomain == null)
            {
                return NotFound();
            }
            
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
