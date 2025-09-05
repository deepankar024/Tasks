using System.Collections.Generic;
using FeedbackFormWebApp.Models;

namespace FeedbackFormWebApp.Services
{
    public interface IFeedbackRepository
    {
        void Add(Feedback feedback);

        List<Feedback> GetAllOrdered();
        List<Feedback> GetByUserId(int userId); // NEW: Get feedbacks by user
        int GetTotalCount();
        int GetTotalCountByUserId(int userId); // NEW: Count by user
        PagedResult<Feedback> GetPagedFeedback(int page, int pageSize, string sortField, string sortDirection);
        PagedResult<Feedback> GetPagedFeedbackByUserId(int userId, int page, int pageSize, string sortField, string sortDirection); // NEW
        Feedback GetById(int id);
        void Update(Feedback feedback);
        void Delete(int id);
        bool CanUserModify(int feedbackId, int userId); // NEW: Check if user can modify feedback
    }
}
