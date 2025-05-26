# System Monitor

A cross-platform .NET 8 console application that:

- Monitors **CPU**, **RAM**, and **Disk** usage at configurable intervals  
- Posts metrics to a **REST API endpoint** in JSON format  
- Supports **platform-specific resource collection** via **Strategy Pattern**
- Extensible via a **plugin architecture** (e.g., log to file)

## Features

- CPU Usage (%)
- RAM Usage (used / total in MB)
- Disk Usage (used / total in MB)
- Configurable monitoring interval and API URL via `appsettings.json`
- Plugin support via `IMonitorPlugin` interface
- Cross-platform (Windows / Linux)

## Technologies Used

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- .NET Core
- `System.Diagnostics`, `System.IO`
- `Newtonsoft.Json`

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-user/system-monitor.git
cd system-monitor
```

### 2. Install Dependencies

```bash
dotnet add package Newtonsoft.Json
dotnet add package System.Diagnostics.PerformanceCounter
dotnet add package System.Diagnostics.Process
dotnet add package System.Runtime.InteropServices
```

### 3. Configure `appsettings.json`

```json
{
  "Monitoring": {
    "IntervalSeconds": 5,
    "ApiUrl": "https://your-api.com/metrics"
  }
}
```

### 4. Run the Application

```bash
dotnet run
```

## Plugin Architecture

To extend functionality, implement the `IMonitorPlugin` interface:

```csharp
public interface IMonitorPlugin
{
    void OnUpdate(SystemMetrics metrics);
}
```

### Sample Plugin: File Logger

```csharp
public class FileLoggerPlugin : IMonitorPlugin
{
    public void OnUpdate(SystemMetrics metrics)
    {
        File.AppendAllText("metrics.log", $"{DateTime.Now}: {metrics}\n");
    }
}
```

## Design Patterns Used

| Pattern         | Purpose                                           |
|----------------|---------------------------------------------------|
| Strategy        | Switch between platform-specific metric collectors |
| Factory         | Select appropriate strategy at runtime            |
| Plugin (Observer-like) | Add extensibility for actions on metrics |

