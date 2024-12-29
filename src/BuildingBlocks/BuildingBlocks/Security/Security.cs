using System.Text.RegularExpressions;

namespace BuildingBlocks.Security;
public class Security
{
    public static string GetConnectionString(string ConnectionString)
    {
      //  var ConnectionString = "Server=localhost;Port=5432;Database=CatalogDb;User Id=postgres;Password=postgres;Include Error Detail=true";// ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

        // Expressão regular para capturar a senha após PASSWORD= até o ponto e vírgula
        var regex = new Regex(@"PASSWORD\s*=\s*(?<senha>[^;]+);", RegexOptions.IgnoreCase);

        var match = regex.Match(ConnectionString);

        if (!match.Success)
        {
            // PASSWORD= não encontrado, retorna a string original
            return ConnectionString;
        }

        // Extrai a senha capturada pelo grupo 'senha', incluindo todos os caracteres especiais
        var passRet = match.Groups["senha"].Value;
        var passVerificad = passRet;

        try
        {
            // Tenta remover as aspas simples temporariamente, caso estejam presentes
            var passCleaner = passRet.Trim('\'');
            string   passDecifer;
            // Verifica se a senha está cifrada (Base64)
            if (AESCipher.IsBase64String(passCleaner))
            {
                var fromBase64String = Convert.FromBase64String(passCleaner);
                  passDecifer = AESCipher.Decipher(Convert.ToBase64String(fromBase64String));
                Console.WriteLine($"Senha Decifrada: {passDecifer}");
            }
            else
            {
                passDecifer = AESCipher.Cipher(passCleaner);
                Console.WriteLine($"Senha Cifrada (Base64): {passDecifer}");
            }
        

            // Substitui a senha cifrada pela senha decifrada
            ConnectionString = ConnectionString.Replace(passRet, passDecifer);
            passVerificad = passDecifer;  // A senha agora é a versão decifrada
        }
        catch (FormatException)
        {
            // Caso a senha não esteja cifrada ou ocorra erro na decodificação, mantém a senha original
        }

        // Agora escapamos os caracteres especiais na string de saída, seja cifrada ou decifrada
        var passEscaped = EscapeSpecialCharacters(passVerificad);
        ConnectionString = ConnectionString.Replace(passVerificad, passEscaped);

        return ConnectionString;
    }

    // Função para escapar caracteres especiais apenas na saída
    private static string EscapeSpecialCharacters(string password)
    {
        // Substitui aspas simples por aspas duplas (para Oracle)
        password = password.Replace("'", "''");

        // Escapa a barra invertida (caso necessário)
        password = password.Replace(@"\", @"\\");

        return password;
    }


}

