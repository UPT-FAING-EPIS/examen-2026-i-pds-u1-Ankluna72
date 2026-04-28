import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { clientsApi } from '../services/api';
import type { Client, CreateClientDto } from '../types';
import Avatar from '../components/Avatar';

export default function ClientsPage() {
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);
  const navigate = useNavigate();

  const [form, setForm] = useState<CreateClientDto>({
    firstName: '', lastName: '', email: '', phone: '', birthDate: ''
  });

  const loadClients = () => {
    setLoading(true);
    clientsApi.getAll().then(setClients).catch(console.error).finally(() => setLoading(false));
  };

  useEffect(() => { loadClients(); }, []);

  const filtered = clients.filter(c =>
    c.fullName.toLowerCase().includes(search.toLowerCase()) ||
    c.email.toLowerCase().includes(search.toLowerCase())
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSaving(true);
    try {
      await clientsApi.create(form);
      setShowModal(false);
      setForm({ firstName: '', lastName: '', email: '', phone: '', birthDate: '' });
      loadClients();
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Error al registrar cliente.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fade-in">
      <div className="page-header">
        <h2>Clientes</h2>
        <p>Gestiona los clientes registrados en el gimnasio</p>
      </div>

      <div className="search-bar">
        <div className="search-input-wrapper">
          <span className="search-icon">🔍</span>
          <input
            id="client-search"
            type="text"
            placeholder="Buscar por nombre o email..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <button id="btn-new-client" className="btn btn-primary" onClick={() => setShowModal(true)}>
          + Nuevo Cliente
        </button>
      </div>

      <div className="card">
        <div className="card-header">
          <h3>Lista de Clientes <span style={{ color: 'var(--text-muted)', fontWeight: 400 }}>({filtered.length})</span></h3>
        </div>
        {loading ? (
          <div className="loading-screen"><div className="spinner" /></div>
        ) : filtered.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">👥</div>
            <h4>{search ? 'Sin resultados' : 'No hay clientes'}</h4>
            <p>{search ? 'Intenta con otro término' : 'Registra el primer cliente'}</p>
          </div>
        ) : (
          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Cliente</th>
                  <th>Email</th>
                  <th>Teléfono</th>
                  <th>Fecha Nacimiento</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {filtered.map(c => (
                  <tr key={c.id}>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                        <Avatar name={c.fullName} size={34} />
                        <div>
                          <div style={{ fontWeight: 600 }}>{c.fullName}</div>
                          <div style={{ fontSize: 12, color: 'var(--text-muted)' }}>ID #{c.id}</div>
                        </div>
                      </div>
                    </td>
                    <td style={{ color: 'var(--text-secondary)' }}>{c.email}</td>
                    <td style={{ color: 'var(--text-secondary)' }}>{c.phone}</td>
                    <td style={{ color: 'var(--text-secondary)' }}>
                      {new Date(c.birthDate).toLocaleDateString('es-PE')}
                    </td>
                    <td>
                      <span className={`badge ${c.isActive ? 'badge-success' : 'badge-muted'}`}>
                        {c.isActive ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                    <td>
                      <button
                        id={`btn-view-client-${c.id}`}
                        className="btn btn-secondary btn-sm"
                        onClick={() => navigate(`/clients/${c.id}`)}
                      >
                        Ver detalle
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Create Client Modal */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal slide-up" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Registrar Nuevo Cliente</h3>
              <button className="modal-close" onClick={() => setShowModal(false)}>✕</button>
            </div>
            {error && <div className="alert alert-error">⚠️ {error}</div>}
            <form onSubmit={handleSubmit}>
              <div className="form-grid">
                <div className="form-group">
                  <label htmlFor="firstName">Nombres</label>
                  <input
                    id="firstName"
                    type="text"
                    required
                    placeholder="Juan"
                    value={form.firstName}
                    onChange={e => setForm({ ...form, firstName: e.target.value })}
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="lastName">Apellidos</label>
                  <input
                    id="lastName"
                    type="text"
                    required
                    placeholder="Pérez García"
                    value={form.lastName}
                    onChange={e => setForm({ ...form, lastName: e.target.value })}
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="email">Email</label>
                  <input
                    id="email"
                    type="email"
                    required
                    placeholder="juan@email.com"
                    value={form.email}
                    onChange={e => setForm({ ...form, email: e.target.value })}
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="phone">Teléfono</label>
                  <input
                    id="phone"
                    type="text"
                    placeholder="999-123-456"
                    value={form.phone}
                    onChange={e => setForm({ ...form, phone: e.target.value })}
                  />
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label htmlFor="birthDate">Fecha de Nacimiento</label>
                  <input
                    id="birthDate"
                    type="date"
                    required
                    value={form.birthDate}
                    onChange={e => setForm({ ...form, birthDate: e.target.value })}
                  />
                </div>
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>
                  Cancelar
                </button>
                <button id="btn-save-client" type="submit" className="btn btn-primary" disabled={saving}>
                  {saving ? <><span className="spinner" style={{width:16,height:16}} /> Guardando...</> : '✓ Registrar Cliente'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
