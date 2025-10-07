# 🏢 Elevator System Simulation

An elementary **Elevator Control System** built with **.NET 8 (C#)** and a **React + TypeScript** frontend.  
The system simulates a 10-floor building with four elevators and models movement, direction, and passenger requests — both random and user-triggered — in real-time via **SignalR**.

---

## 🚀 Overview

This project demonstrates a simple elevator dispatch and movement simulation.  
It focuses on **core elevator logic** rather than physical or real-world concerns like:
- Load limits  
- Fire control systems  
- Priority overrides  
- Emergency holds  

The solution adheres to clean architectural separation between API, core logic, and tests.

---

## 🧱 System Parameters

| Parameter | Value |
|------------|--------|
| **Floors** | 10 |
| **Elevators** | 4 |
| **Travel Time (per floor)** | 10 seconds |
| **Stop Time (boarding/alighting)** | 10 seconds |
| **Simulation Delay** | Configurable (default 10 seconds per step) |

---

## 🧩 Project Structure

ElevatorSystem.sln
│
├── ElevatorSystem.Api/ → ASP.NET Core Web API
│ ├── Controllers/ → API endpoints (SignalR hub, elevator actions)
│ ├── Middleware/ → Cross-cutting logic
│ ├── Program.cs → Entry point
│ └── appsettings.json → API configuration
│
├── ElevatorSystem.Core/ → Core logic (domain + simulation)
│ ├── Contracts/ → Interfaces (e.g., IElevatorService)
│ ├── Enum/ → Direction, ElevatorState enums
│ ├── Models/ → Elevator, FloorRequest, etc.
│ ├── Services/
│ │ ├── ElevatorService.cs → Core logic for request handling and dispatch
│ │ └── SimulationBackgroundService.cs → Background loop for random requests
│ └── State/ → (Optional) shared state models
│
├── ElevatorSystem.Api.RegressionTest/ → API-level regression tests (MSTest)
│ └── ElevatorApiTests.cs
│
├── ElevatorSystem.Core.RegressionTests/ → Core logic regression tests (MSTest)
│ └── ElevatorCoreTests.cs
│
└── client/ (React Frontend)
├── src/
│ ├── components/ → Elevator panel UI
│ ├── services/ → SignalR connection to /elevatorHub
│ └── App.tsx → Real-time visualization of elevators
└── package.json



---

## ⚙️ Core Simulation Logic

### Elevator Behavior
- Each elevator tracks its current floor, direction, and destination queue.  
- An **up-moving** elevator continues upward until all higher destinations are served.  
- Similarly, a **down-moving** elevator continues downward until all lower destinations are cleared.  
- Elevators idle when they have no destinations.

### Dispatch Algorithm
When a new request is generated:
1. The system evaluates all elevators.
2. It selects the most suitable one using a **simple scoring heuristic**:
   - Idle elevators are prioritized.
   - Elevators closer to the requested floor score higher.
   - Elevators with fewer destinations are preferred.
3. The chosen elevator receives the new floor request.

### Background Simulation
- The `SimulationBackgroundService` runs continuously using `BackgroundService`.
- At each simulation tick:
  - It randomly generates new floor requests (60% probability).
  - Invokes `StepAllAsync()` on `ElevatorService` to move elevators one step.
  - Waits for the configured delay before the next tick.

---

## 💻 Frontend (React + TypeScript)

The frontend connects via **SignalR** to `/elevatorHub` to display:
- Live elevator positions and directions.
- New incoming requests (Up/Down).
- Logs of movements and assignments.

**Tech stack:**  
React, TypeScript, TailwindCSS, SignalR client.

---

## 🧪 Testing

### ✅ Core Regression Tests (`ElevatorSystem.Core.RegressionTests`)
- Validates elevator dispatch logic.
- Ensures requests are distributed among elevators.
- Verifies movement, direction, and request clearing.

### ✅ API Regression Tests (`ElevatorSystem.Api.RegressionTest`)
- Ensures API endpoints return correct states.
- Tests SignalR hub message contracts (if applicable).

All tests use **MSTest** and **Moq** for mocking.

Run tests from solution root:
```bash
dotnet test


Initila settings -- can be configured in AppSettings.Json file
"ElevatorSettings": {
  "NumberOfElevators": 4,
  "NumberOfFloors": 10,
  "TimeToTravelOneFloorInSeconds": 10,
  "TimeOnFloorInSeconds": 10
}
