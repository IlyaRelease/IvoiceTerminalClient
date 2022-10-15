using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTerminalService.Models
{
    /// <summary>
    /// Представляет Данные о обращении клиента
    /// </summary>
    public class Invoice : INumber, IDate, IId, IGroup
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public GroupName Group { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
    }
}