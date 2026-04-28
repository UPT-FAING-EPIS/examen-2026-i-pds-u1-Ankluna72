// ── Type definitions for the Gym Management System ──

export interface Client {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  birthDate: string;
  createdAt: string;
  isActive: boolean;
}

export interface CreateClientDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  birthDate: string;
}

export type MembershipStatus = 'Active' | 'Expired' | 'Suspended' | 'Cancelled';

export interface Membership {
  id: number;
  clientId: number;
  clientFullName: string;
  planName: string;
  price: number;
  startDate: string;
  endDate: string;
  status: number;
  statusName: MembershipStatus;
  isValid: boolean;
  createdAt: string;
}

export interface CreateMembershipDto {
  clientId: number;
  planName: string;
  price: number;
  startDate: string;
  endDate: string;
}

export interface RenewMembershipDto {
  newEndDate: string;
}

export interface WorkoutExercise {
  id: number;
  exerciseName: string;
  sets: number;
  reps: number;
  weightKg: number | null;
  notes: string | null;
  order: number;
}

export interface WorkoutSession {
  id: number;
  clientId: number;
  clientFullName: string;
  sessionDate: string;
  startTime: string;
  endTime: string | null;
  notes: string | null;
  createdAt: string;
  exercises: WorkoutExercise[];
}

export interface CreateWorkoutSessionDto {
  clientId: number;
  sessionDate: string;
  startTime: string;
  notes?: string;
  exercises?: CreateWorkoutExerciseDto[];
}

export interface CreateWorkoutExerciseDto {
  exerciseName: string;
  sets: number;
  reps: number;
  weightKg?: number;
  notes?: string;
  order: number;
}
