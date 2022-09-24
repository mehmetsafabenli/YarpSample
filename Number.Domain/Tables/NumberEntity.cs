using Number.Domain.Base;

namespace Number.Domain.Tables;

public class NumberEntity : BaseEntity, IEntity
{
    public int Number { get; set; }
}