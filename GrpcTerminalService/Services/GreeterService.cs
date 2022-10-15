using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcTerminalService.Models;

namespace GrpcTerminalService
{
    public class InvoicerService : Invoicer.InvoicerBase
    {
        private readonly ILogger<InvoicerService> _logger;
        private readonly IInvoiceManager _invoiceManager;

        public InvoicerService(ILogger<InvoicerService> logger, IInvoiceManager invoiceManager)
        {
            _logger = logger;
            _invoiceManager = invoiceManager;
        }

        public override Task<ClientIdReply> GetNewClientNumber(ClientIdRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ClientIdReply
            {
                Number = _invoiceManager.GetNewClientNumber((GroupName)request.Group)
            });
        }

        public override Task<ClientStatusReply> SetClientStatus(ClientStatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ClientStatusReply
            {
                Status = _invoiceManager.PostStatus(request.Number)
            });
        }

        public override Task<ClientStatusReply> GetClientStatus(ClientStatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ClientStatusReply
            {
                Status = _invoiceManager.GetStatus(request.Number)
            });
        }

        public override async Task GetInvoices(ClientIdRequest request, IServerStreamWriter<InvoiceReply> responseStream, ServerCallContext context)
        {
            var data = _invoiceManager.GetAllInvoiceByGroup((GroupName)request.Group);

            if (data == null) return;

            foreach (Invoice invoice in data)
            {
                await responseStream.WriteAsync(new InvoiceReply()
                {
                    Id = invoice.Id,
                    Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(invoice.Date.ToUniversalTime()),
                    Group = (int)invoice.Group,
                    Number = invoice.Number,
                    Status = invoice.Status
                });

                // Имитация вычислений
                await Task.Delay(100);
            }
        }
    }
}
