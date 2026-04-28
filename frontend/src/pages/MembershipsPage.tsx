import { useState, useEffect } from 'react';
import { clientsApi, membershipsApi } from '../services/api';
import type { Membership, Client, CreateMembershipDto } from '../types';
import StatusBadge from '../components/StatusBadge';
import Avatar from '../components/Avatar';

export default function MembershipsPage() {
  const [clients, setClients] = useState<Client[]>([]);
  const [memberships, setMemberships] = useState<Membership[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [filter, setFilter] = useState<string>('all');
  const [showModal, setShowModal] = useState(false);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const [form, setForm] = useState<CreateMembershipDto>({
    clientId: 0, planName: '', price: 0, startDate: '', endDate: ''
  });

  const loadData = async () => {
    setLoading(true);
    try {
      const cs = await clientsApi.getAll();
      setClients(cs);
      const allMemberships = await Promise.all(cs.map(c => clientsApi.getMemberships(c.id)));
      setMemberships(allMemberships.flat());
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadData(); }, []);

  const filtered = memberships
    .filter(m => filter === 'all' || m.statusName === filter)
    .filter(m => m.clientFullName?.toLowerCase().includes(search.toLowerCase()) || m.planName.toLowerCase().includes(search.toLowerCase()));

  const counts = {
    all: memberships.length,
    Active: memberships.filter(m => m.statusName === 'Active').length,
    Expired: memberships.filter(m => m.statusName === 'Expired').length,
    Suspended: memberships.filter(m => m.statusName === 'Suspended').length,
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(''); setSaving(true);
    try {
      await membershipsApi.create(form);
      setShowModal(false);
      setForm({ clientId: 0, planName: '', price: 0, startDate: '', endDate: '' });
      await loadData();
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Error al registrar membresía.');
    } finally { setSaving(false); }
  };

  const handleRenew = async (id: number, endDate: string) => {
    const days = prompt('¿Cuántos días deseas extender?', '30');
    if (!days) return;
    const d = new Date(endDate);
    d.setDate(d.getDate() + Number(days));
    try {
      await membershipsApi.renew(id, { newEndDate: d.toISOString() });
      await loadData();
    } catch (err: any) {
      alert(err.response?.data?.message ?? 'Error al renovar');
    }
  };

  return (
    <div className="fade-in">
      <div className="page-header">
        <h2>Membresías</h2>
        <p>Gestiona los planes de membresía de todos los clientes</p>
      </div>

      {/* Status filter tabs */}
      <div className="stats-grid" style={{ marginBottom: 20 }}>
        {[
          { key: 'all', label: 'Total', icon: '🎟️', count: counts.all, color: 'purple' },
          { key: 'Active', label: 'Activas', icon: '✅', count: counts.Active, color: 'green' },
          { key: 'Expired', label: 'Vencidas', icon: '⏰', count: counts.Expired, color: 'red' },
          { key: 'Suspended', label: 'Suspendidas', icon: '⏸️', count: counts.Suspended, color: 'amber' },
        ].map(item => (
          <div
            key={item.key}
            className="stat-card"
            style={{ cursor: 'pointer', borderColor: filter === item.key ? 'var(--color-primary)' : undefined }}
            onClick={() => setFilter(item.key)}
          >
            <div className={`stat-icon ${item.color}`}>{item.icon}</div>
            <div className="stat-info">
              <h3>{item.count}</h3>
              <p>{item.label}</p>
            </div>
          </div>
        ))}
      </div>

      <div className="search-bar">
        <div className="search-input-wrapper">
          <span className="search-icon">🔍</span>
          <input
            id="membership-search"
            type="text"
            placeholder="Buscar por cliente o plan..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <button id="btn-new-membership-global" className="btn btn-primary" onClick={() => { setShowModal(true); setError(''); }}>
          + Nueva Membresía
        </button>
      </div>

      <div className="card">
        {loading ? (
          <div className="loading-screen"><div className="spinner" /></div>
        ) : filtered.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">🎟️</div>
            <h4>Sin membresías</h4>
            <p>No se encontraron membresías con los filtros actuales</p>
          </div>
        ) : (
          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr><th>Cliente</th><th>Plan</th><th>Precio</th><th>Inicio</th><th>Vencimiento</th><th>Estado</th><th>Acciones</th></tr>
              </thead>
              <tbody>
                {filtered.map(m => {
                  const client = clients.find(c => c.id === m.clientId);
                  return (
                    <tr key={m.id}>
                      <td>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                          {client && <Avatar name={client.fullName} size={30} />}
                          <span style={{ fontWeight: 500 }}>{m.clientFullName}</span>
                        </div>
                      </td>
                      <td style={{ fontWeight: 600 }}>{m.planName}</td>
                      <td>S/ {m.price.toFixed(2)}</td>
                      <td style={{ color: 'var(--text-secondary)' }}>{new Date(m.startDate).toLocaleDateString('es-PE')}</td>
                      <td style={{ color: 'var(--text-secondary)' }}>{new Date(m.endDate).toLocaleDateString('es-PE')}</td>
                      <td><StatusBadge status={m.statusName} /></td>
                      <td>
                        {m.statusName === 'Active' && (
                          <button className="btn btn-success btn-sm" onClick={() => handleRenew(m.id, m.endDate)}>
                            🔄 Renovar
                          </button>
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal slide-up" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Nueva Membresía</h3>
              <button className="modal-close" onClick={() => setShowModal(false)}>✕</button>
            </div>
            {error && <div className="alert alert-error">⚠️ {error}</div>}
            <form onSubmit={handleCreate}>
              <div className="form-grid">
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Cliente</label>
                  <select required value={form.clientId || ''} onChange={e => setForm({ ...form, clientId: Number(e.target.value) })}>
                    <option value="">Seleccionar cliente...</option>
                    {clients.filter(c => c.isActive).map(c => (
                      <option key={c.id} value={c.id}>{c.fullName}</option>
                    ))}
                  </select>
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Plan</label>
                  <select required value={form.planName} onChange={e => setForm({ ...form, planName: e.target.value })}>
                    <option value="">Seleccionar plan...</option>
                    <option value="Plan Mensual">Plan Mensual</option>
                    <option value="Plan Trimestral">Plan Trimestral</option>
                    <option value="Plan Semestral">Plan Semestral</option>
                    <option value="Plan Anual">Plan Anual</option>
                  </select>
                </div>
                <div className="form-group">
                  <label>Precio (S/)</label>
                  <input type="number" step="0.01" min="0" required placeholder="50.00"
                    value={form.price || ''} onChange={e => setForm({ ...form, price: parseFloat(e.target.value) })} />
                </div>
                <div className="form-group">
                  <label>Fecha de Inicio</label>
                  <input type="date" required value={form.startDate} onChange={e => setForm({ ...form, startDate: e.target.value })} />
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Fecha de Vencimiento</label>
                  <input type="date" required value={form.endDate} onChange={e => setForm({ ...form, endDate: e.target.value })} />
                </div>
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>Cancelar</button>
                <button type="submit" className="btn btn-primary" disabled={saving}>
                  {saving ? 'Guardando...' : '✓ Registrar Membresía'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
