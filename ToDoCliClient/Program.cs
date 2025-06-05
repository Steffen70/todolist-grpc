using System.Security.Cryptography.X509Certificates;
using Grpc.Core;
using Grpc.Net.Client;
using ToDoGrpc;

var baseAddress = new Uri("https://localhost:8443");

// Load root CA
using var rootCa = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "cert", "root_ca.crt"));

// Create handler that trusts your custom cert
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (_, cert, chain, __) =>
{
    // Manually trust your own root CA
    chain!.ChainPolicy.ExtraStore.Add(rootCa);
    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

    var isValid = chain.Build(cert!);
    return isValid;
};

// Create gRPC channel using custom handler
var httpClient = new HttpClient(handler);
var channel = GrpcChannel.ForAddress(baseAddress, new()
{
    HttpClient = httpClient
});

// Use your client
var todoClient = new todoit.todoitClient(channel);



Console.WriteLine("Hello In ToDoList-App" +
    "\n1->To create a new list" +
    "\n2->To Add new Item to a List" +
    "\n3-To Update An Item" +
    "\n4-To Delete Item");
int Choice = int.Parse(Console.ReadLine());

switch(Choice)
{
    //Create List 1 
    case 1:
        {
            Console.WriteLine("You have to Enter List_name:");
            string list_name = Console.ReadLine();
            todoClient.CreateList(new() { ListName = list_name });
            var result = todoClient.ReadLists(new()).Lists.FirstOrDefault(x=>x.ListName ==list_name);

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("|{0,-10} | {1,-5} |", "ListId", "ListName");
            Console.WriteLine(new string('-', 20));
            Console.WriteLine("|{0,-10} | {1,-5} |", result.Id , result.ListName);
            Console.WriteLine(new string('-', 20));
            break;
        }
    case 2:
        {
            Console.WriteLine("Enter Id of List:");
            Int32 list_id = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter ItemName:");
            string item_name = Console.ReadLine();
            var result = todoClient.ReadLists(new());
            if (result.Lists.FirstOrDefault(x => x.Id == list_id) == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "can't find this one"));
                
            }
            else
            {
                todoClient.AddItemToList(new()
                {
                    ToDoListId = list_id,
                    ItemName = item_name,
                });
                Console.WriteLine("--->>>>>Item is added!<<<<<---");
                break;
            }
            
        }
    default: 
            {

            break;
            }
}
//Add Item 2 

//Read Item 3 

// Delete Item

//Update Item 


//MarkIsDone

//todoClient.CreateList(new() { ListName = "List from CLI" });


//var result = todoClient.ReadLists(new());

//foreach (var todoList in result.Lists)
//{
//    Console.WriteLine($"Found list: {todoList.ListName}");
//}

Console.ReadLine();