using DatabaseLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;


namespace DatabaseLayer.Data;
    
    public class EcommerceDbContext : IdentityDbContext<ApplicationUser>
    {

        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Discount> Discounts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Discount>()
       .HasMany(d => d.Products)
       .WithMany(p => p.Discounts)
       .UsingEntity(j => j.ToTable("DiscountProduct"));

        modelBuilder.Entity<ApplicationUser>()
                     .HasOne(a => a.Cart)
                     .WithOne(c => c.User)
                     .HasForeignKey<Cart>(c => c.UserId);

        modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartId).HasName("PK__Carts__2EF52A27E9B274A3");
                entity.Property(e => e.CartId).HasColumnName("cart_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("updated_at");
                entity.HasOne(d => d.Customer).WithMany(p => p.Carts).HasForeignKey(d => d.CustomerId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Carts_customer");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__5D9A6C6EBEF04A68");
                entity.HasIndex(e => new { e.CartId, e.ProductId }, "UQ_CartItem").IsUnique();
                entity.Property(e => e.CartItemId).HasColumnName("cart_item_id");
                entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("added_at");
                entity.Property(e => e.CartId).HasColumnName("cart_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasDefaultValue(1).HasColumnName("quantity");
                entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasForeignKey(d => d.CartId).HasConstraintName("FK_CartItems_cart");
                entity.HasOne(d => d.Product).WithMany(p => p.CartItems).HasForeignKey(d => d.ProductId).HasConstraintName("FK_CartItems_product");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__Categori__D54EE9B437901D77");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.Description).HasColumnType("text").HasColumnName("description");
                entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(false).HasColumnName("name");
                entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("updated_at");
                entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory).HasForeignKey(d => d.ParentCategoryId).HasConstraintName("FK_Categories_parent");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB8550FADB46");
                entity.HasIndex(e => e.Email, "UQ__Customer__AB6E6164166FF0C7").IsUnique();
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.Address).HasColumnType("text").HasColumnName("address");
                entity.Property(e => e.City).HasMaxLength(100).IsUnicode(false).HasColumnName("city");
                entity.Property(e => e.Country).HasMaxLength(100).IsUnicode(false).HasColumnName("country");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.Email).HasMaxLength(255).IsUnicode(false).HasColumnName("email");
                entity.Property(e => e.FirstName).HasMaxLength(100).IsUnicode(false).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasMaxLength(100).IsUnicode(false).HasColumnName("last_name");

                entity.Property(e => e.Phone).HasMaxLength(20).IsUnicode(false).HasColumnName("phone");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("updated_at");
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.HasKey(e => e.DiscountId).HasName("PK__Discount__BDBE9EF91CF656A8");
                entity.HasIndex(e => e.Code, "UQ__Discount__357D4CF9FAC1A324").IsUnique();
                entity.Property(e => e.DiscountId).HasColumnName("discount_id");
                entity.Property(e => e.Code).HasMaxLength(50).IsUnicode(false).HasColumnName("code");
                entity.Property(e => e.CurrentUses).HasDefaultValue(0).HasColumnName("current_uses");
                entity.Property(e => e.Description).HasColumnType("text").HasColumnName("description");
                entity.Property(e => e.DiscountType).HasMaxLength(20).IsUnicode(false).HasColumnName("discount_type");
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(10, 2)").HasColumnName("discount_value");
                entity.Property(e => e.EndDate).HasColumnType("datetime").HasColumnName("end_date");
                entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
                entity.Property(e => e.MaxUses).HasColumnName("max_uses");
                entity.Property(e => e.MinOrderAmount).HasDefaultValue(0m).HasColumnType("decimal(10, 2)").HasColumnName("min_order_amount");
                entity.Property(e => e.StartDate).HasColumnType("datetime").HasColumnName("start_date");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__Orders__465962294AACD45A");
                entity.HasIndex(e => e.CustomerId, "idx_orders_customer");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.BillingAddress).HasColumnType("text").HasColumnName("billing_address");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.Notes).HasColumnType("text").HasColumnName("notes");
                entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("order_date");
                entity.Property(e => e.ShippingAddress).HasColumnType("text").HasColumnName("shipping_address");
                entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("pending").HasColumnName("status");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)").HasColumnName("total_amount");
                entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasForeignKey(d => d.CustomerId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Orders_customer");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__3C5A4080FE683357");
                entity.HasIndex(e => e.OrderId, "idx_orderdetails_order");
                entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)").HasColumnName("subtotal");
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)").HasColumnName("unit_price");
                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasForeignKey(d => d.OrderId).HasConstraintName("FK_OrderDetails_order");
                entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasForeignKey(d => d.ProductId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_OrderDetails_product");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EA8E418DB9");
                entity.Property(e => e.PaymentId).HasColumnName("payment_id");
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)").HasColumnName("amount");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("payment_date");
                entity.Property(e => e.PaymentMethod).HasMaxLength(20).IsUnicode(false).HasColumnName("payment_method");
                entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("pending").HasColumnName("status");
                entity.Property(e => e.TransactionId).HasMaxLength(100).IsUnicode(false).HasColumnName("transaction_id");
                entity.HasOne(d => d.Order).WithMany(p => p.Payments).HasForeignKey(d => d.OrderId).HasConstraintName("FK_Payments_order");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF57DF0549E");
                entity.HasIndex(e => e.Sku, "UQ__Products__DDDF4BE7EABB670C").IsUnique();
                entity.HasIndex(e => e.CategoryId, "idx_products_category");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.Description).HasColumnType("text").HasColumnName("description");
                entity.Property(e => e.ImageUrl).HasMaxLength(255).IsUnicode(false).HasColumnName("image_url");
                entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
                entity.Property(e => e.Name).HasMaxLength(255).IsUnicode(false).HasColumnName("name");
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
                entity.Property(e => e.Sku).HasMaxLength(100).IsUnicode(false).HasColumnName("sku");
                entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("updated_at");
                entity.HasOne(d => d.Category).WithMany(p => p.Products).HasForeignKey(d => d.CategoryId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Products_category");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__60883D90800907A3");
                entity.HasIndex(e => new { e.ProductId, e.CustomerId }, "UQ_Review").IsUnique();
                entity.HasIndex(e => e.ProductId, "idx_reviews_product");
                entity.Property(e => e.ReviewId).HasColumnName("review_id");
                entity.Property(e => e.Comment).HasColumnType("text").HasColumnName("comment");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("review_date");
                entity.HasOne(d => d.Customer).WithMany(p => p.Reviews).HasForeignKey(d => d.CustomerId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Reviews_customer");
                entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasForeignKey(d => d.ProductId).HasConstraintName("FK_Reviews_product");
            });

            modelBuilder.Entity<Shipping>(entity =>
            {
                entity.HasKey(e => e.ShippingId).HasName("PK__Shipping__059B15A90B5A20B3");
                entity.ToTable("Shipping");
                entity.Property(e => e.ShippingId).HasColumnName("shipping_id");
                entity.Property(e => e.ActualDelivery).HasColumnName("actual_delivery");
                entity.Property(e => e.Carrier).HasMaxLength(100).IsUnicode(false).HasColumnName("carrier");
                entity.Property(e => e.EstimatedDelivery).HasColumnName("estimated_delivery");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.ShippingCost).HasColumnType("decimal(10, 2)").HasColumnName("shipping_cost");
                entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("processing").HasColumnName("status");
                entity.Property(e => e.TrackingNumber).HasMaxLength(100).IsUnicode(false).HasColumnName("tracking_number");
                entity.HasOne(d => d.Order).WithMany(p => p.Shippings).HasForeignKey(d => d.OrderId).HasConstraintName("FK_Shipping_order");
            });

        }
    }
