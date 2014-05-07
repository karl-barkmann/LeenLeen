using Smart.Common;
using Smart.Native;
using System;

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
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>"+
            @"<GetEquipmentrepairInfoReq>
            <ReqSign>safasf-sdf3-sdfsf-3fds</ReqSign>	
            <Data>	
            <IsExportQuery>1</IsExportQuery>
             <QueryPara>
	            <PageCount></PageCount>
	            <PageIndex></PageIndex>
	            <OrgIDs></OrgIDs>
	            <StartDate></StartDate>
	            <EndDate></EndDate>
	            <State></State>
	            <EquTypes></EquTypes>
	            <BillPara>
	               <BillSerialNum></BillSerialNum>
	               <BillDesc></BillDesc>
	            </BillPara>
	            <ContainBillInfo></ContainBillInfo>
             </QueryPara>
             <BillInfo>
		            <BillSerialNum></BillSerialNum>
		            <BillName></BillName>
		            <ReportUser></ReportUser>
		            <ReportDate></ReportDate>
		            <ReceiveUser></ReceiveUser>
		            <ReportDeptTel></ReportDeptTel>
		            <ReceiveDeptTel></ReceiveDeptTel>
		            <BillDesc></BillDesc>
		            <ExtendedInfo1></ExtendedInfo1>
		            <ExtendedInfo2></ExtendedInfo2>
		            <ExtendedInfo3></ExtendedInfo3>
             </BillInfo>
            </Data>
            </GetEquipmentrepairInfoReq>";
            xml = xml.Replace("\r\n", String.Empty);
            dynamic dynamicXml = DynamicXml.Parse(xml);
            Console.WriteLine(dynamicXml.IsExportQuery);
            Console.ReadLine();

        }

        static void ApplicationRuntime_TimelyInfoUpdated(TimelyInfoUpdatedEventArgs e)
        {
            Console.WriteLine("CPU: {0}%", Math.Round(e.TimelyInfo.CpuUsage, 0));
            Console.WriteLine("Memory: {0} / {1} MB",
                              Math.Round(e.TimelyInfo.PrivatePhysicalMemory, 1),
                              Math.Round(e.TimelyInfo.UsedPhysicalMemory, 1));
        }
    }
}
