using ZXing.Rendering;
using ZXing;

namespace MoonCafe.Utils
{
    public static class ZXingQrCode
    {
        public static string GenerateQR(this Guid id)
        {
            string idStr = id.ToString();
            BarcodeWriterSvg writer = new BarcodeWriterSvg();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = new ZXing.QrCode.QrCodeEncodingOptions
            {
                Height = 300,
                Width = 300,
                Margin = 0
            };
            SvgRenderer.SvgImage qrCodeImage = writer.Write(idStr);
            return qrCodeImage.Content;
        }

        public static Guid GetIdFromQR(this string qrData)
        {
            Guid id = Guid.Empty;
            if (Guid.TryParse(qrData, out id))
            {
                return id;
            }
            return Guid.Empty;
        }

        public static string GenerateSvgFromQR(this string qrData)
        {
            Guid id = qrData.GetIdFromQR();
            return id.GenerateQR();
        }
    }

}
