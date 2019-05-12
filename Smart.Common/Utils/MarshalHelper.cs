using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Leen.Common.Utils
{
    /// <summary>
    ///  提供了一个方法集，这些方法用于在跨平台调用时分配非托管内存、复制非托管内存块、将托管类型转换为非托管类型或将非托管内存转换为托管类型，此外还提供针对特定托管对象的扩展方法。
    /// </summary>
    public static class MarshalHelper
    {
        /// <summary>
        /// 将非托管内存指针转换为类型为 <typeparamref name="T"/> 的结构体数组。
        /// <para>注意：调用者应回收非托管内存指针。</para>
        /// </summary>
        /// <typeparam name="T">要转换的托管结构体类型。</typeparam>
        /// <param name="p">跨平台调用获取到的非托管内存指针。</param>
        /// <param name="count">指针指向内存的结构体数量。</param>
        /// <returns>转换后的托管结构体列表。</returns>
        public static List<T> MarshalPtrToStructList<T>(IntPtr p, int count) where T : struct
        {
            List<T> l = new List<T>();
            for (int i = 0; i < count; i++, p = new IntPtr(p.ToInt32() + Marshal.SizeOf(typeof(T))))
            {
                T t = MarshalPtrToStruct<T>(p);
                l.Add(t);
            }
            return l;
        }

        /// <summary>
        /// 将类型为 <typeparamref name="T"/> 的托管结构体集合转换为非托管内存指针。
        /// <para>注意：调用者应回收非托管内存指针。</para>
        /// </summary>
        /// <typeparam name="T">要转换的托管结构体类型。</typeparam>
        /// <param name="structs">托管结构体集合。</param>
        /// <returns>转换后的非托管内存指针。</returns>
        public static IntPtr MarshalStructListToPtr<T>(this IEnumerable<T> structs) where T : struct
        {
            if (structs == null)
            {
                throw new ArgumentNullException(nameof(structs));
            }

            if (structs.Count() < 1)
            {
                throw new ArgumentException("array length should larger than zero.", nameof(structs));
            }

            int sizeOfEachItem = Marshal.SizeOf(typeof(T));

            int index = 0;
            byte[] buffer = new byte[sizeOfEachItem * structs.Count()];
            foreach (var structObject in structs)
            {
                IntPtr pDataEach = Marshal.AllocHGlobal(sizeOfEachItem);
                try
                {
                    Marshal.StructureToPtr(structObject, pDataEach, true);
                    Marshal.Copy(pDataEach, buffer, sizeOfEachItem * index, sizeOfEachItem);
                    index++;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Marshal.FreeHGlobal(pDataEach);
                }
            }

            IntPtr ret = CreatePtrFromBuffer(buffer, 0, buffer.Length);

            return ret;
        }

        /// <summary>
        /// 将类型为 <typeparamref name="T"/> 的嵌套托管结构体集合转换为非托管内存指针数组。
        /// </summary>
        /// <typeparam name="T">要转换的托管结构体类型。</typeparam>
        /// <param name="collection">托管结构体嵌套集合。</param>
        /// <returns>转换后的非托管内存指针数组。</returns>
        public static IntPtr[] MarshalNestedStruListToPtrList<T>(this IEnumerable<IEnumerable<T>> collection) where T : struct
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection.Count() < 1)
            {
                throw new ArgumentException("collection must have at least on element.", nameof(collection));
            }

            if (collection == null || !collection.Any() || collection.Any(x => !x.Any()))
            {
                return new IntPtr[0];
            }

            List<IntPtr> ptrs = new List<IntPtr>(collection.Count());
            foreach (var structs in collection)
            {
                ptrs.Add(structs.MarshalStructListToPtr());
            }

            return ptrs.ToArray();
        }

        /// <summary>
        /// 将类型为 <typeparamref name="T"/> 的二维数组转换为非托管内存指针数组。
        /// </summary>
        /// <typeparam name="T">要转换的托管结构体类型。</typeparam>
        /// <param name="structArray">二维托管结构体数组。</param>
        /// <returns>转换后的非托管内存指针。</returns>
        public static IntPtr Marshal2DStructArrayToPtr<T>(this T[,] structArray) where T : struct
        {
            if (structArray == null)
            {
                throw new ArgumentNullException(nameof(structArray));
            }

            if (structArray.Length < 1)
            {
                throw new ArgumentException("array length should larger than zero.", nameof(structArray));
            }

            int sizeOfSturct = Marshal.SizeOf(typeof(T));
            int sizeOfArray = structArray.Length * sizeOfSturct;

            int offset = 0;
            byte[] buffer = new byte[sizeOfArray];
            for (int i = 0; i < structArray.GetLength(0); i++)
            {
                for (int j = 0; j < structArray.GetLength(1); j++)
                {
                    IntPtr pDataEach = Marshal.AllocHGlobal(sizeOfSturct);
                    try
                    {
                        Marshal.StructureToPtr(structArray[i, j], pDataEach, true);
                        Marshal.Copy(pDataEach, buffer, offset, sizeOfSturct);
                        offset += sizeOfSturct;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pDataEach);
                    }
                }
            }

            IntPtr ret = CreatePtrFromBuffer(buffer, 0, buffer.Length);

            return ret;
        }

        /// <summary>
        /// 将非托管内存指针转换为类型为 <typeparamref name="T"/> 的托管结构体。
        /// </summary>
        /// <para>注意：调用者应回收非托管内存指针。</para>
        /// <typeparam name="T">要转换的托管结构体类型。</typeparam>
        /// <param name="p">跨平台调用获取到的非托管内存指针。</param>
        /// <returns>转换后的托管结构体。</returns>
        public static T MarshalPtrToStruct<T>(IntPtr p) where T : struct
        {
            return (T)Marshal.PtrToStructure(p, typeof(T));
        }

        /// <summary>
        /// 分配一片内存，并创建一个非托管内存指针，然后将一组8位无符号整数数组中的数据复制到该指针中。
        /// </summary>
        /// <para>注意：调用者应回收非托管内存指针。</para>
        /// <param name="buffer">一组8位无符号整数数组。</param>
        /// <param name="startIndex">从数组中指定的索引处开始复制。</param>
        /// <param name="length">要复制的字节长度。</param>
        /// <returns></returns>
        public static IntPtr CreatePtrFromBuffer(byte[] buffer, int startIndex, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length < 1)
            {
                throw new ArgumentException("buffer length should larger than zero.", nameof(buffer));
            }

            IntPtr pointer = Marshal.AllocHGlobal(length);

            try
            {
                Marshal.Copy(buffer, startIndex, pointer, length);
            }
            catch (Exception)
            {
                Marshal.FreeHGlobal(pointer);
                throw;
            }

            return pointer;
        }
    }
}
