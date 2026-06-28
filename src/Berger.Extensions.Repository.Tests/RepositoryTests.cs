using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Berger.Extensions.Repository.Tests;

public sealed class RepositoryTests
{
    [Fact]
    public async Task AddAndQueryAsync_ShouldPersistEntity()
    {
        await using var db = CreateContext();
        var repository = new Repository<TestEntity>(db);

        var entity = await repository.AddAsync(new TestEntity { Id = Guid.NewGuid(), Name = "Unit" });

        var loaded = await repository.FirstOrDefaultAsync(x => x.Id == entity.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Unit", loaded!.Name);
    }

    [Fact]
    public async Task SoftDeleteAsync_ShouldSetIsDeleted_WhenPropertyExists()
    {
        await using var db = CreateContext();
        var repository = new Repository<TestEntity>(db);
        var entity = await repository.AddAsync(new TestEntity { Id = Guid.NewGuid(), Name = "Delete" });

        await repository.SoftDeleteAsync(entity);

        Assert.True(entity.IsDeleted);
    }

    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }
}

public sealed class TestDbContext(DbContextOptions<TestDbContext> options) : BaseContext<TestDbContext>(options)
{
    public DbSet<TestEntity> Entities => Set<TestEntity>();
}

public sealed class TestEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}
