using AutoMapper;
using Fabstore.Domain.Models;
using FabstoreWebApplication.ViewModels;

namespace FabstoreWebApplication.Mapping
    {
    public class MappingProfile : Profile
        {
        public MappingProfile()
            {
            // ViewModel to Model
            CreateMap<BrandView, Brand>();
            CreateMap<CartView, Cart>();
            CreateMap<CategoryView, Category>();
            CreateMap<OrderItemView, OrderItem>();
            CreateMap<OrderView, Order>();
            CreateMap<PaymentView, Payment>();
            CreateMap<ProductImageView, ProductImage>();
            CreateMap<ProductVariantView, ProductVariant>();
            CreateMap<ProductView, Product>();
            CreateMap<ReviewView, Review>();
            CreateMap<UserView, User>();
            CreateMap<WishlistView, Wishlist>();

            //Model to ViewModel
            CreateMap<Brand, BrandView>();
            CreateMap<Cart, CartView>();
            CreateMap<Category, CategoryView>();
            CreateMap<OrderItem, OrderItemView>();
            CreateMap<Order, OrderView>();
            CreateMap<Payment, PaymentView>();
            CreateMap<ProductImage, ProductImageView>();
            CreateMap<ProductVariant, ProductVariantView>();
            CreateMap<Product, ProductView>();
            CreateMap<Review, ReviewView>();
            CreateMap<User, UserView>();
            CreateMap<Wishlist, WishlistView>();

            }
        }
    }
