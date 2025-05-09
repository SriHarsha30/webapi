﻿using System;
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
            try
            {
                var maintainances = service.ViewOwnerRequests(userId);

                if (maintainances == null || maintainances.Count == 0)
                {
                    return NotFound("No maintenance requests found for the owner.");
                }

                return Ok(maintainances);
            }
            catch (Exception ex)
            {
                // Log the exception details
                //_logger.LogError(ex, "Error viewing owner requests");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("tenant/{userId}")]
        [Authorize(Roles = "t")]
        public ActionResult<List<Maintainance>> ViewTenantRequests(string userId)
        {
            try
            {
                var maintainances = service.ViewTenantRequests(userId);

                if (maintainances == null || maintainances.Count == 0)
                {
                    return NotFound("No maintenance requests found for the tenant.");
                }

                return Ok(maintainances);
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Example: _logger.LogError(ex, "Error viewing tenant requests");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "t")]
        public ActionResult InsertMaintainance(int PropertyId, string TenantId, string Description,string Status,string ImagePath)
        {
            try
            {
                service.InsertMaintainance(
                    PropertyId,
                    TenantId,
                    Description,
                    Status,
                    ImagePath
                );
                return StatusCode(201, "Maintenance request inserted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Example: _logger.LogError(ex, "Error inserting maintenance request");
                return StatusCode(500, $"An error occurred: {ex.ToString()}");
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

        [HttpGet("all_owner/{userId}")]
        [Authorize(Roles = "o")]
        public ActionResult<List<Maintainance>> ViewallOwnerRequests(string userId)
        {
            try
            {
                var maintainances = service.ViewallOwnerRequests(userId);

                if (maintainances == null || maintainances.Count == 0)
                {
                    return NotFound("No maintenance requests found for the owner.");
                }

                return Ok(maintainances);
            }
            catch (Exception ex)
            {
                // Log the exception details
                //_logger.LogError(ex, "Error viewing owner requests");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }

}
