'use client'
import React from "react";
import { Card, CardHeader, CardBody } from "@nextui-org/react";
import Link from "next/link";

type Props = {
    game: {
        id: string;
        gameName: string;
        gameDescription: string;
        price: number;
        gameImages: Array<{
            imageUrl: string;
        }>;
    }
}

export default function GameCard({ game }: Props) {
    const fallbackImageUrl = 'https://i.pinimg.com/originals/c5/ca/ae/c5caae987b65d9e39f6b174d1fd19fae.png';
    
    const imageUrl = game.gameImages && game.gameImages.length > 0 
        ? game.gameImages[0].imageUrl 
        : fallbackImageUrl;

    const handleImageError = (e: React.SyntheticEvent<HTMLImageElement, Event>) => {
        const target = e.target as HTMLImageElement;
        console.error('Failed to load image:', imageUrl);
        target.src = fallbackImageUrl;
    };

    return (
        <Card className="py-4 px-4 border border-gray-600 rounded-s-lg bg-slate-600">
            <Link href={`Games/Details/${game.id}`}>
                <CardHeader className="pb-0 pt-2 px-4 flex-col items-center">
                    <p className="text-tiny uppercase font-bold text-white">${game.price}</p>
                    <small className="text-default-500">{game.gameName}</small>
                    <h4 className="font-bold text-large text-white">{game.gameDescription}</h4>
                </CardHeader>
                <CardBody className="overflow-visible py-2">
                    <div className="relative w-full h-[200px]">
                        <img
                            alt={`${game.gameName} cover`}
                            className="object-cover rounded-xl w-full h-full"
                            src={imageUrl}
                            onError={handleImageError}
                        />
                    </div>
                </CardBody>
            </Link>
        </Card>
    );
}
