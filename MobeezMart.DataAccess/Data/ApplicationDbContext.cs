using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobeezMart.Models;

namespace MobeezMart.DataAccess;

public class ApplicationDbContext: IdentityDbContext
{
    public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options): base(options) 
    {

    }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Condition> Conditions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
	public DbSet<OrderHeader> OrderHeaders { get; set; }
	public DbSet<OrderDetail> OrderDetail { get; set; }








}
