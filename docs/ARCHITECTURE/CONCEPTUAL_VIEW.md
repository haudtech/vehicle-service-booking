# 🏗️ Conceptual Architecture View

**Purpose:** High-level blueprint of the system structure, architectural patterns, layer boundaries, and dependency injection topology.
**Architectural Style:** Clean Architecture with Domain-Driven Design (DDD) principles
**Status:** CURRENT - REFLECTS ACTUAL CODEBASE IMPLEMENTATION ✅
**Document Version:** 3.0

---

## 🏛️ 1. LAYERED ARCHITECTURE (CLEAN ARCHITECTURE)

The application isolates core business invariants from external technical infrastructure, frameworks, and databases. Dependencies flow strictly **inward** toward the Domain Layer.

```
┌──────────────────────────────┐
│      PRESENTATION LAYER      │  Controllers, DTOs, OpenAPI
└──────────────┬───────────────┘
               │ HTTP
               ▼
┌──────────────┴───────────────┐
│      APPLICATION LAYER       │  Interfaces, Biz Services, Validation
└──────────────┬───────────────┘
               │ Implements
               ▼
┌──────────────┴───────────────┐
│     INFRASTRUCTURE LAYER     │  EF Core, Repositories, Observability
└──────────────┬───────────────┘
               │ Mapping
               ▼
┌──────────────┴───────────────┐
│         DOMAIN LAYER         │  Entities, Business Invariants
└──────────────────────────────┘

```