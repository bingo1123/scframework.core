# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

这是一个工业设备数据采集系统（曲靖原机数据采集-非标协议），用于从各种工业设备（卷烟机、包装机、激光打孔机等）中采集实时数据并通过 MQTT 上报到云端。

## 技术栈

- **目标框架**: .NET 6.0
- **开发语言**: C#（使用隐式 using 和可空引用类型）
- **通信协议**: MQTT (MQTTnet)、Beckhoff ADS、Modbus TCP
- **序列化**: Newtonsoft.Json
- **日志**: log4net
- **缓存**: StackExchange.Redis
- **依赖注入**: Microsoft.Extensions.Hosting

## 项目结构

### 核心模块

| 项目 | 说明 | 依赖 |
|------|------|------|
| `YS.Yuanji.Start` | 控制台启动入口 | Yuanji.Manage, Yuanji.core |
| `YS.Yuanji.Start.Service` | Windows 服务版本（使用 Topshelf） | 同上 |
| `YS.Yuanji.WPF` | WPF 桌面客户端 | Yuanji.core |

### 驱动与协议层

| 项目 | 说明 | 设备类型 |
|------|------|----------|
| `YS.Yuanji.Drive` | 数据采集抽象基类定义 | - |
| `YS.PLC.BeckHoff` | Beckhoff ADS 协议实现 | 倍福 PLC 设备 |
| `YS.PLC.ModbusServer` | Modbus TCP 服务端/客户端 | Modbus 设备 |
| `ModbusReader` | Modbus 读取器工具 | - |

### 基础设施层

| 项目 | 说明 | 关键依赖 |
|------|------|----------|
| `YS.Yuanji.Mqtt` | MQTT 通信封装 | MQTTnet 4.3.7 |
| `YS.Yuanji.Log` | 日志服务 | log4net |
| `YS.Yuanji.Commom` | 公共类型、DTO、常量 | Newtonsoft.Json |
| `YS.DataBase.Oracle` | Oracle 数据库访问 | Oracle.ManagedDataAccess |
| `YS.DataBase.MySQL` | MySQL 数据库访问（缺陷数据采集） | MySql.Data 8.0.33 |
| `YS.ShareFile.Picture` | 图片文件共享处理 | - |

### 服务管理层

| 项目 | 说明 |
|------|------|
| `Yuanji.core` | 核心扩展、服务注册、依赖注入配置 |
| `Yuanji.Manage` | 启动/停止服务管理、HostedService 实现 |

## 常用命令

### 构建与发布

```bash
# 还原 NuGet 包
dotnet restore YS.Yuanji.WPF/YS.Yuanji.WPF.sln

# 构建整个解决方案
dotnet build YS.Yuanji.WPF/YS.Yuanji.WPF.sln

# 发布单文件版本（独立部署）
dotnet publish YS.Yuanji.Start/YS.Yuanji.Start.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# 发布 Windows 服务版本
dotnet publish YS.Yuanji.Start.Service/YS.Yuanji.Start.Service.csproj -c Release -r win-x64 --self-contained true
```

### 运行

```bash
# 运行控制台版本
dotnet run --project YS.Yuanji.Start

# 运行 WPF 客户端（需要指定解决方案）
dotnet run --project YS.Yuanji.WPF/YS.Yuanji.WPF.csproj

# 安装 Windows 服务（发布后使用）
YS.Yuanji.Start.Service.exe install
YS.Yuanji.Start.Service.exe start
```

## 架构设计

### 数据采集流程

```
Program.cs (入口)
  ↓
HostSevice (IHostedService)
  ↓
StarupSevice (启动服务)
  ↓
ControllerManage (设备管理器)
  ↓
DataCollectContoller (抽象控制器)
  ↓
具体实现 (BeckHoffController / ModbusController / ...)
  ↓
IChanlel (通道接口) → MQTT 上报
```

### 核心抽象

**DataCollectContoller** (`YS.Yuanji.Drive/DataCollectContoller.cs`)
- 所有设备采集控制器的抽象基类
- 管理设备连接、心跳、数据轮询
- 支持三种数据类型：`product`（生产数据）、`realtime`（实时数据）、`others`（其他）
- 通过 `interItems` 字典按间隔时间组织采集项

