using APDAspire.Contact;
using APDAspire.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APDAspire.ContactStore
{
    public class ContactStore : IContact
    {
        private ContactContext context;

        public ContactStore(IOptions<APDAspire.ContactStore.Setting> settings)
        {
            context = new ContactContext(settings);
        }

        public async  Task<Guid> Create(ContactDto contactModel)
        {
            try
            {
                if (contactModel == null)
                    return Guid.Empty;

                if (string.IsNullOrEmpty(contactModel.FirstName))
                    return Guid.Empty;

                await context.ContactData.InsertOneAsync(contactModel);

                return contactModel.Contact_Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public  async Task<bool> Delete(Guid contact_Id)
        {
            try
            {
                if (contact_Id == Guid.Empty)
                    return false;

                var result = await context.ContactData.DeleteOneAsync(item => item.Contact_Id == contact_Id);

                if (result.DeletedCount <= 0) return false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ContactDto> GetById(Guid contact_Id)
        {
            try
            {
                if (contact_Id == Guid.Empty)
                    return null ;

                return await context.ContactData.FindAsync(item => item.Contact_Id == contact_Id).Result.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ContactDto>> GetByName(string firstName)
        {
            try
            {
                if (string.IsNullOrEmpty(firstName))
                    return null;

                return await context.ContactData.FindAsync(item => item.FirstName.Contains(firstName)).Result.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ContactDto>> GetAll()
        {
            try
            {
                return await context.ContactData.Find(Builders<ContactDto>.Filter.Empty).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> Update(ContactDto contactModel)
        {
            try
            {
                if (contactModel.Contact_Id == Guid.Empty)
                    return false;

                var result = await context.ContactData.ReplaceOneAsync(n => n.Contact_Id.Equals(contactModel.Contact_Id), contactModel);
                if (result.ModifiedCount < 1 )
                    return false;
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
