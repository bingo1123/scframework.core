using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YS.Yuanji.Log;

namespace YS.Yuanji.Commom.helper
{
    public class WorkInfoHepler
    {
        public const string workOrderHashKey = "dc:workorder:cur";

        public const string subscribHash = "channel:dc:workorder:cur";

        private static readonly object _lock = new object();

        private static WorkInfoHepler _instance;

        private string _machineName { get; set; }

        private volatile WorkInfo _CurrentWorkInfo;

        public WorkInfo GetCurrentWorkInfo()
        {
            if (_CurrentWorkInfo == null)
            {
                //LogController.Instance.Error($"redis:没获取到数据，采用默认数据");
                return new WorkInfo()
                {
                    workorder = string.Empty,
                    classid = -9,
                    classtimeid = -9,
                    productid = string.Empty,
                };
            }

            return _CurrentWorkInfo;
        }

        public static WorkInfoHepler Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new WorkInfoHepler();
                    }
                return _instance;
            }
        }


        private WorkInfoHepler()
        {
            //192.168.30.240
            //172.17.39.30
            //string redisConnectionString = "172.17.39.30:6379";

        }

        public void LoadConfig(string MachineId)
        {
            _machineName = MachineId;

            string redisConnectionString = "127.0.0.1:6379";
            _CurrentWorkInfo = GetWorkInfo(MachineId);
        }


        public WorkInfo GetWorkInfo(string MachineId)
        {
            try
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    [Serializable]
    public class SubscribWorkInfo
    {
        public string workdate { get; set; }

        public string machineid { get; set; }

        public string workorder { get; set; }

        public int classid { get; set; }

        public int classtimeid { get; set; }

        public string productid { get; set; }
    }

    [Serializable]
    public class WorkInfo
    {
        //
        public string workorder { get; set; }

        public int classid { get; set; }

        public int classtimeid { get; set; }

        public string productid { get; set; }

        public WorkInfo Clone()
        {
            return new WorkInfo()
            {
                workorder = this.workorder,
                classid = this.classid,
                classtimeid = this.classtimeid,
                productid = this.productid,
            };
        }
    }
}
