using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.RepositoryInterface;
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

        public UserRoleController(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserRolesAsync()
        {

           var userRolesDomain = await dbContext.UserRole.ToListAsync();

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

        [HttpPost]
        public IActionResult AssignRole([FromBody] AddUserRoleRequestDTO dto)
        {
            var ur = new UserRole
            {
                UserID = dto.UserID,
                RoleID = dto.RoleID,
                IsActive = true
            };

            dbContext.UserRole.Add(ur);
            dbContext.SaveChanges();

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveUserRole([FromRoute] int id)
        {
            var ur = dbContext.UserRole.FirstOrDefault(x => x.UserRoleID == id);
            if (ur == null)
                return NotFound();

            dbContext.UserRole.Remove(ur);
            dbContext.SaveChanges();

            return Ok();
        }
    }
}
