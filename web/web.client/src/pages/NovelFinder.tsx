import React, { useState } from 'react';
import { FaSearch, FaDownload, FaSpinner } from 'react-icons/fa';
import { GiSpellBook } from 'react-icons/gi';

interface NovelGeneration {
  id: string;
  generationId: string;
  status: string;
  createdAt: string;
  completedAt: string | null;
  errorMessage: string | null;
  outputPath: string | null;
}

const NovelFinder: React.FC = () => {
  const [generationId, setGenerationId] = useState('');
  const [novel, setNovel] = useState<NovelGeneration | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    setNovel(null);

    try {
      const response = await fetch(`http://localhost:5275/Novel/${generationId}`);
      if (!response.ok) {
        throw new Error('Failed to find novel');
      }
      const data = await response.json();
      setNovel(data);
    } catch (error) {
      setError('Failed to find novel. Please check the ID and try again.');
      console.error('Error searching for novel:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDownload = async () => {
    if (!novel?.outputPath) return;

    try {
      const response = await fetch(`http://localhost:5275/Novel/${novel.generationId}/download`);
      if (!response.ok) {
        throw new Error('Failed to download novel');
      }

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `novel-${novel.generationId}.zip`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (error) {
      setError('Failed to download novel. Please try again.');
      console.error('Error downloading novel:', error);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 to-slate-800 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="text-center mb-12">
          <div className="inline-flex items-center justify-center mb-4">
            <GiSpellBook className="text-4xl text-purple-400 mr-3" />
            <h1 className="text-4xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-purple-400 to-blue-500">
              Find Your Novel
            </h1>
          </div>
          <p className="mt-2 text-xl text-gray-300 max-w-2xl mx-auto">
            Enter your generation ID to find and download your previously generated novel
          </p>
        </div>

        {/* Search Form */}
        <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-xl p-6 shadow-lg">
          <form onSubmit={handleSearch} className="space-y-6">
            <div>
              <label htmlFor="generationId" className="block text-sm font-medium text-gray-300 mb-1">
                Generation ID
              </label>
              <div className="flex space-x-4">
                <input
                  type="text"
                  id="generationId"
                  value={generationId}
                  onChange={(e) => setGenerationId(e.target.value)}
                  className="flex-1 rounded-lg bg-slate-700 border-slate-600 text-white placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 px-4 py-2"
                  placeholder="Enter your generation ID"
                  required
                />
                <button
                  type="submit"
                  disabled={isLoading}
                  className="inline-flex items-center px-6 py-2 border border-transparent text-base font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-all hover:scale-105 disabled:opacity-70 disabled:cursor-not-allowed"
                >
                  {isLoading ? (
                    <FaSpinner className="animate-spin mr-2" />
                  ) : (
                    <FaSearch className="mr-2" />
                  )}
                  Search
                </button>
              </div>
            </div>
          </form>

          {/* Results */}
          {error && (
            <div className="mt-6 p-4 bg-red-500/20 border border-red-500 rounded-lg">
              <p className="text-red-400">{error}</p>
            </div>
          )}

          {novel && (
            <div className="mt-6 space-y-4">
              <div className="bg-slate-700/50 p-4 rounded-lg">
                <div className="flex items-center justify-between mb-2">
                  <span className="font-medium text-gray-300">Status:</span>
                  <span className={`${
                    novel.status === 'Completed' ? 'text-green-400' :
                    novel.status === 'Failed' ? 'text-red-400' :
                    'text-yellow-400'
                  }`}>
                    {novel.status}
                  </span>
                </div>
                <div className="flex items-center justify-between mb-2">
                  <span className="font-medium text-gray-300">Created:</span>
                  <span className="text-gray-300">
                    {new Date(novel.createdAt).toLocaleString()}
                  </span>
                </div>
                {novel.completedAt && (
                  <div className="flex items-center justify-between mb-2">
                    <span className="font-medium text-gray-300">Completed:</span>
                    <span className="text-gray-300">
                      {new Date(novel.completedAt).toLocaleString()}
                    </span>
                  </div>
                )}
                {novel.errorMessage && (
                  <div className="mt-4 p-3 bg-red-500/20 border border-red-500 rounded-lg">
                    <p className="text-red-400">{novel.errorMessage}</p>
                  </div>
                )}
              </div>

              {novel.status === 'Completed' && novel.outputPath && (
                <button
                  onClick={handleDownload}
                  className="w-full mt-4 inline-flex items-center justify-center px-6 py-3 border border-transparent text-base font-medium rounded-lg text-white bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-all"
                >
                  <FaDownload className="mr-2" />
                  Download Novel
                </button>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default NovelFinder; 