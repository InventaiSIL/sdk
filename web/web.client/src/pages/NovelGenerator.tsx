import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { FaMagic, FaUserPlus, FaBook, FaInfoCircle, FaLightbulb, FaClipboardList, FaKey, FaSearch, FaDownload } from 'react-icons/fa';
import { GiSpellBook } from 'react-icons/gi';

interface CharacterCreationRequest {
  prompt: string;
  context: string;
  numCharacters: number;
}

interface NovelCreationRequest {
  characterCreationRequest: CharacterCreationRequest;
  numScenes: number;
  prompt: string;
  context: string;
}

const NovelGenerator: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<NovelCreationRequest>({
    characterCreationRequest: {
      prompt: '',
      context: '',
      numCharacters: 3
    },
    numScenes: 5,
    prompt: '',
    context: ''
  });
  const [apiKeys, setApiKeys] = useState({
    openAiApiKey: '',
    segmindApiKey: ''
  });
  const [isGenerating, setIsGenerating] = useState(false);
  const [activeSection, setActiveSection] = useState<'api' | 'character' | 'novel'>('api');
  const [generationInfo, setGenerationInfo] = useState<{ id: string; status: string } | null>(null);
  const [statusCheckInterval, setStatusCheckInterval] = useState<NodeJS.Timeout | null>(null);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    if (name.startsWith('character.')) {
      const field = name.split('.')[1];
      setFormData(prev => ({
        ...prev,
        characterCreationRequest: {
          ...prev.characterCreationRequest,
          [field]: field === 'numCharacters' ? parseInt(value) : value
        }
      }));
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: name === 'numScenes' ? parseInt(value) : value
      }));
    }
  };

  const checkGenerationStatus = async (generationId: string) => {
    try {
      const response = await fetch(`https://inventai-final-awfabwdge5d5g8bk.canadacentral-01.azurewebsites.net/Novel/${generationId}/status`);
      if (!response.ok) {
        throw new Error('Failed to check status');
      }
      const data = await response.json();
      
      if (data.status === 'Completed' || data.status === 'Failed') {
        if (statusCheckInterval) {
          clearInterval(statusCheckInterval);
          setStatusCheckInterval(null);
        }
      }
      
      setGenerationInfo(prev => prev ? { ...prev, status: data.status } : null);
    } catch (error) {
      console.error('Error checking status:', error);
    }
  };

  useEffect(() => {
    return () => {
      if (statusCheckInterval) {
        clearInterval(statusCheckInterval);
      }
    };
  }, [statusCheckInterval]);

  const handleApiKeyChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setApiKeys(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsGenerating(true);
    try {
      const response = await fetch('https://inventai-final-awfabwdge5d5g8bk.canadacentral-01.azurewebsites.net/Novel', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'OpenAiApiKey': apiKeys.openAiApiKey,
          'SegmindApiKey': apiKeys.segmindApiKey
        },
        body: JSON.stringify(formData)
      });

      if (!response.ok) {
        throw new Error('Failed to create novel');
      }

      const data = await response.json();
      setGenerationInfo({
        id: data.generationId,
        status: data.status
      });

      const interval = setInterval(() => checkGenerationStatus(data.generationId), 5000);
      setStatusCheckInterval(interval);
    } catch (error) {
      console.error('Error creating novel:', error);
      setIsGenerating(false);
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
              Novel Craft AI
            </h1>
          </div>
          <p className="mt-2 text-xl text-gray-300 max-w-2xl mx-auto">
            Transform your ideas into captivating stories with our AI-powered novel generator
          </p>
          <button
            onClick={() => navigate('/find')}
            className="mt-4 inline-flex items-center px-6 py-2 border border-transparent text-base font-medium rounded-lg text-white bg-slate-700 hover:bg-slate-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 transition-all"
          >
            <FaSearch className="mr-2" />
            Find Previous Novels
          </button>
        </div>

        {/* Form Navigation */}
        <div className="flex mb-8 bg-slate-800 rounded-lg p-1">
          <button
            onClick={() => setActiveSection('api')}
            className={`flex-1 py-3 px-4 rounded-md flex items-center justify-center space-x-2 transition-all ${
              activeSection === 'api' ? 'bg-blue-600 text-white' : 'text-gray-300 hover:text-white'
            }`}
          >
            <FaKey />
            <span>API Keys</span>
          </button>
          <button
            onClick={() => setActiveSection('character')}
            className={`flex-1 py-3 px-4 rounded-md flex items-center justify-center space-x-2 transition-all ${
              activeSection === 'character' ? 'bg-blue-600 text-white' : 'text-gray-300 hover:text-white'
            }`}
          >
            <FaUserPlus />
            <span>Characters</span>
          </button>
          <button
            onClick={() => setActiveSection('novel')}
            className={`flex-1 py-3 px-4 rounded-md flex items-center justify-center space-x-2 transition-all ${
              activeSection === 'novel' ? 'bg-blue-600 text-white' : 'text-gray-300 hover:text-white'
            }`}
          >
            <FaBook />
            <span>Story</span>
          </button>
        </div>

        {generationInfo ? (
          <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-xl p-6 shadow-lg">
            <div className="flex items-center mb-6">
              <div className={`p-2 rounded-lg ${
                generationInfo.status === 'Completed' ? 'bg-green-500/20 text-green-400' :
                generationInfo.status === 'Failed' ? 'bg-red-500/20 text-red-400' :
                'bg-yellow-500/20 text-yellow-400'
              } mr-3`}>
                <FaClipboardList className="text-xl" />
              </div>
              <h2 className="text-2xl font-semibold text-white">
                {generationInfo.status === 'Completed' ? 'Generation Complete' :
                 generationInfo.status === 'Failed' ? 'Generation Failed' :
                 'Generation in Progress'}
              </h2>
            </div>
            
            <div className="space-y-4 text-gray-300">
              <p>
                {generationInfo.status === 'Completed' ? 'Your novel has been generated successfully!' :
                 generationInfo.status === 'Failed' ? 'There was an error generating your novel.' :
                 'Your novel is being generated! This process may take some time.'}
              </p>
              
              <div className="bg-slate-700/50 p-4 rounded-lg">
                <div className="flex items-center justify-between mb-2">
                  <span className="font-medium">Generation ID:</span>
                  <code className="bg-slate-800 px-3 py-1 rounded text-sm">{generationInfo.id}</code>
                </div>
                <div className="flex items-center justify-between">
                  <span className="font-medium">Status:</span>
                  <span className={`${
                    generationInfo.status === 'Completed' ? 'text-green-400' :
                    generationInfo.status === 'Failed' ? 'text-red-400' :
                    'text-yellow-400'
                  }`}>
                    {generationInfo.status}
                  </span>
                </div>
              </div>

              {generationInfo.status === 'Completed' && (
                <div className="space-y-4">
                  <button
                    onClick={async () => {
                      try {
                        const response = await fetch(`https://inventai-final-awfabwdge5d5g8bk.canadacentral-01.azurewebsites.net/novels/${generationInfo.id}/game.zip`);
                        if (!response.ok) throw new Error('Download failed');
                        
                        const blob = await response.blob();
                        const url = window.URL.createObjectURL(blob);
                        const a = document.createElement('a');
                        a.href = url;
                        a.download = `novel-${generationInfo.id}.zip`;
                        document.body.appendChild(a);
                        a.click();
                        window.URL.revokeObjectURL(url);
                        document.body.removeChild(a);
                      } catch (error) {
                        console.error('Download error:', error);
                      }
                    }}
                    className="w-full inline-flex items-center justify-center px-6 py-3 border border-transparent text-base font-medium rounded-lg text-white bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 transition-all"
                  >
                    <FaDownload className="mr-2" />
                    Download Game Files
                  </button>
                </div>
              )}
            </div>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-8">
            {/* API Keys Section */}
            {activeSection === 'api' && (
              <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-xl p-6 shadow-lg transition-all hover:shadow-xl">
                <div className="flex items-center mb-6">
                  <div className="p-2 rounded-lg bg-blue-500/20 text-blue-400 mr-3">
                    <FaKey className="text-xl" />
                  </div>
                  <h2 className="text-2xl font-semibold text-white">API Keys</h2>
                </div>
                
                <div className="space-y-6">
                  <div>
                    <label htmlFor="openAiApiKey" className="block text-sm font-medium text-gray-300 mb-1">
                      OpenAI API Key
                    </label>
                    <input
                      type="password"
                      name="openAiApiKey"
                      id="openAiApiKey"
                      value={apiKeys.openAiApiKey}
                      onChange={handleApiKeyChange}
                      className="mt-1 block w-full rounded-lg bg-slate-700 border-slate-600 text-white placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 px-4 py-2"
                      placeholder="Enter your OpenAI API key"
                      required
                    />
                    <p className="mt-1 text-xs text-gray-400">
                      Your API key will be used to generate the story content
                    </p>
                  </div>

                  <div>
                    <label htmlFor="segmindApiKey" className="block text-sm font-medium text-gray-300 mb-1">
                      Segmind API Key
                    </label>
                    <input
                      type="password"
                      name="segmindApiKey"
                      id="segmindApiKey"
                      value={apiKeys.segmindApiKey}
                      onChange={handleApiKeyChange}
                      className="mt-1 block w-full rounded-lg bg-slate-700 border-slate-600 text-white placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 px-4 py-2"
                      placeholder="Enter your Segmind API key"
                      required
                    />
                    <p className="mt-1 text-xs text-gray-400">
                      Your API key will be used to generate images for the story
                    </p>
                  </div>
                </div>

                <div className="mt-8 flex justify-end">
                  <button
                    type="button"
                    onClick={() => setActiveSection('character')}
                    className="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-lg shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-all hover:scale-105"
                  >
                    Next: Characters
                  </button>
                </div>
              </div>
            )}

            {/* Character Information (conditionally rendered) */}
            {activeSection === 'character' && (
              <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-xl p-6 shadow-lg transition-all hover:shadow-xl">
                <div className="flex items-center mb-6">
                  <div className="p-2 rounded-lg bg-blue-500/20 text-blue-400 mr-3">
                    <FaUserPlus className="text-xl" />
                  </div>
                  <h2 className="text-2xl font-semibold text-white">Character Creation</h2>
                </div>
                
                <div className="space-y-6">
                  <div className="relative">
                    <label htmlFor="character.numCharacters" className="block text-sm font-medium text-gray-300 mb-1">
                      Number of Main Characters
                    </label>
                    <div className="relative">
                      <input
                        type="range"
                        name="character.numCharacters"
                        id="character.numCharacters"
                        value={formData.characterCreationRequest.numCharacters}
                        onChange={handleInputChange}
                        min="1"
                        max="10"
                        className="w-full h-2 bg-slate-700 rounded-lg appearance-none cursor-pointer accent-blue-500 px-4 py-2"
                      />
                      <div className="absolute top-6 right-0 bg-blue-500 text-white text-xs font-bold px-2 py-1 rounded-full">
                        {formData.characterCreationRequest.numCharacters}
                      </div>
                    </div>
                  </div>

                  <div>
                    <div className="flex items-center justify-between mb-1">
                      <label htmlFor="character.prompt" className="block text-sm font-medium text-gray-300">
                        Character Descriptions
                      </label>
                      <button type="button" className="text-xs flex items-center text-blue-400 hover:text-blue-300">
                        <FaLightbulb className="mr-1" /> Need ideas?
                      </button>
                    </div>
                    <textarea
                      name="character.prompt"
                      id="character.prompt"
                      rows={4}
                      value={formData.characterCreationRequest.prompt}
                      onChange={handleInputChange}
                      className="mt-1 block w-full rounded-lg bg-slate-700 border-slate-600 text-white placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 px-4 py-2"
                      placeholder="Describe your main characters (e.g., 'A brave knight with a mysterious past, a cunning wizard seeking redemption')"
                      required
                    />
                    <p className="mt-1 text-xs text-gray-400 flex items-center">
                      <FaInfoCircle className="mr-1" /> The more details you provide, the better!
                    </p>
                  </div>

                  <div>
                    <label htmlFor="character.context" className="block text-sm font-medium text-gray-300 mb-1">
                      Character Relationships
                    </label>
                    <textarea
                      name="character.context"
                      id="character.context"
                      rows={4}
                      value={formData.characterCreationRequest.context}
                      onChange={handleInputChange}
                      className="mt-1 block w-full rounded-lg bg-slate-700 border-slate-600 text-white placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 px-4 py-2"
                      placeholder="Describe how characters relate to each other (e.g., 'The knight and wizard are childhood friends turned rivals')"
                      required
                    />
                  </div>
                </div>

                <div className="mt-8 flex justify-between">
                  <button
                    type="button"
                    onClick={() => setActiveSection('api')}
                    className="inline-flex items-center px-6 py-3 border border-gray-600 text-base font-medium rounded-lg shadow-sm text-gray-300 hover:text-white bg-slate-700 hover:bg-slate-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 transition-all"
                  >
                    Back to API Keys
                  </button>
                  <button
                    type="button"
                    onClick={() => setActiveSection('novel')}
                    className="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-lg shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-all hover:scale-105"
                  >
                    Next: Story Details
                  </button>
                </div>
              </div>
            )}

            {/* Novel Information (conditionally rendered) */}
            {activeSection === 'novel' && (
              <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-xl p-6 shadow-lg transition-all hover:shadow-xl">
                <div className="flex items-center mb-6">
                  <div className="p-2 rounded-lg bg-purple-500/20 text-purple-400 mr-3">
                    <FaBook className="text-xl" />
                  </div>
                  <h2 className="text-2xl font-semibold text-white">Story Details</h2>
                </div>

                <div className="space-y-6">
                  <div className="relative">
                    <label htmlFor="numScenes" className="block text-sm font-medium text-gray-300 mb-1">
                      Story Length (Number of Scenes)
                    </label>
                    <div className="flex items-center space-x-4">
                      <input
                        type="range"
                        name="numScenes"
                        id="numScenes"
                        value={formData.numScenes}
                        onChange={handleInputChange}
                        min="1"
                        max="20"
                        className="flex-1 h-2 bg-slate-700 rounded-lg appearance-none cursor-pointer accent-purple-500 px-4 py-2"
                      />
                      <span className="bg-purple-500 text-white text-sm font-bold px-3 py-1 rounded-full">
                        {formData.numScenes} {formData.numScenes === 1 ? 'Scene' : 'Scenes'}
                      </span>
                    </div>
                  </div>

                  <div>
                    <div className="flex items-center justify-between mb-1">
                      <label htmlFor="prompt" className="block text-sm font-medium text-gray-300">
                        Story Premise
                      </label>
                      <button type="button" className="text-xs flex items-center text-purple-400 hover:text-purple-300">
                        <FaLightbulb className="mr-1" /> Examples
                      </button>
                    </div>
                    <textarea
                      name="prompt"
                      id="prompt"
                      rows={4}
                      value={formData.prompt}
                      onChange={handleInputChange}
                      className="mt-1 block w-full rounded-lg bg-slate-700 border-slate-600 text-white placeholder-gray-400 focus:ring-2 focus:ring-purple-500 focus:border-purple-500 px-4 py-2"
                      placeholder="Describe your story idea (e.g., 'A fantasy adventure about a group of unlikely heroes saving their kingdom from an ancient evil')"
                      required
                    />
                  </div>

                  <div>
                    <label htmlFor="context" className="block text-sm font-medium text-gray-300 mb-1">
                      Additional Details
                    </label>
                    <textarea
                      name="context"
                      id="context"
                      rows={4}
                      value={formData.context}
                      onChange={handleInputChange}
                      className="mt-1 block w-full rounded-lg bg-slate-700 border-slate-600 text-white placeholder-gray-400 focus:ring-2 focus:ring-purple-500 focus:border-purple-500 px-4 py-2"
                      placeholder="Include any specific elements you want (themes, tone, plot points, etc.)"
                    />
                    <p className="mt-1 text-xs text-gray-400">
                      Optional but recommended for better results
                    </p>
                  </div>
                </div>

                <div className="mt-8 flex justify-between">
                  <button
                    type="button"
                    onClick={() => setActiveSection('character')}
                    className="inline-flex items-center px-6 py-3 border border-gray-600 text-base font-medium rounded-lg shadow-sm text-gray-300 hover:text-white bg-slate-700 hover:bg-slate-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 transition-all"
                  >
                    Back to Characters
                  </button>
                  <button
                    type="submit"
                    disabled={isGenerating}
                    className="inline-flex items-center px-8 py-3 border border-transparent text-base font-medium rounded-lg shadow-sm text-white bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-all hover:scale-105 disabled:opacity-70 disabled:cursor-not-allowed"
                  >
                    {isGenerating ? (
                      <>
                        <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                        </svg>
                        Generating...
                      </>
                    ) : (
                      <>
                        <FaMagic className="mr-2" />
                        Generate Your Novel
                      </>
                    )}
                  </button>
                </div>
              </div>
            )}
          </form>
        )}
      </div>
    </div>
  );
};

export default NovelGenerator;