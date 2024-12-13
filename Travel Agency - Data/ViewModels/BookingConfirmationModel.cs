// Travel_Agency___Data/ViewModels/BookingConfirmationModel.cs
namespace Travel_Agency___Data.ViewModels
{
    public class BookingConfirmationModel
    {
        public string BookingNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public DateTime TripStart { get; set; }
        public DateTime TripEnd { get; set; }
        public int TravelerCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}