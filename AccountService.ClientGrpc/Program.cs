using AccountService.Application.Grpc; // ← généré à partir de account.proto
using Grpc.Core;
using Grpc.Net.Client;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// 🔁 === MODIFIEZ ICI L'ID DU COMPTE À TESTER ===
var accountIdToTest = "a1b2c3d4-e5f6-7890-1234-567890abcdef"; // 👈 Changez cette valeur
// ==============================================

try
{
    Console.WriteLine("Donner un numéro de compte");
    string accountId = Console.ReadLine();

    accountIdToTest = accountId;

    // Créer le canal gRPC (sans TLS car en dev)
    using var channel = GrpcChannel.ForAddress("http://localhost:5041");

    // Créer le client
    var client = new AccountService.Application.Grpc.AccountService.AccountServiceClient(channel);

    // Appeler le service
    var request = new GetAccountRequest { Id = accountIdToTest };
    // var response = await client.GetAccountByIdAsync(request);
    var response = client.GetAccountById(request);

    // Afficher le résultat
    Console.WriteLine("✅ Compte trouvé :");
    Console.WriteLine($"   ID          : {response.Id}");
    Console.WriteLine($"   Customer ID : {response.CustomerId}");
    Console.WriteLine($"   Solde       : {response.Balance:C}");
    Console.WriteLine($"   Fermé ?     : {response.IsClosed}");

}
catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
{
    Console.WriteLine($"❌ Erreur : Compte '{accountIdToTest}' non trouvé.");
}
catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.InvalidArgument)
{
    Console.WriteLine($"❌ Erreur : ID de compte invalide ('{accountIdToTest}').");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Erreur inattendue : {ex.Message}");
    Console.WriteLine($"   (Vérifiez qu'AccountService est lancé sur http://localhost:5041)");
}
finally {
    Console.ReadKey();
}