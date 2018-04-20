namespace ITOrm.Ms.Models.Logging.Formatters
{
	/// <summary>
	/// 用于格式化日志实体的部分属性
	/// </summary>
	public interface IPartFormatter
	{
		string Format(LogEntry entry);
	}
}
