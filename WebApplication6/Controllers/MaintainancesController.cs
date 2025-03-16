using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Services;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [Authorize]

    [ApiController]

    public class MaintainancesController : ControllerBase

    {

        private readonly IMaintainanceService service;

        public MaintainancesController(IMaintainanceService service)

        {

            this.service = service;

        }

        [HttpGet("owner/{userId}")]
        [Authorize(Roles = "o")]
        public ActionResult<List<Maintainance>> ViewOwnerRequests(string userId)

        {

            var maintainances = service.ViewOwnerRequests(userId);

            if (maintainances == null || maintainances.Count == 0)

            {

                return NotFound("No maintenance requests found for the owner.");

            }

            return Ok(maintainances);

        }

        [HttpGet("tenant/{userId}")]
        [Authorize(Roles = "t")]
        public ActionResult<List<Maintainance>> ViewTenantRequests(string userId)

        {

            var maintainances = service.ViewTenantRequests(userId);

            if (maintainances == null || maintainances.Count == 0)

            {

                return NotFound("No maintenance requests found for the tenant.");

            }

            return Ok(maintainances);

        }

        [HttpPost]
        [Authorize(Roles = "t")]
        public ActionResult InsertMaintainance([FromBody] Maintainance maintainance)

        {

            try

            {

                service.InsertMaintainance(

                    maintainance.RequestId,

                    maintainance.PropertyId,

                    maintainance.TenantId,

                    maintainance.Description,

                    maintainance.Status.ToString(), // Convert enum to string if necessary

                    maintainance.ImagePath

                );

                return StatusCode(201, "Maintenance request inserted successfully.");

            }

            catch (Exception ex)

            {

                // Handle exception (log it, return error response, etc.)

                return StatusCode(500, $"An error occurred: {ex.Message}");

            }

        }

        [HttpPut("{requestId}")]
        [Authorize(Roles = "o")]
        public ActionResult UpdateStatus(int requestId, [FromBody] string newStatus)

        {

            var result = service.UpdateStatus(requestId, newStatus);

            if (!result)

            {

                return NotFound("Maintenance request not found.");

            }

            return NoContent();

        }

    }

}
