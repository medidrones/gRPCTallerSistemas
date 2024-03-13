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

        var client = new PersonaService.PersonaServiceClient(canal);

        Persona[] personaCollection = {
            new Persona() { Email = "medina@gmail.com" }, 
            new Persona() { Email = "jorge@gmail.com" },
            new Persona() { Email = "ingrid@gmail.com" }, 
            new Persona() { Email = "nestor@gmail.com" }, 
            new Persona() { Email = "maria@gmail.com" } 
        };

        var stream = client.RegistrarPersonaBidireccional();

        foreach (var persona in personaCollection)
        {
            Console.WriteLine("Enviando al servidor:" + persona.Email);

            var request = new BidireccionalPersonaRequest()
            {
                Persona = persona
            };
            
            await stream.RequestStream.WriteAsync(request);
        }

        await stream.RequestStream.CompleteAsync();

        var responseCollection = Task.Run(async () =>
        {
            while (await stream.ResponseStream.MoveNext())
            {
                Console.WriteLine("El cliente esta recibiendo del servidor {0} {1}", stream.ResponseStream.Current.Resultado, Environment.NewLine);
            }
        });

        await responseCollection;

        canal.ShutdownAsync().Wait();
        Console.ReadKey();
    }
}