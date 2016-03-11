# Digger
Digger is designed to run as a Windows service (using [TopShelf](https://topshelf.readthedocs.org/en/latest/)). The idea is that it has `Collectors` that collects time series data that are via a `Dispatcher` forwarded to a `Processor` that e.g. stores the data in InfluxDb.

This is a small "util/sample/poc" that I put together to show usage of [my .Net InfluxDb client](https://github.com/danielwertheim/myinfluxdbclient), [InfluxDb](https://influxdata.com/) and [Grafana](http://grafana.org/).

Serilog is used for logging: booting, exceptions... Configure file target in `App.config`. 

## License
It's licensed under MIT. It's as I said. Still a POC/sample. So use at your own risk ;-)

## Collectors
Collectors collects data and transforms the data to `IMeasurement` instances that carries `InfluxDbPoint(s)`. They are configured dynamically in: `digger.agent.conf`. The location of this file is determined by `App.config`.

Currently, the supported collectors are:

- Performance counters
- Windows Services (State, ThreadCount, WorkingSetMb)
- WebHooks

### Performance Counter Collector
Produces `PerformanceCounterMeasurement`. You define what performance counters to monitor and how often they should be extracted in the `digger.agent.conf`.

```
{
  "collectors": [
    {
      "type": "PerformanceCounterCollector",
      "name": "cpuPctTotal",
      "collectInterval": "00:00:01.000",
      "performanceCounterConfig": {
        "categoryName": "Processor",
        "counterName": "% Processor Time",
        "instanceName": "_Total"
      }
    },
    {
      "type": "PerformanceCounterCollector",
      "name": "ramAvailableMb",
      "collectInterval": "00:00:01.000",
      "performanceCounterConfig": {
        "categoryName": "Memory",
        "counterName": "Available MBytes"
      }
    }
  ]
}
```

### Windows Service Status Collector
Produces `WinServiceStatusMeasurement`. You can have many `WinServiceStatusCollector` configured, but that would only make sense if you would like different `collectInterval` or inclussion of process info (`includeProcessInfo`) for different services (`serviceNames`).

```
{
  "collectors": [
    {
      "type": "WinServiceStatusCollector",
      "name": "serviceStatus",
      "collectInterval": "00:00:15.000",
      "includeProcessInfo": true,
      "serviceNames": [ "MSSQL$SQLEXPRESS2014" ]
    }
  ]
}
```

### WebHooksCollector
Produces `WebHookMeasurement`. There's an embeded `ASP.Net WebAPI` hosted in the `Digger.Agent`. You can use this to accept e.g. `POSTS` of web hooks from Mandrill or something and transform them to InfluxDb points.

```
{
  "collectors": [
    {
      "type": "WebHooksCollector",
      "baseAddress": "http://127.0.0.1:62333"
    }
  ]
}
```

## Dispatchers
The dispatchers accepts an `IMeasurement` that is dispatched to `IProcessors`. There are currently two implementations:

- ImmediateInProcessDispatcher
- QueuedInProcessDispatcher

Which one to use is defined in the config:

```
{
  "dispatcherType": "Digger.Agent.Dispatchers.QueuedInProcessDispatcher, Digger.Agent"
}
```

## Processors
The processors is what processes the collected `IMeasurements` and their `InfluxPoints`. There are two processors:

- ConsoleProcessor
- QueuedInfluxDbProcessor

When just running the application manually, e.g. by pressing (`F5`) in Visual Studio, it will use the `ConsoleProcessor`. Otherwise the `InfluxDbProcessor`. You can tweak this in `Digger.Agent.Bootstrap.ProcessorsModule`.

```
{
  "processors": {
    "influxDb": {
      "host": "http://192.168.1.176:8086",
      "database": "digger"
    }
  }
}
```

In code there's support for having different measurements sent to different `InfluxDbTargetInfo`, but there's no config support for that yet so, the above will be used as the sinlge target.