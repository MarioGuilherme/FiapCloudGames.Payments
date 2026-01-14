using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.Interfaces;
using Serilog;

namespace FiapCloudGames.Payments.Application.Services;

public class PagSeguroService : IPagSeguroService
{
    public async Task<Guid> CreateFakePaymentAsync()
    {
        Log.Information("Criando pagamento fake numa API de exemplo");
        await Task.Delay(new Random().Next(1, 5)); // Simula o envio para serviço do PagSeguro por exemplo (levando de 1s a 4s
        Log.Information("Pagamento fake criado com sucesso");
        Guid externalId = Guid.NewGuid();
        Log.Information("ExternalId gerado {externalId}", externalId);
        return externalId;
    }

    public async Task<bool> IsFraudulentAsync(CreatePaymentInputModel inputModel)
    {
        Log.Information("Verificando se a compra é fraudulenta numa API de exemplo");
        await Task.Delay(new Random().Next(1, 5)); // Simula o envio para serviço do PagSeguro por exemplo (levando de 1s a 4s
        bool userIsFraudulent = Random.Shared.Next() % 2 == 0; // Aleatoriza se o usuário é fraudulento
        bool isFraudulent = inputModel.Total > 100_000; // Se valor maior que R$ 100.000,00, é fraudulento
        Log.Information("Compra verificada. Usuário fraudulento: {userIsFraudulent}. Compra fraudulenta: {isFraudulent}", userIsFraudulent, isFraudulent);
        return userIsFraudulent && isFraudulent;
    }
}
