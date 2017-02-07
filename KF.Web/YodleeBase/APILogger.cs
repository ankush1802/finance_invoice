using System;
using System.Collections.Generic;
namespace YodleeBase
{
	// Singleton 
	public class APILogger : ILogger
	{
		private IDictionary<string, Log> _logs = new Dictionary<string, Log>();

		public APILogger () { }

		public void AddLog(Log log)
		{
			this._logs.Add(log.ID.ToString(), log);
		}

		public ICollection<Log> GetLogs() 
		{ 
			return _logs.Values;
		}

		public void ClearLogs()
		{
			this._logs.Clear();
		}
	}
}

