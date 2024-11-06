using Microsoft.AspNetCore.Mvc;
using PersonApi.Services;
using PersonApi.Models;

namespace PersonApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase {

        private readonly PersonService _personService = new PersonService();

        [HttpGet]
        public ActionResult<List<Person>> Get() {
            return _personService.GetPersons();
        }
    }
}
