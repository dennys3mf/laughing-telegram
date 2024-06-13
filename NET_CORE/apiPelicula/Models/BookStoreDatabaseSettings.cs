namespace BookStoreApi.Models;

public class BookStoreDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;
    public string GenerosCollectionName { get; set; } = null!;

    public string PeliculasCollectionName { get; set; } = null!;

    public string MovieRatingCollectionName { get; set; } = null!;

}