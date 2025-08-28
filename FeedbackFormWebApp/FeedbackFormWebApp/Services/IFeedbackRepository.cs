using System.Collections.Generic;
using FeedbackFormWebApp.Models;

namespace FeedbackFormWebApp.Services
{
    public interface IFeedbackRepository
    {
        void Add(Feedback feedback);
        List<Feedback> GetAllOrdered();
        int GetTotalCount();
        PagedResult<Feedback> GetPagedFeedback(int page, int pageSize, string sortField, string sortDirection);
    }
}
