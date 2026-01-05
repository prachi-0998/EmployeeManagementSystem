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
using System;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IEmployeeRepository empRepository;
        private readonly IMapper mapper;
        private readonly ILogger<EmployeesController> _logger;


        public EmployeesController(EMSDbContext dbContext, IEmployeeRepository empRepository, IMapper mapper, ILogger<EmployeesController> logger)
        {
            this.dbContext = dbContext;
            this.empRepository = empRepository;
            this.mapper = mapper;
            this._logger = logger;
        }


        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns>List of all employees with their IDs, names, and active status</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, HR")]
        public async Task<ActionResult<List<EmployeesDTO>>> GetAllEmployeesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all employees");
                var empsDomain = await empRepository.GetAllEmployeesAsync();
                var empsDto = mapper.Map<List<EmployeesDTO>>(empsDomain);
                _logger.LogInformation("Successfully retrieved {Count} employees", empsDto.Count);
                return Ok(empsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all employees");
                throw;
            }
        }

        /// <summary>
        /// Get an employee by ID
        /// Admin/HR can view any employee. Employee/Manager can only view their own employee record.
        /// </summary>
        /// <param name="id">The unique identifier of the employee</param>
        /// <returns>Employee details including ID, name, and active status</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeesDTO>> GetEmployeeByIDAsync([FromRoute] int id)
        {
            try
            {
                // Get current user's ID from JWT token
                var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    _logger.LogWarning("Unable to extract user ID from token");
                    throw new UnauthorizedException("Invalid token");
                }

                _logger.LogInformation("Fetching employee with ID: {EmployeeId}", id);
                var emp = await empRepository.GetEmployeeByIDAsync(id);

                if (emp == null)
                {
                    _logger.LogWarning("Employee with ID: {EmployeeId} not found", id);
                    throw new NotFoundException("Employee", id);
                }

                // Check if user is Admin or HR
                var isAdminOrHR = User.IsInRole("Admin") || User.IsInRole("HR");

               
                if (!isAdminOrHR && emp.UserID != currentUserId)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to access employee {EmployeeId} without permission", 
                        currentUserId, id);
                    throw new UnauthorizedException("You can only view your own employee record");
                }

                var dto = mapper.Map<EmployeesDTO>(emp);
                _logger.LogInformation("Successfully retrieved employee with ID: {EmployeeId}", id);
                return Ok(dto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new employee
        /// </summary>

        [HttpPost]
        [Authorize(Roles = "Admin, HR")]
        public async Task<ActionResult<AddEmployeeRequestDTO>> CreateEmployeeAsync([FromBody] AddEmployeeRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for creating employee");
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Creating new employee: {FirstName} {LastName}", dto.FirstName, dto.LastName);
                var empDomain = mapper.Map<Employees>(dto);
                empDomain = await empRepository.CreateEmployeeAsync(empDomain);

                var newempDto = mapper.Map<EmployeesDTO>(empDomain);
                _logger.LogInformation("Successfully created employee with ID: {EmployeeId}", newempDto.EmployeeID);
                
                
                return Created($"/api/Employees/{newempDto.EmployeeID}", newempDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating employee");
                throw;
            }
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<ActionResult<EmployeesDTO>> UpdateEmployeeAsync([FromRoute] int id, [FromBody] UpdateEmployeeRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for updating employee with ID: {EmployeeId}", id);
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);
                var empDomain = mapper.Map<Employees>(dto);
                empDomain = await empRepository.UpdateEmployeeAsync(id, empDomain);

                if (empDomain == null)
                {
                    _logger.LogWarning("Employee with ID: {EmployeeId} not found for update", id);
                    throw new NotFoundException("Employee", id);
                }

                var empDto = mapper.Map<EmployeesDTO>(empDomain);
                _logger.LogInformation("Successfully updated employee with ID: {EmployeeId}", id);
                return Ok(empDto);
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
                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete an employee
        /// </summary>
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<ActionResult<EmployeesDTO>> DeleteEmployeeAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);
                var empDomain = await empRepository.DeleteEmployeeAsync(id);

                if (empDomain == null)
                {
                    _logger.LogWarning("Employee with ID: {EmployeeId} not found for deletion", id);
                    throw new NotFoundException("Employee", id);
                }

                var empDto = mapper.Map<EmployeesDTO>(empDomain);
                _logger.LogInformation("Successfully deleted employee with ID: {EmployeeId}", id);
                return Ok(empDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee with ID: {EmployeeId}", id);
                throw;
            }
        }
    }
}
