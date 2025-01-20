'use client'
import { addToMyGames } from '@/app/api/game/gameActions'
import { Button } from 'flowbite-react'
import React, { useState } from 'react'
import { toast } from 'react-hot-toast'

export default function AddToLibraryButton({ gameId }: { gameId: string }) {
    const [loading, setLoading] = useState(false)

    const handleAddToLibrary = async () => {
        setLoading(true)
        try {
            const response = await addToMyGames(gameId)
            if (response.isSuccess) {
                toast.success('Game added to your library!')
            } else {
                toast.error(response.message || 'Failed to add game to library')
            }
        } catch (error) {
            console.error('Error adding game to library:', error)
            toast.error('Failed to add game to library')
        } finally {
            setLoading(false)
        }
    }

    return (
        <Button
            onClick={handleAddToLibrary}
            disabled={loading}
            className="bg-green-500 hover:bg-green-600 text-white"
        >
            {loading ? 'Adding...' : 'Add to Library'}
        </Button>
    )
} 