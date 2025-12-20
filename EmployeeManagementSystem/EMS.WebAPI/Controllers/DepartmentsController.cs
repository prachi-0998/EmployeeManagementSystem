using AutoMapper;
using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
using EMS.Domain.Repository;
using EMS.Infra.Data;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class DepartmentsController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IDepartmentRepository deptRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DepartmentsController> _logger;


        public DepartmentsController(EMSDbContext dbContext, IDepartmentRepository deptRepository, IMapper mapper, ILogger<DepartmentsController> logger)
        {
            this.dbContext = dbContext;
            this.deptRepository = deptRepository;
            this.mapper = mapper;
            this._logger = logger;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<ActionResult<List<DepartmentsDTO>>> GetAllDepartmentsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all departments");
                var deptsDomain = await deptRepository.GetAllDepartmentsAsync();
                var deptsDto = mapper.Map<List<DepartmentsDTO>>(deptsDomain);
                _logger.LogInformation("Successfully retrieved {Count} departments", deptsDto.Count);
                return Ok(deptsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all departments");
                throw;
            }
        }

        // GET: api/departments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentsDTO>> GetDepartmentByIDAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Fetching department with ID: {DepartmentId}", id);
                var department = await deptRepository.GetDepartmentByIDAsync(id);

                if (department == null)
                {
                    _logger.LogWarning("Department with ID: {DepartmentId} not found", id);
                    throw new NotFoundException("Department", id);
                }

                var dto = mapper.Map<DepartmentsDTO>(department);
                _logger.LogInformation("Successfully retrieved department with ID: {DepartmentId}", id);
                return Ok(dto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching department with ID: {DepartmentId}", id);
                throw;
            }
        }

        // POST: api/departments
        [HttpPost]
        public async Task<ActionResult<AddDepartmentRequestDTO>> CreateDepartmentAsync([FromBody] AddDepartmentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for creating department");
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Creating new department: {DepartmentName}", dto.DepartmentName);
                var deptDomain = mapper.Map<Departments>(dto);
                deptDomain = await deptRepository.CreateDepartmentAsync(deptDomain);

                var newDeptDto = mapper.Map<DepartmentsDTO>(deptDomain);
                _logger.LogInformation("Successfully created department with ID: {DepartmentId}", newDeptDto.DepartmentID);
                return CreatedAtAction(nameof(GetDepartmentByIDAsync), new { id = newDeptDto.DepartmentID }, newDeptDto);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating department");
                throw;
            }
        }

        // PUT: api/departments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<DepartmentsDTO>> UpdateDepartmentAsync([FromRoute] int id, [FromBody] UpdateDepartmentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for updating department with ID: {DepartmentId}", id);
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Updating department with ID: {DepartmentId}", id);
                var deptDomain = mapper.Map<Departments>(dto);
                deptDomain = await deptRepository.UpdateDepartmentAsync(id, deptDomain);

                if (deptDomain == null)
                {
                    _logger.LogWarning("Department with ID: {DepartmentId} not found for update", id);
                    throw new NotFoundException("Department", id);
                }

                var deptDto = mapper.Map<DepartmentsDTO>(deptDomain);
                _logger.LogInformation("Successfully updated department with ID: {DepartmentId}", id);
                return Ok(deptDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department with ID: {DepartmentId}", id);
                throw;
            }
        }

        // DELETE: api/departments/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<DepartmentsDTO>> DeleteDepartmentAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting department with ID: {DepartmentId}", id);
                var deptDomain = await deptRepository.DeleteDepartmentAsync(id);
                
                if (deptDomain == null)
                {
                    _logger.LogWarning("Department with ID: {DepartmentId} not found for deletion", id);
                    throw new NotFoundException("Department", id);
                }

                var deptDto = mapper.Map<DepartmentsDTO>(deptDomain);
                _logger.LogInformation("Successfully deleted department with ID: {DepartmentId}", id);
                return Ok(deptDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department with ID: {DepartmentId}", id);
                throw;
            }
        }
    }
}
