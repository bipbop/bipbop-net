# ![BIPBOP](https://bipbop.com.br/static/images/favicon.png) bipbop.net

## Cliente BIPBOP para C#

Implementação do cliente BIPBOP para plataforma visa tornar mais fácil a integração dos nossos serviços em clientes que utilizem a tecnologia .NET. O WebService da BIPBOP é customizado para suportar:

1. Grandes arquivos de informação consumindo pequeno footprint de memória nos servidores.
2. Consultas a informação de maneira síncrona e assíncrona.
3. Captura dos retornos de dados via WebSocket e callback HTTP.


## TL-DR

```c#
var description = await Client.Description;
var rfbDatabase = description.Databases.First(database =>
  string.Compare(database.Name, "RFB", StringComparison.OrdinalIgnoreCase) == 0);
var rfbTable = rfbDatabase.Tables.First(table =>
string.Compare(table.Name, "CERTIDAO", StringComparison.OrdinalIgnoreCase) == 0);
Assert.IsNotNull(rfbTable);
Assert.IsNotNull(rfbDatabase);
var query = await Client.Request("SELECT FROM 'RFB'.'CERTIDAO'", new[]
{
  new KeyValuePair<string, string>("DOCUMENTO", "375.543.118-16"),
  new KeyValuePair<string, string>("NASCIMENTO", "08/06/1990"),
});
Console.WriteLine(query.Document.SelectSingleNode("/BPQL/body//nome").InnerText);
```

### Consultas Juristek

Recomendamos a criação de PUSHs para ambientes de produção para consumo assíncrono.

#### Consulta Direta

```c#
var JuristekClient = new Juristek.Client(new Client()));
var tjspTable = (await JuristekClient.Description).findTable(database: "TJSP", table: "PrimeiraInstancia");
var bipbopDocument = await JuristekClient.Request(tjspQuery);
var processos = new Processos(bipbopDocument.Document);
Assert.IsNotEmpty(processos.Retrieve);
var tjspQuery = new Query(tjspTable, new[]
{
  new KeyValuePair<string, string>("NUMERO_PROCESSO", "0016306-32.2019.8.26.0502")
});

var bipbopDocument = await JuristekClient.Request(tjspQuery); // Requisição BIPBOP
var processos = new Processos(bipbopDocument.Document); // Parser do XML
Assert.IsNotEmpty(processos.Retrieve); // Retorna múltiplos processos pela numeração
```

#### Consulta CNJ

É possível consultar através da numeração CNJ sem passar a fonte.

```c#
var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
Assert.AreEqual(cnjQuery.ToString(),
    "SELECT FROM 'CNJ'.'PROCESSO' WHERE 'NUMERO_PROCESSO' = '0016306-32.2019.8.26.0502' AND 'UPLOAD' = 'FALSE'");
```

#### Consulta OAB e CALLBACK HTTP

Feita necessáriamente com um CALLBACK HTTP.

```c#
var uniq = Guid.NewGuid().ToString("N");

var oabParameters = new OABParameters("312375")
{
    Estado = Juristek.Client.Estado.SP,
    OrigemOab = Juristek.Client.Estado.SP,
    TipoInscricao = TipoInscricao.Advogado,
    WebSocket = true,
    Marker = uniq,
    Label = uniq,
    Callback = await listener.ServerAddr,
};

var oab = await JuristekClient.OabProcesso(oabParameters);
await JuristekClient.WebSocket.Start();

listener.OnRequest += (sender, args) => {
  if (args.Exception) {
    Console.WriteLine(args.ParserException);
    return;
  }
  Console.WriteLine(new Processos(args.Document));
};
```

#### Criação de Consultas Assíncronas 

```c#
var JuristekClient = new Juristek.Client(new Client()));
var listener = new Listener();
await listener.Start();

var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
var pushConfiguration = await JuristekClient.Push.Create(Juristek.Client.CreatePushConfiguration(cnjQuery,
    new PushConfiguration
    {
        MaxVersion = 1,
        Expire = DateTime.Now.AddMinutes(30),
        Callback = await listener.ServerAddr
    }));

listener.OnRequest += (sender, args) =>
{
    if (args.Push.Id != pushConfiguration.Id) return;
    listener.Stop();
};

await listener.HandleConnectionsAsync();
listener.StopSync();
```

## Variáveis de Ambiente

São opcionais os envios através das variáveis de ambiente, os argumentos podem ser enviados no tempo de execução de sua aplicação.

- **BIPBOP_APIKEY** - Chave de API fornecida ao cliente.
- **BIPBOP_ENDPOINT** - Endereço da API fornecida pela BIPBOP, opcional.
- **BIPBOP_WEBSOCKET** - Chave de API fornecida ao cliente, opcional, padrão wss://irql.bipbop.com.br/.
- **BIPBOP_PROXY** - Endereço PROXY HTTP por onde a API será consultada, opcional.
- **BIPBOP_SERVER** - URL para ser usada de CALLBACK, exemplo: http://meuservico.com/retorno.
- **BIPBOP_SERVER_PORT** - Porta onde a aplicação escutará por retornos da BIPBOP.
