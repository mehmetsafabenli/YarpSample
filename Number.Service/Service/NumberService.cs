using Number.Abstraction.Service;
using Number.Domain.Tables;

namespace Number.Service.Service;

public class NumberService : INumberService

{
    public async Task<NumberEntity> GenerateRandomNumber()
    {
        var number = new NumberEntity
        {
            Number = new Random().Next(1, 100),
            Id = Guid.NewGuid()
        };

        return number;
    }

    public async Task<IEnumerable<NumberEntity>> GenerateRandomNumbers(int count = 3)
    {
        var numbers = new List<NumberEntity>();

        for (var i = 0; i < count; i++)
        {
            numbers.Add(await GenerateRandomNumber());
        }

        return numbers;
    }
}