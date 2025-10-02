import React, { useEffect, useState } from "react";

type ElevatorState = {
  id: number;
  currentFloor: number;
  direction: string;
  requests: number[];
};

const floors = Array.from({ length: 10 }, (_, i) => 10 - i); // Floors 10 → 1

const App: React.FC = () => {
  const [elevators, setElevators] = useState<ElevatorState[]>([]);
  const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5000";

  useEffect(() => {
    const fetchElevators = async () => {
      try {
        const res = await fetch(`${API_URL}/api/elevator`);
        const data = await res.json();
        setElevators(data);
      } catch (err) {
        console.error("Error fetching elevators:", err);
      }
    };

    // Fetch immediately + every second
    fetchElevators();
    const interval = setInterval(fetchElevators, 1000);
    return () => clearInterval(interval);
  }, []);

  const sendRandomRequest = async () => {
    await fetch(`${API_URL}/api/elevator/request`, { method: "POST" });
  };

  return (
    <div style={styles.container}>
      <h1>🏢 Elevator Simulation (Polling)</h1>
      <div style={styles.simulation}>
        {elevators.map(e => (
          <div key={e.id} style={styles.elevatorCol}>
            <div style={{ textAlign: "center", marginBottom: "4px" }}>
              <b>Elevator {e.id}</b>
            </div>
            <div style={styles.floors}>
              {floors.map(floor => (
                <div key={floor} style={styles.floorBox}>
                  {e.currentFloor === floor && (
                    <div style={styles.car}>{floor}</div>
                  )}
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
      <button style={styles.button} onClick={sendRandomRequest}>
        Call Random Request
      </button>
    </div>
  );
};

// ----------------- styles -----------------
const styles: { [key: string]: React.CSSProperties } = {
  container: {
    fontFamily: "Arial, sans-serif",
    padding: "20px",
  },
  simulation: {
    display: "flex",
    gap: "20px",
    marginBottom: "20px",
  },
  elevatorCol: {
    border: "1px solid #aaa",
    padding: "4px",
    width: "80px",
    display: "flex",
    flexDirection: "column",
  },
  floors: {
    display: "flex",
    flexDirection: "column",
    flex: 1,
  },
  floorBox: {
    border: "1px solid #ddd",
    height: "40px",
    position: "relative",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
  },
  car: {
    background: "blue",
    color: "white",
    width: "100%",
    height: "100%",
    textAlign: "center",
    lineHeight: "40px",
  },
  button: {
    padding: "10px 20px",
    fontSize: "16px",
  },
};

export default App;
