import type { LoadStatus } from '@/shared/model/types';

export interface ContactMessage {
  name: string;
  email: string;
  message: string;
  website?: string;
}

export interface ContactState {
  status: LoadStatus;
  error: string | null;
}
