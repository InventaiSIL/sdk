import React from 'react';
import { Link } from 'react-router-dom';
import { MdAutoStories, MdEditNote, MdRocketLaunch, MdPalette } from 'react-icons/md';
import { FaBrain, FaChartLine } from 'react-icons/fa';

const LandingPage: React.FC = () => {
  return (
    <div className="min-h-screen h-full bg-gradient-to-br from-slate-900 to-slate-800 text-white flex flex-col w-full">
      {/* Hero Section */}
      <section className="flex-1 relative overflow-hidden pt-20 pb-20 md:pt-28 md:pb-28">
        <div className="absolute inset-0 opacity-20">
          <div className="absolute inset-0 bg-[url('https://grainy-gradients.vercel.app/noise.svg')] opacity-20"></div>
        </div>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative z-10">
          <div className="grid md:grid-cols-2 gap-12 items-center">
            <div className="space-y-8">
              <h1 className="text-5xl md:text-6xl lg:text-7xl font-bold leading-tight">
                <span className="block">Create Interactive</span>
                <span className="block bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-purple-500">
                  Choice-Based Games
                </span>
              </h1>
              <p className="text-xl text-gray-300 max-w-lg">
                Transform your ideas into engaging interactive experiences with our AI-powered game creator. Perfect for storytellers, game designers, and content creators.
              </p>
              <div className="flex flex-col sm:flex-row gap-4 pt-4">
                <Link
                  to="/novel-generator"
                  className="px-8 py-4 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg font-medium text-lg hover:shadow-lg hover:shadow-blue-500/30 transition-all duration-300 hover:-translate-y-1 flex items-center justify-center"
                >
                  Start Creating Free
                  <MdRocketLaunch className="ml-2" />
                </Link>
                <Link
                  to="/demo"
                  className="px-8 py-4 border border-gray-700 rounded-lg font-medium text-lg hover:bg-gray-800 transition-all duration-300 flex items-center justify-center"
                >
                  Watch Demo
                </Link>
              </div>
              <div className="flex items-center space-x-2 text-gray-400 pt-4">
                <div className="flex -space-x-2">
                  {[1, 2, 3].map((item) => (
                    <div
                      key={item}
                      className="w-8 h-8 rounded-full bg-gray-700 border-2 border-slate-800"
                      style={{ backgroundImage: `url(https://i.pravatar.cc/150?img=${item})` }}
                    ></div>
                  ))}
                </div>
                <span>Join 10,000+ creators</span>
              </div>
            </div>
            <div className="relative">
              <div className="absolute -top-8 -left-8 w-64 h-64 bg-purple-500 rounded-full mix-blend-multiply filter blur-2xl opacity-30 animate-blob"></div>
              <div className="absolute -bottom-8 -right-8 w-64 h-64 bg-blue-500 rounded-full mix-blend-multiply filter blur-2xl opacity-30 animate-blob animation-delay-2000"></div>
              <div className="relative bg-slate-800/50 backdrop-blur-lg border border-gray-700 rounded-2xl overflow-hidden shadow-2xl">
                <div className="p-4 border-b border-gray-700 flex space-x-2">
                  <div className="w-3 h-3 rounded-full bg-red-500"></div>
                  <div className="w-3 h-3 rounded-full bg-yellow-500"></div>
                  <div className="w-3 h-3 rounded-full bg-green-500"></div>
                </div>
                <div className="p-6">
                  <div className="text-gray-400 mb-4">AI Game Creator</div>
                  <div className="space-y-4">
                    <div className="h-4 bg-gray-700 rounded w-3/4"></div>
                    <div className="h-4 bg-gray-700 rounded"></div>
                    <div className="h-4 bg-gray-700 rounded w-5/6"></div>
                    <div className="h-4 bg-blue-500/30 rounded w-1/2"></div>
                  </div>
                  <div className="mt-8 pt-4 border-t border-gray-700 text-sm text-gray-500">
                    Generating your interactive story...
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center max-w-3xl mx-auto mb-16">
            <span className="inline-block px-3 py-1 text-sm font-medium bg-blue-500/10 text-blue-400 rounded-full mb-4">
              Powerful Features
            </span>
            <h2 className="text-4xl md:text-5xl font-bold mb-6">
              Everything you need to
              <span className="bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-purple-500">
                {' '}
                create immersive experiences
              </span>
            </h2>
            <p className="text-xl text-gray-400">
              Our platform combines cutting-edge AI with intuitive tools to help you craft engaging interactive stories.
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {[
              {
                icon: <FaBrain className="text-2xl" />,
                title: 'AI-Powered Story Generation',
                description:
                  'Our advanced algorithms create rich, branching narratives with meaningful choices and consequences.',
              },
              {
                icon: <MdPalette className="text-2xl" />,
                title: 'Interactive Design',
                description:
                  'Design complex decision trees and branching paths with our intuitive visual editor.',
              },
              {
                icon: <MdEditNote className="text-2xl" />,
                title: 'Smart Editing',
                description:
                  'Seamlessly edit and refine your interactive story with AI-powered suggestions.',
              },
              {
                icon: <FaChartLine className="text-2xl" />,
                title: 'Player Analytics',
                description:
                  'Track player choices and engagement to optimize your game experience.',
              },
              {
                icon: <MdAutoStories className="text-2xl" />,
                title: 'Branch Management',
                description:
                  'Easily organize and manage multiple story branches and endings.',
              },
              {
                icon: <MdRocketLaunch className="text-2xl" />,
                title: 'Rapid Prototyping',
                description:
                  'Quickly generate and test different story paths and outcomes.',
              },
            ].map((feature, index) => (
              <div
                key={index}
                className="bg-slate-800/50 backdrop-blur-sm border border-gray-700 rounded-xl p-6 hover:border-blue-500/50 transition-all duration-300 hover:-translate-y-2"
              >
                <div className="w-12 h-12 rounded-lg bg-blue-500/10 flex items-center justify-center text-blue-400 mb-4">
                  {feature.icon}
                </div>
                <h3 className="text-xl font-bold mb-2">{feature.title}</h3>
                <p className="text-gray-400">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Testimonial Section */}
      <section className="py-20 bg-gradient-to-br from-slate-800 to-slate-900">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center max-w-3xl mx-auto mb-16">
            <span className="inline-block px-3 py-1 text-sm font-medium bg-purple-500/10 text-purple-400 rounded-full mb-4">
              Loved by Creators
            </span>
            <h2 className="text-4xl md:text-5xl font-bold mb-6">
              What our users say
              <span className="bg-clip-text text-transparent bg-gradient-to-r from-purple-400 to-pink-500">
                {' '}
                about InventAI
              </span>
            </h2>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {[
              {
                quote:
                  "InventAI helped me create an interactive mystery game that kept players engaged for hours!",
                author: 'Sarah Johnson',
                role: 'Game Designer',
                avatar: 'https://i.pravatar.cc/150?img=5',
              },
              {
                quote:
                  'The AI suggestions for branching narratives are incredibly intuitive and creative.',
                author: 'Michael Chen',
                role: 'Interactive Storyteller',
                avatar: 'https://i.pravatar.cc/150?img=11',
              },
              {
                quote:
                  'I went from hobbyist to professional game creator thanks to InventAI. The tools are amazing!',
                author: 'Emma Rodriguez',
                role: 'Content Creator',
                avatar: 'https://i.pravatar.cc/150?img=8',
              },
            ].map((testimonial, index) => (
              <div
                key={index}
                className="bg-slate-800/50 backdrop-blur-sm border border-gray-700 rounded-xl p-8 hover:shadow-lg hover:shadow-purple-500/10 transition-all duration-300"
              >
                <div className="text-yellow-400 text-xl mb-4">★★★★★</div>
                <p className="text-lg italic mb-6">"{testimonial.quote}"</p>
                <div className="flex items-center">
                  <img
                    src={testimonial.avatar}
                    alt={testimonial.author}
                    className="w-12 h-12 rounded-full mr-4"
                  />
                  <div>
                    <div className="font-bold">{testimonial.author}</div>
                    <div className="text-gray-400 text-sm">{testimonial.role}</div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="bg-gradient-to-br from-blue-600 to-purple-600 rounded-3xl p-8 md:p-12 lg:p-16 overflow-hidden relative">
            <div className="absolute -top-20 -right-20 w-64 h-64 bg-blue-400 rounded-full mix-blend-multiply filter blur-2xl opacity-20"></div>
            <div className="absolute -bottom-20 -left-20 w-64 h-64 bg-purple-400 rounded-full mix-blend-multiply filter blur-2xl opacity-20"></div>
            <div className="relative z-10 grid md:grid-cols-2 gap-8 items-center">
              <div>
                <h2 className="text-3xl md:text-4xl font-bold mb-4">
                  Ready to create your interactive game?
                </h2>
                <p className="text-lg text-blue-100 max-w-lg">
                  Join thousands of creators who are already building amazing interactive experiences with InventAI.
                </p>
              </div>
              <div className="flex flex-col sm:flex-row gap-4 justify-center md:justify-end">
                <Link
                  to="/game-creator"
                  className="px-8 py-4 bg-white text-blue-600 rounded-lg font-medium text-lg hover:bg-gray-100 transition-all duration-300 hover:shadow-lg flex items-center justify-center"
                >
                  Start Creating Free
                </Link>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-slate-900 border-t border-gray-800">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-8">
            <div className="col-span-2">
              <div className="flex items-center space-x-2 mb-4">
                <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center">
                  <MdAutoStories className="text-white text-xl" />
                </div>
                <span className="text-xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-purple-500">
                  InventAI
                </span>
              </div>
              <p className="text-gray-400 mb-4">
                Empowering creators with AI-powered interactive storytelling tools.
              </p>
            </div>
            <div>
              <h3 className="text-sm font-semibold text-gray-300 uppercase tracking-wider mb-4">
                Product
              </h3>
              <ul className="space-y-3">
                {['Features', 'Pricing', 'Examples', 'API'].map((item) => (
                  <li key={item}>
                    <a
                      href="#"
                      className="text-gray-400 hover:text-white transition-colors duration-300"
                    >
                      {item}
                    </a>
                  </li>
                ))}
              </ul>
            </div>
            <div>
              <h3 className="text-sm font-semibold text-gray-300 uppercase tracking-wider mb-4">
                Resources
              </h3>
              <ul className="space-y-3">
                {['Documentation', 'Open Source'].map((item) => (
                  <li key={item}>
                    <a
                      href="#"
                      className="text-gray-400 hover:text-white transition-colors duration-300"
                    >
                      {item}
                    </a>
                  </li>
                ))}
              </ul>
            </div>
          </div>
          <div className="mt-12 pt-8 border-t border-gray-800 flex flex-col md:flex-row justify-between items-center">
            <p className="text-gray-400 text-sm">
              &copy; {new Date().getFullYear()} InventAI. All rights reserved.
            </p>
            <div className="flex space-x-6 mt-4 md:mt-0">
              <a href="#" className="text-gray-400 hover:text-white text-sm">
                Privacy Policy
              </a>
              <a href="#" className="text-gray-400 hover:text-white text-sm">
                Terms of Service
              </a>
              <a href="#" className="text-gray-400 hover:text-white text-sm">
                Cookies
              </a>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default LandingPage;