using ContactListApp.DTOs;
using ContactListApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace ContactListApp.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public class ContactsListController : ControllerBase
    {
        private readonly ContactsContext _context;

        public ContactsListController(ContactsContext context)
        {
            _context = context;
        }

        // Get a list of all contacts 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactsListItemDTO>>> GetContacts()
        {
            // take all contats and convert to ContactsListItemDTOs
            var contacts = await _context.ContactItems
                .Select(c => new ContactsListItemDTO(c.Id, c.FirstName, c.LastName, c.CategoryId))
                .ToListAsync();

            // return list of contats and Http 200 response code
            return Ok(contacts);
        }

        // GetContactItem - get specific contact with details by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDetailsDTO>> GetContactItem(long id)
        {
            // find contact item in the database
            var contactItem = await _context.ContactItems.FindAsync(id);

            // if nor found, return NotFound
            if (contactItem == null) {
                return NotFound();
            }

            // create DTO from contactItem
            var dto = new ContactDetailsDTO
            {
                Id = contactItem.Id,
                FirstName = contactItem.FirstName,
                LastName = contactItem.LastName,
                Email = contactItem.Email,
                Phone = contactItem.PhoneNumber,
                CategoryId = contactItem.CategoryId,
                SubcategoryId = contactItem.SubcategoryId,
                CustomSubcategory = contactItem.CustomSubcategory,
                BirthDate = contactItem.BirthDate,
                UserId = contactItem.UserId,
            };

            // if found, return the item
            return dto;
        }

        // PostContactItem - post a new ContactItem
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ContactItem>> PostContactItem(CreateContactDTO item)
        {
            // get logged in user's Id
            var loggedInUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            // verify if the email is unique 
            var emailExists = await _context.ContactItems.AnyAsync(x => x.Email == item.Email);
            if (emailExists)
            {
                return BadRequest("Contact with this email already exists.");
            }

            // mapping DTO to entity object (ContactItem)
            var newContact = new ContactItem
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                PhoneNumber = item.Phone,
                CategoryId = item.CategoryId,
                SubcategoryId = item.SubcategoryId,
                BirthDate = item.BirthDate,
                CustomSubcategory = item.CustomSubcategory,
                UserId = loggedInUserId,

                PasswordHash = string.Empty,
            };

            // add new ContactItem and save
            _context.ContactItems.Add(newContact);
            await _context.SaveChangesAsync();

            // return HTTP 201 status code and the contact
            return CreatedAtAction(nameof(GetContactItem), new { id = newContact.Id }, newContact);
        }

        // PutContactItem - update selected contact item
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutContactItem(long id, PutContactDTO item)
        {
            // ensure the id from URL is equal to the Id from DTO
            if (id != item.Id) return BadRequest("ID mismatch.");

            // try to get the contact and check if the contact already exists 
            var contact = await _context.ContactItems.FindAsync(item.Id);
            if (contact == null) return BadRequest("This user doesn't exist.");

            // get logged in user's Id
            var loggedInUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            // check if the user is owner of the contact, if not - return 403 Forbidden response
            if (contact.UserId != loggedInUserId) return Forbid();

            // verify if the email is unique 
            var emailExists = await _context.ContactItems.AnyAsync(x => x.Email == item.Email && x.Id != item.Id);
            if (emailExists) return BadRequest("User with this email already exists.");

            // update the contact entity object (ContactItem)
            contact.FirstName = item.FirstName;
            contact.LastName = item.LastName;
            contact.Email = item.Email;
            contact.PhoneNumber = item.Phone;
            contact.CategoryId = item.CategoryId;
            contact.SubcategoryId = item.SubcategoryId;
            contact.BirthDate = item.BirthDate;
            contact.CustomSubcategory = item.CustomSubcategory;
           

            // save changes in the db
            await _context.SaveChangesAsync();

            // return HTTP 204 status code (NoContent)
            return NoContent();
        }

        // Delete specific contact by Id
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContact(long id)
        {
            // find contact
            var contact = await _context.ContactItems.FindAsync(id);
            if (contact == null) return NotFound(); // if not found, return NotFound

            // get logged in user's Id
            var loggedInUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            // check if the user is owner of the contact, if not - return 403 Forbidden response
            if (contact.UserId != loggedInUserId) return Forbid();

            // remove contact from db
            _context.ContactItems.Remove(contact);
            await _context.SaveChangesAsync();

            // return response code
            return NoContent();
        }
    }
}
