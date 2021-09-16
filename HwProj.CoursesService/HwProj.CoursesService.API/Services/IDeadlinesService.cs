using System.Threading.Tasks;
using HwProj.CoursesService.API.Models;

namespace HwProj.CoursesService.API.Services
{
    public interface IDeadlinesService
    {
        Task<long> AddDeadlineAsync(long taskId, Deadline deadline);
        Task DeleteDeadline(long deadlineId);
        Task<Deadline[]> GetAllDeadlinesAsync();
    }
}
