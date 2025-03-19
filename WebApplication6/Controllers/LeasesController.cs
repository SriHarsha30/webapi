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
        public ActionResult CreateLease(string tenantId, int propertyId, DateTime startDate, DateTime endDate, string signature)
        {
            try
            {
                var result = _leaseService.CreateLease(tenantId, propertyId, startDate, endDate, signature);

                if (result == null)
                {
                    return BadRequest("Tenant signature validation failed or lease already exists.");
                }

                return Ok($"Lease created successfully with Lease ID: {result?.leaseId}, Owner ID: {result?.ownerId}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"error: {ex.Message}");
            }
        }

        // POST: api/lease/owner
        [HttpPost("ownerval")]
        [Authorize(Roles = "o")]
        public ActionResult FinalizeLease(int leaseId, string ownerId, string signature)
        {
            try
            {
                var success = _leaseService.FinalizeLease(leaseId, ownerId, signature);

                if (!success)
                {
                    return BadRequest("Owner signature validation failed or lease is already finalized.");
                }

                return Ok("Lease finalized successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
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
                var lease = _leaseService.GetLeaseById(id);

                if (lease == null)
                {
                    return NotFound("Lease not found.");
                }

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
                var leases = _leaseService.GetLeasesByOwner(ownerId);

                if (!leases.Any())
                {
                    return NotFound("No leases found for this owner.");
                }

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
                var leases = _leaseService.GetAllLeases();

                return Ok(leases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
