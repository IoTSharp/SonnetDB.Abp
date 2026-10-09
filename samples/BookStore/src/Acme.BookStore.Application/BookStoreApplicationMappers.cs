using Acme.BookStore.Authors;
using Acme.BookStore.Books;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.BookStore;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BookToBookDtoMapper : MapperBase<Book, BookDto>
{
    [MapperIgnoreTarget(nameof(BookDto.AuthorName))]
    public override partial BookDto Map(Book source);

    [MapperIgnoreTarget(nameof(BookDto.AuthorName))]
    public override partial void Map(Book source, BookDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public partial class CreateUpdateBookDtoToBookMapper : MapperBase<CreateUpdateBookDto, Book>
{
    public override partial Book Map(CreateUpdateBookDto source);

    public override partial void Map(CreateUpdateBookDto source, Book destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuthorToAuthorDtoMapper : MapperBase<Author, AuthorDto>
{
    public override partial AuthorDto Map(Author source);
    public override partial void Map(Author source, AuthorDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuthorToAuthorLookupDto : MapperBase<Author, AuthorLookupDto>
{
    public override partial AuthorLookupDto Map(Author source);
    public override partial void Map(Author source, AuthorLookupDto destination);
}