**IChanlel** (`YS.Yuanji.Drive/IChanlel.cs`)
- 通道接口，定义设备通信契约
- 实现类负责具体协议通信（ADS、Modbus 等）

**设备配置结构** (`basicconfig.json`)
```json
{
  "DeviceConfig": [
    {
      "Ip": "设备IP",
      "MachineCode": "设备编码",
      "MachineName": "设备名称",
      "MachinePart": "部件编号",
      "MachineType": "设备类型 (ZJ118/ZJ116A/FxsZB48/BeckHoff/ModbusTcpClient/...)",
      "ChanleType": "通道类型 (Visu/Fxs/BeckHoff/ModbusTcp/...)",
      "IsEnable": true,
      "ParameterDict": { "自定义参数": "值" },
      "Port": 端口号
    }
  ],
  "MqttConfig": {
    "Host": "MQTT服务器地址",
    "Port": 1883,
    "UserName": "用户名",
    "Password": "密码"
  }
}
```

### 设备类型支持

| MachineType | ChanleType | 项目 | 说明 |
|-------------|------------|------|------|
| ZJ118/ZJ116A | Visu | YS.Yuanji.Drive | 卷接机 Visu 协议 |
| FxsZB48 | Fxs | YS.Yuanji.Drive | 包装机 Fxs 协议 |
| BeckHoff | BeckHoff | YS.PLC.BeckHoff | 倍福 ADS 协议 |
| ModbusTcpClient | ModbusTcp | YS.PLC.ModbusServer | Modbus TCP 客户端 |
| ModbusServer | ModbusServer | YS.PLC.ModbusServer | Modbus TCP 服务端 |
| JgdkAds | Ads | YS.PLC.BeckHoff | 激光打孔 ADS |
| MySQLDefect | MySQL | YS.DataBase.MySQL | MySQL 缺陷数据采集 |
| Picture | Picture | YS.ShareFile.Picture | 图片文件采集 |

### MySQL 设备配置示例

在 `basicconfig.json` 中配置 MySQL 设备：

```json
{
  "MachineCode": "DEFECT_T1",
  "MachineName": "T1#缺陷采集",
  "MachinePart": "",
  "MachineType": "MySQLDefect",
  "ChanleType": "MySQL",
  "IsEnable": true,
  "Ip": "Server=127.0.0.1;Port=3306;Database=data;Uid=root;Pwd=123456;",
  "Port": 3306,
  "ParameterDict": {
    "TableName": "currentdefect",
    "Brand": "云烟（小熊猫家园）",
    "QueryInterval": "3600000"
  }
}
```

**字段说明**:
- `Ip`: MySQL 连接字符串
- `TableName`: 要查询的表名（默认 `currentdefect`）
- `Brand`: 品牌过滤（可选）
- `QueryInterval`: 查询时间范围（毫秒，默认 1 小时）

### 数据上报格式

**实时数据** (`RealTimeDataDto`)
```csharp
{
  "MachineId": "设备编码",
  "Ts": 时间戳,
  "Details": [
    {
      "Code": "数据项编码",
      "Name": "数据项名称",
      "Value": 值,
      "IsGood": 质量标志
    }
  ]
}
```

**MySQL 缺陷数据上报格式** (`YS.DataBase.MySQL/MySQLDefectController.cs`)

MySQL 控制器使用泛型 `RealTimeDataDto<T>` 结构上报数据：

```csharp
// 缺陷记录 DTO - 对应 MySQL 表结构
public class DefectRecordDto
{
    public DateTime date { get; set; }      // 时间戳
    public string brand { get; set; }       // 品牌
    public string DefectClass { get; set; } // 缺陷类别
    public int Count { get; set; }          // 缺陷数量
}

// 上报数据结构
RealTimeDataDto<DefectRecordDto> realTimeDataDto = new RealTimeDataDto<DefectRecordDto>();
realTimeDataDto.MachineId = MachineCode;
realTimeDataDto.Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();
realTimeDataDto.Details = new List<DefectRecordDto>();
```

