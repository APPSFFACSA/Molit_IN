using System;

namespace Molit_IN.Client.Services
{
    public class YourImageService : IYourImageService
    {
        public string VisualizarImagen(string nombre, byte[] imageData)
        {
            try
            {
                // Validaciones básicas
                if (imageData == null || imageData.Length == 0)
                    return null;

                // Extensión segura (sin exceptions si nombre es null/vacío)
                string ext = "";
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    var dot = nombre.LastIndexOf('.');
                    ext = dot >= 0 ? nombre.Substring(dot).ToLowerInvariant() : "";
                }

                // MIME por extensión (fallback razonable)
                var mime = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    ".bmp" => "image/bmp",
                    _ => "image/jpeg"
                };

                var base64 = Convert.ToBase64String(imageData);
                return $"data:{mime};base64,{base64}";
            }
            catch
            {
                // Nunca propagues excepción a la UI
                return null;
            }
        }
    }
}
