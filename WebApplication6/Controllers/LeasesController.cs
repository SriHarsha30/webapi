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
    [ApiController]
    public class LeaseController : ControllerBase
    {
        private readonly ILeaseService _leaseService;

        public LeaseController(ILeaseService leaseService)
        {
            _leaseService = leaseService;
        }

        // POST: api/lease/tenant
        [HttpPost("applytenant")]
        [Authorize(Roles = "t")]
        public ActionResult CreateLease([FromBody] LeaseRequest leaseRequest)
        {
            try
            {
                // Call the service to create a lease
                var result = _leaseService.CreateLease(
                    leaseRequest.ID,
                    leaseRequest.PropertyId,
                    leaseRequest.StartDate,
                    leaseRequest.EndDate,
                    leaseRequest.Signature
                );

                // If validation fails
                if (result == null)
                {
                    return BadRequest("Tenant signature validation failed or lease already exists.");
                }

                // Success response
                return Ok($"Lease created successfully with Lease ID: {result?.leaseId}, Owner ID: {result?.ownerId}");
            }
            catch (KeyNotFoundException ex)
            {
                // For missing entities like Property or Tenant
                return NotFound($"Error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                // For invalid arguments like incorrect signature
                return BadRequest($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Generic error handler
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/lease/owner
        [HttpPost("ownerval")]
        [Authorize(Roles = "o")]
        public ActionResult FinalizeLease([FromBody] OwnerSignatureRequest ownerRequest)
        {
            try
            {
                // Call the service to finalize the lease
                var success = _leaseService.FinalizeLease(
                    ownerRequest.LeaseId,
                    ownerRequest.OwnerId,
                    ownerRequest.Signature
                );

                // If validation fails
                if (!success)
                {
                    return BadRequest("Owner signature validation failed or lease is already finalized.");
                }

                // Success response
                return Ok("Lease finalized successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                // For missing entities like Lease or Owner
                return NotFound($"Error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                // For invalid arguments like incorrect signature
                return BadRequest($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Generic error handler
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/lease/{id}
        [HttpGet("GetLeaseByLId/{id}")]
        [Authorize(Roles = "o,t")]
        public ActionResult<Lease> GetLeaseById(int id)
        {
            try
            {
                // Call the service to retrieve the lease
                var lease = _leaseService.GetLeaseById(id);

                if (lease == null)
                {
                    return NotFound("Lease not found.");
                }

                // Return lease details
                return Ok(lease);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/lease/owner/{ownerId}

        [HttpGet("ownersleases/{ownerId}")]
        [Authorize(Roles = "o,t")]
        public ActionResult<IEnumerable<Lease>> GetLeasesByOwner(string ownerId)
        {
            try
            {
                // Call the service to retrieve leases by owner
                var leases = _leaseService.GetLeasesByOwner(ownerId);

                if (!leases.Any())
                {
                    return NotFound("No leases found for this owner.");
                }

                // Return list of leases
                return Ok(leases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/lease
        [HttpGet]
        [Authorize(Roles = "o,t")]
        public ActionResult<IEnumerable<Lease>> GetAllLeases()
        {
            try
            {
                // Call the service to retrieve all leases
                var leases = _leaseService.GetAllLeases();

                // Return list of leases
                return Ok(leases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // DTOs
    public class LeaseRequest
    {
        public string ID { get; set; }
        public int PropertyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Signature { get; set; }
    }

    public class OwnerSignatureRequest
    {
        public int LeaseId { get; set; }
        public string OwnerId { get; set; }
        public string Signature { get; set; }
    }
}
