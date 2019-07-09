﻿using Leen.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 提供获取设备业务实体的数据接口。
    /// </summary>
    public interface IDeviceDataProvider
    {
        /// <summary>
        /// 获取指定Id的设备业务实体。
        /// </summary>
        /// <param name="id">设备Id。</param>
        /// <returns></returns>
        Task<INamedDataEntity> GetDevice(string id);

        /// <summary>
        /// 获取所有设备数据实体。
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetDevices();

        /// <summary>
        /// 获取指定组织节点Id下的直属设备业务实体集合。
        /// </summary>
        /// <param name="organizationId">组织结构Id。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetDevices(string organizationId);

        /// <summary>
        /// 递归获取指定组织节点Id下的所有设备业务实体集合。
        /// </summary>
        /// <param name="organizationId">组织结构Id。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetDevicesResursive(string organizationId);
    }
}