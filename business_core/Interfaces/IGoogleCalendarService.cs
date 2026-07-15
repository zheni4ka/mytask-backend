using Core.DTOs;
using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task CreateEventAsync(GoogleEventDto request, string userID);
        Task DeleteEventAsync(GoogleEventDto request, string eventId);
    }
}
