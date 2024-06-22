# DSoft.CacheManager
Managed .NET cache management framework for .NET Standard 2.0 and .NET 6 or .NET 8

## Features
	- Works on all platforms that support .NET Standard 2.0, .NET 6 or .NET 8
    - Extendable to add different backends for storing the cached data
		- LiteDb provider is available with more to come
	- Designed to work with Microsoft's standard Dependency Injection mechanism
		- Factory class is provided if you don't use DI
    - async/await compatible

## Usage

To use CacheManager you need to add packages `DSoft.CacheManager` and your preferred backend such as `DSoft.CacheManager.LiteDB`

### Dependency Injection

Call `RegisterCacheManager<T>`, where `T` is the backend implementation, to register the cache manager

    using DSoft.CacheManager;
    using DSoft.CacheManager.LiteDB;

    private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.RegisterCacheManager<LiteDbBackend>();
    }

### Consume

You can inject `ICacheManager` into your services or you can use `CacheManagerFactory.Create` to create an instance manually.

    var cacheManager = ServiceHost.GetRequiredService<ICacheManager>();

    var dataKey = typeof(SomeData).Name;

    await cacheManager.LoadAsync(); // optionally pre-warm the cache

    var items = new List<SomeData>()
                    {
                        new SomeData() { Id = 1, Name = "One", IsEnabled = true },
                        new SomeData() { Id = 2, Name = "Two", IsEnabled = false },
                        new SomeData() { Id = 3, Name = "Three", IsEnabled = true },
                        new SomeData() { Id = 4, Name = "Four", IsEnabled = false },
                    };

    var exists = cacheManager.IsKeyRegistered(dataKey);

    if (exists)
    {
        var ogItems = cacheManager.GetItems<SomeData>(dataKey);

        var count = ogItems.Count;
    }

    cacheManager.SetItems(dataKey, items);


    var itemsOut = cacheManager.GetItems<SomeData>(dataKey); 