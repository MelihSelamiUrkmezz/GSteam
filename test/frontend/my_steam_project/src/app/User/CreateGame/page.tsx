'use client'
import { CreateGame } from '@/app/api/game/gameActions';
import { getCurrentUser } from '@/app/authActions/authNext';
import React, { useEffect, useState } from 'react';
import {toast} from 'react-hot-toast'
import { useRouter } from 'next/navigation'
import { getCategories } from '@/app/api/category/categoryActions'

export default function CreateGamePage() {
    const router = useRouter()
    const [loading, setLoading] = useState(false)
    const [categories, setCategories] = useState([]);

    useEffect(()=>{
        const getUserAccessible = async () => {
            const user = await getCurrentUser();
            if (user?.role !== "SuperAdmin") {
                window.location.replace("/");
            }
        }
        getUserAccessible()
    },[])

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await getCategories();
                if (response.isSuccess) {
                    setCategories(response.data || []);
                } else {
                    console.error('Failed to fetch categories:', response.message);
                    toast.error('Failed to load categories');
                }
            } catch (error) {
                console.error('Error fetching categories:', error);
                toast.error('Failed to load categories');
            }
        };
        
        fetchCategories();
    }, []);

    const [game,setGame] = useState({
        gameName:"",
        gameAuthor:"",
        price:0,
        videoFile:null,
        gameFile:null,
        gameDescription:"",
        categoryId:"",
        minimumSystemRequirement:"",
        recommendedSystemRequirement:"",
    });

    const [videoToBeStore,setVideoStore] = useState<any>();
    const [gameFileToBeStore,setGameFileToBeStore] = useState<any>();
    const [status,setStatus] = useState(false);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);

        try {
            const formData = new FormData();

            // Log the form values before sending
            console.log('Game Data:', {
                gameName: game.gameName,
                gameDescription: game.gameDescription,
                gameAuthor: game.gameAuthor,
                price: game.price
            });

            // Add basic game information
            formData.append('gameName', game.gameName);
            formData.append('gameDescription', game.gameDescription);
            formData.append('gameAuthor', game.gameAuthor);
            formData.append('price', game.price.toString());
            formData.append('categoryId', game.categoryId);

            // Add game file
            if (gameFileToBeStore) {
                console.log('Game File:', gameFileToBeStore.name);
                formData.append('gameFile', gameFileToBeStore);
            }

            // Add game images
            const gameImagesInput = e.currentTarget.querySelector('input[name="gameImages"]') as HTMLInputElement;
            if (gameImagesInput?.files) {
                console.log('Number of images:', gameImagesInput.files.length);
                for (let i = 0; i < gameImagesInput.files.length; i++) {
                    formData.append('gameImages', gameImagesInput.files[i]);
                }
            }

            // Add video file
            if (videoToBeStore) {
                console.log('Video File:', videoToBeStore.name);
                formData.append('videoFile', videoToBeStore);
            }

            // Log the FormData entries
            console.log('FormData entries:');
            for (let [key, value] of formData.entries()) {
                console.log(key, value);
            }

            const response = await CreateGame(formData);
            console.log('API Response:', response);
            
            if (response.isSuccess) {
                toast.success('Game created successfully!');
                router.push('/Games');
            } else {
                toast.error(response.message || 'Failed to create game');
            }
        } catch (error) {
            console.error('Create game error:', error);
            toast.error('Failed to create game');
        } finally {
            setLoading(false);
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files && e.target.files[0];
        if (file) {
            const fileType = file.type.split("/")[1];
            
            // Input name'e göre farklı validasyon uygula
            if (e.target.name === "gameImages") {
                // Resim dosyaları için geçerli tipler
                const validImageTypes = ["jpeg", "jpg", "png", "gif", "webp"];
                if (!validImageTypes.includes(fileType)) {
                    toast.error("Please select a valid image file (JPEG, PNG, GIF, WEBP)");
                    e.target.value = ''; // Reset input
                    return;
                }
            } else if (e.target.name === "gameInfo") {
                // Oyun dosyası için geçerli tipler
                const validGameTypes = ["zip", "x-zip-compressed", "x-zip", "rar", "x-rar-compressed"];
                if (!validGameTypes.includes(fileType)) {
                    toast.error("Please upload a ZIP or RAR file for the game");
                    e.target.value = ''; // Reset input
                    return;
                }
                setGameFileToBeStore(file);
            } else if (e.target.name === "videoUrl") {
                // Video dosyası için geçerli tipler
                const validVideoTypes = ["mp4", "webm", "ogg"];
                if (!validVideoTypes.includes(fileType)) {
                    toast.error("Please upload a valid video file (MP4, WEBM, OGG)");
                    e.target.value = ''; // Reset input
                    return;
                }
                setVideoStore(file);
            }
        }
    };

    return (
        <div className="max-w-2xl mx-auto p-4">
            <h1 className="text-2xl font-bold mb-4">Create New Game</h1>
            <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                    <label className="block mb-2">Game Name</label>
                    <input
                        type="text"
                        name="gameName"
                        required
                        className="w-full p-2 border rounded"
                        onChange={(e) => {setGame((prevState) => {return {...prevState,gameName:e.target.value}})}}
                    />
                </div>

                <div>
                    <label className="block mb-2">Description</label>
                    <textarea
                        name="gameDescription"
                        required
                        className="w-full p-2 border rounded"
                        onChange={(e) => {setGame((prevState) => {return {...prevState,gameDescription:e.target.value}})}}
                    />
                </div>

                <div>
                    <label className="block mb-2">Author</label>
                    <input
                        type="text"
                        name="gameAuthor"
                        required
                        className="w-full p-2 border rounded"
                        onChange={(e) => {setGame((prevState) => {return {...prevState,gameAuthor:e.target.value}})}}
                    />
                </div>

                <div>
                    <label className="block mb-2">Price</label>
                    <input
                        type="number"
                        name="price"
                        step="0.01"
                        required
                        className="w-full p-2 border rounded"
                        onChange={(e) => {setGame((prevState) => {return {...prevState,price:parseFloat(e.target.value)}})}}
                    />
                </div>

                <div>
                    <label className="block mb-2">Category</label>
                    <select
                        name="categoryId"
                        required
                        className="w-full p-2 border rounded"
                        onChange={(e) => setGame({...game, categoryId: e.target.value})}
                    >
                        <option value="">Select a category</option>
                        {categories.map((category) => (
                            <option key={category.id} value={category.id}>
                                {category.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div>
                    <label className="block mb-2">Game File (ZIP or RAR)</label>
                    <input
                        type="file"
                        name="gameInfo"
                        required
                        accept=".zip,.rar"
                        className="w-full p-2 border rounded"
                        onChange={handleFileChange}
                    />
                </div>

                <div>
                    <label className="block mb-2">Game Images (Multiple)</label>
                    <input
                        type="file"
                        name="gameImages"
                        multiple
                        required
                        accept="image/jpeg,image/png,image/gif,image/webp"
                        className="w-full p-2 border rounded"
                        onChange={handleFileChange}
                    />
                </div>

                <div>
                    <label className="block mb-2">Game Video</label>
                    <input
                        type="file"
                        name="videoUrl"
                        accept="video/mp4,video/webm,video/ogg"
                        className="w-full p-2 border rounded"
                        onChange={handleFileChange}
                    />
                </div>

                <button
                    type="submit"
                    disabled={loading}
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 disabled:bg-gray-400"
                >
                    {loading ? 'Creating...' : 'Create Game'}
                </button>
            </form>
        </div>
    )
}