# 👁️ Observability & Monitoring Strategy View

**Purpose:** Technical blueprint for system telemetry, structured log aggregation, distributed tracing, and Service Level Agreement (SLA) verification.
**Tools Stack:** Serilog (Structured Logging) + OpenTelemetry (Distributed Tracing)
**Status:** IMPLEMENTED & VERIFIED ✅
**Document Version:** 3.0

---

## 📝 1. LOGGING ARCHITECTURE (SERILOG)

The application implements semantic, structured logging using **Serilog**. Instead of parsing raw string data, every log entry is emitted as a structured key-value pair payload (JSON), making it instantly ready for centralized log aggregators (e.g., ELK Stack, Grafana Loki, Datadog).

### Telemetry Pipeline & Request Flow

```
┌───────────────────────────────────────────────────────────────────────────────────────┐
│                               [ HTTP Request Arrives ]                                │
└───────────────────────────────────────┬───────────────────────────────────────────────┘
                                        │
                                        ▼
┌───────────────────────────────────────────────────────────────────────────────────────┐
│  Middleware Pipeline Enrichment                                                       │
│  • Extract/Generate Correlation ID     ──► Pushed to Serilog LogContext               │
│  • Track HTTP Method, Path, Query                                                     │
└───────────────────────────────────────┬───────────────────────────────────────────────┘
                                        │
                                        ▼
┌───────────────────────────────────────────────────────────────────────────────────────┐
│  Service Execution Context Logging                                                    │
│  • Injects: CustomerId, VehicleId, TechnicianId, ServiceBayId                         │
└───────────────────────────────────────┬───────────────────────────────────────────────┘
                                        │
                                        ▼
┌───────────────────────────────────────────────────────────────────────────────────────┐
│  Log Sinks (Parallel Routing)                                                        │
│  • Console Sink (Ansi Development)                                                    │
│  • File Sink (Daily Rolling Sync)      ──► Pathed to: logs/app-YYYYMMDD.txt           │
└───────────────────────────────────────────────────────────────────────────────────────┘

```

### Production Logging Invariants (`appsettings.json`)
The system separates infrastructure noise from application logs by applying strict `MinimumLevel` and `Override` settings (e.g., `Microsoft` to `Warning`), ensuring high-fidelity log files and efficient ingestion in production environments.

### Business Context Enrichment Points
In critical paths like `AppointmentService.CreateAsync`, detailed contextual data is injected, including:
*   **Entry Points:** Request payloads (e.g., `customerId`, `technicianId`).
*   **Transactions:** Final status and IDs (`appointmentId`).
*   **Exceptions:** Structured metadata for `BookingConflictException` to aid in debugging.

---

## 🚀 2. DISTRIBUTED TRACING (OPENTELEMETRY)

**OpenTelemetry** provides full visibility by tracing execution across asynchronous boundaries.

### End-to-End Tracing Execution Hierarchy
Each `POST /api/v1/appointments` request initiates a parent span, with children spanning across validation, database interactions (SELECT/INSERT), and domain logic, facilitating performance analysis.

### Core Instrumentation Registration
Traces are configured in `Program.cs` to integrate automatically with ASP.NET Core and Entity Framework Core, as illustrated in the following snippet:
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => {
        tracing.AddAspNetCoreInstrumentation()
               .AddEntityFrameworkCoreInstrumentation()
               .AddConsoleExporter();
    });
```

---

## 🔍 3. CORRELATION & DEBUGGING RUNBOOK

A unified `CorrelationId` (from `X-Correlation-ID` header) is applied to all logs and spans, allowing engineers to trace a single request's entire journey across services. This ensures that errors are instantly traceable within the log aggregation tools.

---

## 📊 4. SLA MONITORING & ALERTS TOPOLOGY

Monitoring targets are aligned with production SLAs:

| Metric Type | Telemetry Source | SLA Target Bound | Alert Rule |
| :--- | :--- | :--- | :--- |
| **Availability Latency** | HTTP Inbound Span | $p95 \le 500\text{ ms}$ | High latency (>1000ms) over 5m. |
| **Booking Latency** | HTTP Inbound Span | $p50 \le 200\text{ ms} / p99 \le 800\text{ ms}$ | High booking times (>1500ms). |
| **Double-Booking Rate**| Database Logs | **Strictly 0%** | **CRITICAL SEV-0 ALERT** on duplicates. |