using Aliyun.Sls.Common.Utilities;
using Aliyun.Sls.Data;
using Aliyun.Sls.Request;
using Aliyun.Sls.Response;
using Aliyun.Sls.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aliyun.Sls.sample
{
    class LoghubSample
    {
        static void Main(string[] args)
        {
            String endpoint = "http://cn-hangzhou-failover-intranet.sls.aliyuncs.com",
                accesskeyId = "",
                accessKey = "",
                project = "",
                logstore = "";
            int shardId = 0;
            LogClient client = new LogClient(endpoint, accesskeyId, accessKey);
            //init http connection timeout
            client.ConnectionTimeout = client.ReadWriteTimeout = 10000;
            //*
            //list logstores
            foreach (String l in client.ListLogstores(new ListLogstoresRequest(project)).Logstores)
            {
                Console.WriteLine(l);
            }
            //put logs
            PutLogsRequest putLogsReqError = new PutLogsRequest();
            putLogsReqError.Project = project;
            putLogsReqError.Topic = "dotnet_topic";
            putLogsReqError.Logstore = logstore;
            putLogsReqError.LogItems = new List<LogItem>();
            for (int i = 1; i <= 10; ++i)
            {
                LogItem logItem = new LogItem();
                logItem.Time = DateUtils.TimeSpan();
                for (int k = 0; k < 10; ++k)
                    logItem.PushBack("error_", "invalid operation");
                putLogsReqError.LogItems.Add(logItem);
            }
            PutLogsResponse putLogRespError = client.PutLogs(putLogsReqError);
            //query logs
            client.GetLogs(new GetLogsRequest(project, logstore, DateUtils.TimeSpan() - 10, DateUtils.TimeSpan()));
            //query histogram
            client.GetHistograms(new GetHistogramsRequest(project, logstore, DateUtils.TimeSpan() - 10, DateUtils.TimeSpan()));
            //list shards
            client.ListShards(new ListShardsRequest(project, logstore));
            //get cursor
            String cursor = client.GetCursor(new GetCursorRequest(project, logstore, shardId, ShardCursorMode.BEGIN)).Cursor;
            Console.WriteLine(cursor);
            //batch get logs, loghub interface
            BatchGetLogsResponse response = client.BatchGetLogs(new BatchGetLogsRequest(project, logstore, shardId, cursor, 10));
            //list topic
            client.ListTopics(new ListTopicsRequest(project, logstore));
            //*/
        }
    }
}
