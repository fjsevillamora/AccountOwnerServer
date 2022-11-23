using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountOwnerServer.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public EmployeeController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                // var employees = await _repository.Employee.GetAllEmployeesAsync();
                // _logger.LogInfo("Returned all employees from the database");

                // var employeesResult = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
                // return Ok(employeesResult);
                return Ok("Return all employees");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllEmployeesAsync action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "EmployeeById")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            try
            {
                var employee = await _repository.Employee.GetEmployeeByIdAsync(id);
                if (employee is null)
                {
                    _logger.LogError($"Employee with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned employee with id: {id}");
                    var employeeResult = _mapper.Map<EmployeeDto>(employee);
                    return Ok(employeeResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside EmployeeById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeForCreationDto employee)
        {
            try
            {
                if (employee is null)
                {
                    _logger.LogError("Employee object sent from client is null.");
                    return BadRequest("Employee object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Employee object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var employeeEntity = _mapper.Map<Employee>(employee);

                _repository.Employee.CreateEmployee(employeeEntity);
                await _repository.SaveAsync();

                var createdEmployee = _mapper.Map<EmployeeDto>(employeeEntity);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.EmployeeId }, createdEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEmployee action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            try
            {
                if (employee is null)
                {
                    _logger.LogError("Employee object sent from client is null.");
                    return BadRequest("Employee object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Employee object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var employeeEntity = await _repository.Employee.GetEmployeeByIdAsync(id);
                if (employeeEntity is null)
                {
                    _logger.LogError($"Employee with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _mapper.Map(employee, employeeEntity);
                _repository.Employee.UpdateEmployee(employeeEntity);
                await _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateEmployee action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var employee = await _repository.Employee.GetEmployeeByIdAsync(id);
                if (employee is null)
                {
                    _logger.LogError($"Employee with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Employee.DeleteEmployee(employee);
                await _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEmployee action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
