using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Controllers;
using CloudinaryDotNet.Actions;

namespace TallerIDWMBackend.Interfaces
{
    /// <summary>
    /// Interfaz para gestionar las operaciones de servicio relacionadas con la carga y eliminación de fotos.
    /// Utiliza la API de Cloudinary para gestionar las fotos cargadas.
    /// </summary>
    public interface IPhotoService
    {
        /// <summary>
        /// Agrega una foto a la plataforma utilizando Cloudinary.
        /// </summary>
        /// <param name="file">El archivo de la foto que se desea cargar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles de la carga de la imagen.</returns>
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        /// <summary>
        /// Elimina una foto de la plataforma utilizando Cloudinary.
        /// </summary>
        /// <param name="publicId">El identificador público de la foto que se desea eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles de la eliminación de la foto.</returns>
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
