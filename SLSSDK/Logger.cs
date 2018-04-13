using System;
using System.Collections.Generic;
using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Common.Utilities;
using Aliyun.Api.LOG.Data;
using Aliyun.Api.LOG.Request;
using Aliyun.Api.LOG.Response;

namespace Aliyun.Api.LOG
{
    public class Logger
    {
        public readonly LogClientConfig _config;

        private LogClient Client => new LogClient(_config.Endpoint, _config.AccesskeyId, _config.AccessKey);

        public Logger(LogClientConfig config)
        {
            _config = config;
        }

        public virtual void PutLogs(String topic,IEnumerable<Object>[] logs)
        {
            if ( String.IsNullOrWhiteSpace(topic)||logs == null||logs.Length==0)
            {
                throw  new ArgumentException("topic or logs  must not be null or empty!");
            }
            var putLogsReqError = new PutLogsRequest
            {
                Project = _config.Project,
                Topic =topic,
                Logstore = _config.Logstore,
                LogItems = new List<LogItem>()
            };
            foreach (var l in logs)
            {
                LogItem logItem = new LogItem
                {
                    Time = DateUtils.TimeSpan(),
                    Contents= l.ToLogContents()
                };
                putLogsReqError.LogItems.Add(logItem);
            }
            Client.PutLogs(putLogsReqError);
        }


        public virtual void PutLogs(String topic,Object log)
        {
             PutLogs(topic,new object[] {log});
        }

    }
}