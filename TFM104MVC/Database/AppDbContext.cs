using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Database
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> _options) : base(_options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPicture> ProductPictures { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Orderdetail> Orderdetails { get; set; }
        //public DbSet<CustomerRating> CustomerRatings { get; set; }


        public DbSet<User> Users { get; set; }
        public DbSet<Firm> Firms { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //設定訂單明細 的 複合主鍵
            modelBuilder.Entity<Orderdetail>().HasKey(t => new { t.OrderId, t.ProductId });
        }
    }
}
