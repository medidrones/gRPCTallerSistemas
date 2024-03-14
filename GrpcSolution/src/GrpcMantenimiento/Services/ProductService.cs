using Grpc.Core;
using GrpcMantenimiento.Data;
using GrpcMantenimiento.Models;
using Microsoft.EntityFrameworkCore;
using static GrpcMantenimiento.ProductoIt;

namespace GrpcMantenimiento.Services;

public class ProductService : ProductoItBase
{
    private readonly GrpcDbContext _dbContext;

    public ProductService(GrpcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Descripcion))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Envie los dados correctamente"));
        }

        var product = new Product
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion
        };

        await _dbContext.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new CreateProductResponse
        {
            Id = product.Id
        });
    }

    public override async Task<ReadProductResponse> ReadProduct(ReadProductRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "El indice del producto dede ser mayor que cero"));
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (product is not null)
        {
            return await Task.FromResult(new ReadProductResponse
            {
                Id = product.Id,
                Nombre = product.Nombre,
                Descripcion = product.Descripcion,
                Status = product.Status
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"El producto con id {request.Id} no existe"));
    }

    public override async Task<GetAllResponse> ListProduct(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var products = await _dbContext.Products.ToListAsync();

        foreach (var product in products)
        {
            response.Producto.Add(new ReadProductResponse
            {
                Id = product.Id,
                Nombre = product.Nombre,
                Descripcion = product.Descripcion,
                Status = product.Status
            });
        }

        return await Task.FromResult(response);
    }

    public override async Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        if (request.Id <= 0 || string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Descripcion))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Debe ingresar datos correctos"));
        }
        
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (product is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"No exite el product con id {request.Id}"));
        }

        product.Nombre = request.Nombre;
        product.Descripcion = request.Descripcion;
        product.Status = request.Status;

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateProductResponse
        {
            Id= product.Id
        });
    }

    public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "El indice del producto es incorrecto"));
        }
        
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (product is null) 
        { 
            throw new RpcException(new Status(StatusCode.NotFound, $"El producto con id {request.Id} no existe")); 
        }

        _dbContext.Remove(product);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new DeleteProductResponse 
        {  
            Id= product.Id 
        });
    }
}
