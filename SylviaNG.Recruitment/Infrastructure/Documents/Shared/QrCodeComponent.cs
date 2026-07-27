using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>
    /// EP-18 F1: renders a QR code encoding qrPayload (e.g. a verification URL or reference
    /// string), positioned per CompanyBranding.QrCodePosition. Uses QRCoder to generate the
    /// bitmap - QuestPDF itself only renders images, it doesn't generate QR codes.
    /// </summary>
    public class QrCodeComponent : IComponent
    {
        private readonly CompanyBranding _branding;
        private readonly string _qrPayload;
        private readonly int _sizePoints;

        public QrCodeComponent(CompanyBranding branding, string qrPayload, int sizePoints = 60)
        {
            _branding = branding;
            _qrPayload = qrPayload;
            _sizePoints = sizePoints;
        }

        public void Compose(IContainer container)
        {
            if (_branding.QrCodePosition == DocumentElementPositionEnum.None || string.IsNullOrWhiteSpace(_qrPayload))
                return;

            var qrBytes = GenerateQrPng(_qrPayload);

            DocumentPositioning.Apply(container, _branding.QrCodePosition)
                .Height(_sizePoints).Width(_sizePoints)
                .Image(qrBytes).FitArea();
        }

        private static byte[] GenerateQrPng(string payload)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(data);
            return pngQrCode.GetGraphic(10);
        }
    }
}
