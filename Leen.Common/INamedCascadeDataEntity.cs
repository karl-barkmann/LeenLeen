namespace Leen.Common
{
    /// <summary>
    /// <see cref="INamedDataEntity"/> 定义整个业务系统中的可命名的级联/树形结构的业务实体。
    /// </summary>
    public interface INamedCascadeDataEntity : ICascadeDataEntity, INamedDataEntity
    {
    }
}
