using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Telegram.Bot.Mvc.Scheduler
{
    /// <summary>
    /// Used to throttle tasks according to thier priority and the current load.
    /// </summary>
    /// <remarks>Usefull to throttle requests to telegram servers, and avoid being banned for too many requests. </remarks>
    public interface ISchedualer : IDisposable
    {

        /// <summary>
        /// Enqueue an action and schedule it to start at a later time depending on the load and its priority.
        /// </summary>
        /// <param name="action">An action that will be wrapped in a task.</param>
        /// <param name="priority">Zero-based value specifying the pirority of the task, zero is the highest.</param>
        void Enqueue(Action action, uint priority = 0);

        /// <summary>
        /// Enqueue an array of tasks and schedule them to start at a later time depending on the load and its priority.
        /// Tasks will run <em>sequentially</em>, one after another, with the specified delay in between.
        /// This is a wrapper method around Enqueue(delay, priority, Tasks[]).
        /// </summary>
        /// <param name="delay">How much minimum delay time in milliseconds between each action.</param>
        /// <param name="priority">Zero-based value specifying the pirority of the actions, zero is the highest.</param>
        /// <param name="actions">An array of actions.</param>
        /// <remarks>Useful when sending multiple messages to the same user, without being banned from telegram for too many requests.</remarks>
        void Enqueue(int delay = 1000, uint priority = 0, params Action[] actions);

        /// <summary>
        /// Enqueue an unstarted task and schedule it to start at a later time depending on the load and its priority.
        /// </summary>
        /// <param name="task">An unstarted task.</param>
        /// <param name="priority">Zero-based value specifying the pirority of the task, zero is the highest.</param>
        void Enqueue(Task task, uint priority = 0);

        /// <summary>
        /// Enqueue an unstarted array of tasks and schedule them to start at a later time depending on the load and its priority.
        /// Tasks will run <em>sequentially</em>, one after another, with the specified delay in between.
        /// </summary>
        /// <param name="delay">How much minimum delay time in milliseconds between each task.</param>
        /// <param name="priority">Zero-based value specifying the pirority of the tasks, zero is the highest.</param>
        /// <param name="tasks">An array of unstarted tasks.</param>
        /// <remarks>Useful when sending multiple messages to the same user, without being banned from telegram for too many requests.</remarks>
        void Enqueue(int delay = 1000, uint priority = 0, params Task[] tasks);

        /// <summary>
        /// Pause the current execution of enqueued tasks gracefully.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume the current execution of enqueued tasks from where it is paused.
        /// </summary>
        void Resume();

        /// <summary>
        /// Remove all unstarted tasks that are of certain priority.
        /// </summary>
        /// <param name="priority">Zero-based value specifying the pirority of the task to be removed, zero is the highest.</param>
        void Clear(uint priority);

        /// <summary>
        /// Remove all unstarted tasks 
        /// </summary>
        void Clear();
    }
}
