import React, { useState, useEffect } from 'react';
import { api } from '../Services/api.js';
import TaskItem from './TaskItem';

const TaskList = () => {
    const [tasks, setTasks] = useState([]);

    useEffect(() => {
        fetchTasks();
    }, []);

    const fetchTasks = async () => {
        try {
            const fetchedTasks = await api.getAllTasks();
            setTasks(fetchedTasks);
        } catch (error) {
            console.error('Error fetching tasks:', error);
        }
    };

    const deleteTask = async (id) => {
        try {
            await api.deleteTask(id);
            fetchTasks();
        } catch (error) {
            console.error('Error deleting task:', error);
        }
    };


    function getStatusString(status) {
        const statusMap = {
            0: 'Pending',
            1: 'In Progress',
            2: 'Completed',
            3: 'Overdue'
        };
        return statusMap[status] || 'Unknown';
    }

    function getStatusColor(status) {
        const colorMap = {
            0: 'bg-yellow-200 text-yellow-800',
            1: 'bg-blue-200 text-blue-800',
            2: 'bg-green-200 text-green-800',
            3: 'bg-red-200 text-red-800'
        };
        return colorMap[status] || 'bg-gray-200 text-gray-800';
    }

    function formatDate(dateString) {
        const options = { year: 'numeric', month: 'long', day: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    }

    function isOverdue(dueDate) {
        return new Date(dueDate) < new Date();
    }

    return (
        <div>
            <h2 className="text-2xl font-bold mb-6 text-gray-800">Task List</h2>
            {tasks.length === 0 ? (
                <p className="text-gray-600">No tasks found. Create a new task to get started!</p>
            ) : (
                <ul className="space-y-4">
                    {tasks.map((task) => (
                        <TaskItem
                            key={task.id}
                            task={task}
                            onDelete={deleteTask}
                            getStatusString={getStatusString}
                            getStatusColor={getStatusColor}
                            formatDate={formatDate}
                            isOverdue={isOverdue}
                        />
                    ))}
                </ul>
            )}
        </div>
    );
};

export default TaskList;