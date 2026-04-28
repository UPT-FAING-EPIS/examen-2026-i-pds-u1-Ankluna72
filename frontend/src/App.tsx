import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Dashboard from './pages/Dashboard';
import ClientsPage from './pages/ClientsPage';
import ClientDetailPage from './pages/ClientDetailPage';
import MembershipsPage from './pages/MembershipsPage';
import SessionsPage from './pages/SessionsPage';

export default function App() {
  return (
    <BrowserRouter>
      <div className="app-layout">
        <Sidebar />
        <main className="main-content">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/clients" element={<ClientsPage />} />
            <Route path="/clients/:id" element={<ClientDetailPage />} />
            <Route path="/memberships" element={<MembershipsPage />} />
            <Route path="/sessions" element={<SessionsPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}
