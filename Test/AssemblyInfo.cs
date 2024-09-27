using Xunit;

// Desabilitar shadow copying e paralelismo nos testes
[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]
