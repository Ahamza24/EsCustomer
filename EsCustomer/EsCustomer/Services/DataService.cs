using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace EsCustomer.Data
{
    public class DataService
    {
        private HttpClient client;
        private Dictionary<string, string> modelEndpoints;
        private string baseUrlAddress;

        public DataService(string baseAddress)
        {
            // add the insecure SSL certificate handler to the HttpClient only if this is a debug build
#if DEBUG
            HttpClientHandler insecureHandler = DependencyService.Get<IHttpClientHandlerService>().GetInsecureHandler();
            this.client = new HttpClient(insecureHandler);
#else
            this.client = new HttpClient();
#endif
            // set base address of the RESTful web service 
            this.baseUrlAddress = baseAddress;

            // dictionary of endpoints, one for each model class in service
            this.modelEndpoints = new Dictionary<string, string>();
        }

        public DataService AddEntityModelEndpoint<TEntity>(string endpoint)
        {
            // this uses reflection to get the name of the class type and use that as the dictionary entry's key
            // the endpoint is the value for this key/value pair
            this.modelEndpoints.Add(typeof(TEntity).FullName, endpoint);
            return this;
        }

        private string GetEntityEndpoint<TEntity>(string nonDefaultEndpoint = null)
        {
            // private helper method to get the endpoint, factored out into separate method for code reuse purposes
            StringBuilder endpoint = new StringBuilder(this.modelEndpoints[typeof(TEntity).FullName]);
            if (!string.IsNullOrEmpty(nonDefaultEndpoint))
            {
                endpoint.Append($"/{nonDefaultEndpoint}");
            }
            return endpoint.ToString();
        }

        public async Task<List<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> searchLambda = null, string nonDefaultEndpoint = null)
            where TEntity : new()
        {
            // form the URI for the webservice GET request 
            var endpoint = GetEntityEndpoint<TEntity>(nonDefaultEndpoint);
            var url = $"{this.baseUrlAddress}/{endpoint}?searchExpression={searchLambda}";
            var uri = new Uri(string.Format(url));

            // make the GET request to the URI
            HttpResponseMessage response =
                await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                // read the content returned by the GET request
                var content = await
                   response.Content
                           .ReadAsStringAsync();

                // deserialise the read content back to C# objects
                var deserialisedContent =
                    JsonConvert.DeserializeObject<IEnumerable<TEntity>>(content);

                // return deserialised objects back to caller as List<TEntity> collection
                return deserialisedContent.ToList<TEntity>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<TEntity> GetAsync<TEntity, T>(T id, string nonDefaultEndpoint = null)
             where TEntity : new()
        {
            // form the URI for the webservice GET request 
            var endpoint = GetEntityEndpoint<TEntity>(nonDefaultEndpoint);
            var uri = new Uri($"{this.baseUrlAddress}/{endpoint}/{id}");

            // make the GET request to the URI
            HttpResponseMessage response =
                await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                // read the content returned by the GET request
                var content = await
                   response.Content
                           .ReadAsStringAsync();

                // deserialise the read content back to C# object
                var deserialisedContent =
                    JsonConvert.DeserializeObject<TEntity>(content);

                // return deserialised object back to caller as TEntity type
                return deserialisedContent;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<bool> UpdateAsync<TEntity, T>(TEntity entity, T id, string nonDefaultEndpoint = null)
        {
            // form the URI for the webservice PUT request 
            var endpoint = GetEntityEndpoint<TEntity>(nonDefaultEndpoint);
            var url = $"{this.baseUrlAddress}/{endpoint}/{id}";
            var uri = new Uri(url);

            // serialise the TEntity object to a JSON string
            string jsonEntity = JsonConvert.SerializeObject(entity);
            StringContent content = new StringContent(jsonEntity, Encoding.UTF8, "application/json");

            // make the PUT request to the URI
            HttpResponseMessage response = await client.PutAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            // return success or failure boolean
            return response.IsSuccessStatusCode;
        }

        public async Task<TEntity> InsertAsync<TEntity>(TEntity entity, string nonDefaultEndpoint = null)
        {
            // form the URI for the webservice POST request 
            var endpoint = GetEntityEndpoint<TEntity>(nonDefaultEndpoint);
            var url = $"{this.baseUrlAddress}/{endpoint}";
            var uri = new Uri(url);

            // serialise the TEntity object to a JSON string
            string jsonEntity = JsonConvert.SerializeObject(entity);
            StringContent content = new StringContent(jsonEntity, Encoding.UTF8, "application/json");

            // make the POST request to the URI
            HttpResponseMessage response = response = await client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                // read the response content returned by the POST request (the created object)
                var responseContent = await
                   response.Content
                           .ReadAsStringAsync();

                // deserialise the read content back to C# object
                var deserialisedContent =
                    JsonConvert.DeserializeObject<TEntity>(responseContent);

                // return deserialised object back to caller as TEntity type
                return deserialisedContent;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<bool> DeleteAsync<TEntity, T>(TEntity entity, T id, string nonDefaultEndpoint = null)
        {
            // form the URI for the webservice DELETE request 
            var endpoint = GetEntityEndpoint<TEntity>(nonDefaultEndpoint);
            var url = $"{this.baseUrlAddress}/{endpoint}/{id}";
            var uri = new Uri(url);

            // make the DELETE request to the URI
            HttpResponseMessage response = await client.DeleteAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            // return success or failure boolean
            return response.IsSuccessStatusCode;
        }
    }
}
