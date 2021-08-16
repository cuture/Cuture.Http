namespace Cuture.Http
{
    /// <summary>
    /// 表单数据格式化器
    /// </summary>
    public interface IFormDataFormatter
    {
        #region Public 方法

        /// <summary>
        /// 格式化到表单字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options">格式化选项</param>
        /// <returns></returns>
        string Format(object obj, FormDataFormatOptions options = default);

        #endregion Public 方法
    }
}