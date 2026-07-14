using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.PLC.ModbusServer
{
    // 自定义数据存储实现
    public class ModbusDataStore : ISlaveDataStore
    {
        private readonly Dictionary<ushort, ushort> _dataStore;

        public ModbusDataStore(Dictionary<ushort, ushort> dataStore)
        {
            _dataStore = dataStore;

            // 初始化存储区
            CoilDiscretes = new PointSource<bool>();
            CoilInputs = new PointSource<bool>();
            HoldingRegisters = new ModbusRegisterSource(_dataStore);
            InputRegisters = new ModbusRegisterSource(_dataStore);
        }

        public IPointSource<bool> CoilDiscretes { get; }
        public IPointSource<bool> CoilInputs { get; }
        public IPointSource<ushort> HoldingRegisters { get; }
        public IPointSource<ushort> InputRegisters { get; }
    }

    // 寄存器数据源实现
    public class ModbusRegisterSource : IPointSource<ushort>
    {
        private readonly Dictionary<ushort, ushort> _dataStore;

        public ModbusRegisterSource(Dictionary<ushort, ushort> dataStore)
        {
            _dataStore = dataStore;
        }

        public ushort this[ushort key]
        {
            get
            {
                return _dataStore.TryGetValue(key, out ushort value) ? value : (ushort)0;
            }
            set
            {
                _dataStore[key] = value;
            }
        }

        public ushort[] ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            var result = new ushort[numberOfPoints];
            for (ushort i = 0; i < numberOfPoints; i++)
            {
                result[i] = this[(ushort)(startAddress + i)];
            }
            return result;
        }

        public void WritePoints(ushort startAddress, ushort[] points)
        {
            for (ushort i = 0; i < points.Length; i++)
            {
                this[(ushort)(startAddress + i)] = points[i];
            }
        }
    }

    // 简单的点源实现
    public class PointSource<T> : IPointSource<T>
    {
        private readonly Dictionary<ushort, T> _values = new Dictionary<ushort, T>();

        public T this[ushort key]
        {
            get => _values.TryGetValue(key, out var value) ? value : default(T);
            set => _values[key] = value;
        }

        public T[] ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            var result = new T[numberOfPoints];
            for (ushort i = 0; i < numberOfPoints; i++)
            {
                result[i] = this[(ushort)(startAddress + i)];
            }
            return result;
        }

        public void WritePoints(ushort startAddress, T[] points)
        {
            for (ushort i = 0; i < points.Length; i++)
            {
                this[(ushort)(startAddress + i)] = points[i];
            }
        }
    }
}
