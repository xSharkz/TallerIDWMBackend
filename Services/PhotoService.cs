using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TallerIDWMBackend.Helpers;
using TallerIDWMBackend.Interfaces;
using Microsoft.AspNetCore.Http;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="PhotoService"/> y configura el cliente de Cloudinary.
    /// </summary>
    /// <param name="config">Configuración de Cloudinary, que incluye el nombre de la nube, la clave de la API y el secreto de la API.</param>
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var account = new Account
        (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }
    /// <summary>
    /// Agrega una foto a Cloudinary.
    /// </summary>
    /// <param name="file">El archivo de imagen que se va a cargar.</param>
    /// <returns>El resultado de la carga de la imagen, que incluye información sobre el archivo subido.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si el archivo no es de tipo .jpg o .png.</exception>
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file?.Length > 0)
        {
            // Validar el tipo de archivo
            var allowedTypes = new List<string> { "image/jpeg", "image/png" };
            if (!allowedTypes.Contains(file.ContentType))
            {
                throw new InvalidOperationException("Solo se permiten archivos .jpg y .png.");
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                    .Height(500)
                    .Width(500)
                    .Crop("fill")
                    .Gravity("face"),
                Folder = "Test"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }
    /// <summary>
    /// Elimina una foto de Cloudinary utilizando su ID público.
    /// </summary>
    /// <param name="publicId">El ID público de la imagen en Cloudinary que se desea eliminar.</param>
    /// <returns>El resultado de la operación de eliminación, que indica si fue exitosa o fallida.</returns>
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return new DeletionResult { Result = "failure" };

        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}
