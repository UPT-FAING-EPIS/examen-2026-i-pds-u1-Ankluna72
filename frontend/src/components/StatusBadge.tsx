import type { MembershipStatus } from '../types';

interface StatusBadgeProps {
  status: MembershipStatus | string;
}

const statusConfig: Record<string, { label: string; className: string }> = {
  Active: { label: 'Activa', className: 'badge-success' },
  Expired: { label: 'Vencida', className: 'badge-danger' },
  Suspended: { label: 'Suspendida', className: 'badge-warning' },
  Cancelled: { label: 'Cancelada', className: 'badge-muted' },
};

export default function StatusBadge({ status }: StatusBadgeProps) {
  const config = statusConfig[status] ?? { label: status, className: 'badge-muted' };
  return <span className={`badge ${config.className}`}>{config.label}</span>;
}
