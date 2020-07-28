
# Welcome to Easy Socket
Easy socket is just an abstraction over the C# socket<br>
Main advantage of this library is to use the events from the library<br>

# Content
+ [How to use](https://github.com/Touseefelahi/EasySockets#How-To-Use)
+ [MCU](https://github.com/Touseefelahi/EasySockets#MCU)
+ [TCP Packet](https://github.com/Touseefelahi/EasySockets#Simple-Tcp-packet-TxRx)
+ [UDP Packet](https://github.com/Touseefelahi/EasySockets#Simple-Udp-packet-TxRx)
+ [Fire on Sent](https://github.com/Touseefelahi/EasySockets#FireOnSent)
+ [TCP Listener](https://github.com/Touseefelahi/EasySockets#TCP-Server_Listener)
+ [TCP Client](https://github.com/Touseefelahi/EasySockets#TCP-Client)
+ [EndBytesIdentifier](https://github.com/Touseefelahi/EasySockets#Listen-till-endbyte)


## How To Use
### MCU
This library is built in concept of MCU _Main Control Unit_. MCU is like a universal socket that
can send and receive both TCP and UDP using different methods _Mcu.SendTcpAsync()_ and _Mcu.SendUdpAsync()_
### Simple Tcp packet TxRx
This piece of code will send TCP packet and waits for the reply from server. If there was
any error you can check _ReplyPacket.Error_ property
 
    using Stira.Socket.Models;
    var Mcu = new Mcu()
                  {
                      IP = "192.168.10.252",
                      PortTcp = 3030,
                      PortUdp = 2020,
                  };
    ReplyPacket rxPacket = await Mcu.SendTcpAsync(byteCommand).ConfigureAwait(false);
    if (rxPacket.IsSentAndReplyReceived)
    {
        //Here is your reply
        var reply = rxPacket.Reply;
    }

### Simple Udp packet TxRx
This piece of code will send UDP packet and waits for the reply from server. If there was
any error you can check _ReplyPacket.Error_ property
    
    using Stira.Socket.Models;
    var Mcu = new Mcu()
                  {
                      IP = "192.168.10.252",
                      PortTcp = 3030,
                      PortUdp = 2020,
                  };
    ReplyPacket rxPacket = await Mcu.SendUdpAsync(byteCommand).ConfigureAwait(false);
    if (rxPacket.IsSentAndReplyReceived)
    {
        //Here is your reply
        var reply = rxPacket.Reply;
    }

#### FireOnSent
If you want to know if packet is sent and don't want to wait for Rx/rxTimeout then use fireOnSent parameter delegate
   
    var byteCommand = new byte[]{0x01, 0x02, 0x03};
    ReplyPacket reply = await Mcu.SendTcpAsync(byteCommand, true, fireOnSent: (isSent) =>
                {
                   //This code will be called when packet is sent
                }).ConfigureAwait(false);
    if (reply.IsSentAndReplyReceived)
    {
        DevReplyIncoming?.Invoke(this, reply);
    }
### TCP Server_Listener
If you want to listen on some port (3030 in this example) on your system then use ListenerTcp Class as follows

    ListenerTcp = new ListenerTcp();
    ListenerTcp.Port = 3030; 
    ListenerTcp.DataEvent += DataIncoming;
    ListenerTcp.StartListener();

    
    /// <summary>
    /// This method will be invoked everytime when there's some incoming data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DataIncoming(object sender, IReplyPacket e)
    {
        //e.Reply will be the packet received
    }

### TCP Client
If you want to connect to the server (here 192.168.10.227 is server ip) and port number (4040 in the given example) <br>
This example will fire the _DataEvent_ whenever there's a new packet

    ITcpClient tcpClient = new EasyTcpClient
    {
        Ip = "192.168.10.227",
        Port = 4040
    };
    tcpClient.Connect();
    tcpClient.DataEvent += DataIncoming;
    
    /// <summary>
    /// This method will be invoked everytime when there's 
    /// some incoming data from the server. This example will loopback the data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DataIncoming(object sender, IReplyPacket e)
    {
        //e.Reply will be the packet received, it will loopback
        tcpClient.SendAsync(e.Reply.ToArray()).ConfigureAwait(false);
    }

### Listen till endbyte
If you want to connect to the server (here 192.168.10.227 is server ip) and port number (4040 in the given example) <br>
This example will fire the _DataEvent_ whenever it will receive the given endbytes <br>
The data will be accumulated till it receives the give endbytes (**0x0a 0x0b** in this example)

    ITcpClient tcpClient = new EasyTcpClient
    {
        Ip = "192.168.10.227",
        Port = 4040
    };
    
    var endBytesIdentifier = new byte[] { 0x0a, 0x0b };
    tcpClient.Connect(endBytesIdentifier);
    tcpClient.DataEvent += DataIncoming;

e.g. Lets assume server sent 4 packets as given bellow
1. **0x11 0x12 0x13** (Rx buffer will accumulate this and DataEvent wont trigger)
2. **0x10 0x15** (Rx buffer will accumulate this and DataEvent wont trigger)
3. **0x0a 0x45 0x78** (Rx buffer will accumulate this and DataEvent wont trigger)
4. **0x15 _0x0a 0x0b_** (Here we received the endbytes as requested so the DataEvent will be triggered)

Data will be **0x11 0x12 0x13 0x10 0x15 0x0a 0x45 0x78 0x15 _0x0a 0x0b_**
    