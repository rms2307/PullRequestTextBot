using Microsoft.Extensions.Configuration;
using PullRequestTextBot.Dtos;
using System.Text;
using System.Text.Json;

namespace PullRequestTextBot
{
    public class IAService
    {
        public async Task<string> GenerateTextPullRequest(string diffs, IConfigurationRoot configuration)
        {
            StringBuilder promptSystem = GeneratePromptSystem();
            StringBuilder prompt = GeneratePrompt(diffs);

            var body = new
            {
                system_instruction = new
                {
                    parts = new
                    {
                        text = promptSystem.ToString(),
                    }
                },
                contents = new
                {
                    role = "user",
                    parts = new
                    {
                        text = prompt.ToString(),
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "text/plain",
                }
            };

            string model = configuration["Model"]!;
            string apiKey = configuration["ApiKey"]!;

            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            string jsonBody = JsonSerializer.Serialize(body);

            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");

            using HttpClient client = new();

            using HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<object>(await response.Content.ReadAsStringAsync());
            }

            IAResponse? iaResponse = JsonSerializer.Deserialize<IAResponse>(await response.Content.ReadAsStringAsync());

            if (iaResponse == null)
            {
                return string.Empty;
            }

            return iaResponse.Candidates[0].Content.Parts[0].Text;
        }

        private static StringBuilder GeneratePromptSystem()
        {
            StringBuilder promptStringBuilder = new();
            promptStringBuilder.AppendLine("Você é um assistente especializado em análise de código e controle de versão. A partir de um diff gerado pela comparação entre duas branches de um repositório Git, sua tarefa é gerar um resumo detalhado e organizado das mudanças encontradas.");
            promptStringBuilder.AppendLine("Instruções:");
            promptStringBuilder.AppendLine("- Analise o diff fornecido que contém as diferenças entre dois branches.");
            promptStringBuilder.AppendLine("- Para cada arquivo mencionado no diff, gere um resumo explicando as principais mudanças realizadas.");
            promptStringBuilder.AppendLine("- O resumo deve incluir o nome do arquivo seguido de uma explicação das alterações, destacando mudanças importantes como adições, remoções, modificações de código, alterações de lógica, refatorações, melhorias de performance, e qualquer outro detalhe relevante.");
            promptStringBuilder.AppendLine("- Organize a explicação em uma lista, separando cada arquivo com seu respectivo resumo.");
            promptStringBuilder.AppendLine("- Use uma linguagem clara e concisa, adequada para desenvolvedores e pessoas técnicas que revisam código.");
            promptStringBuilder.AppendLine("Exemplo de Saída Esperada:");
            promptStringBuilder.AppendLine("- src/main/app.js: Adiciona novas funções de validação de entrada para melhorar a segurança dos dados de entrada. Refatora a lógica de processamento de eventos para reduzir a complexidade ciclomática.");
            promptStringBuilder.AppendLine("- src/components/Header.jsx: Muda a estrutura do componente para ser uma função pura. Remove o uso de estado local desnecessário e uso de hooks do React para melhorar a reutilização de código.");
            promptStringBuilder.AppendLine("GERAR TUDO EM CÓDIGO MARKDOWN PARA SER COPIADO E COLADO NO GITHUB/GITLAB");

            return promptStringBuilder;
        }

        private static StringBuilder GeneratePrompt(string diffs)
        {
            StringBuilder promptStringBuilder = new();
            promptStringBuilder.AppendLine("Gere um resumo detalhado e organizado das mudanças abaixo:");
            promptStringBuilder.AppendLine("");
            promptStringBuilder.AppendLine("DIFFS:");
            promptStringBuilder.AppendLine(diffs);

            return promptStringBuilder;
        }
    }
}