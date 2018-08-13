using APDAspire.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APDAspire.Contact
{
    public interface IContact
    {
        Task<Guid> Create(ContactModel contactModel);

        Task<bool> Update(ContactModel contactModel);

        Task<bool> Delete(Guid id);

        Task<IEnumerable<ContactModel>> GetAll();

        Task<ContactModel> GetById(Guid id);

        Task<IEnumerable<ContactModel>> GetByName(string  firstName);
    }
}
