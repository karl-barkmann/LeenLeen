namespace Leen.Common
{
    /// <summary>
    /// <see cref="IDataEntity"/> 定义整个业务系统中的级联/树形结构的业务实体。
    /// </summary>
    public interface ICascadeDataEntity : IDataEntity
    {
        /// <summary>
        /// 获取此实体的父级实体的唯一标识。
        /// </summary>
        string ParentId { get; }
    }
}
