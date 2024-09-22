import React from 'react';
import { Link } from 'react-router-dom';

const TaskItem = ({ task, onDelete, getStatusString, getStatusColor, formatDate, isOverdue }) => {
    return (
        <li className={`bg-white shadow-md rounded-lg p-6 hover:shadow-lg transition duration-300 ${isOverdue(task.dueDate) && task.status !== 2 ? 'border-l-4 border-red-500' : ''}`}>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">{task.title}</h3>
            <p className="text-gray-600 mb-3">{task.description}</p>
            <div className="flex justify-between items-center">
                <div>
                    <p className={`text-sm ${isOverdue(task.dueDate) && task.status !== 2 ? 'text-red-600 font-semibold' : 'text-gray-500'}`}>
                        Due: {formatDate(task.dueDate)}
                        {isOverdue(task.dueDate) && task.status !== 2 && ' (Overdue)'}
                    </p>
                    <span className={`inline-block px-2 py-1 text-xs font-semibold rounded-full mt-2 ${getStatusColor(task.status)}`}>
                        {getStatusString(task.status)}
                    </span>
                </div>
                <div className="space-x-2">
                    <Link to={`/edit/${task.id}`} className="text-indigo-600 hover:text-indigo-800 font-medium">Edit</Link>
                    <button onClick={() => onDelete(task.id)} className="text-red-600 hover:text-red-800 font-medium">Delete</button>
                </div>
            </div>
        </li>
    );
};

export default TaskItem;