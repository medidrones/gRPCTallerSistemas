using Grpc.Core;
using Medina;

namespace Client;

class Program
{
    const string serverPoint = "127.0.0.1:50008";

    static async Task Main(string[] args)
    {
        Channel canal = new Channel(serverPoint, ChannelCredentials.Insecure);

        await canal.ConnectAsync().ContinueWith((task) =>
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine("El cliente se conecto al servidor GRPC correctamente");
            }
        });

        var persona = new Persona() 
        {
            Nombre = "Jorge",
            Apellido = "Medina",
            Email = "medicode.developer@gmail.com"
        };

        var client = new PersonaService.PersonaServiceClient(canal);
        var request = new ClientMultiplePersonaRequest()
        {
            Persona = persona
        };

        var stream = client.RegistrarPersonaClientMultiple();

        foreach (int i in Enumerable.Range(1, 10))
        {
            await stream.RequestStream.WriteAsync(request);
        }

        await stream.RequestStream.CompleteAsync();
        var response = await stream.ResponseAsync;

        Console.WriteLine(response.Resultado);

        canal.ShutdownAsync().Wait();
        Console.ReadKey();
    }
}