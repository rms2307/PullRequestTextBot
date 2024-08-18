using PullRequestTextBot.Dtos;
using System;
using System.Text;
using System.Text.Json;

namespace PullRequestTextBot
{
    public class IAService
    {
        public async Task<string> GenerateTextPullRequest(string diffs)
        {
            StringBuilder promptStringBuilder = GeneratePrompt(diffs);

            var body = new
            {
                model = "llama3.1",
                prompt = promptStringBuilder.ToString(),
            };

            string jsonBody = JsonSerializer.Serialize(body);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");

            string apiUrl = "http://localhost:11434/api/generate";
            HttpRequestMessage request = new(HttpMethod.Post, apiUrl)
            {
                Content = content
            };

            using (HttpClient client = new())
            {
                client.Timeout = TimeSpan.FromMinutes(10);

                using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using Stream stream = await response.Content.ReadAsStreamAsync();
                using StreamReader reader = new(stream);
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    IAResponse? jsonObject = JsonSerializer.Deserialize<IAResponse>(line);
                    Console.Write($"{jsonObject.Response}");
                }
            }

            return promptStringBuilder.ToString();
        }

        private static StringBuilder GeneratePrompt(string diffs)
        {
            StringBuilder promptStringBuilder = new();
            promptStringBuilder.AppendLine("A partir das mudanças informadas abaixo (output do comando diff do Git) escrever um texto explicando essas mudanças, seguindo as instruções abaixo.");
            promptStringBuilder.AppendLine("Instruções:");
            promptStringBuilder.AppendLine("- Destacar as mudanças de cada arquivo, em forma de lista.");
            promptStringBuilder.AppendLine("- Usar apenas o nome do arquivo, e não o caminho completo do arquivo.");
            promptStringBuilder.AppendLine("- Usar hifens (-) como marcadores da listagem de arquivos alterados.");
            promptStringBuilder.AppendLine("- A lista de alterações deve seguir o modelo abaixo:");
            promptStringBuilder.AppendLine("  Modelo:");
            promptStringBuilder.AppendLine("  NomeDoArquivoAlterado");
            promptStringBuilder.AppendLine("  - Alteração um");
            promptStringBuilder.AppendLine("  - Alteração dois");
            promptStringBuilder.AppendLine("  - Alteração três");
            promptStringBuilder.AppendLine("- Não escreva nada a mais além das mudanças de cada arquivo.");
            promptStringBuilder.AppendLine("- GERAR TUDO EM CÓDIGO MARKDOWN PARA SER COPIADO E COLADO NO GITHUB/GITLAB");

            promptStringBuilder.AppendLine("DIFFS:");
            promptStringBuilder.AppendLine(diffs);
            return promptStringBuilder;
        }
    }
}
