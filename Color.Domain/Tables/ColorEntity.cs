using Color.Domain.Base;

namespace Color.Domain.Tables;

public class ColorEntity : BaseEntity, IEntity
{
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    
}