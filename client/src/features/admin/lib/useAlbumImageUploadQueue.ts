import { useCallback, useState } from 'react';
import { useAppDispatch } from '@/app/hooks';
import { uploadAlbumImage } from '../api/thunks';

export type UploadQueueStatus = 'queued' | 'uploading' | 'succeeded' | 'failed';

export interface UploadQueueItem {
  id: string;
  name: string;
  status: UploadQueueStatus;
  error?: string;
}

export function useAlbumImageUploadQueue(albumId: string): {
  uploadQueue: UploadQueueItem[];
  uploadFiles: (files: File[]) => Promise<void>;
} {
  const dispatch = useAppDispatch();
  const [uploadQueue, setUploadQueue] = useState<UploadQueueItem[]>([]);

  const uploadFiles = useCallback(async (files: File[]): Promise<void> => {
    if (files.length === 0) return;
    const queued = files.map((file, index) => ({
      id: `${file.name}-${file.size}-${file.lastModified}-${index}`,
      name: file.name,
      status: 'queued' as const,
    }));
    setUploadQueue(queued);

    for (const [index, file] of files.entries()) {
      const queueId = queued[index].id;
      setUploadQueue((queue) => updateItem(queue, queueId, { status: 'uploading', error: undefined }));
      const result = await dispatch(uploadAlbumImage({ albumId, file }));
      setUploadQueue((queue) => updateItem(queue, queueId, uploadAlbumImage.fulfilled.match(result)
        ? { status: 'succeeded', error: undefined }
        : { status: 'failed', error: String(result.payload ?? 'Upload failed') }));
    }
  }, [albumId, dispatch]);

  return { uploadQueue, uploadFiles };
}

function updateItem(queue: UploadQueueItem[], id: string, patch: Partial<UploadQueueItem>): UploadQueueItem[] {
  return queue.map((item) => (item.id === id ? { ...item, ...patch } : item));
}
