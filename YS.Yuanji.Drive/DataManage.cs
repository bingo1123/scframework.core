using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using YS.Yuanji.Commom;

namespace YS.Yuanji.Drive
{
    public class DataManage
    {
        private ConcurrentDictionary<string, DataRealtimePro> dataCollention = new ConcurrentDictionary<string, DataRealtimePro>();

        public void AddDataList(string key,List<DataItemDetailDto> dataList)
        {
            if (dataCollention.TryGetValue(key, out var _v))
            {
                _v.AddDataList(dataList);
            }
            else
            {
                var dataItem = new DataRealtimePro();
                dataItem.AddDataList(dataList);
                dataCollention.TryAdd(key, dataItem);
            }
        }

        public void UpdateData(string key,string code, string name)
        {
            if (dataCollention.TryGetValue(key,out var _v))
            {
                _v.SetData(code, name);
            }
        }

        public void RemoveData(string key)
        {
            dataCollention.TryRemove(key, out _);
        }

        public void ClearData()
        {
            dataCollention.Clear();
        }

        public void UpdateData(string key,List<DataItemDetailDto> newData)
        {
            AddDataList(key, newData);
        }

        public Dictionary<string,List<DataItemDetailDto>> GetAllData()
        { 
            var data = new Dictionary<string,List<DataItemDetailDto>>();
            foreach (var item in dataCollention)
            {
                data.TryAdd(item.Key, item.Value.GetData());
            }

            return data;
        }

        public DataItemDetailDto GetData(string key,string code)
        {
            if (dataCollention.TryGetValue(key,out var _v))
            {
                return _v.GetData(code);
            }
            return null;

        }

        public List<DataItemDetailDto> GetDataItemDetailDtos()
        {
            var list = new List<DataItemDetailDto>();
            foreach (var item in dataCollention)
            {
                list.AddRange(item.Value.GetData());
            }
            return list;
        }

    }

    public class DataRealtimePro
    { 
        public ConcurrentDictionary<string, DataItemDetailDto> dataCollention = new ConcurrentDictionary<string, DataItemDetailDto>();

        public void AddData(DataRealtimePb dataItemDetailDto)
        {
            dataCollention.TryAdd(dataItemDetailDto.Code, dataItemDetailDto);
        }

        public void AddDataList(List<DataItemDetailDto> dataItemDetailDto)
        {
            dataItemDetailDto?.ForEach(d =>
            {
                if (dataCollention.ContainsKey(d.Code))
                {
                    dataCollention[d.Code].Value = d.Value;
                    dataCollention[d.Code].IsGood = d.IsGood;
                }
                else
                {
                    var it = new DataItemDetailDto
                    {
                        Code = d.Code,
                        Value = d.Value,
                        IsGood = d.IsGood,
                        Name = d.Name
                    };
                    dataCollention.TryAdd(d.Code, it);
                }
            });
        }
        public List<DataItemDetailDto> GetData()
        {
            return dataCollention.Values.ToList();
        }

        public void Clear()
        {
            dataCollention.Clear();
        }

        public void Remove(string code)
        {
            dataCollention.TryRemove(code, out _);
        }

        public DataItemDetailDto GetData(string code)
        {
            dataCollention.TryGetValue(code, out var dataItemDetailDto);
            return dataItemDetailDto;
        }
        public void SetData(string code, string name)
        {
            if(dataCollention.TryGetValue(code, out var dataItemDetailDto))
            {
                dataItemDetailDto.Name = name;
            }
        }
    }

}
