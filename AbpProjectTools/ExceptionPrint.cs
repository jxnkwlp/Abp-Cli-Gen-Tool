using Pastel;
using System;
using System.Drawing;

namespace AbpProjectTools;

public static class ExceptionPrint
{
    public static void Print(this Exception exception)
    {
        Console.WriteLine();
        Console.WriteLine(exception.Message.Pastel(Color.IndianRed));
        Console.WriteLine();

#if DEBUG
        Console.WriteLine(exception.ToString().Pastel(Color.LightSlateGray));
        Console.WriteLine();
#endif

        if (exception.InnerException != null)
        {
            Print(exception.InnerException);
        }
    }
}
