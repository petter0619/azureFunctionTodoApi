using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionTodoApi
{
    public static class TodoDataAccess
    {
        public static (List<TodoModel> Response, Exception Error) ReadAllTodos(bool? todoIsCompleted)
        {
            (List<TodoModel> Response, Exception Error) res = (null, null);
            using (var _db = new tododb0619Context())
            {
                try
                {
                    var allTodos = todoIsCompleted == null
                        ? _db.Todos.ToList()
                        : _db.Todos.Where(t => t.Completed == todoIsCompleted).ToList();

                    res.Response = allTodos;
                    return res;
                }
                catch (Exception ex)
                {
                    res.Error = ex;
                    return res;
                }
            }
        }

        public static (int? Response, Exception Error) AddTodo(string todoText)
        {
            (int? Response, Exception Error) res = (null, null);
            using (var _db = new tododb0619Context())
            {
                try
                {
                    _db.Todos.Add(new TodoModel()
                    {
                        Todo = todoText,
                        Completed = false
                    });
                    var affectedRows = _db.SaveChanges();
                    res.Response = affectedRows;
                    return res;
                }
                catch (Exception ex)
                {
                    res.Error = ex;
                    return res;
                }
            }
        }

        public static (TodoModel Response, Exception Error) ReadSingleTodo(int todoId)
        {
            (TodoModel Response, Exception Error) res = (null, null);
            using (var _db = new tododb0619Context())
            {
                try
                {
                    var singleTodo = _db.Todos.SingleOrDefault<TodoModel>(t => t.Id == todoId);
                    res.Response = singleTodo;
                    return res;
                }
                catch (Exception ex)
                {
                    res.Error = ex;
                    return res;
                }
            }
        }

        public static (int? Response, Exception Error) UpdateSingleTodo(int todoId, string todoText, bool isCompleted)
        {
            (int? Response, Exception Error) res = (null, null);
            using (var _db = new tododb0619Context())
            {
                try
                {
                    TodoModel todo = _db.Todos.FirstOrDefault(c => c.Id == todoId);
                    todo.Todo = todoText;
                    todo.Completed = isCompleted;
                    int affectedRows = _db.SaveChanges();
                    res.Response = affectedRows;
                    return res;
                }
                catch (Exception ex)
                {
                    res.Error = ex;
                    return res;
                }
            }
        }

        public static (int? Response, Exception Error) DeleteSingleTodo(int todoId)
        {
            (int? Response, Exception Error) res = (null, null);
            using (var _db = new tododb0619Context())
            {
                try
                {
                    TodoModel todo = _db.Todos.FirstOrDefault(c => c.Id == todoId);
                    _db.Todos.Remove(todo);
                    var affectedRows = _db.SaveChanges();
                    res.Response = affectedRows;
                    return res;
                }
                catch (Exception ex)
                {
                    res.Error = ex;
                    return res;
                }
            }
        }

    }
}
