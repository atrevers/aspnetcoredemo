using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public void AddStop(string tripName, Stop newStop, string username)
        {
            var trip = GetUserTripByName(tripName, username);

            if (trip != null) {
                trip.Stops.Add(newStop);
                _context.Stops.Add(newStop);
            }
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting All Trips from the Database");

            return _context.Trips.ToList();
        }
        
        public IEnumerable<Trip> GetTripsByUsername(string name)
        {
            return _context.Trips
                .Where(t => t.UserName == name)
                .OrderBy(t => t.Name)
                .ToList();
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .Where(t => t.Name == tripName)
                .FirstOrDefault();
        }

        public Trip GetUserTripByName(string tripName, string username)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .Where(t => t.Name == tripName && t.UserName == username)
                .FirstOrDefault();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

    }
}
