using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;

namespace PaymentWeb.Reports
{
    public class PdfFooter : PdfPageEventHelper
    {
        private readonly Font _pageNumberFont = new Font(Font.NORMAL, 9f, Font.NORMAL, BaseColor.Black);

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            AddPageNumber(writer, document);
        }

        public void AddPageNumber(PdfWriter writer, Document document)
        {
            var numberTable = new PdfPTable(1);

            string text1 = "Page No : " + writer.PageNumber.ToString(), text2 = "Report Generated Time : " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

            var pdfCell = new PdfPCell(new Phrase(text1, _pageNumberFont));
            pdfCell.MinimumHeight = 30f;
            pdfCell.BackgroundColor = BaseColor.White;
            pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfCell.Border = 0;
          //  pdfCell.BorderColorTop = new BaseColor(Color.Black);
          //  pdfCell.BorderWidthTop = .5f;
            numberTable.AddCell(pdfCell);

            pdfCell = new PdfPCell(new Phrase(text2, _pageNumberFont));
            pdfCell.BackgroundColor = BaseColor.White;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfCell.Border = 0;
            numberTable.AddCell(pdfCell);

            numberTable.TotalWidth = 500f;
            numberTable.WriteSelectedRows(0, -1, document.Left, document.Bottom + 10, writer.DirectContent);
        }

    }
}