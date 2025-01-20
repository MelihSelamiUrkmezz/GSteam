'use client'
import Link from 'next/link'

export default function Navbar() {
    return (
        <nav className="bg-gray-800">
            <div className="max-w-7xl mx-auto px-4">
                <div className="flex items-center justify-between h-16">
                    <div className="flex items-center space-x-4">
                        <Link href="/" className="text-white font-bold">
                            Home
                        </Link>
                        <Link href="/Games" className="text-gray-300 hover:text-white">
                            Games
                        </Link>
                        <Link href="/MyGames" className="text-gray-300 hover:text-white">
                            My Games
                        </Link>
                    </div>
                </div>
            </div>
        </nav>
    )
} 