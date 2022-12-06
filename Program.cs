using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

List<Cart> carts = new List<Cart>();
List<Product> products = new List<Product>();

carts.Add(new Cart() {Id = 1, Products = new List<Product>()});
carts.Add(new Cart() {Id = 2, Products = new List<Product>()});
carts.Add(new Cart() {Id = 3, Products = new List<Product>()});

products.Add(new Product() {Id = 1, Name = "Banana", Price = 10, Quantity = 1});
products.Add(new Product() {Id = 2, Name = "Maça", Price = 5, Quantity = 1});
products.Add(new Product() {Id = 3, Name = "Pera", Price = 2, Quantity = 1});

app.MapGet("/cart", () =>
{
    return carts;
});

app.MapGet("cart/{id}", (int id) => 
{
    foreach (var cart in carts)
    {
        if (cart.Id == id)
        {
            return cart;
        }
    }
    throw new Exception("Cart Not Found");
});

app.MapPost("cart/{cartId}/{productId}", (int cartId, int productId) => 
{
    var cart = carts.Find(c => c.Id == cartId);
    var product = products.Find(p => p.Id == productId);
    if (cart == null)
    {
        cart = new Cart() {Id = cartId, Products = new List<Product>()};
        carts.Add(cart);
    }
    if (product == null)
    {
        throw new Exception("Product Not Found");
    }

    var productAlreadyExist = cart.Products.Find(p => p.Id == productId);
    if (productAlreadyExist != null)
    {
        product.AddOneQuantity();
    }
    else
    {
        cart.Products.Add(product);
    }
    return cart;
});

app.MapDelete("cart/{cartId}/{productId}", (int cartId, int productId) => 
{
    var cart = carts.Find(c => c.Id == cartId);
    if (cart == null)
    {
        throw new Exception("Cart not found");
    }
    var product = cart.Products.Find(p => p.Id == productId);
    if (product != null && product.Quantity > 1)
    {
        product.RemoveOneQuantity();
    }
    else if(product != null)
    {
        cart.Products.Remove(product);
    }
    return;
});

app.MapDelete("cart/{cartId}", (int cartId) => 
{
    // A instância de Cart não é excluída, mas retirada da "base de dados".
    var cart = carts.Find(c => c.Id == cartId);
    if (cart == null)
    {
        throw new Exception("Cart not found");
    }
    carts.Remove(cart);
    return;
});

app.MapGet("cart/{cartId}/finish", (int cartId) =>
{
    var cart = carts.Find(c => c.Id == cartId);
    if (cart == null)
    {
        throw new Exception("Cart not found");
    }
    decimal total = 0;
    int qtd = 0;
    foreach (var product in cart.Products)
    {
        total += product.Price * product.Quantity;
        qtd += product.Quantity;
    }
    return $"Total R${total}. Quantidade de produtos: {qtd}";
});


app.Run();

class Cart
{
    public int Id {get; set;}
    public List<Product> Products {get; set;}
    public int GetId()
    {
        return Id;
    }
}

class Product
{
    public int Id {get; set;}
    public string ?Name {get; set;}
    public decimal Price {get; set;}
    public string ?Description {get; set;}
    public int Quantity {get; set;}

    public void AddOneQuantity()
    {
        Quantity++;
    }
    public void RemoveOneQuantity()
    {
        Quantity--;
    }
}
