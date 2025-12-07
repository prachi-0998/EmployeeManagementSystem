using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IUserRoleRepository urRepository;

        public UserRoleController(EMSDbContext dbContext, IUserRoleRepository urRepository)
        {
            this.dbContext = dbContext;
            this.urRepository = urRepository;

        }

        [HttpGet]
        public async Task<ActionResult<List<UserRoleDTO>>> GetAllUserRoleAsync()
        {
            var userRolesDomain = await urRepository.GetAllUserRoleAsync();

            //Map Domain Models to DTOs
            var userRolesDto = new List<UserRoleDTO>();

            //Return DTOs back to client
            foreach (var userroleDomain in userRolesDomain)
            {
                var userRoleDto = new UserRoleDTO
                {

                    UserRoleID = userroleDomain.UserRoleID,
                    UserID = userroleDomain.UserID,
                    RoleID = userroleDomain.RoleID,
                    IsActive = userroleDomain.IsActive

                };
                userRolesDto.Add(userRoleDto);
            }

           
            return Ok(userRolesDto);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoleDTO>> GetUserRoleByIDAsync([FromRoute] int id)
        {
            var userRole = await urRepository.GetUserRoleByIDAsync(id);

            if (userRole == null)
                return NotFound();

            var dto = new UserRoleDTO
            {
                UserRoleID = userRole.UserRoleID,
                UserID = userRole.UserID,
                RoleID = userRole.RoleID,   
                IsActive = userRole.IsActive
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<AddUserRoleRequestDTO>> CreateUserRoleAsync([FromBody] AddUserRoleRequestDTO dto)
        {

            var urDomain = new UserRole
            {
                UserID = dto.UserID,
                RoleID = dto.RoleID,
                IsActive = dto.IsActive
            };
            
            urDomain = await urRepository.CreateUserRoleAsync(urDomain);

            //Mapping Domain model back to DTO
            var newurDto = new AddUserRoleRequestDTO
            {
                UserID = urDomain.UserID,
                RoleID = urDomain.RoleID,
                IsActive = urDomain.IsActive

            };

            return CreatedAtAction(nameof(GetUserRoleByIDAsync), new { id = newurDto.UserID }, newurDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserRoleDTO>> UpdateUserRoleAsync([FromRoute] int id, [FromBody] AddUserRoleRequestDTO dto)
        {
            var urDomain = new UserRole
            {
                UserID = dto.UserID,
                RoleID = dto.RoleID,
                IsActive = dto.IsActive
            };
            
            urDomain = await urRepository.UpdateUserRoleAsync(id, urDomain);

            if (urDomain == null)
            {
                return NotFound();
            }

            //Convert DomainModel to DTO
            var urDto = new UserRoleDTO
            {
                UserRoleID = urDomain.UserRoleID,
                UserID = urDomain.UserID,
                RoleID = urDomain.RoleID,
                IsActive = urDomain.IsActive
            };
            return Ok(urDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserRoleDTO>> DeleteUserRoleAsync([FromRoute] int id)
        {
            var urDomain = await urRepository.DeleteUserRoleAsync(id);

            if (urDomain == null)
            {
                return NotFound();
            }


            var urDto = new UserRoleDTO
            {
                UserRoleID = urDomain.UserRoleID,
                UserID = urDomain.UserID,
                RoleID = urDomain.RoleID,

            };
            return Ok(urDto);
        }
    }
}
