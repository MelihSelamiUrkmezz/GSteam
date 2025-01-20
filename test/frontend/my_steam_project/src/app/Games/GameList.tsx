import React from 'react'
import GameCard from './GameCard';

async function GetData() {
    const res = await fetch('http://localhost:6001/game', {
        cache: 'no-store',
        headers: {
            'Accept': 'application/json'
        }
    });
    
    if (!res.ok) {
        console.error('Failed to fetch games:', res.status);
        throw new Error("Failed to fetch data");
    }
    
    const data = await res.json();
    console.log('Games data:', data); // Debug için
    return data;
}

async function GameList() {
    const data = await GetData();

    if (!data || !data.data) {
        return <div>No games found</div>;
    }

    return (
        <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10'>
            {data.data.map((game: any) => (
                <GameCard game={game} key={game.id} />
            ))}
        </div>
    );
}

export default GameList;