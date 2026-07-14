// 假设你在WPF页面中使用ObservableCollection<DataItemDetailDto>来绑定数据
// 下面是一个用于实时刷新或新增DataItemDetailDto的辅助方法

using System.Collections.ObjectModel;
using YSDL.WPF.View;
using YSDL.WPF.ViewModel;

public static class DataItemDetailDtoHelper
{
    /// <summary>
    /// 根据Code实时刷新或新增DataItemDetailDto
    /// </summary>
    /// <param name="collection">绑定到页面的ObservableCollection</param>
    /// <param name="newItems">新收到的数据列表</param>
    public static void UpdateOrAddItems(
        ObservableCollection<ParameterData> collection,
        List<ParameterData> newItems)
    {
        foreach (var newItem in newItems)
        {
            var existing = collection.FirstOrDefault(x => x.Code == newItem.Code);
            if (existing != null)
            {
                existing.Value = newItem.Value;
                existing.IsGood = newItem.IsGood;
                // 如果需要通知UI属性变化，DataItemDetailDto应实现INotifyPropertyChanged
            }
            else
            {
                collection.Add(new ParameterData
                {
                    Code = newItem.Code,
                    Value = newItem.Value,
                    IsGood = newItem.IsGood
                });
            }
        }
    }
}
