import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../Components/Button';
import { Service1, Gateway } from '../api';

export default function GatewayPage() {
    const [flights, setFlights] = useState([]);
    // let newFlights = [];
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleFetchFlights = () => {
        Gateway.get('/gateway/AllFlights')
            .then(res => {
                setFlights(res.data);
                console.log("Fetched flights:", res.data);
            })
            .catch(err => {
                console.error("API Error:", err);
                setError("Failed to load flights");
            });
    };


    const handleGatewayNavigation = () => {
        navigate('/service1');
    };

    return (
        <div>
            <h1>Welcome to Gateway</h1>
            <h2>Exported Flights</h2>

            <Button text="Fetch Flights" onClick={handleFetchFlights} />
            <br />
            <br />
            <Button text="Go to Gateway" onClick={handleGatewayNavigation} />

            {error && <p style={{ color: 'red' }}>{error}</p>}

            <ul>
                {flights.map(f => (
                    <li key={f.flightId}>
                        Flight ID: {f.flightId} – <a href={f.detailUrl}>View Details</a>
                    </li>
                ))}
            </ul>
        </div>
    );
}