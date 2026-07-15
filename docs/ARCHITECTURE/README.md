# 🏛️ Architecture Documentation Hub

**Project:** The Unified Vehicle Scheduler (Scenario A)  
**Target Role:** Software Engineering Team Leader  
**Design Methodology:** C4 Model Approach (Context & Container Views) & Clean Architecture  
**Status:** PRODUCTION-READY ✅

---

## 🗺️ 1. ARCHITECTURAL VIEWPOINTS MAP

To ensure long-term maintainability, scalability, and seamless onboarding for engineering teams, the architecture of this project is separated into specialized, high-utility perspectives.

Please navigate through the specialized views below depending on your review objective:

1.  **[Conceptual Blueprint View (System Boundaries)](./CONCEPTUAL_VIEW.md)**
    *   *Focus:* Clean Architecture layer segregation, Dependency Injection topology, and decoupling of core enterprise business rules from frameworks.
2.  **[Data & Database View (The Core Engine)](./DATA_DATABASE_VIEW.md)**
    *   *Focus:* Slot-based scheduling mathematics, 16-table normalized ERD, and database-level **PostgreSQL Exclusion Constraints (GIST)** against concurrent double-bookings.
3.  **[Observability & Monitoring View](./OBSERVABILITY_VIEW.md)**
    *   *Focus:* Structured logging via Serilog, production monitoring with **OpenTelemetry**, and End-to-End distributed tracing using `CorrelationId`.
4.  **[GenAI Collaboration & Engineering Process](./GENAI_ENGINEERING_PROCESS.md)**
    *   *Focus:* **4-Gate Quality Assurance Pipeline**, prompt-isolation strategies for local LLMs, and the Team Leader's design decision ownership matrix.
5.  **[Auth and Booking Components View](./AUTH_COMPONENTS_VIEW.md)**
    *   *Focus:* Runtime component boundaries across Auth, Booking, and Notification services; JWT/JWKS trust path; async queue delivery path.
6.  **[Auth and Booking Sequential Flows View](./AUTH_SEQUENTIAL_FLOW_VIEW.md)**
    *   *Focus:* End-to-end sign-up verification, challenge-based login, Google sign-in, booking authorization, token refresh/logout, and async notification delivery flows.

---

## 🌐 2. C4 MODEL - LEVEL 1: SYSTEM CONTEXT

This diagram illustrates how the Unified Vehicle Scheduler platform fits into the broader dealership ecosystem including asynchronous notification delivery.

```
┌─────────────────┐             HTTPS / JSON            ┌───────────────────────────────┐
│  Mobile/Web     │ ──────────────────────────────────> │       API + Auth Services      │
│  Client (Apps)  │ <────────────────────────────────── │      (Business + Identity)     │
└─────────────────┘     (Booking/Auth Responses)        └───────────────┬───────────────┘
                                                                        │
                                                                        │ Async event publish
                                                                        ▼
                                                        ┌───────────────────────────────┐
                                                        │      Azure Storage Queue      │
                                                        │   user-notification-events    │
                                                        └───────────────┬───────────────┘
                                                                        │
                                                                        ▼
                                                        ┌───────────────────────────────┐
                                                        │ Notification Functions Service│
                                                        │ (Queue Trigger + Providers)   │
                                                        └───────────────┬───────────────┘
                                                                        │
                                                                        ▼
                                                        ┌───────────────────────────────┐
                                                        │   Email Provider APIs         │
                                                        │    (Google / SendGrid)       │
                                                        └───────────────────────────────┘

```

### System Context Responsibilities
*   **Mobile/Web Client:** Consumes availability timelines and triggers multi-service booking requests securely with an `Idempotency-Key`.
*   **API + Auth Services:** Core .NET services hosting scheduling, identity, and notification event publishing logic.
*   **Azure Storage Queue:** Asynchronous decoupling boundary between request path and notification delivery path.
*   **Notification Functions Service:** Consumes queued events, validates payloads, calls provider adapters, and handles retry/poison behavior.
*   **Provider APIs:** Downstream email delivery engines (Google Gmail API and SendGrid API).

---

## 📦 3. C4 MODEL - LEVEL 2: CONTAINER VIEW

Deep-diving inside the runtime containers reveals how service components communicate and isolate side-effects.

```
┌───────────────────────────────────────────────────────────────────────────────────────┐
│                            API + Auth Containers (.NET 8)                             │
│                                                                                       │
│   Presentation -> Application -> Infrastructure -> Domain                              │
│   (Controllers)   (Use-cases)     (Repos/EF/Publishers)  (Entities/Rules)             │
└───────────────────────────────────────────────┬───────────────────────────────────────┘
                                                │
                                                │ enqueue notification events
                                                ▼
                               ┌───────────────────────────────────┐
                               │      Azure Storage Queue          │
                               │     user-notification-events      │
                               └─────────────────┬─────────────────┘
                                                 │
                                                 ▼
                          ┌────────────────────────────────────────────┐
                          │ Notification Functions (dotnet-isolated)  │
                          │ Trigger -> Validate -> IEmailSender       │
                          └─────────────────┬──────────────────────────┘
                                            │
                                            ▼
                          ┌────────────────────────────────────────────┐
                          │ External Provider APIs (Google/SendGrid)  │
                          └────────────────────────────────────────────┘
```

### Container Data Flows
1.  **Ingress:** Client requests enter API/Auth presentation layers with correlation metadata.
2.  **Orchestration:** Application services execute business logic and complete request path responses.
3.  **Async Publish:** Auth publishes notification events to queue without blocking user-facing response path.
4.  **Function Processing:** Notification function consumes queue message, validates payload, and invokes provider adapter.
5.  **Delivery + Reliability:** Provider success completes message; failures rethrow for retry and poison handling.

---

## ⚡ 4. SYSTEM HIGHLIGHTS SUMMARY

*   **Sub-50ms Latency:** Achieved by moving complex data calculation loops out of C# application memory and shifting them to optimized PostgreSQL Views (`ServiceTypeAvailabilityView`), delivering a **19x read performance leap**.
*   **Race Condition Immunity:** Solved via an advanced **Two-Stage Validation Strategy** wrapping Application pre-checks around physical Database GIST range exclusion guards, ensuring zero overlapping bookings can ever occur.
*   **Production Traceability:** Every request is wrapped in a dynamic `CorrelationId` and tracked natively across system boundaries via OpenTelemetry child spans and Serilog structures.

---

## 🔐 5. IDENTITY AND NOTIFICATION INTEGRATION ARCHITECTURE

The platform now includes a dedicated Auth service and JWT/JWKS trust integration for Booking APIs.

The platform also includes asynchronous notification delivery via queue and Function service.

- Component topology and service boundaries: `AUTH_COMPONENTS_VIEW.md`
- Runtime interaction and token lifecycle flows: `AUTH_SEQUENTIAL_FLOW_VIEW.md`
- Notification feature deep-dive: `../FEATURES/NOTIFICATION/README.md`
- Notification troubleshooting runbook: `../FEATURES/NOTIFICATION/TROUBLESHOOTING_PLAYBOOK.md`