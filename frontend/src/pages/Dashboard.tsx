import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { clientsApi } from '../services/api';
import type { Client } from '../types';

export default function Dashboard() {
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    clientsApi.getAll()
      .then(setClients)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const activeClients = clients.filter(c => c.isActive).length;

  if (loading) return (
    <div className="loading-screen">
      <div className="spinner" />
      <p>Cargando dashboard...</p>
    </div>
  );

  return (
    <div className="fade-in">
      <div className="page-header">
        <h2>Dashboard</h2>
        <p>Bienvenido al Sistema de Gestión de Gimnasio</p>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon purple">👥</div>
          <div className="stat-info">
            <h3>{clients.length}</h3>
            <p>Total Clientes</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green">✅</div>
          <div className="stat-info">
            <h3>{activeClients}</h3>
            <p>Clientes Activos</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon amber">🎟️</div>
          <div className="stat-info">
            <h3>—</h3>
            <p>Membresías Activas</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue">🏋️</div>
          <div className="stat-info">
            <h3>—</h3>
            <p>Sesiones Este Mes</p>
          </div>
        </div>
      </div>

      <div className="card">
        <div className="card-header">
          <h3>Clientes Recientes</h3>
          <button className="btn btn-primary btn-sm" onClick={() => navigate('/clients')}>
            Ver todos
          </button>
        </div>
        {clients.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">👥</div>
            <h4>No hay clientes registrados</h4>
            <p>Registra el primer cliente del gimnasio</p>
            <button className="btn btn-primary" style={{ marginTop: 12 }} onClick={() => navigate('/clients')}>
              + Registrar cliente
            </button>
          </div>
        ) : (
          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Cliente</th>
                  <th>Email</th>
                  <th>Teléfono</th>
                  <th>Estado</th>
                  <th>Registrado</th>
                </tr>
              </thead>
              <tbody>
                {clients.slice(0, 5).map(c => (
                  <tr key={c.id} style={{ cursor: 'pointer' }} onClick={() => navigate(`/clients/${c.id}`)}>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                        <div className="avatar" style={{ width: 32, height: 32, fontSize: 12 }}>
                          {c.fullName.split(' ').map(w => w[0]).join('').slice(0, 2)}
                        </div>
                        <span style={{ fontWeight: 600 }}>{c.fullName}</span>
                      </div>
                    </td>
                    <td style={{ color: 'var(--text-secondary)' }}>{c.email}</td>
                    <td style={{ color: 'var(--text-secondary)' }}>{c.phone}</td>
                    <td>
                      <span className={`badge ${c.isActive ? 'badge-success' : 'badge-muted'}`}>
                        {c.isActive ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                    <td style={{ color: 'var(--text-secondary)', fontSize: 13 }}>
                      {new Date(c.createdAt).toLocaleDateString('es-PE')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
