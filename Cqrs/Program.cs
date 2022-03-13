using Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Store>(c => c.UseInMemoryDatabase("db"));

var app = builder.Build();


app.MapGet("/", (Store store) => store.Products.AsNoTracking().Select(d => new
{
    d.Id,
    d.Name,
    Stock = d.Stocks.AsQueryable().Select(s => new { s.Id, s.Description }).ToList()
}).ToListAsync());


app.MapGet("/{id}", (int Id, Store store) => store.Products.AsNoTracking().Where(d => d.Id == Id).Select(k => new
{
    k.Id,
    k.Name,
    Stock = k.Stocks.AsQueryable().Select(s => new { s.Id, s.Description }).ToList()


}).FirstOrDefaultAsync());

app.MapPost("/products", async (Store store) =>
{
    Product product = new() { Name = Guid.NewGuid().ToString() };
    store.Products.Add(product);
    await store.SaveChangesAsync();
});

app.MapPost("/products/{id}/stock", async (int id, Store store) =>
{
    Stock stock = new Stock() { ProductId = id, Description = Guid.NewGuid().ToString() };
    store.Stocks.Add(stock);
    await store.SaveChangesAsync();
});




app.Run();
