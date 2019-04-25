using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientDadu : MonoBehaviour
{
    [SerializeField] int player;
    [SerializeField] InputField inputField;
    Channel channel;
    int[] hasil;
    int[] winCon;
    [SerializeField] Text[] text;
    
    public interface IMyFirstService : IService<IMyFirstService>
    {
        // Return type must be `UnaryResult<T>` or `Task<UnaryResult<T>>`.
        // If you can use C# 7.0 or newer, recommend to use `UnaryResult<T>`.
        UnaryResult<int> SumAsync(int x, int y);
        UnaryResult<int[]> RandomDadu(int player);
        UnaryResult<int[]> WinCon();
    }

    // Start is called before the first frame update
    void Start()
    {
        inputField = FindObjectOfType<InputField>();
    }

    public async void RandomDaduAsync(int Player)
    {
        // standard gRPC channel
        string a = inputField.text;
        channel = new Channel(a, 12345, ChannelCredentials.Insecure);

        player = Player;
        // get MagicOnion dynamic client proxy
        var client = MagicOnionClient.Create<IMyFirstService>(channel);

        // call method.
        var result = await client.SumAsync(100, 200);
        //Console.WriteLine("Client Received:" + result);

        hasil = await client.RandomDadu(player);
        text[0].text = "Player = " + hasil[0] + "\n" + "Hasil dadu : " + hasil[1];
    }

    public async void GetHighest()
    {
        var client = MagicOnionClient.Create<IMyFirstService>(channel);

        winCon = await client.WinCon();
        text[1].text = "Menang Player = " + winCon[0] + "\n" + "Hasil dadu : " + winCon[1];
    }
}
