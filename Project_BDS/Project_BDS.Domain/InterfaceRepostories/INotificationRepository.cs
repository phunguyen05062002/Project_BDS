using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.InterfaceRepostories
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
    }
}
