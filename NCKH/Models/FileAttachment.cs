using Newtonsoft.Json;

namespace NCKH.Models
{
   
    public class ApplicationFileItem
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        [JsonProperty("fileId")]
        public string FileId { get; set; }

        [JsonProperty("file")]
        public FileDetail File { get; set; }
    }

    public class FileDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("length")]
        public long Length { get; set; }

        public string FileSizeDisplay => Length < 1024 * 1024
            ? $"{Length / 1024} KB"
            : $"{(double)Length / (1024 * 1024):F1} MB";
    }

    public class FileAttachment
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonProperty("uploadDate")]
        public string UploadDate { get; set; } = string.Empty;

        [JsonProperty("fileSize")]
        public string FileSize { get; set; } = string.Empty;

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;

        private string Ext => Path.GetExtension(FileName).ToUpperInvariant().TrimStart('.');

        public string FileTypeLabel => Ext switch
        {
            "PDF" => "PDF",
            "DOCX" or "DOC" => "DOCX",
            "PNG" => "PNG",
            "JPG" or "JPEG" => "JPG",
            _ => Ext.Length > 0 ? Ext : "FILE"
        };

        public string ThumbBackColor => Ext switch
        {
            "PDF" => "#FFF1F1",
            "DOCX" or "DOC" => "#EFF6FF",
            "PNG" or "JPG" or "JPEG" => "#F0FDF4",
            _ => "#F8FAFC"
        };

        public string ThumbBorderColor => Ext switch
        {
            "PDF" => "#FF8A8A",
            "DOCX" or "DOC" => "#93C5FD",
            "PNG" or "JPG" or "JPEG" => "#86EFAC",
            _ => "#CBD5E1"
        };

        public string ThumbTextColor => Ext switch
        {
            "PDF" => "#E11D48",
            "DOCX" or "DOC" => "#2563EB",
            "PNG" or "JPG" or "JPEG" => "#16A34A",
            _ => "#64748B"
        };
    }
}