import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../Components/Button';
import { Service1, Gateway } from '../api';
import GetInputComponent from '../Components/GetInput';

export default function Service1Page() {
    const [flights, setFlights] = useState([]);
    // let newFlights = [];
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleFetchFlights = () => {
        Service1.get('/VluchtenService/AllVluchtenExports')
            .then(res => {
                setFlights(res.data);
                setError(null); // Clear any previous error

            })
            .catch(err => {
                console.error("API Error:", err);
                setError("Failed to load flights");
            });
    };

    const [selectedFlight, setSelectedFlight] = useState(null);

    const handleFlightByflightId = (flightId) => {

        if (!flightId) {
            alert("Please enter a flight ID.");
            return;
        }

        if (isNaN(flightId)) {
            alert("Invalid flight ID. Please enter a numeric value.");
            return;
        }

        if (flightId <= 0) {
            alert("Invalid flight ID. Please enter a positive value.");
            return;
        }

        Service1.get(`/VluchtenService/entry?flightId=${flightId}`)
            .then(res => {
                console.log("Flight Details:", res.data);
                setSelectedFlight(res.data);
                setError(null); // Clear any previous error
            })

            .catch(err => {
                console.error("API Error:", err);
                setError("Failed to load flight details");
            });
    };

    const handleGatewayNavigation = () => {
        navigate('/gateway');
    };

    function GetInput() {
        const input = prompt("Please enter flight ID:");
        return input;
    }

    return (
        <div>
            <h1>Welcome to Service1</h1>
            <h2>Exported Flights</h2>

            <Button text="Fetch Flights" onClick={handleFetchFlights} />
            <br />
            <br />
            <Button text="Fetch Flight by ID" onClick={() => handleFlightByflightId(GetInput())} />
            <br />
            <br />
            <Button text="Go to Gateway" onClick={handleGatewayNavigation} />


            {error && <p style={{ color: 'red' }}>{error}</p>}

            {selectedFlight ? (
                <div>
                    <h3>Flight Details</h3>
                    <p>Flight ID: {selectedFlight.flightId}</p>
                    <a
                        href={`http://localhost:5041/VluchtenService/entry?flightId=${selectedFlight.flightId}`}
                        target="_blank"
                        rel="noopener noreferrer"
                    >
                        Open Flight Entry
                    </a>
                    <br />
                    <button
                        onClick={() => {
                            setSelectedFlight(null);
                            setFlights([]);
                        }}
                    >
                        Clear List
                    </button>
                </div>
            ) : (
                flights && flights.length > 0 ? (
                    <div>
                        <button onClick={() => setFlights([])}>Clear List</button>
                        <ul>
                            {flights.map(f => (
                                <li key={f.flightId}>
                                    Flight ID: {f.flightId} –{" "}
                                    <a href="#" onClick={() => setSelectedFlight(f)}>
                                        View Details
                                    </a>
                                </li>
                            ))}
                        </ul>
                    </div>
                ) : (
                    null // No flights to display
                )
            )}

        </div>
    );
}