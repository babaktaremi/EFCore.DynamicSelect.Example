// See https://aka.ms/new-console-template for more information

using System.Linq.Expressions;
using EFCore.DynamicSelect.Example.Database;
using EFCore.DynamicSelect.Example.Database.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

var columnsToSelect = "FirstName,LastName";

await using (var db=new UserDbContext())
{
    await db.Database.MigrateAsync();
    
    var query = db.Users.Select(SelectColumns<UserEntity>(columnsToSelect)).ToQueryString(); 
    
    //query will be: SELECT [u].[FirstName], [u].[LastName] FROM [Users] AS [u]


    var result = await db.Users.Select(SelectColumns<UserEntity>(columnsToSelect)).ToListAsync();


    foreach (var r in result)
    {
        Console.WriteLine($"{r.FirstName} - {r.LastName}");
    }

    Console.WriteLine("Loaded");
}

// await SeedDbAsync();

static async Task SeedDbAsync()
{
    await using var db = new UserDbContext();

    var users = new List<UserEntity>()
    {
        new UserEntity()
        {
            FirstName = "FirstName",
            LastName = "LastName",
            UserName = "UserName",
            Id = Guid.NewGuid()
        },
        new UserEntity()
        {
            FirstName = "FirstName2",
            LastName = "LastName2",
            UserName = "UserName2",
            Id = Guid.NewGuid()
        },
        new UserEntity()
        {
            FirstName = "FirstName3",
            LastName = "LastName3",
            UserName = "UserName3",
            Id = Guid.NewGuid()
        }
    };
    
    
    db.Users.AddRange(users);

    await db.SaveChangesAsync();

}


static Expression<Func<T, T>> SelectColumns<T>(string columns)
{
    var parameter = Expression.Parameter(typeof(T), "t");
    var bindings = columns.Split(',')
        .Select(column => Expression.Bind(
            typeof(T).GetProperty(column),
            Expression.PropertyOrField(parameter, column)))
        .ToArray();

    var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
    return Expression.Lambda<Func<T, T>>(Expression.Convert(body, typeof(T)), parameter);
}