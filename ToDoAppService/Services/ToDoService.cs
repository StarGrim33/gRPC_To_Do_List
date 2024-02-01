using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoAppService.Data;
using ToDoAppService.Models;

namespace ToDoAppService
{
    public class ToDoService : ToDo.ToDoBase
    {
        private readonly AppDbContext _dbContext;

        public ToDoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateTodoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext callContext)
        {
            if (request.Title?.Length == 0 || request.Description?.Length == 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

            var toDoItem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description,
            };

            await _dbContext.AddAsync(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateTodoResponse
            {
                Id = toDoItem.Id,
            });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resourse index must be greater than 0"));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(c => c.Id == request.Id);

            if (toDoItem != null)
            {
                return await Task.FromResult(new ReadToDoResponse
                {
                    Id = toDoItem.Id,
                    Title = toDoItem.Title,
                    Description = toDoItem.Description,
                    ToDoStatus = toDoItem.ToDoStatus,
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var toDoItems = await _dbContext.ToDoItems.ToListAsync();

            foreach(var toDoItem in toDoItems )
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = toDoItem.Id,
                    Title = toDoItem.Title,
                    Description = toDoItem.Description,
                    ToDoStatus = toDoItem.ToDoStatus,
                });
            }

            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || request.Title.Length == 0 || request.Description.Length == 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(c => c.Id == request.Id);

            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));

            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.ToDoStatus = request.ToDoStatus;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = toDoItem.Id,
            });
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resourse index must be greater than 0"));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(c => c.Id == request.Id);

            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));

            _dbContext.Remove(toDoItem);

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = request.Id,
            });
        }
    }
}
