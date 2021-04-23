namespace Leen.Common
{
    /// <summary>
    /// <see cref="INamedDataEntity"/> 定义整个业务系统中的可命名业务实体。
    /// </summary>
    public interface INamedDataEntity : IDataEntity
    {
        /// <summary>
        /// 获取此业务实体的有效名称。
        /// </summary>
        string Name { get; }
    }
}
