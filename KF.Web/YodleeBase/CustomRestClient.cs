using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;

namespace YodleeBase
{
	public class CustomRestClient
	{
        private IRestClient _restClient;
        private ILogger _logger;
        private string _baseUrl = "";

		public CustomRestClient (IRestClient client, ILogger logger) 
		{
			if (client == null)
				client = new RestClient();
            this._logger = logger;
			this._restClient = client;
		}

		public CustomRestClient(string baseUrl, ref ILogger logger) : this(new RestClient(baseUrl), logger) {
            this._logger = logger;
            this._baseUrl = baseUrl;
        }

        public CustomRestClient(string baseUrl) : this(new RestClient(baseUrl), null) {
            this._baseUrl = baseUrl;
        }

        public void Post(string url, IDictionary<string, object> parameters) {
            var log = new Log();
            var request = new RestRequest(url, Method.POST);
            foreach (KeyValuePair<string, object> parameter in parameters)
                request.AddParameter(parameter.Key, parameter.Value);

            // Create logger and log request
            if (this._logger != null)
            {
                log.ShortUrl = url;
                log.LongUrl = this._baseUrl + url;

                RequestLog requestLog = new RequestLog();
                requestLog.Body = parameters;
                log.Request = requestLog;
            }

            IRestResponse response = this._restClient.Execute(request);
            var content = response.Content;

            // Create logger and log response
            if (this._logger != null)
            {
                // Response
                var responseLog = new ResponseLog();
                var timer = DateTime.Now.ToString();
                responseLog.Timer = timer;
                responseLog.Body = content;
                log.Response = responseLog;
                this._logger.AddLog(log);
            }
        }

        public string RemovePost(string url, IDictionary<string, object> parameters)
        {
            var log = new Log();
            var request = new RestRequest(url, Method.POST);
            foreach (KeyValuePair<string, object> parameter in parameters)
                request.AddParameter(parameter.Key, parameter.Value);

            // Create logger and log request
            if (this._logger != null)
            {
                log.ShortUrl = url;
                log.LongUrl = this._baseUrl + url;

                RequestLog requestLog = new RequestLog();
                requestLog.Body = parameters;
                log.Request = requestLog;
            }

            IRestResponse response = this._restClient.Execute(request);
             return  response.Content;

            //// Create logger and log response
            //if (this._logger != null)
            //{
            //    // Response
            //    var responseLog = new ResponseLog();
            //    var timer = DateTime.Now.ToString();
            //    responseLog.Timer = timer;
            //    responseLog.Body = content;
            //    log.Response = responseLog;
            //    this._logger.AddLog(log);
            //}
        }

		public T Post<T>(string url, IDictionary<string, object> parameters) 
		{

            var log = new Log();
			var request = new RestRequest(url, Method.POST);
			foreach(KeyValuePair<string, object> parameter in parameters)
				request.AddParameter(parameter.Key, parameter.Value);

            // Create logger and log request
            if (this._logger != null)
            {
                log.ShortUrl = url;
                log.LongUrl = this._baseUrl + url;

                RequestLog requestLog = new RequestLog();
                requestLog.Body = parameters;
                log.Request = requestLog;
            }

			IRestResponse response = this._restClient.Execute(request);
            var content = response.Content;

            // Create logger and log response
            if (this._logger != null)
            {
                // Response
                var responseLog = new ResponseLog();
                var timer = DateTime.Now.ToString();
                responseLog.Timer = timer;
                responseLog.Body = content;
                log.Response = responseLog;
                this._logger.AddLog(log);
            }

			try
			{
				return JsonConvert.DeserializeObject<T>(content);
			}
			catch(Exception e)
			{
				throw e;
			}
		}

	}
}

