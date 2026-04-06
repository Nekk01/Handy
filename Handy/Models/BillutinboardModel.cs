namespace Handy.Models
{
    public class BillutinboardModel
    {
        public long ID { get; set; }
        public string ADTitle { get; set; }
        public string ADDescription{ get; set; }
        public string ADPreviewImageUrl { get; set; }
        public decimal ADPrice { get; set; }
        public long ADType { get; set; } //поле связоно со справочником (1 - не указано, 2 - товар, 3 - услуга)
        public string Address { get; set; }
        public DateTime PublicationDate { get; set; }
        public long UserID { get; set; }
        public List<string> Images { get; set; }
    }
}
