using System.Net.Http.Json;
using System.Text.Json;
using NCKH.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace NCKH.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        // URL cơ sở cho các thao tác với Application
        private const string BaseApiUrl = "http://rms-staging.ap-southeast-1.elasticbeanstalk.com/api/applications";
        private const string FileApiUrl = "http://rms-staging.ap-southeast-1.elasticbeanstalk.com/api/ApplicationFiles";

        // StepId cố định cho Dvqltt
        // DvqlttStepId = "11111111-1111-1111-1111-111111111111"
        private const string DvqlttStepId = "11111111-1111-1111-1111-111111111111";

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                UseCookies = true
            };

            _httpClient = new HttpClient(handler);

            // Cookie xác thực
            string myCookie = ".AspNetCore.Identity.Application=CfDJ8JBk1L_A9jJLpiB9FYWW_tMP3s5ron2MF7RstSFl7HFpHbd_L2LSQYwn0rQ-wiaM3McjwNT38xs-EQnVIoA5ixcRf8JvjqTif6BVopHD1rCcE3vspWkgPJbnLQvxqNTSvw713y2DpLuTKtZLaBTRWqWr1MmvWY9q1XPd5FT5ktok-yRwYt_IO_pZsm0152G9p4rquc5Fh_fUNuGddyaFqyL1xmCWdnGqj8scBarc7u01iZUCV0phpLxlfnu6Db0m66aDh2sTp3N6m83_Tne8fRoqjLMlJoT-zed85CZHhqMzSmlTh5NnS2hAvUi4Tnn_FxJggr0ppM2Ds1LAiah5imbaQAbSx2ta5shY681zhqgyUGrebwW8xUUV6vYhZK1GsMO5GumLUVe4LsHfq-F4tlKvPcL9N_OhuhCYCHEqCgeDi8-B_zGD_6G65E9gtNGnBy_w07xKHVz3Pwp_Ie1tLimC1JDUYlJEyISqqz9jVIWa-s6ikBU6uPRsAqY_aFrurcRiXt4GnuE3M71nTXkbG-xd88X8YWSTpyK8vT6UNaddB1fmtj1SS21Nd2UKBftSmuoXbvOp-G7oWSL_OkOdKiStvZsjYh_R5XXFpgTaQJ69FhKCz_zNSsvS14ep2ub76_ubw6hDlLYy5TCcgAFVjVsR5yXwwnBorwMPWdMuX44ejbEBLp1-gCJzuNhm46mVNhIlOSKv7FJyj6fksjpuo-7MjAkd66Eg41H-0QgXwlXXjGyK3Z-Tb1LlV8QkK_UCaz7WZJbWoKEfwo3PB3JlfDzpYevm4Tla5ZVdFwurBQszENLo47Kr1h97NDNQlaRO7AxesqVJyQvoUuRAbCvUzc0";

            _httpClient.DefaultRequestHeaders.Add("Cookie", myCookie);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36");
        }


        // Lấy danh sách hồ sơ dựa trên StepId của Đơn vị quản lý trực tiếp
        public async Task<List<ApplicationModel>> GetApplicationsAsync()
        {
            try
            {
                // Thêm query parameter stepId vào URL[cite: 23]
                string url = $"{BaseApiUrl}?stepId={DvqlttStepId}";
                Debug.WriteLine($"--- Calling API: {url} ---");

                var response = await _httpClient.GetFromJsonAsync<ApplicationResponse>(url, _jsonOptions);

                if (response?.items != null)
                {
                    Debug.WriteLine($"--- SUCCESS: Found {response.items.Count} items ---");
                    return response.items;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--- SYSTEM ERROR: {ex.Message} ---");
            }

            return new List<ApplicationModel>();
        }


        // Gửi yêu cầu cập nhật trạng thái hồ sơ
        // Theo yêu cầu Dvqltt sẽ update qua StepDetail
        public async Task<bool> UpdateApplicationStatusAsync(Guid applicationId, Guid stepDetailId, int newStatus, string note = "")
        {
            try
            {
                // Endpoint chuẩn theo cấu trúc REST của nhóm bạn
                string updateUrl = $"{BaseApiUrl}/{applicationId}/steps/{stepDetailId}";

                var payload = new
                {
                    status = newStatus,
                    comment = note, // Gửi lý do trả hồ sơ hoặc ghi chú chuyển tiếp
                    updatedAt = DateTime.Now
                };

                // Sử dụng PUT để cập nhật trạng thái bước hiện tại
                var response = await _httpClient.PutAsJsonAsync(updateUrl, payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--- API ERROR: {ex.Message} ---");
                return false;
            }
        }

        // Tải minh chứng lên cho hồ sơ của đơn vị
        public async Task<bool> UploadAttachmentAsync(Guid applicationId, FileResult file)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Thêm file stream
                var fileStream = await file.OpenReadAsync();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "files", file.FileName);

                // Thêm applicationId
                content.Add(new StringContent(applicationId.ToString()), "applicationId");

                var response = await _httpClient.PostAsync($"{FileApiUrl}/CreateApplicationFiles", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--- UPLOAD ERROR: {ex.Message} ---");
                return false;
            }
        }


        // Xóa file khỏi hệ thống
        public async Task<bool> DeleteAttachmentAsync(Guid applicationId, Guid fileId)
        {
            try
            {
                string url = $"{FileApiUrl}/{applicationId}/{fileId}";
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--- DELETE ERROR: {ex.Message} ---");
                return false;
            }
        }
    }
}