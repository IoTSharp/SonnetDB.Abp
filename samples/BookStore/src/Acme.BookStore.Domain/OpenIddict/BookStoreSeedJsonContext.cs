using System.Text.Json.Serialization;

namespace Acme.BookStore.OpenIddict;

[JsonSerializable(typeof(string[]))]
internal partial class BookStoreSeedJsonContext : JsonSerializerContext;
