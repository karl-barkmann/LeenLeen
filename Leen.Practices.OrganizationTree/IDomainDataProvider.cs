using Leen.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 提供获取域数据接口。
    /// </summary>
    public interface IDomainDataProvider
    {
        /// <summary>
        /// 获取域数据集合。
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetDomains();
    }
}
