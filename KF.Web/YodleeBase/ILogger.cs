using System;
using System.Collections.Generic;

namespace YodleeBase
{

	public class RequestLog 
	{
		public string Timer { get; set; }
		public object Body { get; set; }

		public RequestLog(object body) 
		{
            var _timer = DateTime.Now.ToString();
            this.Timer = _timer;
            this.Body = body;
		}

        public RequestLog() : this(null) { }
	}

	public class ResponseLog 
	{
        public string Timer { get; set; }
		public object Body { get; set; }

        public ResponseLog(string timer, object body)
        {
            this.Timer = timer;
            this.Body = body;
        }

        public ResponseLog() { }
	}

	public class Log 
	{
		public Guid ID { get { return System.Guid.NewGuid(); } }
		public string ShortUrl { get; set; }
		public string LongUrl { get;  set; }
		public RequestLog Request { get; set; }
		public ResponseLog Response { get; set; }
	}

	public interface ILogger
	{
		void AddLog(Log log);
		ICollection<Log> GetLogs();
		void ClearLogs();
	}
}

