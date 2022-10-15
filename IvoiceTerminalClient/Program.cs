using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using GrpcTerminalService.Models;
using Grpc.Core;
//using GrpcTerminalService;

namespace IvoiceTerminalClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Пример .NET клиент-серверного приложения с использованием протокола GRPC и NoSQL БД. Никитин Илья 2022");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------\n");


            // Соединение
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
            Invoicer.InvoicerClient client = new Invoicer.InvoicerClient(channel);

            Console.WriteLine("Вас приветсвует терминал обслуживания клиентов.");
            Console.WriteLine("Выберите категорию обращения:");

            while (true)
            {
                Console.WriteLine("\nЧто бы открыть новую заявку нажмите \"N\", " +
                    "\nчто бы узнать статус уже существующего обращения нажмите \"F\", " +
                    "\nчто бы закрыть уже существующую обращение нажмите \"C\"," +
                    "\nчто бы запросить все обращения определенной группы нажмите \"A\".");

                switch (Console.ReadKey(true).Key)
                {
                    case (ConsoleKey.N):
                        {
                            Console.WriteLine("\nВведите категорию обращения, доступные опции: \"A\", \"B\", \"C\", \"D\".");

                            // Ожидание ввода оператором группы обращения
                            while (true)
                            {
                                var inputKey = (byte)Console.ReadKey(true).Key;
                                if (!(inputKey == 'A' || inputKey == 'B' || inputKey == 'C' || inputKey == 'D')) continue;


                                var replyId = await client.GetNewClientNumberAsync(new ClientIdRequest { Group = inputKey });
                                Console.WriteLine($"Номер вашего обращения: {replyId.Number}");

                                break;
                            }

                            break;
                        }
                    case (ConsoleKey.F):
                        {
                            Console.WriteLine("\nВведите полный номер обращения, например - \"D207\".");

                            try
                            {
                                string inputString = Console.ReadLine();

                                var replyId = await client.GetClientStatusAsync(new ClientStatusRequest { Number = inputString });

                                Console.WriteLine($"Статус вашего обращения: {replyId.Status}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Обращение не найдено.\n");
                            }

                            break;
                        }
                    case (ConsoleKey.C):
                        {
                            Console.WriteLine("\nВведите полный номер обращения, например - \"D207\".");
                            string inputString = Console.ReadLine();

                            var reply = await client.SetClientStatusAsync(new ClientStatusRequest { Number = inputString });

                            if (reply.Status == true)
                                Console.WriteLine($"Обращение закрыто.");
                            else
                                Console.WriteLine($"Обращение не найдено.");

                            break;
                        }
                    case (ConsoleKey.A):
                        {
                            Console.WriteLine("\nВведите категорию обращений, доступные опции: \"A\", \"B\", \"C\", \"D\".");

                            while (true)
                            {
                                var inputKey = (byte)Console.ReadKey(true).Key;
                                if (!(inputKey == 'A' || inputKey == 'B' || inputKey == 'C' || inputKey == 'D')) continue;


                                using (var reply = client.GetInvoices(new ClientIdRequest { Group = inputKey }))
                                {

                                    if (!await reply.ResponseStream.MoveNext())
                                    {
                                        Console.WriteLine($"Обращения не найдены.");
                                        break;
                                    }

                                    Console.WriteLine($"\nФормат ответа - Номер сообщения, группа обращения, дата обращения, статус, уникальный идентификатор:");
                                    do
                                    {
                                        var invoice = reply.ResponseStream.Current;

                                        Console.WriteLine($"{invoice.Number}, {(char)invoice.Group}, {invoice.Date}, {invoice.Status}, {invoice.Id}"); // TODO BooleanToStatusConverter method
                                    }
                                    while (await reply.ResponseStream.MoveNext());
                                }

                                break;
                            }

                            break;
                        }
                    default:
                        break;
                }
            }

        }
    }
}
