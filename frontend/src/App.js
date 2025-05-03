import { BrowserRouter as Router, Routes, Route, useNavigate } from 'react-router-dom';
import Service1Page from './Pages/Service1';
import GatewayPage from './Pages/Gateway';
import Button from './Components/Button';

function Home() {
  const navigate = useNavigate();

  return (
    <div>
      <Button text="Service1" onClick={() => navigate('/service1')} />
      <br /><br />
      <Button text="Gateway" onClick={() => navigate('/gateway')} />
    </div>
  );
}

export default function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/service1" element={<Service1Page />} />
        <Route path="/gateway" element={<GatewayPage />} />
      </Routes>
    </Router>
  );
}


{ }