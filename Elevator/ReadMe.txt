This project is a .NET Core Web API with a simple React frontend to simulate an elevator system.
It demonstrates multi-elevator coordination, request handling, and visualization.
Ready to be deployed on docker and kubernetes

ElevatorSolution/
│
├── ElevatorApi/               # Web API project
│   ├── Controllers/
│   │   └── ElevatorController.cs  # API endpoints
│   ├── Services/
│   │   └── ElevatorService.cs     # Core elevator simulation logic
│   │   └── SimulationBackgroundService.cs # Background task (steps elevators)
│
├── ElevatorApi.Tests/         # MSTest unit tests
│   └── ElevatorServiceTests.cs    # Unit tests for service methods
│
└── elevator-frontend/         # React app (UI)
    ├── src/
    │   ├── App.tsx            # Polls API, shows elevators visually
    

Features Implemented

    => Multi-elevator system.
    => Request queue & intelligent assignment to nearest elevator.
    => Random + user-driven requests.
    => Background stepping simulation.
    => Unit tests for validation.
    => Simple React frontend for visualization.