﻿namespace ITOrm.Ms.Models.Logging.Formatters
{
	/// <summary>
    /// MessageFormatter
	/// </summary>
	public class MessageFormatter : IPartFormatter
	{
		public string Format(LogEntry entry)
		{
			return entry.Message;
		}
	}
}
