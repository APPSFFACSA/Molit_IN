using System.Text;
using System.Security.Cryptography;

namespace Molit_IN.API.Functions
{
    public class Cifrar
    {
        public static string cifrarCadena(string cadena)
        {
            SHA256Managed sha = new SHA256Managed();
            byte[] bytecadena = Encoding.Default.GetBytes(cadena);
            byte[] bytecifrado = sha.ComputeHash(bytecadena);
            return BitConverter.ToString(bytecifrado).Replace("-", "");
        }
    }
}
