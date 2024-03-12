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

        /*var request = new PersonaRequest()
        {
            Persona = persona
        };*/

        var request = new ServerMultiplePersonaRequest()
        {
            Persona = persona
        };

        var client = new PersonaService.PersonaServiceClient(canal);
        //var response = client.RegistrarPersona(request);
        var response = client.RegistrarPersonasServidorMultiple(request);

        while (await response.ResponseStream.MoveNext())
        {
            Console.WriteLine(response.ResponseStream.Current.Resultado);
            await Task.Delay(250);
        }               

        canal.ShutdownAsync().Wait();
        Console.ReadKey();
    }
}