using Number.Domain.Tables;

namespace Number.Abstraction.Service;

public interface INumberService
{
    Task<NumberEntity> GenerateRandomNumber();

    Task<IEnumerable<NumberEntity>> GenerateRandomNumbers(int count = 3);
}