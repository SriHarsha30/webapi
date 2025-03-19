using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Models;
using WebApplication6.Repository;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryRepository _historyRepository;

        public HistoryController(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        [HttpGet("tenant-history/{tenantId}")]
        [Authorize(Roles = "t")]
        public ActionResult<IEnumerable<History>> GetTenantHistory(string tenantId)
        {
            var history = _historyRepository.GetTenantHistory(tenantId);
            if (history == null || !history.Any())
            {
                return NotFound("Tenant history not found.");
            }
            return Ok(history);
        }

        [HttpGet("owner-tenant-history/{tenantId}")]
        [Authorize(Roles = "o")]
        public ActionResult<IEnumerable<dynamic>> GetTenantHistoryForOwner(string tenantId)
        {
            var history = _historyRepository.GetTenantHistoryForOwner(tenantId);
            if (history == null || !history.Any())
            {
                return NotFound("Tenant history for owner not found.");
            }
            return Ok(history);
        }
    }
}