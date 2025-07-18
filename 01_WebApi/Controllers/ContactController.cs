using Application.Service.Interface;
using Application.ViewModel;
using Core.Entity;
using Core.Message.Command;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Extensions;

namespace WebApi.Controllers;
[Route("api/v1/contacts")]
[ApiController]
public class ContactController : ControllerBase
{
    // Controller de contatos 
    private readonly IContactService _contactService;
    private readonly IMemoryCache _cache;

    public ContactController(IContactService contactService, IMemoryCache cache)
    {
        _contactService = contactService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<List<Contact>>> GetAllAsync()
    {
        try
        {
            var contacts = await _contactService.GetAllAsync();

            return Ok(new ResultViewModel<IList<Contact>>(contacts));
        }
        catch (SystemException)
        {
            // 01X01 é um código único qualquer que facilita identificar onde o erro foi gerado (Uma boa prática)
            return StatusCode(500, new ResultViewModel<IList<Contact>>("01X01 - Internal server error")); 
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Contact>> GetByIdAsync(int id)
    {
        try
        {
            var contact = await _contactService.GetByIdAsync(id);

            if (contact is null)
                return NoContent();

            return Ok(new ResultViewModel<Contact>(contact));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Contact>("01X02- Internal server error"));
        }
    }

    [HttpGet("ddd-code/{id:int}")]
    public async Task<ActionResult<List<Contact>>> GetAllByDddAsync(int id)
    {
        try
        {
            var cacheKey = $"ContactsByDDDCodeCache_{id}";
            
            var contacts = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _contactService.GetAllByDddAsync(id);
            });
                        
            return Ok(new ResultViewModel<IList<Contact>>(contacts));
        }
        catch (ArgumentException)
        {
            return BadRequest(new ResultViewModel<Contact>("01X03 - Invalid direct distance dialing code"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<Contact>("01X04- Internal server error"));
        }
    }

    [HttpGet("persistence-error-test/{fail:bool}")]
    public async Task<IActionResult> PersistanceApiErrorTest(bool fail)
    {
        var result = await _contactService.ResilienceTest(fail);
        return Ok(new ResultViewModel<string>(result));
    }

    [HttpPost()]
    public async Task<IActionResult> PostAsync([FromBody] ContactViewModel model)
    {
        try
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(new ResultViewModel<Contact>(ModelState.GetErrors()));
            }            

            var command = new CreateContactCommand(model.Name, model.Phone, model.Email, model.DddId);

            await _contactService.CreateAsync(command);
            
            return Accepted(new ResultViewModel<CreateContactCommand>(command));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<Contact>("01X05 - Internal server error"));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] ContactViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<Contact>(ModelState.GetErrors()));
            }

            var contact = await _contactService.GetByIdAsync(id);
            
            if (contact is null)
                return BadRequest(new ResultViewModel<Contact>("01X06 - Invalid contact id"));


            var command = new EditContactCommand(contact.Id, model.Name, model.Phone, model.Email, model.DddId);

            await _contactService.EditAsync(command);

            return Accepted(new ResultViewModel<EditContactCommand>(command));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<Contact>("01X07 - Internal server error"));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        try
        {
            var contact = await _contactService.GetByIdAsync(id);

            if (contact is null)
                return BadRequest(new ResultViewModel<Contact>("01X08 - Invalid contact id"));

            var command = new DeleteContactCommand(contact.Id);

            await _contactService.DeleteAsync(command);

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<Contact>("01X09 - Internal server error"));
        }
    }
}
