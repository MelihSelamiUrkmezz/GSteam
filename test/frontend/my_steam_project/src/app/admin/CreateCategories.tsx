'use client'
import { createCategory } from '@/app/api/category/categoryActions';
import { useEffect, useState } from 'react';
import { toast } from 'react-hot-toast';

export default function CreateCategories() {
    const [loading, setLoading] = useState(false);

    const categories = [
        { categoryName: "Action", categoryDescription: "Fast-paced games focusing on combat and movement" },
        { categoryName: "Adventure", categoryDescription: "Story-driven exploration games" },
        { categoryName: "RPG", categoryDescription: "Role-playing games with character development" },
        { categoryName: "Strategy", categoryDescription: "Games focusing on tactical decision making" },
        { categoryName: "Sports", categoryDescription: "Competitive sports and racing games" },
        { categoryName: "Simulation", categoryDescription: "Realistic simulation of activities" },
        { categoryName: "Puzzle", categoryDescription: "Brain teasers and problem solving games" },
        { categoryName: "Horror", categoryDescription: "Scary and suspenseful games" }
    ];

    const createCategories = async () => {
        setLoading(true);
        try {
            for (const category of categories) {
                const response = await createCategory(category);
                if (response.isSuccess) {
                    toast.success(`Created category: ${category.categoryName}`);
                } else {
                    toast.error(`Failed to create category: ${category.categoryName}`);
                }
            }
        } catch (error) {
            console.error('Error creating categories:', error);
            toast.error('Failed to create categories');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="p-4">
            <button
                onClick={createCategories}
                disabled={loading}
                className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 disabled:bg-gray-400"
            >
                {loading ? 'Creating Categories...' : 'Create Default Categories'}
            </button>
        </div>
    );
} 