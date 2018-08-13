using System;
using System.Linq;
using System.Threading.Tasks;
using APDAspire.Contact;
using APDAspire.Model;
using Microsoft.AspNetCore.Mvc;

namespace APDAspire.ContactServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContact contactStore;

        public ContactController(IContact contact)
        {
            this.contactStore = contact ?? throw new ArgumentNullException("contact");
        }

        [HttpPut]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] ContactModel contactModel)
        {
            try
            {
                if (contactModel == null)
                    return BadRequest();

                if (string.IsNullOrEmpty(contactModel.FirstName))
                    return BadRequest();

                var status = await this.contactStore.Create(contactModel);

                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("all", Name = "GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await this.contactStore.GetAll();

                if (!result.Any())
                    return NotFound();

                return Ok(result);
            }            
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{uid}", Name = "GetById")]
        public async Task<IActionResult> GetById(string uid)
        {
            try
            {
                if (!Guid.TryParse(uid, out Guid contactiD))
                    return BadRequest();

                var result = await this.contactStore.GetById(contactiD);

                if (result == null)
                    return NotFound(contactiD);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("firstName/{name}", Name = "GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest();
                var searchtext = name.Trim();
                if (string.IsNullOrEmpty(searchtext))
                    return BadRequest();
                var result = await this.contactStore.GetByName(searchtext);
                if (result == null || !result.Any())        
                    return NotFound(searchtext);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{uid}")]
        public async Task<IActionResult> Delete(string uid)
        {
            try
            {
                if (!Guid.TryParse(uid, out Guid contact_id))
                    return BadRequest();

                var result = await this.contactStore.Delete(contact_id);

                if (!result)
                {
                    return NotFound(contact_id);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] ContactModel contactModel)
        {
            try
            {
                if (contactModel == null)
                    return BadRequest();

                if (contactModel.Contact_Id == Guid.Empty)
                    return BadRequest();

                var result = await this.contactStore.Update(contactModel);

                if (!result)
                     return NotFound(contactModel.Contact_Id);

                return Ok(contactModel.Contact_Id);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
