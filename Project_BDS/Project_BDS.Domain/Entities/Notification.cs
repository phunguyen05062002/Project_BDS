using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int CreateId { get; set; }
        public User? Create { get; set; }
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public NotificationStatus? Status { get; set; }
        public string? Type {  get; set; }
        public DateTime? ExpiredDateTime {  get; set; }
        public string? Code { get; set; }
        public int? ContactId {  get; set; }
        public Contact? Contact { get; set; }
    }
}
