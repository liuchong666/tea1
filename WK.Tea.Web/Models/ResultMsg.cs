namespace WK.Tea.Web.Models
{
    public class ResultMsg
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 操作信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 统计数
        /// </summary>
        public int? count { get; set; }

    }
}