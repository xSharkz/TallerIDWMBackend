using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Services
{
    public class InvoiceService
    {
        public byte[] GenerateInvoicePdf(Order order, List<CartItem> cartItems)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Título
                document.Add(new Paragraph($"Factura de la Orden #{order.Id}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));

                // Información de la Orden
                document.Add(new Paragraph($"Fecha: {order.OrderDate:yyyy-MM-dd HH:mm:ss}"));
                document.Add(new Paragraph($"Dirección de Entrega: {order.DeliveryAddress}"));
                document.Add(new Paragraph($"Total: {order.TotalAmount:C}"));

                // Detalle de Productos
                var table = new Table(5);
                table.AddHeaderCell("Producto");
                table.AddHeaderCell("Tipo");
                table.AddHeaderCell("Precio Unitario");
                table.AddHeaderCell("Cantidad");
                table.AddHeaderCell("Subtotal");

                foreach (var item in order.OrderItems)
                {
                    var product = item.Product;
                    table.AddCell(product.Name);
                    table.AddCell(product.Type);
                    table.AddCell($"{item.UnitPrice:C}");
                    table.AddCell($"{item.Quantity}");
                    table.AddCell($"{item.Quantity * item.UnitPrice:C}");
                }

                document.Add(table);
                document.Close();

                return stream.ToArray();
            }
        }
    }
}