using Aliyun.Api.LOG.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Common.Utilities;
using Aliyun.Api.LOG.Data;
using Aliyun.Api.LOG.Request;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // select you endpoint https://help.aliyun.com/document_detail/29008.html
            String endpoint = "http://cn-hangzhou.log.aliyuncs.com",
                accesskeyId = "", //阿里云授权id
                accessKey = "",  //阿里云授权Key
                project = "",//项目名称，每个项目可以创建10个日志库
                logstore = "";//日志库
            //int shardId = 0;//分区id
            LogClient client = new LogClient(endpoint, accesskeyId, accessKey);
            //init http connection timeout
            client.ConnectionTimeout = client.ReadWriteTimeout = 10000;
            //list logstores
            foreach (String l in client.ListLogstores(new ListLogstoresRequest(project)).Logstores)
            {
                Console.WriteLine(l);
            }
            //put logs
            PutLogsRequest putLogsReqError = new PutLogsRequest
            {
                Project = project,
                Topic = "dotnet_topic",
                Logstore = logstore,
                LogItems = new List<LogItem>()
            };
            for (int i = 1; i <= 10; ++i)
            {
                LogItem logItem = new LogItem {Time = DateUtils.TimeSpan()};
                for (int k = 0; k < 10; ++k)
                    logItem.PushBack("info", "GetLogs 接口查询指定 Project 下某个 Logstore 中的日志数据。还可以通过指定相关参数仅查询符合指定条件的日志数据。");
                putLogsReqError.LogItems.Add(logItem);
            }
            PutLogsResponse putLogRespError = client.PutLogs(putLogsReqError);

            Thread.Sleep(5000);

            //query logs, if query string is "", it means query all data
            GetLogsRequest getLogReq = new GetLogsRequest(project,
                logstore,
                DateUtils.TimeSpan() - 100,
                DateUtils.TimeSpan(),
                "dotnet_topic",
                "",
                100,
                0,
                false);
            GetLogsResponse getLogResp = client.GetLogs(getLogReq);
            Console.WriteLine("Log count : " + getLogResp.Logs.Count.ToString());
            for (int i = 0; i < getLogResp.Logs.Count; ++i)
            {
                var log = getLogResp.Logs[i];
                Console.WriteLine("Log time : " + DateUtils.GetDateTime(log.Time));
                for (int j = 0; j < log.Contents.Count; ++j)
                {
                    Console.WriteLine("\t" + log.Contents[j].Key + " : " + log.Contents[j].Value);
                }
                Console.WriteLine("");
            }

            //query histogram
            GetHistogramsResponse getHisResp = client.GetHistograms(new GetHistogramsRequest(project,
                logstore,
                DateUtils.TimeSpan() - 100,
                DateUtils.TimeSpan(),
                "dotnet_topic",
                ""));
            Console.WriteLine("Histograms total count : " + getHisResp.TotalCount.ToString());

            //list shards
            ListShardsResponse listResp = client.ListShards(new ListShardsRequest(project, logstore));
            Console.WriteLine("Shards count : " + listResp.Shards.Count.ToString());

            //batch get logs
            for (int i = 0; i < listResp.Shards.Count; ++i)
            {
                //get cursor
                String cursor = client.GetCursor(new GetCursorRequest(project, logstore, listResp.Shards[i], ShardCursorMode.BEGIN)).Cursor;
                Console.WriteLine("Cursor : " + cursor);
                BatchGetLogsResponse batchGetResp = client.BatchGetLogs(new BatchGetLogsRequest(project, logstore, listResp.Shards[i], cursor, 10));
                Console.WriteLine("Batch get log, shard id : " + listResp.Shards[i].ToString() + ", log count : " + batchGetResp.LogGroupList.LogGroupList_Count.ToString());
            }

            Console.ReadKey();
        }
    }
}