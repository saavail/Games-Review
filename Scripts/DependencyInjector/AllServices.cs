using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace DependencyInjector
{
    public class AllServices
    {
        private static AllServices _instance;

        public static AllServices Container => _instance ??= new AllServices();

        private readonly HashSet<IService> _services = new();

        public TService RegisterSingle<TService>(TService implementation) 
            where TService : IService
        {
            Implementation<TService>.ServiceInstance = implementation;

            if (_services.Any(i => i.GetType() == implementation.GetType()))
                _services.Remove(implementation);
            
            _services.Add(implementation);
            return implementation;
        }

        public IEnumerable<TService> GetAll<TService>()
            where TService : IService
            => _services.SelectAndCast<IService, TService>();

        public TService Single<TService>() 
            where TService : IService
            => Implementation<TService>.ServiceInstance;

        private static class Implementation<TService> 
            where TService : IService
        {
            public static TService ServiceInstance;
        }
    }
}