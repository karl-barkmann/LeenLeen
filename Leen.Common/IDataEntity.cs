namespace Leen.Common
{
    /// <summary>
    /// <see cref="IDataEntity"/> 定义整个业务系统中的业务实体。
    /// </summary>
    public interface IDataEntity
    {
        /// <summary>
        /// 获取此业务实体的唯一标识。
        /// </summary>
        string Id { get; }
    }
}
