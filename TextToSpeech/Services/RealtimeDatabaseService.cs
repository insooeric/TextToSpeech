using System.Text;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using DotNetEnv;

namespace TextToSpeech.Services
{
    public class RealtimeDatabaseService
    {
        private readonly string _databaseUrl;
        private readonly string _firebase_type, _firebase_project_id, _firebase_private_key_id, _firebase_private_key, _firebase_client_email, _firebase_client_id, _firebase_auth_uri, _firebase_token_uri, _firebase_auth_provider_x509_cert_url, _firebase_client_x509_cert_url, _firebase_universe_domain;

        private static readonly string[] _scopes =
        {
            "https://www.googleapis.com/auth/firebase.database",
            "https://www.googleapis.com/auth/userinfo.email"
        };

        public RealtimeDatabaseService(IConfiguration config)
        {
            Env.Load();
            _databaseUrl = Environment.GetEnvironmentVariable("DB_URL")?? "";

            _firebase_type = Environment.GetEnvironmentVariable("FIRE_TYPE") ?? "";
            _firebase_project_id = Environment.GetEnvironmentVariable("FIRE_PROJECT_ID") ?? "";
            _firebase_private_key_id = Environment.GetEnvironmentVariable("FIRE_PRIVATE_KEY_ID") ?? "";
            _firebase_private_key = Environment.GetEnvironmentVariable("FIRE_PRIVATE_KEY") ?? "";
            _firebase_client_email = Environment.GetEnvironmentVariable("FIRE_CLIENT_EMAIL") ?? "";
            _firebase_client_id = Environment.GetEnvironmentVariable("FIRE_CLIENT_ID") ?? "";
            _firebase_auth_uri = Environment.GetEnvironmentVariable("FIRE_AUTH_URI") ?? "";
            _firebase_token_uri = Environment.GetEnvironmentVariable("FIRE_TOKEN_URI") ?? "";
            _firebase_auth_provider_x509_cert_url = Environment.GetEnvironmentVariable("FIRE_AUTH_PROVIDER_X509_CERT_URL") ?? "";
            _firebase_client_x509_cert_url = Environment.GetEnvironmentVariable("FIRE_CLIENT_X509_CERT_URL") ?? "";
            _firebase_universe_domain = Environment.GetEnvironmentVariable("FIRE_UNIVERSE_DOMAIN") ?? "";


            if (string.IsNullOrWhiteSpace(_databaseUrl))
            {
                throw new ArgumentException("Firebase DatabaseUrl is not configured properly in appsettings.");
            }
        }

        public async Task PutDataAsync<T>(string path, T data)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = $"{_databaseUrl.TrimEnd('/')}/{path}.json?access_token={accessToken}";

            var json = JsonConvert.SerializeObject(data);

            using var client = new HttpClient();
            var response = await client.PutAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
        }

        public async Task<string> PushDataAsync<T>(string path, T data)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = $"{_databaseUrl.TrimEnd('/')}/{path}.json?access_token={accessToken}";

            var json = JsonConvert.SerializeObject(data);

            using var client = new HttpClient();
            var response = await client.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            var resultContent = await response.Content.ReadAsStringAsync();

            var resultObj = JsonConvert.DeserializeObject<PushResponse>(resultContent);
            if (string.IsNullOrEmpty(resultObj?.name))
            {
                throw new InvalidOperationException("The push operation was unsuccessful: 'name' is missing in the response.");
            }

            return resultObj.name;
        }

        public async Task<T> GetDataAsync<T>(string path)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = $"{_databaseUrl.TrimEnd('/')}/{path}.json?access_token={accessToken}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(content);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to retrieve data from '{url}': Deserialized result is null.");
            }

            return result;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var privateKey = _firebase_private_key.Replace("\\n", "\n");

            var serviceAccountJson = new
            {
                type = _firebase_type,
                project_id = _firebase_project_id,
                private_key_id = _firebase_private_key_id,
                private_key = privateKey,
                client_email = _firebase_client_email,
                client_id = _firebase_client_id,
                auth_uri = _firebase_auth_uri,
                token_uri = _firebase_token_uri,
                auth_provider_x509_cert_url = _firebase_auth_provider_x509_cert_url,
                client_x509_cert_url = _firebase_client_x509_cert_url,
                universe_domain = _firebase_universe_domain
            };


            var json = JsonConvert.SerializeObject(serviceAccountJson);

            GoogleCredential credential = GoogleCredential.FromJson(json);

            credential = credential.CreateScoped(_scopes);

            var token = await credential
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync()
                .ConfigureAwait(false);

            return token;
        }
        private class PushResponse
        {
            public string name { get; set; } = "";
        }
    }
}
