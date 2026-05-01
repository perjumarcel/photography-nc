import type { LoadStatus } from '@/shared/model/types';

export interface ContactMessage {
  name: string;
  email: string;
  message: string;
  phone?: string;
  eventType?: string;
  preferredDate?: string;
  venue?: string;
  estimatedBudgetRange?: string;
  sourcePage?: string;
  website?: string;
}

export interface ContactState {
  status: LoadStatus;
  error: string | null;
}
