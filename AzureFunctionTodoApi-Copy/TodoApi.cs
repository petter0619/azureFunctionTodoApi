using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionTodoApi
{
    public static class TodoApi
    {
        [Function("GetAllTodos")]
        public static HttpResponseData GetAllTodos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetAllTodos");
            logger.LogInformation("C# HTTP trigger function GetAllTodos processed a request.");

            // Parse potential query parameter "completed=[true || false]"
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            bool? completedStatus = null;
            if (queryParams.HasKeys() && (queryParams.Get("completed").ToLower() == "true" || queryParams.Get("completed").ToLower() == "false"))
            {
                completedStatus = Convert.ToBoolean(queryParams.Get("completed"));
            }

            // Get all todos
            (List<TodoModel> Response, Exception Error) query = TodoDataAccess.ReadAllTodos(completedStatus);

            // Response
            if (query.Error != null)
            {
                logger.LogError(query.Error.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonSerializer.Serialize(query.Response));
            return response;
        }

        [Function("CreateTodo")]
        public static HttpResponseData CreateTodo(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todos")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("CreateTodo");
            logger.LogInformation("C# HTTP trigger function CreateTodo processed a request.");

            // Deserialize request body
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var data = JsonSerializer.Deserialize<TodoModel>(requestBody);

            // Add new todo to database
            (int? Response, Exception Error) query = TodoDataAccess.AddTodo(data.Todo);

            // Response
            if (query.Error != null)
            {
                logger.LogError(query.Error.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return req.CreateResponse(HttpStatusCode.Created);
        }

        [Function("GetSingleTodo")]
        public static HttpResponseData GetSingleTodo(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos/{id}")] HttpRequestData req,
            string id,
            FunctionContext executionContext
        )
        {
            var logger = executionContext.GetLogger("GetSingleTodo");
            logger.LogInformation("C# HTTP trigger function GetSingleTodo processed a request.");

            // Parse id
            int todoId;
            if (!int.TryParse(id, out todoId)) return req.CreateResponse(HttpStatusCode.BadRequest);

            // Get single todo
            (TodoModel Response, Exception Error) query = TodoDataAccess.ReadSingleTodo(todoId);

            // Response
            if (query.Error != null)
            {
                logger.LogError(query.Error.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            if (query.Error == null && query.Response == null) return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonSerializer.Serialize(query.Response));
            return response;
        }

        [Function("UpdateTodo")]
        public static HttpResponseData UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "todos/{id}")] HttpRequestData req,
            string id,
            FunctionContext executionContext
        )
        {
            var logger = executionContext.GetLogger("UpdateTodo");
            logger.LogInformation("C# HTTP trigger function UpdateTodo processed a request.");

            // Deserialize request body
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var data = JsonSerializer.Deserialize<TodoModel>(requestBody);

            // Parse id
            int todoId;
            if (!int.TryParse(id, out todoId)) return req.CreateResponse(HttpStatusCode.BadRequest);

            // Update todo
            (int? Response, Exception Error) query = TodoDataAccess.UpdateSingleTodo(todoId, data.Todo, data.Completed);

            // Response
            if (query.Error != null)
            {
                logger.LogError(query.Error.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            if (query.Error == null && query.Response < 1) return req.CreateResponse(HttpStatusCode.NotFound); ;
            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        [Function("DeleteTodo")]
        public static HttpResponseData DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todos/{id}")] HttpRequestData req,
            string id,
            FunctionContext executionContext
        )
        {
            var logger = executionContext.GetLogger("DeleteTodo");
            logger.LogInformation("C# HTTP trigger function DeleteTodo processed a request.");

            // Parse id
            int todoId;
            if (!int.TryParse(id, out todoId)) return req.CreateResponse(HttpStatusCode.BadRequest);

            // Delete todo
            (int? Response, Exception Error) query = TodoDataAccess.DeleteSingleTodo(todoId);

            // Response
            if (query.Error != null)
            {
                logger.LogError(query.Error.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            if (query.Error == null && query.Response < 1) return req.CreateResponse(HttpStatusCode.NotFound);
            return req.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
