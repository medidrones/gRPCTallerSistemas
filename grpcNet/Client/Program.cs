﻿using Grpc.Core;
using Medina;

namespace Client;

class Program
{
    const string serverPoint = "127.0.0.1:50008";

    static void Main(string[] args)
    {
        Channel canal = new Channel(serverPoint, ChannelCredentials.Insecure);

        canal.ConnectAsync().ContinueWith((task) =>
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine("El cliente se conecto al servidor GRPC correctamente");
            }
        });

        var client = new Operaciones.OperacionesClient(canal);

        canal.ShutdownAsync().Wait();
        Console.ReadKey();
    }
}