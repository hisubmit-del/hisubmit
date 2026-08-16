using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.GenerateQrCode;
using QRCoder;

namespace HiSubmit.Infrastructure.Services;

public class GenerateQrCode:IGenerateQrCode
{
    public async Task<byte[]> Generate(string data)
    {
        var qrcodeGenerator = new QRCodeGenerator();
        var qrData = qrcodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new QRCode(qrData);
        var bitmap = qrCode.GetGraphic(20);
        using var memory=new MemoryStream();
        bitmap.Save(memory, ImageFormat.Jpeg);
        var qrCodeByteArray = memory.ToArray();
        return qrCodeByteArray;
    }
}