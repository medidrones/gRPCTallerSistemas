using Grpc.Core;
using Medina;
using static Medina.PersonaService;

namespace Server;

public class PersonaServiceImpl : PersonaServiceBase
{
    public override Task<PersonaResponse> RegistrarPersona(PersonaRequest request, ServerCallContext context)
    {
        string mensaje = "Se inserto correctamente el usuario: " + request.Persona.Nombre + " - " + request.Persona.Email;

        PersonaResponse response = new PersonaResponse() 
        {
            Resultado = mensaje
        };

        return Task.FromResult(response);
    }

    public override async Task RegistrarPersonasServidorMultiple(ServerMultiplePersonaRequest request, IServerStreamWriter<ServerMultiplePersonaResponse> responseStream, ServerCallContext context)
    {
        Console.WriteLine("El servidor recibio el request del cliente:" + request.ToString());

        string mensaje = "Se inserto correctamente el usuario: " + request.Persona.Nombre + " - " + request.Persona.Email;

        foreach (int i in Enumerable.Range(1, 10))
        {
            ServerMultiplePersonaResponse response = new ServerMultiplePersonaResponse() 
            {
                Resultado = String.Format("El response {0} tiene contenido {1}", i, mensaje)
            };

            await responseStream.WriteAsync(response);
        }
    }

    public override async Task<ClientMultiplePersonaResponse> RegistrarPersonaClientMultiple(IAsyncStreamReader<ClientMultiplePersonaRequest> requestStream, ServerCallContext context)
    {
        string resultado = "";

        while (await requestStream.MoveNext())
        {
            resultado += String.Format("Request Mensaje en el Servidor {0} {1}", requestStream.Current.Persona.Email, Environment.NewLine);
        }

        var responseMessage = new ClientMultiplePersonaResponse()
        {
            Resultado = resultado
        };

        return responseMessage;
    }

    public override async Task RegistrarPersonaBidireccional(IAsyncStreamReader<BidireccionalPersonaRequest> requestStream, IServerStreamWriter<BidireccionalPersonaResponse> responseStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            var  mensaje = String.Format("Comunicacion Bidireccional: {0} {1}", requestStream.Current.Persona.Email, Environment.NewLine);
            Console.WriteLine(mensaje);

            var response = new BidireccionalPersonaResponse()
            {
                Resultado = mensaje
            };

            await responseStream.WriteAsync(response);
        }
    }
}
