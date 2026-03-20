using System.Threading.Tasks;
using Desafio.Umbler.Services;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Umbler.Controllers
{
    [Route("api")]
    public class DomainController : Controller
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet, Route("domain/{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName) || !domainName.Contains("."))
                return BadRequest("Domínio inválido.");

            var result = await _domainService.GetDomainInfoAsync(domainName);
            return Ok(result);
        }
    }
}