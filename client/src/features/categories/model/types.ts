export interface CategoryDto {
  id: number;
  name: string;
  slug: string | null;
  displayOrder: number;
  showAsFilter: boolean;
}
