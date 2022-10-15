using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTerminalService.Models
{
    /// <summary>
    /// Представляет одну из групп обращений клиентов
    /// </summary>
    public class InvoiceGroup
    {
        public InvoiceGroup(GroupName groupeName, DbHelper dbHelper)
        {
            this.groupName = groupeName;
            this.dbHelper = dbHelper;
            InitPerioditResetNumber();
        }

        GroupName groupName;
        // Последний использованный номер
        uint number;
        DbHelper dbHelper;
        Task numberRefresher;
        // Период сбора счетчика номеров заявки к начальному значению
        TimeSpan refreshPeriod = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Создает новое обращение клиента, записывает его в БД и возвращает его номер
        /// </summary>
        /// <returns></returns>
        public string GetNewNumber()
        {
            string result = this.groupName + (++this.number).ToString();

            Invoice invoice = new Invoice()
            {
                Date = DateTime.Now,
                Group = groupName,
                Number = result,
            };

            // Write invoice to DB
            dbHelper.SendEntry(invoice);

            return result;
        }

        /// <summary>
        /// Восстановление последнего выданного номера клиенту из БД
        /// </summary>
        public void InitNumber()
        {
            number = Convert.ToUInt32(dbHelper.QueryMaxNumber(groupName).Remove(0,1));
        }

        public void ResetNumber()
        {
            number = 0;
        }

        /// <summary>
        /// Запуск автоматического сброса номера заявки спустя некоторое время
        /// </summary>
        void InitPerioditResetNumber()
        {
            this.numberRefresher = new Task(() =>
            {
                while (true)
                {
                    Task.Delay(refreshPeriod).Wait();

                    ResetNumber();
                }
            });

            this.numberRefresher.Start();
        }
    }
}
