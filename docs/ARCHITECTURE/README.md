# 🏛️ Architecture Documentation Hub

**Project:** The Unified Vehicle Scheduler (Scenario A)  
**Target Role:** Software Engineering Team Leader  
**Design Methodology:** C4 Model Approach (Context & Container Views) & Clean Architecture  
**Status:** PRODUCTION-READY ✅

---

## 🗺️ 1. ARCHITECTURAL VIEWPOINTS MAP

To ensure long-term maintainability, scalability, and seamless onboarding for engineering teams, the architecture of this project is separated into four specialized, high-utility perspectives. 

Please navigate through the specialized views below depending on your review objective:

1.  **[Conceptual Blueprint View (System Boundaries)](./CONCEPTUAL_VIEW.md)**
    *   *Focus:* Clean Architecture layer segregation, Dependency Injection topology, and decoupling of core enterprise business rules from frameworks.
2.  **[Data & Database View (The Core Engine)](./DATA_DATABASE_VIEW.md)**
    *   *Focus:* Slot-based scheduling mathematics, 16-table normalized ERD, and database-level **PostgreSQL Exclusion Constraints (GIST)** against concurrent double-bookings.
3.  **[Observability & Monitoring View (Day-2 Operations)](./OBSERVABILITY_VIEW.md)**
    *   *Focus:* Structured logging via Serilog, production monitoring with **OpenTelemetry**, and End-to-End distributed tracing using `CorrelationId`.
4.  **[GenAI Collaboration & Engineering Process](./GENAI_ENGINEERING_PROCESS.md)**
    *   *Focus:* **4-Gate Quality Assurance Pipeline**, prompt-isolation strategies for local LLMs, and the Team Leader's design decision ownership matrix.

---

## 🌐 2. C4 MODEL - LEVEL 1: SYSTEM CONTEXT

This diagram illustrates how the Unified Vehicle Scheduler API fits into the broader Keyloop dealership ecosystem.

```
┌─────────────────┐             HTTPS / JSON            ┌───────────────────────────────┐
│  Mobile/Web     │ ──────────────────────────────────> │   Unified Vehicle Scheduler   │
│  Client (Apps)  │ <────────────────────────────────── │         (.NET 8 API)          │
└─────────────────┘         (Booking & Queries)         └───────────────┬───────────────┘
                                                                        │
                                                                        │ Reads / Writes
                                                                        ▼
                                                        ┌───────────────────────────────┐
                                                        │      PostgreSQL Engine        │
                                                        │  (Data Store & Constraints)   │
                                                        └───────────────────────────────┘

```

### System Context Responsibilities
*   **Mobile/Web Client:** Consumes availability timelines and triggers multi-service booking requests securely with an `Idempotency-Key`.
*   **Unified Vehicle Scheduler API:** The core .NET application hosting business invariants, input validation, and scheduling coordination logic.
*   **PostgreSQL Engine:** Stores persistent operational entities and acts as the final relational validator using transactional mathematical exclusion boundaries.

---

## 📦 3. C4 MODEL - LEVEL 2: CONTAINER VIEW

Deep-diving inside the API container reveals how the software components communicate asynchronously and isolate side-effects.

```
                  ┌────────────────────────────────────────────────────────┐
                  │                 VS Code / .NET 8 API                   │
                  │                                                        │
                  │   ┌─────────────────┐            ┌─────────────────┐   │
   HTTP Requests  │   │  Presentation   │            │   Application   │   │
─────────────────>│   │     Layer       │───────────>│     Layer       │   │
                  │   │ (Controllers)   │            │(Commands/Queries)│   │
                  │   └─────────────────┘            └────────┬────────┘   │
                  │                                           │            │
                  │                                           │ Uses       │
                  │                                           ▼            │
                  │   ┌─────────────────┐            ┌─────────────────┐   │
                  │   │  Infrastructure │            │     Domain      │   │
                  │   │     Layer       │───────────>│     Layer       │   │
                  │   │(EF Core/Serilog)│  Implements│(Business Entities│  │
                  │   └────────┬────────┘            └─────────────────┘   │
                  └────────────┼───────────────────────────────────────────┘
                               │
                               │ SQL / GIST Queries
                               ▼
               ┌───────────────────────────────┐
               │    PostgreSQL Database        │
               └───────────────────────────────┘

```

### Container Data Flows
1.  **Ingress:** API Gateway routes requests to the **Presentation Layer** (`Controllers`), mapping incoming telemetry headers (`Correlation-Id`, `Idempotency-Key`).
2.  **Orchestration:** Controllers forward thin DTOs to the **Application Layer** where FluentValidation intercepts and processes the data through custom commands.
3.  **Domain Execution:** Business workflows check rules against pure domain aggregates, maintaining total separation from concrete infrastructure details.
4.  **Persistence:** The **Infrastructure Layer** fulfills repository contracts using Entity Framework Core 8, communicating via connection strings directly into the **PostgreSQL** relational tables.

---

## ⚡ 4. SYSTEM HIGHLIGHTS SUMMARY

*   **Sub-50ms Latency:** Achieved by moving complex data calculation loops out of C# application memory and shifting them to optimized PostgreSQL Views (`ServiceTypeAvailabilityView`), delivering a **19x read performance leap**.
*   **Race Condition Immunity:** Solved via an advanced **Two-Stage Validation Strategy** wrapping Application pre-checks around physical Database GIST range exclusion guards, ensuring zero overlapping bookings can ever occur.
*   **Production Traceability:** Every request is wrapped in a dynamic `CorrelationId` and tracked natively across system boundaries via OpenTelemetry child spans and Serilog structures.