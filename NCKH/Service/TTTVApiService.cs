using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NCKH.Models;

namespace NCKH.Services
{
    public class TTTVApiService
    {
        private readonly HttpClient _httpClient;

        // Cập nhật URL chính xác với tham số stepId
        private static readonly string ApiUrl = "http://rms-staging.ap-southeast-1.elasticbeanstalk.com/api/Applications?stepId=22222222-2222-2222-2222-222222222222";
        public TTTVApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Cookie", ".AspNetCore.Identity.Application=CfDJ8JBk1L_A9jJLpiB9FYWW_tOVdQ3bVAEW7N5PXtnDvD9tyBrKcYmoMTdD0YiDlex9a5EoBFSbXBEpgBZWodktNgkl1amCvu2kfuXET2lEUnx53r3qXCKPA-qQiKsmgrl2euvTc4Jv77PdZS-dJN3VJSFKubG2cjfY6JRDayA7brC77bFitVVCXVk8M9tbT-oyzlJAcjKLO3bMvWBH_ggaHQT3pbuQpEZMuNvGLVB5tOcHt7ikRS3_NWQ3zmicKd-s6aESpp9OlXV7higOadmmQcgQIYDmzd97hqua5yXYYyXFIgMDrPnbAbMq8GeHlSFfqnxN8kRut6zp8GTBNQQnSKDbvwrY8UNlskUWGudzTakFY0SxUv0ferWqU3zLQ2St5208G8pSW5i5_pZf8e4O09cnwbwVyt-TFRLHP_ItJljvGrPV054PNEpTnnaMV_fFLTSTgUYTGqrw6KNwsZkzWSVwgw60NhWdXu0kx8-iacLqvgORr3Sk-_VE35cICmYVeHOlAuWRW-NtabrlrOxl2ktpIf19JIeXm7CvooaSnufz48ZQMYAq3VU79XUC6RLmSx96ZHJoa6uuu9ti0OvV2Rr-fqn4z-3Mwigzv_2pacDZJgqZJiq7P8J1wVMcNi_sz1OzHUY-dszYS4ASVkuiRoOzfdzE2JnO1J3OvSrYZkfGtcZb-cRvExxAhBAruIkfwFSToelBSL7K42AsdHxOeopWvd8F01C2JIgv5ALnye8cQUYeMun_8BahiAzvzxUTptUsIYageSqwWy7evX3kcdmxWg29Ga7nckSKS0BICcBbw3SmyG4dJfKm1N4yDZt0d8G4hNf5vmJMgY4pY504uP4");
        }

        /// <summary>
        /// Lấy danh sách hồ sơ từ API
        /// </summary>
        public async Task<List<TTTVHoSoItem>> GetTTTVHoSoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TTTVApiResponse>();
                    return result?.Items ?? new List<TTTVHoSoItem>();
                }
                else
                {
                    // Hiển thị mã lỗi lên màn hình (Dùng để debug nhanh)
                    await App.Current.MainPage.DisplayAlert("Lỗi API", $"Mã lỗi: {response.StatusCode}", "Đóng");
                    return new List<TTTVHoSoItem>();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Lỗi Kết Nối", ex.Message, "Đóng");
                return new List<TTTVHoSoItem>();
            }
        }

        /// <summary>
        /// Cập nhật trạng thái hồ sơ (Duyệt hoặc Trả về)
        /// </summary>
        public async Task<bool> UpdateApplicationStatusAsync(string applicationId, int status, string note = "")
        {
            try
            {
                var updateData = new { status = status, description = note };
                string updateUrl = $"http://rms-staging.ap-southeast-1.elasticbeanstalk.com/api/applications/{applicationId}";
                var response = await _httpClient.PutAsJsonAsync(updateUrl, updateData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật trạng thái: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi file bổ sung lên server
        /// </summary>
        public async Task<bool> UploadAdditionalFilesAsync(string applicationId, List<FileResult> files)
        {
            try
            {
                var content = new MultipartFormDataContent();
                foreach (var file in files)
                {
                    var fileStream = await file.OpenReadAsync();
                    content.Add(new StreamContent(fileStream), "files", file.FileName);
                }

                string uploadUrl = $"http://rms-staging.ap-southeast-1.elasticbeanstalk.com/api/applications/{applicationId}/attachments";
                var response = await _httpClient.PostAsync(uploadUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi upload file: {ex.Message}");
                return false;
            }
        }
    } // Kết thúc class TTTVApiService

    /// <summary>
    /// Class bổ trợ để khớp với cấu trúc JSON trả về từ API
    /// </summary>
    public class TTTVApiResponse
    {
        [JsonPropertyName("items")]
        public List<TTTVHoSoItem> Items { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
}