import { useState, useEffect } from 'react';
import { clientsApi, workoutSessionsApi } from '../services/api';
import type { WorkoutSession, Client, CreateWorkoutSessionDto } from '../types';
import Avatar from '../components/Avatar';

export default function SessionsPage() {
  const [clients, setClients] = useState<Client[]>([]);
  const [sessions, setSessions] = useState<WorkoutSession[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [selectedSession, setSelectedSession] = useState<WorkoutSession | null>(null);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const [form, setForm] = useState<CreateWorkoutSessionDto>({
    clientId: 0, sessionDate: '', startTime: '', notes: ''
  });

  const loadData = async () => {
    setLoading(true);
    try {
      const cs = await clientsApi.getAll();
      setClients(cs);
      const allSessions = await Promise.all(cs.map(c => clientsApi.getWorkoutSessions(c.id)));
      setSessions(allSessions.flat().sort((a, b) => new Date(b.sessionDate).getTime() - new Date(a.sessionDate).getTime()));
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadData(); }, []);

  const filtered = sessions.filter(s =>
    s.clientFullName?.toLowerCase().includes(search.toLowerCase()) ||
    s.notes?.toLowerCase().includes(search.toLowerCase())
  );

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(''); setSaving(true);
    try {
      await workoutSessionsApi.create(form);
      setShowModal(false);
      setForm({ clientId: 0, sessionDate: '', startTime: '', notes: '' });
      await loadData();
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Error al registrar sesión.');
    } finally { setSaving(false); }
  };

  return (
    <div className="fade-in">
      <div className="page-header">
        <h2>Sesiones de Entrenamiento</h2>
        <p>Historial y registro de sesiones de trabajo por cliente</p>
      </div>

      <div className="search-bar">
        <div className="search-input-wrapper">
          <span className="search-icon">🔍</span>
          <input
            id="session-search"
            type="text"
            placeholder="Buscar por cliente o notas..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <button id="btn-new-session-global" className="btn btn-primary" onClick={() => { setShowModal(true); setError(''); }}>
          + Nueva Sesión
        </button>
      </div>

      <div className="card">
        {loading ? (
          <div className="loading-screen"><div className="spinner" /></div>
        ) : filtered.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">🏋️</div>
            <h4>Sin sesiones registradas</h4>
            <p>Registra la primera sesión de entrenamiento</p>
          </div>
        ) : (
          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr><th>Cliente</th><th>Fecha</th><th>Hora</th><th>Ejercicios</th><th>Notas</th><th>Detalle</th></tr>
              </thead>
              <tbody>
                {filtered.map(s => {
                  const client = clients.find(c => c.id === s.clientId);
                  return (
                    <tr key={s.id}>
                      <td>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                          {client && <Avatar name={client.fullName} size={30} />}
                          <span style={{ fontWeight: 500 }}>{s.clientFullName}</span>
                        </div>
                      </td>
                      <td style={{ fontWeight: 600 }}>
                        {new Date(s.sessionDate).toLocaleDateString('es-PE', { weekday: 'short', month: 'short', day: 'numeric' })}
                      </td>
                      <td style={{ color: 'var(--text-secondary)' }}>{s.startTime}{s.endTime ? ` — ${s.endTime}` : ''}</td>
                      <td><span className="badge badge-info">{s.exercises.length} ejercicios</span></td>
                      <td style={{ color: 'var(--text-secondary)', maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {s.notes || '—'}
                      </td>
                      <td>
                        <button className="btn btn-secondary btn-sm" onClick={() => setSelectedSession(s)}>
                          Ver detalle
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Session Detail Modal */}
      {selectedSession && (
        <div className="modal-overlay" onClick={() => setSelectedSession(null)}>
          <div className="modal slide-up" style={{ maxWidth: 640 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Detalle de Sesión #{selectedSession.id}</h3>
              <button className="modal-close" onClick={() => setSelectedSession(null)}>✕</button>
            </div>
            <div style={{ marginBottom: 16 }}>
              <div style={{ display: 'flex', gap: 20, flexWrap: 'wrap', marginBottom: 12 }}>
                <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                  👤 <strong>{selectedSession.clientFullName}</strong>
                </div>
                <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                  📅 {new Date(selectedSession.sessionDate).toLocaleDateString('es-PE', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
                </div>
                <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                  🕐 {selectedSession.startTime}{selectedSession.endTime ? ` — ${selectedSession.endTime}` : ''}
                </div>
              </div>
              {selectedSession.notes && (
                <div style={{ background: 'var(--bg-3)', padding: 12, borderRadius: 8, fontSize: 13, color: 'var(--text-secondary)' }}>
                  📝 {selectedSession.notes}
                </div>
              )}
            </div>

            {selectedSession.exercises.length === 0 ? (
              <div className="empty-state" style={{ padding: '30px 0' }}>
                <div className="empty-state-icon">💪</div>
                <h4>Sin ejercicios registrados</h4>
              </div>
            ) : (
              <table className="data-table">
                <thead>
                  <tr><th>#</th><th>Ejercicio</th><th>Series</th><th>Reps</th><th>Peso (kg)</th><th>Notas</th></tr>
                </thead>
                <tbody>
                  {[...selectedSession.exercises].sort((a, b) => a.order - b.order).map(ex => (
                    <tr key={ex.id}>
                      <td style={{ color: 'var(--text-muted)' }}>{ex.order}</td>
                      <td style={{ fontWeight: 600 }}>{ex.exerciseName}</td>
                      <td>{ex.sets}</td>
                      <td>{ex.reps}</td>
                      <td>{ex.weightKg ? `${ex.weightKg} kg` : '—'}</td>
                      <td style={{ color: 'var(--text-secondary)', fontSize: 12 }}>{ex.notes || '—'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      )}

      {/* Create Session Modal */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal slide-up" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Nueva Sesión de Entrenamiento</h3>
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
                <div className="form-group">
                  <label>Fecha de Sesión</label>
                  <input type="date" required value={form.sessionDate}
                    onChange={e => setForm({ ...form, sessionDate: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Hora de Inicio</label>
                  <input type="time" required value={form.startTime}
                    onChange={e => setForm({ ...form, startTime: e.target.value })} />
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Notas</label>
                  <textarea rows={3} placeholder="Descripción de la sesión..."
                    value={form.notes} onChange={e => setForm({ ...form, notes: e.target.value })}
                    style={{ resize: 'vertical' }} />
                </div>
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>Cancelar</button>
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
