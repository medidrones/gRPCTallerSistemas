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
}
