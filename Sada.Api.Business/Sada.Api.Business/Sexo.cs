using Microsoft.Extensions.Logging;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business;

public class Sexo : ISexo
{
    private readonly ILogger<Sexo> _logger;
    private readonly ISexoRepository _sexoRepository;

    public Sexo(ILogger<Sexo> logger, ISexoRepository sexoRepository)
    {
        _logger = logger;
        _sexoRepository = sexoRepository;
    }

    public async Task<SexoModelResponse> CadastrarSexoAsync(SexoModelRequest model)
    {
        var sexoCadastrado = await _sexoRepository.IncluirSexoAsync(model);
        _logger.LogInformation(
            "Cadastro de sexo realizado com sucesso. ID: {IdSexo}",
            sexoCadastrado.IdSexo);

        return sexoCadastrado;
    }

    public async Task<List<SexoModelResponse>> ListarSexosAsync()
    {
        var sexos = await _sexoRepository.ListarSexosAsync();
        _logger.LogInformation(
            "Listagem de sexos realizada. Quantidade: {Quantidade}",
            sexos.Count);

        return sexos;
    }

    public async Task<SexoModelResponse> ObterSexoPorIdAsync(int idSexo)
    {
        var sexo = await _sexoRepository.ObterSexoPorIdAsync(idSexo);

        if (sexo is null)
        {
            _logger.LogWarning("Sexo com ID {IdSexo} não encontrado.", idSexo);
            throw new KeyNotFoundException($"Sexo com ID {idSexo} não encontrado.");
        }

        _logger.LogInformation("Sexo com ID {IdSexo} obtido com sucesso.", idSexo);
        return sexo;
    }

    public async Task<SexoModelResponse> AlterarSexoAsync(SexoModelRequest model)
    {
        var sexoAlterado = await _sexoRepository.AlterarSexoAsync(model);

        if (sexoAlterado is null)
        {
            _logger.LogWarning(
                "Sexo com ID {IdSexo} não encontrado para alteração.",
                model.IdSexo);
            throw new KeyNotFoundException(
                $"Sexo com ID {model.IdSexo} não encontrado para alteração.");
        }

        _logger.LogInformation(
            "Sexo com ID {IdSexo} alterado com sucesso.",
            model.IdSexo);

        return sexoAlterado;
    }

    public async Task<bool> ExcluirSexoAsync(int idSexo)
    {
        var sexoExcluido = await _sexoRepository.ApagarSexoAsync(idSexo);

        if (!sexoExcluido)
        {
            _logger.LogWarning(
                "Sexo com ID {IdSexo} não encontrado para exclusão.",
                idSexo);
            throw new KeyNotFoundException(
                $"Sexo com ID {idSexo} não encontrado para exclusão.");
        }

        _logger.LogInformation(
            "Sexo com ID {IdSexo} excluído com sucesso.",
            idSexo);

        return true;
    }
}
