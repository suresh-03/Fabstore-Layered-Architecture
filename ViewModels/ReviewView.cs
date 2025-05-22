using System.ComponentModel.DataAnnotations;

namespace FabstoreWebApplication.ViewModels
    {


    public class ReviewView
        {

        public int ReviewID { get; set; }

        public int UserID { get; set; }


        public UserView? User { get; set; }


        public int ProductID { get; set; }


        public ProductView? Product { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? ReviewText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

    }
