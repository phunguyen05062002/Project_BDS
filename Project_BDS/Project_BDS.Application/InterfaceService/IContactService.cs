using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.InterfaceService
{
    public interface IContactService
    {
        Task<ResponseObject<Contact>> SendContactAsync(ContactRequest contactRequest);
        Task<ResponseObject<List<Contact>>> GetContactsForRoleId3Async();
    }
}
