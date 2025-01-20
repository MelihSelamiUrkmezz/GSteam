'use client'
import React, { useEffect, useState } from 'react'
import { FetchMyGames } from '../api/game/gameActions'
import { toast } from 'react-hot-toast';

interface MyGame {
    id: string;
    gameName: string;
    gameDescription: string;
    gameInfo: string;
    gameImages: Array<{
        imageUrl: string;
    }>;
}

export default function MyGamesPage() {
    const [myGames, setMyGames] = useState<MyGame[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchMyGames = async () => {
            try {
                console.log('Fetching my games...');
                const response = await FetchMyGames();
                console.log('MyGames API response:', response);
                
                if (response.isSuccess) {
                    console.log('Setting games:', response.data);
                    setMyGames(response.data);
                } else {
                    console.error('Failed to load games:', response.message);
                    toast.error('Failed to load your games');
                }
            } catch (error) {
                console.error('Error fetching my games:', error);
                toast.error('Error loading your games');
            } finally {
                setLoading(false);
            }
        };

        fetchMyGames();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!myGames || myGames.length === 0) {
        return (
            <div className="text-center py-10">
                <h2 className="text-2xl font-bold mb-4">My Games</h2>
                <p>You don't have any games yet.</p>
            </div>
        );
    }

    return (
        <div className="container mx-auto px-4 py-8">
            <h2 className="text-2xl font-bold mb-6">My Games</h2>
            <div className="grid gap-6">
                {myGames.map((game) => (
                    <div key={game.id} className="bg-white shadow overflow-hidden sm:rounded-lg">
                        <div className="px-4 py-5 sm:px-6">
                            <div className="flex items-center justify-between">
                                <div>
                                    <h3 className="text-lg leading-6 font-medium text-gray-900">
                                        {game.gameName}
                                    </h3>
                                    <p className="mt-1 max-w-2xl text-sm text-gray-500">
                                        {game.gameDescription}
                                    </p>
                                </div>
                                <div className="flex items-center space-x-4">
                                    <a
                                        href={game.gameInfo}
                                        download
                                        className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                                    >
                                        Download Game
                                    </a>
                                </div>
                            </div>
                            {game.gameImages && game.gameImages[0] && (
                                <img
                                    src={game.gameImages[0].imageUrl}
                                    alt={game.gameName}
                                    className="mt-4 w-full h-48 object-cover rounded-lg"
                                />
                            )}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
