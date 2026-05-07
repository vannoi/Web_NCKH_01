namespace NCKH.Models
{
    public class FileAttachment
    {
        public Guid id { get; set; }
        public string fileName { get; set; }
        public string fileUrl { get; set; }
        public bool isInternal { get; set; }
        public string FileSize { get; set; }
    }
}