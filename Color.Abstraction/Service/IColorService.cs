using Color.Domain.Tables;

namespace Color.Abstraction.Service;

public interface IColorService
{
    Task<ColorEntity> GenerateRandomColor();
    Task<IEnumerable<ColorEntity>> GenerateRandomColors(int count = 3);
}