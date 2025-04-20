import React from 'react';
import { MdAutoStories } from 'react-icons/md';
import { Link, useNavigate } from 'react-router-dom';

const Navbar: React.FC = () => {
    const navigate = useNavigate();
    
    return (
    <div className="bg-gradient-to-br from-slate-900 to-slate-800 text-white flex flex-col w-full">
        <nav className="w-full py-6 px-4 sm:px-6 lg:px-8 bg-slate-900/50 backdrop-blur-sm">
            <div className="max-w-7xl mx-auto flex justify-between items-center">
                <div className="flex items-center space-x-2 cursor-pointer" onClick={() => navigate('/')}>
                    <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center">
                        <MdAutoStories className="text-white text-xl" />
                    </div>
                    <span className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-purple-500">
                        InventAI
                    </span>
                </div>
                <div className="hidden md:flex space-x-8">
                    <Link to="/features" className="text-gray-300 hover:text-white transition-colors duration-300">
                        Features
                    </Link>
                    <Link to="/examples" className="text-gray-300 hover:text-white transition-colors duration-300">
                        Examples
                    </Link>
                    <Link to="/pricing" className="text-gray-300 hover:text-white transition-colors duration-300">
                        Pricing
                    </Link>
                </div>
                <Link
                    to="/novel-generator"
                    className="px-4 py-2 rounded-md border border-blue-500 text-blue-500 hover:bg-blue-500 hover:text-white transition-all duration-300"
                >
                    Generate a Novel
                </Link>
            </div>
        </nav>
    </div>
    );
};

export default Navbar; 