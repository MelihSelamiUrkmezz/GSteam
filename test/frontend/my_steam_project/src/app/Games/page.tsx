'use client'
import React, { useEffect, useState } from 'react'
import { getAllGames } from '../api/game/gameActions'
import Link from 'next/link';
import { toast } from 'react-hot-toast';

interface Game {
    id: string;
    gameName: string;
    gameDescription: string;
    price: number;
    gameImages: Array<{
        id: string;
        imageUrl: string;
    }>;
}

export default function GamesPage() {
    const [games, setGames] = useState<Game[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchGames = async () => {
            try {
                const response = await getAllGames();
                console.log('Games response:', response); // Debug için
                if (response.isSuccess) {
                    setGames(response.data);
                } else {
                    toast.error('Failed to load games');
                }
            } catch (error) {
                console.error('Error fetching games:', error);
                toast.error('Error loading games');
            } finally {
                setLoading(false);
            }
        };

        fetchGames();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <div className="container mx-auto px-4 py-8">
            <h2 className="text-2xl font-bold mb-6">Games</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {games.map((game) => (
                    <div key={game.id} className="bg-white rounded-lg shadow-md overflow-hidden">
                        {game.gameImages && game.gameImages.length > 0 && (
                            <div className="h-48 overflow-hidden">
                                <img 
                                    src={game.gameImages[0].imageUrl} 
                                    alt={game.gameName}
                                    className="w-full h-full object-cover"
                                    onError={(e) => {
                                        const target = e.target as HTMLImageElement;
                                        console.error('Failed to load image:', game.gameImages[0].imageUrl);
                                        target.src = 'https://res.cloudinary.com/dh0dvetek/image/upload/v1/game-images/placeholder';
                                    }}
                                    loading="lazy"
                                />
                            </div>
                        )}
                        <div className="p-4">
                            <h3 className="text-xl font-semibold mb-2">{game.gameName}</h3>
                            <p className="text-gray-600 mb-4 line-clamp-2">{game.gameDescription}</p>
                            <div className="flex justify-between items-center">
                                <span className="text-blue-600 font-semibold">${game.price}</span>
                                <Link 
                                    href={`/Games/Details/${game.id}`}
                                    className="text-blue-500 hover:text-blue-700"
                                >
                                    View Details
                                </Link>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
} 