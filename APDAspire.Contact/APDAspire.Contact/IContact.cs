using APDAspire.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APDAspire.Contact
{
    public interface IContact
    {
        Task<Guid> Create(ContactDto contactModel);

        Task<bool> Update(ContactDto contactModel);

        Task<bool> Delete(Guid id);

        Task<IEnumerable<ContactDto>> GetAll();

        Task<ContactDto> GetById(Guid id);

        Task<IEnumerable<ContactDto>> GetByName(string  firstName);
    }
}
