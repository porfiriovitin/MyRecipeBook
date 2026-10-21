using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Storage;

namespace MyRecipeBook.Infraestructure.Storage;

public class AzureStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ProfilePictureContainerName = "profile-picture";

    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadProfilePicture(User user, Stream file, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.Id.ToString());

        await containerClient.CreateIfNotExistsAsync();

        var blobclient = containerClient.GetBlobClient(ProfilePictureContainerName);

        await blobclient.UploadAsync(file, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        });
    }

    public async Task UploadIllustration(Recipe recipe, Stream file, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(recipe.UserId.ToString());

        await containerClient.CreateIfNotExistsAsync();

        var blobclient = containerClient.GetBlobClient(recipe.Id.ToString());

        await blobclient.UploadAsync(file, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        });
    }
}
