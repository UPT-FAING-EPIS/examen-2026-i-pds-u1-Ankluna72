import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { clientsApi, membershipsApi, workoutSessionsApi } from '../services/api';
import type { Client, Membership, WorkoutSession, CreateMembershipDto, CreateWorkoutSessionDto } from '../types';
import StatusBadge from '../components/StatusBadge';
import Avatar from '../components/Avatar';

type Tab = 'memberships' | 'sessions';

export default function ClientDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const clientId = Number(id);

  const [client, setClient] = useState<Client | null>(null);
  const [memberships, setMemberships] = useState<Membership[]>([]);
  const [sessions, setSessions] = useState<WorkoutSession[]>([]);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState<Tab>('memberships');
  const [showMembershipModal, setShowMembershipModal] = useState(false);
  const [showSessionModal, setShowSessionModal] = useState(false);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const [membershipForm, setMembershipForm] = useState<CreateMembershipDto>({
    clientId, planName: '', price: 0, startDate: '', endDate: ''
  });
  const [sessionForm, setSessionForm] = useState<CreateWorkoutSessionDto>({
    clientId, sessionDate: '', startTime: '', notes: ''
  });

  const loadData = async () => {
    setLoading(true);
    try {
      const [c, m, s] = await Promise.all([
        clientsApi.getById(clientId),
        clientsApi.getMemberships(clientId),
        clientsApi.getWorkoutSessions(clientId),
      ]);
      setClient(c);
      setMemberships(m);
      setSessions(s);
    } catch {
      navigate('/clients');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadData(); }, [clientId]);

  const handleCreateMembership = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(''); setSaving(true);
    try {
      await membershipsApi.create(membershipForm);
      setShowMembershipModal(false);
      await loadData();
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Error al registrar membresía.');
    } finally { setSaving(false); }
  };

  const handleCreateSession = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(''); setSaving(true);
    try {
      await workoutSessionsApi.create(sessionForm);
      setShowSessionModal(false);
      await loadData();
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Error al registrar sesión.');
    } finally { setSaving(false); }
  };

  const handleRenew = async (membershipId: number) => {
    const days = prompt('¿Cuántos días deseas extender la membresía?', '30');
    if (!days) return;
    const current = memberships.find(m => m.id === membershipId);
    if (!current) return;
    const newDate = new Date(current.endDate);
    newDate.setDate(newDate.getDate() + Number(days));
    try {
      await membershipsApi.renew(membershipId, { newEndDate: newDate.toISOString() });
      await loadData();
    } catch (err: any) {
      alert(err.response?.data?.message ?? 'Error al renovar membresía.');
    }
  };

  const daysUntilExpiry = (endDate: string) => {
    const diff = new Date(endDate).getTime() - Date.now();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  };

  if (loading) return <div className="loading-screen"><div className="spinner" /><p>Cargando cliente...</p></div>;
  if (!client) return null;

  const activeMembership = memberships.find(m => m.statusName === 'Active');

  return (
    <div className="fade-in">
      {/* Header */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 24 }}>
        <button className="btn btn-secondary btn-sm" onClick={() => navigate('/clients')}>← Volver</button>
        <div style={{ flex: 1 }} />
        <button id="btn-new-membership" className="btn btn-primary btn-sm" onClick={() => { setShowMembershipModal(true); setError(''); }}>
          + Nueva Membresía
        </button>
        <button id="btn-new-session" className="btn btn-secondary btn-sm" onClick={() => { setShowSessionModal(true); setError(''); }}>
          + Nueva Sesión
        </button>
      </div>

      {/* Client Profile Card */}
      <div className="card" style={{ marginBottom: 20 }}>
        <div style={{ display: 'flex', gap: 20, alignItems: 'flex-start', flexWrap: 'wrap' }}>
          <Avatar name={client.fullName} size={64} />
          <div style={{ flex: 1 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12, flexWrap: 'wrap' }}>
              <h2 style={{ fontSize: 22, fontWeight: 800 }}>{client.fullName}</h2>
              <span className={`badge ${client.isActive ? 'badge-success' : 'badge-muted'}`}>
                {client.isActive ? 'Activo' : 'Inactivo'}
              </span>
              {activeMembership && <StatusBadge status={activeMembership.statusName} />}
            </div>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: 8, marginTop: 12 }}>
              <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                <span>📧</span> {client.email}
              </div>
              <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                <span>📞</span> {client.phone}
              </div>
              <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                <span>🎂</span> {new Date(client.birthDate).toLocaleDateString('es-PE')}
              </div>
              <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                <span>📅</span> Desde {new Date(client.createdAt).toLocaleDateString('es-PE')}
              </div>
            </div>
          </div>
        </div>

        {/* Active membership bar */}
        {activeMembership && (
          <div style={{ marginTop: 20, paddingTop: 20, borderTop: '1px solid var(--border)' }}>
            <div className="expiry-label">
              <span>Membresía: <strong>{activeMembership.planName}</strong></span>
              <span style={{ color: daysUntilExpiry(activeMembership.endDate) < 7 ? 'var(--color-danger)' : 'var(--text-secondary)' }}>
                {daysUntilExpiry(activeMembership.endDate) > 0
                  ? `Vence en ${daysUntilExpiry(activeMembership.endDate)} días`
                  : 'Vencida'}
              </span>
            </div>
            <div className="progress-bar">
              <div
                className="progress-bar-fill"
                style={{
                  width: `${Math.max(0, Math.min(100, (daysUntilExpiry(activeMembership.endDate) / 30) * 100))}%`,
                  background: daysUntilExpiry(activeMembership.endDate) < 7
                    ? 'var(--color-danger)' : 'linear-gradient(90deg, var(--color-primary), var(--color-accent))'
                }}
              />
            </div>
          </div>
        )}
      </div>

      {/* Tabs */}
      <div className="tabs">
        <div className={`tab ${tab === 'memberships' ? 'active' : ''}`} onClick={() => setTab('memberships')}>
          🎟️ Membresías ({memberships.length})
        </div>
        <div className={`tab ${tab === 'sessions' ? 'active' : ''}`} onClick={() => setTab('sessions')}>
          🏋️ Sesiones ({sessions.length})
        </div>
      </div>

      {/* Memberships Tab */}
      {tab === 'memberships' && (
        <div className="card">
          {memberships.length === 0 ? (
            <div className="empty-state">
              <div className="empty-state-icon">🎟️</div>
              <h4>Sin membresías</h4>
              <p>Registra la primera membresía del cliente</p>
            </div>
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr><th>Plan</th><th>Precio</th><th>Inicio</th><th>Vencimiento</th><th>Estado</th><th>Acciones</th></tr>
                </thead>
                <tbody>
                  {memberships.map(m => (
                    <tr key={m.id}>
                      <td style={{ fontWeight: 600 }}>{m.planName}</td>
                      <td>S/ {m.price.toFixed(2)}</td>
                      <td style={{ color: 'var(--text-secondary)' }}>{new Date(m.startDate).toLocaleDateString('es-PE')}</td>
                      <td style={{ color: 'var(--text-secondary)' }}>{new Date(m.endDate).toLocaleDateString('es-PE')}</td>
                      <td><StatusBadge status={m.statusName} /></td>
                      <td>
                        {m.statusName === 'Active' && (
                          <button className="btn btn-success btn-sm" onClick={() => handleRenew(m.id)}>
                            🔄 Renovar
                          </button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* Sessions Tab */}
      {tab === 'sessions' && (
        <div className="card">
          {sessions.length === 0 ? (
            <div className="empty-state">
              <div className="empty-state-icon">🏋️</div>
              <h4>Sin sesiones registradas</h4>
              <p>Registra la primera sesión de entrenamiento</p>
            </div>
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr><th>Fecha</th><th>Hora Inicio</th><th>Hora Fin</th><th>Ejercicios</th><th>Notas</th></tr>
                </thead>
                <tbody>
                  {sessions.map(s => (
                    <tr key={s.id}>
                      <td style={{ fontWeight: 600 }}>{new Date(s.sessionDate).toLocaleDateString('es-PE', { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' })}</td>
                      <td>{s.startTime}</td>
                      <td style={{ color: 'var(--text-secondary)' }}>{s.endTime ?? '—'}</td>
                      <td>
                        <span className="badge badge-info">{s.exercises.length} ejercicios</span>
                      </td>
                      <td style={{ color: 'var(--text-secondary)', maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {s.notes ?? '—'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* Membership Modal */}
      {showMembershipModal && (
        <div className="modal-overlay" onClick={() => setShowMembershipModal(false)}>
          <div className="modal slide-up" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Nueva Membresía</h3>
              <button className="modal-close" onClick={() => setShowMembershipModal(false)}>✕</button>
            </div>
            {error && <div className="alert alert-error">⚠️ {error}</div>}
            <form onSubmit={handleCreateMembership}>
              <div className="form-grid">
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Plan de Membresía</label>
                  <select value={membershipForm.planName} onChange={e => setMembershipForm({ ...membershipForm, planName: e.target.value })} required>
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
                    value={membershipForm.price || ''}
                    onChange={e => setMembershipForm({ ...membershipForm, price: parseFloat(e.target.value) })} />
                </div>
                <div className="form-group">
                  <label>Fecha de Inicio</label>
                  <input type="date" required value={membershipForm.startDate}
                    onChange={e => setMembershipForm({ ...membershipForm, startDate: e.target.value })} />
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Fecha de Vencimiento</label>
                  <input type="date" required value={membershipForm.endDate}
                    onChange={e => setMembershipForm({ ...membershipForm, endDate: e.target.value })} />
                </div>
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowMembershipModal(false)}>Cancelar</button>
                <button type="submit" className="btn btn-primary" disabled={saving}>
                  {saving ? 'Guardando...' : '✓ Registrar Membresía'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Session Modal */}
      {showSessionModal && (
        <div className="modal-overlay" onClick={() => setShowSessionModal(false)}>
          <div className="modal slide-up" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Nueva Sesión de Entrenamiento</h3>
              <button className="modal-close" onClick={() => setShowSessionModal(false)}>✕</button>
            </div>
            {error && <div className="alert alert-error">⚠️ {error}</div>}
            <form onSubmit={handleCreateSession}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Fecha de Sesión</label>
                  <input type="date" required value={sessionForm.sessionDate}
                    onChange={e => setSessionForm({ ...sessionForm, sessionDate: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Hora de Inicio</label>
                  <input type="time" required value={sessionForm.startTime}
                    onChange={e => setSessionForm({ ...sessionForm, startTime: e.target.value })} />
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Notas de la Sesión</label>
                  <textarea rows={3} placeholder="Observaciones, objetivos, etc."
                    value={sessionForm.notes}
                    onChange={e => setSessionForm({ ...sessionForm, notes: e.target.value })}
                    style={{ resize: 'vertical' }} />
                </div>
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowSessionModal(false)}>Cancelar</button>
                <button type="submit" className="btn btn-primary" disabled={saving}>
                  {saving ? 'Guardando...' : '✓ Registrar Sesión'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
