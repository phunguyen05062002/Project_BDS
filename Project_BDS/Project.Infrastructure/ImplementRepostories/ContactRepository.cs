using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.DataContexts;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.ImplementRepostories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;
        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Contact> GetContactByIdAsync(int contactId)
        {
            return await _context.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);
        }

        public async Task AddContactAsync(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateContactAsync(Contact contact)
        {
            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Contact>> GetAllContactsAsync()
        {
            return await _context.Contacts.ToListAsync();
        }

    }
}
