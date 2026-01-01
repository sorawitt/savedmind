/**
 * Shared types between frontend and extension
 */

export interface Bookmark {
    id: string;
    url: string;
    title: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
}

export interface CreateBookmarkRequest {
    url: string;
    title: string;
    description?: string;
}

export interface SearchRequest {
    query: string;
    limit?: number;
}

export interface SearchResult {
    bookmarks: Bookmark[];
    total: number;
}

export interface ApiResponse<T> {
    data?: T;
    error?: string;
    success: boolean;
}
