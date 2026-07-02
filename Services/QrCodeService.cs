using QRCoder;

namespace BuysimTechnology.Services
{
    public class QrCodeService
    {
        // Generates a QR code from any string and returns it as a Base64 PNG
        public string GenerateQrCodeBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var pngBytes = qrCode.GetGraphic(10);
            return Convert.ToBase64String(pngBytes);
        }
    }
}
