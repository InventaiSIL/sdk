import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Layout/Navbar';
import LandingPage from './pages/LandingPage';
import NovelGenerator from './pages/NovelGenerator';
import NovelFinder from './pages/NovelFinder';
import './App.css';

function App() {
    return (
        <Router>
            <div className="min-h-screen bg-gray-50 w-full">
                <Navbar />
                <Routes>
                    <Route path="/" element={<LandingPage />} />
                    <Route path="/novel-generator" element={<NovelGenerator />} />
                    <Route path="/find" element={<NovelFinder />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;