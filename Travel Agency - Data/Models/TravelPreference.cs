// Travel_Agency___Data/Models/TravelPreference.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Agency___Data.Models
{
    public class TravelPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PreferenceId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public string? PreferredClimate { get; set; }
        public string? Activities { get; set; }
        public string? TravelCompanion { get; set; }
        public string? PreferredLocation { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}