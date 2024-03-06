using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace CloudCommunication
{
    public class CloudCommunication 
    {
        public static async Task Starter()
        {
            //string filePath = @"D:\UnityProjects\MT Project\MT Project\Assets\Resources\plan.json";
            //到我本地文件的文件
            //string cloudPath = "data.json";
            try
            {
                // 调用上传文件方法
                //await CloudStorage.GetFile(filePath, cloudPath);
                Debug.Log("to get batch urls");
                var batchGetDownloadUrls = GetFile();
                Debug.Log("get batch urls finish");
                Debug.Log("batch urls : " + batchGetDownloadUrls.ToString());
                //file id:cloud://lyon-cloud-6gs83v3wf23a354a.6c79-lyon-cloud-6gs83v3wf23a354a-1258884290/data.csv
                // 获取下载URL（这里需要替换为实际的文件ID）
                string fileId = "cloud://lyon-cloud-6gs83v3wf23a354a.6c79-lyon-cloud-6gs83v3wf23a354a-1258884290/data.csv";

                // 下载文件
                string localDownloadPath = "Assets / Resources/ receivedData.json";
                //@"D:\UnityProjects\MT Project\MT Project\Assets\Resources\received_data.json";
                //Assets/Resources
                await Download(localDownloadPath);
                //下载到的本地路径

            }
            catch (Exception ex)
            {
                Debug.Log($"发生错误: {ex.Message}");
            }
        }
        public static Tuple<string, string> GetAuthorization()
        {
            // 密钥参数
            string secretId = "AKIDGOL7kEXhmjZKznU54DVfcN6gDYndIs9I";
            string secretKey = "iSDn14vd8RbbR6eNtX7YvEPPF5pRwGRT";
            string service = "tcb";
            string version = "1.0";
            string algorithm = "TC3-HMAC-SHA256";
            long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // 拼接规范请求串
            Debug.Log("拼接规范请求串");
            string signedHeaders = "content-type;host";
            string canonicalRequest = $"POST\n//api.tcloudbase.com/\n\ncontent-type:application/json; charset=utf-8\nhost:api.tcloudbase.com\n\ncontent-type;host\ne3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            Debug.Log("canonical request is " + canonicalRequest);
            Debug.Log("拼接完成");
            // 拼接待签名字符串
            Debug.Log("拼接代签名字符串");
            string credentialScope = $"{date}/{service}/tc3_request";
            string hashedCanonicalRequest = Sha256Hex(canonicalRequest);
            string stringToSign = $"{algorithm}\n{timestamp}\n{credentialScope}\n{hashedCanonicalRequest}";
            Debug.Log("credential scope is " + credentialScope);
            //debug.log("hashed canonical request is " + hashedcanonicalrequest);
            //debug.log("string to sign is " + stringtosign);
            Debug.Log("拼接代签名字符串完成");
            // 计算签名
            Debug.Log("计算签名");
            var secretDate = Sign(Encoding.UTF8.GetBytes("TC3" + secretKey), date);
            var secretService = Sign(secretDate, service);
            var secretSigning = Sign(secretService, "tc3_request");
            //string signature = ToHexString(HmacSHA256(stringToSign, secretSigning));

            string signature;
            using (var hmacsha256 = new HMACSHA256(secretSigning))
            {
                byte[] hashValue = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
                signature = BitConverter.ToString(hashValue).Replace("-", "").ToLowerInvariant();
            }
            Debug.Log("secret service is " + secretService);
            Debug.Log("secret signing is " + secretSigning);
            Debug.Log("signature is " + signature);
            Debug.Log("计算签名完成");

            // 拼接Authorization
            Debug.Log("准备拼接Authorization");
            string authorization = $"1.0 {algorithm} Credential={secretId}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";
            Debug.Log(authorization);
            Debug.Log("authorization拼接完成");
            //return authorization;
            Debug.Log(timestamp.ToString());
            return new Tuple<string, string>(authorization, timestamp.ToString());
        }

        private static byte[] Sign(byte[] key, string msg)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(msg));
            }
        }


        public static string Sha256Hex(string data)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static async Task Download(string local)
        {
            // 在这里替换您的下载链接和本地文件名
            var downloadUrl = "https://6c79-lyon-cloud-6gs83v3wf23a354a-1258884290.tcb.qcloud.la/data.csv";
            //var localFilename = "downloaded_file.csv";
            await DownloadFileAsync(downloadUrl, local);
        }

        static async Task DownloadFileAsync(string url, string localFilename)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        using (var streamToWriteTo = File.Open(localFilename, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }
                }
            }

            Debug.Log($"文件已保存为: {localFilename}");
        }

        static async Task GetFile()
        {
            // 请将以下变量替换为实际的值
            var envId = "lyon-cloud-6gs83v3wf23a354a"; // Environment ID
            var fileId = "cloud://lyon-cloud-6gs83v3wf23a354a.6c79-lyon-cloud-6gs83v3wf23a354a-1258884290/data.csv";
            var authResult = GetAuthorization();
            string authorization = authResult.Item1;
            string timestamp = authResult.Item2;
            var sessionToken = "none";
            Debug.Log("to batchGetDownloadUrl");
            var result = await BatchGetDownloadUrl(envId, fileId, authorization, sessionToken, timestamp);
            Debug.Log("batchGetDownloadUrlRes: " + result);
        }

        static async Task<string> BatchGetDownloadUrl(string envId, string fileId, string authorization, string sessionToken, string timestamp)
        {
            var url = $"https://tcb-api.tencentcloudapi.com/api/v2/envs/{envId}/storages:batchGetTempUrls";

            var body = new
            {
                fileList = new[] { new { fileID = fileId } }
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-CloudBase-Authorization", authorization);
            httpClient.DefaultRequestHeaders.Add("X-CloudBase-SessionToken", sessionToken);
            httpClient.DefaultRequestHeaders.Add("X-CloudBase-TimeStamp", timestamp);

            var requestBody = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            //JsonSerializer.Serialize(body)


            var response = await httpClient.PostAsync(url, requestBody);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.Log($"Error: {response.StatusCode}");
            }
            Debug.Log(body);
            Debug.Log($"Response: {responseString}");

            return responseString;
        }

    }
}
