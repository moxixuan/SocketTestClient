// See https://aka.ms/new-console-template for more information
// 目标，1000毫秒钟传278个数据块

using System.Net;
using System.Net.Sockets;
using System.Text;


Task[] tasks = new Task[10];
for (int i = 0; i < tasks.Length; i++)
{
    var port = 6660 + i;
    Console.WriteLine("端口：" + port);
    tasks[i] = Task.Run(async () => await SendDataAsync(port));
}

await Task.WhenAll(tasks);

foreach (var task in tasks)
{
    Console.WriteLine($"Task status: {task.Status}");
}

Console.WriteLine("代码执行完毕，按任意键退出");
Console.ReadLine();


async Task SendDataAsync(int port)
{
    TcpClient? tcpClient = null;
    try
    {
        
// 指定数组大小为24MB
        int size = 24 * 1024 * 1024;
// 创建byte[]数组
        byte[] data = new byte[size];
// 填充数组
        for (int i = 0; i < data.Length; i++)
        {
            // 这里我们简单地将每个字节设置为i % 256，
            // 但这可以替换为任何其他填充逻辑。
            data[i] = (byte)(i % 256);
        }
        
// 记录开始时间
        DateTime startTime = DateTime.Now;
        
        var ipAddress = IPAddress.Parse("127.0.0.1"); 
        tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(ipAddress, port);
        var stream = tcpClient.GetStream();


//整块data传输
        for (int i = 0; i < 10; i++)
        {
            stream.Write(data);
        }

//data本次分成1024传输
//本机测试49716.613毫秒\50194.957毫秒\49305.161毫秒

// for (int i = 0; i < 100; i++)
// {
//     foreach (var block in IterateData(data, 1024))
//     {
//         stream.Write(block);
//     }
// }

// 记录结束时间
        DateTime endTime = DateTime.Now;

// 计算执行时间
        TimeSpan elapsedTime = endTime - startTime;
        Console.WriteLine("代码执行时间为：" + elapsedTime.TotalMilliseconds + "毫秒");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    finally
    {
        tcpClient?.Close();
        tcpClient?.Dispose();
    }

    static IEnumerable<byte[]> IterateData(byte[] data, int blockSize)
    {
        for (int i = 0; i < data.Length; i += blockSize)
        {
            int length = Math.Min(blockSize, data.Length - i);
            yield return data.Skip(i).Take(length).ToArray();
        }
    }
}


/*
var ipAddress = IPAddress.Parse("127.0.0.1");
// var ipEndPoint = new IPEndPoint(ipAddress, 6663);
TcpClient tcpClient = new TcpClient();
await tcpClient.ConnectAsync(ipAddress, 6663);
var stream = tcpClient.GetStream();


// 指定数组大小为24MB
int size = 24 * 1024 * 1024;
// 创建byte[]数组
byte[] data = new byte[size];
// 填充数组
for (int i = 0; i < data.Length; i++)
{
    // 这里我们简单地将每个字节设置为i % 256，
    // 但这可以替换为任何其他填充逻辑。
    data[i] = (byte)(i % 256);
}

// 目标，1000毫秒钟传278个数据块
// 记录开始时间
DateTime startTime = DateTime.Now;

//整块data传输
//本机测试49785.688毫秒\43200.909毫秒
for (int i = 0; i < 100; i++)
{
    stream.Write(data);
}

//data本次分成1024传输
//本机测试10389.492毫秒

// for (int i = 0; i < 100; i++)
// {
//     foreach (var block in IterateData(data, 1024))
//     {
//         stream.Write(block);
//     }
// }

// 记录结束时间
DateTime endTime = DateTime.Now;

// 计算执行时间
TimeSpan elapsedTime = endTime - startTime;
Console.WriteLine("代码执行时间为：" + elapsedTime.TotalMilliseconds + "毫秒");

tcpClient.Close();
tcpClient.Dispose();

static IEnumerable<byte[]> IterateData(byte[] data, int blockSize)
{
    for (int i = 0; i < data.Length; i += blockSize)
    {
        int length = Math.Min(blockSize, data.Length - i);
        yield return data.Skip(i).Take(length).ToArray();
    }
}
*/


//
// using (CancellationTokenSource source = new CancellationTokenSource())
// {
//     var ipEndPoint = new IPEndPoint(IPAddress.Any, 6663);
//     TcpListener Listener = new(ipEndPoint);
//     // long len = 24L * 1024L * 1024L;
//     long totalReadBytes = 0;
//     // Start the TcpListener
//     // TcpListener Listener = new TcpListener(IPAddress.Any, this.ExternalPort);
//     Listener.Start();
//
//     var Token = source.Token;
//
//     // Continually wait for new client
//     while (!Token.IsCancellationRequested)
//     {
//         // Handle the client asynchronously in a new thread
//         TcpClient client = await Listener.AcceptTcpClientAsync();
//         _ = Task.Run(async () =>
//         {
//             client.ReceiveTimeout = client.SendTimeout = 0;
//             NetworkStream stream = client.GetStream();
//             stream.ReadTimeout = stream.WriteTimeout = Timeout.Infinite;
//             while (!Token.IsCancellationRequested)
//             {
//                 var buffer = new byte[1_024];
//                 int received = await stream.ReadAsync(buffer);
//                 totalReadBytes += received;
//             }
//         });
//     }
// }

// var ipEndPoint = new IPEndPoint(IPAddress.Any, 6663);
// TcpListener listener = new(ipEndPoint);

// try
// {    
//     listener.Start();
//     
//     using TcpClient handler = await listener.AcceptTcpClientAsync();
//     await using NetworkStream stream = handler.GetStream();
//
//     var message = $"📅 {DateTime.Now} 🕛";
//     var dateTimeBytes = Encoding.UTF8.GetBytes(message);
//     await stream.WriteAsync(dateTimeBytes);
//
//     Console.WriteLine($"Sent message: \"{message}\"");
//     // Sample output:
//     //     Sent message: "📅 8/22/2022 9:07:17 AM 🕛"
// }
// finally
// {
//     listener.Stop();
// }

// IPAddress ipAddress = new IPAddress(new byte[] { 0, 0, 0, 0 });
// IPEndPoint ipEndPoint = new(ipAddress, 6668);
//
// using Socket client = new(
//     ipEndPoint.AddressFamily, 
//     SocketType.Stream, 
//     ProtocolType.Tcp);
//
// await client.ConnectAsync(ipEndPoint);
// while (true)
// {
//     // Send message.
//     var message = "Hi friends 👋!<|EOM|>";
//     var messageBytes = Encoding.UTF8.GetBytes(message);
//     _ = await client.SendAsync(messageBytes, SocketFlags.None);
//     Console.WriteLine($"Socket client sent message: \"{message}\"");
//
//     // client.BeginReceive()
//     
//     // Receive ack.
//     var buffer = new byte[1_024];
//     var received = await client.ReceiveAsync(buffer, SocketFlags.None);
//     var response = Encoding.UTF8.GetString(buffer, 0, received);
//     if (response == "<|ACK|>")
//     {
//         Console.WriteLine(
//             $"Socket client received acknowledgment: \"{response}\"");
//         break;
//     }
//     // Sample output:
//     //     Socket client sent message: "Hi friends 👋!<|EOM|>"
//     //     Socket client received acknowledgment: "<|ACK|>"
// }
//
// client.Shutdown(SocketShutdown.Both);