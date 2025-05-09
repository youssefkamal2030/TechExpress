using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models.Dto_s
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [Display(Name = "Rating (1-5 stars)")]
        public int Rating { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        [Display(Name = "Your Review")]
        public string Comment { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Review Title")]
        public string Title { get; set; }

        [Display(Name = "Date")]
        public DateTime CreatedAt { get; set; }

        public bool IsApproved { get; set; }
        public string UserProfileImageUrl { get; set; }
    }
}