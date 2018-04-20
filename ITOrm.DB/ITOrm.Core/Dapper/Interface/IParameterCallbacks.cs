
namespace ITOrm.Core.Dapper
{
    /// <summary>
    /// Extends IDynamicParameters with facilities for executing callbacks after commands have completed
    /// </summary>
    public partial interface IParameterCallbacks : IDynamicParameters
    {
        /// <summary>
        /// Invoked when the command has executed
        /// </summary>
        void OnCompleted();
    }
}
