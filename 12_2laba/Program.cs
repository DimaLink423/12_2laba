using System;
using System.Numerics;

class Program
{
    static void Main()
    {
        int p = 23; // модуль
        int a = 1;
        int b = 1;
        int[] G = { 17, 20 }; // базова точка
        int privateKey = 6; // приватний ключ

        // Генеруємо випадковий публічний ключ
        int[] publicKey = MultiplyPoint(G, privateKey, a, p);

        Console.WriteLine("Public key (x, y): (" + publicKey[0] + ", " + publicKey[1] + ")");

        // Повідомлення, яке потрібно зашифрувати
        int message = 8;

        // Шифрування повідомлення
        Tuple<int[], int[]> encryptedMessage = EncryptMessage(G, publicKey, message, p, a, b);

        Console.WriteLine("Encrypted message:");
        Console.WriteLine("Ciphertext (C1, C2): (" + encryptedMessage.Item1[0] + ", " + encryptedMessage.Item1[1] + "), (" + encryptedMessage.Item2[0] + ", " + encryptedMessage.Item2[1] + ")");
    }

    static Tuple<int[], int[]> EncryptMessage(int[] G, int[] publicKey, int message, int p, int a, int b)
    {
        // Генеруємо випадковий криптографічно стійкий k
        Random rand = new Random();
        int k = rand.Next(1, p - 1);

        // Обчислюємо C1 = k * G
        int[] C1 = MultiplyPoint(G, k, a, p);

        // Обчислюємо C2 = k * PublicKey + message * PublicKey
        int[] kG = MultiplyPoint(publicKey, k, a, p);
        int[] messageTimesPublicKey = MultiplyPoint(publicKey, message, a, p);

        int[] C2 = AddPoints(kG, messageTimesPublicKey, p);

        return new Tuple<int[], int[]>(C1, C2);
    }

    static int[] MultiplyPoint(int[] point, int k, int a, int p)
    {
        int[] result = new int[2];
        int[] current = point;

        for (int i = 1; i < k; i++)
        {
            current = AddPoints(current, point, p);
        }

        return current;
    }

    static int[] AddPoints(int[] P, int[] Q, int p)
    {
        int[] result = new int[2];
        int lambda;
        int a = 0;

        if (P[0] == Q[0] && P[1] == Q[1])
        {
            lambda = ((3 * P[0] * P[0] + 2 * a * P[0] + 1) * ModInverse(2 * P[1], p)) % p;
        }
        else
        {
            lambda = ((Q[1] - P[1]) * ModInverse(Q[0] - P[0] + p, p)) % p;
        }

        result[0] = (lambda * lambda - P[0] - Q[0]) % p;
        result[1] = (lambda * (P[0] - result[0]) - P[1]) % p;

        if (result[0] < 0)
        {
            result[0] += p;
        }

        if (result[1] < 0)
        {
            result[1] += p;
        }

        return result;
    }

    static int ModInverse(int a, int m)
    {
        a = a % m;

        for (int x = 1; x < m; x++)
        {
            if ((a * x) % m == 1)
            {
                return x;
            }
        }

        return 1;
    }
}