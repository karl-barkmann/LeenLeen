using Leen.Common;
using Leen.Common.Utils;
using Leen.Common.Xml;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Demo
{
    public static class Program
    {
        private static void Main()
        {
            //Console.WriteLine(NativeMethodsUtil.IsAnyUserLogon() ? "有用户登陆" : "无用户登陆");

            //ApplicationRuntime.TimelyInfoUpdated += ApplicationRuntime_TimelyInfoUpdated;
            //Console.WriteLine("System start up time: {0}", ApplicationRuntime.StartUpTime);
            //Console.WriteLine("Host name: {0}", ApplicationRuntime.LocalHostName);
            //Console.WriteLine("Host address: {0}", ApplicationRuntime.LocalHostAddress);

            //Console.ReadLine();
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
            @"<GetEquipmentrepairInfoReq>
            <ReqSign>safasf-sdf3-sdfsf-3fds</ReqSign>	
            <Data>
            <IsExportQuery>1</IsExportQuery>
             <QueryPara>
	            <PageCount>1</PageCount>
	            <PageIndex>1</PageIndex>
	            <OrgIDs></OrgIDs>
	            <StartDate>2014-05-10</StartDate>
	            <EndDate>2014-05-10</EndDate>
	            <State></State>
	            <EquTypes></EquTypes>
	            <BillPara>
	               <BillSerialNum></BillSerialNum>
	               <BillDesc></BillDesc>
	            </BillPara>
	            <ContainBillInfo>1</ContainBillInfo>
             </QueryPara>
             <BillInfo>
		            <BillSerialNum></BillSerialNum>
		            <BillName></BillName>
             </BillInfo>
            </Data>
            </GetEquipmentrepairInfoReq>";
            xml = xml.Replace("\r\n", String.Empty);

            TimeSpan total = new TimeSpan();
            for (int i = 0; i < 1000000; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                dynamic dynamicXml = DynamicXml.Parse(xml);

                Query query=new Query();
                query.ReqSign = dynamicXml.ReqSign;

                dynamic data = dynamicXml.Data;
                dynamic queryPara = data.QueryPara;

                query.Data = new QueryData();
                query.Data.IsExportQuery = (Int32.Parse(data.IsExportQuery));

                query.Data.QueryPara = new QueryParam();
                query.Data.QueryPara.ContainBillInfo = Int32.Parse(queryPara.ContainBillInfo);
                query.Data.QueryPara.EndDate = DateTime.Parse(queryPara.EndDate);
                query.Data.QueryPara.EquTypes = queryPara.EquTypes;
                query.Data.QueryPara.OrgIDs = queryPara.OrgIDs;
                query.Data.QueryPara.PageCount = Int32.Parse(queryPara.PageCount);
                query.Data.QueryPara.PageIndex = Int32.Parse(queryPara.PageIndex);
                query.Data.QueryPara.StartDate = DateTime.Parse(queryPara.StartDate);
                query.Data.QueryPara.State = queryPara.State;

                dynamic billPara = queryPara.BillPara;

                query.Data.QueryPara.BillPara = new BillPara();
                query.Data.QueryPara.BillPara.BillDesc = billPara.BillDesc;
                query.Data.QueryPara.BillPara.BillSerialNum = billPara.BillSerialNum;

                dynamic billInfo = data.BillInfo;

                query.Data.BillInfo = new BillInfo();
                query.Data.BillInfo.BillName = billInfo.BillName;
                query.Data.BillInfo.BillSerialNum = billInfo.BillSerialNum;

                //dynamicXml.ReqSign = "";

                //dynamic data = dynamicXml.Data;
                //data.IsExportQuery = 1;

                //dynamic billInfo = data.BillInfo;
                //billInfo.BillName = "value";
                //billInfo.BillSerialNum = "value";

                //dynamic queryPara = data.QueryPara;

                //queryPara.ContainBillInfo = 1;

                //queryPara.PageCount = 1;
                //queryPara.PageIndex = 1;
                //queryPara.StartDate = DateTime.Now;
                //queryPara.OrgIDs = "value";
                //queryPara.EndDate = DateTime.Now;
                //queryPara.State = "value";
                //queryPara.EquTypes = "value";

                //queryPara.ContainBillInfo = 1;

                //dynamic billPara = queryPara.BillPara;

                //billPara.BillDesc = "value";
                //billPara.BillSerialNum = "value";

                total = total.Add(watch.Elapsed);
            }

            Console.WriteLine("Dynamic Cost:" + total.TotalMilliseconds / 1000000);

            total = new TimeSpan();
            for (int i = 0; i < 1000000; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Query query = (Query)new XmlSerializer(typeof(Query)).Deserialize(new StringReader(xml));
                total = total.Add(watch.Elapsed);
            }

            Console.WriteLine("Serializer Cost:" + total.TotalMilliseconds / 1000000);

            total = new TimeSpan();
            for (int i = 0; i < 1000000; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Query query = DirectSerialize(xml);
                total = total.Add(watch.Elapsed);
            }

            Console.WriteLine("Direct Cost:" + total.TotalMilliseconds / 1000000);

            total = new TimeSpan();
            for (int i = 0; i < 1000000; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Query query = (Query)XmlObjectSerializer.ToObject(XDocument.Parse(xml).Root, typeof (Query));
                total = total.Add(watch.Elapsed);
            }

            Console.WriteLine("Reflection Cost:" + total.TotalMilliseconds / 1000000);
            

            Console.ReadLine();

        }

        private static Query DirectSerialize(string xml)
        {
            XDocument doc=XDocument.Parse(xml);

            Query query = new Query();
            query.ReqSign = doc.Descendants("ReqSign").First().Value;

            query.Data = new QueryData();
            query.Data.IsExportQuery = (Int32.Parse((doc.Descendants("IsExportQuery").First().Value)));

            query.Data.QueryPara = new QueryParam();
            query.Data.QueryPara.ContainBillInfo = Int32.Parse(doc.Descendants("ContainBillInfo").First().Value);
            query.Data.QueryPara.EndDate = DateTime.Parse(doc.Descendants("EndDate").First().Value);
            query.Data.QueryPara.EquTypes = doc.Descendants("EquTypes").First().Value;
            query.Data.QueryPara.OrgIDs = doc.Descendants("OrgIDs").First().Value;
            query.Data.QueryPara.PageCount = Int32.Parse(doc.Descendants("PageCount").First().Value);
            query.Data.QueryPara.PageIndex = Int32.Parse(doc.Descendants("PageIndex").First().Value);
            query.Data.QueryPara.StartDate = DateTime.Parse(doc.Descendants("StartDate").First().Value);
            query.Data.QueryPara.State = doc.Descendants("State").First().Value;

            query.Data.QueryPara.BillPara = new BillPara();
            query.Data.QueryPara.BillPara.BillDesc = doc.Descendants("BillDesc").First().Value;
            query.Data.QueryPara.BillPara.BillSerialNum = doc.Descendants("BillSerialNum").First().Value;


            query.Data.BillInfo = new BillInfo();
            query.Data.BillInfo.BillName = doc.Descendants("BillName").First().Value;
            query.Data.BillInfo.BillSerialNum = doc.Descendants("BillSerialNum").First().Value;

            return query;
        }


        private static void Test(int a)
        {
        }

        static void ApplicationRuntime_TimelyInfoUpdated(TimelyInfoUpdatedEventArgs e)
        {
            Console.WriteLine("CPU: {0}%", Math.Round(e.TimelyInfo.CpuUsage, 0));
            Console.WriteLine("Memory: {0} / {1} MB",
                              Math.Round(e.TimelyInfo.PrivatePhysicalMemory, 1),
                              Math.Round(e.TimelyInfo.UsedPhysicalMemory, 1));
        }
    }

    [Serializable]
    [XmlRoot("GetEquipmentrepairInfoReq")]
    public class Query
    {
        public string ReqSign { get; set; }

        public QueryData Data { get; set; }
    }

    /*
     
     <ReportUser></ReportUser>
		            <ReportDate></ReportDate>
		            <ReceiveUser></ReceiveUser>
		            <ReportDeptTel></ReportDeptTel>
		            <ReceiveDeptTel></ReceiveDeptTel>
		            <BillDesc></BillDesc>
		            <ExtendedInfo1></ExtendedInfo1>
		            <ExtendedInfo2></ExtendedInfo2>
		            <ExtendedInfo3></ExtendedInfo3>
     */
    [Serializable]
    public class QueryData
    {
        public int IsExportQuery { get; set; }

        public QueryParam QueryPara { get; set; }

        public BillInfo BillInfo { get; set; }
    }

    [Serializable]
    public class BillInfo
    {
        public string BillSerialNum { get; set; }
        public string BillName { get; set; }
    }

    [Serializable]
    public class QueryParam
    {
        public int PageCount { get; set; }

        public int PageIndex { get; set; }
        public string OrgIDs { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string State { get; set; }

        public string EquTypes { get; set; }

        public BillPara BillPara { get; set; }

        public int ContainBillInfo { get; set; }
    }

    [Serializable]
    public class BillPara
    {
        public string BillSerialNum { get; set; }
        public string BillDesc { get; set; }

    }
}
