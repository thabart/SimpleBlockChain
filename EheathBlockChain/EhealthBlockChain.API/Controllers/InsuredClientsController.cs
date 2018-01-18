using EhealthBlockChain.API.Core.Repositories;
using EhealthBlockChain.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace EhealthBlockChain.API.Controllers
{
    [Route(Constants.RouteNames.InsuredClients)]
    public class InsuredClientsController : Controller
    {
        private readonly IInsuredClientsRepository _insuredClientsRepository;

        public InsuredClientsController(IInsuredClientsRepository insuredClientsRepository)
        {
            _insuredClientsRepository = insuredClientsRepository;
        }

        [HttpPost(Constants.ActionNames.Search)]
        public async Task<IActionResult> Search([FromBody] JObject jObj)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            var command = jObj.GetSearchInsuredClients();
            var result = await _insuredClientsRepository.Search(command);
            if (result == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(result.ToDto());
        }
    }
}
