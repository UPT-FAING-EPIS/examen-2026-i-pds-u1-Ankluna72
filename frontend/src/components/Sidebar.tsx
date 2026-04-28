import { NavLink } from 'react-router-dom';

const navItems = [
  { to: '/', icon: '📊', label: 'Dashboard' },
  { to: '/clients', icon: '👥', label: 'Clientes' },
  { to: '/memberships', icon: '🎟️', label: 'Membresías' },
  { to: '/sessions', icon: '🏋️', label: 'Sesiones' },
];

export default function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon">🏋️</div>
        <div className="sidebar-logo-text">
          <h1>GymPro</h1>
          <span>Sistema de Gestión</span>
        </div>
      </div>
      <nav className="sidebar-nav">
        <span className="nav-section-title">Menú Principal</span>
        {navItems.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.to === '/'}
            className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}
          >
            <span className="nav-item-icon">{item.icon}</span>
            {item.label}
          </NavLink>
        ))}
      </nav>
      <div style={{ padding: '16px', borderTop: '1px solid var(--border)' }}>
        <div style={{ fontSize: '12px', color: 'var(--text-muted)', textAlign: 'center' }}>
          GymPro v1.0.0 — PDS 2026-I
        </div>
      </div>
    </aside>
  );
}
