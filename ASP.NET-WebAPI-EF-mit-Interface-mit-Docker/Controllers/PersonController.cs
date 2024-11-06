using Microsoft.AspNetCore.Mvc;
using PersonApi.Data;
using PersonApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Alle Personen abrufen.
        /// </summary>
        /// <returns>Liste von Personen</returns>
        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get()
        {
            var persons = await _personService.GetAllPersonsAsync();
            return Ok(persons);
        }

        /// <summary>
        /// Eine spezifische Person anhand der ID abrufen.
        /// </summary>
        /// <returns>Person-Objekt</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        /// <summary>
        /// Neue Person hinzufügen.
        /// </summary>
        /// <param name="person">Person-Daten</param>
        /// <returns>Resultat der Operation</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest("Personendaten sind ungültig.");
            }

            await _personService.AddPersonAsync(person);
            return CreatedAtAction(nameof(GetById), new { id = person.PersonId }, person);
        }

        /// <summary>
        /// Vorhandene Person aktualisieren.
        /// </summary>
        /// <param name="id">ID der Person</param>
        /// <param name="person">Aktualisierte Person-Daten</param>
        /// <returns>Resultat der Operation</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Person person)
        {
            if (person == null || person.PersonId != id)
            {
                return BadRequest();
            }

            var existingPerson = await _personService.GetPersonByIdAsync(id);
            if (existingPerson == null)
            {
                return NotFound();
            }

            await _personService.UpdatePersonAsync(person);
            return NoContent();
        }

        /// <summary>
        /// Person löschen.
        /// </summary>
        /// <param name="id">ID der zu löschenden Person</param>
        /// <returns>Resultat der Operation</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingPerson = await _personService.GetPersonByIdAsync(id);
            if (existingPerson == null)
            {
                return NotFound();
            }

            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
    }
}
