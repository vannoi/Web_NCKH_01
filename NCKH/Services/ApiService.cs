using NCKH.Models;
using Newtonsoft.Json;
using System.Net;

namespace NCKH.Services
{
    public class HoSoRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public class UpdateHoSoRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public class LoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("twoFactorCode")]
        public string TwoFactorCode { get; set; } = null;

        [JsonProperty("twoFactorRecoveryCode")]
        public string TwoFactorRecoveryCode { get; set; } = null;
    }

    public class ApiService
    {
        private static readonly CookieContainer _cookieContainer = new CookieContainer();
        private static readonly HttpClient _httpClient = new HttpClient(
            new HttpClientHandler { CookieContainer = _cookieContainer });

        private const string BaseUrl = "http://rms-staging.ap-southeast-1.elasticbeanstalk.com";

        // =====================
        // ĐĂNG NHẬP
        // =====================
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var body = JsonConvert.SerializeObject(new LoginRequest
                {
                    Email = email,
                    Password = password
                });
                var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/Users/login?useCookies=true", content);

                System.Diagnostics.Debug.WriteLine($"[LOGIN] Status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error LoginAsync: {ex.Message}");
                return false;
            }
        }

        // =====================
        // LẤY DANH SÁCH HỒ SƠ
        // =====================
        public async Task<List<HoSo>> GetHoSoListAsync(
            string stepId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{BaseUrl}/api/applications?stepId={stepId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[GET] Response: {json}");
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse<List<HoSo>>>(json);
                    return apiResult?.Items ?? new List<HoSo>();
                }
                System.Diagnostics.Debug.WriteLine($"[GET] StatusCode: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetHoSoListAsync: {ex.Message}");
            }
            return new List<HoSo>();
        }

        // =====================
        // TẠO HỒ SƠ MỚI
        // =====================
        public async Task<bool> AddHoSoAsync(HoSo hoso)
        {
            try
            {
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(hoso.TieuDe ?? ""), "title");
                form.Add(new StringContent(hoso.MoTa ?? ""), "description");
                form.Add(new StringContent(hoso.Step.ToString()), "status");

                // Nếu có file đính kèm thì thêm vào form
                if (hoso.FileStream != null && !string.IsNullOrEmpty(hoso.FileName))
                {
                    var fileContent = new StreamContent(hoso.FileStream);
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    form.Add(fileContent, "files", hoso.FileName);
                }

                var response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/Applications/CreateApplication", form);

                System.Diagnostics.Debug.WriteLine($"[POST] StatusCode: {response.StatusCode}");
                var body = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[POST] Body: {body}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error AddHoSoAsync: {ex.Message}");
            }
            return false;
        }

        // =====================
        // CẬP NHẬT HỒ SƠ (Teacher — chỉ update được status Draft)
        // =====================
        public async Task<bool> UpdateHoSoAsync(string id, HoSo hoso)
        {
            try
            {
                var dto = new
                {
                    id = id,
                    title = hoso.TieuDe,
                    description = hoso.MoTa,
                    status = hoso.Step
                };
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/Applications/UpdateApplication?id={id}", content);

                System.Diagnostics.Debug.WriteLine($"[UPDATE] StatusCode: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error UpdateHoSoAsync: {ex.Message}");
            }
            return false;
        }

        // =====================
        // CẬP NHẬT STEP DETAIL (Phòng ban — không phải Teacher)
        // =====================
        public async Task<bool> UpdateStepDetailAsync(string applicationId, string stepDetailId)
        {
            try
            {
                var dto = new { stepDetailId = stepDetailId };
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/Applications/UpdateApplicationStepDetail?applicationId={applicationId}",
                    content);

                System.Diagnostics.Debug.WriteLine($"[UPDATE STEP] StatusCode: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error UpdateStepDetailAsync: {ex.Message}");
            }
            return false;
        }

        // =====================
        // XÓA HỒ SƠ
        // =====================
        public async Task<bool> DeleteHoSoAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{BaseUrl}/api/applications/{id}");

                System.Diagnostics.Debug.WriteLine($"[DELETE] StatusCode: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error DeleteHoSoAsync: {ex.Message}");
            }
            return false;
        }

        // =====================
        // LẤY THÔNG BÁO
        // =====================
        public async Task<List<NotificationModel>> GetNotificationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/api/notifications");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<NotificationModel>>(json)
                           ?? new List<NotificationModel>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetNotificationsAsync: {ex.Message}");
            }
            return new List<NotificationModel>();
        }

        // =====================
        // LẤY DANH SÁCH FILE CỦA HỒ SƠ
        // =====================
        public async Task<List<FileAttachment>> GetHoSoFilesAsync(string hoSoId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{BaseUrl}/api/applications/{hoSoId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var hoso = JsonConvert.DeserializeObject<HoSo>(json);
                    return hoso?.ApplicationFiles ?? new List<FileAttachment>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetHoSoFilesAsync: {ex.Message}");
            }
            return new List<FileAttachment>();
        }

        // =====================
        // UPLOAD FILE VÀO HỒ SƠ
        // =====================
        public async Task<bool> UploadFileAsync(string hoSoId, FileResult file)
        {
            try
            {
                var formData = new MultipartFormDataContent();
                var stream = await file.OpenReadAsync();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                formData.Add(fileContent, "files", file.FileName);

                var response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/Applications/UploadFile?applicationId={hoSoId}", formData);

                var body = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(
                    $"[UPLOAD] Status: {response.StatusCode}, Body: {body}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error UploadFileAsync: {ex.Message}");
            }
            return false;
        }

        // =====================
        // XÓA FILE KHỎI HỒ SƠ
        // =====================
        public async Task<bool> DeleteFileAsync(string hoSoId, string fileId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{BaseUrl}/api/Applications/DeleteFile?applicationId={hoSoId}&fileId={fileId}");

                System.Diagnostics.Debug.WriteLine($"[DELETE FILE] Status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error DeleteFileAsync: {ex.Message}");
            }
            return false;
        }
    }

    public class ApiResponse<T>
    {
        [JsonProperty("items")]
        public T Items { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
    }
}