上报的 JSON 格式示例：

```json
{
  "MachineId": "DEFECT_T1",
  "Ts": 1720598253123,
  "Details": [
    {
      "date": "2026-07-10T09:37:31",
      "brand": "云烟（小熊猫家园）",
      "DefectClass": "烟气含量面侧耳缺失",
      "Count": 0
    },
    {
      "date": "2026-07-10T09:37:31",
      "brand": "云烟（小熊猫家园）",
      "DefectClass": "侧面长边粘贴不牢",
      "Count": 12
    },
    {
      "date": "2026-07-10T09:37:31",
      "brand": "云烟（小熊猫家园）",
      "DefectClass": "斜角翻折",
      "Count": 11
    }
  ]
}
```

**特点**：
- 使用 `RealTimeDataDto<DefectRecordDto>` 泛型结构
- `Details` 字段直接是 `List<DefectRecordDto>`，与数据库表结构完全一致
- 每条记录包含完整的 `date`、`brand`、`DefectClass`、`Count` 字段
- 便于接收端直接反序列化为对应的数据结构

## 配置文件说明

### basicconfig.json
设备主配置，定义所有需要采集的设备列表及其通信参数。

### dataconfig.json
数据点配置，定义每个设备需要采集的具体数据项（地址、类型、轮询间隔等）。

### appsettings.json
应用基础配置，包含服务器地址、终端编码等。

## 配置中心

支持从远程配置中心加载配置，失败时自动回退到本地文件。

### 配置中心设置 (`appsettings.json`)

```json
{
  "ConfigCenter": {
    "Enabled": false,
    "Url": "http://10.97.65.20:8080",
    "AppId": "PtMachine",
    "Environment": "prod",
    "TimeoutSeconds": 10,
    "ReloadOnChange": true
  }
}
```

**字段说明**:
- `Enabled`: 是否启用配置中心
- `Url`: 配置中心地址
- `AppId`: 应用标识
- `Environment`: 环境名称（dev/prod）
- `TimeoutSeconds`: 请求超时时间
- `ReloadOnChange`: 是否启用配置热重载

### 使用环境变量配置

可通过环境变量覆盖配置中心参数：
- `CONFIG_CENTER_URL`: 配置中心地址
- `CONFIG_CENTER_APPID`: 应用ID
- `CONFIG_CENTER_ENV`: 环境名称

### 工作原理

1. 启动时首先加载 `appsettings.json`（包含配置中心参数）
2. 如果 `ConfigCenter.Enabled = true`:
   - 尝试从配置中心获取 `basicconfig` 和 `dataconfig`
   - 配置中心请求失败或超时时，自动回退到本地文件
3. 如果 `ConfigCenter.Enabled = false` 或配置中心不可用:
   - 直接读取本地 `basicconfig.json` 和 `dataconfig.json`
4. 配置热重载:
   - 配置中心模式：每5分钟检查一次配置更新
   - 本地文件模式：监视文件变化自动重载

## 扩展新设备类型

1. 在 `YS.Yuanji.Drive` 中定义新的通道接口实现或复用现有接口
2. 在对应协议项目中（如 `YS.PLC.BeckHoff`）创建新的 Controller 类
3. 继承 `DataCollectContoller`，重写以下方法：
   - `Initialization()`: 设备初始化
   - `RealtimeAsync()`: 实时数据采集
   - `ProductAsync()`: 生产数据采集（如需要）
   - `WriteDataAynsc()`: 写入数据（如需要支持下发）
4. 在 `Yuanji.core` 的服务注册扩展中添加新类型的注册逻辑

## 注意事项

- 所有项目目标框架统一为 .NET 6.0
- 使用 `ImplicitUsings` 和 `Nullable` 特性
- 配置文件在发布时需要设置 `CopyToOutputDirectory` 为 `PreserveNewest`
- MQTT 主题格式：`realtime/{MachineCode}` 和 `realtime/status`
- 服务启动时会自动根据 `basicconfig.json` 中的 `IsEnable` 筛选启用的设备
