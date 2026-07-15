using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using business_logic.Specifications;

namespace BLL.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private const string ApplicationName = "MyTask";
        private readonly IRepository<Assignment> _assignmentRepo;

        public GoogleCalendarService(IRepository<Assignment> repository)
        {
            _assignmentRepo = repository;
        }

        private CalendarService GetCalendarService(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);
            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public async Task CreateEventAsync(GoogleEventDto eventdto, string userId)
        {
            var task = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(eventdto.AssignmentId, userId));

            var service = GetCalendarService(eventdto.GoogleAccessToken);

            var newEvent = new Event()
            {
                Summary = task.Title,
                Description = task.Description
            };

            if (task.DueDate.TimeOfDay.TotalSeconds == 0)
            {
                string dateOnly = task.DueDate.ToString("yyyy-MM-dd");
                newEvent.End = new EventDateTime { Date = dateOnly };
                newEvent.Start = new EventDateTime { Date = dateOnly };
            }
            else
            {
                newEvent.Start = new EventDateTime { DateTimeDateTimeOffset = task.DueDate.AddHours(-1) };
                newEvent.End = new EventDateTime { DateTimeDateTimeOffset = task.DueDate };
            }

            var createdEvent = await service.Events.Insert(newEvent, "primary").ExecuteAsync();

            task.GoogleEventId = createdEvent.Id;

            _assignmentRepo.Update(task);
            await _assignmentRepo.SaveAsync();
        }

        public async Task DeleteEventAsync(GoogleEventDto eventdto, string userId)
        {
            var task = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(eventdto.AssignmentId, userId));

            if (task == null)
            {
                throw new Exception("Assignment not found/you don`t have access");
            }

            if (string.IsNullOrEmpty(task.GoogleEventId))
            {
                throw new Exception("This assignment is not attached to google calendar");
            }

            var service = GetCalendarService(eventdto.GoogleAccessToken);

            try
            {
                await service.Events.Delete("primary", task.GoogleEventId).ExecuteAsync();
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
            }

            task.GoogleEventId = null;

            _assignmentRepo.Update(task);
        }
    }
}
