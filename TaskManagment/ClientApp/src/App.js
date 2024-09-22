import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import TaskList from './Components/TaskList';
import TaskForm from './Components/TaskForm';

const App = () => {
    return (
        <div className="min-h-screen bg-gray-100">
            <div className="container mx-auto px-4 py-8">
                <h1 className="text-3xl font-bold text-center mb-8 text-indigo-600">Task Manager</h1>
                <nav className="mb-8 bg-white shadow-md rounded-lg">
                    <ul className="flex p-4 space-x-4">
                        <li>
                            <Link to="/" className="text-indigo-600 hover:text-indigo-800 font-medium">Task List</Link>
                        </li>
                        <li>
                            <Link to="/create" className="text-indigo-600 hover:text-indigo-800 font-medium">Create Task</Link>
                        </li>
                    </ul>
                </nav>

                <div className="bg-white shadow-md rounded-lg p-6">
                    <Routes>
                        <Route path="/" element={<TaskList />} />
                        <Route path="/create" element={<TaskForm />} />
                        <Route path="/edit/:id" element={<TaskForm />} />
                    </Routes>
                </div>
            </div>
        </div>
    );
};

export default App;