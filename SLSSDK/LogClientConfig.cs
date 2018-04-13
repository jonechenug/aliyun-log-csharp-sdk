using System;

namespace Aliyun.Api.LOG
{
    public class LogClientConfig
    {
        /// <summary>
        /// 阿里的日志云服务器
        /// </summary>
        public String Endpoint { get; set; }
        /// <summary>
        /// AccesskeyId
        /// </summary>
        public String AccesskeyId { get; set; }
        /// <summary>
        /// AccessKey
        /// </summary>
        public String AccessKey { get; set; }
        /// <summary>
        /// 项目名称，每个项目可以创建10个日志库
        /// </summary>
        public String Project { get; set; }
        /// <summary>
        /// 日志库
        /// </summary>
        public String Logstore { get; set; }
    }
}