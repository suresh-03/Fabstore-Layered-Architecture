using System.ComponentModel.DataAnnotations;


namespace FabstoreWebApplication.ViewModels
    {
    public class UserView
        {

        public int UserID { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string Password { get; set; }

        [Phone]
        [StringLength(10)]
        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<CartView> Carts { get; set; } = new List<CartView>();
        public ICollection<WishlistView> Wishlists { get; set; } = new List<WishlistView>();

        public ICollection<ReviewView> Reviews { get; set; } = new List<ReviewView>();

        public ICollection<OrderView> Orders { get; set; } = new List<OrderView>();

        }

    }




