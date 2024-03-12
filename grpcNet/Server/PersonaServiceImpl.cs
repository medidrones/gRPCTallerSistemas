﻿using Grpc.Core;
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
}
