using System;
using System.ComponentModel;

public class PointData : INotifyPropertyChanged
{
    private string _value;
    private string _status;
    private DateTime _timestamp;

    public string Name { get; set; }
    public string Description { get; set; }
    public ushort Address { get; set; }
    public string RegisterType { get; set; }
    public string DataType { get; set; }
    public string Unit { get; set; }

    public string Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged(nameof(Value));
        }
    }

    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public DateTime Timestamp
    {
        get => _timestamp;
        set
        {
            _timestamp = value;
            OnPropertyChanged(nameof(Timestamp));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}