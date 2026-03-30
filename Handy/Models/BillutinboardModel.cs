namespace Handy.Models
{
    public class BillutinboardModel
    {
        public int ID { get; set; }
        public string ADTitle { get; set; }
        public string ADDescription{ get; set; }
        public string ADImageUrl { get; set; }
        public string ADPrice { get; set; }
        public int ADType { get; set; } //поле связоно со справочником (0 - товар, 1 - услуга)
        public string AuthorPhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime PublicationDate { get; set; }
        public int UserID { get; set; }
    }
}
