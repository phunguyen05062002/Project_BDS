using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.InterfaceRepostories
{
    public interface IContactRepository
    {
        Task<Contact> GetContactByIdAsync(int contactId);
        Task AddContactAsync(Contact contact);
        Task UpdateContactAsync(Contact contact);
        Task<List<Contact>> GetAllContactsAsync();
    }
}
