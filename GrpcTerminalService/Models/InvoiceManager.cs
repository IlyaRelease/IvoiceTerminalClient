using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTerminalService.Models
{
    /// <summary>
    /// Предоставляет доступ к функционалу для работы над заявками
    /// </summary>
    public class InvoiceManager : IInvoiceManager
    {
        public InvoiceManager()
        {
            this.dbHelper = new DbHelper();
            this.invoiceGroupes = new Dictionary<GroupName, InvoiceGroup>();

            foreach (GroupName group in Enum.GetValues(typeof(GroupName)))
            {
                InvoiceGroup invoiceGroup = new InvoiceGroup(group, dbHelper);
                invoiceGroup.InitNumber();

                invoiceGroupes.Add(group, invoiceGroup);
            }
        }

        Dictionary<GroupName, InvoiceGroup> invoiceGroupes;
        DbHelper dbHelper;

        /// <summary>
        /// Возвращает новый ID для указанной группы заявок
        /// </summary>
        public string GetNewClientNumber(GroupName groupe)
        {
            return invoiceGroupes[groupe].GetNewNumber();
        }

        /// <summary>
        /// Возвращает статус последней заявки с указанным номером
        /// </summary>
        public bool GetStatus(string number)
        {
            var result = dbHelper.QueryEntryByNumber<Invoice>(number);
            
            return result.Status;
        }

        /// <summary>
        /// Закрывает указанную заявку
        /// </summary>
        public bool PostStatus(string number)
        {
            var result = dbHelper.QueryEntryByNumber<Invoice>(number);

            if (result != null)
            {
                result.Status = true;
                dbHelper.UpdateEntry(result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Возвращяет список всех обращений указанной группы
        /// </summary>
        public List<Invoice> GetAllInvoiceByGroup(GroupName group)
        {
            return dbHelper.QueryEntryesByGroup<Invoice>(group);
        }
    }

}
