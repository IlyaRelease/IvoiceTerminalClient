using System.Collections.Generic;

namespace GrpcTerminalService.Models
{
    public interface IInvoiceManager
    {
        string GetNewClientNumber(GroupName groupe);
        bool GetStatus(string id);
        bool PostStatus(string id);
        List<Invoice> GetAllInvoiceByGroup(GroupName group);
    }

}
