'use server'
import { fetchProccess } from "@/app/library/fetchProcess"

export async function createCategory(category: { categoryName: string, categoryDescription: string }) {
    return await fetchProccess.post('category', category);
}

export async function getCategories() {
    try {
        const response = await fetch('http://localhost:6001/category', {
            cache: 'no-store',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Failed to fetch categories');
        }

        return response.json();
    } catch (error) {
        console.error('Error fetching categories:', error);
        throw error;
    }
} 