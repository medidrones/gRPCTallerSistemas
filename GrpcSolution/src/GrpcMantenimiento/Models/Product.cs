namespace GrpcMantenimiento.Models;

public class Product
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Status { get; set; } = "NUEVO";
}
