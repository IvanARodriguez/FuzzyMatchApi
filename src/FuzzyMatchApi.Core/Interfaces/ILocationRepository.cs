using FuzzySearchApi.Core.Models;

namespace FuzzySearchApi.Core.Interfaces;

public interface ILocationRepository
{
  Task<IEnumerable<LocationRecord>> GetAllAddressesAsync();
  Task<int> GetAddressCountAsync();
  Task LoadAddressDataAsync();
}

