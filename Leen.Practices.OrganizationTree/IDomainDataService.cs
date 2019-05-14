using Leen.Common;
using System.Collections.Generic;

namespace DCCS.Client.Infrastructure.OrganizationTree
{
    /// <summary>
    /// 提供获取域数据接口。
    /// </summary>
    public interface IDomainDataService
    {
        /// <summary>
        /// 获取域数据集合。
        /// </summary>
        /// <returns></returns>
        IEnumerable<INamedDataEntity> GetDomains();
    }
}
