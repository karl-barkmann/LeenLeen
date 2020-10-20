using System;

namespace Leen.Common
{
    /// <summary>
    /// 表示某一服务接口的请求结果，基于错误码或异常信息及服务处理响应结果来识别服务请求是否已成功处理。
    /// </summary>
    public class OperationResult<T> : OperationResult
    {
        static OperationResult()
        {
        }

        /// <summary>
        /// 构造 <see cref="OperationResult"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">当请求未成功处理时，服务返回的错误码。</param>
        /// <param name="errorMessage">当请求未成功处理时，服务返回的错误描述（如果有）。</param>
        public OperationResult(int errorCode, string errorMessage) : this(errorCode, errorMessage, null)
        {

        }

        /// <summary>
        /// 构造 <see cref="OperationResult"/> 类的实例。
        /// </summary>
        /// <param name="error">当请求未成功处理时，表示服务请求处理过程中发生的异常信息。</param>
        public OperationResult(Exception error) : this(0, null, error)
        {

        }

        /// <summary>
        /// 构造 <see cref="OperationResult"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">当请求未成功处理时，服务返回的错误码。</param>
        /// <param name="errorMessage">当请求未成功处理时，服务返回的错误描述（如果有）。</param>
        /// <param name="error">当请求未成功处理时，表示服务请求处理过程中发生的异常信息。</param>
        public OperationResult(int errorCode, string errorMessage, Exception error) : base(errorCode, errorMessage, error)
        {

        }

        /// <summary>
        /// 构造 <see cref="OperationResult"/> 类的实例。
        /// </summary>
        /// <param name="responseResult">请求服务成功处理时，处理的响应结果。</param>
        public OperationResult(T responseResult) : base(0, null, null)
        {
            Result = responseResult;
        }

        /// <summary>
        /// 当请求服务成功处理时，处理的响应结果。
        /// </summary>
        public T Result { get; private set; }

        /// <summary>
        /// 获取表示请求成功的操作结果。
        /// </summary>
        /// <param name="result">包含正确操作返回的操作结果。</param>
        public static OperationResult<T> FromResult(T result)
        {
            return new OperationResult<T>(result);
        }

        /// <summary>
        /// 构建包含错误消息的操作结果。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        /// <returns></returns>
        public new static OperationResult<T> FromErrorMessage(string errorMessage)
        {
            return new OperationResult<T>(0, errorMessage);
        }

        /// <summary>
        /// 构建包含错误码的操作结果。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <returns></returns>
        public new static OperationResult<T> FromErrorCode(int errorCode)
        {
            return new OperationResult<T>(errorCode, string.Empty);
        }

        /// <summary>
        /// 构建包含错误内容的操作结果。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="errorMessage">错误消息。</param>
        /// <returns></returns>
        public new static OperationResult<T> FromError(int errorCode, string errorMessage)
        {
            return new OperationResult<T>(errorCode, errorMessage);
        }

        /// <summary>
        /// 构建包含异常对象的操作结果。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        /// <returns></returns>
        public new static OperationResult<T> FromException(Exception ex)
        {
            return new OperationResult<T>(ex);
        }

        /// <summary>
        /// 构建操作结果。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="errorMessage">错误消息。</param>
        /// <param name="ex">异常信息。</param>
        /// <returns></returns>
        public new static OperationResult<T> Create(int errorCode, string errorMessage, Exception ex)
        {
            return new OperationResult<T>(errorCode, errorMessage, ex);
        }
    }

    /// <summary>
    /// 表示某一服务接口的请求结果，基于错误码或异常信息来识别服务请求是否已成功处理。
    /// </summary>
    public class OperationResult : IOperationResult
    {
        static OperationResult()
        {
            Success = new OperationResult(null);
        }

        /// <summary>
        /// 构造 <see cref="OperationResult"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">当请求未成功处理时，服务返回的错误码。</param>
        /// <param name="errorMessage">当请求未成功处理时，服务返回的错误描述（如果有）。</param>
        public OperationResult(int errorCode, string errorMessage) : this(errorCode, errorMessage, null)
        {

        }

        /// <summary>
        /// 构造 <see cref="OperationResult"/> 类的实例。
        /// </summary>
        /// <param name="error">当请求未成功处理时，表示服务请求处理过程中发生的异常信息。</param>
        public OperationResult(Exception error) : this(0, null, error)
        {

        }

        /// <summary>
        /// 构造 <see cref="OperationResult"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">当请求未成功处理时，服务返回的错误码。</param>
        /// <param name="errorMessage">当请求未成功处理时，服务返回的错误描述（如果有）。</param>
        /// <param name="error">当请求未成功处理时，表示服务请求处理过程中发生的异常信息。</param>
        public OperationResult(int errorCode, string errorMessage, Exception error)
        {
            Error = error;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 获取表示请求成功的操作结果。
        /// </summary>
        public static OperationResult Success { get; private set; }

        /// <summary>
        /// 获取一个值，指示请求是否已由服务成功处理。
        /// </summary>
        public bool Processed
        {
            get
            {
                return ErrorCode == 0 && Error == null;
            }
        }

        /// <summary>
        /// 当请求未成功处理时，服务返回的错误描述（如果有）。
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 当请求未成功处理时，服务返回的错误码。
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// 当请求未成功处理时，表示服务请求处理过程中发生的异常信息。
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// 构建包含错误消息的操作结果。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        /// <returns></returns>
        public static OperationResult FromErrorMessage(string errorMessage)
        {
            return new OperationResult(0, errorMessage);
        }

        /// <summary>
        /// 构建包含错误码的操作结果。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <returns></returns>
        public static OperationResult FromErrorCode(int errorCode)
        {
            return new OperationResult(errorCode, string.Empty);
        }

        /// <summary>
        /// 构建包含错误内容的操作结果。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="errorMessage">错误消息。</param>
        /// <returns></returns>
        public static OperationResult FromError(int errorCode, string errorMessage)
        {
            return new OperationResult(errorCode, errorMessage);
        }

        /// <summary>
        /// 构建包含异常对象的操作结果。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        /// <returns></returns>
        public static OperationResult FromException(Exception ex)
        {
            return new OperationResult(ex);
        }

        /// <summary>
        /// 构建操作结果。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="errorMessage">错误消息。</param>
        /// <param name="ex">异常信息。</param>
        /// <returns></returns>
        public static OperationResult Create(int errorCode, string errorMessage, Exception ex)
        {
            return new OperationResult(errorCode, errorMessage, ex);
        }
    }

    /// <summary>
    /// 表示某一服务接口的请求结果。
    /// </summary>
    public interface IOperationResult
    {
        /// <summary>
        /// 获取一个值，指示请求是否已由服务成功处理。
        /// </summary>
        bool Processed { get; }

        /// <summary>
        /// 当请求未成功处理时，服务返回的错误描述（如果有）。
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// 当请求未成功处理时，服务返回的错误码。
        /// </summary>
        int ErrorCode { get; }

        /// <summary>
        /// 当请求未成功处理时，表示服务处理请求过程中发生的异常信息。
        /// </summary>
        Exception Error { get; }
    }
}