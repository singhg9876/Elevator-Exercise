import React, { useEffect, useState } from "react";

type Direction = "Up" | "Down" | "Idle";

interface ElevatorState {
  id: number;
  currentFloor: number;
  direction: Direction;
  requests: number[];
}

const floors = Array.from({ length: 10 }, (_, i) => 10 - i);

const API_URL = import.meta.env.VITE_API_URL || "https://localhost:7156";

const App: React.FC = () => {
  const [elevators, setElevators] = useState<ElevatorState[]>([]);
  const [floor, setFloor] = useState(1);
  const [error, setError] = useState<string | null>(null);

  // ----------------- Polling for elevator states -----------------
  useEffect(() => {
    const fetchElevators = async () => {
      try {
        const res = await fetch(`${API_URL}/api/elevator`);
        const data: ElevatorState[] = await res.json();
        setElevators(data);
      } catch (ex: any) {
        setError(ex?.message || "Failed to fetch elevators");
      }
    };

    fetchElevators(); // initial fetch
    const interval = setInterval(fetchElevators, 1000); // poll every 1s

    return () => clearInterval(interval);
  }, []);

  // ----------------- Send user request -----------------
  const sendRequest = async () => {
    try {
      // Determine direction for backend
      const nearestElevator = elevators.reduce((prev, curr) => {
        return Math.abs(curr.currentFloor - floor) < Math.abs(prev.currentFloor - floor)
          ? curr
          : prev;
      }, elevators[0]);

      const direction: Direction =
        floor > (nearestElevator?.currentFloor ?? 1) ? "Up" : "Down";

      await fetch(`${API_URL}/api/elevator/request`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ floor, direction }),
      });
    } catch (ex: any) {
      setError(ex?.message || "Failed to send request");
    }
  };

  // ----------------- Send random request -----------------
  const sendRandomRequest = async () => {
    try {
      const randomFloor = Math.floor(Math.random() * 10) + 1;
      const randomDir: Direction = Math.random() > 0.5 ? "Up" : "Down";

      await fetch(`${API_URL}/api/elevator/request`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ floor: randomFloor, direction: randomDir }),
      });
    } catch (ex: any) {
      setError(ex?.message || "Failed to send random request");
    }
  };

  // ----------------- Render -----------------
  const renderShaft = (elevator: ElevatorState, floorNumber: number) => {
    const isHere = elevator.currentFloor === floorNumber;
    return (
      <div
        key={floorNumber}
        style={{
          width: "60px",
          height: "40px",
          border: "1px solid #ddd",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          backgroundColor: isHere ? "blue" : undefined,
          color: isHere ? "white" : undefined,
        }}
      >
        {isHere ? `Car ${elevator.id}` : ""}
      </div>
    );
  };

  return (
    <div style={{ fontFamily: "Arial, sans-serif", padding: "20px" }}>
      <h1>🏢 Elevator Simulation</h1>

      {error && <div style={{ color: "red" }}>{error}</div>}

      <div style={{ display: "flex", gap: "20px", marginTop: "20px" }}>
        {elevators.map(elevator => (
          <div key={elevator.id} style={{ display: "flex", flexDirection: "column" }}>
            <div style={{ textAlign: "center", marginBottom: "4px" }}>
              <b>Elevator {elevator.id}</b>
            </div>
            {floors.map(f => renderShaft(elevator, f))}
          </div>
        ))}
      </div>

      {/* Control Panel */}
      <div style={{ marginTop: "20px" }}>
        <label>
          Floor:
          <select value={floor} onChange={e => setFloor(+e.target.value)} style={{ marginLeft: "5px" }}>
            {Array.from({ length: 10 }, (_, i) => i + 1).map(f => (
              <option key={f} value={f}>
                {f}
              </option>
            ))}
          </select>
        </label>

        <button style={{ marginLeft: "10px", padding: "5px 15px" }} onClick={sendRequest}>
          Send Request
        </button>

        <button style={{ marginLeft: "10px", padding: "5px 15px" }} onClick={sendRandomRequest}>
          Random Request
        </button>
      </div>
    </div>
  );
};

export default App;
