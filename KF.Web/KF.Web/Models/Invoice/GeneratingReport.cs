using KF.Dto.Modules.Invoice;
using KF.Entity;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;


namespace KF.Web.Models.Invoice
{
    public class GeneratingReport
    {
        private int PageIndex;
        private IList<Stream> m_stream;

        private Stream CreateStream(string name, string FileNameExtenstion, Encoding encoding, string mimeType, bool WillSeek)
        {
            Stream stm = new FileStream(@"..\..\" + name + "." + FileNameExtenstion, FileMode.Create);
            return stm;
        }
        public string PrintRun(int UserId, int isCustomer)
        {
            try { 
            KFentities context = new KFentities();

            string Rpath = "";

            var data = context.KI_GetInvoiceDataforpdf(UserId, isCustomer).FirstOrDefault();

            if (data != null)
            {
                string logo = string.Empty;
                string imagePath = string.Empty;
                string Defaultlogo = string.Empty;



                imagePath = new Uri(HttpContext.Current.Server.MapPath("~/" + data.CompanyLogo)).AbsoluteUri;
                if (imagePath == null)
                {
                    Defaultlogo = new Uri(HttpContext.Current.Server.MapPath("~/Logos/2016-11-17--11-55-58-.png")).AbsoluteUri;
                }
                LocalReport localReport = new LocalReport();
                string path = HttpContext.Current.Server.MapPath("~/Reports/KippinInvoiceReport.rdlc");
                ReportDataSource rds = new ReportDataSource("DataSet1", context.KI_GetInvoiceDataforpdf(UserId, isCustomer));
                ReportParameter rp = new ReportParameter("logo", imagePath);
                localReport.DataSources.Clear();
                localReport.ReportPath = path;
                localReport.EnableExternalImages = true;
                localReport.SetParameters(new ReportParameter[] { rp });
                localReport.DataSources.Add(rds);
                localReport.Refresh();

                Rpath = PdfExport(localReport, UserId);
            }

            Dispose();
            return Rpath;
                }
            catch(Exception ex)
            {
                return ex.ToString();
            }

        }

        private string PdfExport(LocalReport report, int userid)
        {
            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>EMF</OutputFormat>" +
              "  <PageWidth>18.5in</PageWidth>" +
              "  <PageHeight>19.5in</PageHeight>" +
              "  <MarginTop>0.25in</MarginTop>" +
              "  <MarginLeft>0.25in</MarginLeft>" +
              "  <MarginRight>0.25in</MarginRight>" +
              "  <MarginBottom>0.25in</MarginBottom>" +
              "</DeviceInfo>";
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtention;

            byte[] bytes = report.Render(
                "PDF", deviceInfo, out mimeType, out encoding, out filenameExtention, out streamids, out warnings);
            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");

            string reportpath = HttpContext.Current.Server.MapPath("~/InvoicePdf/" + datetime + userid + ".pdf");
            using (FileStream fs = new FileStream(reportpath, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            return reportpath;
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new Metafile(m_stream[PageIndex]);
            ev.Graphics.DrawImage(pageImage, ev.PageBounds);
            PageIndex++;
            ev.HasMorePages = (PageIndex < m_stream.Count);

        }

      public void Dispose()
        {
            if (m_stream != null)
            {
                foreach (Stream strem in m_stream)
                {
                    strem.Close();
                    m_stream = null;
                }
            }
        }
    }
}