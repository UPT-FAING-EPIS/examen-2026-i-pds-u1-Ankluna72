import axios from 'axios';
import type { Client, CreateClientDto, Membership, CreateMembershipDto, RenewMembershipDto, WorkoutSession, CreateWorkoutSessionDto } from '../types';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' }
});

// ── Clients ──────────────────────────────────────────────────────────────────

export const clientsApi = {
  getAll: () => api.get<Client[]>('/clients').then(r => r.data),
  getById: (id: number) => api.get<Client>(`/clients/${id}`).then(r => r.data),
  create: (dto: CreateClientDto) => api.post<Client>('/clients', dto).then(r => r.data),
  getMemberships: (id: number) => api.get<Membership[]>(`/clients/${id}/memberships`).then(r => r.data),
  getWorkoutSessions: (id: number) => api.get<WorkoutSession[]>(`/clients/${id}/workout-sessions`).then(r => r.data),
};

// ── Memberships ───────────────────────────────────────────────────────────────

export const membershipsApi = {
  getById: (id: number) => api.get<Membership>(`/memberships/${id}`).then(r => r.data),
  create: (dto: CreateMembershipDto) => api.post<Membership>('/memberships', dto).then(r => r.data),
  renew: (id: number, dto: RenewMembershipDto) => api.post<Membership>(`/memberships/${id}/renew`, dto).then(r => r.data),
  suspend: (id: number) => api.post<Membership>(`/memberships/${id}/suspend`).then(r => r.data),
  cancel: (id: number) => api.post<Membership>(`/memberships/${id}/cancel`).then(r => r.data),
};

// ── Workout Sessions ──────────────────────────────────────────────────────────

export const workoutSessionsApi = {
  getById: (id: number) => api.get<WorkoutSession>(`/workout-sessions/${id}`).then(r => r.data),
  create: (dto: CreateWorkoutSessionDto) => api.post<WorkoutSession>('/workout-sessions', dto).then(r => r.data),
};
