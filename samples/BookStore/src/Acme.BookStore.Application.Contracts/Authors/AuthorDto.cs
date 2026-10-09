using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Authors;

public class AuthorDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string? ShortBio { get; set; }
}
