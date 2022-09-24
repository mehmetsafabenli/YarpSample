using Color.Abstraction.Service;
using Color.Domain.Tables;

namespace Color.Service.Service;

public class ColorService : IColorService
{
    public async Task<ColorEntity> GenerateRandomColor()
    {
        var random = new Random();
        var color = new ColorEntity
        {
            Red = random.Next(0, 255),
            Green = random.Next(0, 255),
            Blue = random.Next(0, 255),
            Id = Guid.NewGuid()
        };

        return color;
    }

    public async Task<IEnumerable<ColorEntity>> GenerateRandomColors(int count = 3)
    {
        var colors = new List<ColorEntity>();
        for (var i = 0; i < count; i++)
        {
            colors.Add(await GenerateRandomColor());
        }

        return colors;
    }
}