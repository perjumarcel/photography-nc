import type { LoadStatus } from '@/features/albums/model/types';

export interface ContactMessage {
  name: string;
  email: string;
  message: string;
}

export interface ContactState {
  status: LoadStatus;
  error: string | null;
}
