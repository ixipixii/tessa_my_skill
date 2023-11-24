using System.IO;
using System.Threading.Tasks;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Default.Console.GetToken
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            TextWriter output)
        {
            string tokenSignature = RuntimeHelper.GenerateSignatureString();
            output.Write(tokenSignature);

            return 0;
        }
    }
}
